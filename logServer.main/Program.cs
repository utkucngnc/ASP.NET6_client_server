using System.Diagnostics.Metrics;
using System.Diagnostics.Tracing;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace logServer.main
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            RunServer().Wait();
        }


        static async Task RunServer()
        {
            int milliseconds = 300;
            int num_iter = 0;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7023");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Console.WriteLine("Server is initializing...");

            while (true)
            {
                HttpResponseMessage response = await client.GetAsync("/api/Log");
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var logs = await response.Content.ReadFromJsonAsync<IEnumerable<LogDataObj>>();
                    
                    if (logs == null)
                    {
                        if (num_iter == 10)
                        {
                            Console.WriteLine("No incoming request for a long time, server is shutting down");
                            break;
                        }
                        int delay = num_iter * milliseconds;
                        Console.WriteLine(string.Format("No loggings was found. Waiting {0} seconds for the next cycle...",delay/1000 ));
                        Thread.Sleep(delay);
                        continue;
                    }
                    else
                    {
                        foreach (var log in logs)
                        {
                            if (log.Status == 0)
                            {
                                await Process(log, client);
                            }
                        }
                    }
                    
                }
            }            
        }

        static async Task Process(LogDataObj log, HttpClient client)
        {
            var pckg = new DelDataObj
            {
                Name = log.LogName,
                Content = (log.ID % 2) == 0 ? "Even" : "Odd",
                Response = true
            };

            string s1 = string.Format("api/Log/id?id={0}", log.ID);

            HttpClient newClient = new()
            {
                BaseAddress = new Uri("https://localhost:7039")
            };
            newClient.DefaultRequestHeaders.Accept.Clear();
            newClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Console.Out.WriteLine("Connecting with client: " + log.LogName);
            HttpResponseMessage response = await newClient.PostAsJsonAsync("/api/Delivery", pckg);
            newClient.Dispose();
            LogDataObj logData = log;

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("\nDelivery sent successfully!!\n\n");
                logData.Status = Status.Success;
                logData.DoneAt = DateTime.Now;
            }
            else
            {
                Console.WriteLine("\nDelivery failed!!\n\n");
                logData.Status = Status.Failure;
                logData.DoneAt = DateTime.Now;
            }

            await client.PutAsJsonAsync(s1, logData);
        }
    }
}