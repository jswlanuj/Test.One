using Microsoft.Extensions.Options;
using Test.One.Models;

namespace Test.One
{
    public class SomeService
    {
        private readonly GoogleClientSettings _googleClientSettings;

        public SomeService(IOptions<GoogleClientSettings> googleClientOptions)
        {
            _googleClientSettings = googleClientOptions.Value;
        }

        public void UseGoogleClient()
        {
            string clientId = _googleClientSettings.ClientId;
            string clientSecret = _googleClientSettings.ClientSecret;
            // Use these securely
        }
    }
}
