using System;

namespace LogViewer.LogFile
{
    public class Row : IComparable<Row>
    {
        public DateTime Timestamp { get; set; }
        public string Output { get; set; }
        public string SourceIdentifier { get; set; }
        public int RowNumber { get; set; }
        public string Level { get; set; }
        public string Meta { get; set; }

        public int CompareTo(Row other)
        {
            if (SourceIdentifier == other.SourceIdentifier)
            {
                // sort by row number
                return RowNumber - other.RowNumber;
            }
            return Timestamp < other.Timestamp ? -1 : 1;
        }
    }
}