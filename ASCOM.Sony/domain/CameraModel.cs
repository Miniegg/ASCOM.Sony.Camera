﻿using System;
using System.Collections.Generic;

namespace ASCOM.Sony
{
    public class CameraData
    {
        public CameraModel[] CameraModels { get; set; }
    }

    public class Sensor
    {
        public string Name { get; set; }

        /// <summary>
        /// Sensor width in millimeters
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Sensor height in millimeters
        /// </summary>
        public double Height { get; set; }

        ///// <summary>
        ///// Total sensor width in pixels (including reserved sensor areas)
        ///// </summary>
        //public ushort RawWidth { get; set; }
        ///// <summary>
        ///// Total sensor height in pixels (including reserved sensor areas)
        ///// </summary>
        //public ushort RawHeight { get; set; }

        /// <summary>
        /// Usable sensor width in pixels
        /// </summary>
        public ushort FrameWidth { get; set; }

        /// <summary>
        /// Usable sensor height in pixels
        /// </summary>
        public ushort FrameHeight { get; set; }

        /// <summary>
        /// Camera crop width (used for JPG files)
        /// </summary>
        public ushort CropWidth { get; set; }
        /// <summary>
        /// Camera crop height (used for JPG files)
        /// </summary>
        public ushort CropHeight { get; set; }

        public double PixelSizeWidth { get; set; }
        public double PixelSizeHeight { get; set; }

        public int MaxADU { get; set; }

        public double ElectronsPerADU { get; set; }

        public double FullWellCapacity { get; set; }

        public double? CCDTemperature { get; set; }

        public ushort GetReadoutWidth(ImageFormat imageFormat)
        {
            switch (imageFormat)
            {
                case ImageFormat.CFA:
                case ImageFormat.Debayered:
                    return FrameWidth;
                case ImageFormat.JPG:
                    return CropWidth;
                default:
                    throw new ArgumentOutOfRangeException(nameof(imageFormat), imageFormat, null);
            }
        }

        public ushort GetReadoutHeight(ImageFormat imageFormat)
        {
            switch (imageFormat)
            {
                case ImageFormat.CFA:
                case ImageFormat.Debayered:
                    return FrameHeight;
                case ImageFormat.JPG:
                    return CropHeight;
                default:
                    throw new ArgumentOutOfRangeException(nameof(imageFormat), imageFormat, null);
            }
        }
    }

    public class CameraModel
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public Sensor Sensor { get; set; }

        public bool CanStopExposure { get; set; }

        public double ExposureMin { get; set; }

        public double ExposureMax { get; set; }

        // all gain options in the cameras menu
        public string[] AllGains { get; set; }

        // gains available to ASCOM (limited to a short int)
        public short[] Gains { get; set; }

        public string[] AvaiableShutterSpeeds { get; set; }

        public ShutterSpeed[] ShutterSpeeds { get; set; }
        
        public double ExposureResolution { get; set; }
    }
}