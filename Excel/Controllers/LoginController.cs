using Excel.AppService;
using Excel.EfCoreDb;
using Excel.IService;
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
using System.Threading.Tasks;

namespace Excel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly ILoginAppService _loginAppService;
        private readonly JwtSettings _jwt;
        private readonly ILogger<LoginController> _logger;
        private readonly IRabbitMqService _rabbitMqService;

        public LoginController(IOptions<JwtSettings> jwtOptions, ILogger<LoginController> logger, ILoginAppService loginAppService, IRabbitMqService rabbitMqService)
        {
            _jwt = jwtOptions.Value;
            _logger = logger;
            _loginAppService = loginAppService;
            _rabbitMqService = rabbitMqService;
        }

        
        /// <summary>
        /// 这个不用多说了吧（登录接口）
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        public async Task<UserResultVM> Login([FromBody] LoginVM vm)
        {
            var user = await _loginAppService.GetUserAsync(vm.Username, vm.orm);

            if (user == null)
            {
                _logger.LogInformation("用户登录失败，用户名：{Username}，请求体：{@LoginVM}", vm.Username, vm);
                throw new Exception("用户不存在");
            }

            if (vm.Username != user.username || vm.Password != user.password)
                throw new Exception("用户名或密码错误");

            ;
            if (!user.isenabled)
            {
                _logger.LogInformation("用户登录失败，用户名：{Username}，请求体：{@LoginVM}", vm.Username, vm);
                throw new Exception("用户已禁用");
            }
            ;

            var claims = new[]
            {
                new Claim("uid",user.id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );
            user.token = new JwtSecurityTokenHandler().WriteToken(token);
            _logger.LogInformation("用户登录，用户名：{Username}，请求体：{@LoginVM}", vm.Username, vm);

            // 构造通知 DTO
            var notification = new
            {
                UserId = user.id,
                Info = "登陆成功",
                Timestamp = DateTime.UtcNow
            };
            await _rabbitMqService.PublishAsync(notification, "login.login");

            return user;
        }
    }

}
