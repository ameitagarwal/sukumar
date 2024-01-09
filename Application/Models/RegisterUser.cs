namespace TMS_Traning_Management.Models
{
	public class RegisterUser
	{
		public string EmiratesID { get; set; } = null!;
		public string EnglishName { get; set; } = null;
		public string ArabicName { get; set; } = null;
		public string Mobile { get; set; } = null!;
		public string Email { get; set; } = null!;
		//public string Nationality { get; set; } = null!;
		public string? DOB { get; set; }
		public string? Gender { get; set; }
		public string? Address { get; set; }
		public string? PreferredLang { get; set; }
		public string? UserName { get; set; }
		public string? Password { get; set; }
		public string? SecurityQues { get; set; }
		public string? SecurityAns { get; set; }
	}
}
