﻿using OneMedify.DTO.Patient;
using OneMedify.DTO.Prescription;
using OneMedify.DTO.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneMedify.UnitTests.OneMedify.API.UnitTests.MockData
{
    public class MockPatientControllerData
    {
        public Task<ResponseDto> GetPatientSuccessResponse()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Successfully Register",
                StatusCode = 200,
            });
        }

        public Task<ResponseDto> GetPatientInternalServerResponse()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Internal Server Error",
                StatusCode = 500
            });
        }

        public Task<ResponseDto> GetPatientPrescriptionFailedResponse()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Internal Server Error",
                StatusCode = 500
            });
        }

        public Task<ResponseDto> GetPatientPrescriptionSuccessResponse()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = new PrescriptionDetailsDto()
                {
                    Diseases = new List<string>() { "Dengue, Fever" },
                    PrescriptionMedicines = new List<PrescriptionMedicineDetailsDto>
                   {
                        new PrescriptionMedicineDetailsDto
                        {
                            MedicineName ="Primaquine phosphate",
                            MedicineDosage = 1,
                            MedicineTiming = "Mo,Af,Ev,Ni",
                            AfterBeforeMeal = true,
                            MedicineDays = 180
                        }
                   },
                    CreatedDate = "0001-01-01T00:00"
                },
                StatusCode = 200
            });
        }

        public Task<ResponseDto> GetPatientPrescriptionBadRequestResponse()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Patient does not exist",
                StatusCode = 400
            });
        }

        public Task<ResponseDto> GetPatientByPatientMobileNo_BadRequest()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Patient Does Not Exists.",
                StatusCode = 400
            });
        }

        public Task<ResponseDto> GetPatientByPatientMobileNo_InternalServerIssue()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Internal Server Issues",
                StatusCode = 500
            });
        }

        public Task<ResponseDto> GetPatientByPatientMobileNo_SuccessResponse()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = new PatientDetailsDto
                {
                    MobileNumber = "7485961263",
                    Email = "abc@gmail.com",
                    FullName = "abc",
                    Gender = "male",
                    Age = "21",
                    Diseases = new List<string> { "Fever", "Malaria", "Dengue", "Typhoid" }
                },
                StatusCode = 200
            });
        }

        public Task<ResponseDto> GetPatientProfile_ByPatientMobileNo_SuccessResponse()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = new PatientProfileDetailsDto
                {
                    ProfilePicture = "iVBORnsjklfhjnj",
                    FirstName = "Test",
                    LastName = "Test",
                    MobileNumber = "1234567890",
                    Email = "test@gmail.com",
                    Gender = "Male",
                    Diseases = new List<string> { "Fever", "Malaria", "Dengue", "Typhoid" },
                    DateOfBirth = DateTime.Now.ToString(),
                    Weight = 25.0M,
                    Address = "valsad",
                    State = "Gujarat",
                    City = "Valsad"
                },
                StatusCode = 200
            });
        }

        public Task<ResponseDto> GetPatient_InvalidMobileNumber_Response()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Mobile number does not exist.",
                StatusCode = 400
            });
        }

        public Task<ResponseDto> GetPatientUploadedPrescriptionSuccessResponse()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = new List<MyUploadedPrescriptionsDto>
                {
                    new MyUploadedPrescriptionsDto
                    {
                        PrescriptionId = 1,
                        PrescriptionStatus = "Rejected",
                        ActionDateTime = DateTime.Now.ToString(),
                        DoctorName = "test",
                        IsExpired = "Prescription Expired",
                        Diseases = new List<string>{ "Fever" }
                    }
                },
                StatusCode = 200
            });
        }

        public Task<ResponseDto> GetPatient_InvalidPageIndex_Response()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "PageIndex Shoud not be less than 0.",
                StatusCode = 400
            });
        }

        public PatientUpdateDto Mock_PatientUpdateDto()
        {
            return new PatientUpdateDto
            {
                Address ="301,tulsi vakita",
                CityId = 12,
                Weight = 67,
                Gender = "Male",
                ProfilePicture = "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AAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0AAAMITQAAAwhNAAADCE0A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"
            };
        }

        public Task<ResponseDto> Mock_PatientUpdate_NothingToUpdate_Response()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Nothing to Update.",
                StatusCode = 200
            });
        }

        public Task<ResponseDto> Mock_PatientUpdate_ValidPatient_Response()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Patient With This Mobile Number Does Not Exists.",
                StatusCode = 400
            });
        }

        public Task<ResponseDto> Mock_PatientUpdate_InvalidFileSize_Response()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "File exceeds the limit.",
                StatusCode = 400
            });
        }

        public Task<ResponseDto> Mock_PatientUpdate_InvalidFileExtension_Response()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Invalid File Type.",
                StatusCode = 400
            });
        }

        public Task<ResponseDto> Mock_PatientUpdate_Failed_Response()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Patient Update Failed.",
                StatusCode = 500
            });
        }

        public Task<ResponseDto> Mock_PatientUpdate_Success_Response()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Patient Updated Successfully.",
                StatusCode = 200
            });
        }

        public Task<ResponseDto> GetCreatedPrescriptionsCountDtoResponse()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = new CreatedPrescriptionsCountDto
                {
                    CreatedPrescriptionsCount = 1
                },
                StatusCode = 200
            });
        }    

        public Task<ResponseDto> UploadPrescription_SuccessResponse()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = new List<UploadPrescriptionDto>
                {
                    new UploadPrescriptionDto
                    {
                        PatientMobileNumber = "8650231777",
                        PrescriptionDocument="c/test.png",
                        DiseaseIds = new List<int>{ 1,5,3}
                    }
                },
                StatusCode = 200
            });
        }
        public Task<ResponseDto> UploadPrescription_FailureResponse()
        {
            return Task.FromResult(new ResponseDto
            {
                StatusCode = 500,
                Response= "Prescription not uploaded."
            });
        }
        public Task<ResponseDto> ToCheckPrescriptionInvalidFileTypeResponse()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Invalid File Type.",
                StatusCode = 400
            });
        }

        public Task<ResponseDto> ToCheckPrescriptionInvalidFileSizeResponse()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "The file should not be more than 3MB.",
                StatusCode = 400
            });
        }
        public Task<ResponseDto> Patient_InvalidMobileNumber_Response()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Patient’s mobile number does not exist.",
                StatusCode = 400
            });
        }
        public Task<ResponseDto> GetDisease_ByDiseaseInvalid()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Disease passed in prescription does not exist.",
                StatusCode = 400
            });
        }

        public Task<ResponseDto> GetApprovedCreatedPrescriptionByPatientMobileNumberAsync_SuccessResponse()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = new ApprovedCreatedPrescriptionsDto
                {
                    Diseases = new List<string> { "Dengue", "Fever", "Jaundice" },
                    DoctorName = "Abhishek Patel",
                    ActionDateTime = "2022-06-01 05:32"
                },
                StatusCode = 200
            });
        }

        public Task<ResponseDto> GetApprovedCreatedPrescriptionByPatientMobileNumberAsync_BadRequest()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Approved Created Prescription By Patient Mobile Number in that Mobile Number is not valid.",
                StatusCode = 400
            });
        }

        public Task<ResponseDto> GetApprovedCreatedPrescriptionByPatientMobileNumberAsync_InternalServerError()
        {
            return Task.FromResult(new ResponseDto
            {
                Response = "Approved Created Prescription By Patient Mobile Number dooesnt exist",
                StatusCode = 500
            });
        }
    }
}