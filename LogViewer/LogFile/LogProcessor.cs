using System.Collections.Generic;
using System.IO;

namespace LogViewer.LogFile
{
    public class LogProcessor
    {
        private readonly IRowParser _rowParser;
        private readonly string _filename;

        private int _rowNum;
        private long _bytes;

        public LogProcessor(string filename)
        {
            _rowParser = new RowParser();

            _filename = filename;
            _bytes = 0;
            _rowNum = 0;
        }


        public IEnumerable<Row> GetNewLines()
        {
            var previousReadBytes = _bytes;
            var ret = new List<Row>();

            using (var x = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                x.Seek(previousReadBytes, SeekOrigin.Begin);

                using (var y = new StreamReader(x))
                {
                    while (!y.EndOfStream)
                    {
                        var row = _rowParser.Parse(y.ReadLine());
                        row.RowNumber = ++_rowNum;
                        ret.Add(row);
                    }

                    _bytes = x.Length;
                }
            }

            return ret;
        }
    }
}
