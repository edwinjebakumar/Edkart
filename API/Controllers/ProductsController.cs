using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Core.Specifications;
using API.DTOs;
using System.Linq;
using AutoMapper;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<ProductBrand> _brandRepo;
        private readonly IGenericRepository<ProductType> _typeRepo;
        private readonly IMapper _mapper;


        // private readonly IProductRepository _repository;

        public ProductsController(IGenericRepository<Product> productRepo,
                                    IGenericRepository<ProductBrand> brandRepo,
                                    IGenericRepository<ProductType> typeRepo,
                                    IMapper mapper)
        {
            _productRepo = productRepo;
            _brandRepo = brandRepo;
            _typeRepo = typeRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDTO>>> GetProducts()
        {
            //var productList = await _context.Products.ToListAsync();
            //var productList = await _productRepo.ListAllAsync();

            var spec = new ProductWithTypesAndBrandsSpecification();
            var productList = await _productRepo.ListAsync(spec);
            //return Ok(productList);
            // return productList.Select(p => new ProductToReturnDTO
            // {
            //     Id = p.Id,
            //     Name = p.Name,
            //     Description = p.Description,
            //     PictureUrl = p.PictureUrl,
            //     Price = p.Price,
            //     ProductBrand = p.ProductBrand.Name,
            //     ProductType = p.ProductType.Name
            // }).ToList();

            return Ok(_mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDTO>>(productList));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDTO>> GetProduct(int id)
        {
            // return await _context.Products.FindAsync(id);

            var spec = new ProductWithTypesAndBrandsSpecification(id);
            //return await _productRepo.GetEntityWithSpec(spec);
            var product = await _productRepo.GetEntityWithSpec(spec);
            // return new ProductToReturnDTO
            // {
            //     Id = product.Id,
            //     Name = product.Name,
            //     Description = product.Description,
            //     PictureUrl = product.PictureUrl,
            //     Price = product.Price,
            //     ProductBrand = product.ProductBrand.Name,
            //     ProductType = product.ProductType.Name
            // };

            return _mapper.Map<Product, ProductToReturnDTO>(product);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            //var productList = await _context.Products.ToListAsync();
            var brandList = await _brandRepo.ListAllAsync();
            return Ok(brandList);
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            //var productList = await _context.Products.ToListAsync();
            var typeList = await _typeRepo.ListAllAsync();
            return Ok(typeList);
        }
    }
}