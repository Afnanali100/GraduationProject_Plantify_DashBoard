using AutoMapper;
using ControlPanel.DAL.Models;
using ControlPanel.PLL.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace ControlPanel.PLL.Profiles
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterViewModel, ApplicationUser>();
            CreateMap<ApplicationUser, UserViewModel>().ReverseMap();

            CreateMap<IdentityRole, RoleViewModel>().ForMember(i=>i.RoleName,m=>m.MapFrom(r=>r.Name)).ReverseMap();
        }
    }
}
