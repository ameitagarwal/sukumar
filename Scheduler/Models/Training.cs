namespace TMS_Training_Management_StatusChange_Scheduler
{
    public class Training
    {
        public int Id { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int LocationId { get; set; }
        public int CenterId { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
    }
}
