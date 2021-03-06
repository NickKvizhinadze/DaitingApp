﻿using AutoMapper;
using DatingApp.Api.Dtos;
using DatingApp.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Api.Dtos
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repo, IConfiguration configuration, IMapper mapper)
        {
            _repo = repo;
            _configuration = configuration;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserRegistrationDto model)
        {
            if (!string.IsNullOrEmpty(model.Username))
                model.Username = model.Username.ToLower();

            if (await _repo.UserExists(model.Username))
                ModelState.AddModelError("", "Username is already taken");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var createdUser = await _repo.Register(_mapper.Map<User>(model), model.Password);

            return CreatedAtRoute("GetUser", new { controller = "Users", id = createdUser.Id }, _mapper.Map<UserForDetaildDto>(createdUser));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserLoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _repo.Login(model.Username.ToLower(), model.Password);
            if (existingUser == null)
                return Unauthorized();

            //TODO: Generate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Token").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()),
                    new Claim(ClaimTypes.Name, existingUser.Username)
                }),
                Expires = DateTime.Now.AddYears(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { token = tokenString, user = _mapper.Map<UserForListDto>(existingUser) });
        }
    }
}
