namespace finansije.Entities
{
    public class AddressInfo
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public int HouseNumber { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public List<User?> Users { get; set; }
    }
}
