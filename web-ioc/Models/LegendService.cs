namespace web_ioc.Models
{
    public class LegendService : ILegendService
    {
        public ISessionModel Session { get; }
        public bool Touched { get; set; }

        public LegendService(ISessionModel session)
        {
            Session = session;
        }
    }
}