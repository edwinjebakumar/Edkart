using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Core.Specifications;
using API.DTOs;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using API.Errors;
using System.Net;
using API.Helpers;

namespace API.Controllers
{

    public class ProductsController : BaseApiController
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
        public async Task<ActionResult<Pagination<ProductToReturnDTO>>> GetProducts
                                                        ([FromQuery] ProductSpecParams productParams)
        {
            //var productList = await _context.Products.ToListAsync();
            //var productList = await _productRepo.ListAllAsync();

            var spec = new ProductWithTypesAndBrandsSpecification(productParams);
            var countSpec = new ProductWithFiltersForCountSpecification(productParams);
            var totalItems = await _productRepo.CountAsync(countSpec);
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

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDTO>>(productList);

            return Ok(new Pagination<ProductToReturnDTO>(productParams.PageIndex, productParams.PageSize, totalItems, data));
            //return Ok(_mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDTO>>(productList));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status404NotFound)]
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

            if (product == null)
            {
                var response = new APIResponse((int)HttpStatusCode.NotFound);
                return NotFound(response);
            }
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