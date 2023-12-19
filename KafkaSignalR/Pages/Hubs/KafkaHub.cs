using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.VisualBasic;
using System.Data.Common;
using System.Net;

namespace KafkaSignalR.Pages.Hubs
{
    public class KafkaHub : Hub
    {
        private readonly IConfigurationRoot _appConfig;
     
        public KafkaHub()
        {
            _appConfig = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();            
        }
        
        public async Task Producer(string message)
        {
          
            
            var config = new ProducerConfig
            {
                BootstrapServers = _appConfig.GetValue<string>("Kafka:ProducerSettings:BootstrapServers"),
                SecurityProtocol = SecurityProtocol.SaslPlaintext,
                SaslMechanism = SaslMechanism.ScramSha512,
                SaslUsername = _appConfig.GetValue<string>("Kafka:ProducerSettings:SaslUsername"),
                SaslPassword = _appConfig.GetValue<string>("Kafka:ProducerSettings:SaslPassword"),
            };
            
            using (var p = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    var dr =await p.ProduceAsync(_appConfig.GetValue<string>("Kafka:ProducerSettings:topic"), new Message<Null, string> { Value = message });                    
                    Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
        }  
    }
}
