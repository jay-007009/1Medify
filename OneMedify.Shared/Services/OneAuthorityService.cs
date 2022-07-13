using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using OneMedify.DTO.Response;
using OneMedify.DTO.User;
using OneMedify.Shared.Contracts;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OneMedify.Shared.Services
{
    public class OneAuthorityService : IOneAuthorityService
    {
        private readonly IRestServiceClient _restServiceClient;

        private readonly Configuration _configuration;

        public OneAuthorityService(IRestServiceClient restServiceClient, Configuration configuration)
        {
            _restServiceClient = restServiceClient;
            _configuration = configuration;
        }


        public virtual async Task<ResponseDto> RegisterUser(UserRegisterModel authorityRegisterModel)
        {
            string baseUri = "http://103.249.120.58:8501/api/userregistration";

            var token = await GenerateToken();
            var response = await _restServiceClient.InvokePostAsync<UserRegisterModel, ResponseDto>(baseUri, authorityRegisterModel, token);
            return response;
        }

        private async Task<string> GenerateToken()
        {
            string tokenUrl = $"http://103.249.120.58:8501/connect/token";
            string clientId = "1Medify";
            string secret = "secret";
            var tokenClient = new TokenClient(tokenUrl, clientId, secret);
            try
            {
                var tokenResponse = await tokenClient.RequestClientCredentialsAsync("1AuthorityApi");
                string token = string.Format("Bearer {0}", tokenResponse?.AccessToken);
                return token;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class Configuration : IConfiguration
    {
        private readonly IConfiguration _configuration;

        public string this[string key]
        {
            get => _configuration[key];
            set => _configuration[key] = value;
        }

        public Configuration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public virtual IConfigurationSection GetSection(string key)
        {
            return _configuration.GetSection(key);
        }

        public virtual IEnumerable<IConfigurationSection> GetChildren()
        {
            return _configuration.GetChildren();
        }

        public virtual IChangeToken GetReloadToken()
        {
            return _configuration.GetReloadToken();
        }
    }
}

