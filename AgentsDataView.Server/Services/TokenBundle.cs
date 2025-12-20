using AgentsDataView.Entities;

namespace AgentsDataView.Services
{
    public class TokenBundle
    {
        public AccessToken AccessToken { get; set; }
        public RefreshToken RefreshToken { get; set; }
    }
}
