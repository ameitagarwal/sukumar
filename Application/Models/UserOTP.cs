using System.ComponentModel.DataAnnotations.Schema;

namespace TMS_Traning_Management.Models
{
	public class UserOTP
	{
		public int Id { get; set; }
		//[NotMapped]
		//public string? UserId { get; set; }
		public int? OTP { get; set; }
		public int? Status { get; set; }
		public string? EmiratesID { get; set; }
	}
}
