// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace NetCams
{
    public class LocalMach3
    {
        public static readonly LocalMach3 Singleton = new LocalMach3();

        private LocalMach3()
        {
        }

        // TODO:when we loose the connection due to an SEHException during a call, should we Disconnect before we re-Connect?

        public void Connect()
        {
            if (!System.IO.File.Exists("Mach3ULink.dll"))
            {
                throw new ArgumentException("Mach3 link not fully installed.  Connection cannot be established.");
            }

            // Connect() will be called if the connection was lost, so first we attempt a disconnect to ensure we clean things up properly
            if (mConnected || mLostConnection) Disconnect();

            bool success = false;
            try
            {
                success = Mach3ULink.Mach3ObjectModelStartup();
            }
            catch (SEHException ex)
            {
                mConnected = false;
                throw new ArgumentException("Unable to connect to Mach3 (code 8347)");
            }
            catch (Exception ex)
            {
                mConnected = false;
                throw new ArgumentException("Unexpected exception encountered while connecting to Mach3.  Message=" + ex.Message);
            }

            if (!success) throw new ArgumentException("Unable to connect to Mach3 (code 8358)");

            mConnected = true;
            mLostConnection = false; // consider the connection healthy
        }

        public void Disconnect()
        {
            try
            {
                Mach3ULink.Mach3ObjectModelShutdown();
            }
            catch (SEHException ex)
            {
                mLostConnection = true;
                throw new ArgumentException("Exception encountered while disconnecting from Mach3.");
            }
            catch (Exception ex)
            {
                mLostConnection = true;
                throw new ArgumentException("Unexpected exception encountered while disconnecting from Mach3.  Message=" + ex.Message);
            }
            mConnected = false;
            mLostConnection = false;
        }

        private bool mConnected = false;
        public bool Connected
        {
            get { return mConnected; }
            set
            {
                if (mConnected && !value)
                {
                    Disconnect();
                }
                else if (mLostConnection || (!mConnected && value))
                {
                    Connect();
                }
            }
        }

        private bool mLostConnection = false;
        public bool LostConnection
        {
            get { return mLostConnection; }
        }

        public bool LoadGCodeFile(string filePath)
        {
            if (!Connected || mLostConnection) { Connect(); }

            bool result = false;
            try
            {
                result = Mach3ULink.LoadGcodeFile(filePath);
            }
            catch (SEHException ex)
            {
                mLostConnection = true;
                throw new ArgumentException("Lost connection to Mach3");
            }
            catch (Exception ex)
            {
                mLostConnection = true;
                throw new ArgumentException("Unexpected exception encountered talking with Mach3.  Message=" + ex.Message);
            }
            return result;
        }

        public void Reset()
        {
            if (!Connected || mLostConnection) { Connect(); }

            try
            {
                Mach3ULink.Reset();
            }
            catch (SEHException ex)
            {
                mLostConnection = true;
                throw new ArgumentException("Lost connection to Mach3");
            }
            catch (Exception ex)
            {
                mLostConnection = true;
                throw new ArgumentException("Unexpected exception encountered talking with Mach3.  Message=" + ex.Message);
            }
        }

        public void CycleStart()
        {
            if (!Connected || mLostConnection) { Connect(); }

            try
            {
                Mach3ULink.CycleStart();
            }
            catch (SEHException ex)
            {
                mLostConnection = true;
                throw new ArgumentException("Lost connection to Mach3");
            }
            catch (Exception ex)
            {
                mLostConnection = true;
                throw new ArgumentException("Unexpected exception encountered talking with Mach3.  Message=" + ex.Message);
            }
        }

        public void SetOEMDRO(short buttonCode, double value)
        {
            if (!Connected || mLostConnection) { Connect(); }

            try
            {
                Mach3ULink.SetOEMDRO(buttonCode, value);
            }
            catch (SEHException ex)
            {
                mLostConnection = true;
                throw new ArgumentException("Lost connection to Mach3");
            }
            catch (Exception ex)
            {
                mLostConnection = true;
                throw new ArgumentException("Unexpected exception encountered talking with Mach3.  Message=" + ex.Message);
            }
        }
    }
}
