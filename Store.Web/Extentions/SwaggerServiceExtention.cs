using Microsoft.OpenApi.Models;

namespace Store.Web.Extentions
{
    public static class SwaggerServiceExtention
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo { 
                    Title = "Store Api", 
                    Version = "v1", 
                    Contact=new OpenApiContact 
                    {
                        Name="Route",
                        Email="Route@gmail.com",
                        Url=new Uri("http://google.com")
                    }
                });
                var securitySchemc = new OpenApiSecurityScheme
                {
                    Description="JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name="Authorization",
                    In= ParameterLocation.Header,
                    Type=SecuritySchemeType.ApiKey,
                    Scheme="bearer",
                    Reference=new OpenApiReference
                    {
                        Id="bearer",
                        Type=ReferenceType.SecurityScheme
                    }
                };
                options.AddSecurityDefinition("bearer",securitySchemc);

                var securityRequirments = new OpenApiSecurityRequirement 
                {
                    {securitySchemc,new[] {"bearer"}}
                };
                options.AddSecurityRequirement(securityRequirments);
            });
        return services;
        }
    }
}
