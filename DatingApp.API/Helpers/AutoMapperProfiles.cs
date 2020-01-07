using System;
using System.Linq;
using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDTO>()
            .ForMember(dest => dest.PhotoUrl, opt =>
                opt.MapFrom(source => source.Photos.FirstOrDefault(x => x.IsProfilePic).Url))
            .ForMember(dest => dest.Age, opt =>
                opt.MapFrom(source => source.DateOfBirth.CalcAge()));
            CreateMap<User, UserForDetailedDTO>()
            .ForMember(dest => dest.PhotoUrl, opt =>
                opt.MapFrom(source => source.Photos.FirstOrDefault(x => x.IsProfilePic).Url))
            .ForMember(dest => dest.Age, opt =>
                opt.MapFrom(source => source.DateOfBirth.CalcAge()));
            CreateMap<Photo, PhotosForDetailedDTO>();
            CreateMap<UserForUpdateDTO, User>();
        }

    }
}