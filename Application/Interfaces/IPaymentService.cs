using TMS_Traning_Management.Models;

namespace TMS_Traning_Management.Interfaces
{
	public interface IPaymentService
    {
		Registration GetRegistrationDetails(string RegistrationNo);
		Registration GetRegistrationDetailsById(int id);
		List<Training> GetTrainings(int LocationId, int CenterId);
		Registration ChangeTraining(int id, int TrainingId);
		Registration UpdateRegistrationDetails(int id, string PaymentStatus, string TransactionID);
        bool isRegistrationExist(string RegistrationNo);
        ComtrustRegistrationResponse RegisterPayment(string RegistrationNo, string returnUrl);
		ComtrustFinalizationResponse FinalizePayment(string RegistrationNo, string TransactionID);
		Training GetTrainingDetailsById(int id);
	}
}
