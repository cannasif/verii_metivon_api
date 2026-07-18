using verii_metivon_api.Modules.Warehouses.Application.Services;using verii_metivon_api.Modules.Warehouses.Application.Parameters;
namespace verii_metivon_api.Modules.Warehouses;
public static class WarehousesModule{public static IServiceCollection AddWarehousesModule(this IServiceCollection s){s.AddScoped<IWarehouseService,WarehouseService>();s.AddScoped<IWarehouseParameterService,WarehouseParameterService>();return s;}}
