using Excel.Options;
using Excel.VM;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Excel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly JwtSettings _jwt;
        public LoginController(IOptions<JwtSettings> jwtOptions)
        {
            _jwt = jwtOptions.Value;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginVM vm)
        {
            // 1. 验证用户（示例硬编码）
            if (vm.Username != "admin" || vm.Password != "123456")
                return Unauthorized("用户名或密码错误");

            // 2. 构造声明
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, vm.Username),
            new Claim("role", "Admin"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            // 3. 创建签名凭据
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 4. 生成 Token
            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );
            var jwtStr = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { Token = jwtStr, Expires = token.ValidTo });
        }
    }

}
