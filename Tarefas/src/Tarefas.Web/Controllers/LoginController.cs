using System;
using Tarefas.DTO;
using Tarefas.DAO;
using AutoMapper;
using Tarefas.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Tarefas.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioDAO _usuarioDAO;
        private readonly IMapper _mapper;
        private readonly int _usuarioId;



        public LoginController(IUsuarioDAO usuarioDAO, IMapper mapper)
        {
            _usuarioDAO = usuarioDAO;
            _mapper = mapper;

        }

        public IActionResult Index()
        {
            return View();
        }


        UsuarioDTO user;

        [HttpPost]
        public IActionResult Index(UsuarioViewModel usuarioViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    user = _usuarioDAO.Autenticar(usuarioViewModel.Email, usuarioViewModel.Senha);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Nome),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.PrimarySid, user.Id.ToString())
                    };


                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                        IsPersistent = true,
                        RedirectUri = "/Login"
                    };

                    HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                    return LocalRedirect("/Home");
                }
                catch (Exception ex)
                {
                    // logica de tratamento da exceção que vamos adicionar

                    ModelState.AddModelError(string.Empty, "Email ou Senha Inválidos!");

                }
            }
            return View();
        }

        public IActionResult Sair()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/Login");

        }




    }
}
