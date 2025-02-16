namespace EmailApiTemplate.Models
{
    public class UserLogin : IBrokerMessage
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string CorrelationId { get; set; } = Guid.NewGuid().ToString();

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
