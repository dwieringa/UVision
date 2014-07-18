﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.312
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Modbus {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Modbus.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specialized use in conjunction with programming commands. The server (or slave) has accepted the request and is processing it, but a long duration of time will be required to do so. This response is returned to prevent a timeout error from occurring in the client (or master). The client (or master) can next issue a Poll Program Complete message to determine if processing is completed..
        /// </summary>
        internal static string Acknowlege {
            get {
                return ResourceManager.GetString("Acknowlege", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specialized use in conjunction with gateways, indicates that the gateway was unable to allocate an internal communication path from the input port to the output port for processing the request. Usually means that the gateway is misconfigured or overloaded..
        /// </summary>
        internal static string GatewayPathUnavailable {
            get {
                return ResourceManager.GetString("GatewayPathUnavailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specialized use in conjunction with gateways, indicates that no response was obtained from the target device. Usually means that the device is not present on the network..
        /// </summary>
        internal static string GatewayTargetDeviceFailedToRespond {
            get {
                return ResourceManager.GetString("GatewayTargetDeviceFailedToRespond", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The data address received in the query is not an allowable address for the server (or slave). More specifically, the combination of reference number and transfer length is invalid. For a controller with 100 registers, the PDU addresses the first register as 0, and the last one as 99. If a request is submitted with a starting register address of 96 and a quantity of registers of 4, then this request will successfully operate (address-wise at least) on registers 96, 97, 98, 99. If a request is submitted with  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string IllegalDataAddress {
            get {
                return ResourceManager.GetString("IllegalDataAddress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A value contained in the query data field is not an allowable value for server (or slave). This indicates a fault in the structure of the remainder of a complex request, such as that the implied length is incorrect. It specifically does NOT mean that a data item submitted for storage in a register has a value outside the expectation of the application program, since the MODBUS protocol is unaware of the significance of any particular value of any particular register..
        /// </summary>
        internal static string IllegalDataValue {
            get {
                return ResourceManager.GetString("IllegalDataValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The function code received in the query is not an allowable action for the server (or slave). This may be because the function code is only applicable to newer devices, and was not implemented in the unit selected. It could also indicate that the server (or slave) is in the wrong state to process a request of this type, for example because it is unconfigured and is being asked to return register values..
        /// </summary>
        internal static string IllegalFunction {
            get {
                return ResourceManager.GetString("IllegalFunction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specialized use in conjunction with function codes 20 and 21 and reference type 6, to indicate that the extended file area failed to pass a consistency check..
        /// </summary>
        internal static string MemoryParityError {
            get {
                return ResourceManager.GetString("MemoryParityError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specialized use in conjunction with programming commands. The server (or slave) is engaged in processing a long–duration program command. The client (or master) should retransmit the message later when the server (or slave) is free..
        /// </summary>
        internal static string SlaveDeviceBusy {
            get {
                return ResourceManager.GetString("SlaveDeviceBusy", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An unrecoverable error occurred while the server (or slave) was attempting to perform the requested action..
        /// </summary>
        internal static string SlaveDeviceFailure {
            get {
                return ResourceManager.GetString("SlaveDeviceFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown slave exception code..
        /// </summary>
        internal static string Unknown {
            get {
                return ResourceManager.GetString("Unknown", resourceCulture);
            }
        }
    }
}
