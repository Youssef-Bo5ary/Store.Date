using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Store.Date.Entities.IdentityEntity;
using Store.Service.Services.TokenServices;
using Store.Service.Services.UserServices.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Service.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly SignInManager<AppUser> _signInManger;
        private readonly Microsoft.AspNetCore.Identity.UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;

        public UserService(SignInManager<AppUser> signInManger,
            Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager,
            ITokenService tokenService)
        {
            _signInManger = signInManger;
            _userManager = userManager;
            _tokenService = tokenService;
        }
        public async Task<UserDto> Login(LoginDto input)
        {
            var user = await _userManager.FindByEmailAsync(input.Email);
            if (user is null) return null;

            var result = await _signInManger.CheckPasswordSignInAsync(user,input.Password,false);

            if (!result.Succeeded)
                throw new Exception("Login Failed!");

            return new UserDto
            {
                Id = Guid.Parse(user.Id),
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = _tokenService.GenerateToken(user)
            };
        }

        public async Task<UserDto> Register(RegisterDto input)
        {
            var user = await _userManager.FindByEmailAsync(input.Email);

            if (user is not null) return null;

            var appUser = new AppUser
            {
                DisplayName = input.DisplayName,
                Email = input.Email,
                UserName = input.DisplayName
            };

            var result = await _userManager.CreateAsync(appUser,input.Password);

            if (!result.Succeeded)
                throw new Exception(result.Errors.Select(x => x.Description).FirstOrDefault());

            return new UserDto
            {
                Id = Guid.Parse(appUser.Id),
                DisplayName = appUser.DisplayName,
                Email = appUser.Email,
                Token = _tokenService.GenerateToken(appUser)
            };

        }

        

    }
}
