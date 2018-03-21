using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CommandLine;

namespace ArrangeWindows
{
    public class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run);
        }

        private static Process[] FindSortedProcesses(IEnumerable<string> windowTitles)
        {
            // Find processes that have the listed titles.
            var matchingProcesses = Process.GetProcesses()
                .Where(p => windowTitles.Contains(p.MainWindowTitle))
                .ToArray();

            // Order the found procesess by the original title order.
            return windowTitles
                .Select(t => matchingProcesses.FirstOrDefault(p => p.MainWindowTitle == t))
                .Where(p => p != null)
                .ToArray();
        }

        private static WindowGrid GetWindowGrid(Rectangle display, int windowCount)
        {
            var square = Math.Sqrt(windowCount);
            var pColumns = (int)Math.Ceiling(square);
            var pRows = (int)Math.Ceiling((decimal)windowCount / pColumns);
            
            var screenIsLandscape = display.Width > display.Height;
            var columns = screenIsLandscape ? Math.Max(pColumns, pRows) : Math.Min(pColumns, pRows);
            var rows = screenIsLandscape ? Math.Min(pColumns, pRows) : Math.Max(pColumns, pRows);

            return new WindowGrid()
            {
                Columns = columns,
                WindowWidth = display.Width / columns,
                WindowHeight = display.Height / rows
            };
        }

        private static void AlignWindowToGrid(Rectangle display, WindowGrid grid, Process proc, int position)
        {
            var column = position % grid.Columns;
            var row = (int)Math.Floor((decimal)position / grid.Columns);
            var newLeft = display.Left + grid.WindowWidth * column;
            var newTop = display.Top + grid.WindowHeight * row;

            // This is stupid. Apparently DPI issues cause the window size to be adjusted incorrectly.
            // To get around this, we first move the window to the appropriate monitor, then re-adjust.
            PInvoke.SetWindowPos(
                proc.MainWindowHandle, PInvoke.HWND_TOP,
                display.Left, display.Top, grid.WindowWidth, grid.WindowHeight,
                PInvoke.SWP_SHOWWINDOW
            );
            PInvoke.SetWindowPos(
                proc.MainWindowHandle, PInvoke.HWND_TOP,
                newLeft, newTop, grid.WindowWidth, grid.WindowHeight,
                PInvoke.SWP_SHOWWINDOW
            );
        }

        private static void Run(Options options)
        {
            var processes = FindSortedProcesses(options.WindowTitles);
            if (processes.Length == 0) { return; }

            var display = Screen.AllScreens[options.DisplayIndex].WorkingArea;
            var grid = GetWindowGrid(display, processes.Length);

            for (var i = 0; i < processes.Length; i++)
            {
                var proc = processes[i];
                AlignWindowToGrid(display, grid, proc, i);
            }
        }
    }
}
