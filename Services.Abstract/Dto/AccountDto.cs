namespace Services.Abstract.Dto
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
