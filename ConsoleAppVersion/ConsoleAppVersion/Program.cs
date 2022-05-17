using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace ConsoleAppVersion
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<Heartbeat>(s =>
                {
                    s.ConstructUsing(Heartbeat => new Heartbeat());
                    s.WhenStarted(Heartbeat => Heartbeat.Start());
                    s.WhenStopped(Heartbeat => Heartbeat.Stop());
                });

                x.RunAsLocalSystem();

                x.SetServiceName("HeartbeatService");
                x.SetDisplayName("Heartbeat Service");
                x.SetDescription("This is a functioning test service.");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}
