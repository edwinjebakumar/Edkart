using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public ProductsController(IProductRepository repository)
        {
            _repository = repository;

        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts()
        {
            //var productList = await _context.Products.ToListAsync();
            var productList = await _repository.GetProductsAsync();
            return Ok(productList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            // return await _context.Products.FindAsync(id);
            return await _repository.GetProductByIdAsync(id);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            //var productList = await _context.Products.ToListAsync();
            var brandList = await _repository.GetProductBrandsAsync();
            return Ok(brandList);
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            //var productList = await _context.Products.ToListAsync();
            var typeList = await _repository.GetProductTypesAsync();
            return Ok(typeList);
        }
    }
}