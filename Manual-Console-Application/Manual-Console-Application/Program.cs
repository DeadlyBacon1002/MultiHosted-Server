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
using System.IO;

namespace Multi_Host_Services_Manual
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string[] arguments = new string[3];
            arguments[0] = "6";
            arguments[1] = "C:/Users/Bacon/AppData/Roaming/.fabric";
            int StartCode = await StartCycle();
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
                    Console.Write("DNS failed to update.\n\nPress enter to exit:");
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
                MinecraftServerWrapper SERVER = new MinecraftServerWrapper(arguments);
                await SERVER.Start();
                var autoEvent = new AutoResetEvent(false);
                var aTimer = new System.Threading.Timer(OnTimedEvent, autoEvent, (60 * 60 * 1000), (60 * 60 * 1000));// start backup cycle every 60 minutes
                bool Flag = false;
                string userInput;
                while(Flag == false)
                {
                    userInput = Console.ReadLine();
                    if(userInput == "stop" || userInput == "/stop")
                    {
                        Flag = true;
                    } 
                    else
                    {
                        SERVER.WriteToServer(userInput);
                    }
                }
                //aTimer.Dispose();
                int code = await EndCycle(SERVER);
                if(code != 0)
                {
                    //Console.WriteLine("Some error has ocurred while closing the server");
                    Console.WriteLine("press enter to exit");
                    Console.ReadLine();
                }
                //exit cycle
            }
        }

        private static async Task<int> StartCycle()// run to determind hosting and initial actions if those actions can be taken
        {
            API api = new API();
            //check serverstatus
            bool serverStatus = await api.checkServerStatus();
            if(serverStatus == false)
            {
                //check allocation
                //if (!checkDNSFlag())
                //{
                    //set allocation
                    //setDNSFlag(true);
                    string res = await api.UpdateDNSP1();//check if dns succeeded 3
                    if(!res.Contains("nochg") && !res.Contains("good"))
                    {
                        return 3;
                    }
                    //run download serverfiles
                    //fileDownload();//check whether download succeeded 4
                    //interGameBK(true);
                    //Start Server |||||||||||||||||||||||||||||||||||||||||||||
                    return 0;
                //} else
                //{
                    //return 2;
                //}
            } else
            {
                return 1;
            }
        }

        private static void OnTimedEvent(object source)//backup cycle
        {
            int code = fileBK("C:/Users/Bacon/AppData/Roaming/.fabric", "Multi-Host");
            if(code == 1)
            {
                Console.WriteLine("Savefile doesn't exist.");
            }
        }

        private static async Task<int> EndCycle(MinecraftServerWrapper SERVER)
        {
            //deallocate recourses
            await SERVER.Stop();
            //setDNSFlag(false);
            //stop server ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
            //run end of session cyle
            //interGameBK(false);
            return 0;
        }

        private static int fileBK(string serverDir, string saveFileName)//LocalFilesystem
        {
            string bkpDir = serverDir + "/.L_BCK";
            string SaveDir = serverDir + "/" + saveFileName;
            if (!Directory.Exists(SaveDir))
            {
                return 1;
            }
            if (!Directory.Exists(bkpDir))
            {
                Directory.CreateDirectory(bkpDir);
            }
            if (File.Exists((bkpDir + "/BK5.7z")))
            {
                File.Delete((bkpDir + "/BK5.7z"));
            }
            for(int I = 4; I > 0; I--)
            {
                if (File.Exists((bkpDir + "/BK"+I+".7z")))
                {
                    File.Move((bkpDir + "/BK" + I + ".7z"), (bkpDir + "/BK" + (I+1) + ".7z"));
                }
            }
            //.bkp folder has been prepped
            //now need to copy SaveFile to Bkp folder
            if(Directory.Exists((bkpDir + "/BKTemp")))
            {
                DeleteFolderContentsRec((bkpDir + "/BKTemp"), true);
                Directory.Delete((bkpDir + "/BKTemp"));
            }
            CopyFolderRec(SaveDir, (bkpDir + "/BKTemp"), true);
            //and compress it to BK1.7z
            CreateZip((bkpDir + "/BKTemp"), (bkpDir + "/BK1.7z"));
            DeleteFolderContentsRec((bkpDir + "/BKTemp"), true);
            Directory.Delete((bkpDir + "/BKTemp"));
            Console.WriteLine("||||     Backup Made     ||||");
            Console.WriteLine();
            Console.WriteLine("||||||      (OwO)      ||||||");
            return 0;
        }

        public static void CreateZip(string sourceName, string targetName)
        {
            sourceName = @sourceName;
            targetName = @targetName;
            System.Diagnostics.ProcessStartInfo p = new System.Diagnostics.ProcessStartInfo();
            p.FileName = @"C:\Program Files\7-Zip\7zG.exe";// might change
            p.Arguments = "a -t7z -m0=LZMA2 -mx=5 -mfb=32 -md=16m -ms=4g " + targetName + " " + sourceName;
            p.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            System.Diagnostics.Process x = System.Diagnostics.Process.Start(p);
            x.WaitForExit();
        }

        private static void DeleteFolderContentsRec(string sourceDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start deleting
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    DeleteFolderContentsRec(subDir.FullName, true);
                    Directory.Delete(subDir.FullName);
                }
            }
        }

        private static void CopyFolderRec(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyFolderRec(subDir.FullName, newDestinationDir, true);
                }
            }
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