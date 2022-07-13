using Microsoft.AspNetCore.Mvc;
using OneMedify.DTO.Prescription;
using OneMedify.Services.Contracts;
using System.Threading.Tasks;

namespace OneMedify.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly IPrintCreatedPrescriptionService _printCreatedPrescriptionService;

        public PrescriptionController(IPrescriptionService prescriptionService, IPrintCreatedPrescriptionService printCreatedPrescriptionService)
        {
            _prescriptionService = prescriptionService;
            _printCreatedPrescriptionService = printCreatedPrescriptionService;
        }

        /// <summary>
        /// GET: /api/prescription/patientprescription/{patientMobileNumber}
        /// Get patient prescription list by pharmacy mobile number.
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pharmacymobilenumber"></param>
        /// <returns></returns>
        [HttpGet("prescriptionStatus/{pharmacyMobileNumber}")]
        public async Task<IActionResult> GetPatientPrescriptionByPharmacyMobileNumberAsync(int pageindex, string pharmacyMobileNumber)
        {
            var prescription = await _prescriptionService.GetAllPatientPrescriptionByPharmacyMobileNumberAsync(pageindex, pharmacyMobileNumber);
            return StatusCode(prescription.StatusCode, prescription);
        }

        /// <summary>
        /// GET: 
        /// Get uploaded prescription by prescription Id
        /// </summary>
        /// <param name="prescriptionid"></param>
        /// <returns></returns>
        [HttpGet("uploadedPrescriptions/{prescriptionId}")]
        public async Task<IActionResult> GetPrescriptionByPrescriptionIdAsync(int prescriptionId)
        {
            var prescription = await _prescriptionService.GetPrescriptionByPrescriptionIdAsync(prescriptionId);
            return StatusCode(prescription.StatusCode, prescription);
        }

        /// <summary>
        /// GET: /api/prescription/{patientMobileNumber}
        /// Get all patient's approved and pending prescription (created by doctor, uploaded by pharmacy, and uploaded by patient) by patient mobile number.
        /// </summary>
        [HttpGet("{patientMobileNumber}")]
        public async Task<IActionResult> GetPrescriptionsAsync([FromQuery(Name = "pageIndex")] int pageIndex, [FromRoute] string patientMobileNumber)
        {
            var result = await _prescriptionService.GetApprovedAndPendingPrescriptionsAsync(pageIndex, patientMobileNumber);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// GET: /api/prescription/createdPrescription/{prescriptionId}
        /// Get Created Prescription by Prescription Id
        /// </summary>
        [HttpGet("createdPrescription/{prescriptionId}")]
        public async Task<IActionResult> GetCreatedPrescriptionByPrescriptionIdAsync(int prescriptionId)
        {
            var result = await _prescriptionService.GetCreatedPrescriptionByPrescriptionIdAsync(prescriptionId);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// PUT: api/prescription/sendForApproval/{prescriptionId}
        /// Send Prescription for Approval by Prescription Id 
        /// </summary>
        /// <param name="prescriptionId"></param>
        /// <returns></returns>

        [HttpPut("sendForApproval/{prescriptionId}")]
        public async Task<IActionResult> SendForApprovalAsync(int prescriptionId, [FromBody] SendForApprovalDto sendForApprovalDto)
        {
            var Updated = await _prescriptionService.SendForApprovalAsync(prescriptionId, sendForApprovalDto);
            return StatusCode(Updated.StatusCode, Updated);
        }

        /// <summary>
        /// GET: api/prescription/print/{prescriptionId}
        /// Print Method of Get Created Prescription by Prescription Id 
        /// </summary>
        /// <param name="prescriptionId"></param>
        /// <returns></returns>
        [HttpGet("print/{prescriptionId}")]
        public async Task<IActionResult> GetCreatedPrescriptionByPrescriptionIdPrintAsync(int prescriptionId)
        {
            var result = await _printCreatedPrescriptionService.PrintPrescriptionAsync(prescriptionId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
