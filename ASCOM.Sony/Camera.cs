using System;
using System.Collections;
using System.Linq;
using ASCOM.DeviceInterface;
using ASCOM.Utilities;

namespace ASCOM.Sony
{

    public enum ImageFormat
    {
        CFA,
        Debayered,
        JPG
    }

    public partial class Camera
    {
        private ImageDataProcessor _imageDataProcessor = new ImageDataProcessor();

        /// <summary>
        /// Private variable to hold the connected state
        /// </summary>
        private bool connectedState;
        private SonyCamera _sonyCamera;
        private CameraStates _cameraState = CameraStates.cameraIdle;

        /// <summary>
        /// Initializes a new instance of the <see cref="ASCOM.Sony"/> class.
        /// Must be public for COM registration.
        /// </summary>
        public Camera()
        {
            tl = new TraceLogger("", "DSLR.Sony");
            tl.Enabled = TraceEnabled;
            tl.LogMessage("Camera", "Starting initialization");
            ReadProfile(); // Read device configuration from the ASCOM Profile store
            connectedState = false; // Initialise connected to false
            _cameraState = CameraStates.cameraIdle;
            tl.LogMessage("Camera", "Completed initialization");
        }

        #region ICamera Methods Implementation
        public void StartExposure(double Duration, bool Light)
        {
            CheckConnected("Camera not connected");

            //todo: add code to avoid possible race condition which could theoretically happen if client application calls StartExposure and then immediately disconnects from camera; so checkConnected above passes but access to _sonlyCamera below would throw NullReferenceException

            if (Duration < 0.0) throw new InvalidValueException("StartExposure", Duration.ToString(), "0.0 upwards");

            ushort readoutWidth = GetSelectedCameraModel().Sensor.GetReadoutWidth(ImageFormat);
            ushort readoutHeight = GetSelectedCameraModel().Sensor.GetReadoutHeight(ImageFormat);

            if (_numX > readoutWidth) throw new InvalidValueException("StartExposure", _numX.ToString(), readoutWidth.ToString());
            if (_numY > readoutHeight) throw new InvalidValueException("StartExposure", _numY.ToString(), readoutHeight.ToString());
            if (_startX > readoutWidth) throw new InvalidValueException("StartExposure", _startX.ToString(), readoutWidth.ToString());
            if (_startY > readoutHeight) throw new InvalidValueException("StartExposure", _startY.ToString(), readoutHeight.ToString());

            if (_cameraState != CameraStates.cameraIdle) throw new InvalidOperationException("Cannot start exposure - camera is not idle");

            tl.LogMessage("StartExposure", $"Duration: {Duration} s. ISO: {Gains[Gain]}. {(Light ? "Light" : "Dark")} frame.");

            _imageReady = false;
            _imageArray = null;
            _lastExposureDuration = Duration;
            _exposureStart = DateTime.Now;
            _cameraState = CameraStates.cameraExposing;

            SubscribeCameraEvents();
            
            short.TryParse(Gains[Gain].ToString(), out short gain);
            _sonyCamera.StartExposure(gain, Duration, Light);
        }

        private void SubscribeCameraEvents()
        {
            _sonyCamera.ExposureCompleted += _sonyCamera_ExposureCompleted;
            _sonyCamera.ExposureReady += _sonyCamera_ExposureReady;
        }

        private void UnsubscribeCameraEvents()
        {
            if (_sonyCamera != null)
            {
                _sonyCamera.ExposureCompleted -= _sonyCamera_ExposureCompleted;
                _sonyCamera.ExposureReady -= _sonyCamera_ExposureReady;
            }
        }

