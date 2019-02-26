using System.ComponentModel.DataAnnotations;

namespace Bidster.Models.Users
{
    public class UserModel
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }

        [MaxLength(50)]
        public string Address { get; set; }
        [MaxLength(50)]
        public string Address2 { get; set; }
        [MaxLength(50)]
        public string City { get; set; }
        [MaxLength(5)]
        public string State { get; set; }
        [MaxLength(15)]
        public string PostalCode { get; set; }
    }
}
