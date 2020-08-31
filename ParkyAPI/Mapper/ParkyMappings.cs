using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repo;

namespace ParkyAPI.Mapper
{
    public class ParkyMappings: Profile
    {
        public ParkyMappings()
        {
            CreateMap<NationalPark, NationalParkDto>().ReverseMap();
            CreateMap<Trail, TrailDto>().ReverseMap();
            CreateMap<Trail, TrailUpdateDto>().ReverseMap();
            CreateMap<Trail, TrailCreateDto>().ReverseMap();
        }
    }
}
