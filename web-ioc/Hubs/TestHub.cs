using Microsoft.AspNet.SignalR;
using web_ioc.Models;

namespace web_ioc.Hubs
{
    public class TestHub : Hub
    {
        private readonly ILegendService _service;

        public TestHub(ILegendService service)
        {
            _service = service;
        }

        public void activate()
        {
            Clients.Caller.Activated();
        }
    }
}