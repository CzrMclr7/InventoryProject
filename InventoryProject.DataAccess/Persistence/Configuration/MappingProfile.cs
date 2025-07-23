using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using InventoryProject.DataAccess.DataContextModels;
using InventoryProject.DataAccess.Models;

namespace InventoryProject.DataAccess.Persistence.Configuration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductModel>().ReverseMap();
            CreateMap<Sale, SalesModel>().ReverseMap();
            CreateMap<SalesDetail, SalesDetailModel>().ReverseMap();
            CreateMap<User, UserModel>().ReverseMap();
            CreateMap<UserAccess, UserAccessModel>().ReverseMap();
            CreateMap<ProductAdjustment, ProductAdjustmentModel>().ReverseMap();
            //CreateMap<reportd, UserModel>().ReverseMap();
        }
    }
}
