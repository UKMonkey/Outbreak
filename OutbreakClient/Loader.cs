using Vortex.Interface;

namespace Outbreak.Client
{
    public static class Loader
    {
        /// <summary>
        /// executed via reflection.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IGame Load(StartArguments args)
        {
            return new GameClient(args);
        }
    }
}