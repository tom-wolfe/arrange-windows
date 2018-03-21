using System.Collections.Generic;
using CommandLine;

namespace ArrangeWindows
{
    public class Options
    {
        [Option('w', "windows", Required = true, HelpText = "Titles of windows to be arranged.")]
        public IEnumerable<string> WindowTitles { get; set; }

        [Option('d', "display", Required = true, HelpText = "The index of the display to arrange within.")]
        public int DisplayIndex { get; set; }
    }
}
