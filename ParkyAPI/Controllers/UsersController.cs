using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Repo.IRepo;

namespace ParkyAPI.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepo _repo;

        public UsersController(IUserRepo repo)
        {
            _repo = repo;
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
        public IActionResult Authenticate([FromBody] AuthorizationModel model)
        {
            var user = _repo.Authenticate(model.Username, model.Password);
            if (user == null)
            {
                return BadRequest(new {message = "Username or password is incorrect"});
            }

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] AuthorizationModel model)
        {
            bool isUserNameUnique = _repo.IsUniqueUser(model.Username);
            if (!isUserNameUnique)
            {
                return BadRequest(new {messagge = "Username already exists!"});
            }

            var user = _repo.Register(model.Username, model.Password);

            if (user == null)
            {
                return BadRequest(new {message = "Error while registering! "});
            }

            return Ok();

        }
    }
}
