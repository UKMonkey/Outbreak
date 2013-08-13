using Vortex.Interface;
using Vortex.Interface.Net;

namespace Outbreak.Server
{
    public class PlayerRespawnTimer : RespawnTimer
    {
        public RemotePlayer Player { get; set; }

        public override void Spawn(IGameServer game)
        {
            game.SpawnPlayer(Player);
        }
    }
}
