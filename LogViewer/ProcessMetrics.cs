namespace LogViewer
{
    public class ProcessMetrics
    {
        public int FileLoadExceptions;
        public int Batches;
        public int Lines;
        public int ProcessCalls;
        public int Queues;

        public override string ToString()
        {
            return string.Format(
                "Lines:{0}. Batches:{1}. Fails:{2}. Queues:{3}. Calls:{4}",
                Lines, Batches, FileLoadExceptions, Queues, ProcessCalls);
        }
    }
}