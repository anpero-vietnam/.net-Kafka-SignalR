using KafkaSignalR.Pages.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace KafkaSignalRFrontend.Pages.Api
{
    [ApiController]
    [Route("api/[controller]")]    
    public class ComsumerController : ControllerBase
    {
        private readonly IHubContext<KafkaHub> kafkaHub;
        public ComsumerController(IHubContext<KafkaHub> _kafkaHub)
        {
            kafkaHub = _kafkaHub;
        }
       
        [HttpPost]
        public void Post([FromForm] string message)
        {
            if (message != null)
            {   
                kafkaHub.Clients.All.SendAsync("StartConsume", message);                
            }
        }
    }
}
