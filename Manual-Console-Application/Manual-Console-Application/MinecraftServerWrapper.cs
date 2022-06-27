using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;


namespace Manual_Console_Application
{
    public class MinecraftServerWrapper
    {
        public Process process = null;
        private StringBuilder output = new StringBuilder();
        public MinecraftServerWrapper(string[] args)
        { //java -Xmx2G -jar fabric-server-mc.1.18.2-loader.0.14.7-launcher.0.11.0.jar nogui
            ProcessStartInfo processToRunInfo = new ProcessStartInfo("java", "-Xmx" + args[0] + "G -jar fabric-server-mc.1.18.2-loader.0.14.7-launcher.0.11.0.jar nogui");
            processToRunInfo.RedirectStandardInput = true;
            processToRunInfo.RedirectStandardOutput = true;
            processToRunInfo.CreateNoWindow = true;
            processToRunInfo.UseShellExecute = false;
            processToRunInfo.WorkingDirectory = @"" + args[1];
            processToRunInfo.FileName = "java.exe";
            process = new Process();
            process.StartInfo = processToRunInfo;
            process.EnableRaisingEvents = true;
        }

        public async Task Start()
        {
            process.Start();
            process.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler((sender, e) =>
            {
                if (!String.IsNullOrEmpty(e.Data))
                {
                    output.Clear();
                    output.Append(e.Data);
                    Console.WriteLine(output);
                }
            });
            process.BeginOutputReadLine();
        }

        public async Task Stop()
        {
            try { process.StandardInput.WriteLine("stop");
                process.WaitForExit();
                process.Close();
            }
            catch { }
        }

        public void WriteToServer(string s)
        {
            if (!String.IsNullOrEmpty(s))
            {
                process.StandardInput.WriteLine(s);
            }
        }
    }
}
