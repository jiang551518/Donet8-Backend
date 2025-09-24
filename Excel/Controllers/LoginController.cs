using Excel.Options;
using Excel.VM;
using Microsoft.AspNetCore.Authorization;
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
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly JwtSettings _jwt;
        private readonly ILogger<LoginController> _logger;
        public LoginController(IOptions<JwtSettings> jwtOptions, ILogger<LoginController> logger)
        {
            _jwt = jwtOptions.Value;
            _logger = logger;
        }

        [HttpPost]
        public object Login([FromBody] LoginVM vm)
        {
            var user = new Users()
            {
                id = Guid.Parse("fd008695-fdae-46d8-a630-88086e3eb048"),
                username = "admin",
                password = "123456",
                isenabled = true,
            };

            var users = new List<Users>();
            users.Add(user);

            if (vm.Username != user.username || vm.Password != user.password)
                return Unauthorized("用户名或密码错误");

            var userInfo = users.FirstOrDefault(x => x.username == vm.Username);

            if (userInfo == null) 
            {
                _logger.LogInformation("用户登录失败，用户名：{Username}，请求体：{@LoginVM}", vm.Username, vm);
                throw new Exception("用户不存在"); 
            };
            if (!userInfo.isenabled) 
            {
                _logger.LogInformation("用户登录失败，用户名：{Username}，请求体：{@LoginVM}", vm.Username, vm);
                throw new Exception("用户已禁用");
            };

            // 2. 构造声明
            var claims = new[]
            {
                new Claim("uid",user.id.ToString())
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
            _logger.LogInformation("用户登录，用户名：{Username}，请求体：{@LoginVM}", vm.Username, vm);

            return new { Token = jwtStr, Expires = token.ValidTo };
        }
    }

}
