// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Reflection;

namespace NetCams
{
	/// <summary>
	/// Summary description for ProgrammingForm.
	/// </summary>
	public class ProgrammingForm : System.Windows.Forms.Form
	{
		public TNDLink_old myTNDLink; //'DWWx
        public TreeNode projectNode;
        public TreeNode readerNode;
        public TreeNode writerNode;
        public TreeNode camerasNode;
        public TreeNode sequencesNode;

        delegate void StringDelegate(String str);
        delegate void TreeNodeDelegate(TreeNode parentNode, TreeNode childNode);
        private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem miLoad;
		private System.Windows.Forms.MenuItem miSave;
		private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem miExit;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolBar toolBar;
        private System.Windows.Forms.MenuItem miAbout;
		private System.Windows.Forms.MenuItem miHelp;
		private System.ComponentModel.IContainer components;
		public const string settingsFileName = "Project.ini";
		// The 2 currentXXX variables indicate the indices of the items selected in the treeView
		TreeNode currentCamera;
        //private System.Windows.Forms.Timer TNDPollTimer;//OLDTND
		private System.Windows.Forms.MenuItem miClearLogWindow;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem miAutoScrollLog;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.Panel statusPanel;
		public System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.TextBox tbStatus;
		private System.Windows.Forms.Splitter statusSplitter;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.Splitter propertySplitter;
        private System.Windows.Forms.Label lblTNDConnectionDisabled;//OLDTND 
        private System.Windows.Forms.Label lblLostTNDConnection;//OLDTND 
		private System.Windows.Forms.PageSetupDialog pageSetupDialog1;
		private SourceGrid3.Grid toolGrid;
		private System.Windows.Forms.ToolBarButton tbbRebuildGrid;
        private MenuItem miLoadSeq;
        private MenuItem miSaveSeq;
        private MenuItem menuItem8;
        private MenuItem miSimulateLoad;
        private MenuItem menuItem5;
        private EnterpriseDT.Net.Ftp.FTPConnection ftpConnection;
		public TreeNode currentROI;

        public object SelectedObject() { return propertyGrid1.SelectedObject; }
        //public SourceGrid3.Grid ToolGrid() { return toolGrid; }
        public SourceGrid3.Grid ToolGrid() { return toolGrid; }
        public CellClickEvent clickController;

		public ProgrammingForm()
		{
            myTNDLink = new TNDLink_old(this); //'DWW
			myTNDLink.TNDLinkEstablished += new TNDLink.TNDLinkEstablishedDelegate(this.tndLink_EstablishedConnection);
			myTNDLink.TNDLinkLost += new TNDLink.TNDLinkLostDelegate(this.tndLink_LostConnection);
			myTNDLink.TNDLinkDisabled += new TNDLink.TNDLinkDisabledDelegate(this.tndLink_ConnectionDisabled);


			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            logMessage("Starting up");
            
            clickController = new CellClickEvent(propertyGrid1);
//			toolGrid.Redim(3,5);
            projectNode = new TreeNode("Project");
            camerasNode = new TreeNode("Cameras");
            readerNode = new TreeNode("TND Reader");
            writerNode = new TreeNode("TND Writer");
            sequencesNode = new TreeNode("Test Sequences");
            treeView1.Nodes.Add(projectNode);
            projectNode.Nodes.Add(camerasNode);
            projectNode.Nodes.Add(readerNode);
            projectNode.Nodes.Add(writerNode);
            projectNode.Nodes.Add(sequencesNode);

        }

