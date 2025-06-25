using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using finansije.Data;
using finansije.Entities;
using finansije.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace finansije.Services
{
    public class UsersService(UsersContext context, IConfiguration configuration, ICurrentUserService currentUserService) : IUsersService
    {
        public async Task<int> RegisterAsync(UserRegistrationDto request, string role)
        {
            if (request.Role != role)
            {
                return -1;
            }
            if (await context.Users.AnyAsync(u => u.UserName == request.UserName))
            {
                return -2;
            }
            if (await context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return -3;
            }
            if (role == "user" && request.UserName.ToLower().Contains("admin"))
            {
                return -4;
            }
            var user = new User();
            user.UserName = request.UserName;
            user.Email = request.Email;
            user.Role = request.Role.ToLower();
            var hashedPassword = new PasswordHasher<User>()
                .HashPassword(user, request.Password);

            user.PasswordHash = hashedPassword;

            user.PersonalInfo = new UserPersonalInfo();
            user.PersonalInfo.FirstName = request.PersonalInfo.FirstName;
            user.PersonalInfo.LastName = request.PersonalInfo.LastName;
            user.PersonalInfo.DateOfBirth = request.PersonalInfo.DateOfBirth;
            user.PersonalInfo.PhoneNumber = request.PersonalInfo.PhoneNumber;

            var existingAddress = await context.AddressInfos
                .FirstOrDefaultAsync(a =>
                    a.City == request.AddressInfo.City &&
                    a.Country == request.AddressInfo.Country &&
                    a.Street == request.AddressInfo.Street &&
                    a.HouseNumber == request.AddressInfo.HouseNumber &&
                    a.PostalCode == request.AddressInfo.PostalCode
                );
            if (existingAddress != null)
            {
                user.AddressInfo = existingAddress;
            }
            else
            {
                user.AddressInfo = new AddressInfo();
                user.AddressInfo.City = request.AddressInfo.City;
                user.AddressInfo.PostalCode = request.AddressInfo.PostalCode;
                user.AddressInfo.HouseNumber = request.AddressInfo.HouseNumber;
                user.AddressInfo.Country = request.AddressInfo.Country;
                user.AddressInfo.Street = request.AddressInfo.Street;
            }

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return 0;
        }

        public async Task<TokenResponseDto?> LoginAsync(UserLoginDto request)
        {
            var user = await context.Users
                .Include(u => u.PersonalInfo)
                .FirstOrDefaultAsync(u => u.UserName == request.UserName);
            
            if (user == null)
            {
                return null;
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(
                user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                return null;
            }

            var response = new TokenResponseDto
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };
            return response;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.DateOfBirth, user.PersonalInfo.DateOfBirth.ToString()),
                new Claim(ClaimTypes.MobilePhone, user.PersonalInfo.PhoneNumber.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );
            
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor); 
        }

        public async Task<List<UserProfilDto>> GetAllUsers(string role)
        {
            var users = await context.Users
                .Include(u => u.PersonalInfo)
                .Include(u => u.AddressInfo)
                .Where(u => u.Role == role)
                .ToListAsync();

            var result = users.Select(u => new UserProfilDto
            {
                UserName = u.UserName,
                Email = u.Email,
                PersonalInfo = new PersonalInfoDto()
                {
                    FirstName = u.PersonalInfo.FirstName,
                    LastName = u.PersonalInfo.LastName,
                    DateOfBirth = u.PersonalInfo.DateOfBirth,
                    PhoneNumber = u.PersonalInfo.PhoneNumber
                },
                AddressInfo = new AddressInfoDto()
                {
                    City = u.AddressInfo.City,
                    Country = u.AddressInfo.Country,
                    HouseNumber = u.AddressInfo.HouseNumber,
                    PostalCode = u.AddressInfo.PostalCode,
                    Street = u.AddressInfo.Street
                }
            }).ToList();

            return result;
        }

        public async Task<int> DeleteUserAsync(Guid id)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return -1;
            }
            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return 0;
        }

        public async Task<int> ChangePhoneNumberAsync(string phoneNumber)
        {
            var userId = currentUserService.UserId;
            var user = await context.Users
                .Include(u => u.PersonalInfo)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return -1;
            }
            user.PersonalInfo.PhoneNumber = phoneNumber;
            await context.SaveChangesAsync();
            return 0;
        }

        public async Task<int> ChangeAddressInfoAsync(AddressInfoDto addressInfo)
        {
            var userId = currentUserService.UserId;
            var user = await context.Users
                .Include(u => u.AddressInfo)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return -1;
            }
            var usersWithSameAddress = await context.Users
                .CountAsync(u => u.AddressInfo.City == addressInfo.City &&
                u.AddressInfo.Street == addressInfo.Street &&
                u.AddressInfo.Country == addressInfo.Country &&
                u.AddressInfo.PostalCode == addressInfo.PostalCode &&
                u.AddressInfo.HouseNumber == addressInfo.HouseNumber);

            if (usersWithSameAddress > 1)
            {
                var newAddress = new AddressInfo
                {
                    City = addressInfo.City,
                    Street = addressInfo.Street,
                    HouseNumber = addressInfo.HouseNumber,
                    PostalCode = addressInfo.PostalCode,
                    Country = addressInfo.Country
                };
                user.AddressInfo = newAddress;
            }
            else
            {
                user.AddressInfo.City = addressInfo.City;
                user.AddressInfo.Street = addressInfo.Street;
                user.AddressInfo.HouseNumber = addressInfo.HouseNumber;
                user.AddressInfo.PostalCode = addressInfo.PostalCode;
                user.AddressInfo.Country = addressInfo.Country;
            }
            await context.SaveChangesAsync();
            return 0;
        }

        public async Task<List<UserProfilDto>> SortUsersAsync(string role = "user", string sortBy = "username", bool ascending = true)
        {
            var users = context.Users
                .Include(u => u.PersonalInfo)
                .Include(u => u.AddressInfo)
                .Where(u => u.Role == role);

            users = (sortBy.ToLower(), ascending) switch
            {
                ("username", true) => users.OrderBy(u => u.UserName),
                ("username", false) => users.OrderByDescending(u => u.UserName),
                ("email", true) => users.OrderBy(u => u.Email),
                ("email", false) => users.OrderByDescending(u => u.Email)
            };

            var sortedUsers = await users.ToListAsync();

            var result = sortedUsers.Select(u => new UserProfilDto
            {
                UserName = u.UserName,
                Email = u.Email,
                PersonalInfo = new PersonalInfoDto
                {
                    FirstName = u.PersonalInfo.FirstName,
                    LastName = u.PersonalInfo.LastName,
                    DateOfBirth = u.PersonalInfo.DateOfBirth,
                    PhoneNumber = u.PersonalInfo.PhoneNumber
                },
                AddressInfo = new AddressInfoDto
                {
                    City = u.AddressInfo.City,
                    Country = u.AddressInfo.Country,
                    HouseNumber = u.AddressInfo.HouseNumber,
                    PostalCode = u.AddressInfo.PostalCode,
                    Street = u.AddressInfo.Street
                }
            }).ToList();

            return result;
        }

        public async Task<List<UserProfilDto>> SortUserByTransactionCountAsync()
        {
            var users = await context.Users
               // .Include(u => u.Transactions)
                .Include(u => u.PersonalInfo)
                .Include(u => u.AddressInfo)
                .Where(u => u.Role == "user")
                .OrderByDescending(u => u.Transactions.Count)
                .ToListAsync();

            var result = users.Select(u => new UserProfilDto
            {
                UserName = u.UserName,
                Email = u.Email,
                PersonalInfo = new PersonalInfoDto
                {
                    FirstName = u.PersonalInfo.FirstName,
                    LastName = u.PersonalInfo.LastName,
                    DateOfBirth = u.PersonalInfo.DateOfBirth,
                    PhoneNumber = u.PersonalInfo.PhoneNumber
                },
                AddressInfo = new AddressInfoDto
                {
                    City = u.AddressInfo.City,
                    Country = u.AddressInfo.Country,
                    HouseNumber = u.AddressInfo.HouseNumber,
                    PostalCode = u.AddressInfo.PostalCode,
                    Street = u.AddressInfo.Street
                }
            }).ToList();

            return result;
        }

        private string GenerateRefreshToken()
        {
            var rendomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(rendomNumber);
            return Convert.ToBase64String(rendomNumber);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();
            return refreshToken;
        }

        private async Task<User?> ValidateRefreshTokenAsync(Guid id, string refreshToken)
        {
            var user = await context.Users
                .Include(u => u.PersonalInfo)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user is null || user.RefreshToken != refreshToken ||
                user.RefreshTokenExpireTime <= DateTime.UtcNow)
            {
                return null;
            }

            return user;
        }
        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (user is null)
                return null;
            var response = new TokenResponseDto()
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };
            return response;
        }
    }
}
