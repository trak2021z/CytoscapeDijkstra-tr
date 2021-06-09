﻿using CytoscapeDijkstra2.Models.DBModels;
using CytoscapeDijkstra2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CytoscapeDijkstra2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(string login, string password)
        {
            var user = userService.Authenticate(login, password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("key")), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            user.DateLastLogin = DateTime.Now;

            return Ok(new
            {
                Id = user.Id,
                Login = user.Login,
                Token = tokenString
            });
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = userService.GetAll();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = userService.GetById(id);
            return Ok(user);
        }

        [HttpPost]
        public IActionResult Register(string login, string password)
        {
            try
            {
                userService.Create(login, password);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, string newLogin, string newPassword)
        {
            var user = new User();
            user.Id = id;
            user.Login = newLogin;

            try
            {
                userService.Update(user, newPassword);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            userService.Delete(id);
            return Ok();
        }



        /*
        [HttpGet]
        public async Task<ActionResult<List<User>>> Get()
        {
          return await _context.Users.ToListAsync();
        }
        */

        /*
        [HttpPost("{login},{password}")]
        public IActionResult RegisterUser(string login, string password)
        {

            _context.Users.Add(new Models.DBModels.User())
        }
        */

        /*
        public IActionResult Index()
        {
            return View();
        }
        */
    }
}
