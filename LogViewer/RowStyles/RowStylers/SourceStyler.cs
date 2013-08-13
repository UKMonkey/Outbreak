using System.Collections.Generic;
using System.Drawing;
using LogViewer.LogFile;

namespace LogViewer.RowStyles.RowStylers
{
    public class SourceStyler : IRowStyler
    {
        private readonly List<string> _knownSources;
        private readonly List<Color> _foregroundColours;
        private readonly List<Color> _backgroundColours;

        public SourceStyler()
        {
            _knownSources = new List<string>();
            _backgroundColours = new List<Color>
                                 {
                                     Color.SkyBlue,
                                     Color.PaleGreen,
                                     Color.Moccasin,
                                     Color.Crimson,
                                     Color.Olive
                                 };
            _foregroundColours = new List<Color>
                                 {
                                     Color.Black,
                                     Color.Black,
                                     Color.Black,
                                     Color.White,
                                     Color.Yellow
                                 };
        }

        public RowStyling GetStyle(Row row)
        {
            if (!_knownSources.Contains(row.SourceIdentifier))
            {
                _knownSources.Add(row.SourceIdentifier);
            }

            var index = _knownSources.IndexOf(row.SourceIdentifier) % 5;

            var result = 
                new RowStyling
                {
                    BackgroundColor = _backgroundColours[index], 
                    ForegroundColor = _foregroundColours[index]
                };

            return result;
        }
    }
}