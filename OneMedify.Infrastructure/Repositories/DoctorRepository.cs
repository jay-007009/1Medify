using Microsoft.EntityFrameworkCore;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Data;
using OneMedify.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneMedify.Infrastructure.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly OneMedifyDbContext _oneMedifyDbContext;

        public DoctorRepository(OneMedifyDbContext oneMedifyDbContext)
        {
            _oneMedifyDbContext = oneMedifyDbContext;
        }

        /// <summary>
        /// Method To Add doctor In Database
        /// </summary>
        /// <param name="newDoctor"></param>
        /// <returns></returns>
        public async Task<bool> AddDoctorAsync(Doctor newDoctor)
        {
            try
            {
                var doctor = await _oneMedifyDbContext.Doctors.AddAsync(newDoctor);
                await _oneMedifyDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Method To Update Doctor's CreatedBy Id In Database
        /// </summary>
        /// <param name="doctor"></param>
        /// <returns></returns>
        public async Task<bool> UpdateDoctorAsync(Doctor doctor)
        {
            try
            {
                doctor.CreatedBy = doctor.DoctorId;
                _oneMedifyDbContext.Doctors.Update(doctor);
                await _oneMedifyDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Method To check If Doctor's mobile Number Is Exist In The Database Or Not
        /// </summary>
        /// <param name="doctormobileno"></param>
        /// <returns></returns>
        public bool IsValidMobileNumber(string doctormobileno)
        {
            try
            {
                if (doctormobileno == null)
                {
                    return true;
                }
                var doctor = _oneMedifyDbContext.Doctors.Where(doctor => doctor.MobileNumber == doctormobileno && doctor.IsDisable == false).FirstOrDefault();
                if (doctor == null)
                {
                    return true;

                }
                return false;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Method to Get Doctor by Doctor's Mobile Number 
        /// </summary>
        /// <param name="doctormobileNo"></param>
        /// <returns></returns>
        public async Task<Doctor> GetDoctorByDoctorMobileNumberAsync(string doctormobileNo)
        {
            try
            {
                return await _oneMedifyDbContext.Doctors.Include(city => city.City)
                                                        .ThenInclude(state => state.State)
                                                        .Where(mobile => mobile.MobileNumber == doctormobileNo && mobile.IsDisable == false)
                                                        .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        /// <summary>
        /// Method for Getting Doctor by Mobile Number
        /// </summary>
        /// <param name="doctormobileNumber"></param>
        /// <returns></returns>
        public async Task<Doctor> GetDoctorAsync(string doctormobileNumber)
        {
            try
            {
                var doctor = await _oneMedifyDbContext.Doctors.FirstOrDefaultAsync(doctor => doctor.MobileNumber == doctormobileNumber && doctor.IsDisable == false);
                return doctor;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Method for Updating Doctor profile in Database
        /// </summary>
        /// <param name="doctor"></param>
        /// <returns></returns>
        public async Task<bool> UpdateDoctorProfileAsync(Doctor doctor)
        {
            try
            {
                _oneMedifyDbContext.Doctors.Update(doctor);
                await _oneMedifyDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Method for Validating Mobile Number of Doctor
        /// </summary>
        /// <param name="doctormobileNumber"></param>
        /// <returns></returns>
        public async Task<bool> IsMobileNumberValidAsync(string doctormobileNumber)
        {
            try
            {
                var doctor = await _oneMedifyDbContext.Doctors.FirstOrDefaultAsync(doctor => doctor.MobileNumber == doctormobileNumber && doctor.IsDisable == false);
                return true;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Get doctor's details by doctor mobile number.
        /// </summary>
        public async Task<Doctor> ReadDoctorByMobileNumber(string doctormobileNumber)
        {
            try
            {
                return await _oneMedifyDbContext.Doctors.FirstOrDefaultAsync(doctor => doctor.MobileNumber == doctormobileNumber && doctor.IsDisable == false);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<int> GetPatientsCountByDoctorIdAsync(int id)
        {
            try
            {
                return await _oneMedifyDbContext.Prescriptions.Include(prescription => prescription.Patient)
                                                   .Where(prescription => prescription.DoctorId == id && prescription.IsDisable == false)
                                                   .Select(prescription => prescription.Patient).Distinct().CountAsync();

            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public async Task<int> GetPharmaciesCount()
        {
            try
            {
                return await _oneMedifyDbContext.Pharmacies.Where(pharmacy => pharmacy.IsDisable == false).CountAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Method To Get List of All Registered Doctors From Database
        /// </summary>
        /// <returns></returns>
        public async Task<List<Doctor>> GetAllDoctorsAsync()
        {
            try
            {
                return await _oneMedifyDbContext.Doctors.OrderByDescending(doctor => doctor.CreatedDate)
                                                        .Select(doctor => new Doctor()
                                                        {
                                                            ProfilePictureFileName = doctor.ProfilePictureFileName,
                                                            ProfilePictureFilePath = doctor.ProfilePictureFilePath,
                                                            FirstName = doctor.FirstName,
                                                            LastName = doctor.LastName,
                                                            MobileNumber = doctor.MobileNumber,
                                                            Specialization = doctor.Specialization,
                                                            InstituteName = doctor.InstituteName,
                                                            IsDisable = doctor.IsDisable
                                                        }).ToListAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Method To Get Count Of All Registered Doctors From Database
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetDoctorsCountAsync()
        {
            try
            {
                return await _oneMedifyDbContext.Doctors.Where(doctor => doctor.IsDisable == false).CountAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public List<Doctor> GetTenDoctors()
        {
            try
            {
                return _oneMedifyDbContext.Doctors.Select(d => new Doctor(){ DoctorId = d.DoctorId })
                        .OrderBy(d => d.DoctorId).Take(10).ToList();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Get doctor by id.
        /// </summary>
        public async Task<Doctor> ReadById(int doctorId)
        {
            try
            {
                return await _oneMedifyDbContext.Doctors.FindAsync(doctorId);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}