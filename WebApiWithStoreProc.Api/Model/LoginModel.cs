using System.ComponentModel.DataAnnotations;

namespace WebApiWithStoreProc.Api.Model
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Passqord is required")]
        public string Password { get; set; }
    }
}
