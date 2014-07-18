// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading;

namespace NetCams
{
    public class VideoCaptureByMJPEGInstance : NetCams.VideoGeneratorInstance
	{
        public VideoCaptureByMJPEGInstance(VideoCaptureByMJPEGDefinition theDefinition, TestExecution testExecution)
            : base(theDefinition, testExecution)
		{
			mCamera = theDefinition.Camera;
            mConnectionTimeout = theDefinition.ConnectionTimeout;
            mStopConditionTimeout = theDefinition.StopConditionTimeout;
            mCapturedVideo = new GeneratedVideoInstance(theDefinition.ResultantVideo, testExecution);
		}

		private NetworkCamera mCamera = null;
        [CategoryAttribute("Input")]
        public NetworkCamera Camera
		{
			get { return mCamera; }
		}

        private int mConnectionTimeout = 3000;
        [CategoryAttribute("Input")]
        public int ConnectionTimeout
        {
            get { return mConnectionTimeout; }
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

                    mWorker = new HTTPMJPegVideoGetter(this);
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
        private HTTPMJPegVideoGetter mWorker = null;

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

            if (((VideoCaptureByMJPEGDefinition)Definition()).AutoSaveEnabled)
            {
                try
                {
                    theFrame.Save(((VideoCaptureByMJPEGDefinition)Definition()).AutoSavePath + Name + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + mCapturedVideo.Video.Frames.Count + ".jpg");
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
    public class HTTPMJPegVideoGetter : BackgroundWorker
    {
        public HTTPMJPegVideoGetter(VideoCaptureByMJPEGInstance httpCameraUser)
        {
            mHttpCameraUser = httpCameraUser;
            WorkerReportsProgress = false; // progress doesn't make sense since we don't know how many frames we will be required to capture
            WorkerSupportsCancellation = true;
            this.RunWorkerCompleted += new RunWorkerCompletedEventHandler(HTTPImageGetter_RunWorkerCompleted);
        }

        void HTTPImageGetter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // TODO: process result codes
            mHttpCameraUser.mWorkerLogMessage = "Completed with code xyz";// + e.Result;
            mHttpCameraUser.mWorkerLogMessageAvailable = true;
        }

        public enum ResultCodes
        {
            Success = 0,
            Unspecified = -1,
            Timeout = -2,
            BufferTooSmall = -3,
            InvalidContentType = -4,
            VideoStreamConnectionLost = -5,
            UnexpectedError = -6,
            NonWebException = -9999
        }

        private VideoCaptureByMJPEGInstance mHttpCameraUser = null;
        private Bitmap mImage = null;

        private enum StreamSearchMode
        {
            SearchingForImageStart = 1,
            SearchingForImageBoundary = 2
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            mHttpCameraUser.mWorkerLogMessage = "snapshot worker kicked off"; mHttpCameraUser.mWorkerLogMessageAvailable = true;

            VideoCaptureByMJPEGDefinition defObject = (VideoCaptureByMJPEGDefinition)mHttpCameraUser.Definition();

            int readSize = 1024;
            int bufferSize = defObject.bufferSize;
            byte[] buffer = defObject.bufferForWorker;
            HttpWebRequest request = null;
            WebResponse response = null;
            Stream stream = null;
            Random randomNumberGenerator = new Random((int)DateTime.Now.Ticks);

            byte[] jpegMarker = new byte[] { 0xFF, 0xD8, 0xFF };
            int jpegMarkerLength = 3;// jpegMarker.GetUpperBound(0) + 1;

            byte[] boundary = null;
            int boundaryLength;

            HttpRequestCachePolicy bypassCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.BypassCache);

            try
            {
                string uri = mHttpCameraUser.Camera.CompleteVideoRequestURL(null,-1,-1,-1);
                if (mHttpCameraUser.Camera.ProxyCacheProtection)
                {
                    uri += ((uri.IndexOf('?') == -1) ? '?' : '&') + "proxyprevention=" + randomNumberGenerator.Next().ToString();
                }
                request = (HttpWebRequest)WebRequest.Create(uri);
                request.CachePolicy = bypassCachePolicy;
                request.Timeout = mHttpCameraUser.ConnectionTimeout;

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

                string contentType = response.ContentType;
                if (contentType.IndexOf("multipart/x-mixed-replace") == -1)
                {
                    mHttpCameraUser.mWorkerLogMessage = "Invalid content type from camera; type='" + contentType + "'"; mHttpCameraUser.mWorkerLogMessageAvailable = true;
                    e.Result = ResultCodes.InvalidContentType;
                    return;
                }

                ASCIIEncoding encoding = new ASCIIEncoding();
                boundary = encoding.GetBytes(contentType.Substring(contentType.IndexOf("boundary=", 0) + 9));
                boundaryLength = boundary.Length;

                stream = response.GetResponseStream();

                int startIndex = -1;
                int endIndex = -1;
                int searchPosition = 0;
                int newBytesRead = 0;
                int bytesInBuffer = 0;
                int totalBytesRead = 0;
                bool needMoreData = true;
                StreamSearchMode searchMode = StreamSearchMode.SearchingForImageStart;
                while (true) // loop "indefinitely" grabbing frames until whomever started this worker(thread) cancels it
                {
                    bool done = false;
                    while (!done)
                    {
                        if (CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }

                        if (needMoreData)
                        {
                            if (bytesInBuffer > bufferSize - readSize)
                            {
                                e.Result = ResultCodes.BufferTooSmall;
                                return;
                            }

                            newBytesRead = stream.Read(buffer, bytesInBuffer, readSize);
                            if (newBytesRead == 0)
                            {
                                e.Result = ResultCodes.VideoStreamConnectionLost;
                                return;
                            }
                            else
                            {
                                totalBytesRead += newBytesRead;
                                bytesInBuffer += newBytesRead;
                            }
                        }

                        switch (searchMode)
                        {
                            case StreamSearchMode.SearchingForImageStart:
                                startIndex = FindBytePattern(buffer, jpegMarker, searchPosition, bytesInBuffer - searchPosition);
                                if (startIndex != -1)
                                {
                                    // found start of JPEG image within stream
                                    searchPosition = startIndex;
                                    searchMode = StreamSearchMode.SearchingForImageBoundary;
                                    needMoreData = false; // we don't need data until we can't find the image's end. We don't want more data until we verify we don't have the entire image in the buffer already (since if we overfill the buffer we will error out)
                                }
                                else
                                {
                                    // image start not found
                                    searchPosition = bytesInBuffer - (jpegMarkerLength-1); // after we add more bytes to buffer, start searching in the current tail end. (ie if marker is 3 bytes, search searching in the last 2...we can't tell if the tail is a marker until we have more data)
                                    needMoreData = true;
                                }
                                break;
                            case StreamSearchMode.SearchingForImageBoundary:
                                endIndex = FindBytePattern(buffer, boundary, searchPosition, bytesInBuffer - searchPosition);
                                if (endIndex != -1)
                                {
                                    // found image boundary within stream, so we know we found the end
                                    mImage = (Bitmap)Bitmap.FromStream(new MemoryStream(buffer, startIndex, endIndex - startIndex));
                                    mHttpCameraUser.ProcessNewImage(mImage);

                                    searchPosition = endIndex + boundaryLength; // now we only care about the data in the buffer after the boundary (e.g. the start of the next image)
                                    bytesInBuffer = bytesInBuffer - searchPosition; // adjust the number of bytes in the buffer
                                    Array.Copy(buffer, searchPosition, buffer, 0, bytesInBuffer); // shift the buffer to remove the 'used' data and make room for data for the next image
                                    searchPosition = 0; // after the shift, our seachPosition is now at 0
                                    searchMode = StreamSearchMode.SearchingForImageStart;
                                    needMoreData = false; // we don't need data until we can't find the next image's start or end. We don't want more data until we verify we don't have another entire image in the buffer
                                }
                                else
                                {
                                    // didn't find the image boundary, so grab some more data and search on
                                    searchPosition = bytesInBuffer - (boundaryLength - 1);
                                    needMoreData = true;
                                }
                                break;
                            default:
                                e.Result = ResultCodes.UnexpectedError;
                                return;
                        }
                    }

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

        private int FindBytePattern(byte[] arrayToSearch, byte[] searchPattern, int startIndex, int numberOfBytesToSearch)
        {
            int searchPatternLength = searchPattern.Length;
            int index;

            while (numberOfBytesToSearch >= searchPatternLength)
            {
                // find the search pattern's first byte
                index = Array.IndexOf(arrayToSearch, searchPattern[0], startIndex, numberOfBytesToSearch - searchPatternLength - 1);

                // if we don't find the first byte of the pattern, we won't find any of it, so give up!
                if (index == -1) return -1;

                // check if the rest of the pattern matches
                int arrayIndex = index;
                int patternIndex;
                for (patternIndex = 0; patternIndex < searchPatternLength; ++patternIndex, ++arrayIndex)
                {
                    if (arrayToSearch[arrayIndex] != searchPattern[patternIndex])
                    {
                        break;
                    }
                }

                if (patternIndex == searchPatternLength)
                {
                    // found pattern! return it's starting position
                    return index;
                }

                // pattern failed to match at some point...continue searching for new match
                numberOfBytesToSearch -= (index - startIndex + 1);
                startIndex = index + 1;
            }
            return -1;
        }
    }
}
