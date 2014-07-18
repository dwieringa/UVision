// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace NetCams
{
    /// <summary>
    /// ImageDisplayDef is created once when a PictureBox row is created.
    /// Each PictureBox keeps the same ImageDisplayDef for its whole life.
    /// In the ImageHistoryForm, PictureBox's in each row use the same ImageDisplayDef.
    /// ImageDisplayDef defines which ImageDefinition, ROIs and Decorations are displayed in the PictureBox
    /// </summary>
    public class ImageDisplayDef
    {
        public ImageDisplayDef(TestSequence theTestSequence)
        {
            mTestSequence = theTestSequence;
        }

        public void AddDisplayMenu(Menu menu)
        {
            MenuItem selectImageMenu = menu.MenuItems.Add("Select Image");
            foreach (ImageDefinition imageDef in mTestSequence.ImageRegistry.ObjectList())
            {
                selectImageMenu.MenuItems.Add(imageDef.Name, new EventHandler(menu_imageSelected));
            }

            MenuItem roiMenu = menu.MenuItems.Add("ROIs");
            foreach (ROIDefinition roiDef in mTestSequence.ROIRegistry.ObjectList())
            {
                roiMenu.MenuItems.Add(roiDef.Name, new EventHandler(menu_roiSelected));
            }

            MenuItem decorationMenu = menu.MenuItems.Add("Decorations");
            foreach (IDecorationDefinition decDef in mTestSequence.DecorationRegistry.ObjectList())
            {
                decorationMenu.MenuItems.Add(decDef.Name, new EventHandler(menu_decorationSelected));
            }

            MenuItem sizeModeMenu = menu.MenuItems.Add("Size Mode");
            sizeModeMenu.MenuItems.Add("Stretch", new EventHandler(menu_sizeModeSelected));
            sizeModeMenu.MenuItems.Add("Zoom", new EventHandler(menu_sizeModeSelected));
            sizeModeMenu.MenuItems.Add("Center", new EventHandler(menu_sizeModeSelected));
            sizeModeMenu.MenuItems.Add("Normal", new EventHandler(menu_sizeModeSelected));

/*            MenuItem sizeModeMenu = menu.MenuItems.Add("Table View");
            sizeModeMenu.MenuItems.Add("Add Row", new EventHandler(menu_tableAdjustment));
            sizeModeMenu.MenuItems.Add("Remove Row", new EventHandler(menu_tableAdjustment));
            sizeModeMenu.MenuItems.Add("Add Column (more history)", new EventHandler(menu_tableAdjustment));
            sizeModeMenu.MenuItems.Add("Remove Column (less history)", new EventHandler(menu_tableAdjustment));*/
        }

        private void menu_imageSelected(object sender, EventArgs e)
        {
            MenuItem miClicked = (MenuItem)sender;
            SetImage(miClicked.Text);
        }

        private void menu_roiSelected(object sender, EventArgs e)
        {
            MenuItem miClicked = (MenuItem)sender;
            ToggleROI(miClicked.Text);
        }

        private void menu_decorationSelected(object sender, EventArgs e)
        {
            MenuItem miClicked = (MenuItem)sender;
            ToggleDecoration(miClicked.Text);
        }
        
        private void menu_sizeModeSelected(object sender, EventArgs e)
        {
            MenuItem miClicked = (MenuItem)sender;
            // WARNING: PictureBoxSizeMode.AutoSize causes PictureBox to resize for some reason
            switch (miClicked.Text)
            {
                case "Stretch":
                    SizeMode = PictureBoxSizeMode.StretchImage;
                    break;
                case "Center":
                    SizeMode = PictureBoxSizeMode.CenterImage;
                    break;
                case "Normal":
                    SizeMode = PictureBoxSizeMode.Normal;
                    break;
                case "Zoom":
                    SizeMode = PictureBoxSizeMode.Zoom;
                    break;
            }
        }

        /*
        private void menu_tableAdjustment(object sender, EventArgs e)
        {
            MenuItem miClicked = (MenuItem)sender;
            // WARNING: PictureBoxSizeMode.AutoSize causes PictureBox to resize for some reason
            switch (miClicked.Text)
            {
                case "Stretch":
                    SizeMode = PictureBoxSizeMode.StretchImage;
                    break;
                case "Center":
                    SizeMode = PictureBoxSizeMode.CenterImage;
                    break;
                case "Normal":
                    SizeMode = PictureBoxSizeMode.Normal;
                    break;
                case "Zoom":
                    SizeMode = PictureBoxSizeMode.Zoom;
                    break;
            }
        }*/

        public delegate void ImageDisplayDefChangeDelegate();
        public event ImageDisplayDefChangeDelegate ImageDisplayDefChange;

        private TestSequence mTestSequence;
        public TestSequence TestSequence
        {
            get { return mTestSequence; }
        }

        public void SetImage(string imageName)
        {
            ImageDefinition = mTestSequence.ImageRegistry.GetObject(imageName);
        }

        public void ToggleROI(string roiName)
        {
            ROIDefinition roiDef = mTestSequence.ROIRegistry.GetObject(roiName);
            if (roisToShow.Contains(roiDef))
            {
                roisToShow.Remove(roiDef);
            }
            else
            {
                roisToShow.Add(roiDef);
            }
            if (ImageDisplayDefChange != null)
            {
                ImageDisplayDefChange();
            }
        }

        public void ToggleDecoration(string decorationName)
        {
            IDecorationDefinition decorationDef = mTestSequence.DecorationRegistry.GetObject(decorationName);
            if (decorationsToShow.Contains(decorationDef))
            {
                decorationsToShow.Remove(decorationDef);
            }
            else
            {
                decorationsToShow.Add(decorationDef);
            }
            if (ImageDisplayDefChange != null)
            {
                ImageDisplayDefChange();
            }
        }

        private ImageDefinition mImageDefinition;
        public ImageDefinition ImageDefinition
        {
            get { return mImageDefinition; }
            set
            {
                if (value != null && value.TestSequence() != mTestSequence) throw new ArgumentException("3290489");
                mImageDefinition = value;
                if (ImageDisplayDefChange != null)
                {
                    ImageDisplayDefChange();
                }
            }
        }
        public List<IDecorationDefinition> decorationsToShow = new List<IDecorationDefinition>();
        public List<ROIDefinition> roisToShow = new List<ROIDefinition>();

        private PictureBoxSizeMode mSizeMode = PictureBoxSizeMode.StretchImage;
        public PictureBoxSizeMode SizeMode
        {
            get { return mSizeMode; }
            set
            {
                mSizeMode = value;
                if (ImageDisplayDefChange != null)
                {
                    ImageDisplayDefChange();
                }
            }
        }

        public void Clone(ImageDisplayDef theSourceDef)
        {
            if (theSourceDef.TestSequence != mTestSequence) throw new ArgumentException("3292289");

            mImageDefinition = theSourceDef.ImageDefinition;

            roisToShow.Clear();
            roisToShow.AddRange(theSourceDef.roisToShow);

            decorationsToShow.Clear();
            decorationsToShow.AddRange(theSourceDef.decorationsToShow);

            mSizeMode = theSourceDef.SizeMode;

            if (ImageDisplayDefChange != null)
            {
                ImageDisplayDefChange();
            }
        }

        public string SerializeDef()
        {
            string def = string.Empty;
            if (ImageDefinition != null) def += ImageDefinition.Name;
            def += "; " + SizeMode + ";";
            string seperator = " ";
            foreach (ROIDefinition roiDef in roisToShow)
            {
                def += seperator + roiDef.Name;
                seperator = ", ";
            }
            def += ";";
            seperator = " "; 
            foreach (IDecorationDefinition decDef in decorationsToShow)
            {
                def += seperator + decDef.Name;
                seperator = ", ";
            }
            return def;
        }
        public void DeserializeDef(string defString)
        {
            string[] mainSplitComponents = defString.Split(new char[] { ';' });
            if (mainSplitComponents.GetUpperBound(0) != 3) throw new ArgumentException("Invalid image row definition; definition='" + defString + "'");

            ImageDefinition = mTestSequence.ImageRegistry.GetObjectIfExists(mainSplitComponents[0].Trim());

            switch (mainSplitComponents[1].Trim())
            {
                case "StretchImage":
                    SizeMode = PictureBoxSizeMode.StretchImage;
                    break;
                case "Zoom":
                    SizeMode = PictureBoxSizeMode.Zoom;
                    break;
                case "Normal":
                    SizeMode = PictureBoxSizeMode.Normal;
                    break;
                case "CenterImage":
                    SizeMode = PictureBoxSizeMode.CenterImage;
                    break;
                default:
                    SizeMode = PictureBoxSizeMode.StretchImage;
                    break;
            }
            string[] objects = mainSplitComponents[2].Split(new char[] { ',' });
            string name;
            for (int ndx = 0; ndx <= objects.GetUpperBound(0); ndx++)
            {
                name = objects[ndx].Trim();
                if (name != string.Empty)
                {
                    ROIDefinition roi = mTestSequence.ROIRegistry.GetObjectIfExists(name);
                    if( roi != null ) roisToShow.Add(roi);
                }
            }

            objects = mainSplitComponents[3].Split(new char[] { ',' });
            for (int ndx = 0; ndx <= objects.GetUpperBound(0); ndx++)
            {
                name = objects[ndx].Trim();
                if (name != string.Empty)
                {
                    IDecorationDefinition decoration = mTestSequence.DecorationRegistry.GetObjectIfExists(name);
                    if (decoration != null) decorationsToShow.Add(decoration);
                }
            }
        }

        public void UpdateDisplayComponents(TestExecution testExecution, ref List<ROIInstance> roiInstances, ref List<IDecorationInstance>  decorationInstances)
        {
            roiInstances.Clear();
            decorationInstances.Clear();

            if (testExecution == null) return;
            if (testExecution.Sequence() != mTestSequence) throw new ArgumentException("3244289");

            foreach (ROIDefinition roiDef in roisToShow)
            {
                if (roiDef == null) continue;
                ROIInstance roi = testExecution.ROIRegistry.GetObjectIfExists(roiDef.Name);
                if( roi != null ) roiInstances.Add(roi);
            }
            foreach (IDecorationDefinition decorationDef in decorationsToShow)
            {
                if (decorationDef == null) continue;
                IDecorationInstance decoration = testExecution.DecorationRegistry.GetObjectIfExists(decorationDef.Name);
                if( decoration != null ) decorationInstances.Add(decoration);
            }
        }
    }
}
