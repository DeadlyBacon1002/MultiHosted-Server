using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Multi_Host_Services_Manual
{
    class Program
    {
        static void Main(string[] args)
        {
            int StartCode = StartCycle();
            if (StartCode != 0)
            {
                Console.Write("Server is already hosted or hosting has not been deallocated.\n\nPress enter to exit:");
                Console.ReadLine();
            }
            else
            {
                var autoEvent = new AutoResetEvent(false);
                var aTimer = new System.Threading.Timer(OnTimedEvent, autoEvent, (60 * 60 * 1000), (60 * 60 * 1000));// start backup cycle every 60 minutes


                Console.WriteLine("press enter to exit");
                Console.ReadLine();
                //exit cycle
            }
        }

        private static int StartCycle()// run to determind ehosting and ititial actions if those actions can be taken
        {
            //check serverstatus

            //check allocation

            //run first cycle
            return 0;
        }

        private static void OnTimedEvent(object source)
        {
            Console.WriteLine(source.ToString());
        }


        private static void updateDNS()
        {
            //call dyudns api
        }

        private static void setDNSFlag(Boolean value)
        {
            //rename "true.MD" to "false.MD" or the otherway around
        }

        private static void checkDNSFlag()
        {
            //check if file "true.MD" exists
        }

        private static void fileBK()
        {
            //delete oldest BK on drive
            //rename remaining BK's
            //upload current gamefile as newest BK
            //rename newest BK
        }

        private static void fileDownload()
        {
            //download most recent BK and rename to standard name on local machine
        }

        private static void interGameBK()
        {
            //At start of program, copy most recent BK to inter-file
        }
    }
}