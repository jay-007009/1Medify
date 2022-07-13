using OneMedify.DTO.Pharmacy;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Resources;
using OneMedify.Services.Contracts;
using OneMedify.Services.Contracts.PharmacyContracts;
using OneMedify.Shared.Contracts;
using OneMedify.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneMedify.Services.Services
{
    public class PharmacyService : IPharmacyService
    {
        private readonly IPharmacyUpdateService _pharmacyUpdateService;
        private readonly IPharmacyRepository _pharmacyRepository;
        private readonly IUserValidations _userValidations;
        private readonly IFileService _fileService;
        private readonly IPharmacyRegistrationService _pharmacyRegistrationService;
        private readonly IGetUploadedPrescriptionsByPharmacyMobileNumber _getUploadedPrescriptionsByPharmacyMobileNumber;

        public PharmacyService(IPharmacyUpdateService pharmacyUpdateService,
                               IPharmacyRepository pharmacyRepository,
                               IUserValidations userValidations,
                               IFileService fileService,
                               IPharmacyRegistrationService pharmacyRegistrationService,
                               IGetUploadedPrescriptionsByPharmacyMobileNumber getUploadedPrescriptionsByPharmacyMobileNumber)
        {
            _pharmacyUpdateService = pharmacyUpdateService;
            _pharmacyRepository = pharmacyRepository;
            _userValidations = userValidations;
            _fileService = fileService;
            _pharmacyRegistrationService = pharmacyRegistrationService;
            _getUploadedPrescriptionsByPharmacyMobileNumber = getUploadedPrescriptionsByPharmacyMobileNumber;
        }



        /// <summary>
        /// Async Method for Pharmacy Registration 
        /// </summary>
        /// <param name="pharmacySignUp"></param>
        /// <returns></returns>
        public async Task<ResponseDto> PharmacyRegistrationAsync(PharmacySignUpDto pharmacySignUpDto)
        {
            try
            {
                return await _pharmacyRegistrationService.PharmacyRegistrationAsync(pharmacySignUpDto);
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PharmacyResources.PharmacyRegistrationFailed };
            }
        }


        /// <summary>
        /// Method for Updating Pharmacy Profile
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <param name="pharmacyUpdateDto"></param>
        /// <returns></returns>
        public async Task<ResponseDto> UpdatePharmacyAsync(string mobileNumber, PharmacyUpdateDto pharmacyUpdateDto)
        {
            try
            {
                return await _pharmacyUpdateService.UpdatePharmacyAsync(mobileNumber, pharmacyUpdateDto);
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PharmacyResources.PharmacyUpdateFailed };
            }
        }


        /// <summary>
        /// Async Method for Getting Pharmacy by MobileNumber
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        public async Task<ResponseDto> GetPharmacyByMobileNumberAsync(string mobileNumber)
        {
            try
            {
                if (_userValidations.IsMobileNumberValid(mobileNumber))
                {
                    return new ResponseDto { StatusCode = 404, Response = PharmacyResources.UnregisteredPharmacyMobileNumber };
                }
                var pharmacy = await _pharmacyRepository.GetPharmacyByMobileNumberAsync(mobileNumber);
                PharmacyDto pharmacyDto = new PharmacyDto
                {
                    ProfilePicture = _fileService.GetFileFromLocation(pharmacy.ProfilePictureFilePath),
                    ProfilePictureName = pharmacy.ProfilePictureFileName,
                    Name = pharmacy.PharmacyName,
                    MobileNumber = pharmacy.MobileNumber,
                    EstablishmentDate = pharmacy.PharmacyEstablishmentDate.ToString(),
                    Email = pharmacy.Email,
                    Address = pharmacy.Address,
                    City = pharmacy.City.CityName,
                    State = pharmacy.City.State.StateName,
                    Certificate = _fileService.GetFileFromLocation(pharmacy.PharmacyCertificateFilePath),
                    CertificateName = pharmacy.PharmacyCertificateFileName,
                    PharmacistDegreeCertificate = _fileService.GetFileFromLocation(pharmacy.PharmacistDegreeFilePath),
                    PharmacistDegreeCertificateName = pharmacy.PharmacistDegreeFileName
                };
                return new ResponseDto
                {
                    StatusCode = 200,
                    Response = pharmacyDto
                };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = StatusCodeResource.InternalServerResponse };
            }
        }
        public async Task<ResponseDto> GetAllPharmacies(int pageIndex, string pharmacyName)
        {
            try
            {
                if (pageIndex < 0)
                {
                    return new ResponseDto { StatusCode = 400, Response = DoctorResources.InvalidPageIndex };
                }
                List<PharmaciesDto> filterpharmaciesDto = new List<PharmaciesDto>();
                List<PharmaciesDto> pharmacyList = new List<PharmaciesDto>();
                var pharmacies = await _pharmacyRepository.GetAllPharmacy(pageIndex);
                if (pharmacies.Count > 0)
                {
                    foreach (var pharmacyItem in pharmacies)
                    {
                        PharmaciesDto pharmacy = new PharmaciesDto
                        {
                            Name = pharmacyItem.PharmacyName,
                            ProfilePicture = _fileService.GetFileFromLocation(pharmacyItem.ProfilePictureFilePath),
                            ProfilePictureName = pharmacyItem.ProfilePictureFileName,
                            City = pharmacyItem.City.CityName,
                            MobileNumber = pharmacyItem.MobileNumber,
                            State = pharmacyItem.City.State.StateName
                        };
                        pharmacyList.Add(pharmacy);
                    }
                    if (pharmacyName != null)
                    {
                        foreach (var pharmacy in pharmacyList)
                        {
                            if ((pharmacy.Name).Contains(pharmacyName, StringComparison.OrdinalIgnoreCase))
                            {
                                filterpharmaciesDto.Add(pharmacy);
                            }
                        }
                    }
                    else
                    {
                        filterpharmaciesDto = pharmacyList;
                    }
                }
                if (Math.Ceiling((decimal)filterpharmaciesDto.Count / 10) - 1 < pageIndex)
                {
                    return new ResponseDto { StatusCode = 200, Response = new List<PharmaciesDto>() };
                }
                return new ResponseDto { StatusCode = 200, Response = filterpharmaciesDto.GetRange(pageIndex * 10, filterpharmaciesDto.Count < (pageIndex * 10) + 10 ? filterpharmaciesDto.Count % 10 : 10) };
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = StatusCodeResource.InternalServerResponse };
            }
        }

        /// <summary>
        /// Author: Ketan Singh
        /// Method for Getting Pharmacy's Uploaded Prescription 
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public async Task<ResponseDto> GetPharmacyUploadedPrescriptionAsync(string mobileNumber, int pageIndex)
        {
            try
            {
                return await _getUploadedPrescriptionsByPharmacyMobileNumber.GetUploadedPrescriptionByPharmacyMobileNumberAsync(mobileNumber, pageIndex);
            }
            catch
            {
                return new ResponseDto { StatusCode = 500, Response = PharmacyResources.InternalServerResponse };
            }
        }
    }
}
