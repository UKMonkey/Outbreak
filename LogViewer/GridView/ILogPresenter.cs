using LogViewer.LogFile;

namespace LogViewer.GridView
{
    public interface ILogPresenter
    {
        void Begin();
        void Add(Row row);
        void End();
    }
}