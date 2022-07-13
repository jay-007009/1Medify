using IdentityServer4.AccessTokenValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace OneMedify.API.Config
{
    public class OneMedifyConfiguration : IConfiguration
    {
        private readonly IConfiguration _configuration;



        public string this[string key]
        {
            get => _configuration[key];
            set => _configuration[key] = value;
        }



        public SqlConnectionString ConnectionString { get; }



        public AuthenticationOptions Authority { get; }



        public OneMedifyConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = new SqlConnectionString(configuration);
            Authority = new AuthenticationOptions(configuration);
        }



        public IConfigurationSection GetSection(string key)
        {
            return _configuration.GetSection(key);
        }



        public IEnumerable<IConfigurationSection> GetChildren()
        {
            return _configuration.GetChildren();
        }



        public IChangeToken GetReloadToken()
        {
            return _configuration.GetReloadToken();
        }



        public sealed class SqlConnectionString
        {
            public SqlConnectionString(IConfiguration configuration)
            {
                configuration.Bind("ConnectionString", this);
            }



            public string CommandConnection { get; set; }



            public string QueryConnection { get; set; }
        }



        public sealed class AuthenticationOptions : IdentityServerAuthenticationOptions
        {
            public AuthenticationOptions(IConfiguration configuration)
            {
                configuration.Bind("1Authority", this);
            }
        }
    }
}