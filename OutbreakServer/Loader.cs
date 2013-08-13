using Vortex.Interface;

namespace Outbreak.Server
{
    public static class Loader
    {
        /// <summary>
        /// executed via reflection.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IGameServer Load(StartArguments args)
        {
            return new GameServer(args);
        }
    }
}