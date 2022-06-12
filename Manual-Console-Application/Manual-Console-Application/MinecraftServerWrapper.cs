using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Manual_Console_Application
{
    public class MinecraftServerWrapper
    {
        static Process process = null;
        public MinecraftServerWrapper(string[] args)
        { //java -Xmx2G -jar fabric-server-mc.1.18.2-loader.0.14.7-launcher.0.11.0.jar nogui
            ProcessStartInfo processToRunInfo = new ProcessStartInfo("java", "-Xmx" + args[0] + "G -jar fabric-server-mc.1.18.2-loader.0.14.7-launcher.0.11.0.jar nogui");
            processToRunInfo.RedirectStandardInput = true;
            processToRunInfo.CreateNoWindow = true;
            processToRunInfo.UseShellExecute = false;
            processToRunInfo.WorkingDirectory = @"" + args[1];
            processToRunInfo.FileName = "java.exe";
            process = new Process();
            process.StartInfo = processToRunInfo;
            //process.Close();
        }
        public async Task Start()
        {
            Console.WriteLine("Server is starting...");
            process.Start();
        }

        public async Task Stop()
        {
            try { process.StandardInput.WriteLine("stop"); }
            catch { }
        }
    }
}
