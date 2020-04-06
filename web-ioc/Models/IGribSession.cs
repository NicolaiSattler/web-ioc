using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace web_ioc.Models
{
    public interface IGribSession : ISessionModel
    {
        bool GribSession { get; }
    }
}