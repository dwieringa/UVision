// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;

namespace NetCams
{
	/// <summary>
	/// 
	/// </summary>
	public class NetCamException : System.Exception
	{
		public NetCamException(string message) : base(message)
		{
			// 
			// TODO: Add constructor logic here
			//
		}
	}

	public class NetCamObjectNotFound : NetCamException
	{
		public NetCamObjectNotFound(string message) : base(message)
		{
			// 
			// TODO: Add constructor logic here
			//
		}
	}

    public class InvalidNetCamOperation : NetCamException
    {
        public InvalidNetCamOperation(string message)
            : base(message)
        {
            // 
            // TODO: Add constructor logic here
            //
        }
    }

}
