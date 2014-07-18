// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using WeifenLuo.WinFormsUI.Docking;

namespace NetCams
{
    public partial class AnalysisForm : DockContent
    {
        private OperationForm mOpForm;
        public AnalysisForm(OperationForm parentForm)
        {
            mOpForm = parentForm;
            InitializeComponent();
            TabText = "Study Chart";
            this.Disposed += new EventHandler(AnalysisForm_Disposed);
        }

        void AnalysisForm_Disposed(object sender, EventArgs e)
        {
            mOpForm.studyChartForm.Dispose();
            mOpForm.studyChartForm = null;
        }

        private void AnalysisForm_Load(object sender, EventArgs e)
        {
            // Setup the graph
            CreateGraph_3panes(zedGraphControl1);
            // Size the control to fill the form with a margin
            SetSize();
        }

        // Respond to the form 'Resize' event
        private void AnalysisForm_Resize(object sender, EventArgs e)
        {
            SetSize();
        }

        // SetSize() is separate from Resize() so we can 
        // call it independently from the Form1_Load() method
        // This leaves a 10 px margin around the outside of the control
        // Customize this to fit your needs
        private void SetSize()
        {
            zedGraphControl1.Location = new Point(10, 10);
            // Leave a small margin around the outside of the control
            zedGraphControl1.Size = new Size(ClientRectangle.Width - 20, ClientRectangle.Height - 20);
        }

        public PointPairList rList = new PointPairList();
        public PointPairList gList = new PointPairList();
        public PointPairList bList = new PointPairList();

        public PointPairList hList = new PointPairList();
        public PointPairList sList = new PointPairList();
        public PointPairList iList = new PointPairList();

        public GraphPane rgbPane;
        public GraphPane hPane;
        public GraphPane siPane;

        private void CreateGraph_3panes(ZedGraphControl zgc)
        {
            // First, clear out any old GraphPane's from the MasterPane collection
            MasterPane master = zgc.MasterPane;
            master.PaneList.Clear();

            // Display the MasterPane Title, and set the outer margin to 10 points
            master.Title.IsVisible = true;
            master.Title.Text = "Color Analysis";
            master.Margin.All = 10;

            // Create some GraphPane's (normally you would add some curves too
            GraphPane rgbPane = new GraphPane();
            GraphPane hPane = new GraphPane();
            GraphPane siPane = new GraphPane();

            // Add all the GraphPanes to the MasterPane
            master.Add(rgbPane);
            master.Add(hPane);
            master.Add(siPane);

            //rgbPane.XAxis.Title.Text = "Value";

            rgbPane.XAxis.Scale.Min = 0;
            rgbPane.XAxis.Scale.Max = 255;
            rgbPane.XAxis.Scale.FontSpec.Size += 8;
            rgbPane.YAxis.Title.Text = "Frequency";
            rgbPane.YAxis.Title.FontSpec.Size += 10;
            rgbPane.YAxis.Scale.FontSpec.Size += 8;
            rgbPane.Legend.FontSpec.Size += 10;

            hPane.XAxis.Scale.Min = 0;
            hPane.XAxis.Scale.Max = 360;
            hPane.XAxis.Scale.MajorStep = 60;
            hPane.XAxis.Scale.FontSpec.Size += 8;
            hPane.YAxis.Title.Text = "Frequency";
            hPane.YAxis.Title.FontSpec.Size += 10;
            hPane.YAxis.Scale.FontSpec.Size += 8;
            hPane.Legend.FontSpec.Size += 10;

            siPane.XAxis.Scale.Min = 0;
            siPane.XAxis.Scale.Max = 100;
            siPane.XAxis.Scale.FontSpec.Size += 8;
            siPane.YAxis.Title.Text = "Frequency";
            siPane.YAxis.Scale.FontSpec.Size += 10;
            siPane.YAxis.Title.FontSpec.Size += 8;
            siPane.Legend.FontSpec.Size += 10;

            LineItem myRCurve = rgbPane.AddCurve("Red",
                  rList, Color.Red, SymbolType.Diamond);
            LineItem myGCurve = rgbPane.AddCurve("Green",
                  gList, Color.Green, SymbolType.Diamond);
            LineItem myBCurve = rgbPane.AddCurve("Blue",
                  bList, Color.Blue, SymbolType.Diamond);

            LineItem myHCurve = hPane.AddCurve("Hue",
                  hList, Color.Cyan, SymbolType.Circle);
            LineItem mySCurve = siPane.AddCurve("Saturation",
                  sList, Color.Magenta, SymbolType.Circle);
            LineItem myICurve = siPane.AddCurve("Intensity",
                  iList, Color.YellowGreen, SymbolType.Circle);

            // Layout the GraphPanes using a default Pane Layout
            using (Graphics g = this.CreateGraphics())
            {
                master.SetLayout(g, PaneLayout.SingleColumn);
            }
        }

        public void udpateChart(int[] rArray, int[] gArray, int[] bArray, int[] hArray, int[] sArray, int[] iArray)
        {
            rList.Clear();
            gList.Clear();
            bList.Clear();
            hList.Clear();
            sList.Clear();
            iList.Clear();
            for (int x = 0; x < 256; x++)
            {
                rList.Add((double)x, (double)rArray[x]);
                gList.Add((double)x, (double)gArray[x]);
                bList.Add((double)x, (double)bArray[x]);
            }
            for (int x = 0; x <= 360; x++)
            {
                hList.Add((double)x, (double)hArray[x]);
            }
            for (int x = 0; x <= 100; x++)
            {
                sList.Add((double)x, (double)sArray[x]);
                iList.Add((double)x, (double)iArray[x]);
            }
            zedGraphControl1.AxisChange();
            zedGraphControl1.Refresh();
        }

