using Sentry;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Cloudforce
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [DllImport("gdi32.dll", EntryPoint="AddFontResourceW", SetLastError=true)]
        public static extern int AddFontResource([In][MarshalAs(UnmanagedType.LPWStr)]
                                         string lpFileName);


        [STAThread]


        static void Main()
        {

            SentrySdk.Init(o =>
            {
                // Tells which project in Sentry to send events to:
                o.Dsn = "https://764b6762054a498098fa542f8bd7cf23@o1302473.ingest.sentry.io/6539929";
                // When configuring for the first time, to see what the SDK is doing:
                o.Debug = true;
                // Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
                // We recommend adjusting this value in production.
                o.TracesSampleRate = 1.0;
            });



            if (!File.Exists(Path.GetTempPath() + "Font1.ttf"))
            {
                File.WriteAllBytes(Path.GetTempPath() + "Font1.ttf", Properties.Resources.NunitoSans_Bold);
                File.WriteAllBytes(Path.GetTempPath() + "Font2.ttf", Properties.Resources.NunitoSans_Regular);


                var font1 = AddFontResource(Path.GetTempPath() + "Font1.ttf");
                var font2 = AddFontResource(Path.GetTempPath() + "Font2.ttf");
                var error = Marshal.GetLastWin32Error();
                if (error != 0)
                {
                    Console.WriteLine(new Win32Exception(error).Message);
                }
                else
                {
                    Console.WriteLine((font1 == 0) ? "Font is already installed." :
                                                      "Font installed successfully.");
                    Console.WriteLine((font2 == 0) ? "Font is already installed." :
                                                      "Font installed successfully.");
                }
            }

            
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Apps");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            
        }
    }
}
