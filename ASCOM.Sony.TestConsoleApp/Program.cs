using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using System.Threading;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ASCOM.Sony.TestConsoleApp
{
    public class Program
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        static void Main(string[] args)
        {
            // make sure to build+install ASCOM driver first
            // use the ASCOM Diagnostics utility to choose the correct camera     
            var camera = new Camera();
            camera.Connected = true;

            if (camera.Connected)
            {
                Console.WriteLine("Press S to take exposure.");
                do
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.S)
                    {
                        var exposureLength = (double)1 / 40;
                        camera.Gain = 0;
                        camera.StartExposure(exposureLength, true);

                        while (!camera.ImageReady)
                        {
                            Thread.Sleep(100);
                        }

                        var image = camera.ImageArray;
                    }
                } while (true);
            }
        }
        

        private static void WriteImageStatistics(ImageDataProcessor dataProcessor, Array array)
        {
            var stats = dataProcessor.GetImageStatistics(array);
            if (stats != null)
            {
                Console.WriteLine(
                    $"Image statistics: ADU min/max/mean/median: {stats.MinADU}/{stats.MaxADU}/{stats.MeanADU}/{stats.MedianADU}.");
            }
            else
            {
                Console.WriteLine("Image statistics not available.");
            }
        }

        private static void Camera_ExposureCompleted(object sender, ExposureCompletedEventArgs e)
        {
            Console.WriteLine("Exposure completed. Downloading ...");
        }

        private static void Camera_ExposureReady(object sender, ExposureReadyEventArgs e)
        {
            var array = (uint[,]) e.ImageArray;

            int width = array.GetLength(0);
            int height = array.GetLength(1);

            var flatArray = new uint[width*height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    flatArray[j * width + i] = array[i, j];
                }
            }

            Console.WriteLine($"Image array length: {array.LongLength}");
            Console.WriteLine($"Image dimensions: {width}x{height}");
            Console.WriteLine($"ADU max/min: {flatArray.Min()}/{flatArray.Max()}");

            Console.WriteLine("Saving to tiff...");
            SaveToGrayscaleTiff("D:\\astrophoto\\~test\\test.tiff", array);

        }


        private static void SaveToGrayscaleTiff(string filename, uint[,] imageArray)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            int width = imageArray.GetLength(0);
            int height = imageArray.GetLength(1);

            var array = new ushort[width * height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    array[j * width + i] = (ushort)imageArray[i, j];
                }
            }

            var bitmapSrc = BitmapSource.Create(width, height, 96, 96, PixelFormats.Gray16, null, array, 4 * ((width * 2 + 3) / 4));
           
            using (var fs = File.OpenWrite(filename))
            {
                TiffBitmapEncoder encoder = new TiffBitmapEncoder();
                encoder.Compression = TiffCompressOption.None;
                encoder.Frames.Add(BitmapFrame.Create(bitmapSrc));
                encoder.Save(fs);
                fs.Close();
            }
        }

        private static void SaveToColorTiff(string filename, uint[,,] imageArray)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            int width = imageArray.GetLength(0);
            int height = imageArray.GetLength(1);

            var array = new ushort[width * height * 3];

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i ++)
                {
                    array[j * width*3 + i*3] = (ushort)imageArray[i, j,2];
                    array[j * width * 3 + i*3+1] = (ushort)imageArray[i, j, 1];
                    array[j * width * 3 + i*3+2] = (ushort)imageArray[i, j, 0];
                }
            }

            var bitmapSrc = BitmapSource.Create(width, height, 96, 96, PixelFormats.Rgb48, null, array, 4 * ((width * 6 + 3) / 4));

            using (var fs = File.OpenWrite(filename))
            {
                TiffBitmapEncoder encoder = new TiffBitmapEncoder();
                encoder.Compression = TiffCompressOption.None;
                encoder.Frames.Add(BitmapFrame.Create(bitmapSrc));
                encoder.Save(fs);
                fs.Close();
            }
        }

        private static void SaveToColor8bitTiff(string filename, uint[,,] imageArray)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            int width = imageArray.GetLength(0);
            int height = imageArray.GetLength(1);

            var array = new Byte[width * height * 3];

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    array[j * width * 3 + i * 3] = (byte)imageArray[i, j, 2];
                    array[j * width * 3 + i * 3 + 1] = (byte)imageArray[i, j, 1];
                    array[j * width * 3 + i * 3 + 2] = (byte)imageArray[i, j, 0];
                }
            }

            var bitmapSrc = BitmapSource.Create(width, height, 96, 96, PixelFormats.Rgb24, null, array, 4 * ((width * 3 + 3) / 4));

            using (var fs = File.OpenWrite(filename))
            {
                TiffBitmapEncoder encoder = new TiffBitmapEncoder();
                encoder.Compression = TiffCompressOption.None;
                encoder.Frames.Add(BitmapFrame.Create(bitmapSrc));
                encoder.Save(fs);
                fs.Close();
            }
        }

        private static CameraModel _sltA99 => new CameraModel()
        {
            ID = "SLTA99",
            Name = "Sony SLT-A99",

            Sensor = new Sensor()
            {
                Name = "IMX157AQR",
                //RawWidth = 6048,
                //RawHeight = 4024,
                FrameWidth = 6018,
                FrameHeight = 4024,
                CropWidth = 6000,
                CropHeight = 4000,
                PixelSizeWidth = 5.93,
                PixelSizeHeight = 5.93,
                Width = 35.8,
                Height = 23.8,
                MaxADU = 16383,
                ElectronsPerADU = 1.0,
                FullWellCapacity = 32767.0,
            },
            ExposureMin = 1.0 / 8000,
            ExposureMax = 3600,
            Gains = new short[] { 50, 64, 80, 100, 125, 160, 200, 250, 320, 400, 500, 640, 800, 1000, 1250, 1600, 2000, 2500, 3200, 4000, 5000, 6400, 8000, 10000, 12800, 16000, 20000, 25600 },
            ShutterSpeeds = new[]
            {
                new ShutterSpeed("1/8000",1.0/8000),
                new ShutterSpeed("1/6400", 1.0/6400),
                new ShutterSpeed("1/5000", 1.0/5000),
                new ShutterSpeed("1/4000", 1.0/4000),
                new ShutterSpeed("1/3200", 1.0/3200),
                new ShutterSpeed("1/2500", 1.0/2500),
                new ShutterSpeed("1/2000",1.0/2000),
                new ShutterSpeed("1/1600",1.0/1600),
                new ShutterSpeed("1/1250", 1.0/1250),
                new ShutterSpeed("1/1000",1.0/1000),
                new ShutterSpeed("1/800",1.0/800),
                new ShutterSpeed("1/640",1.0/640),
                new ShutterSpeed("1/500",1.0/500),
                new ShutterSpeed("1/400",1.0/400),
                new ShutterSpeed("1/320", 1.0/320),
                new ShutterSpeed("1/250",1.0/250),
                new ShutterSpeed("1/200",1.0/200),
                new ShutterSpeed("1/160",1.0/160),
                new ShutterSpeed("1/125",1.0/250),
                new ShutterSpeed("1/80",1.0/80),
                new ShutterSpeed("1/60", 1.0/60),
                new ShutterSpeed("1/50", durationSeconds:1.0/50),
                new ShutterSpeed("1/40", durationSeconds:1.0/40),
                new ShutterSpeed("1/30", durationSeconds:1.0/30),
                new ShutterSpeed("1/25", durationSeconds:1.0/25),
                new ShutterSpeed("1/20", durationSeconds:1.0/20),
                new ShutterSpeed("1/15", durationSeconds:1.0/15),
                new ShutterSpeed("1/13", durationSeconds:1.0/13),
                new ShutterSpeed("1/10", durationSeconds:1.0/10),
                new ShutterSpeed("1/8", durationSeconds:1.0/8),
                new ShutterSpeed("1/6", durationSeconds:1.0/6),
                new ShutterSpeed("1/5", durationSeconds:1.0/5),
                new ShutterSpeed("1/4", durationSeconds:1.0/4),
                new ShutterSpeed("1/3", durationSeconds:1.0/3),
                new ShutterSpeed("0.4\"", durationSeconds:0.4),
                new ShutterSpeed("0.5\"", durationSeconds:0.5),
                new ShutterSpeed("0.6\"", durationSeconds:0.6),
                new ShutterSpeed("0.8\"", durationSeconds:0.8),
                new ShutterSpeed("1\"", durationSeconds:1.0),
                new ShutterSpeed("1.3\"", durationSeconds:1.3),
                new ShutterSpeed("1.6\"", durationSeconds:1.6),
                new ShutterSpeed("BULB", durationSeconds:2),
            },
            ExposureResolution = 0.1
        };
    }
}
