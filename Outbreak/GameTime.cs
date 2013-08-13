using System;

namespace Outbreak
{
    public class GameTime
    {
        public static int TicksPerMinute = 5;

        public int Ticks { get; private set; }
        public int Minute { get; private set; }
        public int Hour { get; private set; }

        public GameTime()
        {
            Ticks = 0;
            Hour = DateTime.UtcNow.Hour;
            Minute = DateTime.UtcNow.Minute;
        }

        public float Fractional
        {
            get
            {
                return Hour + (Minute / 60.0f);
            }
        }

        public void Tick()
        {
            Ticks++;

            if (Ticks == TicksPerMinute)
            {
                Minute++;
                Ticks = 0;
            }

            if (Minute == 60)
            {
                Minute = 0;
                Hour++;
            }

            if (Hour == 24)
            {
                Hour = 0;
            }
        }

        public void SetTime(int hours, int minutes)
        {
            Hour = hours;
            Minute = minutes;
        }
    }
}