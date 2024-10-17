using Microsoft.Extensions.Logging;
using Store.Date.Context;
using Store.Date.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Store.Repository
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(StoreDbContext DB,ILoggerFactory loggerFactory)
        {
            try
            {
                if (DB.ProductBrands != null && !DB.ProductBrands.Any()) 
                {
                    var brandsData = File.ReadAllText("../Store.Repository/SeedData/brands.json");
                    var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);
                    if(brands is not null)
                    {
                        await DB.ProductBrands.AddRangeAsync(brands);
                    }
                }
                if (DB.ProductTypes != null && !DB.ProductTypes.Any())
                {
                    var typeData = File.ReadAllText("../Store.Repository/SeedData/types.json");
                    var types = JsonSerializer.Deserialize<List<ProductType>>(typeData);
                    if (types is not null)
                    {
                        await DB.ProductTypes.AddRangeAsync(types);
                    }
                }
                if (DB.Products != null && !DB.Products.Any())
                {
                    var productData = File.ReadAllText("../Store.Repository/SeedData/products.json");
                    var products = JsonSerializer.Deserialize<List<Product>>(productData);
                    if (products is not null)
                    {
                        await DB.Products.AddRangeAsync(products);
                    }
                }

                if (DB.DeliveryMethods != null && !DB.DeliveryMethods.Any())
                {
                    var deliveryMethodsData = File.ReadAllText("../Store.Repository/SeedData/delivery.json");
                    var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodsData);
                    if (deliveryMethods is not null)
                    {
                        await DB.DeliveryMethods.AddRangeAsync(deliveryMethods);
                        await DB.SaveChangesAsync();
                    }
                }

            }
            catch (Exception exception) 
            { 
                var logger=loggerFactory.CreateLogger<StoreContextSeed>();
                logger.LogError(exception.Message);
            }

        }
    }
}
