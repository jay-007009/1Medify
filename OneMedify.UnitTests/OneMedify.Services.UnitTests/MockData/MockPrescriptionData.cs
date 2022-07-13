using Moq;
using OneMedify.DTO.Doctor;
using OneMedify.DTO.Patient;
using OneMedify.DTO.Prescription;
using OneMedify.DTO.Print;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneMedify.UnitTests.OneMedify.Services.UnitTests.MockData
{
    public class MockPrescriptionData
    {

        public Task<ResponseDto> Get_Success_Response()
        {
            return Task.FromResult(new ResponseDto { StatusCode = 200 });
        }

        public Task<ResponseDto> Get_Failure_Response()
        {
            return Task.FromResult(new ResponseDto { StatusCode = 400 });
        }

        public Task<Doctor> Get_Doctor_Response()
        {
            return Task.FromResult(new Doctor
            {
                DoctorId = 101,
                FirstName = "Test",
                LastName = "Test",
                Email = "Test@Test.com",
                MobileNumber = "7896302541"
            });
        }

        public Task<Doctor> Get_NullDoctor_Response()
        {
            return Task.FromResult(It.IsAny<Doctor>());
        }

        public Task<Patient> Get_Patient_Response()
        {
            return Task.FromResult(new Patient
            {
                PatientId = 101,
                FirstName = "Test",
                LastName = "Test",
                Email = "Test@Test.com",
                MobileNumber = "7896302541",
                PatientDisease = new List<PatientDisease>
                {
                    new PatientDisease
                    {
                        PatientDiseaseId = 1,
                        PatientId = 1,
                        IsDisable = false,
                        DiseaseId = 1
                    }
                }
            });
        }

        public Task<Patient> Get_NullPatient_Response()
        {
            return Task.FromResult(It.IsAny<Patient>());
        }

        public Task<List<Disease>> Get_Disesaes_Response()
        {
            return Task.FromResult(new List<Disease>
            {
                new Disease
                {
                   DiseaseId = 1,
                   DiseaseName = "Test"
                }
            });
        }

        public Task<Disease> Get_NullDisease_Response()
        {
            return Task.FromResult(It.IsAny<Disease>());
        }

        public Task<List<Medicine>> Get_MedicineList_Response()
        {
            return Task.FromResult(new List<Medicine>
            {
                new Medicine
                {
                    MedicineId = 1
                }
            });
        }

        public Task<Medicine> Get_Medicine_Response()
        {
            return Task.FromResult(new Medicine
            {
                MedicineId = 1,
                MedicineName = "Test"
            });
        }

        public Task<Medicine> Get_NullMedicine_Response()
        {
            return Task.FromResult(It.IsAny<Medicine>());
        }

        public Task<Prescription> Get_Prescription_Response()
        {
            return Task.FromResult(new Prescription
            {
                PrescriptionId = 1,
                PatientId = 1,
                DoctorId = 1,
                PharmacyId = 1
            });
        }

        public Task<Prescription> Get_NullPrescription_Response()
        {
            return Task.FromResult(new Prescription());
        }

        public Task<PrescriptionMedicine> Get_PrescriptionMedicine_Response()
        {
            return Task.FromResult(new PrescriptionMedicine
            {
                PrescriptionMedicineId = 1,
                PrescriptionId = 1,
                MedicineId = 1
            });
        }

        public PrescriptionCreateDto Get_PrescriptionDto_Response()
        {
            return new PrescriptionCreateDto
            {
                DoctorMobileNumber = "9635201874",
                PatientMobileNumber = "6932015478",
                DiseaseIds = new List<int> { 1 },
                Medicines = new List<PrescriptionMedicineDto>
                {
                    new PrescriptionMedicineDto
                    {
                        MedicineId = 1,
                        AfterBeforeMeal = true,
                        MedicineDays = 20,
                        MedicineDosage = 1,
                        MedicineTiming = new List<string>{ "Mo" }
                    }
                },
                ExpiryDate = "2022-12-05"
            };
        }

        public Task<List<Prescription>> GetFakeListOfPrescriptionCountByMobileNumber()
        {
            return Task.FromResult(new List<Prescription>
            {
               new  Prescription
                {
                    CreatedDate=DateTime.Parse("2022-02-25")
                }
            });
        }

        public Task<List<Prescription>> Get_PrescriptionList_Response()
        {
            return Task.FromResult(new List<Prescription>
            {
                new Prescription
                {
                    PrescriptionId = 101,
                    CreatedDate = Convert.ToDateTime("2022-05-17 12:30:20"),
                    ModifiedDate = Convert.ToDateTime("2022-05-17 12:30:20"),
                    ExpiryDate = Convert.ToDateTime("2022-05-17 12:30:20"),
                    PrescriptionStatus = 1,
                    PrescriptionType = false,
                    Patient = new Patient
                    {
                        FirstName = "Test",
                        LastName = "Test",
                        MobileNumber = "0123654789",
                        ProfilePictureFileName = "test.jpg",
                        ProfilePictureFilePath = "C://StaticFiles//test.jpg"
                    },
                    PrescriptionMedicines = new List<PrescriptionMedicine>
                    {
                        new PrescriptionMedicine
                        {
                            Medicine = new Medicine
                            {
                                Disease = new Disease
                                {
                                    DiseaseName = "test"
                                }
                            }
                        }
                    },
                    PrescriptionUpload = new PrescriptionUpload
                    {
                        Diseases = "1"
                    },
                    SentFromPharmacy = new Pharmacy
                    {
                        PharmacyName = "Test"
                    },
                    Doctor = new Doctor
                    {
                        FirstName = "Test",
                        LastName = "Test"
                    }
                }
            });
        }

        public Task<List<Prescription>> Get_NullPrescriptionList_Response()
        {
            return Task.FromResult(new List<Prescription>());
        }

        public Task<Prescription> Get_Created_Prescription_By_PrescriptionId_Response()
        {
            return Task.FromResult(new Prescription
            {
                PrescriptionId = 1,
                Doctor = new Doctor
                {
                    InstituteName = "Medicare Institute",
                    FirstName = "Manan",
                    LastName = "Patel",
                    Address = "B/204, Amardham Society,Tithal Road",
                    MobileNumber = "7895213690",
                    City = new City
                    {
                        CityName = "Navsari",
                        State = new State
                        {
                            StateName = "Gujarat"
                        },
                    },
                },
                CreatedDate = DateTime.Parse("1998-05-15"),
                Patient = new Patient
                {
                    FirstName = "Rudra",
                    LastName = "Patel",
                    Email = "Test11@gmail.com",
                    MobileNumber = "7894561230",
                    Gender = "Male",
                    Weight = 70.25,
                    DateOfBirth = DateTime.Parse("1998-05-15"),
                    PatientDisease = new List<PatientDisease>
                    {
                       new PatientDisease
                       {
                           Disease= new Disease
                           {
                                DiseaseName= "Dengue, Fever"
                           }
                       }
                    }
                },
                PrescriptionMedicines = new List<PrescriptionMedicine>
                    {
                        new PrescriptionMedicine
                        {
                           Medicine= new Medicine
                            {
                            MedicineName ="Primaquine phosphate"
                            },
                             MedicineDosage = 1,
                            MedicineTiming = "Mo,Af,Ev,Ni",
                            AfterBeforeMeal = true,
                            MedicineDays = 180,

                        }
                   },
                PrescriptionStatus = 1,
                ExpiryDate = DateTime.Parse("2022-06-10"),
                PrescriptionType = true,
            });
        }

        /// <summary>
        /// jay chauhan
        /// </summary>
        /// <returns></returns>
        public Task<Prescription> Get_Null_Created_Prescription_By_PrescriptionId_Response()
        {
            return Task.FromResult(new Prescription { });
        }


        public Task<Prescription> GetPrescriptionById_Response()
        {
            return Task.FromResult(new Prescription
            {
                PrescriptionId = 1,
                PatientId = 1,
                DoctorId = 1,
                PrescriptionStatus = 2,
                PrescriptionType = false,
                ModifiedByPatient = 1,
                CreatedBy = 1
            });
        }

        public Task<List<PrescriptionUpload>> Get_UploadedPrescriptionList_Response()
        {
            return Task.FromResult(new List<PrescriptionUpload>
            {
                new PrescriptionUpload
                {
                    PrescriptionId = 101,
                    Diseases = "1",
                    Prescription = new Prescription
                    {
                        PrescriptionId = 101,
                        CreatedDate = Convert.ToDateTime("2022-05-17 12:30:20"),
                        ExpiryDate = Convert.ToDateTime("2022-05-17 12:30:20"),
                        PrescriptionStatus = 1,
                        Patient = new Patient
                        {
                            FirstName = "Test",
                            LastName = "Test",
                            MobileNumber = "0123654789",
                            ProfilePictureFileName = "test.jpg",
                            ProfilePictureFilePath = "C://StaticFiles//test.jpg"
                        }
                    }
                }
            });
        }

        public Task<List<PrescriptionUpload>> Get_NullUploadedPrescriptionList_Response()
        {
            return Task.FromResult(new List<PrescriptionUpload>());
        }

        public Task<Disease> Get_Disease_Response()
        {
            return Task.FromResult(new Disease
            {
                DiseaseId = 101,
                DiseaseName = "Test"
            });
        }

        public UploadPrescriptionDto MockUploadPrescriptionDto()
        {
            return new UploadPrescriptionDto
            {
                PatientMobileNumber = "1234567890",
                DiseaseIds = new List<int> { 1, 2, 3 },
                PrescriptionDocument = "c/test.png"
            };
        }

        public UploadPatientPrescriptionDto MockUploadPatientPrescriptionDto()
        {
            return new UploadPatientPrescriptionDto
            {
                PharmacyMobileNumber = "1234567890",
                PatientMobileNumber = "1234567890",
                DiseaseIds = new List<int> { 1, 2, 3 },
                PrescriptionDocument = "c/test.png"
            };
        }

        public DoctorActionDto MockDoctorActionLog()
        {
            return new DoctorActionDto
            {
                DoctorMobileNumber = "1098765432",
                Action = "Approve",
                ExpiryDate = "2022-12-06"
            };
        }

        public Task<Pharmacy> GetFakePharmacyByMobileNumber()
        {
            return Task.FromResult(new Pharmacy
            {
                ProfilePictureFilePath = null,
                PharmacyName = "Test",
                MobileNumber = "9856320121",
                PharmacyEstablishmentDate = DateTime.Parse("2022-05-15"),
                Email = "Test11@gmail.com",
                Address = "Test Society",
                PharmacyCertificateFilePath = "C:\\Users\\Documents\\Test1171af3319-ee77-40ee-8504-a3095894ca25.jpg",
                PharmacistDegreeFilePath = "C:\\Users\\Documents\\Test11ebe1e155-55fd-4af9-ae94-f04c33279d40.jpg"
            });
        }

        public Prescription Mock_UpdatePrescriptionStatus()
        {
            return new Prescription
            {
                ExpiryDate = DateTime.Now,
                PrescriptionStatus = 1,
                ModifiedByDoctor = 1,
                ModifiedDate = DateTime.Now
            };
        }

        public Task<Doctor> Mock_DoctorAsync()
        {
            return Task.FromResult(new Doctor
            {
                FirstName = "abc",
                LastName = "xyz",
                Address = "Valsad",
                CityId = 1,
                CreatedBy = 1,
                CreatedDate = DateTime.Now,
                DoctorDegreeFileName = "xyz.png",
                DoctorDegreeFilePath = "C:/xyz.png",
                DoctorId = 1,
                Email = "abc@gmail.com",
                InstituteCertificateFileName = "xyz.png",
                InstituteEstablishmentDate = DateTime.Now,
                InstituteCertificateFilePath = "C:/xyz.png",
                InstituteName = "xyz",
                IsDisable = false,
                MobileNumber = "7845962154",
                Gender = "Male",
                Specialization = "MBBS"
            });
        }


        public SendForApprovalDto Mock_SendForApproval()
        {
            return new SendForApprovalDto
            {
                PharmacyMobileNumber= "9856320222"

            };

        }
        public Task<Prescription> Mock_PrescriptionAsync()
        {
            return Task.FromResult(new Prescription
            {
                PatientId=2,
                DoctorId=9,
                PrescriptionStatus=2,
                ExpiryDate=null,
                CreatedDate= DateTime.Now,
                ModifiedDate=DateTime.Now,
                IsDisable=false,
                CreatedBy=9,
                PharmacyId=234,
                PrescriptionType=true,
                ModifiedByPatient=null,
                ModifiedByDoctor=null,
                ModifiedByPharmacy=null
            });

        }

        public Prescription Mock_Prescription()
        {
            return new Prescription
            {
                PatientId = 2,
                DoctorId = 9,
                PrescriptionStatus = 2,
                ExpiryDate = null,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                IsDisable = false,
                CreatedBy = 9,
                PharmacyId = 234,
                PrescriptionType = true,
                ModifiedByPatient = null,
                ModifiedByDoctor = null,
                ModifiedByPharmacy = null
            };

        }


        public Task<Prescription> Mock_EmptyPrescriptionAsync()
        {
            return Task.FromResult(new Prescription
            {
            });

        }

        public Task<bool> Mock_TrueAsync()
        {
            return Task.FromResult(true);
        }

        public Task<bool> Mock_FalseAsync()
        {
            return Task.FromResult(false);
        }

        public Task<Prescription> Get_ApprovedCreatedPrescriptionByPatientMobileNumber_Response()
        {
            return Task.FromResult(new Prescription
            {
                PrescriptionId=2,
                PrescriptionMedicines = new List<PrescriptionMedicine>
                {
                    new PrescriptionMedicine
                    {
                        Medicine= new Medicine
                        {
                            Disease =new Disease
                            {
                                DiseaseName = "Fever,Jaundice",

                            }

                        },
                       
                    }
                    
                },
                Doctor = new Doctor 
                { FirstName = "Jigar" },
                ExpiryDate= DateTime.Now,
                CreatedDate= DateTime.Now
            });
        }

        public Task<List<Prescription>> Get_PatientPrescriptionByMobileNumber()
        {
            return Task.FromResult(new List<Prescription>
            {
                new Prescription
                {
                    PrescriptionId = 1,
                    Patient = new Patient
                    {
                        ProfilePictureFileName = "test.jpg",
                        ProfilePictureFilePath = "C://StaticFiles//test.jpg",
                        FirstName = "Raj Patel",
                        LastName ="Patel",
                        PatientDisease = new List<PatientDisease>
                        {
                          new PatientDisease
                          {
                             Disease= new Disease
                             {
                                DiseaseName= "Dengue, Fever"
                             }
                          }
                        }

                    },
                    PrescriptionStatus = 2,
                    Doctor = new Doctor
                    {
                        FirstName = "Raj",
                        LastName ="Patel",
                    },
                    ModifiedDate = DateTime.Parse("1998-05-15"),
                    PrescriptionType = true
                }
            });
        }

        public Task<PrescriptionUpload> Get_UploadedPrescriptionByPrescriptionId_Response()
        {
            return Task.FromResult(new PrescriptionUpload
            {
                PrescriptionId = 870,
                Diseases = "Dengue, Fever",
                Prescription = new Prescription
                {
                    PrescriptionStatus = 1,
                    PrescriptionType = false,
                    DoctorActionLogs = new List<DoctorActionLog>
                    {
                        new DoctorActionLog
                        {
                            ActionTimeStamp = DateTime.Now
                        }
                    }
                },
                PrescriptionFileName = null,
                PrescriptionFilePath = "testing2aeb2399 - a8de - 4949 - 80d9 - 396a77c51d46.png",
            });

        }
    }
}
