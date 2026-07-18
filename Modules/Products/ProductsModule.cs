using verii_metivon_api.Modules.Products.Application.Parameters;
using verii_metivon_api.Modules.Products.Application.Services;

namespace verii_metivon_api.Modules.Products;

public static class ProductsModule
{
    public static IServiceCollection AddProductsModule(this IServiceCollection services)
    {
        services.AddScoped<IProductParameterService, ProductParameterService>();
        services.AddScoped<IProductService, ProductService>(); return services;
    }
}
