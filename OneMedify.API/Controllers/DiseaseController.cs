using Microsoft.AspNetCore.Mvc;
using OneMedify.Services.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneMedify.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiseaseController : ControllerBase
    {
        private readonly IDieasesService _dieasesService;
        private readonly IMedicineService _medicineService;

        public DiseaseController(IDieasesService dieasesService, IMedicineService medicineService)
        {
            _dieasesService = dieasesService;
            _medicineService = medicineService;
        }

        [HttpGet]
        public IActionResult GetDiseases()
        {
            var result = _dieasesService.GetDiseases();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("medicine")]
        public async Task<IActionResult> GetMedicineByDiseasesIdsAsync([FromQuery] List<int> diseasesId)
        {
            var medicines = await _medicineService.GetMedicinesByDiseasesIdsAsync(diseasesId);
            return StatusCode(medicines.StatusCode, medicines);
        }

    }
}
