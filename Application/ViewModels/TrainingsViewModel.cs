using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TMS_Traning_Management.Models;

namespace TMS_Traning_Management.ViewModels
{
	public class TrainingsViewModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Training Location Required")]
		[Range(1, int.MaxValue, ErrorMessage = "Training Location Required")]
		public int LocationId { get; set; }

		[Required(ErrorMessage = "Training Center Required")]
		[Range(1, int.MaxValue, ErrorMessage = "Training Center Required")]
		public int CenterId { get; set; }

		[Required(ErrorMessage = "Training TotalSeats Required")]
		[Range(1, int.MaxValue, ErrorMessage = "Training TotalSeats Required")]
		public int TotalSeats { get; set; }

		[Required(ErrorMessage = "Training AvailableSeats Required")]
		[Range(1, int.MaxValue, ErrorMessage = "Training AvailableSeats Required")]
		public int AvailableSeats { get; set; }
		[Required(ErrorMessage = "Training StartDateTime Required")]
		public DateTime StartDateTime { get; set; }
		[Required(ErrorMessage = "Training EndDateTime Required")]
		public DateTime EndDateTime { get; set; }

		public Training? Training { get; set; }

 		public List<Registration>? Registration { get; set; }
        public List<Location>? Locations { get; set; } = new List<Location>();
		public List<Center>? Centers { get; set; } = new List<Center>();
	}

	public class RegistrationByTrainingViewModel
	{
		public int Id { get; set; }
		[NotMapped]
		public string? EncryptedId { get; set; }
		public int? LocationId { get; set; }
		public int? CenterId { get; set; }
		public int TotalSeats { get; set; }
		public int AvailableSeats { get; set; }
		public DateTime StartDateTime { get; set; }
		public DateTime EndDateTime { get; set; }
		public Location? Location { get; set; }
		public Center? Center { get; set; }
		public int OrderBy { get; set; }
		public int NoOfRegistrations { get; set; }
	}
}
