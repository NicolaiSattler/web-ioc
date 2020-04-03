using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace web_ioc.Models
{
    public interface ISessionStore
    {
        ISessionModel Get(Guid id);
        bool Contains(Guid id);
        void Set(ISessionModel session);
    }
}
