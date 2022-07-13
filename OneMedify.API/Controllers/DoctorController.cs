using Microsoft.AspNetCore.Mvc;
using OneMedify.DTO.Doctor;
using OneMedify.DTO.Prescription;
using OneMedify.DTO.Response;
using OneMedify.Resources;
using OneMedify.Services.Contracts;
using OneMedify.Shared.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneMedify.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _patientService;
        private readonly IPrescriptionService _prescriptionService;
        private readonly IDoctorActionLogService _doctorActionLogService;
        private readonly PrescriptionValidator _prescriptionValidator;

        public DoctorController(IDoctorService doctorService, IPatientService patientService, IPrescriptionService prescriptionService, IDoctorActionLogService doctorActionLogService)
        {
            _doctorService = doctorService;
            _patientService = patientService;
            _prescriptionService = prescriptionService;
            _doctorActionLogService = doctorActionLogService;
            _prescriptionValidator = new PrescriptionValidator();
        }

        /// <summary>
        /// Post Method for Registering Doctor
        /// </summary>
        /// <param name="doctorSignUp"></param>
        /// <returns></returns>
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] DoctorSignUpDto doctorSignUp)
        {
            var registration = await _doctorService.DoctorRegistrationAsync(doctorSignUp);
            return StatusCode(registration.StatusCode, registration);
        }

        /// <summary>
        /// Method for Updating Doctor's Profile
        /// </summary>
        /// <param name="doctorMobileNumber"></param>
        /// <param name="doctorUpdateDto"></param>
        /// <returns></returns>
        [HttpPut("profile/{doctorMobileNumber}")]
        public async Task<IActionResult> UpdateProfileAsync(string doctorMobileNumber, [FromBody] DoctorUpdateDto doctorUpdateDto)
        {
            var doctorUpdated = await _doctorService.UpdateDoctorAsync(doctorMobileNumber, doctorUpdateDto);
            return StatusCode(doctorUpdated.StatusCode, doctorUpdated);
        }

        /// <summary>
        /// Get Method for fetching list of patients by Doctor's mobile number
        /// </summary>
        /// <param name="doctorMobileNo"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet("myPatients/{doctorMobileNo}")]
        public async Task<IActionResult> MyPatients(string doctorMobileNo, [FromQuery] int pageIndex, [FromQuery] string patientName)
        {
            var patients = await _patientService.GetPatientsByDoctorMobileAsync(doctorMobileNo, pageIndex, patientName);
            return StatusCode(patients.StatusCode, patients);
        }

        /// <summary>
        /// POST : /api/doctor/createprescription
        /// Patient's prescription creation by doctor.
        /// </summary>
        [HttpPost("CreatePrescription")]
        public async Task<IActionResult> CreatePrescription([FromBody] PrescriptionCreateDto prescription)
        {
            try
            {
                List<string> errors = new List<string>();
                var prescriptionValidationResult = _prescriptionValidator.Validate(prescription);
                if (!prescriptionValidationResult.IsValid)
                {
                    foreach (var failures in prescriptionValidationResult.Errors)
                    {
                        errors.Add($"{failures.PropertyName}: {failures.ErrorMessage}");
                    }
                    return BadRequest(new ResponseDto { StatusCode = 400, Response = errors });
                }

                var result = await _prescriptionService.CreatePrescriptionAsync(prescription);
                return StatusCode(result.StatusCode, result);
            }
            catch
            {
                return StatusCode(500, new ResponseDto { StatusCode = 500, Response = PrescriptionResource.CreationFailed });
            }
        }

        /// <summary>
        /// Get Last 30 Days PrescriptionCountList By DoctorMobileNumber
        /// </summary>
        /// <param name="doctorMobileNumber"></param>
        /// <returns></returns>
        [HttpGet("prescriptionsCount/{doctorMobileNumber}")]
        public async Task<IActionResult> GetPrescriptionCount(string doctorMobileNumber)
        {
            var prescriptionCount = await _prescriptionService.GetPrescriptionsCountAsync(doctorMobileNumber);
            return StatusCode(prescriptionCount.StatusCode, prescriptionCount);
        }

        /// <summary>
        /// Author: Ketan Singh
        /// To Retrieve Doctor By Doctor MobileNo
        /// </summary>
        /// <param name="doctormobileNo"></param>
        /// <returns></returns>
        [HttpGet("{doctormobileNo}")]
        public async Task<IActionResult> GetDoctorByMobileNo(string doctormobileNo)
        {
            var doctormobileno = await _doctorService.GetDoctorByDoctorMobileNoAsync(doctormobileNo);
            return StatusCode(doctormobileno.StatusCode, doctormobileno);
        }

        /// <summary>
        /// Method to Get Pharmacies and Patient's Count
        /// </summary>
        /// <param name="doctorMobileNumber"></param>
        /// <returns></returns>
        [HttpGet("pharmicesPatientCount/{doctorMobileNumber}")]
        public async Task<IActionResult> GetPharmicesPatientCount(string doctorMobileNumber)
        {
            var mobile = await _doctorService.GetPatientsPharmaciesCountAsync(doctorMobileNumber);
            return StatusCode(mobile.StatusCode, mobile);
        }

        /// <summary>
        /// Method to Get List of All Registered Doctors
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="doctorName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetDoctorsList([FromQuery] int pageIndex, string doctorName)
        {
            var doctors = await _doctorService.GetAllDoctorsAsync(pageIndex, doctorName);
            return StatusCode(doctors.StatusCode, doctors);
        }

        /// <summary>
        /// Method to Get Count of All Registered Doctors
        /// </summary>
        /// <returns></returns>
        [HttpGet("doctorsCount")]
        public async Task<IActionResult> GetDoctorsCount()
        {
            var doctorsCount = await _doctorService.GetDoctorsCountAsync();
            return StatusCode(doctorsCount.StatusCode, doctorsCount);
        }

        /// <summary>
        /// Method to Get List of Prescriptions Sent For Approval By Patient by Doctor's Mobile Number
        /// </summary>
        /// <param name="doctorMobile"></param>
        /// <returns></returns>
        [HttpGet("patient/viewPrescription/{doctorMobile}")]
        public async Task<IActionResult> GetPrescriptionsSentForApprovalbyPatient(string doctorMobile)
        {
            var patientPresciptions = await _doctorActionLogService.GetPrescriptionsSentForApprovalByPatientByDoctorMobile(doctorMobile);
            return StatusCode(patientPresciptions.StatusCode, patientPresciptions);
        }

        /// <summary>
        /// Get PrescriptionList Sent For Approval By Pharmacy 
        /// </summary>
        /// <param name="doctorMobileNumber"></param>
        /// <returns></returns>
        [HttpGet("pharmacy/viewPrescription/{doctorMobileNumber}")]
        public async Task<IActionResult> GetPrescriptionListSentForApprovalByPharmacy(string doctorMobileNumber)
        {
            var prescriptionList = await _doctorActionLogService.GetPrescriptionListSentForApprovalByPharmacyAsync(doctorMobileNumber);
            return StatusCode(prescriptionList.StatusCode, prescriptionList);
        }

        [HttpPut("action/{prescriptionId}")]
        public async Task<IActionResult> UpdateDoctorAction(int prescriptionId, [FromBody] DoctorActionDto doctorActionDto)
        {
            var actionUpdate = await _prescriptionService.UpdatePrescriptionStatus(doctorActionDto, prescriptionId);
            return StatusCode(actionUpdate.StatusCode, actionUpdate);
        }
    }
}