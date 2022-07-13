using OneMedify.DTO.Prescription;
using OneMedify.DTO.Print;
using OneMedify.DTO.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneMedify.UnitTests.OneMedify.API.UnitTests.MockData
{
    public class MockPrescriptionControllerData
    {
        public ResponseDto ToGetPatientPrescriptionSuccessfullResponse()
        {
            return new ResponseDto
            {
                Response = new List<DTO.Pharmacy.PharmacyPrescriptionDto>()
                {
                    new DTO.Pharmacy.PharmacyPrescriptionDto
                    {

                        PrescriptionId = 469,
                        ProfilePicture = null,
                        ProfilePictureName = "DefaultProfileImage",
                        PatientName = "Bindiya Tandel",
                        PrescriptionStatus = "Pending",
                        Diseases = new List<string>() { "Dengue, Fever" },
                        DoctorName = "Shara Shah",
                        ActionDateTime = null
                    }
                },
                StatusCode = 200
            };
        }

        public ResponseDto ToGetPatientPrescriptionBadRequestResponse()
        {
            return new ResponseDto
            {
                Response = "Mobile Number Does Not Exists.",
                StatusCode = 400
            };
        }

        public ResponseDto ToGetPatientPrescriptionFailedResponse()
        {
            return new ResponseDto
            {
                Response = "Internal Server Error.",
                StatusCode = 500
            };
        }

        public ResponseDto ToGetPrescriptionByIdSuccessfullResponse()
        {
            return new ResponseDto
            {
                Response = new PrescriptionUploadDto()
                {
                    
                    PrescriptionExpiryDate = DateTime.Parse("2022-06-08T00:00:00").ToString(),
                    PrescriptionFile = null,
                    PrescriptionFileName = "Test112e68502d-7609-48c7-a7a8-c0ee91a10fa9.png",
                    PrescriptionFilePath = "C:\\StaticFiles\\Test112e68502d-7609-48c7-a7a8-c0ee91a10fa9.png"
                },
                StatusCode = 200
            };
        }

        public ResponseDto ToGetPrescriptionByIdBadRequestResponse()
        {
            return new ResponseDto
            {
                Response = "Prescription Id Does Not Exists.",
                StatusCode = 400
            };
        }

        public ResponseDto ToGetPrescriptionByIdFailedResponse()
        {
            return new ResponseDto
            {
                Response = "Internal Server Error.",
                StatusCode = 500
            };
        }

        public Task<ResponseDto> GetCreatedPrescriptionByPrescriptionIdSuccessResponse()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = new PrescriptionDetailDto()
                {
                    PrescriptionId = 1,
                    InstituteName = "Medicare Institute",
                    DoctorName = "Manan",
                    InstituteAddress = "Navsari",
                    DoctorMobileNumber = "7894561230",
                    InstituteCity = "Navsari",
                    InstituteState = "Gujarat",
                    CreatedDateTime = "2022-06-08T00:00",
                    PatientName = "Hiral",
                    Email = "Test11@gmail.com",
                    PatientMobileNumber = "7895641230",
                    Gender = "FeMale",
                    Weight = 69.15,
                    Age = "28",

                    Diseases = new List<string> { "diaria"},
                    PrescriptionMedicine = new List<MedicineDetailsDto>
                    {
                        new MedicineDetailsDto
                        {
                            MedicineName ="Primaquine phosphate",
                            MedicineDosage = 1,
                            MedicineTiming =  new List<string> {"Mo,Af,Ev,Ni" } ,
                            AfterBeforeMeal = true,
                            MedicineDays = 180
                        }
                   },
                    IsExpired = "Prescription Expired",
                    PrescriptionStatus = "Approved",
                    PrescriptionExpiryDate = "2022-06-08T00:00",
                    ActionTimeStamp = "2022-11-28T12:00"
                },
                StatusCode = 200
            });
        }

        public Task<ResponseDto> GetCreatedPrescriptionByInvalidPrescriptionId_Response()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Prescription not found.",
                StatusCode = 400
            });
        }

        public Task<ResponseDto> GetCreatedPrescriptionByPrescriptionIdInternalServerResponse()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Internal Server Error",
                StatusCode = 500
            });
        }

        public Task<ResponseDto> SendForApproval_SuccessResponse()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Send For Approval Successfully.",
                StatusCode = 200
            });
        }

        public Task<ResponseDto> SendForApproval_InternalServerResponse()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Send For Approval Failed.",
                StatusCode = 500
            });
        }

        public Task<ResponseDto> SendForApproval_BadRequestResponse()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Prescription Id Does Not Exists.",
                StatusCode = 400
            });
        }

        public Task<ResponseDto> Get_Prescription_By_PrescriptionId_Print_Success_Response()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = new PrintDetailDto()
                {
                    FileName = "Test112e68502d-7609-48c7-a7a8-c0ee91a10fa9.png",
                    File = "C:\\StaticFiles\\Test112e68502d-7609-48c7-a7a8-c0ee91a10fa9.png"
                },
                StatusCode = 200
            });
        }

        public Task<ResponseDto> Get_Prescription_By_PrescriptionId_Print_Bad_Request_Response()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Prescription Id Does Not Exists.",
                StatusCode = 400
            });
        }

        public Task<ResponseDto> Get_Prescription_By_PrescriptionId_Print_InternalServer_Response()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Internal Server Error",
                StatusCode = 500
            });
        }
    }
}