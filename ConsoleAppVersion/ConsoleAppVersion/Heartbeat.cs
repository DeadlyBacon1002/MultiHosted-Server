using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ConsoleAppVersion
{
    public class Heartbeat
    {
        private readonly Timer _timer;

        public Heartbeat()
        {
            _timer = new Timer(60000) { AutoReset = true };
            _timer.Elapsed += timerElapsed;
        }

        private void timerElapsed(object sender, ElapsedEventArgs e)
        {
            //Console.WriteLine("penis");
            //throw new NotImplementedException();
            string[] lines = new string[] { DateTime.Now.ToString() };
            File.AppendAllLines(@"C:\Users\MrW\Documents\Dev\Github\MultiHosted-Server\ConsoleAppVersion\Output.txt", lines);
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
