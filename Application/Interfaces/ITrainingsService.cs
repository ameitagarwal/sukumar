using TMS_Traning_Management.Models;

namespace TMS_Traning_Management.Interfaces
{
	public interface ITrainingsService
	{
		List<Training> GetTrainingsDetails();
		Training GetTrainingsDetailsById(int? id);
        Training Update(Training model);
		Training Delete(Training model);
		List<Location> GetLocations();
		List<Center> GetCenters();
		List<Registration> GetRegistrationDetailsByTrainingId(int id);

	}
}
