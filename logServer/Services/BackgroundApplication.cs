using System.Net.Http.Headers;
using System.Security.Cryptography;
using logServer.Models;

namespace logServer.Services
{
    public class BackgroundApplication : BackgroundService
    {
        private readonly ILogger<BackgroundApplication> _logger;
        private HttpClient client;
        private int delayFactor, id;

        public BackgroundApplication(ILogger<BackgroundApplication> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            client = new HttpClient { BaseAddress = new Uri("https://localhost:7023") };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            delayFactor = 1;
            id = 3;

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            client.Dispose();
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested) 
            {
                var firstResponse = await client.GetAsync("api/Log");

                if(!firstResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Website is down!!!");
                    break;
                }
                else
                {
                    string s1 = $"api/Log/id?id={id}";
                    var secondResponse = await client.GetAsync(s1);

                    if (secondResponse.IsSuccessStatusCode)
                    {
                        delayFactor = 1;
                        var log = await secondResponse.Content.ReadFromJsonAsync<Log>();

                        if (log != null)
                        {                
                            if (log.LogName != null)
                            {
                                if (CheckPrime.IsPrime(log.Content)) _logger.LogInformation($"{log.Content} is a prime number.");
                                       
                                HttpClient tempClient = new HttpClient { BaseAddress = new Uri(log.LogName) };
                                _logger.LogInformation($"{log.LogName}");
                                tempClient.DefaultRequestHeaders.Accept.Clear();
                                tempClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                                while (true)
                                {
                                    var thirdResponse = await tempClient.GetAsync("api/Delivery");

                                    if (thirdResponse.IsSuccessStatusCode)
                                    {
                                        _logger.LogInformation("Connection successful");
                                        Delivery delivery = new Delivery {
                                            Content = CheckPrime.IsPrime(log.Content) ? string.Format("{0} is a prime number.", log.Content) : 
                                                string.Format("{0} is not a prime number.", log.Content),
                                            Delivered = DateTime.Now,
                                            };
                                        while (true)
                                        {
                                            var fourthResponse = await tempClient.PostAsJsonAsync("api/Delivery", delivery);

                                            if (fourthResponse.IsSuccessStatusCode)
                                            {
                                                _logger.LogInformation($"Response sent to {log.LogName}");
                                                log.Status = Status.Success;
                                                break;
                                            }
                                            else
                                            {
                                                _logger.LogError("Couldn't send, retrying");
                                            }
                                        }
                                        break;
                                    }
                                    else
                                    {
                                        _logger.LogError($"{log.LogName} is down. Trying again...");
                                        await Task.Delay(delayFactor * 1000);
                                    }
                                }
                                log.DoneAt = DateTime.Now;
                                while (true)
                                {
                                    var fifthResponse = await client.PutAsJsonAsync(s1, log);
                                    if (fifthResponse.IsSuccessStatusCode)
                                    {
                                        _logger.LogInformation("Log is updated");
                                    }
                                    else
                                    {
                                        _logger.LogError("Couldn't update the log, skipping..");
                                    }
                                    break;
                                }
                                tempClient.Dispose();
                                id++;
                            }
                        }
                    }
                    else
                    {
                        _logger.LogInformation("There isn't any log, increasing the delay factor.");
                        delayFactor++;
                    }
                }
                await Task.Delay(delayFactor* 1000);
            }
        }
    }
}
