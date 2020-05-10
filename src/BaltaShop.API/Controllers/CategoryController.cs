using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BaltaShop.API.Data;
using BaltaShop.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using static BaltaShop.API.Extensions.CustomAuthorize;

namespace BaltaShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)] //Cache de 30 minutos
        public async Task<ActionResult<List<Category>>> Get([FromServices] DataContext context)
        {
            var categories = await context.Categories.AsNoTracking().ToListAsync();
            return Ok(categories);
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById(int id, [FromServices] DataContext context)
        {
            var categories = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            return Ok(categories);
        }

        [HttpPost]
        [ClaimsAuthorize("dev")]
        public async Task<ActionResult<Category>> Post([FromBody] Category model, 
                                                       [FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Categories.Add(model);
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = $"Ocorreu um erro ao inserir a Categoria. Erro: {e.Message}." });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [ClaimsAuthorize("dev")]
        public async Task<ActionResult<Category>> Put(int id, [FromBody] Category model,
                                                              [FromServices] DataContext context)
        {
            if (id != model.Id)
                return NotFound(new { message = "Categoria não encontrada." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = $"Ocorreu um erro ao atualizar a Categoria. Erro: {e.Message}." });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [ClaimsAuthorize("dev")]
        public async Task<ActionResult<Category>> Delete(int id, [FromServices] DataContext context)
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return NotFound(new { message = "Categoria não encontrada" });

            try
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok(new { message = "Categoria excluída com sucesso." });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = $"Ocorreu um erro ao excluir a Categoria. Erro: {e.Message}." });
            }
        }
    }
}