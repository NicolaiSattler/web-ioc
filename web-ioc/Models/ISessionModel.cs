using System;

namespace web_ioc.Models
{
    public interface ISessionModel : IDisposable
    {
        Guid Id { get; }
        bool Value { get; set; }
    }
}
