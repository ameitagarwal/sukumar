using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Localization;
using TMS_Traning_Management.Controllers;
using TMS_Traning_Management.Models;

namespace TMS_Traning_Management.ViewModels
{
    public class RegistrationViewModel
    {
        

		// The below error messages are Localized
		[Required(ErrorMessage = "Registration Entity Required")]
        public string Entity { get; set; }
        public string? EntityName { get; set; }
        [Required(ErrorMessage = "Registration Driver Name Required")]
        public string DriverName { get; set; }

        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Registration Email InValid")]
		public string? Email { get; set; }

        [Required(ErrorMessage = "Registration Mobile Required")]
        [RegularExpression("^971-(?:50|51|52|55|56|58|2|3|4|6|7|9)\\d{7}$", ErrorMessage = "Registration Mobile InValid")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Registration EmiratesID Required")]
        [RegularExpression("^784-[0-9]{4}-[0-9]{7}-[0-9]{1}$", ErrorMessage = "Registration EmiratesID InValid")]
        public string EmiratesID { get; set; }

		[Required(ErrorMessage = "Registration Location Required")]
		[Range(1, int.MaxValue, ErrorMessage = "Registration Location Required")]
		public int Location { get; set; }

        [Required(ErrorMessage = "Registration Training Center Required")]
		[Range(1, int.MaxValue, ErrorMessage = "Registration Training Center Required")]
		public int Center { get; set; }

		[Required(ErrorMessage = "Registration Training Required")]
		[Range(1, int.MaxValue, ErrorMessage = "Registration Training Required")]
		public int TrainingId { get; set; }
		public Training? Trainings { get; set; }
		public List<Entity>? Entities { get; set; }
        public List<Location>? Locations { get; set; }
        public List<Center>? Centers { get; set; }
        public List<Registration>? Registration { get; set; }
    }
}
