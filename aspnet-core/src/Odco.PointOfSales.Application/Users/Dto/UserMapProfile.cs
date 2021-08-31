using AutoMapper;
using Odco.PointOfSales.Application.Productions.Categories;
using Odco.PointOfSales.Authorization.Users;
using Odco.PointOfSales.Core.Productions;

namespace Odco.PointOfSales.Users.Dto
{
    public class UserMapProfile : Profile
    {
        public UserMapProfile()
        {
            CreateMap<UserDto, User>();
            CreateMap<UserDto, User>()
                .ForMember(x => x.Roles, opt => opt.Ignore())
                .ForMember(x => x.CreationTime, opt => opt.Ignore());

            CreateMap<CreateUserDto, User>();
            CreateMap<CreateUserDto, User>().ForMember(x => x.Roles, opt => opt.Ignore());


            CreateMap<CategoryDto, Category>();
            CreateMap<Category, CategoryDto>();
        }
    }
}
