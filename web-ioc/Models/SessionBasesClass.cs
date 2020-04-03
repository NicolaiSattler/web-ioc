using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace web_ioc.Models
{
    public class SessionBasesClass: ISessionBasesClass
    {
        public ISessionModel Session { get; set; }

        public SessionBasesClass(ISessionModel session)
        {
            Session = session;
        }
    }
}