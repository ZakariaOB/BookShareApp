using System.ComponentModel.DataAnnotations;

namespace BookShareApp.API.Dto
{
    public class UserForRegisterDto
    {
        [Required]
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}