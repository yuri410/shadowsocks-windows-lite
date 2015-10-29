using Shadowsocks.Controller;
//using Shadowsocks.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
//using System.Windows.Forms;

namespace Shadowsocks
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Util.Utils.ReleaseMemory(true);
//            using (Mutex mutex = new Mutex(false, "Global\\Shadowsocks_" + Application.StartupPath.GetHashCode()))
//            {
//                Application.EnableVisualStyles();
//                Application.SetCompatibleTextRenderingDefault(false);

//                if (!mutex.WaitOne(0, false))
//                {
//                    Process[] oldProcesses = Process.GetProcessesByName("Shadowsocks");
//                    if (oldProcesses.Length > 0)
//                    {
//                        Process oldProcess = oldProcesses[0];
//                    }
//                    MessageBox.Show(I18N.GetString("Find Shadowsocks icon in your notify tray.") + "\n" +
//                        I18N.GetString("If you want to start multiple Shadowsocks, make a copy in another directory."),
//                        I18N.GetString("Shadowsocks is already running."));
//                    return;
//                }
//                //Directory.SetCurrentDirectory(Application.StartupPath);
////#if !DEBUG
////                Logging.OpenLogFile();
////#endif

//                ShadowsocksController controller = new ShadowsocksController();

//                //MenuViewController viewController = new MenuViewController(controller);

//                controller.Start();

//                //Application.Run();
//            }


            ShadowsocksController controller = new ShadowsocksController();

            //MenuViewController viewController = new MenuViewController(controller);

            controller.Start();

            if (controller.IsRunning)
                Thread.Sleep(Timeout.Infinite);
            else
            {
                string readmeFile = "readme.md";
                for (int i = 0; i < 4; i++)
                {
                    if (!File.Exists(readmeFile))
                    {
                        readmeFile = Path.Combine("..", readmeFile);
                    }
                    else break;
                }

                bool hasUsefulInfoRetrieved = false;
                if (File.Exists(readmeFile))
                {
                    Console.WriteLine("Sample configs.json file as following.");

                    string sampleConfig = File.ReadAllText(readmeFile);

                    const string pattern = "```";
                    int pos1 = sampleConfig.IndexOf(pattern);
                    int pos2 = sampleConfig.IndexOf(pattern, pos1 + pattern.Length);
                    if (pos1 !=-1 && pos2 !=-1 && pos2>pos1)
                    {
                        pos1 += pattern.Length;
                        Console.WriteLine(sampleConfig.Substring(pos1, pos2 - pos1).Trim());
                        hasUsefulInfoRetrieved = true;
                    }
                }

                if (!hasUsefulInfoRetrieved)
                {
                    Console.WriteLine("See readme for help.");
                }

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }

        }
    }
}
