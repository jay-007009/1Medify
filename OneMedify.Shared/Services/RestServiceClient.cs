using OneMedify.DTO.Response;
using OneMedify.Shared.Contracts;
using RestSharp;
using System.Net.Http;
using System.Threading.Tasks;

namespace OneMedify.Shared.Services
{
    public class RestServiceClient : IRestServiceClient
    {
        private readonly HttpClient _httpClient;
        public RestServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public virtual async Task<ResponseDto> InvokePostAsync<T, R>(string requestUri, T model, string token = null) where R : new()
        {
            var client = new RestClient(requestUri);
            var restRequest = new RestRequest(requestUri, Method.POST)
            {
                OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; }
            };

            restRequest.AddJsonBody(model);

            if (!string.IsNullOrWhiteSpace(token))
            {
                restRequest.AddParameter("Authorization", token,
                ParameterType.HttpHeader);
            }
            IRestResponse<R> response = client.Execute<R>(restRequest);
            if (response.IsSuccessful)
            {
                return new ResponseDto
                {
                    StatusCode = 200
                };
            }
            return default;
        }
    }
}