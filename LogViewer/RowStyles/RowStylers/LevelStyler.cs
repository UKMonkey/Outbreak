using System;
using System.Drawing;
using LogViewer.LogFile;
using Psy.Core.Logging;

namespace LogViewer.RowStyles.RowStylers
{
    public class LevelStyler : IRowStyler
    {
        public RowStyling GetStyle(Row row)
        {
            var result = new RowStyling();

            LoggerLevel level;
            if (!Enum.TryParse(row.Level, out level))
            {
                return result;
            }

            switch (level)
            {
                case LoggerLevel.Critical:
                    result.Bold = true;
                    result.BackgroundColor = Color.Red;
                    result.ForegroundColor = Color.White;
                    break;
                case LoggerLevel.Error:
                    result.Bold = true;
                    result.BackgroundColor = Color.DarkOrange;
                    result.ForegroundColor = Color.White;
                    break;
                case LoggerLevel.Warning:
                    result.Italic = true;
                    result.ForegroundColor = Color.Yellow;
                    result.BackgroundColor = Color.Black;
                    break;
                case LoggerLevel.Trace:
                    result.Italic = true;
                    result.Bold = true;
                    result.DarkenBackground = true;
                    break;
            }

            return result;
        }
    }
}