// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Forms;
using System.Reflection; //'DWW

namespace NetCams
{
	public class TNDLink_old : TNDLink
	{
		//http://www.developerfusion.co.uk/utilities/convertvbtocsharp.aspx
		//http://www.codeproject.com/dotnet/vbnet_c__difference.asp
		//http://en.csharp-online.net/Value_types

		public TNDLink_old(ProgrammingForm mainForm)
		{
			myForm = mainForm;
            mReconnectTimer = new System.Timers.Timer();
            mReconnectTimer.Enabled = false;
            mReconnectTimer.Interval = 360000;
            mReconnectTimer.Elapsed += new ElapsedEventHandler(mReconnectTimer_Tick);
		}


        public const int InputType = 1;
        public const int FlagType = 2;
        public const int TimerType = 3;
        public const int OutputType = 4;
        public const int CounterType = 5;
        public const int RegisterType = 6;
        public const int NumberType = 7;
        public const int ASCIIType = 8;
        public const int FloatType = 19;
        public const int SystemType = 20;
        public const int StringType = 22;
        public const int ArrayType = 21;
        public const int ByteType = 25;

        private object TND;
        private Type TNDType;
        public const string MACHINE_NAME = "";

		private ProgrammingForm myForm;
        private int mLastCOMErrorDuringConnect = 0;
        private bool mConnected = false;
        public System.Timers.Timer mReconnectTimer;

        private string mName;
        public override string Name  // TODO: make this object of type ProjectDefinition & IProjectInstance
        {
            get { return mName; }
            set { mName = value; }
        }
        
        public override bool Connected
        {
            get { return mConnected; }
            set
            {
                if (value)
                {
                    if (!mConnected) // only try to connect if we aren't connected already
                    {
                        ConnectToRuntime(MACHINE_NAME, false);
                    }
                }
                else
                {
                    mConnected = false;
                }
            }
        }


        void mReconnectTimer_Tick(object sender, EventArgs e)
        {
            mReconnectTimer.Enabled = false;
            ConnectToRuntime(MACHINE_NAME, false);
        }

		public bool ConnectToRuntime(string machineName, bool queryOnErrors)
		{ // TODO: what if multiple threads try to connect at once???
            //throw new ArgumentException("Attempt to use old T&D Link.  Please use new link instead."); //OLDTND
            // if we failed to connect before (reconnect timer running), then only try every second or so...
            if (mReconnectTimer.Enabled) return false;

			bool success = true;
			DialogResult dialogResult;

            TNDType = Type.GetTypeFromProgID("tndrt");
			TND = Activator.CreateInstance(TNDType);

			object[] param=new object[0];
            String projectName = "";
            try
            {
                projectName = (String)TNDType.InvokeMember("GetProjectName", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, TND, param);
            }
            catch (COMException e)
            {
                success = false;
                string logMessage = "";
                switch (e.ErrorCode)
                {
                    case 429:
                        logMessage = "This PC does not appear to have a Think & Do Runtime to link to.";
                        FireTNDLinkDisabled();
                        break;
                    case 462:
                        logMessage = "Unable to connect to the remote T&D machine from here.";
                        break;
                    case 463:
                        logMessage = "This PC does not appear to be properly configured for Think & Do communication.";
                        break;
                    default:
                        logMessage = "UNUSUAL T&D LINK FAILURE DURING CONNECT (runtime error=" + e.ErrorCode + " '" + e.Message + "')";
                        break;
                }
                if (e.ErrorCode != mLastCOMErrorDuringConnect)
                {
                    myForm.logMessage(logMessage);
                    mLastCOMErrorDuringConnect = e.ErrorCode;
                }
            }
            catch (Exception e)
            {
                mLastCOMErrorDuringConnect = 0;
                success = false;
                myForm.logMessage("UNUSUAL FAILURE CONNECTING TO T&D (runtime error='" + e.Message + "')");
            }
            finally
            {
            }
            
            if (!success || projectName.Length == 0) 
			{
				if (queryOnErrors) 
				{
					dialogResult = MessageBox.Show("The Think & Do Runtime isn't running a project." + Environment.NewLine + Environment.NewLine + "Would you like to connect when it starts up?", "Think & Do Runtime not active", MessageBoxButtons.YesNo);
					if (dialogResult == DialogResult.Yes) 
					{
                        FireTNDLinkLost();
					} 
					else 
					{
                        FireTNDLinkDisabled();
					}
				}
				success = false;
			} 
			else 
			{
                FireTNDLinkEstablished();
				success = true;
                mLastCOMErrorDuringConnect = 0;
            }

            if (!success)
            {
                mReconnectTimer.Enabled = true;
                FireTNDLinkLost();
            }
            mConnected = success;
			return success;
		}

