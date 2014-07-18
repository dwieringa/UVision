// Copyright (c) 2004-2008 Userwise Solutions LLC.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace NetCams
{
    public abstract class TNDLink
    {
        // http://www.dotnetfun.com/articles/csharp/CreatingEventsCSharp.aspx
        public delegate void TNDLinkEstablishedDelegate(TNDLink theLink); //'DWW
        public delegate void TNDLinkLostDelegate(TNDLink theLink); //'DWW
        public delegate void TNDLinkDisabledDelegate(TNDLink theLink); //'DWW

        public event TNDLinkEstablishedDelegate TNDLinkEstablished;
        public event TNDLinkLostDelegate TNDLinkLost;
        public event TNDLinkDisabledDelegate TNDLinkDisabled;

        public abstract string Name { get; set; }
        public abstract bool Connected { get; set; }

        protected void FireTNDLinkEstablished()
        {
            if (TNDLinkEstablished != null)
            {
                TNDLinkEstablished(this);
            }
        }
        protected void FireTNDLinkLost()
        {
            if (TNDLinkLost != null)
            {
                TNDLinkLost(this);
            }
        }
        protected void FireTNDLinkDisabled()
        {
            if (TNDLinkDisabled != null)
            {
                TNDLinkDisabled(this);
            }
        }

        public enum TNDDataTypeEnum
        {
            Undefined = 0,
            Input = 1,
            Flag = 2,
            Timer = 3,
            Output = 4,
            Counter = 5,
            Register = 6,
            Number = 7,
            ASCII = 8,
            Float = 19,
            System = 20,
            String = 22,
            Array = 21,
            Byte = 25
        }
        public enum TNDShortDataTypeEnum
        {
            Undefined = 0,
            Counter = 5
        }
        public enum TNDBoolDataTypeEnum
        {
            Undefined = 0,
            Input = 1,
            Flag = 2,
            Output = 4
        }

        public const int ThinkAndDoSuccess = 1;
        public const int ThinkAndDoOperationFailed = 0;
        public const int ThinkAndDoNoDataTable = -1;
        public const int ThinkAndDoTagNotFound = -2;
        public const int ThinkAndDoNoProjectLoaded = -3;
        public const int ThinkAndDoCOMFailure = -4;
        public const int ThinkAndDoException = -5;
        public const int ThinkAndDoInvalidVariant = -6;
        public const int ThinkAndDoProjectLocked = -25;
        public const int ThinkAndDoStringOverFlow = -27;
        public const int ThinkAndDoTagQualityNotOK = -30;

        public static string GetTNDResultMessage(int resultCode)
        {
            if (resultCode == ThinkAndDoSuccess)
            {
                return "Success";
            }
            else if (resultCode == ThinkAndDoOperationFailed)
            {
                return "Operation Failed";
            }
            else if (resultCode == ThinkAndDoNoDataTable)
            {
                return "No Data Table";
            }
            else if (resultCode == ThinkAndDoTagNotFound)
            {
                return "Tag Not Found";
            }
            else if (resultCode == ThinkAndDoNoProjectLoaded)
            {
                return "No Project Loaded";
            }
            else if (resultCode == ThinkAndDoCOMFailure)
            {
                return "COM Failure";
            }
            else if (resultCode == ThinkAndDoException)
            {
                return "Think & DO Exception";
            }
            else if (resultCode == ThinkAndDoInvalidVariant)
            {
                return "Invalid Variant";
            }
            else if (resultCode == ThinkAndDoProjectLocked)
            {
                return "Project Locked";
            }
            else if (resultCode == ThinkAndDoStringOverFlow)
            {
                return "String Overflow";
            }
            else if (resultCode == ThinkAndDoTagQualityNotOK)
            {
                return "Tag Quality Not OK";
            }
            else
            {
                return "<undocumented>";
            }
        }
    }

}
