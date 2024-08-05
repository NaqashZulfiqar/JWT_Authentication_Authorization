using Jwt_Authentication_Authorization.Context;
using Jwt_Authentication_Authorization.Interfaces;
using Jwt_Authentication_Authorization.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Jwt_Authentication_Authorization.Services
{
    public class AuthService : IAuthService
    {
        private readonly JwtContext _context;
        private readonly IConfiguration _configuration;
        public AuthService(JwtContext context,IConfiguration configuration)
        {
            _context= context;
            _configuration= configuration;
        }

        public Role AddRole(Role role)
        {
            var addedRoles = _context.Roles.Add(role);
            _context.SaveChanges();
            return addedRoles.Entity;
        }

        public User AddUser(User user)
        {
            var addedUser = _context.Users.Add(user);
            _context.SaveChanges();
            return addedUser.Entity;            
        }

        public bool AssignRoleToUser(AddUserRole addUserRole)
        {
            try
            {
                var addRole = new List<UserRole>();
                var user = _context.Users.SingleOrDefault(s => s.ID == addUserRole.UserId);
                if (user == null)
                    throw new Exception("User is not valid");
                foreach (int role in addUserRole.RoleIds)
                {
                    var userRole = new UserRole();
                    userRole.RoleID = role;
                    userRole.UserID = user.ID;
                    addRole.Add(userRole);
                }
                _context.UserRoles.AddRange(addRole);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        public string Login(LoginRequest loginRequest)
        {
            if (loginRequest.UserName != null && loginRequest.Password != null)
            {
                var user = _context.Users.SingleOrDefault(s => s.UserName == loginRequest.UserName && s.Password == loginRequest.Password);
                if (user != null)
                {
                    var claims = new List<Claim> {
                        new Claim(JwtRegisteredClaimNames.Sub,_configuration["Jwt:Subject"]),
                        new Claim("Id",user.ID.ToString()),
                        new Claim("UserName",user.Name)
                    };
                    var userRole = _context.UserRoles.Where(u => u.UserID == user.ID).ToList();
                    var roleIds = userRole.Select(s => s.RoleID).ToList();
                    var roles = _context.Roles.Where(r => roleIds.Contains(r.Id)).ToList();
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.Name));
                        //claims.Add(new Claim("Role", role.Name));
                    }
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var sigin = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(10),
                        signingCredentials: sigin);

                    var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
                    return jwtToken;
                }
                else
                {
                    throw new Exception("User is not valid");
                }
            }
            else
            {
                throw new Exception("Credentials are not valid");
            }
        }
    }
}
