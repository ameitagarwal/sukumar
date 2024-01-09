using TMS_Traning_Management.Models;
using TMS_Traning_Management.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using System.Threading.Tasks.Dataflow;
using NLog.Fluent;
using Org.BouncyCastle.Ocsp;
using TMS_Traning_Management.ViewModels;

namespace TMS_Traning_Management.Services
{
	public class TrainingsService : ITrainingsService
	{
		private readonly AppDbContext _dbContext;
		private readonly IConfiguration _config;
		public TrainingsService(AppDbContext dbContext, IConfiguration config)
		{
			_dbContext = dbContext;
			_config = config;
		}
        public Training Update(Training model)
        {
            try
            {
                _dbContext.Trainings.Update(model);
                _dbContext.SaveChanges();

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Training Delete(Training model)
        {
            try
            {
                _dbContext.Trainings.Remove(model);
                _dbContext.SaveChanges();

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Training> GetTrainingsDetails()
		{
			try
			{
				
				return (from trg in _dbContext.Trainings
						join reg in _dbContext.Registrations
						on trg.Id equals reg.TrainingId into Temp
						from A in Temp.DefaultIfEmpty()
							
						select new Training
						{
							Id = trg.Id,
							CenterId = trg.CenterId,
							LocationId = trg.LocationId,
							Location = trg.Location,
							Center = trg.Center,
							TotalSeats = trg.TotalSeats,
							AvailableSeats = trg.AvailableSeats,
							StartDateTime = trg.StartDateTime,
							EndDateTime = trg.EndDateTime,
							RegistrationNo = A.RegistrationNo

						}).ToList();
				//return _dbContext.Trainings.Include(t => t.Location).Include(t => t.Center).ToList();


			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public Training GetTrainingsDetailsById(int? id)
		{
			try
			{
				return _dbContext.Trainings.Where(r => r.Id == id).Include(t => t.Location).Include(r => r.Center).FirstOrDefault();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		public List<Registration> GetRegistrationDetailsByTrainingId(int id)
		{
			try
			{
				return _dbContext.Registrations.Where(r => r.TrainingId == id).ToList();
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
	}
}
