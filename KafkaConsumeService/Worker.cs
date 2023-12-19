using Confluent.Kafka;
using Ultil;

namespace KafkaConsumeService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfigurationRoot _appConfig;
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _appConfig = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };
            string topics = "bizfly-7-636-test01";
            var config = new ConsumerConfig
            {
                BootstrapServers = _appConfig.GetValue<string>("Kafka:ConsumerSettings:BootstrapServers"),
                GroupId = _appConfig.GetValue<string>("Kafka:ConsumerSettings:GroupId"),
                SaslUsername = _appConfig.GetValue<string>("Kafka:ConsumerSettings:SaslUsername"),
                SaslPassword = _appConfig.GetValue<string>("Kafka:ConsumerSettings:SaslPassword"),
                SecurityProtocol = SecurityProtocol.SaslPlaintext,
                SaslMechanism = SaslMechanism.ScramSha512,
                EnableAutoOffsetStore = false,
                EnableAutoCommit = true,                
                EnablePartitionEof = true
            };

            // Note: If a key or value deserializer is not set (as is the case below), the 
            // deserializer corresponding to the appropriate type from Confluent.Kafka.Deserializers
            // will be used automatically (where available). The default deserializer for string
            // is UTF8. The default deserializer for Ignore returns null for all input data
            // (including non-null data).
            using (var consumer = new ConsumerBuilder<Ignore, string>(config)
                .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                .Build())
            {
                consumer.Subscribe(topics);

                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(cts.Token);

                            if (consumeResult.IsPartitionEOF)
                            {   
                                //Log => $"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");
                                continue;
                            }
                            // call RES api                             
                            HttpClientHelper.Post(_appConfig.GetValue<string>("frontendUrl:Url"), new Dictionary<string, string> { { "message", consumeResult.Message.Value } });

                            try
                            {
                                // Store the offset associated with consumeResult to a local cache. Stored offsets are committed to Kafka by a background thread every AutoCommitIntervalMs. 
                                // The offset stored is actually the offset of the consumeResult + 1 since by convention, committed offsets specify the next message to consume. 
                                // If EnableAutoOffsetStore had been set to the default value true, the .NET client would automatically store offsets immediately prior to delivering messages to the application. 
                                // Explicitly storing offsets after processing gives at-least once semantics, the default behavior does not.
                                consumer.StoreOffset(consumeResult);
                            }
                            catch (KafkaException e)
                            {
                                // log =>  e.Error.Reason

                            }
                        }
                        catch (ConsumeException e)
                        {
                            // log =>  e.Error.Reason
                        }
                        await Task.Delay(1000);
                    }
                }
                catch (OperationCanceledException)
                {
                    consumer.Close();
                }
            }
        }
    }
}