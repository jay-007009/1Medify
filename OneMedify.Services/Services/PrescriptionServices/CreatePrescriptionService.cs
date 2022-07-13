using OneMedify.DTO.Prescription;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Entities;
using OneMedify.Resources;
using OneMedify.Services.Contracts;
using OneMedify.Services.Contracts.PrescriptionContracts;
using OneMedify.Shared.Contracts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OneMedify.Services.Services.PrescriptionServices
{
    public class CreatePrescriptionService : ICreatePrescriptionService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IMedicineRepository _medicineRepository;
        private readonly IDiseaseRepository _diseaseRepository;
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IPrescriptionMedicineRepository _prescriptionMedicineRepository;
        private readonly IPatientService _patientService;
        private readonly IEmailService _emailService;

        public CreatePrescriptionService(IDoctorRepository doctorRepository, IPatientRepository patientRepository,
                                         IMedicineRepository medicineRepository, IDiseaseRepository diseaseRepository,
                                         IPrescriptionRepository prescriptionRepository, IPrescriptionMedicineRepository prescriptionMedicineRepository,
                                         IPatientService patientService, IEmailService emailService)
        {
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _medicineRepository = medicineRepository;
            _diseaseRepository = diseaseRepository;
            _prescriptionRepository = prescriptionRepository;
            _prescriptionMedicineRepository = prescriptionMedicineRepository;
            _patientService = patientService;
            _emailService = emailService;
        }


        /// <summary>
        /// Method to create patient's prescription with all validation check calls.
        /// </summary>
        public async Task<ResponseDto> CreatePrescription(PrescriptionCreateDto prescriptionDto)
        {
            try
            {
                //Check doctor with given mobile number exist or not. and get doctor details
                var doctor = await _doctorRepository.ReadDoctorByMobileNumber(prescriptionDto.DoctorMobileNumber);
                if (doctor == null)
                {
                    return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.DoctorNotExistByMobile };
                }

                //Check patient with given mobile number exist or not. and get patient details.
                var patient = await _patientRepository.ReadPatientByMobileNumber(prescriptionDto.PatientMobileNumber);
                if (patient == null)
                {
                    return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.PatientNotExistByMobile };
                }

                //If disease and medicine are not valid then return.
                var PatientDetailsresponse = await CheckPrescriptionFields(prescriptionDto);
                if (PatientDetailsresponse.StatusCode != 200)
                {
                    return PatientDetailsresponse;
                }

                //Create prescription 
                Prescription prescription = new Prescription
                {
                    DoctorId = doctor.DoctorId,
                    PatientId = patient.PatientId,
                    ExpiryDate = Convert.ToDateTime(prescriptionDto.ExpiryDate + " " + DateTime.Now.ToString("HH:mm:ss")),
                    CreatedBy = doctor.DoctorId,
                    PrescriptionStatus = 1,
                    CreatedDate = DateTime.Now,
                    PrescriptionType = true
                };
                var prescriptionResponse = await _prescriptionRepository.Create(prescription);

                //If prescription is created successfully. then add prescription medicine.
                if (prescriptionResponse.PrescriptionId != 0)
                {
                    //If patient's new disease and disease to remove operation is failed return.
                    var patientDiseaseResponse = await AddAndRemovePatientDisease(prescriptionDto, patient, doctor);
                    if (patientDiseaseResponse.StatusCode != 200)
                    {
                        return patientDiseaseResponse;
                    }

                    var PrescriptionMedicineResponse = await PescriptionMedicineCreation((int)prescriptionResponse.DoctorId,
                                                                                   prescriptionResponse.PrescriptionId,
                                                                                   prescriptionDto);
                    if (PrescriptionMedicineResponse.StatusCode != 200)
                    {
                        return PrescriptionMedicineResponse;
                    }

                    //Send email notification to patient that new prescription is created by doctor.
                    var emailResponse = await _emailService.SendEmailAsync(patient.Email,
                    "Prescription Created", "New prescription has been Created");

                    return new ResponseDto { StatusCode = 200, Response = PrescriptionResource.CreatedSuccessfully };
                }
                return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.CreationFailed };

            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PrescriptionResource.CreationFailed };
            }
        }


        /// <summary>
        /// Method to check diseases and medicines passed in request are valid.
        /// </summary>
        private async Task<ResponseDto> CheckPrescriptionFields(PrescriptionCreateDto prescriptionDto)
        {
            try
            {

                foreach (var diseaseId in prescriptionDto.DiseaseIds)
                {
                    //Check given disease with disease id exist or not.
                    var existingDisease = await _diseaseRepository.ReadById(diseaseId);
                    if (existingDisease == null)
                    {
                        return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.DiseaseNotExistById };
                    }
                }

                //Check all passed disease has medicine
                foreach (var diseaseId in prescriptionDto.DiseaseIds)
                {
                    var medicines = await _medicineRepository.GetMedicineByDiseaseId(diseaseId);

                    var diseaseMedicineList = medicines.Select(x => x.MedicineId);
                    var passedMedicineList = prescriptionDto.Medicines.Select(x => x.MedicineId);

                    var checkMedicines = diseaseMedicineList.Except(passedMedicineList);
                    if (checkMedicines.Count() == diseaseMedicineList.Count())
                    {
                        return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.DiseaseNotHaveMedicine };
                    }
                }

                foreach (var medicine in prescriptionDto.Medicines)
                {
                    bool isBelonged = false;
                    Medicine existingMedicine;
                    foreach (var diseaseId in prescriptionDto.DiseaseIds)
                    {
                        //Check given medicine belongs to disease in prescription.
                        existingMedicine = await _medicineRepository.GetMedicineByDiseaseId(medicine.MedicineId, diseaseId);
                        if (existingMedicine != null)
                        {
                            isBelonged = true;
                        }
                    }
                    if (!isBelonged)
                    {
                        return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.MedicineNotBelongsToDisease };
                    }
                }
                return new ResponseDto { StatusCode = 200 };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PrescriptionResource.CreationFailed };
            }
        }


        /// <summary>
        /// Method to add patient's new disease and to remove disease which are not prescribed by doctor.
        /// </summary>
        private async Task<ResponseDto> AddAndRemovePatientDisease(PrescriptionCreateDto prescriptionDto, Patient patient, Doctor doctor)
        {
            try
            {
                //Update patient's diseases as provided by doctor.
                var patientDiseaseList = patient.PatientDisease.Where(x => x.IsDisable == false).Select(x => x.DiseaseId).ToList();

                var diseaseToAddIds = prescriptionDto.DiseaseIds.Except(patientDiseaseList).ToList();

                var diseaseToRemoveIds = patientDiseaseList.Except(prescriptionDto.DiseaseIds).ToList();

                //New patient disease's added
                foreach (var diseaseId in diseaseToAddIds)
                {
                    await _patientService.AddPatientDisease(patient.PatientId, diseaseId, doctor.DoctorId);
                }

                //Not prescribed patient's disease removed
                foreach (var diseaseId in diseaseToRemoveIds)
                {
                    await _patientService.RemovePatientDisease(patient.PatientId, diseaseId, doctor.DoctorId);
                }
                return new ResponseDto { StatusCode = 200 };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PrescriptionResource.CreationFailed };
            }
        }


        /// <summary>
        /// Method to create prescription's medicines.
        /// </summary>
        private async Task<ResponseDto> PescriptionMedicineCreation(int doctorId, int prescriptionId, PrescriptionCreateDto prescriptionDto)
        {
            try
            {
                foreach (var medicine in prescriptionDto.Medicines)
                {
                    //Create prescription's medicine.
                    PrescriptionMedicine prescriptionMedicine = new PrescriptionMedicine
                    {
                        PrescriptionId = prescriptionId,
                        MedicineId = medicine.MedicineId,
                        AfterBeforeMeal = (bool)medicine.AfterBeforeMeal,
                        MedicineDays = medicine.MedicineDays,
                        MedicineDosage = medicine.MedicineDosage,
                        MedicineTiming = String.Join(",", medicine.MedicineTiming),
                        CreatedBy = doctorId,
                        CreatedDate = DateTime.Now
                    };
                    await _prescriptionMedicineRepository.Create(prescriptionMedicine);
                }

                return new ResponseDto { StatusCode = 200 };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PrescriptionResource.CreationFailed };
            }
        }       

    }
}
