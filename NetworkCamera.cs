// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace NetCams
{
    [TypeConverter(typeof(NetworkCameraConverter))]
    public class NetworkCamera : Camera
	{
        public TreeNode treeNode;

		public NetworkCamera(Project visionProject)
            : base(visionProject)
		{
            treeNode = new TreeNode();
            Name = "Untitled Camera";
            Window().camerasNode.Nodes.Add(treeNode);
            visionProject.RegisterCamera(this, true);
		}

        public override string Name
        {
            get { return base.Name; }
            set
            {
                if( project().IsCameraNameUnique( value ) )
                {
                    base.Name = value;
                    treeNode.Text = value;
                }
                else
                {
                    throw new ArgumentException("Another camera already exists with that name. Names must be unique.");
                }
            }
        }

        private string ip;
        /// <summary>
		/// Internet address of the camera
		/// </summary>
		public string IP
		{
			get
			{
				return ip;
			}
			set
			{
                ip = value;
			}
		}

		private int port;
		/// <summary>
		/// Internet port the camera is listening on
		/// </summary>
		public int Port
		{
			get
			{
				return port;
			}
			set
			{
				port = value;
			}
		}

        private string imageURL;
        public string ImageURL
        {
            get
            {
                return imageURL;
            }
            set
            {
                imageURL = value;
            }
        }

        private string videoURL;
        public string VideoURL
        {
            get
            {
                return videoURL;
            }
            set
            {
                videoURL = value;
            }
        }

        private string resolution;
        public string Resolution
		{
			get
			{
				return resolution;
			}
			set
			{
				// TODO: verify valid
				resolution = value;
			}
		}

        public String CompleteImageRequestURL()
        {
            return CompleteImageRequestURL(resolution);
        }
        public String CompleteImageRequestURL(string requestedResolution)
        {
            return "http://" + IP + ":" + port + imageURL + "?resolution=" + requestedResolution;
        }

        public String CompleteVideoRequestURL(string requestedResolution, int requestedCompression, int framesPerSecond, int requestedRotation)
        {
            String url = "http://" + IP + ":" + port + videoURL;
            if (requestedResolution != null && requestedResolution.Length == 0)
            {
                url += "?resolution=" + requestedResolution;
            }
            else
            {
                url += "?resolution=" + resolution;
            }

            if (requestedCompression >= 0)
            {
                url += "&compression=" + requestedCompression;
            }

            if (framesPerSecond >= 0)
            {
                url += "&fps=" + framesPerSecond;
            }

            if (requestedRotation >= 0)
            {
                url += "&rotation=" + requestedRotation;
            }

            return url;
        }

        private String mLogin = String.Empty;
        public String Login
        {
            get { return mLogin; }
            set { mLogin = value; }
        }

        private String mPassword = String.Empty;
        public String Password
        {
            get { return mPassword; }
            set { mPassword = value; }
        }

        private bool mProxyCacheProtection = false;
        public bool ProxyCacheProtection
        {
            get { return mProxyCacheProtection; }
            set { mProxyCacheProtection = value; }
        }

        public Bitmap Acquire()
        {
            return Acquire(resolution);
        }

        private Bitmap lastImage = null;
		public Bitmap Acquire(string resolution)
		{
            // http://www.codeproject.com/cs/media/cameraviewer.asp
            // http://www.codeproject.com/cs/media/Motion_Detection.asp

			try
			{
                lastImage = null;
				WebClient client = new WebClient ();
				String completeURL = "http://" + IP + ":" + port + imageURL + "?Resolution=" + resolution;
				Stream stream = client.OpenRead(completeURL);
				lastImage = new Bitmap(stream);
				stream.Close();
			}
			catch
			{
                lastImage = null;
			}

            return lastImage;
		}
	}

    public class NetworkCameraConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context.Instance is IObjectDefinition)
            {
                return new StandardValuesCollection(((IObjectDefinition)context.Instance).Project().CameraOptions());
            }
            else if (context.Instance is FavoriteSettings)
            {
                return new StandardValuesCollection(((FavoriteSettings)context.Instance).mTestSequence.project().CameraOptions());
            }
            else
            {
                throw new ArgumentException("why are we here? 932083");
            }
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
        {
            if (destinationType == typeof(NetworkCamera))
                return true;

            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
        {
            if (destinationType == typeof(System.String) &&
                value is NetworkCamera)
            {

                NetworkCamera idd = (NetworkCamera)value;

                return idd.Name;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                NetworkCamera namedCamera;

                try
                {
                    string cameraName = (string)value;

                    if (context.Instance is IObjectDefinition)
                    {
                        IObjectDefinition objectBeingEditted = (IObjectDefinition)context.Instance;
                        namedCamera = objectBeingEditted.Project().FindCamera(cameraName);
                    }
                    else if (context.Instance is FavoriteSettings)
                    {
                        namedCamera = ((FavoriteSettings)context.Instance).mTestSequence.project().FindCamera(cameraName);
                    }
                    else
                    {
                        throw new ArgumentException("alksdjaqwhkajlkaj");
                    }
                }
                catch
                {
                    throw new ArgumentException("Can not convert '" + (string)value + "' to a camera");
                }
                return namedCamera;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
