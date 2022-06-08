using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

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
                aTimer.Dispose();
                EndCycle();
                //exit cycle
            }
        }

        private static int StartCycle()// run to determind hosting and initial actions if those actions can be taken
        {
            //check serverstatus
            if(!checkServerStatus())
            {
                //check allocation
                if (!checkDNSFlag())
                {
                    //set allocation
                    setDNSFlag(true);
                    updateDNS();
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

        private static int EndCycle()
        {
            //deallocate recourses
            setDNSFlag(false);
            //stop server ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
            //run end of session cyle
            interGameBK(false);
            return 0;
        }

        private static void OnTimedEvent(object source)//backup cycle
        {
            fileBK();
        }

        private static void updateDNS()
        {
            //call dyudns api
            //http://api.ipify.org/ Port:80
            //http://api.dynu.com/nic/update?hostname=BigPPBoys.ooguy.com&myip='+ip+'&myipv6=no&password=aac699232386fc400bc756468f9baa95
        }

        private static void setDNSFlag(Boolean value)
        {
            //rename "true.MD" to "false.MD" or the otherway around
        }

        private static Boolean checkServerStatus()//returns true if server is runnung
        {
            //https://mcapi.us/server/status?ip=s.nerd.nu&port=25565
            return true;
        }

        private static Boolean checkDNSFlag()//returns true if file "true.MD" exists
        {
            //check if file "true.MD" exists
            return true;
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
    }


    public class API_Call
    {
        
    }
}