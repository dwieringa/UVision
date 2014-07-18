// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;


namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class ImageGeneratorInstance : NetCams.ToolInstance
	{
		public ImageGeneratorInstance(ImageGeneratorDefinition theDefinition, TestExecution testExecution) : base(theDefinition, testExecution)
		{
        }

		public abstract ImageInstance ResultantImage
		{
			get;
		}

        public ImageGeneratorDefinition Definition_ImageGenerator() { return (ImageGeneratorDefinition)Definition(); }

        /// <summary>
        /// Decendant classes should implement this instead of DoWork()
        /// </summary>
        protected abstract void DoWork_impl();

        public override void DoWork()
        {
            DoWork_impl();

            if (ResultantImage != null && ResultantImage.IsComplete() && ResultantImage.Bitmap != null)
            {

                // TODO: make casting safer. throw exception if not correct type...would only help if this code was cut and pasted to another type. store typed reference to definition?
                if (((ImageGeneratorDefinition)Definition()).AutoSaveEnabled)
                {
                    try
                    {
                        ResultantImage.Save(((ImageGeneratorDefinition)Definition()).AutoSavePath, Name, true);
                        TestExecution().LogMessageWithTimeFromTrigger(Name + " resultant image auto saved");
                    }
                    catch (ArgumentException e)
                    {
                        Project().Window().logMessage("ERROR: " + e.Message);
                        TestExecution().LogErrorWithTimeFromTrigger(e.Message);
                    }
                    catch (Exception e)
                    {
                        Project().Window().logMessage("ERROR: Unable to AutoSave result.  Low-level message=" + e.Message);
                        TestExecution().LogErrorWithTimeFromTrigger("Unable to AutoSave result.  Ensure path valid and disk not full.");
                    }
                }
            }
        }
    }
}