        public bool WriteToTNDByTypeIndex(Int16 dataType, Int16 dataItemIndex, Int32 dataValue, string failureMsg)
        { // TODO: thread-safe?

            bool success = true; // good until something goes wrong
            Int16 intTNDCommResult = 0;
            if (!mConnected)
            {
                ConnectToRuntime(MACHINE_NAME, false);
                if (!mConnected) return false;
            }
            // TODO: NotImplemented statement: ICSharpCode.SharpRefactory.Parser.AST.VB.OnErrorStatement
            object[] param = new object[3];
            param[0] = dataType;
            param[1] = dataItemIndex;
            param[2] = dataValue;
            try
            {
                intTNDCommResult = (Int16)TNDType.InvokeMember("WriteDataItemByTypeIndex", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, TND, param);
            }
            catch (COMException e)
            {
                mConnected = false;
                success = false;
                switch (e.ErrorCode)
                {
                    case -2147023170: //VB6 comment: "Automation error <new line> The remote procedure called failed."
                    //VB6 comment: I saw this error after I removed the network cable to this machine...so it lost connection to the remote TND machine.
                    //VB6 comment: Once I had this error, if I performed a Retry, I got a -462 on all subsequent attempts until the cable was reinserted and the connection was restored
                    case -462: //VB6 comment: "The remote server machine does not exist or is not available"
                        //VB6 comment: I saw this error after I attempted a Retry from the -2147023170...without reinserting the network cable.  Once the cable was re-inserted, the connection was restored
                        break;
                    case 462: //VB6 comment: "The remote server machine does not exist or is not available"
                        //VB6 comment: I saw this error after I shutdown the Runtime on the local machine (my notebook).  When I restarted the Runtime, it did NOT recover by simply retrying
                        break;
                    case 91: //VB6 comment: "Object variable or With block variable not set"
                        //VB6 comment: So far, I've only seen this from the Read function...when I enabled the poll timer without a connection established.  Mimicing it here just in case
                        myForm.logMessage("Unable to find T&D Runtime during write operation.");
                        FireTNDLinkDisabled();
                        break;
                    default:
                        myForm.logMessage("UNUSUAL T&D LINK FAILURE DURING WRITE (runtime error=" + e.ErrorCode + " '" + e.Message + "')");
                        break;
                }
            }
            catch (Exception e)
            {
                mConnected = false;
                success = false;
                myForm.logMessage("UNUSUAL FAILURE DURING T&D WRITE (runtime error='" + e.Message + "')");
            }

            if (success && intTNDCommResult == ThinkAndDoSuccess)
            {
                FireTNDLinkEstablished();
            }
            else
            {
                success = false; mConnected = false;
                FireTNDLinkLost();
                if (intTNDCommResult == ThinkAndDoTagNotFound)
                {
                    // TODO: give them a chance to continue without link. warn that link will not be retried
                    MessageBox.Show(failureMsg + Environment.NewLine + "Data item not found in T&D project. This is considered fatal. Shutting down.");
                    System.Environment.Exit(0);
                }
                else if (intTNDCommResult == ThinkAndDoNoDataTable)
                {
                    // You will get this if the Runtime isn't loaded.  So we just want to fail this Read attempt allow for a retry later
                }
                else
                {
                    success = false;
                    myForm.logMessage("write failure: " + intTNDCommResult + " " + GetTNDResultMessage(intTNDCommResult));
                    return success;
                }
            }
            if (!success) Connected = false;
            return success;
        }

