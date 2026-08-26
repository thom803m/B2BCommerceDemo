using B2BCommerceDemo.Core.DTOs.Logins;

namespace B2BCommerceDemo.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto dto);
        Task RegisterAsync(RegisterDto dto);
        Task ConfirmEmailAsync(string userId, string token);
        Task ForgotPasswordAsync(ForgotPasswordDto dto);
        Task ResetPasswordAsync(ResetPasswordDto dto);
        Task ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto);
    }
}
