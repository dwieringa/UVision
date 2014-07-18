// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
    [TypeConverter(typeof(ImageDefinitionConverter))]
	public abstract class ImageDefinition : DataDefinition
	{
		public ImageDefinition(TestSequence testSequence) : base(testSequence)
		{
			testSequence.ImageRegistry.RegisterObject(this);
		}

        public override void Dispose_UVision()
        {
            TestSequence().ImageRegistry.UnRegisterObject(this);
            base.Dispose_UVision();
        }
	}


    public class ImageDefinitionConverter : ObjectDefinitionConverter
    {
        protected override string[] Options(TestSequence theTestSequence)
        {
            return theTestSequence.ImageRegistry.Options();
        }

        protected override IObjectDefinition LookupObject(TestSequence theTestSequence, string theObjectName)
        {
            return theTestSequence.ImageRegistry.GetObject(theObjectName);
        }
    }
}
