using TMS_Traning_Management.Models;
using TMS_Traning_Management.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace TMS_Traning_Management.Services
{
	public class RegistrationService : IRegistrationService
	{
		private readonly AppDbContext _dbContext;
		private readonly IConfiguration _config;
		public RegistrationService(AppDbContext dbContext, IConfiguration config) {
			_dbContext = dbContext;
			_config = config;
		}
		public Registration Create(Registration model)
		{
			try
			{
				_dbContext.Registrations.Add(model);
				_dbContext.SaveChanges();
				var training = _dbContext.Trainings.Include(t=> t.Location).FirstOrDefault(t => t.Id == model.TrainingId);
				model.RegistrationNo = training.Location.Key + model.Id.ToString().PadLeft(6, '0');
				_dbContext.Registrations.Update(model);
				_dbContext.SaveChanges();

				model = _dbContext.Registrations
					.Include(r => r.Training)
					.ThenInclude(t => t.Center)
					.Include(r => r.Training)
					.ThenInclude(t => t.Location)
					.FirstOrDefault(r => r.Id == model.Id);
				return model;
			}
			catch(Exception ex)
			{
				throw ex;
			}
		}
		public Registration GetRegistrationDetailsById(int id)
		{
			try
			{
				return _dbContext.Registrations.Where(r => r.Id == id).Include(r => r.Training).FirstOrDefault();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public Registration GetRegistrationDetailsByEmiratesId(string EmiratesID, int TrainingId)
		{
			try
			{
				return _dbContext.Registrations.Where(r => r.EmiratesID == EmiratesID && r.TrainingId == TrainingId).Include(r => r.Training).FirstOrDefault();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public List<Registration> GetRegistrationDetailsByEmiratesId(string EmiratesID)
		{
			try
			{
				return _dbContext.Registrations.Where(r => r.EmiratesID == EmiratesID).Include(r => r.Training).ToList();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public List<MyRegistration> GetMyRegistrationDetailsByEmiratesId(string EmiratesID)
		{
			try
			{
				return (from reg in _dbContext.Registrations
						join trg in _dbContext.Trainings
						on reg.TrainingId equals trg.Id
						join loc in _dbContext.Locations
						on trg.LocationId equals loc.Id
						join cent in _dbContext.Centers
						on trg.CenterId equals cent.Id
						where reg.EmiratesID == EmiratesID

						select new MyRegistration
					   {
						   RegistrationDate = reg.Created.ToShortDateString(),
						   Location = loc.TitleEn,
						   Center = cent.TitleEn,
						   MapLink = cent.Url,
						   StartDateTime = Convert.ToDateTime(trg.StartDateTime).ToShortDateString(),
						   EndDateTime = Convert.ToDateTime(trg.EndDateTime).ToShortDateString()
						   
					   }).ToList();

			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public List<Location> GetLocations()
		{
			try
			{
				return _dbContext.Locations.ToList();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public List<Training> GetTrainings(int LocationId, int CenterId)
        {
			try
			{
				var days = _config["AllowRegistrationDaysBefore"] ?? "0";
				DateTime today = DateTime.Today.AddDays(Convert.ToInt32(days));
				return _dbContext.Trainings.Where(t => t.LocationId == LocationId && t.CenterId == CenterId && t.AvailableSeats > 0 && t.StartDateTime > today).ToList();
			}
			catch(Exception ex)
			{
				throw ex;
			}
        }
		public List<Entity> GetEntities()
		{
			try
			{
				return _dbContext.Entities.ToList();
			}
			catch (Exception ex)
			{
				throw ex;
			}
        }
        public List<Center> GetCenters()
        {
            try
            {
                return _dbContext.Centers.Include(c => c.Location).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Center> GetCentersByLocation(int LocationId)
        {
            try
            {
                return _dbContext.Centers.Where(c => c.LocationId == LocationId).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
		public Center GetCenterDetails(int Id)
		{
			try
			{
				return _dbContext.Centers.Where(c => c.Id == Id).FirstOrDefault();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}
