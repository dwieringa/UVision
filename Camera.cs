// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class Camera : ProjectComponent
	{
		public Camera(Project visionProject)
		{
			mVisionProject = visionProject;
		}

		private Project mVisionProject;

        public Project project() { return mVisionProject; }
        public ProgrammingForm Window() { return mVisionProject.Window(); }

		private string name;
		/// <summary>
		/// Uniquely identifies all tools used within the software
		/// </summary>
		public virtual string Name
		{
			get
			{
				return name;
			}
			set
			{
				// TODO: ensure unique or throw an exception
				name = value;
			}
		}
	}


}
