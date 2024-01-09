using System.ComponentModel.DataAnnotations;
using TMS_Traning_Management.Models;

namespace TMS_Traning_Management.ViewModels
{
    public class HomePagePaymentViewModel
    {
		[Required(ErrorMessage = "Home Page Make Payment Registration Number")]
		public string RegistrationNo { get; set; }
		public List<Center>? Centers { get; set; }
	}
}
