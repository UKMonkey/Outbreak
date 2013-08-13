using Vortex.Interface;

namespace Outbreak.Server.Persistance.File.Base
{
    public abstract class FileHandler<TLoader, TSaver> : 
        Persistance.Base.Handler<TLoader, TSaver>
        where TLoader: FileLoader, new() 
        where TSaver: FileSaver, new()
    {
        protected IGame Game;

        protected FileHandler(IGame game, string typeName)
            :base(typeName)
        {
            Loader.Game = game;
            Saver.Game = game;
            Game = game;
        }
    }
}
