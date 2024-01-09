using System.ComponentModel.DataAnnotations;
using TMS_Traning_Management.Models;

namespace TMS_Traning_Management.ViewModels
{
    public class UserLoginViewModel
    {
        // [Required(ErrorMessage = "Home Page Make Payment Registration Number")]
        public string Username { get; set; }

        public string Password { get; set; }

        public string EmiratesID { get; set; }

        public string VerifyOTP { get; set; }

    }
}
