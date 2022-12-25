using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using General.Application.Common.Extensions;
using General.Application.Interfaces;
using General.Application.Common.Results;
using General.Domain.Common;
using General.Domain.Entities;
using General.Domain.Models;
using General.Infrastructure.Extensions;
using General.Infrastructure.Persistence;
using Enums = Common.Shared.Enums;
using Common.Shared.Models;

namespace General.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public IdentityService(
            ApplicationDbContext context,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper
            )
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<string> GetUserIdAsync(string userName)
        {
            var user = await _userManager.Users.FirstAsync(u => string.Equals(userName, u.UserName));
            return user.Id;
        }

        public async Task<(Result, string)> CreateUserAsync(
            string userName,
            string email, 
            string password,
            string phoneNumber,
            bool mustChangePassword = false
        ) {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                PhoneNumber = phoneNumber,
                PhoneNumberConfirmed = true,
                EmailConfirmed = false,
                LockoutEnabled = false,
                LockoutEnd = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, password);
            return (result.ToApplicationResult(), user.Id);
        }
        public async Task<Result> CreateUserAsync(ApplicationUser user)
        {
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded == false)
            {
                return result.ToApplicationResult();
            }
            return Result.Success();
        }
        public async Task<Result> ConfirmEmailAsync(ApplicationUser user,string token)
        {
            var identityResult = await _userManager.ConfirmEmailAsync(user, token);
            if (identityResult.Succeeded == false)
            {
                return identityResult.ToApplicationResult();
            }
            return Result.Success();
        }
        public async Task<Result> AddPasswordAsync(ApplicationUser user, string password)
        {
            var identityResult = await _userManager.AddPasswordAsync(user, password);
            if (identityResult.Succeeded == false)
            {
                return identityResult.ToApplicationResult();
            }
            return Result.Success();
        }
        public async Task<Result> DeleteUserAsync(ApplicationUser user)
        {
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded == false)
            {
                return result.ToApplicationResult();
            }
            return Result.Success();
        }

        public async Task<Microsoft.AspNetCore.Identity.IdentityResult> AssignUserToRole(ApplicationUser user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<SignInResult> SignInAsync(string userName, string password, bool rememberMe)
        {
            return await _signInManager.PasswordSignInAsync(userName, password, rememberMe, lockoutOnFailure: false);
        }

        public async Task<IList<string>> GetRolesUserAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var roles = await _userManager.GetRolesAsync(user);
            return roles;
        }

        public async Task<Result> ChangePasswordAsync(string userName, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Result.Failure("Can not be found the user");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!changePasswordResult.Succeeded)
            {
                return changePasswordResult.ToApplicationResult();
            }

            //user.RequireChangePassword = false;
            user.LockoutEnabled = false;
            user.LockoutEnd = DateTime.Now.AddMinutes(-1);

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);

            return Result.Success();
        }

        public async Task<(Result, string)> GenerateNewPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return (Result.Failure("Can not be found the user"), string.Empty);
            }

            //PRODUCTION EXPECTED
            var temporaryPassword = CreatePassword(10).ToBase64Encode();

            ////TEMP SOLUTION
            //var temporaryPassword = "Success@@1234".ToBase64Encode();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var setNewPasswordResult = await _userManager.ResetPasswordAsync(user, token, temporaryPassword);

            if (!setNewPasswordResult.Succeeded)
            {
                return (setNewPasswordResult.ToApplicationResult(), string.Empty);
            }

            user.LockoutEnd = DateTime.Now;
            user.LockoutEnabled = true;

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);

            return (Result.Success(), temporaryPassword);
        }
        public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            string verifyToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return verifyToken;
        }
        public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            string verifyToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            return verifyToken;
        }


        public async Task<(Result, string)> CreateUserWithTemporaryPasswordAsync(string userName, string email, string phoneNumber, Guid? roleId, CreateUserModel userItem)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                return (Result.Failure("User has been existed in system"), string.Empty);
            }

            //PRODUCTION EXPECTED
            var temporaryPassword = CreatePassword(10).ToBase64Encode();

            ////TEMP SOLUTION
            //var temporaryPassword = "Success@@1234".ToBase64Encode();

            (Result result, string userId) = await CreateUserAsync(userName, email, temporaryPassword, phoneNumber, false);

            if (result.Succeeded)
            {
                user = await _userManager.FindByIdAsync(userId);

                var role = await _roleManager.FindByIdAsync(roleId.ToString());

                if (role != null)
                {
                    await AssignUserToRole(user, role.Name);
                }
                
                var humanResource = _mapper.Map<ProfileInformationEntity>(userItem);
                humanResource.UserId = Guid.Parse(userId);
                _context.ProfileInformation.Add(humanResource);

                await _context.SaveChangesAsync();
            }

            return (result, temporaryPassword);
        }

        public async Task<Result> UpdateProfileAsync(UpdateProfileModel updateProfileRequest,Guid userId)
        {
            var humanResource = _context.ProfileInformation.Where(x=>x.UserId == userId).FirstOrDefault();
            if (humanResource == null)
            {
                return (Result.Failure("User has been NOT existed in system"));
            }

            var user = _context.Users.Where(x => x.Id == userId.ToString()).FirstOrDefault();

            humanResource.FirstName = updateProfileRequest.FirstName;
            humanResource.LastName = updateProfileRequest.LastName;
            humanResource.Address = updateProfileRequest.Address;
            humanResource.Avatar = updateProfileRequest.AvatarFilePath;
            //humanResource.BirthDay = updateProfileRequest.BirthDay;
            humanResource.PhoneNumber1 = updateProfileRequest.PhoneNumber1;
            humanResource.PhoneNumber2 = updateProfileRequest.PhoneNumber2;
            humanResource.PhoneNumber3 = updateProfileRequest.PhoneNumber3;
            humanResource.TitleVi = updateProfileRequest.TitleVi;
            humanResource.TitleEn = updateProfileRequest.TitleEn;
            humanResource.TitleDescriptionVi = updateProfileRequest.TitleDescriptionVi;
            humanResource.TitleDescriptionEn = updateProfileRequest.TitleDescriptionEn;
            //humanResource.Descriptions = updateProfileRequest.Descriptions;
            //humanResource.Agency = updateProfileRequest.Agency;
            //humanResource.License = updateProfileRequest.License;

            await _context.SaveChangesAsync();
            
            return (Result.Success());
        }
        public async Task<Result> UpdateUserAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded == false)
            {
                return result.ToApplicationResult();
            }
            return Result.Success();
        }
        public async Task<Result> ResetPasswordAsync(ApplicationUser user, string token, string password)
        {
            var resetPassResult = await _userManager.ResetPasswordAsync(user, token, password);
            if (!resetPassResult.Succeeded)
            {
                return resetPassResult.ToApplicationResult();
            }

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);

            return Result.Success();
        }

        public async Task<Result> SavingUserTokenAsync(ApplicationUser user, JwtAuthResult jwtAuthResult)
        {
            await _userManager.SetAuthenticationTokenAsync(user, "919daffb-3249-4874-a86e-5f38c81c0a2e", "RefreshToken", jwtAuthResult.RefreshToken.TokenString.ToString());

            return (Result.Success());
        }
        public bool GetUserTokenAsync(ApplicationUser user, string refreshToken)
        {
            var result = _context.UserTokens.Where(x => x.UserId == user.Id && x.Value == refreshToken).FirstOrDefault();
            if (result != null)
            {
                return true;
            }
            return false;
        }

        public async Task<ApplicationUser> GetUserByIdentifierAsync(string identifier)
        {
            var user = await _userManager.FindByIdAsync(identifier);

            return user;
        }

        public async Task<ApplicationUser> GetUserByUserNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user;
        }
        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user;
        }
        public async Task<ApplicationUser> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            var user = await _context.Users
                .Where(x => x.PhoneNumber.ToUpper().Equals(phoneNumber.ToUpper()))
                .FirstOrDefaultAsync();
            return user;
        }


        public async Task<List<ApplicationUser>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<List<ApplicationUser>> GetUsersInRoleAsync(string role)
        {
            var listData = await _userManager.GetUsersInRoleAsync(role);
            return listData.ToList();
        }

        public async Task<Result> LockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            user.LockoutEnabled = true;
            user.LockoutEnd = DateTime.UtcNow.AddDays(36500);

            var result = await _userManager.UpdateAsync(user);
            return result.ToApplicationResult();
        }

        public async Task<Result> UnlockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            user.LockoutEnabled = false;
            user.LockoutEnd = DateTime.Now;
            var result = await _userManager.UpdateAsync(user);
            return result.ToApplicationResult();
        }

        public async Task ResetAccessFailedCountAsync(string userId)
        {
            ApplicationUser user = new ApplicationUser()
            {
                Id = userId
            };
            await _userManager.ResetAccessFailedCountAsync(user);
        }

        internal string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public async Task<string> GetRoleUserAsync(string userName)
        {
            var roles = await GetRolesUserAsync(userName);
            return roles.FirstOrDefault();
        }
    }
}
