using E_commerce.DTO;
using E_commerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace E_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        public AccountController(UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            
            _config = configuration;
        }


        [HttpPost("Register")]
       public async Task<ActionResult> Register(RegisterDto dto)
       {
            if(ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser();
                user.Email = dto.Email; user.UserName = dto.Username;
                IdentityResult result=  await _userManager.CreateAsync(user,dto.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors.FirstOrDefault());
                }
                // assign role
                await _userManager.AddToRoleAsync(user, "User");
                return Ok("User registered successfully");
            }
            else
            {
                return BadRequest(ModelState);
            }
       }


        [HttpPost("Register/admin")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult> RegisterAdmin(RegisterDto dto)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser();
                user.Email = dto.Email; user.UserName = dto.Username;
                IdentityResult result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors.FirstOrDefault());
                }
                // assign role 

                await _userManager.AddToRoleAsync(user, "Admin");
                return Ok("Admin registered successfully ");
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginDto dto)
        {
            if (ModelState.IsValid)
            {
               var user=  await _userManager.FindByEmailAsync(dto.Email);
                if(user != null)
                {
                    bool found =await _userManager.CheckPasswordAsync(user,dto.Password);
                    if (found)
                    {
                        var token =GenerateToken(user);
                        return Ok(new { Token = token});
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        // Delete
        
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete( string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user != null)
            {
              IdentityResult result=  await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return Ok("remove seccess");
                }
                else
                    return NotFound();
               
            }
            else
            {
                return NotFound();
            }
        }
        // 
        private async Task <string> GenerateToken(ApplicationUser user)
        {
            //create claims
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            //get roles
            var roles= await _userManager.GetRolesAsync(user);
            foreach (var item in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }
            //credentials 
            SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));

            SigningCredentials credentials =new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);
            // CREATE TOKEN
            JwtSecurityToken securityToken = new JwtSecurityToken
                (
                    issuer: _config["JWT:ValidIssuer"],
                    audience: _config["JWT:ValidAudiance"],
                    claims: claims,
                    expires: DateTime.Now.AddDays(2),
                    signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }

    }
}
