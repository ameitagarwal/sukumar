using TMS_Traning_Management.Services;

namespace TMS_Traning_Management.Models
{
	public class ShowPaging
	{
        public int InputNumber { get; set; }
        public List<MyRegistration> DisplayResult { get; set; }

		//public PageInfo PageInfo;

		//public string EmiratesID { get; set; }
	}
}
