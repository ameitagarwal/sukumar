using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS_Training_Management_StatusChange_Scheduler
{
    public class Registration
    {
        public int Id { get; set; }
        public string? RegistrationNo { get; set; }
        public string? Entity { get; set; }
        public string? EntityName { get; set; }
        public string? DriverName { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? EmiratesID { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TransactionID { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DateTime? TTC { get; set; }
		public int TrainingId { get; set; }

        public Training? Training { get; set; }
	}
}
