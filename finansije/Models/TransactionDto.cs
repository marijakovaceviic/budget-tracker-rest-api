namespace finansije.Models
{
    public class TransactionDto
    {
        public string Type { get; set; }
        public int Amount { get; set; }
        public int CategoryId { get; set; }
        public DateTime DateTime { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
