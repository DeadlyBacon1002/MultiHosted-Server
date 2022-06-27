using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace Manual_Console_Application
{
    class GoogleDriveHandler
    {
        private static string PathToCredentials;
        private static FileDataStore tokenStorage = new FileDataStore("UserCredentialStoragePath", true);
        private static UserCredential credential;
        private static string dir;
        private static DriveService service;

        public GoogleDriveHandler(string ptc)
        {
            dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Console.WriteLine(dir);
            PathToCredentials = dir + "/UserCredentialStoragePath/" + ptc;
            using (FileStream stream = new FileStream(PathToCredentials, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        new[] { DriveService.ScopeConstants.Drive, DriveService.ScopeConstants.DriveMetadata },
                        "userName",
                        System.Threading.CancellationToken.None,
                        tokenStorage)
                    .Result;
            }
            service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });
        }

        public async Task UpdateDrive() 
        {
            //updates filenames and deletes oldest save
            
            var request = service.Files.List();
            var results = await request.ExecuteAsync();
            Google.Apis.Drive.v3.Data.File Temp = null;
            bool Flag = false;

            while(Flag == false) 
            {
                Flag = true;
                for(int I = 0; I < (results.Files.Count-2); I++)//sortfiles
                {

                    string one = (results.Files[I].Name.Substring(2, 1)), two = (results.Files[(I+1)].Name.Substring(2, 1));
                    if (!results.Files[I].Name.Contains(".flag") && !results.Files[(I + 1)].Name.Contains(".flag") && Int32.Parse(one) < Int32.Parse(two)) 
                    {
                        var drivefile = results.Files[I];
                        results.Files[I] = results.Files[(I + 1)];
                        results.Files[(I + 1)] = drivefile;
                        Flag = false;
                    }
                }
            }

            foreach (var drivefile in results.Files)
            {
                for(int I = 1; I <= 7; I++)
                {
                    if(drivefile.Name.Contains("BK" + I.ToString()))
                    {
                        if(I == 7)
                        {
                            await deleteFile(drivefile);
                        }
                        else 
                        {
                            string builder = drivefile.Name;
                            builder = builder.Replace(("BK" + I.ToString()), ("BK" + (I+1).ToString()));
                            await Rename(builder, drivefile);
                        }
                    }
                }
            }
            
        }

        public async Task deleteFile(Google.Apis.Drive.v3.Data.File File) 
        {
            if (File != null)
            {
                try
                {
                    var fileId = File.Id; //the ID of the folder or file you're renaming

                    service.Files.Delete(fileId).Execute();
                    Console.WriteLine("file deleted");
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                }
            }
            else
            {
                Console.WriteLine("An error occurred: File does not exist");
            }
        }

        public string uploadBKP(string filePath, string filename)
        {
            try
            {

                // Upload file photo.jpg on drive.
                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = filename
                };
                FilesResource.CreateMediaUpload request;
                // Create a new file on drive.
                using (var stream = new FileStream((filePath + "/" + filename),
                           FileMode.Open))
                {
                    // Create a new file, with metadata and stream.
                    request = service.Files.Create(
                        fileMetadata, stream, "application/x-7z-compressed");
                    request.Fields = "id";
                    request.Upload();
                }

                var file = request.ResponseBody;
                // Prints the uploaded file id.
                Console.WriteLine("File ID: " + file.Id);
                return file.Id;
            }
            catch (Exception e)
            {
                // TODO(developer) - handle error appropriately
                if (e is AggregateException)
                {
                    Console.WriteLine("Credential Not found");
                }
                else if (e is FileNotFoundException)
                {
                    Console.WriteLine("File not found");
                }
                else
                {
                    throw;
                }
            }
            return null;
        }

        public async Task<int> downloadSave()
        {
            var request = service.Files.List();
            request.Q = "name contains 'BK1'";
            var results = await request.ExecuteAsync();
            if (results.Files.Count == 1)
            {
                var downloadFile = results.Files.FirstOrDefault(file => file.Name.Contains("BK1"));
                var req = service.Files.Get(results.Files[0].Id);
                FileStream FS = new FileStream(downloadFile.Name, FileMode.Create, FileAccess.Write);
                req.MediaDownloader.ProgressChanged += async progress =>
                {
                    switch (progress.Status)
                    {
                        case DownloadStatus.Downloading:
                            {
                                Console.Clear();
                                decimal GB, MB, KB, B;
                                GB = Math.Floor(((decimal)progress.BytesDownloaded) / 1073741824);
                                MB = Math.Floor(((decimal)progress.BytesDownloaded - (GB * 1073741824)) / 1048576);
                                KB = Math.Floor(((decimal)progress.BytesDownloaded - ((GB * 1073741824) + (MB * 1048576))) / 1024);
                                B = ((decimal)progress.BytesDownloaded - ((GB * 1073741824) + (MB * 1048576) + (KB * 1024)));
                                if (GB == 0)
                                {
                                    if(MB == 0)
                                    {
                                        if(KB == 0)
                                        {
                                            Console.WriteLine(B + "B");
                                        }
                                        else
                                        {
                                            Console.WriteLine(KB + "KB");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine(MB + "MB");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine(GB + "GB");
                                }
                                //Console.WriteLine(GB + "GB | " + MB + "MB | " + KB + "KB | " + B + "B");
                                break;
                            }
                        case DownloadStatus.Completed:
                            {
                                Console.Clear();
                                decimal GB, MB, KB, B;
                                GB = Math.Floor(((decimal)progress.BytesDownloaded) / 1073741824);
                                MB = Math.Floor(((decimal)progress.BytesDownloaded - (GB * 1073741824)) / 1048576);
                                KB = Math.Floor(((decimal)progress.BytesDownloaded - ((GB * 1073741824) + (MB * 1048576))) / 1024);
                                B = ((decimal)progress.BytesDownloaded - ((GB * 1073741824) + (MB * 1048576) + (KB * 1024)));
                                Console.WriteLine(GB + "GB | " + MB + "MB | " + KB + "KB | " + B + "B");
                                Console.WriteLine("Download complete.");
                                break;
                            }
                        case DownloadStatus.Failed:
                            {
                                Console.WriteLine("Download failed.");
                                break;
                            }
                    }
                };
                try
                {//call download instance
                    using (FS)
                    {
                        req.DownloadWithStatus(FS);
                    }
                    return 0;
                }
                catch (Exception e)
                {
                    return 1;
                }
            }
            else
            {
                return 1;
            }
        }

        public async Task<bool> CheckFlag() 
        {
            var request = service.Files.List();
            request.Q = "name contains '.flag'";
            var results = await request.ExecuteAsync();

            if(results.Files.Count == 1)
            {
                if (results.Files[0].Name.ToLowerInvariant() == "true.flag")
                {
                    return true;
                }
                if (results.Files[0].Name.ToLowerInvariant() == "false.flag")
                {
                    return false;
                }
            }

            Console.WriteLine("Flag file not found");
            return true;
        }

        public async Task SetFlag(bool Type)
        {
            var request = service.Files.List();
            request.Q = "name contains '.flag'";
            var results = await request.ExecuteAsync();
            Google.Apis.Drive.v3.Data.File Temp = null;

            if (results.Files.Count == 1)
            {
                if (results.Files[0].Name.ToLowerInvariant() == ((!Type).ToString().ToLower() + ".flag"))
                {
                    Temp = results.Files[0];
                }
            }
            await Rename(((Type).ToString().ToLower() + ".flag"), Temp);
        }

        public async Task Rename(string newName, Google.Apis.Drive.v3.Data.File File)
        {
            if (File != null)
            {
                try
                {
                    Google.Apis.Drive.v3.Data.File file = new Google.Apis.Drive.v3.Data.File() { Name = newName };
                    var updateRequest = service.Files.Update(file, File.Id);
                    updateRequest.Fields = "name";
                    file = updateRequest.Execute();
                    Console.WriteLine("file updated");
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: " + e.Message);
                }
            }
            else
            {
                Console.WriteLine("An error occurred: File does not exist");
            }
        }
    }
}
