using System;

namespace Outbreak.Server
{
    public class WeatherController
    {
        private readonly Random _random;

        public bool IsRaining { get; private set; }

        public WeatherController()
        {
            _random = new Random();
        }

        public void Tick()
        {
            if (_random.NextDouble() > 0.9998f)
            {
                IsRaining = !IsRaining;
            }
        }
    }
}