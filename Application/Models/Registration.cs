using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS_Traning_Management.Models
{
    public class Registration
    {
        public int Id { get; set; }
        [NotMapped]
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
        public Boolean? Attended { get; set; }
		public Boolean? Status { get; set; }
		public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DateTime? TTC { get; set; }
		public int TrainingId { get; set; }

        public Training? Training { get; set; }
	}
}
