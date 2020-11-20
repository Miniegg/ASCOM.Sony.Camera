using System.Collections.Generic;

namespace ASCOM.Sony
{
    public class Settings
    {
        public Dictionary<string, int> Delays { get; set; }
        public CameraModel[] CameraModels { get; set; }
        public ShutterSpeed[] ShutterSpeedMap { get; set; }
        public List<Window> Windows { get; set; }
    }

    public class ShutterSpeed
    {
        public double DurationSeconds { get; set; }
        public string Name { get; set; }

        public ShutterSpeed()
        {

        }

        public ShutterSpeed(string name, double durationSeconds)
        {
            Name = name;
            DurationSeconds = durationSeconds;
        }
    }
}
