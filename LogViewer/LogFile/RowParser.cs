using System;

namespace LogViewer.LogFile
{
    class RowParser : IRowParser
    {
        public Row Parse(string input)
        {
            var parts = input.Split('|');

            var timestamp = DateTime.Parse(parts[0]);
            var source = parts[1];
            var level = parts[2];
            var meta = parts[3];
            var text = parts[4];

            return new Row
                   {
                       Timestamp = timestamp,
                       SourceIdentifier = source,
                       Level = level,
                       Meta = meta,
                       Output = text,
                   };
        }
    }
}