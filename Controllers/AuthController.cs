using Jwt_Authentication_Authorization.Interfaces;
using Jwt_Authentication_Authorization.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Jwt_Authentication_Authorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }
        // POST: api/<AuthController>
        [HttpPost("login")]
        public string Login([FromBody]  LoginRequest loginRequest)
        {
            var token = _auth.Login(loginRequest);
            return token;
        }

        // POST api/<AuthController>/
        [HttpPost("assignRole")]
        public bool AssignRoleToUser([FromBody] AddUserRole userRole)
        {
            var addedUserRole = _auth.AssignRoleToUser(userRole);
            return addedUserRole;
        }

        // POST api/<AuthController>
        [HttpPost("addUser")]
        public User AddUser([FromBody] User user)
        {
            var addedUser = _auth.AddUser(user);
            return addedUser;
        }

        // POST api/<AuthController>/5
        [HttpPost("addRole")]
        public Role AddRole([FromBody] Role role)
        {
            var addedRole=_auth.AddRole(role);
            return addedRole;
        }       
    }
}
