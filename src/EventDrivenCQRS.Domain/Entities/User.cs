namespace EventDrivenCQRS.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public required string Name { get; set; } // 'required' ekledik
        public required string Email { get; set; } // 'required' ekledik
    }
}