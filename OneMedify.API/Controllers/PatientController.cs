using Microsoft.AspNetCore.Mvc;
using OneMedify.DTO.Patient;
using OneMedify.DTO.Response;
using OneMedify.Resources;
using OneMedify.Services.Contracts;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OneMedify.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IPrescriptionService _prescriptionService;
        public PatientController(IPatientService patientService, IPrescriptionService prescriptionService)
        {
            _patientService = patientService;
            _prescriptionService = prescriptionService;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUpAsync([FromBody] PatientSignUpDto patient)
        {
            if (Convert.ToDateTime(patient.DateOfBirth) > DateTime.Now || Convert.ToDateTime(patient.DateOfBirth) < DateTime.Now.AddYears(-150))
            {
                return StatusCode(400, new ResponseDto { StatusCode = 400, Response =  PatientResources.InvalidDate });
            }

            var result = await _patientService.PatientRegistration(patient);
            return StatusCode(result.StatusCode, result);
        }


        [HttpGet("{mobilenumber}")]
        public async Task<IActionResult> GetAsync(string mobilenumber)
        {
            var result = await _patientService.GetPatientByMobileNo(mobilenumber);
            return StatusCode(result.StatusCode, result);
        }


        /// <summary>
        /// Patient Get API for Patient's Prescriptions By Patient Mobile Number
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        [HttpGet("prescription/{mobileNumber}")]
        public async Task<IActionResult> GetPatientPrescriptionByMobileNumberAsync(string mobileNumber)
        {
            var prescription = await _patientService.GetPatientPrescriptionByMobileNo(mobileNumber);
            return StatusCode(prescription.StatusCode, prescription);
        }


        /// <summary>
        /// Get List of Prescriptions Status Send For Approval by Patient Mobile Number
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="patientMobileNumber"></param>
        /// <returns></returns>
        [HttpGet("prescriptionsStatusSentForApproval/{patientMobileNumber}")]
        public async Task<IActionResult> GetPrescriptionStatusByPatientMobileNumberAsync([FromQuery] int pageIndex, string patientMobileNumber)
        {
            var prescriptionStatus = await _patientService.GetPrescriptionStatusByPatientMobileNumberAsync(pageIndex, patientMobileNumber);
            return StatusCode(prescriptionStatus.StatusCode, prescriptionStatus);
        }


        /// <summary>
        /// Get List of Uploaded Prescriptions By Pharmacy by Patient Mobile Number
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="patientMobileNumber"></param>
        /// <returns></returns>
        [HttpGet("uploadedPrescriptionsByPharmacy/{patientMobileNumber}")]
        public async Task<IActionResult> UploadedPrescriptionStatusByPatientMobileNumberAsync([FromQuery] int pageIndex, string patientMobileNumber)
        {
            var uploadedPrescription = await _patientService.UploadedPrescriptionByPharmacyPatientMobileNumberAsync(pageIndex, patientMobileNumber);
            return StatusCode(uploadedPrescription.StatusCode, uploadedPrescription);
        }


        /// <summary>
        /// Method for Getting Patient Profile Details by Patient MobileNumber
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        [HttpGet("patientProfile/{mobileNumber}")]
        public async Task<IActionResult> GetPatientProfileByPatientMobileNumberAsync(string mobileNumber)
        {
            var patientProfile = await _patientService.GetPatientProfileAsync(mobileNumber);
            return StatusCode(patientProfile.StatusCode, patientProfile);
        }


        /// <summary>
        /// Method for Getting Patient's Uploaded Prescription by Mobile Number
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet("myUploadedPrescriptions/{mobileNumber}")]
        public async Task<IActionResult> GetPatients_UploadedPrescription_ByPatientMobileNumberAsync(string mobileNumber, [FromQuery] int pageIndex)
        {
            var uploadedPrescription = await _patientService.GetPatientUploadedPrescriptionAsync(mobileNumber, pageIndex);
            return StatusCode(uploadedPrescription.StatusCode, uploadedPrescription);
        }


        /// <summary>
        /// Method for Updating Doctor's Profile
        /// </summary>
        /// <param name="patientMobileNumber"></param>
        /// <param name="patientUpdateDto"></param>
        /// <returns></returns>
        [HttpPut("profile/{PatientMobileNumber}")]
        public async Task<IActionResult> UpdateProfileAsync(string patientMobileNumber, [FromBody] PatientUpdateDto patientUpdateDto)
        {
            var patientUpdated = await _patientService.UpdatePatientAsync(patientMobileNumber, patientUpdateDto);
            return StatusCode(patientUpdated.StatusCode, patientUpdated);
        }

        /// <summary>
        /// GET: api/patient/createdPrescriptions/sentForApprovalByPharmacy/{patientMobileNumber}
        /// API to get all created prescription sended for approval by pharmacy using patient mobile number.
        /// </summary>
        [HttpGet("createdPrescriptions/sentForApprovalByPharmacy/{patientMobileNumber}")]
        public async Task<IActionResult> GetCreatedPrescriptionSentForApprovalByPharmacy([FromRoute] string patientMobileNumber, [FromQuery(Name = "pageIndex")] int pageIndex)
        {
            try
            {
                if (pageIndex < 0)
                {
                    return StatusCode(400, new ResponseDto { StatusCode = 400, Response = PrescriptionResource.InvalidPageIndex });
                }

                //Validate mobile number format.
                if (!Regex.IsMatch(patientMobileNumber, @"^[0-9]{10}$"))
                {
                    return StatusCode(400, new ResponseDto { StatusCode = 400, Response = PrescriptionResource.InvalidMobileFormat });
                }

                var result = await _prescriptionService.GetPatientCreatedPrescriptionsSentForApprovalByPharmacy(pageIndex, patientMobileNumber);

                return StatusCode(result.StatusCode, result);
            }
            catch
            {
                return StatusCode(500, new ResponseDto { StatusCode = 500, Response = PrescriptionResource.InternalServerError });
            }
        }


        /// <summary>
        /// Method for getting patients created prescription count by mobile number
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        [HttpGet("createdPrescriptionsCount/{patientMobileNumber}")]
        public async Task<IActionResult> GetPatientsCreatedPrescriptionCountAsync(string patientMobileNumber)
        {
            var createdPrescriptionCount = await _patientService.GetPatientsCreatedPrescriptionCountAsync(patientMobileNumber);
            return StatusCode(createdPrescriptionCount.StatusCode, createdPrescriptionCount);
        }

        [HttpPost("UploadPrescription")]
        public async Task<IActionResult> UploadPrescription([FromBody] UploadPrescriptionDto uploadPrescriptionDto)
        {
            var result = await _prescriptionService.UploadPrescriptionByPatient(uploadPrescriptionDto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// GET: api/patient/createdPrescriptions/{patientMobileNumber}
        /// API to get all approved created prescription by  patient mobile number.
        /// </summary>
        [HttpGet("createdPrescriptions/{patientMobileNumber}")]
        public async Task<IActionResult> GetApprovedCreatedPrescriptionByPatientMobileNumberAsync([FromQuery] int pageIndex, string patientMobileNumber)
        {
            var result = await _prescriptionService.GetApprovedCreatedPrescriptionByPatientMobileNumberAsync(pageIndex, patientMobileNumber);
            return StatusCode(result.StatusCode, result);
        }
    }
}