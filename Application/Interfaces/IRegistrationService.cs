using TMS_Traning_Management.Models;

namespace TMS_Traning_Management.Interfaces
{
	public interface IRegistrationService
	{
		Registration Create(Registration model);
		Registration GetRegistrationDetailsById(int id);
		Registration GetRegistrationDetailsByEmiratesId(string EmiratesID, int TrainingId);
		List<Registration> GetRegistrationDetailsByEmiratesId(string EmiratesID);
		List<MyRegistration> GetMyRegistrationDetailsByEmiratesId(string EmiratesID);
		List<Location> GetLocations();
		List<Training> GetTrainings(int LocationId, int CenterId);
		List<Entity> GetEntities();
        List<Center> GetCenters();
        List<Center> GetCentersByLocation(int LocationId);
		Center GetCenterDetails(int Id);
    }
}