		public static Project Project;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.miLoad = new System.Windows.Forms.MenuItem();
            this.miSave = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.miLoadSeq = new System.Windows.Forms.MenuItem();
            this.miSaveSeq = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.miSimulateLoad = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.miClearLogWindow = new System.Windows.Forms.MenuItem();
            this.miAutoScrollLog = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.miExit = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.miHelp = new System.Windows.Forms.MenuItem();
            this.miAbout = new System.Windows.Forms.MenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.toolBar = new System.Windows.Forms.ToolBar();
            this.tbbRebuildGrid = new System.Windows.Forms.ToolBarButton();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.statusPanel = new System.Windows.Forms.Panel();
            this.lblLostTNDConnection = new System.Windows.Forms.Label();
            this.lblTNDConnectionDisabled = new System.Windows.Forms.Label();
            this.tbStatus = new System.Windows.Forms.TextBox();
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.statusSplitter = new System.Windows.Forms.Splitter();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.propertySplitter = new System.Windows.Forms.Splitter();
            this.pageSetupDialog1 = new System.Windows.Forms.PageSetupDialog();
            this.toolGrid = new SourceGrid3.Grid();
            this.ftpConnection = new EnterpriseDT.Net.Ftp.FTPConnection(this.components);
            this.statusPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView1.Location = new System.Drawing.Point(3, 28);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(200, 445);
            this.treeView1.TabIndex = 0;
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseUp);
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem9});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miLoad,
            this.miSave,
            this.menuItem4,
            this.miLoadSeq,
            this.miSaveSeq,
            this.menuItem5,
            this.miSimulateLoad,
            this.menuItem8,
            this.miClearLogWindow,
            this.miAutoScrollLog,
            this.menuItem7,
            this.miExit});
            this.menuItem1.Text = "File";
            // 
            // miLoad
            // 
            this.miLoad.Index = 0;
            this.miLoad.Text = "Load Settings";
            this.miLoad.Visible = false;
            this.miLoad.Click += new System.EventHandler(this.miLoad_Click);
            // 
            // miSave
            // 
            this.miSave.Enabled = false;
            this.miSave.Index = 1;
            this.miSave.Text = "Save Project";
            this.miSave.Click += new System.EventHandler(this.miSave_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 2;
            this.menuItem4.Text = "-";
            // 
            // miLoadSeq
            // 
            this.miLoadSeq.Index = 3;
            this.miLoadSeq.Text = "Load Sequence";
            this.miLoadSeq.Visible = false;
            this.miLoadSeq.Click += new System.EventHandler(this.miLoadSeq_Click);
            // 
            // miSaveSeq
            // 
            this.miSaveSeq.Index = 4;
            this.miSaveSeq.Text = "Save Sequence";
            this.miSaveSeq.Click += new System.EventHandler(this.miSaveSeq_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 5;
            this.menuItem5.Text = "-";
            // 
            // miSimulateLoad
            // 
            this.miSimulateLoad.Index = 6;
            this.miSimulateLoad.Text = "Dev Stub (ignore)";
            this.miSimulateLoad.Click += new System.EventHandler(this.miSimulateLoad_Click);
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 7;
            this.menuItem8.Text = "-";
            // 
            // miClearLogWindow
            // 
            this.miClearLogWindow.Index = 8;
            this.miClearLogWindow.Text = "Clear log window";
            this.miClearLogWindow.Click += new System.EventHandler(this.miClearLogWindow_Click);
            // 
            // miAutoScrollLog
            // 
            this.miAutoScrollLog.Checked = true;
            this.miAutoScrollLog.Index = 9;
            this.miAutoScrollLog.Text = "Auto Scroll";
            this.miAutoScrollLog.Click += new System.EventHandler(this.miAutoScrollLog_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 10;
            this.menuItem7.Text = "-";
            // 
            // miExit
            // 
            this.miExit.Index = 11;
            this.miExit.Text = "Exit";
            this.miExit.Click += new System.EventHandler(this.miExit_Click);
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 1;
            this.menuItem9.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miHelp,
            this.miAbout});
            this.menuItem9.Text = "Help";
            // 
            // miHelp
            // 
            this.miHelp.Index = 0;
            this.miHelp.Text = "Program Help";
            this.miHelp.Click += new System.EventHandler(this.miHelp_Click);
            // 
            // miAbout
            // 
            this.miAbout.Index = 1;
            this.miAbout.Text = "About";
            this.miAbout.Click += new System.EventHandler(this.miAbout_Click);
            // 
            // toolBar
            // 
            this.toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.tbbRebuildGrid});
            this.toolBar.ButtonSize = new System.Drawing.Size(70, 22);
            this.toolBar.DropDownArrows = true;
            this.toolBar.Location = new System.Drawing.Point(0, 0);
            this.toolBar.Name = "toolBar";
            this.toolBar.ShowToolTips = true;
            this.toolBar.Size = new System.Drawing.Size(800, 28);
            this.toolBar.TabIndex = 4;
            this.toolBar.TextAlign = System.Windows.Forms.ToolBarTextAlign.Right;
            this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
            // 
            // tbbRebuildGrid
            // 
            this.tbbRebuildGrid.Name = "tbbRebuildGrid";
            this.tbbRebuildGrid.Text = "Refresh Grid";
            // 
            // splitter2
            // 
            this.splitter2.Location = new System.Drawing.Point(0, 28);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 573);
            this.splitter2.TabIndex = 12;
            this.splitter2.TabStop = false;
            // 
            // statusPanel
            // 
            this.statusPanel.Controls.Add(this.lblLostTNDConnection);
            this.statusPanel.Controls.Add(this.lblTNDConnectionDisabled);
            this.statusPanel.Controls.Add(this.tbStatus);
            this.statusPanel.Controls.Add(this.statusBar);
            this.statusPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusPanel.Location = new System.Drawing.Point(3, 481);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(797, 120);
            this.statusPanel.TabIndex = 15;
            // 
            // lblLostTNDConnection
            // 
            this.lblLostTNDConnection.BackColor = System.Drawing.Color.Red;
            this.lblLostTNDConnection.Location = new System.Drawing.Point(342, 52);
            this.lblLostTNDConnection.Name = "lblLostTNDConnection";
            this.lblLostTNDConnection.Size = new System.Drawing.Size(120, 16);
            this.lblLostTNDConnection.TabIndex = 20;
            this.lblLostTNDConnection.Text = " Lost T&&D Connection ";
            this.lblLostTNDConnection.Visible = false;
            // 
            // lblTNDConnectionDisabled
            // 
            this.lblTNDConnectionDisabled.BackColor = System.Drawing.Color.Yellow;
            this.lblTNDConnectionDisabled.Location = new System.Drawing.Point(330, 52);
            this.lblTNDConnectionDisabled.Name = "lblTNDConnectionDisabled";
            this.lblTNDConnectionDisabled.Size = new System.Drawing.Size(144, 16);
            this.lblTNDConnectionDisabled.TabIndex = 19;
            this.lblTNDConnectionDisabled.Text = " T&&D Connection Disabled ";
            this.lblTNDConnectionDisabled.Visible = false;
            // 
            // tbStatus
            // 
            this.tbStatus.AcceptsReturn = true;
            this.tbStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbStatus.Location = new System.Drawing.Point(0, 0);
            this.tbStatus.Multiline = true;
            this.tbStatus.Name = "tbStatus";
            this.tbStatus.ReadOnly = true;
            this.tbStatus.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbStatus.Size = new System.Drawing.Size(797, 104);
            this.tbStatus.TabIndex = 17;
            this.tbStatus.WordWrap = false;
            this.tbStatus.Click += new System.EventHandler(this.tbStatus_Click);
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 104);
            this.statusBar.Name = "statusBar";
            this.statusBar.ShowPanels = true;
            this.statusBar.Size = new System.Drawing.Size(797, 16);
            this.statusBar.TabIndex = 0;
            this.statusBar.Text = "statusBar";
            // 
            // statusSplitter
            // 
            this.statusSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusSplitter.Location = new System.Drawing.Point(3, 473);
            this.statusSplitter.Name = "statusSplitter";
            this.statusSplitter.Size = new System.Drawing.Size(797, 8);
            this.statusSplitter.TabIndex = 16;
            this.statusSplitter.TabStop = false;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(203, 28);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(8, 445);
            this.splitter1.TabIndex = 17;
            this.splitter1.TabStop = false;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Right;
            this.propertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.propertyGrid1.Location = new System.Drawing.Point(570, 28);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(230, 445);
            this.propertyGrid1.TabIndex = 22;
            // 
            // propertySplitter
            // 
            this.propertySplitter.Dock = System.Windows.Forms.DockStyle.Right;
            this.propertySplitter.Location = new System.Drawing.Point(561, 28);
            this.propertySplitter.Name = "propertySplitter";
            this.propertySplitter.Size = new System.Drawing.Size(9, 445);
            this.propertySplitter.TabIndex = 20;
            this.propertySplitter.TabStop = false;
            // 
            // toolGrid
            // 
            this.toolGrid.AllowDrop = true;
            this.toolGrid.AutoStretchColumnsToFitWidth = true;
            this.toolGrid.AutoStretchRowsToFitHeight = true;
            this.toolGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.toolGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolGrid.GridToolTipActive = true;
            this.toolGrid.Location = new System.Drawing.Point(211, 28);
            this.toolGrid.Name = "toolGrid";
            this.toolGrid.Size = new System.Drawing.Size(350, 445);
            this.toolGrid.SpecialKeys = ((SourceGrid3.GridSpecialKeys)(((((((SourceGrid3.GridSpecialKeys.Arrows | SourceGrid3.GridSpecialKeys.Tab)
                        | SourceGrid3.GridSpecialKeys.PageDownUp)
                        | SourceGrid3.GridSpecialKeys.Enter)
                        | SourceGrid3.GridSpecialKeys.Escape)
                        | SourceGrid3.GridSpecialKeys.Control)
                        | SourceGrid3.GridSpecialKeys.Shift)));
            this.toolGrid.StyleGrid = null;
            this.toolGrid.TabIndex = 21;
            this.toolGrid.DragOver += new SourceGrid3.GridDragEventHandler(this.toolGrid_DragOver);
            this.toolGrid.DragEnter += new SourceGrid3.GridDragEventHandler(this.toolGrid_DragEnter);
            this.toolGrid.DragDrop += new SourceGrid3.GridDragEventHandler(this.toolGrid_DragDrop);
            this.toolGrid.DragLeave += new SourceGrid3.GridEventHandler(this.toolGrid_DragLeave);
            // 
            // ftpConnection
            // 
            this.ftpConnection.EventsEnabled = true;
            this.ftpConnection.ParentControl = this;
            this.ftpConnection.TransferNotifyInterval = ((long)(4096));
            this.ftpConnection.UseGuiThreadIfAvailable = true;
            // 
            // ProgrammingForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(800, 601);
            this.Controls.Add(this.toolGrid);
            this.Controls.Add(this.propertySplitter);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.statusSplitter);
            this.Controls.Add(this.statusPanel);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.toolBar);
            this.Menu = this.mainMenu1;
            this.Name = "ProgrammingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "UVision";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ProgrammingForm_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProgrammingForm_FormClosing);
            this.Layout += new LayoutEventHandler(ProgrammingForm_Layout);
            this.Load += new System.EventHandler(this.ProgrammingForm_Load);
            this.statusPanel.ResumeLayout(false);
            this.statusPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
            CheckForIllegalCrossThreadCalls = false; //DWW_SPLASH_HACK
            SplashScreen.ShowSplashScreen();
            Application.DoEvents();
            SplashScreen.SetStatus("Starting up...");
            Application.Run(new ProgrammingForm());
		}

        private string ftpServer = "SECRET-NOT_IN_GITHUB";
        private void ProgrammingForm_Load(object sender, System.EventArgs e)
        {
            try
            {
                this.Top = (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2;

                Project = new Project(this);

                // Read setting from command line
                if (Environment.GetCommandLineArgs().GetUpperBound(0) > 0)
                {
                    Project.loadSettings(Environment.GetCommandLineArgs()[1]);
                }
                // Read settings from application folder
                else if (System.IO.File.Exists(settingsFileName))
                {
                    Project.loadSettings(settingsFileName);
                }

                MemoryStream ms = new MemoryStream();
                StreamWriter sw = new StreamWriter(ms);
                sw.WriteLine(ProgramVersion());
                sw.WriteLine(ProgramBuildDate());
                sw.WriteLine(DateTime.Now);
                sw.WriteLine(System.Environment.MachineName);
                try
                {
                    String strHostName = Dns.GetHostName();
                    sw.WriteLine(strHostName);
                    IPHostEntry iphostentry = Dns.GetHostByName(strHostName);
                    foreach (IPAddress ipaddress in iphostentry.AddressList)
                    {
                        sw.Write(ipaddress.ToString() + "   ");
                    }
                }
                catch (Exception ex)
                {
                }
                sw.WriteLine();
                sw.WriteLine(System.Environment.OSVersion);
                sw.WriteLine(System.Environment.ProcessorCount);
                sw.WriteLine(System.Environment.UserName);
                sw.WriteLine(System.Environment.CurrentDirectory);
                sw.WriteLine(System.Environment.WorkingSet);
                sw.WriteLine(Project.Name);
                foreach (TestSequence testSeq in Project.mTestSequences)
                {
                    sw.Write(testSeq.Name + "   ");
                }
                sw.WriteLine();
                foreach (string camName in Project.CameraOptions())
                {
                    sw.Write(camName + "   ");
                }
                sw.WriteLine();
                sw.Flush();
                try
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    // TODO: switch from FTPConnection to FTPClient class
                    ftpConnection.ServerAddress = ftpServer;
                    ftpConnection.UserName = "SECRET-NOT_IN_GITHUB";
                    ftpConnection.Password = "SECRET-NOT_IN_GITHUB";
                    ftpConnection.ConnectMode = EnterpriseDT.Net.Ftp.FTPConnectMode.PASV;
                    ftpConnection.Connect();
                    ftpConnection.TransferType = EnterpriseDT.Net.Ftp.FTPTransferType.BINARY;
                    ftpConnection.UploadStream(ms, Project.Name.Replace(' ','_') + "__" + System.Environment.MachineName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                    ftpConnection.Close();
                }
                catch (Exception ex)
                {
                    Project.LogWarning("Starting up...");
                }

                //            Project.SimulateLoadingProjectFromConfigFile();

                miSave.Enabled = true;

                //myTNDLink.ConnectToRuntime(TNDLink_old.MACHINE_NAME, false); //OLDTND
                //TNDPollTimer.Interval = 50; //OLDTND
                //TNDPollTimer.Enabled = false; //OLDTND
                //logMessage("started poll timer"); //OLDTND
                projectNode.Expand();
                camerasNode.Expand();
                sequencesNode.Expand();

                //if (DateTime.Today.Month == 9)// || DateTime.Today.Month > 10)
                /*
                if (DateTime.Today.Year > 2009)
                {
                    ShutdownApp(false);
                }
                */

                //                Project.globalTNDReader.Connected = true;//zzz
                //                Project.globalTNDWriter.Connected = true;//zzz

                if (Project.mTestSequences.Count == 1)
                {
                    // if there is only 1 test sequence, then select it as a time saver
                    Project.SelectedTestSequence = (TestSequence)Project.mTestSequences[0];
                    //TODO: rebuild grid here?
                }
                else
                {
                    // if there is more than one sequence, we make sure none look selected...
                    // HACK: empty out toolGrid...it gets built up when the Test Sequences are loaded from config files.  The code that sets/fixes Tool Grid rows and columns for each object uses the grid to manage corrections...currently we only have one instance of the tool grid
                    toolGrid.RowsCount = 0;
                    toolGrid.ColumnsCount = 0;
                }
                SplashScreen.SetStatus("Start up complete.");
                this.Activate();
                SplashScreen.CloseForm();
                //Thread.Sleep(1000); //DWW_SPLASH_HACK (we don't want to turn on the CheckForIllegalCrossThreadCalls until the SplashScreen is completely gone...after it fades out
                CheckForIllegalCrossThreadCalls = true; //DWW_SPLASH_HACK
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting up.  Message=" + ex.Message);
                ShutdownApp(false);
            }
        }



		private void miLoad_Click(object sender, System.EventArgs e)
		{
            /*
			openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
			openFileDialog1.ShowDialog();
			if (openFileDialog1.FileName.Trim().Length > 0)
			{
				if (System.IO.File.Exists(openFileDialog1.FileName))
				{
					// Close all opened cameraForms
					if ((appsettings!=null) && (appsettings.cameras != null))
						foreach (NetworkCamera_old cam in appsettings.cameras)
							if (cam.cameraForm != null)
								cam.cameraForm.Close();
					treeView1.Nodes.Clear();
					camerasNode.Nodes.Clear();
					treeView1.Nodes.Add(camerasNode);
                    treeView1.Nodes.Add(sequencesNode);
					Project.loadSettings(openFileDialog1.FileName);
				}
			}
            */
		}

		private void miSave_Click(object sender, System.EventArgs e)
		{
			Project.saveSettings();
		}

		private void treeView1_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
            propertyGrid1.SelectedObject = null;

			// context menu on nodes:http://msdn2.microsoft.com/en-gb/library/ms171707.aspx

			//
			// This function's main concern is to find:
			// currentCamera, currentROI
			// It also initiates the blinking effect
			//

			// Make sure that a camera is selected

            if (treeView1.SelectedNode == projectNode) propertyGrid1.SelectedObject = Project;

            // Get camera node
			TreeNode camNode = treeView1.SelectedNode;
			while (camNode.Parent != null && camNode.Parent.Text != "Cameras")
				camNode = camNode.Parent;

			// Store the current camera index to be used in further processing
			if( camNode.Parent != null && camNode.Parent.Text == "Cameras" )
			{
				currentCamera = camNode;
				//enableCameraControls(true);
				propertyGrid1.SelectedObject = Project.FindCamera(currentCamera);

				TreeNode roiNode = treeView1.SelectedNode;
				while (roiNode.Parent != null && roiNode.Parent != currentCamera && roiNode.Parent.Text != "ROIs")
					roiNode = roiNode.Parent;

			}
			else
			{
				//enableCameraControls(false);
				currentCamera = null;
				currentROI = null;
			}

            TestSequence previouslySelectedSequence = Project.SelectedTestSequence;
            if (treeView1.SelectedNode.Tag is TestSequence)
            {
                Project.SelectedTestSequence = (TestSequence)treeView1.SelectedNode.Tag;
                propertyGrid1.SelectedObject = treeView1.SelectedNode.Tag;
            }
            else if (treeView1.SelectedNode.Tag is TestExecutionCollection)
            {
                TestExecutionCollection selectedCollection = (TestExecutionCollection)treeView1.SelectedNode.Tag;
                Project.SelectedTestSequence = selectedCollection.TestSequence;
                propertyGrid1.SelectedObject = treeView1.SelectedNode.Tag;
            }

            if (treeView1.SelectedNode == readerNode) propertyGrid1.SelectedObject = Project.globalTNDReader;
            if (treeView1.SelectedNode == writerNode) propertyGrid1.SelectedObject = Project.globalTNDWriter;

            if ( Project.SelectedTestSequence != previouslySelectedSequence)
            {
                Project.SelectedTestSequence.RebuildToolGrid();
            }
		}

        public void AddTreeNode(TreeNode parentNode, TreeNode childNode)
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new TreeNodeDelegate(AddTreeNode), new object[] { parentNode, childNode });
                return;
            }
            parentNode.Nodes.Add(childNode);
            if (parentNode.TreeView != null) parentNode.TreeView.Refresh();
        }

        public void RemoveTreeNode(TreeNode parentNode, TreeNode childNode)
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new TreeNodeDelegate(RemoveTreeNode), new object[] { parentNode, childNode });
                return;
            }
            parentNode.Nodes.Remove(childNode);
            if (parentNode.TreeView != null) parentNode.TreeView.Refresh();
        }
        delegate void TreeNodeTextDelegate(Form form, TreeNode node, String text);
        public void SetTreeNodeText(Form form, TreeNode node, String text)
        {
            if (form.InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                form.BeginInvoke(new TreeNodeTextDelegate(SetTreeNodeText), new object[] { form, node, text });
                return;
            }
            node.Text = text;
            if (node.TreeView != null) node.TreeView.Refresh();
        }


		private void treeView1_DoubleClick(object sender, System.EventArgs e)
		{			
            if( treeView1.SelectedNode.Tag is TestExecutionCollection )
            {
                TestExecutionCollection collection = (TestExecutionCollection)treeView1.SelectedNode.Tag;
                if (collection.OperationForm == null)
                {
                    collection.OperationForm = new OperationForm(collection);
                }
                collection.OperationForm.Show();
                collection.OperationForm.BringToFront();
                return;
            }
		}

        private StreamWriter log = new StreamWriter("UVision.log", true);
		public void logMessage(string Message)
		{
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new StringDelegate(logMessage), new object[] { Message });
                return;
            }
            string prefix = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " : ";
            tbStatus.Text += prefix + Message + Environment.NewLine;
			tbStatus.SelectionStart = tbStatus.Text.Length;
			tbStatus.SelectionLength = 0;
			if( miAutoScrollLog.Checked ) tbStatus.ScrollToCaret();
            log.WriteLine(prefix + Message);
		}

        public void logMessageWithFlush(string Message)
        {
            logMessage(Message);
            log.Flush();
        }

		private void treeView1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// This function is used to handle the right click on the treeview
			if (e.Button == MouseButtons.Right)
			{
				treeView1.SelectedNode = treeView1.GetNodeAt(e.X, e.Y);
			}
		}

		private void toolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button.Equals(tbbRebuildGrid))
			{
                if (Project.SelectedTestSequence == null)
                {
                    toolGrid.RowsCount = 0;
                    toolGrid.ColumnsCount = 0;
                    return;
                }
				Project.SelectedTestSequence.RebuildToolGrid();
			}
		}

        public Version ProgramVersion() 
        {
            Assembly assem = Assembly.GetExecutingAssembly();
            return assem.GetName().Version;
        }
        public DateTime ProgramBuildDate()
        {
            //http://dotnetfreak.co.uk/blog/archive/2004/07/08/146.aspx
            //http://forums.msdn.microsoft.com/en-US/csharpgeneral/thread/b55e0fa8-09c6-4692-850a-4c3f7c30f218
            Version vers = ProgramVersion();
            DateTime buildDate = new DateTime(2000, 1, 1).AddDays(vers.Build).AddSeconds(vers.Revision * 2);
            if (TimeZone.CurrentTimeZone.IsDaylightSavingTime(DateTime.Now)) buildDate = buildDate.AddHours(1);
            return buildDate;
        }
		private void miAbout_Click(object sender, System.EventArgs e)
		{
            //http://dotnetfreak.co.uk/blog/archive/2004/07/08/146.aspx
            //http://forums.msdn.microsoft.com/en-US/csharpgeneral/thread/b55e0fa8-09c6-4692-850a-4c3f7c30f218
            Version vers = ProgramVersion();
            DateTime buildDate = ProgramBuildDate();
            MessageBox.Show(this, "           UVision prototype version " + vers + " (" + buildDate + ")  Copyright 2007-2009 Userwise Solutions           " +
                "\n\n           UVision is licensed per-camera from Userwise Solutions.  If you add a camera or install on a new machine,           " +
                "\n           contact Matt Weeda (Innotec) or Dave Wieringa (davew@userwise.com) for licensing details.", "About", MessageBoxButtons.OK);
		}

		private void miHelp_Click(object sender, System.EventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start("access.chm");
			}
			catch
			{
				MessageBox.Show("Error openning help file");
			}
		}


		private void tndLink_EstablishedConnection(TNDLink theLink) //'DWW
		{
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new TNDLink.TNDLinkEstablishedDelegate(tndLink_EstablishedConnection), new object[] { theLink });
                return;
            }
            if (lblLostTNDConnection.Visible || lblTNDConnectionDisabled.Visible)
			{
				logMessage("Established connection to Think & Do"); //' (" & debugCode & ")"
			}
			lblLostTNDConnection.Visible = false;
		}

        private void tndLink_LostConnection(TNDLink theLink) //'DWW
		{
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new TNDLink.TNDLinkLostDelegate(tndLink_LostConnection), new object[] { theLink });
                return;
            }
            if (!lblLostTNDConnection.Visible)
			{
				logMessage("WARNING: Lost connection to Think & Do"); // ' (" & debugCode & ")"
			}
			lblLostTNDConnection.Visible = true;
		}

        private void tndLink_ConnectionDisabled(TNDLink theLink) //'DWW
		{
			myTNDLink.Connected = false;
			lblTNDConnectionDisabled.Visible = true;
            //TNDPollTimer.Enabled = false;//OLDTND 
		}

        /* //OLDTND 
		private void TNDPollTimer_Tick(object sender, System.EventArgs e) //'DWW
		{
			foreach (NetworkCamera cam in appsettings.cameras)
			{
				if (cam.cameraForm == null)
					cam.cameraForm = new CameraForm(cam, this);
				cam.cameraForm.queryTND();
			}
		}// */

		private void miClearLogWindow_Click(object sender, System.EventArgs e)
		{
			tbStatus.Text = "";
		}

		private void miAutoScrollLog_Click(object sender, System.EventArgs e)
		{
			miAutoScrollLog.Checked = !miAutoScrollLog.Checked;
		}


		/*
		private void FormAdded(Form form) //'DWW
		{
			myCameraForms.Add(form);
			form.Disposed += new EventHandler(form_Disposed);
		}
		void form_Disposed(object sender, EventArgs e) //'DWW
		{
			myCameraForms.Remove(sender);
		}

		private Form[] GetFormsOfType(Type t)  //'DWW // Helper method to get all forms of given type
		{
			ArrayList ar = new ArrayList();
			foreach (Form f in myCameraForms)
			{
				if (f.GetType() == t)
					ar.Add(f);
			}
			return (Form[])ar.ToArray(typeof(Form));
		}
		*/
		public class CellClickEvent : SourceGrid3.Cells.Controllers.ControllerBase
		{
			private PropertyGrid mPropertyGrid;
			public CellClickEvent(PropertyGrid propertyGrid)
			{
				mPropertyGrid = propertyGrid;
			}
			public override void OnClick(SourceGrid3.CellContext sender, EventArgs e)
			{
				base.OnClick (sender, e);

				mPropertyGrid.SelectedObject = ((SourceGrid3.Cells.Real.Cell)sender.Grid.GetCell(sender.Position)).Value;
                ((IObjectDefinition)mPropertyGrid.SelectedObject).TestSequence().RebuildToolGrid(); // update which object is selected...yet a hack
			}
		}

        void toolGrid_DragEnter(SourceGrid3.GridVirtual sender, DragEventArgs e)
        {
            statusBar.Text = "Dragged in to " + toolGrid.DragCellPosition;
        }

        private void toolGrid_DragLeave(SourceGrid3.GridVirtual sender, EventArgs e)
        {
            statusBar.Text = "Dragged out from " + toolGrid.DragCellPosition;
        }

        private void toolGrid_DragOver(SourceGrid3.GridVirtual sender, DragEventArgs e)
        {
            statusBar.Text = "Dragged over " + toolGrid.DragCellPosition;
            SourceGrid3.Cells.Real.Cell cell = (SourceGrid3.Cells.Real.Cell)toolGrid[toolGrid.DragCellPosition.Row, toolGrid.DragCellPosition.Column];

            if (cell != null && cell.Value != null)
            {
                ((IObjectDefinition)cell.Value).VerifyValidItemsForDrop(sender, e);
                return;
            }
            e.Effect = DragDropEffects.None;
        }

        void toolGrid_DragDrop(SourceGrid3.GridVirtual sender, DragEventArgs e)
        {
            //http://www.jonasjohn.de/snippets/csharp/drag-and-drop-example.htm
            SourceGrid3.Cells.Real.Cell cell = (SourceGrid3.Cells.Real.Cell)toolGrid[toolGrid.DragCellPosition.Row, toolGrid.DragCellPosition.Column];

            if (cell != null && cell.Value != null )
            {
                ((IObjectDefinition)cell.Value).HandleDrop(sender, e);
            }
        }
	
		protected override void OnResize(EventArgs e)
		{
			// TODO:  Add ProgrammingForm.OnResize implementation
			base.OnResize (e);
		}

        private void miLoadSeq_Click(object sender, EventArgs e)
        {
//            Project.SoleTestSequence().LoadDefinition();
        }

        private void miSaveSeq_Click(object sender, EventArgs e)
        {
            Project.SelectedTestSequence.SaveDefinition();
        }

        private void miSimulateLoad_Click(object sender, EventArgs e)
        {
            Project.SimulateLoadingProjectFromConfigFile(Project.SelectedTestSequence);
        }

        private void lblLostTNDConnection_Click(object sender, EventArgs e)
        {
            myTNDLink.mReconnectTimer.Enabled = false;
            myTNDLink.ConnectToRuntime(TNDLink_old.MACHINE_NAME, false);
        }

        public void ShutdownApp(bool requestedByOperator)
        {
            bool problemWithSimpleShutdownStuff = false;
            try
            {
                shuttingDown = true;
                if (requestedByOperator)
                {
                    logMessage("Shutting down by operator");
                }
                else
                {
                    logMessage("Shutting down");
                }
            }
            catch (Exception e)
            {
                problemWithSimpleShutdownStuff = true;
                MessageBox.Show("Exception during initial shutdown sequence.");
            }

            try
            {
                /*
                foreach (TestSequence sequence in Project.mTestSequences)
                {
                    MessageBox.Show("pre abort state: " + sequence.mThread.ThreadState + " " + sequence.mThread.IsAlive); // TO DO: make sure we cleanly dispose of objects and stop all threads.
                }
                 */
                foreach (TestSequence sequence in Project.mTestSequences)
                {
                    sequence.mThread.Abort(); // TO DO: make sure we cleanly dispose of objects and stop all threads.
                }
                bool foundSomeAlive = false;
                long waitLoops = 0;
                do
	            {
                    Thread.Sleep(10);
                    foundSomeAlive = false;
                    foreach (TestSequence sequence in Project.mTestSequences)
                    {
                        foundSomeAlive |= sequence.mThread.IsAlive;
                    }
                    waitLoops++;        	         
	            } while (foundSomeAlive && waitLoops < 100);
                if (waitLoops >= 100 && !problemWithSimpleShutdownStuff)
                {
                    logMessageWithFlush("ERROR: Test Sequence threads not aborting in a timely manner.");
                }
                /*
                foreach (TestSequence sequence in Project.mTestSequences)
                {
                    MessageBox.Show("post abort state: " + sequence.mThread.ThreadState + " " + sequence.mThread.IsAlive); // TO DO: make sure we cleanly dispose of objects and stop all threads.
                }
                foreach (TestSequence sequence in Project.mTestSequences)
                {
                    MessageBox.Show("post abort 2 state: " + sequence.mThread.ThreadState + " " + sequence.mThread.IsAlive); // TO DO: make sure we cleanly dispose of objects and stop all threads.
                }
                 */
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception while aborting threads.");
            }

            try
            {
                if (Project.globalTNDReader != null && Project.globalTNDReader.ConnectionInitialized)
                {
                    Project.globalTNDReader.Connected = false;
                    Project.globalTNDReader.AutoReconnectEnabled = false;
                }
                if (Project.globalTNDWriter != null && Project.globalTNDWriter.ConnectionInitialized)
                {
                    Project.globalTNDWriter.Connected = false;
                    Project.globalTNDWriter.AutoReconnectEnabled = false;
                }
            }
            catch (Exception e)
            {
                string msg = "ERROR: Exception while shutting down Think & Do link.";
                if(problemWithSimpleShutdownStuff)
                {
                    MessageBox.Show(msg);
                }
                else
                {
                    logMessageWithFlush(msg);
                }
            }

            try
            {
                if (LocalMach3.Singleton.Connected || LocalMach3.Singleton.LostConnection)
                {
                    LocalMach3.Singleton.Disconnect();
                }
            }
            catch (Exception e)
            {
                string msg = "ERROR: Exception while shutting down Mach3 link.";
                if(problemWithSimpleShutdownStuff)
                {
                    MessageBox.Show(msg);
                }
                else
                {
                    logMessageWithFlush(msg);
                }
            }

            try
            {
                Project.CleanupForShutdown();
            }
            catch (Exception e)
            {
                string msg = "ERROR: Exception shutting down Project.  Message='" + e.Message + "'";
                if (problemWithSimpleShutdownStuff)
                {
                    MessageBox.Show(msg);
                }
                else
                {
                    logMessageWithFlush(msg);
                }
            }

            try
            {
                foreach (TestSequence seq in Project.mTestSequences)
                {
                    seq.CleanupForShutdown();
                }
            }
            catch (Exception e)
            {
                string msg = "ERROR: Exception shutting down Test Sequences.  Message='" + e.Message + "'";
                if (problemWithSimpleShutdownStuff)
                {
                    MessageBox.Show(msg);
                }
                else
                {
                    logMessageWithFlush(msg);
                }
            }


            try
            {
                log.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception closing log");
            }

            try
            {
                this.Close();
            }
            catch (Exception e)
            {

            }
        }

        public bool shuttingDown = false;
        private bool VerifyShutdownDesired()
        {
            if (shuttingDown) return true;
            logMessageWithFlush("Operator requested shutdown");

            bool foundUnsavedChanges = false;
            foreach (TestSequence seq in Project.mTestSequences)
            {
                if (seq.HasUnsavedChanges())
                {
                    logMessage("Warning: Unsaved changes in sequence '" + seq.Name + "'");
                    foundUnsavedChanges = true;
                    //break;
                }
            }

            if (foundUnsavedChanges)
            {
                if (MessageBox.Show("You have unsaved changes.  Are you sure you want to shut down without first saving them?", "Close Application with Unsaved Changes", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    logMessageWithFlush("Operator cancelled shutdown due to unsaved changes");
                    return false;
                }
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to stop the vision system?", "Close Application", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    logMessageWithFlush("Operator cancelled shutdown");
                    return false;
                }
            }

            shuttingDown = true;
            return true;
        }

        private void miExit_Click(object sender, System.EventArgs e)
        {
            if (VerifyShutdownDesired())
            {
                ShutdownApp(true);
            }
        }

        private void ProgrammingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // NOTE: this could be called either by the user clicking the X button in the upper right corner of the window OR by ShutdownApp() invoking this.Close()
            if (!shuttingDown)
            {
                e.Cancel = !VerifyShutdownDesired();

                if (!e.Cancel)
                {
                    ShutdownApp(true);
                }
            }
        }

        private void ProgrammingForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // resources should already be cleaned up by this point
        }

        private void tbStatus_Click(object sender, EventArgs e)
        {
            log.Flush(); // bit of a hack. we should do this on a timed interval or something
        }

        private bool m_bLayoutCalled = false;
        private void ProgrammingForm_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if (m_bLayoutCalled == false)
            {
                m_bLayoutCalled = true;
                //m_dt = DateTime.Now;

                if (SplashScreen.SplashForm != null)
                    SplashScreen.SplashForm.Owner = this;
                //SplashScreen.SetOwner(this); This was my attempt to accomplish setting the owner with invoke, but it locked up...so I just went back to ignoring the cross thread checks for now...yuck

                // MOVED THIS CODE BECAUSE HERE IT WAS CLOSING SPLASH SCREEN BEFORE CONFIG FILE LOADS
                //this.Activate();
                //SplashScreen.CloseForm_originalWay();
                //timer1.Start();
            }
        }

	}
}

