namespace finansije.Models
{
    public class UserRegistrationDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public PersonalInfoDto PersonalInfo { get; set; }
        public AddressInfoDto AddressInfo { get; set; }
        public string Role { get; set; }
        public bool AcceptTerms { get; set; }

    }
}
