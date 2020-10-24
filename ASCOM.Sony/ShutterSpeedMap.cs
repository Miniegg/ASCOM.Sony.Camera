using System;

namespace ASCOM.Sony
{
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
