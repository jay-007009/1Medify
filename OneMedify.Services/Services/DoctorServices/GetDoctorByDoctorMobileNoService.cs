using OneMedify.DTO.Doctor;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Resources;
using OneMedify.Services.Contracts.DoctorContracts;
using OneMedify.Shared.Contracts;
using System;
using System.Threading.Tasks;

namespace OneMedify.Services.Services.DoctorServices
{
    public class GetDoctorByDoctorMobileNoService : IGetDoctorByDoctorMobileNoService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IFileService _fileService;

        public GetDoctorByDoctorMobileNoService(IDoctorRepository doctorRepository, IFileService fileService)
        {
            _doctorRepository = doctorRepository;
            _fileService = fileService;
        }

        public async Task<ResponseDto> GetDoctorByDoctorMobileNoAsync(string mobileNo)
        {
            try
            {
                if (_doctorRepository.IsValidMobileNumber(mobileNo))
                {
                    return new ResponseDto { StatusCode = 400, Response = DoctorResources.UnregisteredDoctorMobileNumber };
                }
                var doctor = await _doctorRepository.GetDoctorByDoctorMobileNumberAsync(mobileNo);
                    var doctorProfile = new DoctorProfileDto
                    {
                        MobileNumber = doctor.MobileNumber,
                        ProfilePicture = _fileService.GetFileFromLocation(doctor.ProfilePictureFilePath),
                        ProfilePictureName = doctor.ProfilePictureFileName,
                        Specialization = doctor.Specialization,
                        FirstName = doctor.FirstName,
                        LastName = doctor.LastName,
                        Email = doctor.Email,
                        Gender = doctor.Gender,
                        City = doctor.City.CityName,
                        State = doctor.City.State.StateName,
                        DateofEstablishment = doctor.InstituteEstablishmentDate.ToString("yyyy-MM-dd"),
                        InstituteName = doctor.InstituteName,
                        Address = doctor.Address,
                        CertificateOfInstitute = _fileService.GetFileFromLocation(doctor.InstituteCertificateFilePath),
                        CertificateOfInstituteName = doctor.InstituteCertificateFileName,
                        Degreecertificate= _fileService.GetFileFromLocation(doctor.DoctorDegreeFilePath),
                        DegreecertificateName = doctor.DoctorDegreeFileName
                    };
                return new ResponseDto { StatusCode = 200, Response = doctorProfile };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