        private void _sonyCamera_ExposureReady(object sender, ExposureReadyEventArgs e)
        {   
            tl.LogMessage("_sonyCamera_ExposureReady", string.Format("ImageArray Length: {0}",e.ImageArray.Length));

            if (IsConnected == false)
            {
                _cameraState = CameraStates.cameraIdle;
                return;
            }
            
            _cameraState = CameraStates.cameraDownload;
            
            try
            {
                try
                {
                    //report stats to log (if tracing enabled)
                    if (tl.Enabled)
                    {
                        tl.LogMessage("_sonyCamera_ExposureReady", "GetStats");
                        var stats = _imageDataProcessor.GetImageStatistics(e.ImageArray);
                        if (stats != null)
                        {
                            tl.LogMessage("_sonyCamera_ExposureReady", $"Image statistics: ADU min/max/mean/median: {stats.MinADU}/{stats.MaxADU}/{stats.MeanADU}/{stats.MedianADU}.");
                        }
                    }

                    tl.LogMessage("_sonyCamera_ExposureReady", StartX + " StartX," + StartY + " StartY, " + NumX + "NumX," + NumY + "NumY," +  CameraXSize + "CameraXSize," + CameraYSize + "CameraYSize");
                    // ToDo: is this nessacary
                    _imageArray = _imageDataProcessor.CutImageArray(e.ImageArray, StartX, StartY, NumX, NumY, CameraXSize, CameraYSize);
                    _cameraState = CameraStates.cameraIdle;
                    _imageReady = true;
                }
                catch (Exception ex)
                {
                    tl.LogIssue(ex.Message, ex.StackTrace);
                    _cameraState = CameraStates.cameraError;
                }
            }
            finally
            {
                UnsubscribeCameraEvents();
            }
        }

        private void _sonyCamera_ExposureCompleted(object sender, ExposureCompletedEventArgs e)
        {
            tl.LogMessage("_sonyCamera_ExposureCompleted", "cameraReading"); 
            _cameraState = CameraStates.cameraReading;
        }

        public void AbortExposure()
        {
            if (!BulbMode)
                throw new MethodNotImplementedException("Cannot Stop Exposure, CanStopExposure set to false");

            CheckConnected("Camera not connected");
            tl.LogMessage("AbortExposure","");
            if (_cameraState == CameraStates.cameraExposing)
            {
                _sonyCamera.AbortExposure();
            }

            _cameraState = CameraStates.cameraIdle;
        }

        public void StopExposure()
        {
            if (!BulbMode)
                throw new MethodNotImplementedException("Cannot Abort Exposure, CanStopExposure set to false");

            CheckConnected("Camera not connected");
            tl.LogMessage("StopExposure", "");
            if (_cameraState == CameraStates.cameraExposing)
            {
                _sonyCamera.StopExposure();
            }
        }
        #endregion

        #region ICamera Connected
        public bool Connected
        {
            get
            {
                LogMessage("Connected", "Get {0}", IsConnected);
                return IsConnected;
            }
            set
            {
                tl.LogMessage("Connected", "Set {0}", value);
                if (value == IsConnected)
                    return;

                if (value)
                {
                    connectedState = true;
                    LogMessage("Connected Set", "Connecting to Sony Imaging Edge Remote app");

                    try
                    {
                        if (_sonyCamera == null)
                        {
                            _sonyCamera = new SonyCamera(GetSelectedCameraModel(), ImageFormat, AutoDeleteImage, tl);
                        }
                        _sonyCamera.Connect();
                    }
                    catch (Exception e)
                    {
                        connectedState = false;
                        LogMessage("Connected Set", $"Connection failed. Reason: {e}");
                        throw new ASCOM.NotConnectedException(e);
                    }
                }
                else
                {
                    connectedState = false;
                    _sonyCamera = null;
                    LogMessage("Connected Set", "Disconnecting from Sony Imaging Edge Remote app");
                }
            }
        }

        #endregion

        #region ICamera Properties Implementation
        private DateTime _exposureStart = DateTime.MinValue;
        private double _lastExposureDuration = 0.0;
        private bool _imageReady = false;
        private Array _imageArray; //sony interop component will return it as UInt16, needs to be converted to Int32 before returned to driver users

        private int _numX;
        private int _numY;
        private int _startX = 0;
        private int _startY = 0;
        private short _binX = 1;
        private short _binY = 1;
        private short _gain;

        public short BayerOffsetX
        {
            get
            {
                tl.LogMessage("BayerOffsetX Get Get", 0.ToString());
                return 0;
            }
        }

        public short BayerOffsetY
        {
            get
            {
                tl.LogMessage("BayerOffsetY Get Get", 0.ToString());
                return 0;
            }
        }

        public short BinX
        {
            get
            {
                tl.LogMessage("BinX Get", _binX.ToString());
                return _binX;
            }
            set
            {
                tl.LogMessage("BinX Set", value.ToString());
                _binX = value;
            }
        }

        public short BinY
        {
            get
            {
                tl.LogMessage("BinY Get", _binY.ToString());
                return _binY;
            }
            set
            {
                tl.LogMessage("BinY Set", value.ToString());
                _binY = value;
            }
        }

