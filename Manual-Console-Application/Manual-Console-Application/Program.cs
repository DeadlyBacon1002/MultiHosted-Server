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
            string res = await updateDNS();
            if (res.Contains("nochg")) { Console.WriteLine("penis"); }
            if (res.Contains("good")) { Console.WriteLine("good"); }
            Console.WriteLine(res);
            Console.ReadLine();

            /*int StartCode = await StartCycle();
            if (StartCode != 0)
            {
                if(StartCode == 1)
                {
                    Console.Write("Server is already hosted.\n\nPress enter to exit:");
                    Console.ReadLine();
                }
                else
                {
                    Console.Write("Server flag is set to active.\n\nPress enter to exit:");
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
            API Validator = new API();
            //check serverstatus
            bool serverStatus = await Validator.checkServerStatus();
            if(serverStatus == false)
            {
                //check allocation
                if (!checkDNSFlag())
                {
                    //set allocation
                    setDNSFlag(true);
                    string res = await updateDNS();
                    //run download serverfiles
                    fileDownload();
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

        private static Boolean checkServerStatus()//returns true if server is runnung
        {
            //https://mcapi.us/server/status?ip=s.nerd.nu&port=25565
            return true;
        }

        private static async Task<string> updateDNS()
        {
            //call dyudns api
            API aa = new API();
            var result = await aa.UpdateDNSP1();
            return result;

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

        private static void interGameBK(Boolean StartOfSession)
        {
            //At start of program, copy most recent BK to inter-file
        }

        private static Boolean checkDNSFlag()//returns true if file "true.MD" exists
        {
            //check if file "true.MD" exists
            return true;
        }

        private static void setDNSFlag(Boolean value)
        {
            //rename "true.MD" to "false.MD" or the otherway around
        }
    }


}