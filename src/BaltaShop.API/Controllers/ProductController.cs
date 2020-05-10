using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaltaShop.API.Data;
using BaltaShop.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using static BaltaShop.API.Extensions.CustomAuthorize;

namespace BaltaShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [ClaimsAuthorize("dev")]
        public async Task<ActionResult<List<Product>>> Get([FromServices] DataContext context)
        {
            var categories = await context.Products.Include(c => c.Category).AsNoTracking().ToListAsync();
            return Ok(categories);
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetById(int id, [FromServices] DataContext context)
        {
            var category = await context.Products.Include(c => c.Category).AsNoTracking()
                                                   .FirstOrDefaultAsync(p => p.CategoryId == id);

            return Ok(category);
        }

        [HttpGet]
        [Route("categories/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategory(int id, [FromServices] DataContext context)
        {
            var products = await context.Products.Include(c => c.Category).AsNoTracking()
                                                    .Where(c => c.CategoryId == id).ToListAsync();
            return Ok(products);
        }

        [HttpPost]
        [ClaimsAuthorize("dev")]
        public async Task<ActionResult<Product>> Post(Product model, [FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Products.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}