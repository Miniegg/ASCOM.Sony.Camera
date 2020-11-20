//tabs=4
// --------------------------------------------------------------------------------
//
// ASCOM Camera driver for DSLR.Sony
//
// Description:	Experimental ASCOM camera driver for Sony DSLR cameras. Relies on Sony Imaging Edge software (Remote app) to be in running stateLorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam 
//
// Implements:	ASCOM Camera interface version: <To be completed by driver developer>
// Author:		Alexey Sobolev <sobolev@aexsoft.com>
//
// Edit Log:
//
// Date			Who	Vers	Description
// -----------	---	-----	-------------------------------------------------------
// dd-mmm-yyyy	XXX	6.0.0	Initial edit, created from ASCOM driver template
// --------------------------------------------------------------------------------
//


// This is used to define code in the template that is specific to one class implementation
// unused code canbe deleted and this definition removed.
#define Camera

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ASCOM.Astrometry.AstroUtils;
using ASCOM.Utilities;
using ASCOM.DeviceInterface;
using System.Globalization;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace ASCOM.Sony
{
    /// <summary>
    /// ASCOM Camera Driver for Sony Cameras
    /// </summary>
    [Guid("151bf226-53eb-4d11-9822-a56aa6ffbaec")]
    [ClassInterface(ClassInterfaceType.None)]
    public partial class Camera : ICameraV2
    {
        /// <summary>
        /// ASCOM DeviceID (COM ProgID) for this driver.
        /// The DeviceID is used by ASCOM applications to load the driver at runtime.
        /// </summary>
        private static string _deviceId = "ASCOM.Sony.Camera";
        private static string _deviceType = "Camera";
        private static string _deviceDescription = "ASCOM Sony Camera";

        // ASCOM device property keys
        private static string _selectedCameraIdKey = "selectedCameraId";
        private static string _selectedIsoKey = "selectedIso";
        private static string _imageFormatKey = "imageFormat";
        private static string _autoDeleteImageKey = "autoDelete";
        private static string _traceEnableKey = "traceEnable";
        private static string _sonyAppPathKey = "sonyAppPath";
        private static string _bulbModeKey = "bulbMode";

        public static string SelectedCameraId = "";
        public static short SelectedIso = 100;
        public static ImageFormat ImageFormat = ImageFormat.CFA;
        public static bool AutoDeleteImage = false;
        public static bool TraceEnabled = false;
        public static string SonyAppPath = "";
        public static bool BulbMode = false;

        internal static Settings Settings;

        /// <summary>
        /// Private variable to hold an ASCOM Utilities object
        /// </summary>
        private Util utilities;

        /// <summary>
        /// Private variable to hold an ASCOM AstroUtilities object to provide the Range method
        /// </summary>
        private AstroUtils astroUtilities;

        /// <summary>
        /// Variable to hold the trace logger object (creates a diagnostic log file with information that you specify)
        /// </summary>
        internal static TraceLogger tl;

        //
        // PUBLIC COM INTERFACE ICameraV2 IMPLEMENTATION
        //

        #region Common properties and methods.

        /// <summary>
        /// Displays the Setup Dialog form.
        /// If the user clicks the OK button to dismiss the form, then
        /// the new settings are saved, otherwise the old values are reloaded.
        /// THIS IS THE ONLY PLACE WHERE SHOWING USER INTERFACE IS ALLOWED!
        /// </summary>
        public void SetupDialog()
        {
            // consider only showing the setup dialog if not connected
            // or call a different dialog if connected

            var profile = ReadProfile();

            using (SetupDialogForm F = new SetupDialogForm())
            {
                var result = F.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    WriteProfile(profile); // Persist device configuration values to the ASCOM Profile store
                }
            }
        }

        public ArrayList SupportedActions
        {
            get
            {
                tl.LogMessage("SupportedActions Get", "Returning empty arraylist");
                return new ArrayList();
            }
        }

        public string Action(string actionName, string actionParameters)
        {
            LogMessage("", "Action {0}, parameters {1} not implemented", actionName, actionParameters);
            throw new ASCOM.ActionNotImplementedException("Action " + actionName + " is not implemented by this driver");
        }

        public void CommandBlind(string command, bool raw)
        {
            CheckConnected("CommandBlind");
            // Call CommandString and return as soon as it finishes
            this.CommandString(command, raw);
            // or
            throw new ASCOM.MethodNotImplementedException("CommandBlind");
            // DO NOT have both these sections!  One or the other
        }

        public bool CommandBool(string command, bool raw)
        {
            CheckConnected("CommandBool");
            string ret = CommandString(command, raw);
            // TODO decode the return string and return true or false
            // or
            throw new ASCOM.MethodNotImplementedException("CommandBool");
            // DO NOT have both these sections!  One or the other
        }

        public string CommandString(string command, bool raw)
        {
            CheckConnected("CommandString");
            // it's a good idea to put all the low level communication with the device here,
            // then all communication calls this function
            // you need something to ensure that only one command is in progress at a time

            throw new ASCOM.MethodNotImplementedException("CommandString");
        }

        public void Dispose()
        {
            // Clean up the tracelogger and util objects
            tl.Enabled = false;
            tl.Dispose();
            tl = null;
            utilities.Dispose();
            utilities = null;
            astroUtilities.Dispose();
            astroUtilities = null;
        }

        public string Description
        {
            // TODO customise this device description
            get
            {
                tl.LogMessage("Description Get", _deviceDescription);
                return _deviceDescription;
            }
        }

        public string DriverInfo
        {
            get
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                // TODO customise this driver description
                string driverInfo = "Information about the driver itself. Version: " + String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
                tl.LogMessage("DriverInfo Get", driverInfo);
                return driverInfo;
            }
        }

        public string DriverVersion
        {
            get
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                string driverVersion = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
                tl.LogMessage("DriverVersion Get", driverVersion);
                return driverVersion;
            }
        }

        public short InterfaceVersion
        {
            // set by the driver wizard
            get
            {
                LogMessage("InterfaceVersion Get", "2");
                return Convert.ToInt16("2");
            }
        }

        public string Name
        {
            get
            {
                string name = "ASCOM.Sony.Driver";
                tl.LogMessage("Name Get", name);
                return name;
            }
        }

        #endregion

        

        #region Private properties and methods
        // here are some useful properties and methods that can be used as required
        // to help with driver development

        #region ASCOM Registration

        // Register or unregister driver for ASCOM. This is harmless if already
        // registered or unregistered. 
        //
        /// <summary>
        /// Register or unregister the driver with the ASCOM Platform.
        /// This is harmless if the driver is already registered/unregistered.
        /// </summary>
        /// <param name="bRegister">If <c>true</c>, registers the driver, otherwise unregisters it.</param>
        private static void RegUnregASCOM(bool bRegister)
        {
            using (var P = new ASCOM.Utilities.Profile())
            {
                P.DeviceType = "Camera";
                if (bRegister)
                {
                    P.Register(_deviceId, _deviceDescription);
                }
                else
                {
                    P.Unregister(_deviceId);
                }
            }
        }

        /// <summary>
        /// This function registers the driver with the ASCOM Chooser and
        /// is called automatically whenever this class is registered for COM Interop.
        /// </summary>
        /// <param name="t">Type of the class being registered, not used.</param>
        /// <remarks>
        /// This method typically runs in two distinct situations:
        /// <list type="numbered">
        /// <item>
        /// In Visual Studio, when the project is successfully built.
        /// For this to work correctly, the option <c>Register for COM Interop</c>
        /// must be enabled in the project settings.
        /// </item>
        /// <item>During setup, when the installer registers the assembly for COM Interop.</item>
        /// </list>
        /// This technique should mean that it is never necessary to manually register a driver with ASCOM.
        /// </remarks>
        [ComRegisterFunction]
        public static void RegisterASCOM(Type t)
        {
            RegUnregASCOM(true);
        }

        /// <summary>
        /// This function unregisters the driver from the ASCOM Chooser and
        /// is called automatically whenever this class is unregistered from COM Interop.
        /// </summary>
        /// <param name="t">Type of the class being registered, not used.</param>
        /// <remarks>
        /// This method typically runs in two distinct situations:
        /// <list type="numbered">
        /// <item>
        /// In Visual Studio, when the project is cleaned or prior to rebuilding.
        /// For this to work correctly, the option <c>Register for COM Interop</c>
        /// must be enabled in the project settings.
        /// </item>
        /// <item>During uninstall, when the installer unregisters the assembly from COM Interop.</item>
        /// </list>
        /// This technique should mean that it is never necessary to manually unregister a driver from ASCOM.
        /// </remarks>
        [ComUnregisterFunction]
        public static void UnregisterASCOM(Type t)
        {
            RegUnregASCOM(false);
        }

        #endregion

        /// <summary>
        /// Returns true if there is a valid connection to the driver hardware
        /// </summary>
        private bool IsConnected
        {
            get
            {
                // TODO check that the driver hardware connection exists and is connected to the hardware
                return connectedState;
            }
        }

        /// <summary>
        /// Use this function to throw an exception if we aren't connected to the hardware
        /// </summary>
        /// <param name="message"></param>
        private void CheckConnected(string message)
        {
            if (!IsConnected)
            {
                throw new ASCOM.NotConnectedException(message);
            }
        }

        /// <summary>
        /// Get the last selected camera model from the list of cameras by ID
        /// If a camera has not been selected then choose the first from the list
        /// Or if an id is provided get that one
        /// </summary>
        public static CameraModel GetSelectedCameraModel(string id = null)
        {
            id = id ?? SelectedCameraId;
            return Settings.CameraModels.Where(cm => cm.ID == id).FirstOrDefault() ?? Settings.CameraModels.FirstOrDefault();
        }

        /// <summary>
        /// Read the device configuration from the ASCOM Profile store
        /// </summary>
        internal Profile ReadProfile()
        {
            // new ASCOM profile
            var profile = new Profile();
            profile.DeviceType = _deviceType;

            // register if first time setup
            if (!profile.IsRegistered(_deviceId))
                profile.Register(_deviceId, _deviceDescription);

            // create new cameras if new models exist
            DeserializeModelsFromJson();

            // check if user has selected a camera before
            try
            {
                ReadProfileValues(profile);
            }
            catch(Exception ex)
            {
                WriteProfileValues(profile);
            }
            return profile;
        }

        /// <summary>
        /// Write the profile passed in to the ASCOM Profile store
        /// Then read it to the class variables
        /// </summary>
        internal void WriteProfile(Profile profile)
        {
            WriteProfileValues(profile);
            ReadProfileValues(profile);
        }

        /// <summary>
        /// Read the device configuration from the ASCOM Profile store to class variables
        /// </summary>
        private void ReadProfileValues(Profile profile)
        {
            SelectedCameraId = profile.GetValue(_deviceId, _selectedCameraIdKey);
            short.TryParse(profile.GetValue(_deviceId, _selectedIsoKey), out SelectedIso);
            ImageFormat = (ImageFormat) Enum.Parse(typeof(ImageFormat), profile.GetValue(_deviceId, _imageFormatKey));
            AutoDeleteImage = Boolean.Parse(profile.GetValue(_deviceId, _autoDeleteImageKey));
            TraceEnabled = Boolean.Parse(profile.GetValue(_deviceId, _traceEnableKey));
            tl.Enabled = TraceEnabled;
            SonyAppPath = profile.GetValue(_deviceId, _sonyAppPathKey);
            BulbMode = Boolean.Parse(profile.GetValue(_deviceId, _bulbModeKey));
        }

        /// <summary>
        /// Write the device class variables to ASCOM  Profile store
        /// </summary>
        internal void WriteProfileValues(Profile profile)
        {
            profile.WriteValue(_deviceId, _selectedCameraIdKey, SelectedCameraId);
            profile.WriteValue(_deviceId, _selectedIsoKey, SelectedIso.ToString());
            profile.WriteValue(_deviceId, _imageFormatKey, ImageFormat.ToString());
            profile.WriteValue(_deviceId, _autoDeleteImageKey, AutoDeleteImage.ToString());
            profile.WriteValue(_deviceId, _traceEnableKey, TraceEnabled.ToString());
            profile.WriteValue(_deviceId, _sonyAppPathKey, SonyAppPath);
            profile.WriteValue(_deviceId, _bulbModeKey, BulbMode.ToString());
        }

        /// <summary>
        /// Log helper function that takes formatted strings and arguments
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        internal static void LogMessage(string identifier, string message, params object[] args)
        {
            var msg = string.Format(message, args);
            tl.LogMessage(identifier, msg);
        }
        #endregion

        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private static void DeserializeModelsFromJson()
        {
            tl.LogMessage("DeserializeModelsFromJson", "");
            
            string settingsPath = Path.Combine(AssemblyDirectory, "Settings.json");
            string settingsModel = File.ReadAllText(settingsPath);
            Settings = JsonConvert.DeserializeObject<Settings>(settingsModel);     

            foreach (var cameraModel in Settings.CameraModels)
            {
                var shutterSpeeds = new List<ShutterSpeed>();
                foreach (var avaiableShutterSpeed in cameraModel.AvaiableShutterSpeeds)
                {
                    shutterSpeeds.Add(Settings.ShutterSpeedMap.Where(SSM => SSM.Name == avaiableShutterSpeed).First());
                }
                cameraModel.ShutterSpeeds = shutterSpeeds.ToArray();
            }
        }
    }
}
