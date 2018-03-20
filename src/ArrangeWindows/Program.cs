using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                .WithParsed(Run)
                .WithNotParsed(HandleErrors);
        }

        private static void Run(Options options)
        {
            var processes = Process.GetProcesses().Where(p => options.WindowTitles.Contains(p.MainWindowTitle)).ToArray();
            var display = Screen.AllScreens[options.DisplayIndex].WorkingArea;

            var screenIsLandscape = display.Width > display.Height;
            var square = Math.Sqrt(processes.Length);
            var floor = (int)Math.Floor(square);
            var ceiling = (int)Math.Ceiling(square);
            var columns = screenIsLandscape ? ceiling : floor;
            var rows = screenIsLandscape ? floor : ceiling;

            var windowWidth = display.Width / columns;
            var windowHeight = display.Height / rows;

            for (var i = 0; i < processes.Length; i++)
            {
                var proc = processes[i];
                var column = i % columns;
                var row = (int)Math.Floor((decimal)i / columns);
                var newLeft = display.Left + windowWidth * column;
                var newTop = display.Top + windowHeight * row;

                // This is stupid. Apparently DPI issues cause the window size to be adjusted incorrectly.
                // To get around this, we first move the window to the appropriate monitor, then re-adjust.
                PInvoke.SetWindowPos(
                    proc.MainWindowHandle,
                    new IntPtr(0),
                    display.Left, display.Top,
                    windowWidth, windowHeight,
                    PInvoke.SWP_SHOWWINDOW
                );

                PInvoke.SetWindowPos(
                    proc.MainWindowHandle,
                    new IntPtr(0),
                    newLeft, newTop,
                    windowWidth, windowHeight,
                    PInvoke.SWP_SHOWWINDOW
                );
            }
        }

        private static void HandleErrors(IEnumerable<Error> errors)
        {
            // TODO.
        }
    }
}
