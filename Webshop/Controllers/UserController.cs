﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Webshop.Data;
using Webshop.Data.Models;
using ApplicationDbContext = Webshop.Data.ApplicationDbContext;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Webshop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationSettings _appSettings;
        //private readonly UserManager<User> _userManager;

        public UserController(ApplicationDbContext context, IMapper mapper, Microsoft.AspNetCore.Identity.UserManager<User> userManager
            ,IOptions<ApplicationSettings> appSettings, SignInManager<User> signInManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _signInManager = signInManager;
        }
        // GET: api/<UserController>
        [HttpGet]
        public async Task<IEnumerable<UserDto>> Get()
        {
            var res = await _context.Users.ToListAsync();
            var mappelt = _mapper.Map<List<UserDto>>(res);
            return mappelt;
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> Get(string id)
        {
            var res = await _context.Users.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (res == null) return NotFound();
            var mappelt = _mapper.Map<UserDto>(res);
            return mappelt;
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UserDto newUser)
        {
            User user = _mapper.Map<User>(newUser);

            //Assigning the role to the user
            user.Role = "Customer";

            Cart newCart = new Cart()
            {
                UserId = user.Id,
                User = user
            };
            //System.Diagnostics.Debug.WriteLine("newcart: " + newCart);
            _context.Carts.Add(newCart);          

            user.Cart = newCart;



            //System.Diagnostics.Debug.WriteLine("user.Cart: " + user.Cart.CartId);

            try
            {
                var result = await _userManager.CreateAsync(user, newUser.Password);
                await _userManager.AddToRoleAsync(user, user.Role);              
            }
            catch (Exception e) {
                Debug.WriteLine(e);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpPost]
        [Route("registerAdmin")]
        public async Task<ActionResult> Post_Admin([FromBody] UserDto newUser)
        {
            User user = _mapper.Map<User>(newUser);

            //Assigning the role to the user
            user.Role = "Admin";

            Cart newCart = new Cart()
            {
                UserId = user.Id,
                User = user
            };
            //System.Diagnostics.Debug.WriteLine("newcart: " + newCart);
            _context.Carts.Add(newCart);

            user.Cart = newCart;
            //System.Diagnostics.Debug.WriteLine("user.Cart: " + user.Cart);

            try
            {
                var result = await _userManager.CreateAsync(user, newUser.Password);
                await _userManager.AddToRoleAsync(user, user.Role);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }




        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(string id, [FromBody] UserDto newUser)
        {

            var user = _mapper.Map<User>(newUser);
            var userWaitingForUpdate = _context.Users.SingleOrDefault(p => p.Id == id);

            if (userWaitingForUpdate == null)
                return NotFound();

            // modositasok elvegzese    
            if (user.UserName != null) {
                userWaitingForUpdate.UserName = user.UserName;
                userWaitingForUpdate.NormalizedUserName = user.UserName.ToUpper();
            }

            if (user.Email != null) {
                userWaitingForUpdate.Email = user.Email;
                userWaitingForUpdate.NormalizedEmail = user.Email.ToUpper();
            }
            

            if (newUser.Password != null) {
                await _userManager.RemovePasswordAsync(userWaitingForUpdate);
                await _userManager.AddPasswordAsync(userWaitingForUpdate, newUser.Password); 
            }
            
            // mentes az adatbazisban
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var dbUser = _context.Users.SingleOrDefault(p => p.Id == id);

            if (dbUser == null)
                return NotFound();
            
            var cart = _context.Carts.Where(c => c.UserId == dbUser.Id).FirstOrDefault();
            _context.Carts.Remove(cart);
            
            _context.Users.Remove(dbUser);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(Webshop.Data.Models.LoginModel model) {

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var role = await _userManager.GetRolesAsync(user);
                IdentityOptions _options = new IdentityOptions();

                var tokenDescriptor = new SecurityTokenDescriptor
                {         
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID", user.Id.ToString()),
                        new Claim(_options.ClaimsIdentity.RoleClaimType,role.FirstOrDefault())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(5),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new { token });
            }
            else {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
        }
    }
}
