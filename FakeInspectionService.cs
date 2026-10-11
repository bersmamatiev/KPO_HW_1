using VyshKat.Models;

namespace VyshKat.Services;

public class ServiceCenter : IInspectionService
{
    public bool Inspect(Transport transport)
    {
        return transport.IsOperational;
    }
}
