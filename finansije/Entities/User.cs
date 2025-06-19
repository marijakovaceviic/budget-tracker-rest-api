namespace finansije.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }

        public UserPersonalInfo PersonalInfo { get; set; }
        public int AddressInfoId { get; set; }
        public AddressInfo AddressInfo { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
