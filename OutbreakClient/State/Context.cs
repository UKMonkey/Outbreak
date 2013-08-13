namespace Outbreak.Client.State
{
    public class Context
    {
        public GameClient GameClient { get; private set; }

        public Context(GameClient gameClient)
        {
            GameClient = gameClient;
        }
    }
}