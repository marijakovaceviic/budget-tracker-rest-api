using Azure.Core;
using finansije.Entities;
using finansije.Models;
using finansije.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace finansije.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUsersService usersService, IValidator<UserRegistrationDto> validator, IValidator<AddressInfoDto> addressValidator) : ControllerBase
    {
        
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDto request)
        {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new { field = e.PropertyName, error = e.ErrorMessage });

                return BadRequest(errors);
            }
            var status = await usersService.RegisterAsync(request, "user");
            if (status == -1)
                return BadRequest("Invalid role");
            if (status == -2)
                return BadRequest("Username already exists");
            if (status == -3)
                return BadRequest("Already exists profil with this email");
            if (status == -4)
                return BadRequest("Username cannot contain word 'admin'");
            return Ok("Successful registration!");
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(UserLoginDto request)
        {
            var result = await usersService.LoginAsync(request);
            if (result is null)
                return BadRequest("Invalid username or password");
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("all-users")]
        public async Task<ActionResult<List<UserProfilDto>>> GetAllUsers()
        {
            return Ok(await usersService.GetAllUsers("user"));
        }

        [Authorize(Roles = "admin")]
        [HttpGet("all-admins")]
        public async Task<ActionResult<List<UserProfilDto>>> GetAllAdmins()
        {
            return Ok(await usersService.GetAllUsers("admin"));
        }

        [Authorize(Roles = "admin")]
        [HttpPost("addAdmin")]
        public async Task<IActionResult> RegisterAdmin(UserRegistrationDto request)
        {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new { field = e.PropertyName, error = e.ErrorMessage });

                return BadRequest(errors);
            }
            var status = await usersService.RegisterAsync(request, "admin");
            if(status == -1)
                return BadRequest("Invalid role");
            if (status == -2)
                return BadRequest("Username already exists");
            if (status == -3)
                return BadRequest("Already exists profil with this email");

            return Ok("Successful registration!");
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("deleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var status = await usersService.DeleteUserAsync(id);
            if (status != 0)
                return NotFound();
            return NoContent();
        }

        [Authorize]
        [HttpPut("changePhoneNumber")]
        public async Task<IActionResult> ChangePhoneNumber([FromBody] string number)
        {
            var status = await usersService.ChangePhoneNumberAsync(number);
            if (status != 0)
                return NotFound();
            return NoContent();
        }

        [Authorize]
        [HttpPut("changeAddressInfo")]
        public async Task<IActionResult> ChangeAddressInfo(AddressInfoDto addressInfo)
        {
            var validationResult = addressValidator.Validate(addressInfo);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            var status = await usersService.ChangeAddressInfoAsync(addressInfo);
            if (status != 0)
                return NotFound();
            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpGet("sortUsersByUsername")]
        public async Task<ActionResult<List<UserProfilDto>>> GetSortedUsersByUsername()
        {
            var result = await usersService.SortUsersAsync();
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("sortUsersByUsernameDesc")]
        public async Task<ActionResult<List<UserProfilDto>>> GetSortedUsersByUsernameDesc()
        {
            var result = await usersService.SortUsersAsync("user", "username", false);
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("sortUsersByEmail")]
        public async Task<ActionResult<List<UserProfilDto>>> GetSortedUsersByEmail()
        {
            var result = await usersService.SortUsersAsync("user", "email", true);
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("sortUsersByEmailDesc")]
        public async Task<ActionResult<List<UserProfilDto>>> GetSortedUsersByEmailDesc()
        {
            var result = await usersService.SortUsersAsync("user","email", false);
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("sortAdminsByUserName")]
        public async Task<ActionResult<List<UserProfilDto>>> GetSortedAdminsByUsername()
        {
            var result = await usersService.SortUsersAsync("admin", "username", true);
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("sortAdminsByUserNameDesc")]
        public async Task<ActionResult<List<UserProfilDto>>> GetSortedAdminsByUsernameDesc()
        {
            var result = await usersService.SortUsersAsync("admin", "username", false);
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("sortAdminsByEmail")]
        public async Task<ActionResult<List<UserProfilDto>>> GetSortedAdminsByEmail()
        {
            var result = await usersService.SortUsersAsync("admin", "email", true);
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("sortAdminsByEmailDesc")]
        public async Task<ActionResult<List<UserProfilDto>>> GetSortedAdminsByEmailDesc()
        {
            var result = await usersService.SortUsersAsync("admin", "email", false);
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("sortUsersByTransactionCount")]
        public async Task<ActionResult<List<UserProfilDto>>> GetUsersSortedByTransactionCount()
        {
            var result = await usersService.SortUserByTransactionCountAsync();
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await usersService.RefreshTokenAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
                return Unauthorized("Invalid refresh token");

            return Ok(result);
        }
    }
}
