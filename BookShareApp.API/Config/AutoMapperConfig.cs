using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BookShareApp.API.Dto;
using BookShareApp.API.Framework;
using BookShareApp.API.Models;

namespace BookShareApp.API.Config
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            ApplyUserMapping<UserForListDto>();
            ApplyUserMapping<UserForDetailDto>();
            CreateMap<Photo, PhotoForDetailDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto, Photo>();
        }

        private void ApplyUserMapping<T>() where T : IUserDto
        {
            var mappUserDto = CreateMap<User, T>();
            var mapPhotoUrl = ApplyPhotoUrlMapping(mappUserDto);
            var mapAge = ApplyAgeMapping(mapPhotoUrl);
        }

        private IMappingExpression<User, G> ApplyPhotoUrlMapping<G>(IMappingExpression<User, G> mapping)
           where G : IUserDto
        {
            return mapping.ForMember(dest => dest.PhotoUrl,
               opt =>
               {
                   opt.MapFrom(src => GetPhotoUrl(src.Photos));
               }
           );
        }

        private IMappingExpression<User, G> ApplyAgeMapping<G>(IMappingExpression<User, G> mapping)
           where G : IUserDto
        {
            return mapping.ForMember(dest => dest.Age,
               opt =>
               {
                   opt.ResolveUsing(d => d.DateOfBirth.ToAge());
               }
           );
        }

        private string GetPhotoUrl(IEnumerable<Photo> photos)
        {
            if (photos == null)
                return null;

            return photos.FirstOrDefault(p => p.IsMain).Url;
        }
    }
}