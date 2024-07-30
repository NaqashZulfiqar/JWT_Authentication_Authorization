namespace Jwt_Authentication_Authorization.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        private string Password { get; set; }
    }
}
