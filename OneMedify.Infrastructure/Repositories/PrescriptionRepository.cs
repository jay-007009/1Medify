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
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly OneMedifyDbContext _oneMedifyDbContext;

        public PrescriptionRepository(OneMedifyDbContext oneMedifyDbContext)
        {
            _oneMedifyDbContext = oneMedifyDbContext;
        }

        /// <summary>
        /// Craete prescription.
        /// </summary>
        public async Task<Prescription> Create(Prescription prescription)
        {
            try
            {
                await _oneMedifyDbContext.Prescriptions.AddAsync(prescription);
                await _oneMedifyDbContext.SaveChangesAsync();
                return prescription;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Prescription> GetPrescriptionById(int prescriptionId)
        {
            try
            {
                return _oneMedifyDbContext.Prescriptions.Include(p => p.Patient).Include(p => p.Pharmacy).Include(p => p.Doctor).Where(p => p.PrescriptionId == prescriptionId && p.IsDisable == false).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Read prescription by prescription id.
        /// </summary>
        public async Task<Prescription> ReadById(int prescriptionId)
        {
            try
            {
                return await _oneMedifyDbContext.Prescriptions.Include(p => p.Patient)
                                                              .Include(p => p.Pharmacy)
                                                              .Include(p => p.SentFromPharmacy)
                                                              .FirstOrDefaultAsync(p => p.IsDisable == false && p.PrescriptionId == prescriptionId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Get approved and rejected prescription by doctor mobile number sent from patient
        /// </summary>
        public async Task<List<Prescription>> GetApprovedAndRejectedPrescriptionByDoctorMobileNumberSentFromPatient(string doctorMobileNumber)
        {
            try
            {
                return await _oneMedifyDbContext.Prescriptions.Include(x => x.Patient).Include(x => x.ApprovedByDoctor).Where(p => p.ApprovedByDoctor.MobileNumber == doctorMobileNumber
                                                                      && ((p.ModifiedByPharmacy == null && p.ModifiedByPatient != null) 
                                                                      || (p.ModifiedByPharmacy == null && p.PharmacyId == null && p.ModifiedByPatient == null))
                                                                      && p.ApprovedByDoctor.IsDisable == false && p.IsDisable == false
                                                                      && (p.PrescriptionStatus == 1 || p.PrescriptionStatus == 3)).ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Get approved and rejected prescription by doctor mobile number sent from pharmacy
        /// </summary>
        public async Task<List<Prescription>> GetApprovedAndRejectedPrescriptionByDoctorMobileNumberSentFromPharmacy(string doctorMobileNumber)
        {
            try
            {
                return await _oneMedifyDbContext.Prescriptions.Include(x => x.Patient)
                                                              .Include(x => x.ApprovedByDoctor)
                                                              .Include(x => x.SentFromPharmacy)
                                                              .Include(x => x.Pharmacy)
                                                              .Where(p => p.ApprovedByDoctor.MobileNumber == doctorMobileNumber
                                                                      && (p.PharmacyId != null || p.ModifiedByPharmacy != null)
                                                                      && p.ApprovedByDoctor.IsDisable == false && p.IsDisable == false
                                                                      && (p.PrescriptionStatus == 1 ||
                                                                          p.PrescriptionStatus == 3)).ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Read all patient's approved and pending prescription by patient's mobile number with patient's details
        /// </summary>
        public async Task<List<Prescription>> GetApprovedAndPendingPrescriptionByPatientMobileNumber(string patientMobilenumber)
        {
            try
            {
                return await _oneMedifyDbContext.Prescriptions.Where(p => p.Patient.MobileNumber == patientMobilenumber
                                                                     && p.IsDisable == false
                                                                     && (p.PrescriptionStatus == 1 ||
                                                                          p.PrescriptionStatus == 2))
                                                              .Include(p => p.PrescriptionUpload)
                                                              .Include(pm => pm.PrescriptionMedicines)
                                                              .ThenInclude(m => m.Medicine)
                                                              .ThenInclude(d => d.Disease).ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Read all patient's created prescription sent for approval by pharmacy using patient's mobile number.
        /// </summary>
        public async Task<List<Prescription>> GetCreatedPrescriptionSentForApprovalByPharmacy(string patientMobilenumber)
        {
            try
            {
                return await _oneMedifyDbContext.Prescriptions.Where(p => p.Patient.MobileNumber == patientMobilenumber
                                                                    && p.IsDisable == false
                                                                    && p.PrescriptionType == true
                                                                    && p.ModifiedByPharmacy != null && p.ModifiedByPharmacy != 0
                                                                    && p.PharmacyId == null)
                                                                .Include(p => p.Doctor)
                                                                .Include(p => p.SentFromPharmacy)
                                                                .Include(pm => pm.PrescriptionMedicines)
                                                                .ThenInclude(m => m.Medicine)
                                                                .ThenInclude(d => d.Disease).ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
       


        /// <summary>
        /// Get All PatientPrescription List By Pharmacy Mobile Number
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="phamacymobilenumber"></param>
        /// <returns></returns>
        public async Task<List<Prescription>> GetAllPatientPrescriptionByPharmacyMobileNumber(string pharmacymobilenumber)
        {
            try
            {
                var prescription = await _oneMedifyDbContext.Prescriptions.Include(prescription => prescription.ApprovedByDoctor).Include(prescription => prescription.Patient)
                                                                          .Where(x => (x.Pharmacy.MobileNumber == pharmacymobilenumber || x.SentFromPharmacy.MobileNumber == pharmacymobilenumber) && x.IsDisable == false)
                                                                          .Include(prescription => prescription.PrescriptionUpload)
                                                                          .Include(prescription => prescription.PrescriptionMedicines)
                                                                          .ThenInclude(prescription => prescription.Medicine)
                                                                          .ThenInclude(prescription => prescription.Disease).ToListAsync();
                return prescription;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Get Prescription By Prescription Id
        /// </summary>
        /// <param name="prescriptionId"></param>
        /// <returns></returns>
        public async Task<PrescriptionUpload> GetPrescriptionByPrescriptionIdAsync(int prescriptionId)
        {
            try
            {
                var prescription = await _oneMedifyDbContext.PrescriptionUploads.Include(prescription => prescription.Prescription)
                                                                                .ThenInclude(x=>x.DoctorActionLogs)
                                                                                .Where(x => x.Prescription.PrescriptionId == prescriptionId && x.IsDisable == false).FirstOrDefaultAsync();
                return prescription;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Get List Of Prescription By DoctorMobileNumber
        /// </summary>
        /// <param name="doctorMobileNumber"></param>
        /// <returns></returns>
        public async Task<List<Prescription>> GetPrescriptionsCountAsync(string doctorMobileNumber)
        {
            try
            {
                var prescrptionCount = await _oneMedifyDbContext.Prescriptions.Include(x => x.Doctor).Where(x => x.Doctor.MobileNumber == doctorMobileNumber && x.CreatedDate > DateTime.Now.AddDays(-30) && x.IsDisable == false).ToListAsync();
                return prescrptionCount;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

        }

        /// <summary>
        /// Method For Checking Valid MobileNumber
        /// </summary>
        /// <param name="doctorMobileNumber"></param>
        /// <returns></returns>
        public bool IsMobileNumberValid(string doctorMobileNumber)
        {
            var doctor = _oneMedifyDbContext.Doctors.Where(doctor => doctor.MobileNumber == doctorMobileNumber && doctor.IsDisable == false).FirstOrDefault();
            if (doctor == null)
            {
                return true;
            }
            return false;
        }

        public Prescription UpdatePrescription(Prescription prescription)
        {
            try
            {
                _oneMedifyDbContext.Prescriptions.Update(prescription);
                _oneMedifyDbContext.SaveChanges();
                return prescription;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Get List Approval an Rejected Prescriptions Sent For Pharmacy By DoctorMobileNumber
        /// </summary>
        /// <param name="doctorMobileNumber"></param>
        /// <returns></returns>
        public async Task<List<Prescription>> GetPrescriptionsListSentForApprovalByPharmacyAsync(string doctorMobileNumber)
        {
            try
            {
                return await _oneMedifyDbContext.Prescriptions.Include(x => x.Patient)
                                                        .Include(x => x.Doctor)
                                                        .Include(x => x.Pharmacy)
                                                        .Where(x => x.Doctor.MobileNumber == doctorMobileNumber && x.Doctor.IsDisable == false
                                                        && x.IsDisable == false && x.Pharmacy.IsDisable == false
                                                        && x.PrescriptionStatus == 1 && x.PrescriptionStatus == 3 && x.ModifiedByPharmacy != null).ToListAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

        }

        /// <summary>
        /// Get Created Prescription by Prescription Id
        /// </summary>
        /// <param name="prescriptionId"></param>
        /// <returns></returns>
        public async Task<Prescription> GetCreatedPrescriptionByPrescriptionIdAsync(int prescriptionId)
        {
            try
            {
                return await _oneMedifyDbContext.Prescriptions.Include(prescription => prescription.Doctor)
                                                              .ThenInclude(doctor => doctor.City)
                                                              .ThenInclude(city => city.State)
                                                              .Where(prescription => prescription.Doctor.IsDisable == false && prescription.Doctor.City.IsDisable == false && prescription.Doctor.City.State.IsDisable == false)
                                                              .Include(prescription => prescription.Patient)
                                                              .Where(prescription => prescription.Patient.IsDisable == false)
                                                              .Include(prescription => prescription.PrescriptionMedicines)
                                                              .ThenInclude(prescriptionmedicine => prescriptionmedicine.Medicine)
                                                              .ThenInclude(medicine => medicine.Disease)
                                                              .Include(prescription => prescription.DoctorActionLogs)
                                                              .Where(prescription => prescription.PrescriptionId == prescriptionId && prescription.IsDisable == false && prescription.PrescriptionType == true)
                                                              .FirstOrDefaultAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// validation for Created Prescription Id
        /// </summary>
        public bool IsCreatedPrescriptionIdExist(int prescriptionId)
        {
            if (prescriptionId == 0)
            {
                return true;
            }
            var prescription = _oneMedifyDbContext.Prescriptions.Where(prescription => prescription.PrescriptionId == prescriptionId && prescription.IsDisable == false && prescription.PrescriptionType == true)
                                                                .Select(x => x.PrescriptionId).FirstOrDefault();
            if (prescription == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method For Checking If Uploaded Prescription Id Exist.
        /// </summary>
        /// <param name="prescriptionId"></param>
        /// <returns></returns>
        public bool IsUploadedPrescriptionIdExist(int prescriptionId)
        {
            try
            {
                if (prescriptionId == 0)
                {
                    return true;
                }
                var prescription = _oneMedifyDbContext.PrescriptionUploads.Where(prescription => prescription.Prescription.PrescriptionId == prescriptionId && 
                                                                                                 prescription.IsDisable == false && prescription.Prescription.PrescriptionType == false)
                                                                           .Select(x => x.PrescriptionId).FirstOrDefault();
                if (prescription == 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// GET: api/patient/createdPrescriptions/{patientMobileNumber}
        /// API to get all approved created prescription by  patient mobile number.
        /// </summary>
        public async Task<List<Prescription>> GetApprovedCreatedPrescriptionByPatientMobileNumberAsync(int pageIndex, string patientMobileNumber)
        {
            try
            {
                var createdPrescription = await _oneMedifyDbContext.Prescriptions.OrderByDescending(x => x.CreatedDate).Include(x => x.Doctor)
                                                                                .Include(x => x.PrescriptionMedicines).
                                                                                ThenInclude(x => x.Medicine).
                                                                                ThenInclude(x => x.Disease).
                                                                                Where(x => x.Patient.MobileNumber == patientMobileNumber && 
                                                                                      x.PrescriptionStatus == 1 && x.IsDisable == false && 
                                                                                      x.PrescriptionType == true).ToListAsync();
                return createdPrescription;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public bool IsValidPatientMobileNumber(string patientMobileNumber)
        {
            if (patientMobileNumber == null)
            {
                return true;
            }
            return false;
        }

        public bool MobileNumbereExists(string patientMobileNumber)
        {

            var patient = _oneMedifyDbContext.Patients.Where(patient => patient.MobileNumber == patientMobileNumber && patient.IsDisable == false).FirstOrDefault();
            if (patient == null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> SendForApprovalAsync(Prescription prescription)
        {
            try
            {
                _oneMedifyDbContext.Prescriptions.Update(prescription);
                await _oneMedifyDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Get All Doctor by mobile number
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public async Task<List<Doctor>> GetAllDoctorsAsync(int number)
        {
            try
            {
                return await _oneMedifyDbContext.Doctors.OrderByDescending(doctor => doctor.CreatedDate)
                                                        .Select(doctor => new Doctor()
                                                        {
                                                            DoctorId = doctor.DoctorId
                                                        }).Take(number).ToListAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Get Prescription by Prescription Id
        /// </summary>
        /// <param name="prescriptionId"></param>
        /// <returns></returns>

        public async Task<Prescription> GetPrescriptionsAsync(int prescriptionId)
        {
            try
            {
                var prescription = await _oneMedifyDbContext.Prescriptions.Include(prescription => prescription.Doctor)
                                                                          .Include(prescription => prescription.Patient)
                                                                          .FirstOrDefaultAsync(prescription => prescription.PrescriptionId == prescriptionId && prescription.IsDisable == false);
                return prescription;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Get Doctor Name by Doctor Id
        /// </summary>
        /// <param name="doctorId"></param>
        /// <returns></returns>
        public async Task<Doctor> GetDoctorNameAsync(int doctorId)
        {
            try
            {
                return await _oneMedifyDbContext.Doctors.Where(doctor => doctor.DoctorId == doctorId).FirstOrDefaultAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Get Patient Name by Patient Id
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        public async Task<string> GetPatientNameAsync(int patientId)
        {
            try
            {
                return await _oneMedifyDbContext.Patients.Where(patient => patient.PatientId == patientId).Select(patient => patient.Email).FirstOrDefaultAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Get Pharmacy Name by Pharmacy Id
        /// </summary>
        /// <param name="pharmacyId"></param>
        /// <returns></returns>
        public async Task<string> GetPharmacyNameAsync(int pharmacyId)
        {
            try
            {
                return await _oneMedifyDbContext.Pharmacies.Where(pharmacy => pharmacy.PharmacyId == pharmacyId).Select(pharmacy => pharmacy.PharmacyName).FirstOrDefaultAsync();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
        public async Task<Prescription> GetUploadedPrescriptionByPrescriptionIdAsync(int prescriptionId)
        {
            try
            {
                var prescription = await _oneMedifyDbContext.Prescriptions.Include(prescription => prescription.PrescriptionUpload)
                                                                          .Include(prescription => prescription.Patient)
                                                                                .Where(x => x.PrescriptionId == prescriptionId && x.PrescriptionType == false && x.IsDisable == false).FirstOrDefaultAsync();
                return prescription;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
