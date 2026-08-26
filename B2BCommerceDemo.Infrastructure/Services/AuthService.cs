using B2BCommerceDemo.Core.DTOs.Logins;
using B2BCommerceDemo.Core.Events.Companies;
using B2BCommerceDemo.Core.Events.Users;
using B2BCommerceDemo.Core.Interfaces.Events;
using B2BCommerceDemo.Core.Interfaces.Services;
using B2BCommerceDemo.Core.Interfaces.Services.Validate;
using B2BCommerceDemo.Core.Models;
using B2BCommerceDemo.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;


public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly IValidateUniqueness _validateUniqueness;
    private readonly IJwtService _jwtService;

    public AuthService(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        IEventDispatcher eventDispatcher,
        IValidateUniqueness validateUniqueness,
        IJwtService jwtService)
    {
        _context = context;
        _userManager = userManager;
        _eventDispatcher = eventDispatcher;
        _validateUniqueness = validateUniqueness;
        _jwtService = jwtService;
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
    {
        var email = dto.Email.Trim();

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (user.CompanyId.HasValue)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == user.CompanyId);

            if (company == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            if (company.Status != CompanyStatus.Active)
            {
                throw new UnauthorizedAccessException("Company awaiting approval.");
            }
        }

        if (!user.EmailConfirmed)
        {
            throw new UnauthorizedAccessException("Email not confirmed.");
        }

        var validPassword = await _userManager.CheckPasswordAsync(user, dto.Password);

        if (!validPassword)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }       

        var token = await _jwtService.GenerateToken(user, user.CompanyId);

        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);

        await _userManager.UpdateAsync(user);

        return new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            CompanyId = user.CompanyId
        };
    }

    public async Task RegisterAsync(RegisterDto dto)
    {
        var email = dto.Email.Trim();

        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        await _validateUniqueness.ValidateUniqueCompanyNameAsync(dto.CompanyName);

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var company = new Company
            {
                Name = dto.CompanyName.Trim(),
                Status = CompanyStatus.Pending
            };

            _context.Companies.Add(company);

            await _context.SaveChangesAsync();

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                CompanyId = company.Id
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(", ",
                        result.Errors.Select(e => e.Description)));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "User");

            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(", ",
                        roleResult.Errors.Select(e => e.Description)));
            }

            await transaction.CommitAsync();

            await _eventDispatcher.PublishAsync(
                new CompanyRegisteredEvent
                {
                    CompanyId = company.Id,
                    CompanyName = company.Name!,
                    UserEmail = user.Email!
                });

            await _eventDispatcher.PublishAsync(
                new UserRegisteredEvent
                {
                    UserId = user.Id,
                    Email = user.Email!
                });
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        if (user.EmailConfirmed)
        {
            return;
        }

        var decodedToken = Encoding.UTF8.GetString(
        WebEncoders.Base64UrlDecode(token));

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(", ",
                    result.Errors.Select(e => e.Description)));
        }
    }

    public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        var email = dto.Email.Trim();

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return;
        }

        if (!user.EmailConfirmed)
        {
            return;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        await _eventDispatcher.PublishAsync(
            new PasswordResetRequestedEvent
            {
                UserId = user.Id,
                Email = user.Email!,
                Token = token
            });
    }

    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }    

        var decodedToken = Encoding.UTF8.GetString(
            WebEncoders.Base64UrlDecode(dto.Token));

        var result = await _userManager.ResetPasswordAsync(user, decodedToken, dto.NewPassword);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(", ",
                    result.Errors.Select(e => e.Description)));
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await _userManager.UpdateAsync(user);
    }

    public async Task ChangePasswordAsync(string userId, ChangePasswordDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(", ",
                    result.Errors.Select(e => e.Description)));
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await _userManager.UpdateAsync(user);
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.RefreshToken == dto.RefreshToken);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Refresh token has expired.");
        }

        var newAccessToken = await _jwtService.GenerateToken(user, user.CompanyId);

        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);

        await _userManager.UpdateAsync(user);

        return new LoginResponseDto
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken,
            CompanyId = user.CompanyId
        };
    }
}
