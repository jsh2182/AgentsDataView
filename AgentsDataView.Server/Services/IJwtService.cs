using AgentsDataView.Entities;

namespace AgentsDataView.Services
{
    public interface IJwtService
    {
        TokenBundle Generate(SystemUser user);
    }
}