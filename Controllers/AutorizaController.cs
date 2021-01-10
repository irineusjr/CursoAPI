using ApiCatalogo.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiCatalogo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutorizaController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AutorizaController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            return "AutorizaController - Acessado em " + DateTime.Now.ToLongDateString();
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser([FromBody] UsuarioDTO usermodel)
        {
            var user = new IdentityUser
            {
                UserName = usermodel.Email,
                Email = usermodel.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, usermodel.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _signInManager.SignInAsync(user, false);
            return Ok(GeraToken(usermodel));

        }
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UsuarioDTO userinfo)
        {
            //verifica as credenciais do usuario e retorna um valor
            var result = await _signInManager.PasswordSignInAsync(userinfo.Email, userinfo.Password,
                isPersistent: false, lockoutOnFailure: false);
            
            if (result.Succeeded)
            {
                return Ok(GeraToken(userinfo));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Login invalido...");
                return BadRequest(ModelState);
            }
        }

        //[HttpGet("recuperapassword")]
        //public async Task<IdentityUser> GetPassword()
        //{
        //    return await _userManager.FindByEmailAsync("irineusjr@outlook.com");
        //}

        private UsuarioToken GeraToken(UsuarioDTO userinfo)
        {
            //define declaracoes do usuario
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, userinfo.Email),
                new Claim(JwtRegisteredClaimNames.Email, userinfo.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //gera uma chave com base em algoritmo simetrico
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));
            //gera a assinatura digital do token, usando o algoritmo hmac e a chave privada
            var credenciais = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //obtem o tempo de expiracao - importante ser em UTC porque nunca se sabe onde o cliente esta
            var expiracao = _configuration["TokenConfiguration:ExpireHours"];
            var expirationTime = DateTime.UtcNow.AddHours(double.Parse(expiracao));

            //classe que representa o token JWT e gera o token
            JwtSecurityToken securityToken = new JwtSecurityToken(
                issuer : _configuration["TokenConfiguration:Issuer"],
                audience: _configuration["TokenConfiguration:Audience"],
                claims : claims,
                expires : expirationTime,
                signingCredentials : credenciais
            );

            //retorna os dados com o token e informacoes
            return new UsuarioToken
            {
                Authenticated = true,
                Expiration = expirationTime,
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),  //serializa o token gerado
                Message = "Token JWT OK"
            };

        }

    }
}