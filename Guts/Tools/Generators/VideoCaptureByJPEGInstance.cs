// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Threading;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public class VideoCaptureByJPEGInstance : VideoGeneratorInstance
	{
        public VideoCaptureByJPEGInstance(VideoCaptureByJPEGDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
			mCamera = theDefinition.Camera;
            mFrameTimeout = theDefinition.FrameTimeout;
            mStopConditionTimeout = theDefinition.StopConditionTimeout;
            mCapturedVideo = new GeneratedVideoInstance(theDefinition.ResultantVideo, testExecution);
		}

		private NetworkCamera mCamera = null;
        [CategoryAttribute("Input")]
        public NetworkCamera Camera
		{
			get { return mCamera; }
		}

        private int mFrameTimeout = 3000;
        [CategoryAttribute("Input")]
        public int FrameTimeout
        {
            get { return mFrameTimeout; }
        }

        private int mStopConditionTimeout = 3000;
        [CategoryAttribute("Input")]
        public int StopConditionTimeout
        {
            get { return mStopConditionTimeout; }
        }

        private VideoInstance mCapturedVideo = null;
        public override VideoInstance ResultantVideo
		{
			get
			{
				return mCapturedVideo;
			}
		}

        private DateTime mCaptureStartTime;

		public override bool IsComplete() { return mCapturedVideo.IsComplete(); }

		public override void DoWork()
		{
            //if (!AreExplicitDependenciesComplete()) return;

            if (mWorkerLogMessageAvailable)
            {
                TestExecution().LogMessageWithTimeFromTrigger("Snapshot worker: " + mWorkerLogMessage);
                mWorkerLogMessageAvailable = false;
            }

            if (IsComplete()) // this is (currently) needed as a hack for the times when images are loaded via drag & drop.  The drag & drop event loads the image and sets the IsComplete, but the TestExecution doesn't remove it from the execution array until AFTER DoWork() is called...so it is called an extra time...and the loaded image gets lost
            {
                return;
            }

            if (mWorker == null)
            {
                if (true) // TODO: check "StartCondition"
                {
                    TestExecution().LogMessageWithTimeFromTrigger("Requesting video capture");

                    mCaptureStartTime = DateTime.Now;

                    mWorker = new HTTPJPegVideoGetter(this);
                    mWorker.RunWorkerAsync();
                }
            }
            else
            {
                if (!mWorker.CancellationPending)
                {
                    TimeSpan runningPeriod = DateTime.Now - mCaptureStartTime;
                    if (false || runningPeriod.TotalMilliseconds > mStopConditionTimeout)  // TODO: check for "StopCondition"
                    {
                        TestExecution().LogMessageWithTimeFromTrigger("Stopping video capture");
                        mWorker.CancelAsync();
                        mCapturedVideo.SetIsComplete();
                    }
                }
            }

			// thread pooling: http://www.c-sharpcorner.com/UploadFile/mmehta/Multithreading411162005051609AM/Multithreading4.aspx?ArticleID=a3d4a3e9-533e-49c3-9fda-5bb6a7359953
		}

        public String mWorkerLogMessage = String.Empty;
        public bool mWorkerLogMessageAvailable = false;
        private HTTPJPegVideoGetter mWorker = null;

        public void ProcessNewImage(Bitmap theFrame)
        {
            if (mWorkerLogMessageAvailable)
            {
                TestExecution().LogMessageWithTimeFromTrigger("Snapshot worker after completed: " + mWorkerLogMessage);
                mWorkerLogMessageAvailable = false;
            }

            TestExecution().LogMessageWithTimeFromTrigger("New frame aquired");

            mCapturedVideo.AddFrame(theFrame);

            // TODO: make casting safer. throw exception if not correct type...would only help if this code was cut and pasted to another type. store typed reference to definition?

            if (((VideoCaptureByJPEGDefinition)Definition()).AutoSaveEnabled)
            {
                try
                {
                    theFrame.Save(((VideoCaptureByJPEGDefinition)Definition()).AutoSavePath + Name + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + mCapturedVideo.Video.Frames.Count + ".jpg");
                    TestExecution().LogMessage("Frame saved");
                }
                catch (Exception e)
                {
                    Project().Window().logMessage("ERROR: Unable to AutoSave frame.  Ensure path valid and disk not full.  Low-level message=" + e.Message);
                    TestExecution().LogMessage("ERROR: Unable to AutoSave frame.  Ensure path valid and disk not full.");
                }
            }
            TestExecution().LogMessageWithTimeFromTrigger("Frame processing complete");
        }
	}

    //http://wp.netscape.com/assist/net_sites/pushpull.html
    public class HTTPJPegVideoGetter : BackgroundWorker
    {
        public HTTPJPegVideoGetter(VideoCaptureByJPEGInstance httpCameraUser)
        {
            mHttpCameraUser = httpCameraUser;
            WorkerReportsProgress = false; // progress doesn't make sense since we don't know how many frames we will be required to capture
            WorkerSupportsCancellation = true;
            this.RunWorkerCompleted += new RunWorkerCompletedEventHandler(HTTPImageGetter_RunWorkerCompleted);
        }

        void HTTPImageGetter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // TODO: process result codes
            mHttpCameraUser.mWorkerLogMessage = "Completed with code " + e.Result;
            mHttpCameraUser.mWorkerLogMessageAvailable = true;
        }

        public enum ResultCodes
        {
            Success = 0,
            Unspecified = -1,
            Timeout = -2,
            BufferTooSmall = -3,
            NonWebException = -9999
        }

        private VideoCaptureByJPEGInstance mHttpCameraUser = null;
        private Bitmap mImage = null;

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            mHttpCameraUser.mWorkerLogMessage = "snapshot worker kicked off"; mHttpCameraUser.mWorkerLogMessageAvailable = true;

            VideoCaptureByJPEGDefinition defObject = (VideoCaptureByJPEGDefinition)mHttpCameraUser.Definition();

            int readSize = 1024;
            int bufferSize = defObject.bufferSize;
            byte[] buffer = defObject.bufferForWorker;
            HttpWebRequest request = null;
            WebResponse response = null;
            Stream stream = null;
            Random randomNumberGenerator = new Random((int)DateTime.Now.Ticks);

            HttpRequestCachePolicy bypassCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.BypassCache);

            try
            {
                while (true) // loop "indefinitely" grabbing frames until whomever started this worker(thread) cancels it
                {
                    string uri = mHttpCameraUser.Camera.CompleteImageRequestURL();
                    if (mHttpCameraUser.Camera.ProxyCacheProtection)
                    {
                        uri += ((uri.IndexOf('?') == -1) ? '?' : '&') + "proxyprevention=" + randomNumberGenerator.Next().ToString();
                    }
                    request = (HttpWebRequest)WebRequest.Create(uri);
                    request.CachePolicy = bypassCachePolicy;
                    request.Timeout = mHttpCameraUser.FrameTimeout;

                    if (mHttpCameraUser.Camera.Login != null && mHttpCameraUser.Camera.Login.Length > 0)
                    {
                        if (mHttpCameraUser.Camera.Password == null) mHttpCameraUser.Camera.Password = String.Empty;
                        request.Credentials = new NetworkCredential(mHttpCameraUser.Camera.Login, mHttpCameraUser.Camera.Password);
                    }

                    // setting ConnectionGroupName to make sure we don't run out of connections amongst all the cameras (see http://msdn2.microsoft.com/en-us/library/ms998562.aspx section: "Connections")
                    request.ConnectionGroupName = mHttpCameraUser.TestSequence().Name + " : " + mHttpCameraUser.Name; // a unique group for each HTTP user within each TestSequence. We don't want to create unique groups every request because that would be inefficient
                    // TODO: check maxconnection attribute in Machine.config (limits the number of concurrent outbound calls)

                    ///////////
                    response = request.GetResponse();
                    stream = response.GetResponseStream();

                    long contentLength = response.ContentLength;

                    int bytesRead = 0;
                    int totalBytesRead = 0;

                    //ReportProgress(0, "Requesting image");
                    bool done = false;
                    while (!done)
                    {
                        if (CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }

                        if (totalBytesRead > bufferSize - readSize)
                        {
                            e.Result = ResultCodes.BufferTooSmall;
                            return;
                        }

                        bytesRead = stream.Read(buffer, totalBytesRead, readSize);
                        if (bytesRead == 0)
                        {
                            done = true;
                        }
                        else
                        {
                            totalBytesRead += bytesRead;
                            //ReportProgress((int)(bytesRead * 100 / contentLength), "Downloading...");
                        }
                    }

                    mImage = (Bitmap)Bitmap.FromStream(new MemoryStream(buffer, 0, totalBytesRead));
                    mHttpCameraUser.ProcessNewImage(mImage);
                }
                // we never get here since the while loop never terminates...we exit the loop on worker cancellation (above) or an exception (below)
            }
            catch (WebException exception)
            {
                switch( exception.Status )
                {
                    // http://msdn2.microsoft.com/en-us/library/system.net.webexceptionstatus(vs.80).aspx
                    case WebExceptionStatus.Timeout:
                        e.Result = ResultCodes.Timeout;
                        break;
                    default:
                        e.Result = ResultCodes.Unspecified;
                        break;
                }
            }
            catch (Exception exception)
            {
                e.Result = ResultCodes.NonWebException;
            }
            finally
            {
                if (request != null)
                {
                    request.Abort();
                    request = null;
                }
                if (stream != null)
                {
                    stream.Close();
                    stream = null;
                }
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }
        }
    }
}
