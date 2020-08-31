using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repo.IRepo;

namespace ParkyAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/nationalparks")]
    [ApiController]
    //[ApiExplorerSettings(GroupName = "ParkyOpenApiSpecNP")]
    [ProducesResponseType(400)]
    public class NationalParksController : ControllerBase
    {
        private readonly INationalParkRepository _repo;
        private readonly IMapper _mapper;

        public NationalParksController(INationalParkRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        /// <summary>
        /// Get list of national parks
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [ProducesResponseType(200,Type = typeof(List<NationalParkDto>))]
        public IActionResult GetNationalParks()
        {
            var nationalParks = _repo.GetNationalParks();

            var nationalParksDtos = new List<NationalParkDto>();

            foreach (var obj in nationalParks)
            {
                nationalParksDtos.Add(_mapper.Map<NationalParkDto>(obj));
            }

            return Ok(nationalParksDtos);
        }

        /// <summary>
        /// Get individual national park
        /// </summary>
        /// <param name="id">Id of the national park</param>
        /// <returns></returns>

        [HttpGet("{id:int}", Name = "GetNationalPark")]
        [ProducesResponseType(200,Type = typeof(NationalParkDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        [Authorize]

        public IActionResult GetNationalPark(int id)
        {
            var obj = _repo.GetNationalPark(id);
            if (obj == null)
            {
                return NotFound();
            }

            var objDto = _mapper.Map<NationalParkDto>(obj);
            return Ok(objDto);
        }

        /// <summary>
        /// Create a national park
        /// </summary>
        /// <param name="nationalParkDto">National Park</param>
        /// <returns></returns>

        [HttpPost]
        [ProducesResponseType(201,Type = typeof(NationalPark))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesDefaultResponseType]
        public IActionResult CreateNationalPark([FromBody]NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_repo.NationalParkExists(nationalParkDto.Name))
            {
                ModelState.AddModelError("","National Park Already Exists!");
                return StatusCode(404, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);
            if (!_repo.CreateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went wrong when saving the record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetNationalPark", new {Version = HttpContext.GetRequestedApiVersion().ToString() ,
                id = nationalParkObj.Id}, nationalParkObj);

        }

        /// <summary>
        /// Update already exist national park
        /// </summary>
        /// <param name="id">Id of national park which should be update</param>
        /// <param name="nationalParkDto">Updated object of national park</param>
        /// <returns></returns>

        [HttpPatch("{id:int}",Name = "UpdateNationalPark")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateNationalPark(int id, [FromBody]NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null || id != nationalParkDto.Id)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);

            if (!_repo.UpdateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Problem was occurred during updating {nationalParkDto.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();


        }

        /// <summary>
        /// Delete already exist national park
        /// </summary>
        /// <param name="id">Id of the national park</param>
        /// <returns></returns>

        [HttpDelete("{id}", Name = "DeleteNationalPark")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult DeleteNationalPark(int id)
        {
            if (!_repo.NationalParkExists(id))
            {
                return NotFound();
            }

            var nationalParkObj = _repo.GetNationalPark(id);
            if (!_repo.DeleteNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("",
                    $"Error on server was occurred during deleting a object which have given id, this object: {nationalParkObj}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

    }
}
