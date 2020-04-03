namespace web_ioc.Models
{
    public interface ILegendService
    {
        bool Touched { get; set; }
        ISessionModel Session { get; }
    }
}