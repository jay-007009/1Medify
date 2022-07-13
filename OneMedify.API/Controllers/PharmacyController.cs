using Microsoft.AspNetCore.Mvc;
using OneMedify.DTO.Pharmacy;
using OneMedify.DTO.Prescription;
using OneMedify.Services.Contracts;
using System.Threading.Tasks;

namespace OneMedify.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PharmacyController : ControllerBase
    {
        private readonly IPharmacyService _pharmacyService;
        private readonly IPrescriptionService _prescriptionService;

        public PharmacyController(IPharmacyService pharmacyService, IPrescriptionService prescriptionService)
        {
            _pharmacyService = pharmacyService;
            _prescriptionService = prescriptionService;
        }

        /// <summary>
        /// A Post Method for Registering Pharmacy 
        /// </summary>
        /// <param name="pharmacySignUp"></param>
        /// <returns></returns>
        [HttpPost("Signup")]
        public async Task<IActionResult> Signup([FromBody] PharmacySignUpDto pharmacySignUp)
        {
            var registration = await _pharmacyService.PharmacyRegistrationAsync(pharmacySignUp);
            return StatusCode(registration.StatusCode, registration);
        }

        /// <summary>
        /// Author: Ketan Singh
        /// Method for Updating Pharmacy's Profile
        /// </summary>
        /// <param name="pharmacyMobileNumber"></param>
        /// <param name="pharmacyUpdateDto"></param>
        /// <returns></returns>
        [HttpPut("profile/{pharmacyMobileNumber}")]
        public async Task<IActionResult> UpdateProfileAsync(string pharmacyMobileNumber, [FromBody] PharmacyUpdateDto pharmacyUpdateDto)
        {
            var pharmacyUpdated = await _pharmacyService.UpdatePharmacyAsync(pharmacyMobileNumber, pharmacyUpdateDto);
            return StatusCode(pharmacyUpdated.StatusCode, pharmacyUpdated);
        }

        /// <summary>
        /// Get Method for Pharmacy By Mobile Number
        /// </summary>
        /// <param name="mobilenumber"></param>
        /// <returns></returns>
        [HttpGet("{mobilenumber}")]
        public async Task<IActionResult> GetPharmacy(string mobilenumber)
        {
            var pharmacy = await _pharmacyService.GetPharmacyByMobileNumberAsync(mobilenumber);
            return StatusCode(pharmacy.StatusCode, pharmacy);
        }

        [HttpGet]
        public async Task<IActionResult> GetPharmacyList([FromQuery] int pageIndex, [FromQuery] string pharmacyName)
        {
            var pharmacies = await _pharmacyService.GetAllPharmacies(pageIndex, pharmacyName);
            return StatusCode(pharmacies.StatusCode, pharmacies);
        }

        /// <summary>
        /// Method for Getting Patient's Uploaded Prescription by Pharmacy Mobile Number
        /// </summary>
        /// <param name="pharmacyMobileNumber"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet("UploadedPrescription/{pharmacyMobileNumber}")]
        public async Task<IActionResult> GetPharmacyUploadedPrescription_ByPharmacyMobileNumberAsync(string pharmacyMobileNumber, [FromQuery] int pageIndex)
        {
            var uploadedPrescription = await _pharmacyService.GetPharmacyUploadedPrescriptionAsync(pharmacyMobileNumber, pageIndex);
            return StatusCode(uploadedPrescription.StatusCode, uploadedPrescription);
        }


        [HttpPost("UploadPrescription")]
        public async Task<IActionResult> UploadPrescription([FromBody] UploadPatientPrescriptionDto uploadPatientPrescriptionDto)
        {
            var result = await _prescriptionService.UploadPrescriptionByPharmacy(uploadPatientPrescriptionDto);
            return StatusCode(result.StatusCode, result);
        }
    }
}