        public bool WriteFloatToTNDByIndex(Int16 dataItemIndex, float dataValue, string failureMsg)
        { // TODO: thread-safe?

            bool success = true; // good until something goes wrong
            Int16 intTNDCommResult = 0;
            if (!mConnected)
            {
                ConnectToRuntime(MACHINE_NAME, false);
                if (!mConnected) return false;
            }
            // TODO: NotImplemented statement: ICSharpCode.SharpRefactory.Parser.AST.VB.OnErrorStatement
            object[] param = new object[2];
            param[0] = dataItemIndex;
            param[1] = dataValue;
            try
            {
                intTNDCommResult = (Int16)TNDType.InvokeMember("WriteFloatByIndex", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, TND, param);
            }
            catch (COMException e)
            {
                mConnected = false;
                success = false;
                switch (e.ErrorCode)
                {
                    case -2147023170: //VB6 comment: "Automation error <new line> The remote procedure called failed."
                    //VB6 comment: I saw this error after I removed the network cable to this machine...so it lost connection to the remote TND machine.
                    //VB6 comment: Once I had this error, if I performed a Retry, I got a -462 on all subsequent attempts until the cable was reinserted and the connection was restored
                    case -462: //VB6 comment: "The remote server machine does not exist or is not available"
                        //VB6 comment: I saw this error after I attempted a Retry from the -2147023170...without reinserting the network cable.  Once the cable was re-inserted, the connection was restored
                        break;
                    case 462: //VB6 comment: "The remote server machine does not exist or is not available"
                        //VB6 comment: I saw this error after I shutdown the Runtime on the local machine (my notebook).  When I restarted the Runtime, it did NOT recover by simply retrying
                        break;
                    case 91: //VB6 comment: "Object variable or With block variable not set"
                        //VB6 comment: So far, I've only seen this from the Read function...when I enabled the poll timer without a connection established.  Mimicing it here just in case
                        myForm.logMessage("Unable to find T&D Runtime during write operation.");
                        FireTNDLinkDisabled();
                        break;
                    default:
                        myForm.logMessage("UNUSUAL T&D LINK FAILURE DURING WRITE (runtime error=" + e.ErrorCode + " '" + e.Message + "')");
                        break;
                }
            }
            catch (Exception e)
            {
                mConnected = false;
                success = false;
                myForm.logMessage("UNUSUAL FAILURE DURING T&D WRITE (runtime error='" + e.Message + "')");
            }

            if (success && intTNDCommResult == ThinkAndDoSuccess)
            {
                FireTNDLinkEstablished();
            }
            else
            {
                success = false; mConnected = false;
                FireTNDLinkLost();
                if (intTNDCommResult == ThinkAndDoTagNotFound)
                {
                    // TODO: give them a chance to continue without link. warn that link will not be retried
                    MessageBox.Show(failureMsg + Environment.NewLine + "Data item not found in T&D project. This is considered fatal. Shutting down.");
                    System.Environment.Exit(0);
                }
                else if (intTNDCommResult == ThinkAndDoNoDataTable)
                {
                    // You will get this if the Runtime isn't loaded.  So we just want to fail this Read attempt allow for a retry later
                }
                else
                {
                    success = false;
                    myForm.logMessage("write failure: " + intTNDCommResult + " " + GetTNDResultMessage(intTNDCommResult));
                    return success;
                }
            }
            if (!success) Connected = false;
            return success;
        }

