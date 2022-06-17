using System.ComponentModel.DataAnnotations;

namespace WebApiWithStoreProc.Api.Model
{
    public class RegisterModel
    {
        [Required(ErrorMessage ="User name is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Passqord is required")]
        public string Password { get; set; }
    }
}