        /*        // Build the Chart
                private void CreateGraph(ZedGraphControl zgc)
                {
                    // get a reference to the GraphPane
                    GraphPane myPane = zgc.GraphPane;

                    // Set the Titles
                    myPane.Title.Text = "My Test Graph\n(blah blah blah)";
                    myPane.XAxis.Title.Text = "My X Axis";
                    myPane.YAxis.Title.Text = "My Y Axis";


                    LineItem myRCurve = myPane.AddCurve("Red",
                          rList, Color.Red, SymbolType.Diamond);
                    LineItem myGCurve = myPane.AddCurve("Green",
                          gList, Color.Green, SymbolType.Diamond);
                    LineItem myBCurve = myPane.AddCurve("Blue",
                          bList, Color.Blue, SymbolType.Diamond);

                    LineItem myHCurve = myPane.AddCurve("Hue",
                          hList, Color.Cyan, SymbolType.Circle);
                    LineItem mySCurve = myPane.AddCurve("Saturation",
                          sList, Color.Magenta, SymbolType.Circle);
                    LineItem myICurve = myPane.AddCurve("Intensity",
                          iList, Color.YellowGreen, SymbolType.Circle);

                    // Tell ZedGraph to refigure the
                    // axes since the data have changed
                    zgc.AxisChange();
                }
         */

        public void changeAxis()
        {
            zedGraphControl1.AxisChange();
        }

        public void DrawChart(Image theImage, Rectangle theROI)
        {
            int[] rArray = new int[256];
            int[] gArray = new int[256];
            int[] bArray = new int[256];
            int[] hArray = new int[361];
            int[] sArray = new int[101];
            int[] iArray = new int[101];
            bool chartingROI = true;
            if (theImage != null)
			{
				// Analyze current image
//				DateTime startTime = DateTime.Now;
//				mainWindow.logMessage("Starting ROI study");

//				mainWindow.logMessage("Study results :");
				// Print RGB and HSI values for each ROI

				double averageR = 0, averageG = 0, averageB = 0;

				int minR = 999, minG = 999, minB = 999, maxR = -1, maxG = -1, maxB = -1;
				for (int j = theROI.Top; j <= theROI.Bottom; j++)
				{
					long sumR = 0, sumG = 0, sumB = 0;
			
					// We average each row then sum the averages
					// to avoid overflows while summing
					for (int i = theROI.Left; i <= theROI.Right; i++)
					{
                        Color color = ((Bitmap)theImage).GetPixel(i, j);
						if( color.R < minR ) minR = color.R;
						if( color.G < minG ) minG = color.G;
						if( color.B < minB ) minB = color.B;
						if( color.R > maxR ) maxR = color.R;
						if( color.G > maxG ) maxG = color.G;
						if( color.B > maxB ) maxB = color.B;
						sumR += color.R;
						sumG += color.G;
						sumB += color.B;
                        if (chartingROI)
                        {
                            rArray[color.R] += 1;
                            gArray[color.G] += 1;
                            bArray[color.B] += 1;
                            hArray[(int)color.GetHue()] += 1;
                            sArray[(int)(color.GetSaturation()*100)] += 1;
                            iArray[(int)(color.GetBrightness()*100)] += 1;
                        }
                    }
                    averageR += (double)sumR / (double)((theROI.Right - theROI.Left + 1) * (theROI.Bottom - theROI.Top + 1));
                    averageG += (double)sumG / (double)((theROI.Right - theROI.Left + 1) * (theROI.Bottom - theROI.Top + 1));
                    averageB += (double)sumB / (double)((theROI.Right - theROI.Left + 1) * (theROI.Bottom - theROI.Top + 1));

				}

                udpateChart(rArray, gArray, bArray, hArray, sArray, iArray);

                /*
				Color RGB = Color.FromArgb((int) averageR,(int) averageG, (int) averageB);

				int H,S,I;
				ColorSpace.RGBToHSI(RGB.R, RGB.G, RGB.B, out H, out S, out I);
				mainWindow.logMessage("\tROI " + roiNumber.ToString() + " :");
				mainWindow.logMessage("\t  Min R :" + minR);
				mainWindow.logMessage("\t  Min G :" + minG);
				mainWindow.logMessage("\t  Min B :" + minB);
				mainWindow.logMessage("\t  Max R :" + maxR);
				mainWindow.logMessage("\t  Max G :" + maxG);
				mainWindow.logMessage("\t  Max B :" + maxB);
				mainWindow.logMessage("\t  Average R :" + RGB.R);
				mainWindow.logMessage("\t  Average G :" + RGB.G);
				mainWindow.logMessage("\t  Average B :" + RGB.B);
				mainWindow.logMessage("\t  Average H :" + H);
				mainWindow.logMessage("\t  Average S :" + S + "%");
				mainWindow.logMessage("\t  Average I :" + I);
                */
//				mainWindow.logMessage("Image analyzed in:" + DateTime.Now.Subtract(startTime));
			}
        }
    }
}