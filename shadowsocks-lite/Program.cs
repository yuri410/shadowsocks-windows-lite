using Shadowsocks.Controller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Shadowsocks
{
    static class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [PreserveSig]
        public static extern uint GetModuleFileName
        (
            [In]
            IntPtr hModule,

            [Out]
            StringBuilder lpFilename,

            [In]
            [MarshalAs(UnmanagedType.U4)]
            int nSize
        );

        [DllImport("kernel32.dll")]
        static extern uint GetLastError();

        const uint ERROR_INSUFFICIENT_BUFFER = 122;

        public static string GetModuleFileName()
        {
            StringBuilder exeFilePath = new StringBuilder(8);
            while (GetModuleFileName(IntPtr.Zero, exeFilePath, exeFilePath.Capacity) != 0)
            {
                uint errCode = GetLastError();
                if (errCode == ERROR_INSUFFICIENT_BUFFER)
                {
                    exeFilePath.Clear();
                    exeFilePath.Capacity = exeFilePath.Capacity * 2;
                }
                else if (errCode == 0)
                    break;
                else return "";
            }
            return exeFilePath.ToString();
        }



        [STAThread]
        static void Main()
        {
            Util.Utils.ReleaseMemory(true);

            string exePath = GetModuleFileName();
            if (exePath.Length > 0)
            {
                exePath = Path.GetDirectoryName(exePath);
                Environment.CurrentDirectory = exePath;
            }

            ShadowsocksController controller = new ShadowsocksController();

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
