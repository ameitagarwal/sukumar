using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS_Traning_Management.Models
{
    public class Training
    {
        public int Id { get; set; }
		[NotMapped]
		public string? EncryptedId { get; set; }
		[NotMapped]
		public string? RegistrationNo { get; set; }
		public int? LocationId { get; set; }
        public int? CenterId { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
		public DateTime StartDateTime { get; set; }
		public DateTime EndDateTime { get; set; }
		public Location? Location { get; set; }
        public Center? Center { get; set; }
        public int OrderBy { get; set; }
    }
}
