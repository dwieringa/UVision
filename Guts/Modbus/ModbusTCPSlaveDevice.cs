// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Modbus;
using Modbus.Data;
using Modbus.Device;
using Modbus.Utility;

namespace NetCams
{
    public enum ModbusModules
    {
        Undefined = 0,
        Coils,
        InputDiscretes,
        InputRegisters,
        HoldingRegisters
    }

    public class ModbusTCPSlaveDevice
    {
        public ModbusTCPSlaveDevice(Project theProject)
        {
            mProject = theProject;

            // TODO: make ID and port properties and "start" object after fully initialized (register as needing "start" after config fully processed?  add a method to all objects which gets called after object is fully config'ed?
            byte slaveID = 1;
            int port = 502;
            //IPAddress address = new IPAddress(new byte[] { 127, 0, 0, 1 });

            // create and start the TCP slave
            TcpListener slaveTcpListener = new TcpListener(port);
            slaveTcpListener.Start();
            mSlave = ModbusTcpSlave.CreateTcp(slaveID, slaveTcpListener);
            mSlave.DataStore = DataStoreFactory.CreateDefaultDataStore();
            Thread slaveThread = new Thread(mSlave.Listen);
            slaveThread.Start();
            theProject.globalModbusSlave = this;
        }

        private ModbusTcpSlave mSlave = null;
        public DataStore DataStore
        {
            get { return mSlave.DataStore; }
        }

        protected Project mProject;
        public Project Project()
        {
            return mProject;
        }

        private string mName;
        public string Name  // TODO: make this object of type ProjectDefinition & IProjectInstance
        {
            get { return mName; }
            set { mName = value; }
        }


    }
}
