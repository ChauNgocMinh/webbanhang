using PayPal.Api;
using System;

namespace WebBanHang.ViewModels
{
    public static class PaypalConfiguration
    {
        public static Dictionary<string, string> GetConfig(string mode)
        {
            return new Dictionary<string, string>()
            {
                {"mode",mode}
            };
        }

        private static string GetAccessToken(string ClientId, string ClientSecret, string mode)
        {
            // getting accesstocken from paypal  
            string accessToken = new OAuthTokenCredential(ClientId, ClientSecret, new Dictionary<string, string>()
            {
                {"mode",mode}
            }).GetAccessToken();
            return accessToken;
        }

        public static APIContext GetAPIContext(string clientId, string clientSecret, string mode)
        {
            // return apicontext object by invoking it with the accesstoken  
            APIContext apiContext = new(GetAccessToken(clientId, clientSecret, mode))
            {
                Config = GetConfig(mode)
            };
            return apiContext;
        }

    }
}
