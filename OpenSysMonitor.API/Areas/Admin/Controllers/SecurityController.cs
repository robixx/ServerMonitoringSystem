using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SysMonitor.Application;
using SysMonitor.Application.IInterface;
using SysMonitor.Infrastructure;

namespace OpenSysMonitor.API.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [AllowAnonymous]
   
    public class SecurityController : ControllerBase
    {

        private readonly IAuth _auth;
        private readonly JwtConfig _jwtConfig;
        public SecurityController(IAuth auth,  JwtConfig jwtConfig)
        {
            _auth = auth;
            _jwtConfig = jwtConfig;
        }



        [HttpPost("auth-user")]
        [EnableCors("AllowAll")]
        public async Task<IActionResult> UserLogInRequest([FromBody] AuthenticationRequest Auth)
        {
            if (Auth == null)
            {
                var json = new
                {
                    code = "106",
                    message = "Endpoint parameter required",
                    data = ""
                };

                return BadRequest(json);
            }
            var username = Auth.UserName;
            if (string.IsNullOrWhiteSpace(username))
            {
                var jsonData = new
                {
                    code = "108",
                    message = "Invalid username",
                    data = new
                    {
                        token = ""
                    }
                };
                return Unauthorized(jsonData);
            }

            try
            {
                var password = Auth.Password;

            }
            catch (Exception ex)
            {

                var jsonData = new
                {
                    code = "108",
                    message = "Invalid password during decryption. " + ex.StackTrace,
                    data = new
                    {
                        token = ""
                    }
                };
                return BadRequest(jsonData);
            }


            WebUserResponse? response = await _auth.LoginWebUser(Auth);
            if (response != null && response.UserId > 0)
            {
                JwtUser jwt = new()
                {
                    UserId = response.UserId,
                    EmployeeId = response.EmployeeId,
                    DispalyName = response.DispalyName,
                    RoleId = response.RoleId,
                    RoleName = response.RoleName,
                    TokenExpired = DateTime.Now.AddMinutes(30)
                };


                if (jwt != null)
                {
                    string strToken = _jwtConfig.Generate(jwt);                    
                    var jsonData = new
                    {
                        code = "200",
                        message = "Login Successfull",                       
                        token = strToken
                    };

                    return Ok(jsonData);
                }
                else
                {
                    var jsonData = new
                    {
                        code = "108",
                        message = "Invalid username/password",
                        token = ""
                    };
                    return Unauthorized(jsonData);
                }
            }
            else
            {
                var jsonData = new
                {
                    code = "108",
                    message = "Invalid username/password",
                    token = ""
                };
                return Unauthorized(jsonData);
            }
        }
    }
}
