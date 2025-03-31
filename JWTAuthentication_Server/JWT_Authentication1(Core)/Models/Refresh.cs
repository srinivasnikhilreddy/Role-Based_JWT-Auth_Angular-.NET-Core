namespace JWT_Authentication1_Core_.Models
{
    public class Refresh
    {
        public int Id { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public int UserId { get; set; }  // Foreign key to User
        public User User { get; set; }   // Navigation property
    }

}
