using System;
using System.ComponentModel.DataAnnotations;

namespace BookShareApp.API.Dto
{
    public class UserForRegisterDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        //[StringLength(4, ErrorMessage = "The {0} must be at least {1} characters long.", MinimumLength = 5)]
        public string Password { get; set; }

        public string Gender { get; set; }

        public string knownAs { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastActive { get; set; }

        public UserForRegisterDto() {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
    }
}