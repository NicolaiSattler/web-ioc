using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace web_ioc.Models
{
    public class GribService : IGribService
    {
        private IGribSession _session;

        public GribService(IGribSession session)
        {
            _session = session;
        }
    }
}