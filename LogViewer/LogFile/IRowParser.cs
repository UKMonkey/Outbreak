namespace LogViewer.LogFile
{
    public interface IRowParser
    {
        Row Parse(string input);
    }
}