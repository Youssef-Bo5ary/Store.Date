using Store.Date.Entities;
using Store.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductEntity = Store.Date.Entities.Product;
using Store.Service.Services.ProductServices;
using Store.Service.Services.ProductServices.Dtos;
using AutoMapper;
using Store.Repository.Specifications.ProductSpecs;
using Store.Service.Helper;


namespace Store.Services.ProductServices
{
    public class ProductService : IProductService { 
        private readonly IUnitOfwork unitOfwork;
        private readonly IMapper mapper;

        public ProductService(IUnitOfwork unitOfwork,IMapper mapper)
        {
            this.unitOfwork = unitOfwork;
            this.mapper = mapper;
        }

        public async Task<IReadOnlyList<BrandTypeDetailsDto>> GetAllBrandsAsync()
        {
            var brands = await unitOfwork.Repository<ProductBrand, int>().GetAllAsNoTrackingAsync();
            var mappedBrands = mapper.Map < IReadOnlyList < BrandTypeDetailsDto >>(brands);
            return mappedBrands;
        }

        public async Task<PaginatedResultDto<ProductDetailsDto>> GetAllProductsAsync(ProductSpecification input)
        {
            var specs=new ProductWithSpecifications(input);
            var products = await unitOfwork.Repository<ProductEntity, int>().GetAllWithSpecificationAsync(specs);
            var countSpecs = new ProductWithCountSpecifications(input);
            var count = await unitOfwork.Repository<ProductEntity, int>().GetCountSpecificationAsync(countSpecs);
            var mappedProducts = mapper.Map < IReadOnlyList < ProductDetailsDto >>(products);
            return new PaginatedResultDto<ProductDetailsDto>(input.PageSize,input.PageIndex,count,mappedProducts);
        }

        public async Task<IReadOnlyList<BrandTypeDetailsDto>> GetAllTypesAsync()
        {
            var types = await unitOfwork.Repository<ProductType, int>().GetAllAsNoTrackingAsync();
            var mappedTypes = mapper.Map<IReadOnlyList<BrandTypeDetailsDto>>(types);
            return mappedTypes;
        }

        public async Task<ProductDetailsDto> GetProductByIdAsync(int? productId)
        {
            if (productId is null) throw new Exception("Id is Null");

            var specs= new ProductWithSpecifications(productId);


            var products = await unitOfwork.Repository<ProductEntity, int>().GetWithSpecificationByIdAsync(specs);
            
            if (products == null) throw new Exception("Product Not Found");
            var mappedProduct = mapper.Map<ProductDetailsDto>(products);
            return mappedProduct;

        }
    }
}
