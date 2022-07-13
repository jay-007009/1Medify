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
    public class PatientRepository : IPatientRepository
    {
        private readonly OneMedifyDbContext _oneMedifyDbContext;

        public PatientRepository(OneMedifyDbContext context)
        {
            _oneMedifyDbContext = context;
        }

        /// <summary>
        /// Patient Registration
        /// </summary>
        public Patient CreatePatient(Patient patient)
        {
            try
            {
                _oneMedifyDbContext.Patients.Add(patient);
                _oneMedifyDbContext.SaveChanges();
                return patient;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Patient> PatientByMobileNumberAsync(string mobileNumber)
        {
            try
            {
                return await _oneMedifyDbContext.Patients.Include(disease => disease.PatientDisease)
                                                         .ThenInclude(dieseas => dieseas.Disease)
                                                         .FirstOrDefaultAsync(patient => patient.MobileNumber == mobileNumber && patient.IsDisable == false);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public Patient UpdatePatient(Patient patient)
        {
            try
            {
                _oneMedifyDbContext.Patients.Update(patient);
                _oneMedifyDbContext.SaveChanges();
                return patient;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public bool ValiadateDiseaseIds(List<int> diseaseIds)
        {
            try
            {
                foreach (var Id in diseaseIds)
                {
                    var disease = _oneMedifyDbContext.Diseases.Select(disease => new Disease()
                    {
                        DiseaseId = Convert.ToInt32(disease.DiseaseId),
                        IsDisable = false
                    }).FirstOrDefault();
                    _oneMedifyDbContext.SaveChanges();
                    if (disease == null)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<int> GetDoctorIdByDoctoMobileAsync(string mobileNumber)
        {
            try
            {
                return await _oneMedifyDbContext.Doctors.Where(doctor => doctor.MobileNumber == mobileNumber && doctor.IsDisable == false)
                                                        .Select(doctor => doctor.DoctorId)
                                                        .FirstOrDefaultAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public async Task<List<Prescription>> GetPrescriptionsByDoctorIdAsync(int id)
        {
            try
            {
                return await _oneMedifyDbContext.Prescriptions.Include(prescription => prescription.Patient)
                                                   .Where(prescription => prescription.DoctorId == id && prescription.IsDisable == false)
                                                   .OrderByDescending(prescription => prescription.CreatedDate)
                                                   .ToListAsync();

            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Method for getting patients created prescription count by mobile number
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        public async Task<int> PatientCreatedPrescriptionByPatientMobileNumberAsync(string mobileNumber)
        {
            try
            {
                return await _oneMedifyDbContext.Prescriptions
                             .Where(prescription => prescription.PrescriptionType == true && prescription.IsDisable == false && prescription.Patient.MobileNumber == mobileNumber).CountAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Async method to check if entered mobile number is exists in Database or not.
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        public async Task<bool> IsDoctorMobileNumberExistsAsync(string mobileNumber)
        {
            try
            {
                var doctor = await _oneMedifyDbContext.Doctors.Where(doctor => doctor.MobileNumber == mobileNumber && doctor.IsDisable == false).FirstOrDefaultAsync();
                if (doctor != null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }


        /// <summary>
        /// Get patient's details by patient mobile number.[with disease]
        /// </summary>
        public async Task<Patient> ReadPatientByMobileNumber(string mobileNumber)
        {
            try
            {
                return await _oneMedifyDbContext.Patients.Include(patient => patient.PatientDisease).ThenInclude(x => x.Disease).Include(patient => patient.City).ThenInclude(city => city.State)
                    .FirstOrDefaultAsync(patient => patient.MobileNumber == mobileNumber && patient.IsDisable == false);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Method for Getting Uploaded Prescriptions of Patient 
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        public async Task<List<PrescriptionUpload>> PatientsUploadedPrescriptionByPatientMobileNumberAsync(string mobileNumber, int pageIndex)
        {
            try
            {
                return await _oneMedifyDbContext.PrescriptionUploads.Include(prescriptionUpload => prescriptionUpload.Prescription)
                                                                    .ThenInclude(prescription => prescription.Patient)
                                                                    .ThenInclude(patient => patient.PatientDisease)
                                                                    .ThenInclude(patientDisease => patientDisease.Disease)
                                                                    .Include(prescriptionUpload => prescriptionUpload.Prescription)
                                                                    .ThenInclude(prescription => prescription.ApprovedByDoctor)
                                                                    .Where(prescriptionUpload => prescriptionUpload.Prescription.Patient.MobileNumber == mobileNumber
                                                                                    && prescriptionUpload.IsDisable == false && prescriptionUpload.Prescription.PharmacyId == null
                                                                                    && prescriptionUpload.Prescription.PrescriptionType == false)
                                                                    .OrderByDescending(prescriptionUpload => prescriptionUpload.Prescription.CreatedDate)
                                                                    .Skip(pageIndex * 10).Take(10)
                                                                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Get Patient PrescriptionMedicine By PrescriptionId
        /// </summary>
        public async Task<IEnumerable<PrescriptionMedicine>> MedicineByPrescriptionIdAsync(int id)
        {
            try
            {
                return await _oneMedifyDbContext.Prescriptions.Include(prescription => prescription.PrescriptionMedicines)
                                                              .ThenInclude(prescriptionMedicine => prescriptionMedicine.Medicine)
                                                              .Where(prescription => prescription.PrescriptionId == id && prescription.IsDisable == false)
                                                              .Select(prescription => prescription.PrescriptionMedicines).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Get Patient Medicine Details By PrescriptionId
        /// </summary>
        public async Task<IEnumerable<Medicine>> PrescriptionMedicineByPrescriptionIdAsync(int id)
        {
            try
            {
                return await _oneMedifyDbContext.Prescriptions.Include(disease => disease.PrescriptionMedicines)
                                                              .ThenInclude(prescription => prescription.Medicine)
                                                              .ThenInclude(x => x.Disease)
                                                              .Where(patient => patient.PrescriptionId == id && patient.IsDisable == false)
                                                              .Select(x => x.PrescriptionMedicines.Select(x => x.Medicine)).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
                var PatientMobileNumber = _oneMedifyDbContext.Patients.Where(x => x.MobileNumber == mobileNumber && x.IsDisable == false)
                                                                      .Select(x => x.MobileNumber).FirstOrDefault();
                if (PatientMobileNumber == null)
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
        /// Method for Getting Doctor by Mobile Number
        /// </summary>
        /// <param name="patientmobileNumber"></param>
        /// <returns></returns>
        public async Task<Patient> GetPatientAsync(string patientmobileNumber)
        {
            try
            {
                var patient = await _oneMedifyDbContext.Patients.FirstOrDefaultAsync(patient => patient.MobileNumber == patientmobileNumber && patient.IsDisable == false);
                return patient;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }


        /// <summary>
        /// Get List of Prescriptions Status Send For Approval by Patient Mobile Number
        /// </summary>
        /// <param name="patientMobileNumber"></param>
        /// <returns></returns>
        public async Task<List<Prescription>> GetPrescriptionStatusByPatientMobileNumberAsync(string patientMobileNumber)
        {
            try
            {

                return await _oneMedifyDbContext.Prescriptions.Include(patient => patient.Patient)
                                                              .Where(p => p.Patient.MobileNumber == patientMobileNumber && p.Patient.IsDisable == false
                                                              && p.IsDisable == false && p.PrescriptionType == true && p.ModifiedByPatient != null)
                                                              .Include(p => p.PrescriptionMedicines)
                                                              .ThenInclude(m => m.Medicine)
                                                              .ThenInclude(d => d.Disease)
                                                              .Include(x => x.Doctor).ToListAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }


        /// <summary>
        /// Method for Getting Uploaded Prescriptions of Patient 
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        public async Task<List<PrescriptionUpload>> PatientsUploadedPrescriptionByPatientMobileNumberAsync(string mobileNumber)
        {
            try
            {
                return await _oneMedifyDbContext.PrescriptionUploads.Include(prescriptionUpload => prescriptionUpload.Prescription)
                                                                    .ThenInclude(prescription => prescription.Patient)
                                                                    .ThenInclude(patient => patient.PatientDisease)
                                                                    .ThenInclude(patientDisease => patientDisease.Disease)
                                                                    .Include(prescriptionUpload => prescriptionUpload.Prescription)
                                                                    .ThenInclude(prescription => prescription.Doctor)
                                                                    .Where(prescriptionUpload => prescriptionUpload.Prescription.Patient.MobileNumber == mobileNumber && prescriptionUpload.IsDisable == false)
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
        /// To Update Patient Profile
        /// </summary>
        public async Task<bool> UpdatePatientProfileAsync(Patient patient)
        {
            try
            {
                _oneMedifyDbContext.Patients.Include(city => city.City).ThenInclude(state => state.State);
                _oneMedifyDbContext.Patients.Update(patient);
                await _oneMedifyDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Get List of Uploaded Prescriptions By Pharmacy by Patient Mobile Number
        /// </summary>
        /// <param name="patientMobileNumber"></param>
        /// <returns></returns>
        public async Task<List<PrescriptionUpload>> UploadedPrescriptionByPharmacyPatientMobileNumberAsync(string patientMobileNumber)
        {
            try
            {

                var result =  await _oneMedifyDbContext.PrescriptionUploads.Include(prescriptionUpload => prescriptionUpload.Prescription)
                                                                   .ThenInclude(prescription => prescription.Patient)
                                                                   .ThenInclude(patient => patient.PatientDisease)
                                                                   .ThenInclude(patientDisease => patientDisease.Disease)
                                                                   .Include(prescriptionUpload => prescriptionUpload.Prescription)
                                                                   .ThenInclude(prescription => prescription.ApprovedByDoctor)
                                                                   .Include(prescriptionUpload => prescriptionUpload.Prescription)
                                                                   .ThenInclude(prescription => prescription.Pharmacy)
                                                                   .Where(prescriptionUpload => prescriptionUpload.Prescription.Patient.MobileNumber == patientMobileNumber
                                                                   && prescriptionUpload.IsDisable == false && prescriptionUpload.Prescription.PrescriptionType == false && prescriptionUpload.Prescription.PharmacyId != null)
                                                                   .ToListAsync();
                return result;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Get Patient Prescription By MobileNumber
        /// </summary>
        public async Task<ICollection<Prescription>> PatientPrescriptionByPatientMobileNumberAsync(string mobileNumber)
        {
            try
            {
                return await _oneMedifyDbContext.Prescriptions.Include(prescription => prescription.Patient).Include(prescription => prescription.PrescriptionMedicines)
                                                .Where(prescription => prescription.PrescriptionType == true && prescription.Patient.MobileNumber == mobileNumber && prescription.IsDisable == false).ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}