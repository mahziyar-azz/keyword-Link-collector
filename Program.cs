using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace link_collector
{
    internal static class Program
    {

        // This is the main entry point for the application.
        [STAThread]
        static void Main()
        {
            // Define a unique name for your mutex
            using (Mutex mutex = new Mutex(true, "{E2A81F35-AF69-4F80-81C4-5782544A38E2}"))
            {
                // Check if another instance is running
                if (mutex.WaitOne(TimeSpan.Zero, true))
                {
                    // If the mutex was successfully acquired, run the application
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());  // Replace Form1 with your main form
                }
                else
                {
                    // If another instance is running, bring focus to the application and make it vibrate or highlight
                    BringToFrontAndAlert();
                }
            }
        }

        // Method to bring the window to the front and highlight it
        private static void BringToFrontAndAlert()
        {
            IntPtr handle = FindWindow(null, "Link Collector"); // Replace with your actual window title
            SetForegroundWindow(handle);
            FlashWindow(handle, true);
            SystemSounds.Exclamation.Play();

            //MessageBox.Show("An instance of the application is already running.");
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool FlashWindow(IntPtr hWnd, bool bInvert);
    }
}
