using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TMS_Traning_Management.Models
{
    public class ApplicationUser : IdentityUser
    {
		[Column("EmiratesID")]
		[StringLength(256)]
		public string EmiratesID { get; set; } = null!;

		[Column("EnglishName")]
		[StringLength(256)]
		public string EnglishName { get; set; } = null!;

		[Column("ArabicName")]
		[StringLength(256)]
		public string ArabicName { get; set; } = null!;

		[Column("Mobile")]
		[StringLength(256)]
		public string Mobile { get; set; } = null!;

		[Column("Email")]
		[StringLength(256)]
		public string Email { get; set; } = null!;

		[Column("Nationality")]
		[StringLength(256)]
		public string Nationality { get; set; } = null!;

		[Column("DOB", TypeName = "datetime")]
		public DateTime? DOB { get; set; }

		[Column("Gender")]
		[StringLength(256)]
		public string? Gender { get; set; }

		[Column("Address")]
		[StringLength(256)]
		public string? Address { get; set; }

		[Column("PreferredLang")]
		[StringLength(256)]
		public string? PreferredLang { get; set; }

		
	}
}
