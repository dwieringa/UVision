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
    public partial class ChartForm : Form
    {
        public OperationForm mOpForm;
        public ChartForm()//OperationForm parentForm)
        {
            //mOpForm = parentForm;
            InitializeComponent();
            //TabText = "Study Chart";
            this.Disposed += new EventHandler(ChartForm_Disposed);
        }

        void ChartForm_Disposed(object sender, EventArgs e)
        {
            //mOpForm.studyChartForm.Dispose();
            //mOpForm.studyChartForm = null;
        }

        private void ChartForm_Load(object sender, EventArgs e)
        {
            // Setup the graph
            CreateGraph_1pane(zedGraphControl1);
            // Size the control to fill the form with a margin
            SetSize();
        }

        // Respond to the form 'Resize' event
        private void ChartForm_Resize(object sender, EventArgs e)
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

        public PointPairList points = new PointPairList();

        public GraphPane pane1;

        private void CreateGraph_1pane(ZedGraphControl zgc)
        {
            // First, clear out any old GraphPane's from the MasterPane collection
            MasterPane master = zgc.MasterPane;
            master.PaneList.Clear();

            // Display the MasterPane Title, and set the outer margin to 10 points
            master.Title.IsVisible = true;
            master.Title.Text = "Value Analysis";
            master.Margin.All = 10;

            // Create some GraphPane's (normally you would add some curves too
            GraphPane pane1 = new GraphPane();

            // Add all the GraphPanes to the MasterPane
            master.Add(pane1);

            //pane1.XAxis.Title.Text = "Value";

            //pane1.XAxis.Scale.Min = 0;
            //pane1.XAxis.Scale.Max = 255;
            pane1.XAxis.Scale.FontSpec.Size += 8;
            pane1.YAxis.Title.Text = "Value";
            pane1.YAxis.Title.FontSpec.Size += 10;
            pane1.YAxis.Scale.FontSpec.Size += 8;
            pane1.Legend.FontSpec.Size += 10;

            LineItem myRCurve = pane1.AddCurve("Red", points, Color.Red, SymbolType.Diamond);

            // Layout the GraphPanes using a default Pane Layout
            using (Graphics g = this.CreateGraphics())
            {
                master.SetLayout(g, PaneLayout.SingleColumn);
            }
        }

        public void Clear()
        {
            points.Clear();
        }

        public void AddPoint(double x, double y)
        {
            points.Add(x, y);
        }

        public void UpdateChart()
        {
            zedGraphControl1.AxisChange();
            zedGraphControl1.Refresh();
        }

        public void changeAxis()
        {
            zedGraphControl1.AxisChange();
        }
    }
}