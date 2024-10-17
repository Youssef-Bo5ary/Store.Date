using Store.Date.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Repository.Specifications.ProductSpecs
{
    public class ProductWithSpecifications : BaseSpecification<Product>
    {
        public ProductWithSpecifications(ProductSpecification specs)
            : base(product => (!specs.BrandId.HasValue || product.BrandId == specs.BrandId.Value) &&
                            (!specs.TypeId.HasValue || product.TypeId == specs.TypeId.Value)&&
                            (string.IsNullOrEmpty(specs.Search)||product.Name.Trim().ToLower().Contains(specs.Search))
            )
        {
            AddInclud(x => x.Brand);
            AddInclud(x => x.Type);
            AddorderByAsc(x => x.Name);

            int pageIndex = Math.Max(1, specs.PageIndex); // Ensure PageIndex is at least 1
            int offset = specs.PageSize * (pageIndex - 1);

            ApplyPagination(offset, specs.PageSize);

            if (!string.IsNullOrEmpty(specs.Sort))
            {
                switch (specs.Sort)
                {
                    case "priceAsc":
                        AddorderByAsc(x => x.Price); 
                        break;

                    case "priceDesc":
                        AddorderByDesc(x => x.Price); 
                        break;

                    default:
                        AddorderByAsc(x => x.Name); 
                        break;
                }
            }
        }


        // One Product
        public ProductWithSpecifications(int? id): base(product => product.Id==id)
        {
            AddInclud(x => x.Brand);
            AddInclud(x => x.Type);
        }

    }
}
