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
    public class PharmacyRepository : IPharmacyRepository
    {
        private readonly OneMedifyDbContext _oneMedifyDbContext;

        public PharmacyRepository(OneMedifyDbContext oneMedifyDbContext)
        {
            _oneMedifyDbContext = oneMedifyDbContext;
        }
        /// <summary>
        /// Method for Getting Pharmacy by MobileNumber
        /// </summary>
        /// <param name="mobilenumber"></param>
        /// <returns></returns>
        public async Task<Pharmacy> GetPharmacyByMobileNumberAsync(string mobilenumber)
        {
            try
            {
                return await _oneMedifyDbContext.Pharmacies.Include(pharmacy => pharmacy.City)
                                                           .ThenInclude(city => city.State)
                                                           .Where(pharmacies => pharmacies.MobileNumber == mobilenumber && pharmacies.IsDisable == false)
                                                           .FirstOrDefaultAsync();

            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        /// <summary>
        /// Method for Registering Pharmacy in Database
        /// </summary>
        /// <param name="pharmacy"></param>
        /// <returns></returns>
        public bool PharmacyRegistration(Pharmacy pharmacy)
        {
            try
            {
                var addPharmacy = _oneMedifyDbContext.Pharmacies.Add(pharmacy);
                _oneMedifyDbContext.SaveChanges();
                pharmacy.CreatedBy = pharmacy.PharmacyId;
                _oneMedifyDbContext.Pharmacies.Update(pharmacy);
                _oneMedifyDbContext.SaveChanges();
                return true;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }


        /// <summary>
        /// Method for Updating Pharmacy profile in Database
        /// </summary>
        /// <param name="pharmacy"></param>
        /// <returns></returns>
        public async Task<bool> UpdatePharmacyProfileAsync(Pharmacy pharmacy)
        {
            try
            {
                _oneMedifyDbContext.Pharmacies.Include(city => city.City).ThenInclude(state => state.State);
                _oneMedifyDbContext.Pharmacies.Update(pharmacy);
                await _oneMedifyDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }


        /// <summary>
        /// Method for Getting Pharmacy by Mobile Number
        /// </summary>
        /// <param name="pharmacymobileNumber"></param>
        /// <returns></returns>
        public async Task<Pharmacy> GetPharmacyAsync(string pharmacymobileNumber)
        {
            try
            {
                var pharmacy = await _oneMedifyDbContext.Pharmacies.FirstOrDefaultAsync(pharmacy => pharmacy.MobileNumber == pharmacymobileNumber && pharmacy.IsDisable == false);
                return pharmacy;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }



        /// <summary>
        /// Method To Get All Registered Pharmacy List
        /// </summary>
        /// <param name="pageindex"></param>
        /// <returns></returns>
        public async Task<List<Pharmacy>> GetAllPharmacy(int pageindex)
        {
            try
            {
                return await _oneMedifyDbContext.Pharmacies.OrderByDescending(x => x.CreatedDate)
                                                           .Include(pharmacy => pharmacy.City)
                                                           .ThenInclude(city => city.State).Select(p => new Pharmacy()
                {
                    PharmacyName = p.PharmacyName,
                    MobileNumber = p.MobileNumber,
                    ProfilePictureFilePath = p.ProfilePictureFilePath,
                    City = p.City,
                    IsDisable = false
                }).ToListAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Method for Getting Uploaded Prescriptions of Patients by PharmacyMobileNumber  
        /// </summary>
        /// <param name="pharmacyMobileNumber"></param>
        /// <returns></returns>
        public async Task<List<PrescriptionUpload>> PharmacyUploadedPrescriptionByPharmacyMobileNumberAsync(string pharmacyMobileNumber)
        {
            try
            {
                return await _oneMedifyDbContext.PrescriptionUploads.Include(prescriptionUpload => prescriptionUpload.Prescription)
                                                                    .ThenInclude(prescription => prescription.Patient)
                                                                    .ThenInclude(patient => patient.PatientDisease)
                                                                    .ThenInclude(patientDisease => patientDisease.Disease)
                                                                    .Include(prescriptionUpload => prescriptionUpload.Prescription)
                                                                    .ThenInclude(prescription => prescription.ApprovedByDoctor)
                                                                    .Where(prescriptionUpload => prescriptionUpload.Prescription.Pharmacy.MobileNumber == pharmacyMobileNumber
                                                                                  && prescriptionUpload.IsDisable == false && prescriptionUpload.Prescription.PrescriptionType == false
                                                                                  && prescriptionUpload.Prescription.PharmacyId != null)
                                                                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public async Task<List<Disease>> ReadDiseaseByIds(List<int> diseaseIds)
        {
            try
            {
                return await _oneMedifyDbContext.Diseases.Where(x => diseaseIds.Contains(x.DiseaseId)).ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Method for Updating Pharmacy in Database
        /// </summary>
        /// <param name="pharmacy"></param>
        /// <returns></returns>
        public bool UpdatePharmacy(Pharmacy pharmacy)
        {
            try
            {
                pharmacy.CreatedBy = pharmacy.PharmacyId;
                _oneMedifyDbContext.Pharmacies.Update(pharmacy);
                _oneMedifyDbContext.SaveChanges();
                return true;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }


        /// <summary>
        /// Method for Checking CityId Exists in Database Or Not
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public bool VerfifyCity(int cityId)
        {
            try
            {
                var city = _oneMedifyDbContext.Cities.Where(city => city.CityId == cityId && city.IsDisable == false).FirstOrDefault();
                _oneMedifyDbContext.SaveChanges();
                return city == null;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }


        /// <summary>
        /// validation for Mobile Number
        /// </summary>
        public bool IsValidMobileNo(string mobileNumber)
        {
            try
            {
                if (mobileNumber == null)
                {
                    return true;
                }
                var PharmacyMobileNumber = _oneMedifyDbContext.Pharmacies.Where(x => x.MobileNumber == mobileNumber && x.IsDisable == false)
                                                                      .Select(x => x.MobileNumber).FirstOrDefault();
                if (PharmacyMobileNumber == null)
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
    }
}