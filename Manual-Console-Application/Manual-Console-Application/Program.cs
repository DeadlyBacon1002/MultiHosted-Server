using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Manual_Console_Application;

namespace Multi_Host_Services_Manual
{
    class Program
    {
        static async Task Main(string[] args)
        {
            API api = new API();
            string res = await api.UpdateDNSP1();
            if (res.Contains("nochg")) { Console.WriteLine("No change"); }
            if (res.Contains("good")) { Console.WriteLine("penis"); }
            Console.ReadLine();

            /*int StartCode = await StartCycle();
            if (StartCode != 0)// initial error codes
            {
                if(StartCode == 1)
                {
                    Console.Write("Server is already hosted.\n\nPress enter to exit:");
                    Console.ReadLine();
                }
                if(StartCode == 2)
                {
                    Console.Write("Server flag is set to active.\n\nPress enter to exit:");
                    Console.ReadLine();
                }
                if (StartCode == 3)
                {
                    Console.Write("DNS failed to upload.\n\nPress enter to exit:");
                    Console.ReadLine();
                }
                if (StartCode == 4)
                {
                    Console.Write("Savefile failed to download.\n\nPress enter to exit:");
                    Console.ReadLine();
                }
            }
            else
            {
                var autoEvent = new AutoResetEvent(false);
                var aTimer = new System.Threading.Timer(OnTimedEvent, autoEvent, (60 * 60 * 1000), (60 * 60 * 1000));// start backup cycle every 60 minutes
                Console.WriteLine("press enter to exit");
                Console.ReadLine();
                aTimer.Dispose();
                EndCycle();
                //exit cycle
            }*/
        }

        private static async Task<int> StartCycle()// run to determind hosting and initial actions if those actions can be taken
        {
            API api = new API();
            //check serverstatus
            bool serverStatus = await api.checkServerStatus();
            if(serverStatus == false)
            {
                //check allocation
                if (!checkDNSFlag())
                {
                    //set allocation
                    setDNSFlag(true);
                    string res = await api.UpdateDNSP1();//check if dns succeeded 3
                    //run download serverfiles
                    fileDownload();//check whether download succeeded 4
                    interGameBK(true);
                    //Start Server |||||||||||||||||||||||||||||||||||||||||||||
                    return 0;
                } else
                {
                    return 2;
                }
            } else
            {
                return 1;
            }
        }

        private static void OnTimedEvent(object source)//backup cycle
        {
            fileBK();
        }

        private static int EndCycle()
        {
            //deallocate recourses
            setDNSFlag(false);
            //stop server ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
            //run end of session cyle
            interGameBK(false);
            return 0;
        }

        private static void fileBK()//LocalFilesystem
        {
            //delete oldest BK on drive
            //rename remaining BK's
            //upload current gamefile as newest BK
            //rename newest BK
        }

        private static void fileDownload()//Drive
        {
            //download most recent BK and rename to standard name on local machine
        }

        private static void interGameBK(Boolean StartOfSession)//Drive
        {
            //At start of program, copy most recent BK to inter-file
        }

        private static Boolean checkDNSFlag()//returns true if file "true.MD" exists//Drive
        {
            //check if file "true.MD" exists
            return true;
        }

        private static void setDNSFlag(Boolean value)//Drive
        {
            //rename "true.MD" to "false.MD" or the otherway around
        }
    }


}