        public bool ReadFromTNDByTypeIndex(Int16 dataType, Int16 dataItemIndex, ref Int32 dataValue, string failureMsg)
		{
			bool success = true; // good until something goes wrong
			int intTNDCommResult = -99999999;
			if (!mConnected) 
			{
				ConnectToRuntime(MACHINE_NAME, false);
                if (!mConnected) return false;
			}
			// TODO: NotImplemented statement: ICSharpCode.SharpRefactory.Parser.AST.VB.OnErrorStatement

			// Create an array containing the arguments.
			object[] param=new object[3];
			param[0] = dataType;
			param[1] = dataItemIndex;
			param[2] = dataValue;

			// Initialize a ParameterModifier with the number of parameters.
			ParameterModifier p = new ParameterModifier(3);

			// Pass the third parameter by reference.
			p[2] = true;

			// The ParameterModifier must be passed as the single element
			// of an array.
			ParameterModifier[] mods = { p };

			// Invoke the method late bound.
            try
            {
                intTNDCommResult = (Int16)TNDType.InvokeMember("ReadDataItemByTypeIndex", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, TND, param, mods, null, null);
                dataValue = Convert.ToInt32(param[2]);
            }
            catch (COMException e)
            {
                success = false; mConnected = false;
                switch (e.ErrorCode)
                {
                    case -2147023169:
                    case -2147417848:
                    case -2147023170: //VB6 comment: "Automation error <new line> The remote procedure called failed."
                    //VB6 comment: I saw this error after I removed the network cable to this machine...so it lost connection to the remote TND machine.
                    //VB6 comment: Once I had this error, if I performed a Retry, I got a -462 on all subsequent attempts until the cable was reinserted and the connection was restored
                    case -462: //VB6 comment: "The remote server machine does not exist or is not available"
                        //VB6 comment: I saw this error after I attempted a Retry from the -2147023170...without reinserting the network cable.  Once the cable was re-inserted, the connection was restored
                        break;
                    case 462: //VB6 comment: "The remote server machine does not exist or is not available"
                        //VB6 comment: I saw this error after I shutdown the Runtime on the local machine (my notebook).  When I restarted the Runtime, it did NOT recover by simply retrying
                        break;
                    case 91: //VB6 comment: "Object variable or With block variable not set"
                        //VB6 comment: I saw this error after I clicked "T&D Connection Disabled" on a machine that didn't have T&D support installed...I saw this when I started the polling timer BEFORE I tried the connect...so the poll function tried to read the link before it was setup.  ...a bug on my part.
                        myForm.logMessage("Unable to find T&D Runtime.");
                        FireTNDLinkDisabled();
                        break;
                    default:
                        myForm.logMessage("UNUSUAL T&D LINK FAILURE (runtime error=" + e.ErrorCode + " '" + e.Message + "')");
                        break;
                }
            }
            catch (Exception e)
            {
                success = false; mConnected = false;
                myForm.logMessage("UNUSUAL FAILURE DURING T&D READ (runtime error='" + e.Message + "')");
            }
//			finally
//			{
//			}
			if (success && intTNDCommResult == ThinkAndDoSuccess) 
			{
                FireTNDLinkEstablished();
			} 
			else 
			{
                success = false; mConnected = false;
                FireTNDLinkLost();
				if (intTNDCommResult == ThinkAndDoTagNotFound) 
				{
					MessageBox.Show(failureMsg + Environment.NewLine + "Data item for test result of front part not found in T&D project. This is considered fatal. Shutting down.");
					System.Environment.Exit(0);
				} 
				else if (intTNDCommResult == ThinkAndDoNoDataTable) 
				{
				} 
				else 
				{
					myForm.logMessage("read failure: " + intTNDCommResult + " " + GetTNDResultMessage(intTNDCommResult));
					return false;
				}
			}
            if (!success) Connected = false;
            return success;
		}

		public bool ReadTNDFloatByIndex(Int16 dataItemIndex, ref float dataValue, string failureMsg)
		{
			bool success = false;
			int intTNDCommResult;
			if (!mConnected) 
			{
				ConnectToRuntime(MACHINE_NAME, false);
                if (!mConnected) return false;
            }

            object[] param=new object[2];
			param[0] = dataItemIndex;
			param[1] = dataValue;
			intTNDCommResult = (Int16)TNDType.InvokeMember("ReadFloatByIndex", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, TND, param );
			if (intTNDCommResult == ThinkAndDoSuccess) 
			{
				success = true;
                FireTNDLinkEstablished();
			} 
			else 
			{
                success = false; mConnected = false;
                FireTNDLinkLost();
				if (intTNDCommResult == ThinkAndDoTagNotFound) 
				{
					MessageBox.Show(failureMsg + Environment.NewLine + "Data item for test result of front part not found in T&D project. This is considered fatal. Shutting down.");
					System.Environment.Exit(0);
				} 
				else if (intTNDCommResult == ThinkAndDoNoDataTable) 
				{
				} 
				else 
				{
					success = false;
					myForm.logMessage("read failure: " + intTNDCommResult + " " + GetTNDResultMessage(intTNDCommResult));
					return success;
				}
			}
            if (!success) Connected = false;
            return success;
		}
	}
}
