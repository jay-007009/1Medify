using OneMedify.DTO.Doctor;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Resources;
using OneMedify.Services.Contracts.DoctorContracts;
using OneMedify.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneMedify.Services.Services.DoctorServices
{
    public class GetDoctorsListService : IGetDoctorsListService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IFileService _fileService;

        public GetDoctorsListService(IDoctorRepository doctorRepository, IFileService fileService)
        {
            _doctorRepository = doctorRepository;
            _fileService = fileService;
        }

        /// <summary>
        /// Method To Get List Of All Registered Doctors
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public async Task<ResponseDto> GetAllDoctorsAsync(int pageIndex, string doctorName)
        {
            try
            {
                if (pageIndex < 0)
                {
                    return new ResponseDto { StatusCode = 400, Response = DoctorResources.InvalidPageIndex };
                }
                var doctors = await _doctorRepository.GetAllDoctorsAsync();
                var doctorList = doctors.Select(doctor => new DoctorDetailDto
                {
                    ProfilePicture = _fileService.GetFileFromLocation(doctor.ProfilePictureFilePath),
                    ProfilePictureName = doctor.ProfilePictureFileName,
                    doctorName = doctor.FirstName + " " + doctor.LastName,
                    MobileNumber = doctor.MobileNumber,
                    Specialization = doctor.Specialization,
                    InstituteName = doctor.InstituteName
                }).ToList();
                var filteredDoctors = DoctorsFilteredByName(doctorList, doctorName);
                return new ResponseDto { StatusCode = 200, Response = filteredDoctors.Skip(pageIndex * 10).Take(10) };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = DoctorResources.InternalServerError };
            }
        }

        private List<DoctorDetailDto> DoctorsFilteredByName(List<DoctorDetailDto> doctors, string doctorName)
        {
            List<DoctorDetailDto> filteredDoctors = new List<DoctorDetailDto>();
            if (doctorName != null)
            {
                foreach (var doctor in doctors)
                {
                    if (doctor.doctorName.Contains(doctorName, StringComparison.OrdinalIgnoreCase))
                    {
                        filteredDoctors.Add(doctor);
                    }
                }
            }
            else
            {
                filteredDoctors = doctors;
            }
            return filteredDoctors;
        }
    }
}
