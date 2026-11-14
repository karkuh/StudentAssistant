using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;

namespace StudentAssistant.Services
{
    public static class ApiClient
    {
        public static HttpClient Client { get; private set; }

        public static void Initialize()
        {
            Client = new HttpClient();
            
            string baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "https://localhost:7295/api/";
            
            if (!baseUrl.EndsWith("/"))
                baseUrl += "/";

            Client.BaseAddress = new Uri(baseUrl);
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static void SetAuthToken(string token)
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}