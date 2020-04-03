using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace web_ioc.Models
{
    public class SessionStore : ISessionStore
    {
        MemoryCache Cache = new MemoryCache("SessionStore", new System.Collections.Specialized.NameValueCollection());

        public SessionStore()
        {
        }

        public bool Contains(Guid id)
        {
            return Cache.Contains(id.ToString());
        }

        public ISessionModel Get(Guid id)
        {
            return Cache.Get(id.ToString()) as ISessionModel;
        }

        public void Set(ISessionModel session)
        {
            this.Cache.Add(session.Id.ToString(), session, new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 0, 10) });
        }
    }
}