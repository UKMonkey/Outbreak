using Vortex.Interface;

namespace Outbreak.Server
{
    public abstract class RespawnTimer
    {
        public int Countdown { get; set; }

        public bool Tick()
        {
            Countdown -= 1;
            return (Countdown <= 0);
        }

        abstract public void Spawn(IGameServer game);
    }
}
