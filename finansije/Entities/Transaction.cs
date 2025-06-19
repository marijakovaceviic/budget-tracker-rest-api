namespace finansije.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int Amount { get; set; }
        public int CategoryId { get; set; }
        public DateTime DateTime { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
