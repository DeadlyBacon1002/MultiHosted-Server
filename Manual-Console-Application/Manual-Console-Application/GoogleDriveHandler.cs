using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace Manual_Console_Application
{
    class GoogleDriveHandler
    {
        private const string PathToCredentials = @"C:\Users\MrW\Documents\Dev\Github\MultiHosted-Server\Manual-Console-Application\Manual-Console-Application\client_secret_240159371726-oe38ki8v2ffedg01chkognhvdqt4fnts.apps.googleusercontent.com.json";
        
        //public GoogleDriveHandler()
        //{

        //}





        public async Task DriveStuff(string[] args)
        {
            var tokenStorage = new FileDataStore("UserCredentialStoragePath", true);

            UserCredential credential;
            using (FileStream stream = new FileStream(PathToCredentials, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        new[] { DriveService.ScopeConstants.DriveReadonly },
                        "userName",
                        System.Threading.CancellationToken.None,
                        tokenStorage)
                    .Result;
            }

            //Drive service
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });

            var request = service.Files.List();
            var results = await request.ExecuteAsync();

            foreach(var drivefile in results.Files)
            {
                Console.WriteLine($"{drivefile.Name} {drivefile.MimeType} {drivefile.Id}");
            }


        }
    }
}
