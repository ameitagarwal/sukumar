using System.ComponentModel.DataAnnotations;
using TMS_Traning_Management.Models;

namespace TMS_Traning_Management.ViewModels
{
    public class PaymentViewModel
	{
		public int Id { get; set; }
		public string EncryptedId { get; set; }
		public string? RegistrationNo { get; set; }
		public string? Entity { get; set; }
		public string? EntityName { get; set; }
		public string? DriverName { get; set; }
		public string? Email { get; set; }
		public string? Mobile { get; set; }
		public string? EmiratesID { get; set; }
		public string? PaymentStatus { get; set; }
		public string? TransactionID { get; set; }

		[Required(ErrorMessage = "Registration Location Required")]
		[Range(1, int.MaxValue, ErrorMessage = "Registration Location Required")]
		public int LocationId { get; set; }

		[Required(ErrorMessage = "Registration Training Center Required")]
		[Range(1, int.MaxValue, ErrorMessage = "Registration Training Center Required")]
		public int CenterId { get; set; }

		[Required(ErrorMessage = "Registration Training Required")]
		[Range(1, int.MaxValue, ErrorMessage = "Registration Training Required")]
		public int TrainingId { get; set; }
		public Training? Training { get; set; }
		public List<Training>? Trainings { get; set; }
		public List<Location>? Locations { get; set; }
		public List<Center>? Centers { get; set; }
	}
}
