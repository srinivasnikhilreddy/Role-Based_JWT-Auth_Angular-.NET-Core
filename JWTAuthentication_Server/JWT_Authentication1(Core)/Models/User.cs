using System.ComponentModel.DataAnnotations;

// “User.cs” Model is to store the userName and password which we will use for authorization.
namespace JWT_Authentication1_Core_.Models
{
    public class User
    {
        public int userId { get; set; }

        [Required]
        [StringLength(50)]
        public string userName { get; set; }

        [Required]
        public string userPassword { get; set; }

        public string userRoles { get; set; }

        public string userEmail { get; set; }
        
    }
}
