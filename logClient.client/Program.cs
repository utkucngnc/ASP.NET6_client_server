using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using logClient.client.DataObjects;

namespace Client;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("App started\n\n");
        Globals.numRequest = 4;

        while (true)
        {
            Console.Write("Enter an integer (anything else to exit): ");
            RunRequest().Wait();
            if (Globals.req.Content != -1)
            {
                RunDelivery().Wait();
            }
            else
            {
                Console.WriteLine("\n\nExitting...\n\n");
                break;
            }
        }
    }

    static async Task RunRequest()
    {
        using (var serverClient = new HttpClient { BaseAddress = new Uri(Globals.serverAddress) } )
        {
            serverClient.DefaultRequestHeaders.Accept.Clear();
            serverClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            Globals.req = new ReqDataObj
            {
                LogName = Globals.baseAddress,
                Created = DateTime.Now,
                Content = Int32.TryParse(Console.ReadLine(), out int temp) ? Math.Abs(temp) : -1,
                Status = Status.Waiting
            };
            if (Globals.req.Content != -1)
            {
                await SendReq(serverClient, Globals.req);
            }                
        }
    }

    static async Task RunDelivery()
    {
        using (HttpClient client = new HttpClient { BaseAddress = new Uri(Globals.baseAddress) })
        {
            Console.Write("Waiting for delivery...");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            await GetDel(client);
        }
    }

    static async Task SendReq(HttpClient client, ReqDataObj req)
    {
        HttpResponseMessage response;

        Console.Write("\n\nRequesting...");
        while (true)
        {
            
            response = await client.PostAsJsonAsync("/api/Log", Globals.req);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("\nRequest sent successfully!!\n\n");
                break;
            }
            else
            {
                Console.Write(".");
                await Task.Delay(1000);
            }            
        }
    }

    static async Task GetDel(HttpClient client)
    {
        while (true)
        {
            Console.Write(".");
            HttpResponseMessage response = await client.GetAsync($"api/Delivery/id?id={Globals.numRequest}");

            if (response.IsSuccessStatusCode)
            {
                Globals.del = await response.Content.ReadFromJsonAsync<DeliveryDataObj>();

                Console.WriteLine("\nPackage is received.\n");
                Globals.numRequest++;
                break;
            }
            else
            {
                Console.Write(".");
                await Task.Delay(1000);
            }
        }
    }

    static class Globals
    {
        public static int numRequest;
        public static string serverAddress = "https://localhost:7023";
        public static string baseAddress = "https://localhost:7039";
        public static ReqDataObj? req;
        public static DeliveryDataObj? del;
    }
}



