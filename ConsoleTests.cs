using VyshKat.Models;

namespace VyshKat.Services;

public interface IInspectionService
{
    bool Inspect(Transport transport);
}
