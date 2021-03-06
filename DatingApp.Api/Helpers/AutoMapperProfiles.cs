﻿using AutoMapper;
using DatingApp.Api.Dtos;
using DatingApp.Api.Models;
using System.Linq;

namespace DatingApp.Api.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, opt =>
                {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt =>
                {
                    opt.ResolveUsing(src => src.DateOfBirth.CalculateAge());
                });

            CreateMap<User, UserForDetaildDto>()
                .ForMember(dest => dest.PhotoUrl, opt =>
                {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt =>
                {
                    opt.ResolveUsing(src => src.DateOfBirth.CalculateAge());
                });
            CreateMap<UserForUpdateDto, User>();
            CreateMap<UserRegistrationDto, User>().ReverseMap();

            CreateMap<Photo, PhotosForDetailedDto>();
            CreateMap<Photo, PhotoForCreationDto>().ReverseMap();
            CreateMap<PhotoForReturnDto, Photo>().ReverseMap();
            
            
            CreateMap<MessageCreationDto, Message>().ReverseMap();
            CreateMap<Message, MessageDto>()
                .ForMember(m => m.SenderPhotoUrl, opt => opt.MapFrom(u => u.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(m => m.RecipientPhotoUrl, opt => opt.MapFrom(u => u.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));


        }
    }
}
