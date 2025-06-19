namespace finansije.Entities
{
    public class UserPersonalInfo
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
    }
}
