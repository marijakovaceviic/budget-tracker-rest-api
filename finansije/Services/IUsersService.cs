using finansije.Entities;
using finansije.Models;

namespace finansije.Services
{
    public interface IUsersService
    {
        public Task<int> RegisterAsync(UserRegistrationDto request, string role);
        public Task<TokenResponseDto?> LoginAsync(UserLoginDto request);
        public Task<List<UserProfilDto>> GetAllUsers(string role);
        public Task<int> DeleteUserAsync(Guid id);
        public Task<int> ChangePhoneNumberAsync(string phoneNumber);
        public Task<int> ChangeAddressInfoAsync(AddressInfoDto addressInfo);
        public Task<List<UserProfilDto>> SortUsersAsync(string role = "user", string sortBy = "username", bool ascending = true);
        public Task<List<UserProfilDto>> SortUserByTransactionCountAsync();
        public Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
    }
}
