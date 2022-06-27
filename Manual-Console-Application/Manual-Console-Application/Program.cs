using System;
using System.Threading.Tasks;
using Manual_Console_Application;
using System.IO;

namespace Multi_Host_Services_Manual
{
    class Program
    {
        static string[] arguments = new string[3];

        public static bool ConfigSetter()
        {
            string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            dir += "/config.json";
            arguments[0] = "6";
            arguments[1] = "C:/Users/Bacon/AppData/Roaming/.fabric";
            arguments[2] = "client_secret_240159371726-koqlnoj44uhvcfu3k8e48o6926r4sqju.apps.googleusercontent.com.json";
            return false;
        }

        static async Task Main(string[] args)
        {
            API a = new API();
            await a.UpdateDNSP1();
            return;
            GoogleDriveHandler GDH = new GoogleDriveHandler(arguments[2]);
            arguments[1] = "C:/Users/MrW/AppData/Roaming/.fabric";
            //await GDH.UpdateDrive();
            //GDH.uploadBKP("C:/Users/MrW/AppData/Roaming/.fabric/.L_BCK/BK1.7z");
            fileBK(arguments[1], "Multi-Host");
            //ExtractZip((arguments[1] + "/.L_BCK/BK1.7z"),(arguments[1] + "/.L_BCK/Extracted"));
            Console.ReadKey();
            return;
            int StartCode = await StartCycle(GDH);
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
            }
            else
            {
                MinecraftServerWrapper SERVER = new MinecraftServerWrapper(arguments);
                await SERVER.Start();
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
                        if(userInput == "help")
                        {
                            Console.WriteLine("||||||||||||||||||||||||||||||||||||||||||||\n\n"+
                                              "||||||||||||||||||||||||||||||||||||||||||||");
                            SERVER.WriteToServer(userInput);
                        }
                        else
                        {
                            SERVER.WriteToServer(userInput);
                        }
                    }
                }
                int code = await EndCycle(SERVER, GDH);
                if(code != 0)
                {
                    //Console.WriteLine("Some error has ocurred while closing the server");
                    Console.WriteLine("press enter to exit");
                    Console.ReadLine();
                }
                //exit cycle
            }
        }

        private static async Task<int> StartCycle(GoogleDriveHandler GDH)// run to determind hosting and initial actions if those actions can be taken
        {
            API api = new API();
            //check serverstatus
            bool serverStatus = await api.checkServerStatus();
            if(serverStatus == false)
            {
                serverStatus = await GDH.CheckFlag();
                if (!serverStatus)
                {
                    string res = await api.UpdateDNSP1();//check if dns succeeded 3
                    if(!res.Contains("nochg") && !res.Contains("good"))
                    {
                        return 3;
                    }
                    fileBK(arguments[1], "Multi-Host");
                    if (0 == await GDH.downloadSave())
                    {
                        DeleteFolderContentsRec((arguments[1] + "/" + "Multi-Host"), true);//delete current local save && misc files
                        File.Delete(arguments[1] + "/server.properties");
                        File.Delete(arguments[1] + "/usercache.json");
                        Directory.Delete((arguments[1] + "/" + "Multi-Host"));
                        var dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                        string[] allFiles = System.IO.Directory.GetFiles(dir + "/");
                        string fileName = "";
                        foreach (string file in allFiles)
                        {
                            if(file.Contains("BK1"))
                            {
                                fileName = file;
                                break;
                            }
                        }
                        ExtractZip((fileName),(arguments[1]+ "/" + "Multi-Host"));//delete downloaded file
                        File.Delete(fileName);
                        await GDH.SetFlag(true);
                    } 
                    else
                    {
                        Console.WriteLine("Drive file not found, continuing with local save");
                    }
                    return 0;
                } 
                else
                {
                    return 2;
                }
            } else
            {
                return 1;
            }
        }

        private static async Task<int> EndCycle(MinecraftServerWrapper SERVER, GoogleDriveHandler GDH)
        {
            
            await SERVER.Stop();
            fileBK(arguments[1], "Multi-Host");
            await GDH.UpdateDrive();
            string[] allFiles = System.IO.Directory.GetFiles(arguments[1] + "/.L_BCK/");//Change path to yours
            foreach(string fileName in allFiles)
            {
                if (fileName.Contains(arguments[1] + "/.L_BCK/BK1"))
                {
                    GDH.uploadBKP((arguments[1] + "/.L_BCK/"), fileName);
                }
            }
           //|||||||||||||||||||||||
            await GDH.SetFlag(false);
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
            string[] allFiles = System.IO.Directory.GetFiles(bkpDir+"/");//Change path to yours
            var FlagSorted = false;
            while(FlagSorted == false)
            {
                FlagSorted = true;
                for (int I = 0; I < (allFiles.Length-1); I++)
                {
                    
                    if (allFiles[I].TrimStart((bkpDir + "/").ToCharArray())[0] < allFiles[(I + 1)].TrimStart((bkpDir + "/").ToCharArray())[0])
                    {
                        var swopper = allFiles[I];
                        allFiles[I] = allFiles[(I + 1)];
                        allFiles[(I + 1)] = swopper;
                        FlagSorted = false;
                    }
                }
            }
            foreach (string file in allFiles)
            {
                if (file.Contains((bkpDir + "/BK" + 7)))
                {
                    File.Delete(file);
                }
                else
                {
                    for (int I = 1; I < 7; I++)
                    {
                        var Check = (bkpDir + "/BK" + I);
                        if (file.Contains(Check))
                        {
                            var Builder = file;
                            Builder = Builder.Replace(("/BK" + I), ("/BK" + (I + 1)));
                            File.Move(file, Builder);
                        }
                    }
                }
           
            }
            //.bkp folder has been prepped
            //now need to copy SaveFile to Bkp folder
            if(Directory.Exists((bkpDir + "/" + saveFileName)))
            {
                DeleteFolderContentsRec((bkpDir + "/" + saveFileName), true);
                Directory.Delete((bkpDir + "/" + saveFileName));
            }
            CopyFolderRec(SaveDir, (bkpDir + "/" + saveFileName), true);
            //add misc fils
            File.Copy((serverDir + "/server.properties"), (bkpDir + "/" + saveFileName + "/server.properties"));
            File.Copy((serverDir + "/usercache.json"), (bkpDir + "/" + saveFileName + "/usercache.json"));

            //and compress it to BK1.7z
            DateTime Stamp = DateTime.Now;
            var ZipName = (bkpDir + "/BK1-" + Stamp.Year + "'" + Stamp.Month + "'" + Stamp.Day + "-" + Stamp.Hour + "'" + Stamp.Minute + "'" + Stamp.Second  + ".7z");
            CreateZip((bkpDir + "/" + saveFileName), (ZipName));
            DeleteFolderContentsRec((bkpDir + "/" + saveFileName), true);
            Directory.Delete((bkpDir + "/" + saveFileName));
            Console.WriteLine("||||     Backup Made     ||||\n||||||      (OwO)      ||||||");
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

        public static void ExtractZip(string sourceName, string destination)
        {
            sourceName = @sourceName;
            destination = @destination;
            // If the directory doesn't exist, create it.
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);

            string zPath = @"C:\Program Files\7-Zip\7zG.exe";
            // change the path and give yours 
            try
            {
                System.Diagnostics.ProcessStartInfo pro = new System.Diagnostics.ProcessStartInfo();
                pro.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                pro.FileName = zPath;
                pro.Arguments = "x \"" + sourceName + "\" -o" + destination;
                System.Diagnostics.Process x = System.Diagnostics.Process.Start(pro);
                x.WaitForExit();
            }
            catch (System.Exception Ex)
            {
                //DO logic here 
            }
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

    }
}