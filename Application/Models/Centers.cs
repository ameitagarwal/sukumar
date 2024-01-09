namespace TMS_Traning_Management.Models
{
	public class Center
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string TitleEn { get; set; }
        public string TitleAr { get; set; }
        public string TitleUr { get; set; }
        public int LocationId { get; set; }
        public Location? Location { get; set; }
        public int OrderBy { get; set; }
        public string? Url { get; set; }
    }
}