        public double CCDTemperature
        {
            get
            {
                //normally we should throw  ASCOM.PropertyNotImplementedException because we cannot control sensor temperature; but some apps seems to try to read that value and do not work correctly if they cant (example NINA)
                //so as workaround user can set fixed temperature in cameramodel.JSON file

                if (GetSelectedCameraModel().Sensor.CCDTemperature.HasValue)
                {
                   return GetSelectedCameraModel().Sensor.CCDTemperature.Value;
                }
                
                tl.LogMessage("CCDTemperature Get Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("CCDTemperature", false);
            }
        }

        
        public CameraStates CameraState
        {
            get
            {
                // this gets called a LOT, will bloat debug if enabled
                tl.LogMessage("CameraState Get", _cameraState.ToString());
                return _cameraState;
            }
        }

        public int CameraXSize
        {
            get
            {
                ushort readoutWidth = GetSelectedCameraModel().Sensor.GetReadoutWidth(ImageFormat);
                
                tl.LogMessage("CameraXSize Get", readoutWidth.ToString());
                return readoutWidth;
            }
        }

        public int CameraYSize
        {
            get
            {
                ushort readoutHeight = GetSelectedCameraModel().Sensor.GetReadoutHeight(ImageFormat);
                tl.LogMessage("CameraYSize Get", readoutHeight.ToString());
                return readoutHeight;
            }
        }

        public bool CanAbortExposure
        {
            get
            {
                tl.LogMessage("CanAbortExposure Get", BulbMode.ToString()); 
                return BulbMode;
            }
        }

        public bool CanAsymmetricBin
        {
            get
            {
                tl.LogMessage("CanAsymmetricBin Get", false.ToString());
                return false;
            }
        }

        public bool CanFastReadout
        {
            get
            {
                tl.LogMessage("CanFastReadout Get", false.ToString());
                return false;
            }
        }

        public bool CanGetCoolerPower
        {
            get
            {
                tl.LogMessage("CanGetCoolerPower Get", false.ToString());
                return false;
            }
        }

        public bool CanPulseGuide
        {
            get
            {
                tl.LogMessage("CanPulseGuide Get", false.ToString());
                return false;
            }
        }

        public bool CanSetCCDTemperature
        {
            get
            {
                tl.LogMessage("CanSetCCDTemperature Get", false.ToString());
                return false;
            }
        }

        public bool CanStopExposure
        {
            get
            {
                tl.LogMessage("CanStopExposure Get", BulbMode.ToString());
                return BulbMode;
            }
        }

        public bool CoolerOn
        {
            get
            {
                tl.LogMessage("CoolerOn Get Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("CoolerOn", false);
            }
            set
            {
                tl.LogMessage("CoolerOn Set Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("CoolerOn", true);
            }
        }

        public double CoolerPower
        {
            get
            {
                tl.LogMessage("CoolerPower Get Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("CoolerPower", false);
            }
        }

        public double ElectronsPerADU
        {
            get
            {
                tl.LogMessage("ElectronsPerADU Get", GetSelectedCameraModel().Sensor.ElectronsPerADU.ToString());
                return GetSelectedCameraModel().Sensor.ElectronsPerADU;
            }
        }

        public double ExposureMax
        {
            get
            {
                tl.LogMessage("ExposureMax Get", GetSelectedCameraModel().ExposureMax.ToString());
                return GetSelectedCameraModel().ExposureMax;
            }
        }

        public double ExposureMin
        {
            get
            {
                tl.LogMessage("ExposureMin Get", GetSelectedCameraModel().ExposureMin.ToString());
                return GetSelectedCameraModel().ExposureMin;
            }
        }

        public double ExposureResolution
        {
            get
            {
                tl.LogMessage("ExposureResolution Get", GetSelectedCameraModel().ExposureResolution.ToString());
                return GetSelectedCameraModel().ExposureResolution;
            }
        }

        public bool FastReadout
        {
            get
            {
                tl.LogMessage("FastReadout Get", "Not implemented");
                CheckConnected("Not Connected");
                throw new ASCOM.PropertyNotImplementedException("FastReadout", false);
            }
            set
            {
                tl.LogMessage("FastReadout Set", "Not implemented");
                CheckConnected("Not Connected");
                throw new ASCOM.PropertyNotImplementedException("FastReadout", true);
            }
        }

        public double FullWellCapacity
        {
            get
            {
                tl.LogMessage("FullWellCapacity Get", GetSelectedCameraModel().Sensor.FullWellCapacity.ToString());
                return GetSelectedCameraModel().Sensor.FullWellCapacity;
            }
        }

        public short Gain
        {
            get
            {
                tl.LogMessage("Gain Get", _gain.ToString());
                return _gain;
            }
            set
            {
                tl.LogMessage("Gain Set", value.ToString());
                _gain = value;
            }
        }

        public short GainMax
        {
            get
            {
                tl.LogMessage("GainMax Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("GainMax", false);
            }
        }

        public short GainMin
        {
            get
            {
                tl.LogMessage("GainMin Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("GainMin", true);
            }
        }

        public ArrayList Gains
        {
            // ASCOM standard = ArrayList of STRINGS!!!!
            get
            {
                var arrayList = new ArrayList();
                var stringGains = GetSelectedCameraModel().Gains.Select(g => g.ToString()).ToArray();
                arrayList.AddRange(stringGains);
                
                string gainsOutput = "";
                foreach (var item in arrayList)
                    gainsOutput = $"{gainsOutput}, {item}";
                tl.LogMessage("Gains Get", gainsOutput);

                return arrayList;//todo; should we create new array list every time?
            }
        }

        public bool HasShutter
        {
            get
            {
                tl.LogMessage("HasShutter Get", false.ToString());
                return false;
            }
        }

        public double HeatSinkTemperature
        {
            get
            {
                tl.LogMessage("HeatSinkTemperature Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("HeatSinkTemperature", false);
            }
        }

        public object ImageArray
        {
            get
            {
                if (!_imageReady)
                {
                    tl.LogMessage("ImageArray Get", "Throwing InvalidOperationException because of a call to ImageArray before the first image has been taken!");
                    throw new ASCOM.InvalidOperationException("Call to ImageArray before the first image has been taken!");
                }

                return _imageArray;
            }
        }

        public object ImageArrayVariant
        {
            get
            {
                if (!_imageReady)
                {
                    tl.LogMessage("ImageArrayVariant Get", "Throwing InvalidOperationException because of a call to ImageArrayVariant before the first image has been taken!");
                    throw new ASCOM.InvalidOperationException("Call to ImageArrayVariant before the first image has been taken!");
                }

                return _imageDataProcessor.ToVariantArray(_imageArray);
            }
        }

        public bool ImageReady
        {
            get
            {
                tl.LogMessage("ImageReady Get", _imageReady.ToString());
                return _imageReady;
            }
        }

        public bool IsPulseGuiding
        {
            get
            {
                tl.LogMessage("IsPulseGuiding Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("IsPulseGuiding", false);
            }
        }

        public double LastExposureDuration
        {
            get
            {
                if (!_imageReady)
                {
                    tl.LogMessage("LastExposureDuration Get", "Throwing InvalidOperationException because of a call to LastExposureDuration before the first image has been taken!");
                    throw new ASCOM.InvalidOperationException("Call to LastExposureDuration before the first image has been taken!");
                }
                tl.LogMessage("LastExposureDuration Get", _lastExposureDuration.ToString());
                return _lastExposureDuration;
            }
        }

        public string LastExposureStartTime
        {
            get
            {
                if (!_imageReady)
                {
                    tl.LogMessage("LastExposureStartTime Get", "Throwing InvalidOperationException because of a call to LastExposureStartTime before the first image has been taken!");
                    throw new ASCOM.InvalidOperationException("Call to LastExposureStartTime before the first image has been taken!");
                }
                string exposureStartString = _exposureStart.ToString("yyyy-MM-ddTHH:mm:ss");
                tl.LogMessage("LastExposureStartTime Get", exposureStartString.ToString());
                return exposureStartString;
            }
        }

        public int MaxADU
        {
            get
            {
                int maxADU;
                
                switch (ImageFormat)
                {
                    case ImageFormat.CFA:
                        maxADU = GetSelectedCameraModel().Sensor.MaxADU;
                        if (maxADU == 0)
                            maxADU = 16383; // if maxADU is not defined in the JSON then asssign it default value of 14 bits
                        break;
                    case ImageFormat.Debayered: //note: we dont use sensor characteristic here because we use LibRaw to debayer RAW image and it is doing lot of things with the file, including I believe expanding color values to full 16bit
                        maxADU = ushort.MaxValue;
                        break;
                    case ImageFormat.JPG:
                        maxADU = byte.MaxValue;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                tl.LogMessage("MaxADU Get", maxADU.ToString());
                return maxADU;
            }
        }

        public short MaxBinX
        {
            get
            {
                tl.LogMessage("MaxBinX Get", "1");
                return 1;
            }
        }

        public short MaxBinY
        {
            get
            {
                tl.LogMessage("MaxBinY Get", "1");
                return 1;
            }
        }

        public int NumX
        {
            get
            {
                tl.LogMessage("NumX Get", _numX.ToString());
                return _numX;
            }
            set
            {
                _numX = value;
                tl.LogMessage("NumX set", value.ToString());
            }
        }

        public int NumY
        {
            get
            {
                tl.LogMessage("NumY Get", _numY.ToString());
                return _numY;
            }
            set
            {
                _numY = value;
                tl.LogMessage("NumY set", value.ToString());
            }
        }

        public short PercentCompleted
        {
            get
            {
                if (InterfaceVersion == 1)
                {
                    throw new NotSupportedException("PercentCompleted (not supported for Interface V1)");
                }

                CheckConnected("Can't get PercentCompleted when not connected");

                short percentCompleted = 0;
                switch (_cameraState)
                {
                    case CameraStates.cameraIdle:
                        percentCompleted = (short) (ImageReady ? 100 : 0);
                        break;
                    case CameraStates.cameraWaiting:
                    case CameraStates.cameraExposing:
                    case CameraStates.cameraReading:
                    case CameraStates.cameraDownload:
                        percentCompleted = (short) ((DateTime.Now-_exposureStart).TotalSeconds/LastExposureDuration*100.00);
                        break;
                    case CameraStates.cameraError:
                        return (short) 0;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                tl.LogMessage("PercentCompleted Get", "Not implemented");
                return percentCompleted;
            }
        }

        public double PixelSizeX
        {
            get
            {
                return GetSelectedCameraModel().Sensor.PixelSizeWidth*BinX;
            }
        }

        public double PixelSizeY
        {
            get
            {
                return GetSelectedCameraModel().Sensor.PixelSizeHeight*BinY;
            }
        }

        public void PulseGuide(GuideDirections Direction, int Duration)
        {
            tl.LogMessage("PulseGuide", "Not implemented");
            throw new ASCOM.MethodNotImplementedException("PulseGuide");
        }

        public short ReadoutMode
        {
            get
            {
                tl.LogMessage("ReadoutMode Get", $"{(short)ReadoutModes.IndexOf(ImageFormat.ToString())} {ImageFormat.ToString()}");
                return (short)ReadoutModes.IndexOf(ImageFormat.ToString());
            }
            set
            {
                tl.LogMessage("ReadoutMode Set", value.ToString());
                ImageFormat = (ImageFormat) Enum.Parse(typeof(ImageFormat), (string) ReadoutModes[value], true);
            }
        }

        public ArrayList ReadoutModes
        {
            get
            {
                tl.LogMessage("ReadoutModes Get", "Not implemented");
                return new ArrayList(new [] { Sony.ImageFormat.CFA.ToString(), Sony.ImageFormat.Debayered.ToString(), Sony.ImageFormat.JPG.ToString() });
            }
        }

        public string SensorName
        {
            get
            {
                tl.LogMessage("SensorName Get", GetSelectedCameraModel().Sensor.Name);
                return GetSelectedCameraModel().Sensor.Name;
            }
        }

        public SensorType SensorType
        {
            get
            {
                tl.LogMessage("SensorType Get", SensorType.RGGB.ToString());
                switch (ImageFormat)
                {
                    case ImageFormat.CFA:
                        return SensorType.RGGB;
                    case ImageFormat.Debayered:
                    case ImageFormat.JPG:
                        return SensorType.Color;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public double SetCCDTemperature
        {
            get
            {
                tl.LogMessage("SetCCDTemperature Get", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("SetCCDTemperature", false);
            }
            set
            {
                tl.LogMessage("SetCCDTemperature Set", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("SetCCDTemperature", true);
            }
        }

        public int StartX
        {
            get
            {
                tl.LogMessage("StartX Get", _startX.ToString());
                return _startX;
            }
            set
            {
                _startX = value;
                tl.LogMessage("StartX Set", value.ToString());
            }
        }

        public int StartY
        {
            get
            {
                tl.LogMessage("StartY Get", _startY.ToString());
                return _startY;
            }
            set
            {
                _startY = value;
                tl.LogMessage("StartY set", value.ToString());
            }
        }
        
        #endregion
    }
}
