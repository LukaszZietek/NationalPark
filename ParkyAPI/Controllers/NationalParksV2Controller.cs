using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repo.IRepo;

namespace ParkyAPI.Controllers
{

    [Route("api/v{version:apiVersion}/nationalparks")]
    [ApiVersion("2.0")]
    //[Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(400)]
    public class NationalParksV2Controller : ControllerBase
    {
        private readonly INationalParkRepository _repo;
        private readonly IMapper _mapper;

        public NationalParksV2Controller(INationalParkRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        /// <summary>
        /// Get list of national parks
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<NationalParkDto>))]
        public IActionResult GetNationalParks()
        {
            var nationalPark = _repo.GetNationalParks().FirstOrDefault();

            return Ok(_mapper.Map<NationalParkDto>(nationalPark));
        }

    }
}
