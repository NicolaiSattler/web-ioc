using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using web_ioc.Models;

namespace web_ioc.Controllers
{
    [Authorize]
    public class ValuesController : ApiController
    {
        private ISessionModel _session;
        private ISessionStore _store;

        public ValuesController(ISessionModel session, ISessionStore store)
        {
            _session = session;
            _store = store;
        }

        // GET api/values
        public IEnumerable<string> Get()
        {
            var aa = (this.User.Identity as System.Security.Claims.ClaimsIdentity).Claims?.ToList();
            var id = aa.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);

            return new string[] { "openId Connect", id?.Value };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
