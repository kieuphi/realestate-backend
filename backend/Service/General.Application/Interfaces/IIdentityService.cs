using System.Threading.Tasks;
using General.Application.Common.Results;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using General.Domain.Common;
using System;
using General.Domain.Models;
using Common.Shared.Models;

namespace General.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<string> GetUserIdAsync(string userName);
        Task<(Result, string)> CreateUserAsync(string userName, string email, string password, string PhoneNumber, bool mustChangePassword = false);
        Task<Result> CreateUserAsync(ApplicationUser user);
        Task<Result> UpdateUserAsync(ApplicationUser user);
        Task<Result> ResetPasswordAsync(ApplicationUser user, string token, string password);
        Task<Result> ConfirmEmailAsync(ApplicationUser user, string token);
        Task<Result> AddPasswordAsync(ApplicationUser user, string password);
        Task<Result> DeleteUserAsync(ApplicationUser user);
        Task<SignInResult> SignInAsync(string userName, string password, bool rememberMe);
        Task<IList<string>> GetRolesUserAsync(string userName);
        Task<string> GetRoleUserAsync(string userName);
        Task<Result> ChangePasswordAsync(string userName, string oldPassword, string newPassword);
        Task<(Result, string)> GenerateNewPasswordAsync(string email);
        Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<ApplicationUser> GetUserByIdentifierAsync(string identifier);
        Task<ApplicationUser> GetUserByUserNameAsync(string userName);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<ApplicationUser> GetUserByPhoneNumberAsync(string phoneNumber);
        Task<List<ApplicationUser>> GetUsersAsync();
        Task<List<ApplicationUser>> GetUsersInRoleAsync(string role);
        Task<Microsoft.AspNetCore.Identity.IdentityResult> AssignUserToRole(ApplicationUser user, string role);
        Task ResetAccessFailedCountAsync(string userId);
        Task<Result> LockUserAsync(string userId);
        Task<Result> UnlockUserAsync(string userId);
        Task<(Result, string)> CreateUserWithTemporaryPasswordAsync(string userName, string email, string phoneNumber, Guid? roleId, CreateUserModel userItem);
        Task<Result> UpdateProfileAsync(UpdateProfileModel updateProfileRequest, Guid userId);
        Task<Result> SavingUserTokenAsync(ApplicationUser user, JwtAuthResult jwtAuthResult);
        bool GetUserTokenAsync(ApplicationUser user, string refreshToken);
    }
}
