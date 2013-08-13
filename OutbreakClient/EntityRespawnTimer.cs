using Beer.Interface;

namespace Daybreak
{
    class EntityRespawnTimer: RespawnTimer
    {
        public short EntityTypeId { get; set; }

        public override void Spawn(IGame game)
        {
            game.SpawnEntity(EntityTypeId);
        }
    }
}
