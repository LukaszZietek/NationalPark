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
    //[Route("api/Trail")]
    [Route("api/v{version:apiVersion}/trail")]
    [ApiController]
    [ProducesResponseType(400)]
    //[ApiExplorerSettings(GroupName = "ParkyOpenApiSpecTrails")]
    public class TrailController : ControllerBase
    {
        private readonly ITrailRepository _repo;
        private readonly IMapper _mapper;

        public TrailController(ITrailRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        /// <summary>
        /// Get list of trails
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<TrailDto>))]
        public IActionResult GetTrails()
        {
            var trails = _repo.GetTrails();

            var trailsDtos = new List<TrailDto>();

            foreach (var obj in trails)
            {
                trailsDtos.Add(_mapper.Map<TrailDto>(obj));
            }

            return Ok(trailsDtos);
        }

        /// <summary>
        /// Get individual trail
        /// </summary>
        /// <param name="id">Id of the trail</param>
        /// <returns></returns>

        [HttpGet("{id:int}", Name = "GetTrail")]
        [ProducesResponseType(200, Type = typeof(TrailDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        [Authorize(Roles = "Admin")]
        public IActionResult GetTrail(int id)
        {
            var obj = _repo.GetTrail(id);
            if (obj == null)
            {
                return NotFound();
            }

            var objDto = _mapper.Map<TrailDto>(obj);
            return Ok(objDto);
        }

        [HttpGet("[action]/{nationalParkId:int}", Name = "GetTrailInNationalPark")]
        [ProducesResponseType(200, Type = typeof(TrailDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]

        public IActionResult GetTrailInNationalPark(int nationalParkId)
        {
            var objList = _repo.GetTrailsInNationalPark(nationalParkId);
            if (objList == null)
            {
                return NotFound();
            }

            var objDtoList = new List<TrailDto>();
            foreach (var obj in objList)
            {
                objDtoList.Add(_mapper.Map<TrailDto>(obj));
            }
            return Ok(objDtoList);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Trail))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesDefaultResponseType]
        public IActionResult CreateTrail([FromBody] TrailCreateDto trailDto)
        {
            if (trailDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_repo.TrailExists(trailDto.Name))
            {
                ModelState.AddModelError("", "Trail Already Exists!");
                return StatusCode(404, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var trailObj = _mapper.Map<Trail>(trailDto);
            if (!_repo.CreateTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when saving the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetTrail", new { Version = HttpContext.GetRequestedApiVersion().ToString(),
                id = trailObj.Id }, trailObj);

        }

        [HttpPatch("{id:int}", Name = "UpdateTrail")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateTrail(int id, [FromBody] TrailUpdateDto trailDto)
        {
            if (trailDto == null || id != trailDto.Id)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var trailObj = _mapper.Map<Trail>(trailDto);

            if (!_repo.UpdateTrail(trailObj))
            {
                ModelState.AddModelError("", $"Problem was occurred during updating {trailDto.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();


        }

        [HttpDelete("{id}", Name = "DeleteTrail")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult DeleteTrail(int id)
        {
            if (!_repo.TrailExists(id))
            {
                return NotFound();
            }

            var trailObj = _repo.GetTrail(id);
            if (!_repo.DeleteTrail(trailObj))
            {
                ModelState.AddModelError("",
                    $"Error on server was occurred during deleting a object which have given id, this object: {trailObj}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
