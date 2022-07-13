using System.Collections.Generic;

namespace OneMedify.DTO.User
{
    public class UserRegisterModel 
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public string UserClient { get; set; }
        public List<Claims> Claims { get; set; }
        public List<Roles> Roles { get; set; }
    }
}
