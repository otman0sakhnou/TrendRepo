using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace TrendScope
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMongoCollection<BsonDocument> _trendsCollection;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var database = mongoClient.GetDatabase("TrendScopeDB");
            _trendsCollection = database.GetCollection<BsonDocument>("Trends");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                string apiKey = "7532e5764d4cb02f7dc0e350ed42e472e6b54819d61c9ca9138087f344417c75";
                string query = "artificial intelligence,machine learning,deep learning,AI technology,chatbots";


                string url = $"https://serpapi.com/search?engine=google_trends&q={query}&api_key={apiKey}";

                using (HttpClient httpclient = new HttpClient())
                {
                    try
                    {

                        HttpResponseMessage resMessage = await httpclient.GetAsync(url);

                        if (resMessage.IsSuccessStatusCode)
                        {
                            string jsonRes = await resMessage.Content.ReadAsStringAsync();
                            JObject data = JObject.Parse(jsonRes);


                            var trends = data["interest_over_time"]?["timeline_data"];

                            if (trends != null)
                            {
                                var mapData = trends
                                    .Select(day => new
                                    {
                                        Date = day["date"]?.ToString(),
                                        Queries = day["values"]?.Select(trend => new
                                        {
                                            Query = trend["query"]?.ToString(),
                                            Value = trend["value"]?.ToObject<int>() ?? default
                                        }).ToList()
                                    })
                                    .ToList();

                                var reduceData = mapData
                                    .SelectMany(d => d.Queries.Select(q => new { d.Date, q.Query, q.Value }))
                                    .GroupBy(entry => entry.Query)
                                    .Select(g => new
                                    {
                                        Query = g.Key,
                                        TotalValue = g.Sum(x => x.Value)
                                    });

                                foreach (var entry in reduceData)
                                {
                                    var document = new BsonDocument
                                    {
                                        { "Query", entry.Query },
                                        { "TotalValue", entry.TotalValue }
                                    };

                                    await _trendsCollection.InsertOneAsync(document, cancellationToken: stoppingToken);
                                }

                                _logger.LogInformation("Data successfully processed and stored in MongoDB.");
                            }
                            else
                            {
                                _logger.LogWarning("No trends found for the query");
                            }

                        }
                        else
                        {
                            _logger.LogError($"Error: {resMessage.StatusCode}, {resMessage.ReasonPhrase}");
                        }
                    }
                    catch (Exception ex)
                    {
                       _logger.LogError(ex, "Exception while fetching or processing data.");
                    }
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
            }
        }
    }
}
