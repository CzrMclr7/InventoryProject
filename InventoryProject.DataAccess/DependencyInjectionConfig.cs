using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using InventoryProject.DataAccess.Models;
using InventoryProject.DataAccess.DataContextModels;
using Microsoft.EntityFrameworkCore;
using InventoryProject.DataAccess.Persistence.Configuration;
using InventoryProject.DataAccess.Services;
using InventoryProject.DataAccess.DataContext;
using InventoryProject.DataAccess.Persistence.Repositories.ProductRepo;
using InventoryProject.DataAccess.Persistence.Repositories.SalesRepo;
using InventoryProject.DataAccess.Persistence.Repositories.SalesDetailRepo;
using InventoryProject.DataAccess.Persistence.Repositories.UserRepo;
using InventoryProject.DataAccess.Services.Interface;
using InventoryProject.DataAccess.DataAccess.Interface;
using InventoryProject.DataAccess.DataAccess;
using InventoryProject.DataAccess.Persistence.Repositories.ProductAdjustmentRepo;
using InventoryProject.DataAccess.Persistence.Repositories.UserModuleAccessRepo;
using InventoryProject.DataAccess.Persistence.Repositories.ModuleRepo;
//using InventoryProject.DataAccess.Persistence.Repositories.UserModuleAccess;

namespace InventoryProject.DataAccess
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection
        services, IConfiguration configuration)
        {
            string? defaultConstr = configuration.GetConnectionString("Default");
            services.AddDbContext<InventoryProjectDatabaseContext>(options =>
            {
                options.EnableSensitiveDataLogging();
                options.UseSqlServer(defaultConstr);
            });
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
            services.AddSingleton<ISqlDataAccessService, SqlDataAccessService>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ISalesDetailRepository, SalesDetailRepository>();
            services.AddScoped<ISalesRepository, SalesRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IReportDA, ReportDa>();
            services.AddScoped<IProductAdjustmentRepository, ProductAdjustmentRepository>();
            services.AddScoped<IUserModuleAccessRepository, UserModuleAccessRepository>();
            services.AddScoped<IModuleRepository, ModuleRepository>();
            // Add user access repository

            return services;
        }
    }
}
