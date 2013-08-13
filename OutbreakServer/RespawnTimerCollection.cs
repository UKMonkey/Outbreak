using System.Collections.Generic;
using System.Linq;
using Vortex.Interface;
using Vortex.Interface.Net;

namespace Outbreak.Server
{
    public class RespawnTimerCollection
    {
        private readonly IGameServer _game;
        private readonly ICollection<RespawnTimer> _respawnTimerList;

        public RespawnTimerCollection(IGameServer game)
        {
            _respawnTimerList = new LinkedList<RespawnTimer>();
            _game = game;
        }

        public void RespawnPlayer(RemotePlayer player, int delayInSeconds)
        {
            _respawnTimerList.Add(new PlayerRespawnTimer
                                      {
                                          Player = player,
                                          Countdown = delayInSeconds
                                      });
        }

        public void Update()
        {
            var dead = new List<RespawnTimer>();

            foreach (var timer in _respawnTimerList.Where(timer => timer.Tick()))
            {
                dead.Add(timer);
            }

            foreach (var deadtimer in dead)
            {
                _respawnTimerList.Remove(deadtimer);
                deadtimer.Spawn(_game);
            }
        }
    }
}