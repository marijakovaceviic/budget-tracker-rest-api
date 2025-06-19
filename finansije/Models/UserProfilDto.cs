namespace finansije.Models
{
    public class UserProfilDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public PersonalInfoDto? PersonalInfo { get; set; }
        public AddressInfoDto? AddressInfo { get; set; }

    }
}
