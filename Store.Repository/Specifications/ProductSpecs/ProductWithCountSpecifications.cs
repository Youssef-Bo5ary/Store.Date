using Store.Date.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Repository.Specifications.ProductSpecs
{
    public class ProductWithCountSpecifications:BaseSpecification<Product> 
     {
        public ProductWithCountSpecifications(ProductSpecification specs)
             : base(product => (!specs.BrandId.HasValue || product.BrandId == specs.BrandId.Value) &&
                            (!specs.TypeId.HasValue || product.TypeId == specs.TypeId.Value)&&
                            (string.IsNullOrEmpty(specs.Search) || product.Name.Trim().ToLower().Contains(specs.Search))
             )
        { 
        }
    }
}
