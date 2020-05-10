using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaltaShop.API.Data;
using BaltaShop.API.Models;
using BaltaShop.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using static BaltaShop.API.Extensions.CustomAuthorize;

namespace BaltaShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Create([FromBody] User model,[FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Users.Add(model);
                await context.SaveChangesAsync();

                model.Password = "*";

                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "Não foi possível criar o usuário. Erro: " + e.Message });
            }
        } 

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> Login([FromBody] User model, [FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await context.Users.AsNoTracking()
                    .Where(u => u.Username == model.Username && u.Password == model.Password).FirstOrDefaultAsync();
                if (user == null)
                    return NotFound(new { message = "Usuário ou senha inválidos" });

                var token = TokenService.GenerateToken(user);

                user.Password = "*";

                return new
                {
                    user = user,
                    token = token
                };
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "Ocorreu um erro ao gerar o token. Erro: " + e.Message });
            }
        }

        [HttpGet]
        [Route("allUsers")]
        [ClaimsAuthorize("manager")]
        public async Task<ActionResult<List<User>>> GetUsers([FromServices] DataContext context)
        {
            var result = await context.Users.AsNoTracking().ToListAsync();
            return result;
        }
    }
}