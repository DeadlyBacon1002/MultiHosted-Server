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
            //http://api.ipify.org/ Port:80
            //http://api.dynu.com/nic/update?hostname=BigPPBoys.ooguy.com&myip='+ip+'&myipv6=no&password=aac699232386fc400bc756468f9baa95
        }

        private static void setDNSFlag(Boolean value)
        {
            //rename "true.MD" to "false.MD" or the otherway around
        }

        private static void checkServerStatus()
        {
            //https://mcapi.us/server/status?ip=s.nerd.nu&port=25565
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

        private static void interGameBK(Boolean StartOfSession)
        {
            //At start of program, copy most recent BK to inter-file
        }
    }


    public class API_Call
    {
        static HttpClient client = new HttpClient();

        static async Task<Uri> CreateProductAsync(Product product)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/products", product);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }

        static async Task<Boolean> GetProductAsync(string path)
        {
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        static async Task<Product> UpdateProductAsync(Product product)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(
                $"api/products/{product.Id}", product);
            response.EnsureSuccessStatusCode();

            // Deserialize the updated product from the response body.
            product = await response.Content.ReadAsAsync<Product>();
            return product;
        }

        static async Task<HttpStatusCode> DeleteProductAsync(string id)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                $"api/products/{id}");
            return response.StatusCode;
        }

        static void Main()
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            // Update port # in the following line.
            client.BaseAddress = new Uri("http://localhost:64195/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Create a new product
                Product product = new Product
                {
                    Name = "Gizmo",
                    Price = 100,
                    Category = "Widgets"
                };

                var url = await CreateProductAsync(product);
                Console.WriteLine($"Created at {url}");

                // Get the product
                product = await GetProductAsync(url.PathAndQuery);
                ShowProduct(product);

                // Update the product
                Console.WriteLine("Updating price...");
                product.Price = 80;
                await UpdateProductAsync(product);

                // Get the updated product
                product = await GetProductAsync(url.PathAndQuery);
                ShowProduct(product);

                // Delete the product
                var statusCode = await DeleteProductAsync(product.Id);
                Console.WriteLine($"Deleted (HTTP Status = {(int)statusCode})");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
}