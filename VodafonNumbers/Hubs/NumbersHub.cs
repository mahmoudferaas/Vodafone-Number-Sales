using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VodafonNumbers.Hubs
{

    [HubName("NumbersHub")]
    public class NumbersHub:Hub
    {
        public static void Show()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NumbersHub>();
            context.Clients.All.DispalyNumbers();
        }
        
    }
}