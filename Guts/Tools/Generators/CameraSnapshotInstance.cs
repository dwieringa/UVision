// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
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
	public class CameraSnapshotInstance : NetCams.ImageGeneratorInstance, ITriggerInstance
	{
		public CameraSnapshotInstance(CameraSnapshotDefinition theDefinition, TestExecution testExecution) : base(theDefinition, testExecution)
		{
            mCameraSnapshotDefintion = theDefinition;

            if (theDefinition.Camera == null) throw new ArgumentException("'" + theDefinition.Name + "' doesn't have a camera assigned");
            mCamera = theDefinition.Camera;

            if (theDefinition.Camera == null) throw new ArgumentException("'" + theDefinition.Name + "' doesn't have a value assigned to Timeout");
            mTimeout = theDefinition.Timeout;

            mSnapshotImage = new GeneratedImageInstance(theDefinition.ResultantImage, testExecution);
            mResolution = theDefinition.Resolution;

            testExecution.RegisterTrigger(this);
		}

        private NetworkCamera mCamera = null;
        [CategoryAttribute("Input")]
        public NetworkCamera Camera
        {
            get { return mCamera; }
        }

        private string mResolution;
        [CategoryAttribute("Input")]
        public string Resolution
        {
            get { return mResolution; }
        }

        private int mTimeout = 3000;
        [CategoryAttribute("Input")]
        public int Timeout
        {
            get { return mTimeout; }
        }

        private ImageInstance mSnapshotImage = null;
		public override ImageInstance ResultantImage
		{
			get
			{
				return mSnapshotImage;
			}
		}

		public override bool IsComplete() { return mSnapshotImage.IsComplete(); }

		//private bool mRequestedSnapshot = false;
		protected override void DoWork_impl()
		{
            if (mWorkerLogMessageAvailable)
            {
                TestExecution().LogMessageWithTimeFromTrigger("Snapshot worker: " + mWorkerLogMessage);
                mWorkerLogMessageAvailable = false;
            }

            if (IsComplete()) // this is (currently) needed as a hack for the times when images are loaded via drag & drop.  The drag & drop event loads the image and sets the IsComplete, but the TestExecution doesn't remove it from the execution array until AFTER DoWork() is called...so it is called an extra time...and the loaded image gets lost
            {
                return;
            }

            if (mWorker != null)
            {
                // TODO: throw an exception if it is not busy?  log something when it stops being busy? after it stops being busy this object will get marked completed and shouldn't be called again
                return;
            }

            if (mPrerequisite != null && !mPrerequisite.ValueAsBoolean())
            {
                TestExecution().LogMessageWithTimeFromTrigger(Name + ": prerequisites not met. Skipping.");
                mSnapshotImage.SetIsComplete(); // need to set this since this snapshot tool uses it for its IsComplete method
            }
            else
            {
                //mRequestedSnapshot = true;
                TestExecution().LogMessageWithTimeFromTrigger("Requesting snapshot");

                //CameraSnapshotDefinition theDefinition = (CameraSnapshotDefinition)Definition();

                if (mCameraSnapshotDefintion.DragAndDropFileNames.Count > 0)
                {
                    string filename = mCameraSnapshotDefintion.DragAndDropFileNames.Dequeue();
                    string ext = Path.GetExtension(filename).ToLower();
                    if ((ext == ".jpg") || (ext == ".png") || (ext == ".bmp"))
                    {
                        // load image from disk
                        Bitmap theImage = new Bitmap(filename);

                        // set result image of instance in active test execution
                        ProcessNewImage(theImage, filename);
                    }
                }
                else if (mCameraSnapshotDefintion.NumberOfImagesAvailableForRetesting() > 0)
                {
                    ProcessNewImage(mCameraSnapshotDefintion.GetImageToRetest(), "Retest");
                }
                else
                {
                    switch (mCameraSnapshotDefintion.SimulationMode)
                    {
                        case CameraSnapshotDefinition.SimulationModes.Directory_Loop:
                            // TODO: make a way for file names to be passed to ProcessNewImage()...maybe by calling ProcessNewImage() from within theDefinition.Get___Image()                  
                            ProcessNewImage(mCameraSnapshotDefintion.GetNextSimulatedImage(true), "SIMULATION"); // this marks the instance complete
                            break;
                        case CameraSnapshotDefinition.SimulationModes.Directory_Sequence:
                            ProcessNewImage(mCameraSnapshotDefintion.GetNextSimulatedImage(false), "SIMULATION"); // this marks the instance complete
                            break;
                        case CameraSnapshotDefinition.SimulationModes.Directory_Random:
                            ProcessNewImage(mCameraSnapshotDefintion.GetRandomSimulatedImage(), "SIMULATION"); // this marks the instance complete
                            break;
                        case CameraSnapshotDefinition.SimulationModes.Off:

                            mWorker = new HTTPImageGetter(this);
                            mWorker.RunWorkerAsync();

                            break;
                        default:
                            throw new ArgumentException("Current simulation mode isn't supported. 38479");
                    }
                }
            }
			// thread pooling: http://www.c-sharpcorner.com/UploadFile/mmehta/Multithreading411162005051609AM/Multithreading4.aspx?ArticleID=a3d4a3e9-533e-49c3-9fda-5bb6a7359953
		}

        CameraSnapshotDefinition mCameraSnapshotDefintion = null;
        public bool CheckTrigger()
        {
            return mCameraSnapshotDefintion.DragAndDropFileNames.Count > 0 || mCameraSnapshotDefintion.NumberOfImagesAvailableForRetesting() > 0;
        }
        public ITriggerDefinition Definition_Trigger()
        {
            return (ITriggerDefinition)Definition();
        }

        public String mWorkerLogMessage = String.Empty;
        public bool mWorkerLogMessageAvailable = false;
        private HTTPImageGetter mWorker = null;

        public void ProcessNewImage(Bitmap theImage, string fileName)
        {
            if (mWorkerLogMessageAvailable)
            {
                TestExecution().LogMessageWithTimeFromTrigger("Snapshot worker after completed: " + mWorkerLogMessage);
                mWorkerLogMessageAvailable = false;
            }

            TestExecution().LogMessageWithTimeFromTrigger(Name + " acquired image from '" + fileName + "'");

            mSnapshotImage.SetImageGenerator(this);
            mSnapshotImage.SetSource(fileName);
            mSnapshotImage.SetImage(theImage);
            mSnapshotImage.SetIsComplete();
            TestExecution().LogMessageWithTimeFromTrigger("Snapshot complete");
        }
    }

    //http://wp.netscape.com/assist/net_sites/pushpull.html
    public class HTTPImageGetter : BackgroundWorker
    {
        public HTTPImageGetter(CameraSnapshotInstance httpCameraUser)
        {
            mHttpCameraUser = httpCameraUser;
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
            this.RunWorkerCompleted += new RunWorkerCompletedEventHandler(HTTPImageGetter_RunWorkerCompleted);
            this.ProgressChanged += new ProgressChangedEventHandler(HTTPImageGetter_ProgressChanged);
        }

        private int lastProgress = -1;
        void HTTPImageGetter_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage != lastProgress)
            {
                lastProgress = e.ProgressPercentage;
                mHttpCameraUser.mWorkerLogMessage = "progress: " + lastProgress + "%";
                mHttpCameraUser.mWorkerLogMessageAvailable = true;
            }
        }

        void HTTPImageGetter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // TODO: process result codes
            mHttpCameraUser.mWorkerLogMessage = "Completed with code " + e.Result;
            mHttpCameraUser.mWorkerLogMessageAvailable = true;
            mHttpCameraUser.ProcessNewImage(mImage, mHttpCameraUser.Camera.Name);
        }

        public enum ResultCodes
        {
            Success = 0,
            Unspecified = -1,
            Timeout = -2,
            BufferTooSmall = -3,
            NonWebException = -9999
        }

        private CameraSnapshotInstance mHttpCameraUser = null;
        private Bitmap mImage = null;

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            mHttpCameraUser.mWorkerLogMessage = "snapshot worker kicked off"; mHttpCameraUser.mWorkerLogMessageAvailable = true;

            CameraSnapshotDefinition defObject = (CameraSnapshotDefinition)mHttpCameraUser.Definition();
            int readSize = 1024*1;//zxz
            int bufferSize = defObject.bufferSize;
            byte[] buffer = defObject.bufferForWorker;
            HttpWebRequest request = null;
            WebResponse response = null;
            Stream stream = null;
            Random randomNumberGenerator = new Random((int)DateTime.Now.Ticks);

            HttpRequestCachePolicy bypassCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.BypassCache);

            try
            {
                string uri = mHttpCameraUser.Camera.CompleteImageRequestURL(defObject.Resolution);
                if (mHttpCameraUser.Camera.ProxyCacheProtection)
                {
                    uri += ((uri.IndexOf('?') == -1) ? '?' : '&') + "proxyprevention=" + randomNumberGenerator.Next().ToString();
                }
                request = (HttpWebRequest)WebRequest.Create(uri);
                request.CachePolicy = bypassCachePolicy;
                request.Timeout = mHttpCameraUser.Timeout;

                if (mHttpCameraUser.Camera.Login != null && mHttpCameraUser.Camera.Login.Length > 0)
                {
                    if( mHttpCameraUser.Camera.Password == null ) mHttpCameraUser.Camera.Password = String.Empty;
                    request.Credentials = new NetworkCredential(mHttpCameraUser.Camera.Login, mHttpCameraUser.Camera.Password);
                }

                // setting ConnectionGroupName to make sure we don't run out of connections amongst all the cameras (see http://msdn2.microsoft.com/en-us/library/ms998562.aspx section: "Connections")
                request.ConnectionGroupName = mHttpCameraUser.TestSequence().Name + " : " + mHttpCameraUser.Name; // a unique group for each HTTP user within each TestSequence. We don't want to create unique groups every request because that would be inefficient
                // TODO: check maxconnection attribute in Machine.config (limits the number of concurrent outbound calls)

                mHttpCameraUser.mWorkerLogMessage = "snapshot request setup"; mHttpCameraUser.mWorkerLogMessageAvailable = true;

                response = request.GetResponse();
                stream = response.GetResponseStream();

                long contentLength = response.ContentLength;

                int bytesRead = 0;
                int totalBytesRead = 0;

                ReportProgress(0, "Requesting image");
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
                    //Console.WriteLine("Read " + bytesRead + " bytes in one chunk");
                    if (bytesRead == 0)
                    {
                        done = true;
                    }
                    else
                    {
                        totalBytesRead += bytesRead;
                        ReportProgress((int)(totalBytesRead * 100 / contentLength), "Downloading...");
                    }
                }

                mImage = (Bitmap)Bitmap.FromStream(new MemoryStream(buffer, 0, totalBytesRead));

                e.Result = ResultCodes.Success;
                ReportProgress(100, "Done!  Read " + totalBytesRead + " bytes");
                //Console.WriteLine("Read total of " + totalBytesRead + " bytes");
            }
            catch (WebException exception)
            {
                Console.WriteLine("web exception: " + exception.Message);
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
