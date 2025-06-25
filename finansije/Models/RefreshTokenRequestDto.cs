using System.ComponentModel.DataAnnotations;

namespace finansije.Models
{
    public class RefreshTokenRequestDto
    {
        [Required(ErrorMessage = "User id is required")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; }
    }
}
