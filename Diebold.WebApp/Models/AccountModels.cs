using System.ComponentModel.DataAnnotations;

namespace Diebold.WebApp.Models
{
    public class LogOnModel
    {
        [Required(ErrorMessage = "User name is required")]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public string DefaultUserName { get; set; }
        public string DefaultPassword { get; set; }
        public string Message { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }   
}
