using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.IServices;
using ECommerce.Application.Mapping;
using ECommerce.Application.Others;
using ECommerce.Application.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace ECommerce.Application
{
    public static class DependencyInjection
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IUserAddressService, UserAddressService>();
            services.AddScoped<IProductInventoryService, ProductInventoryService>();
            services.AddScoped<IProductVariationService, ProductVariationService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IProductDiscountService, ProductDiscountService>();
            services.AddScoped<IProductImageService, ProductImageService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IInventoryVariationService, InventoryVariationService>();
            services.AddScoped<IRedisCacheService>(provider =>
            {
                var connectionString = configuration.GetConnectionString("Redis");
                return new RedisCacheService(connectionString);
            });
            services.AddScoped<IBlobStorageHelper, BlobStorageHelper>();
            services.AddScoped<IVnPayService, VnPayService>();

            services.AddAutoMapperProfiles();
            services.AddRedis(configuration);
        }

        public static void AddAutoMapperProfiles(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(UserProfile).Assembly);
            services.AddAutoMapper(typeof(DiscountProfile).Assembly);
            services.AddAutoMapper(typeof(ProductProfile).Assembly);
            services.AddAutoMapper(typeof(CartProfile).Assembly);
            services.AddAutoMapper(typeof(UserAddressProfile).Assembly);
            services.AddAutoMapper(typeof(ProductInventoryProfile).Assembly);
            services.AddAutoMapper(typeof(OrderProfile).Assembly);
            services.AddAutoMapper(typeof(ImageProfile).Assembly);
        }

        public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConnectionMultiplexer>(opt =>
            {
                var redisUrl = configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(redisUrl!);
            });
        }
    }
}
