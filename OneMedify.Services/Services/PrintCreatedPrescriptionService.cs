using DinkToPdf;
using DinkToPdf.Contracts;
using OneMedify.DTO.Prescription;
using OneMedify.DTO.Print;
using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Entities;
using OneMedify.Resources;
using OneMedify.Services.Contracts;
using OneMedify.Services.Contracts.PrescriptionContracts;
using OneMedify.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneMedify.Services.Services
{
    public class PrintCreatedPrescriptionService : IPrintCreatedPrescriptionService
    {
        private readonly IGetCreatedPrescriptionDetailsByPrescriptionId _getCreatedPrescriptionDetailsByPrescriptionId;
        private readonly IFileUpload _fileUpload;
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IFileService _fileService;
        private readonly IConverter _converter;
        private readonly IPatientRepository _patientRepository;

        public PrintCreatedPrescriptionService(IGetCreatedPrescriptionDetailsByPrescriptionId getCreatedPrescriptionDetailsByPrescriptionId,
                                               IFileUpload fileUpload, IPrescriptionRepository prescriptionRepository,
                                               IFileService fileService, IConverter converter, IPatientRepository patientRepository)
        {
            _getCreatedPrescriptionDetailsByPrescriptionId = getCreatedPrescriptionDetailsByPrescriptionId;
            _fileUpload = fileUpload;
            _prescriptionRepository = prescriptionRepository;
            _fileService = fileService;
            _converter = converter;
            _patientRepository = patientRepository;
        }

        public async Task<ResponseDto> PrintPrescriptionAsync(int prescriptionId)
        {
            try
            {
                var prescription = await _prescriptionRepository.ReadById(prescriptionId);
                var printDetails = new PrintDetailDto();
                if (prescription.PrescriptionType)
                {
                    var createdPrescription = await _getCreatedPrescriptionDetailsByPrescriptionId.GetCreatedPrescriptionByPrescriptionIdAsync(prescriptionId);
                    if (createdPrescription.StatusCode != 200)
                    {
                        return new ResponseDto { StatusCode = 400, Response = PrescriptionResource.PrescriptionNotExistsById };
                    }
                    var htmlString = GenerateCreatedTemplate(createdPrescription);
                    var data = GeneratePdf(htmlString, true);
                    var file = Convert.ToBase64String(data);
                    printDetails.File = file;
                    printDetails.FileName = _fileUpload.GetPrescriptionFileName(createdPrescription.Response.PatientName, PrescriptionResource.PdfFileExtension);

                }
                else
                {
                    var uploadedPrescription = await _prescriptionRepository.GetUploadedPrescriptionByPrescriptionIdAsync(prescriptionId);
                    var diseases = _patientRepository.ReadDiseaseByIds(uploadedPrescription.PrescriptionUpload.Diseases.Split(",")
                        .Select(int.Parse).ToList()).Result.Select(x => x.DiseaseName).ToList();
                    var uploadedHtml = GenerateUploadedTemplate(uploadedPrescription, diseases);
                    var data = GeneratePdf(uploadedHtml, false);
                    printDetails.File = Convert.ToBase64String(data);
                    printDetails.FileName = _fileUpload.GetPrescriptionFileName(uploadedPrescription.Patient.FirstName + " " 
                                                                                 + uploadedPrescription.Patient.LastName, PrescriptionResource.PdfFileExtension);
                }
                return new ResponseDto { StatusCode = 200, Response = printDetails };
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        private byte[] GeneratePdf(string htmlContent, bool isCreated)
        {
            GlobalSettings globalSettings;
            if (isCreated)
            {
                globalSettings = new GlobalSettings
                {
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings
                    {
                        Top = 1,
                        Bottom = 0.5,
                        Left = 1,
                        Right = 1,
                        Unit = Unit.Inches
                    }
                };
            }
            else
            {
                globalSettings = new GlobalSettings
                {
                    PaperSize = PaperKind.A4
                };
            }
            var objectSettings = new ObjectSettings
            {
                HtmlContent = htmlContent
            };

            var htmlToPdfDocument = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            return _converter.Convert(htmlToPdfDocument);
        }

        public string PrescriptionMedicineList(List<MedicineDetailsDto> medicineDetailsDtos)
        {
            string output = "";
            foreach (var medicineDetail in medicineDetailsDtos)
            {
                output += ($"<div class=\"d-flex ng-star-inserted\">" +
                                $"<div class=\"text-start pb-3 text-wrap col-5\">{medicineDetail.MedicineName}</div>" +
                                $"<div class=\"text-center pb-3 px-4 col-2\"> {medicineDetail.MedicineDosage} </div>" +
                                $"<div class=\"pb-3 px-4 col-4\"><span>{string.Join(", ", medicineDetail.MedicineTiming)} { GetMedicineTiming((bool)medicineDetail.AfterBeforeMeal) }" +
                                $"</span><span class=\"text-info\"></span></div>" +
                                $"<div class=\"text-center pb-3 pe-2 col-1\">{medicineDetail.MedicineDays}</div>" +
                            $"</div>");
            }
            return output;
        }

        public string GetMedicineTiming(bool afterBeforeMeal)
        {
            return afterBeforeMeal ? "After" : "Before";
        }

        public string GenerateCreatedTemplate(ResponseDto response)
        {
            var htmlString = $"<!DOCTYPE html>" +
                                 $"<html lang = \"en\">" +

                                 $"<head>" +
                                    $"<meta charset = \"UTF-8\">" +
                                    $"<meta http - equiv = \"X-UA-Compatible\" content = \"IE=edge\">" +
                                    $"<meta name = \"viewport\" content = \"width=device-width, initial-scale=1.0\">" +
                                    $"<title> Prescription </title>" +
                                    "<style>" +
                                       "@import url(https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500&display=swap);@media all{h-100{height: 100%;}body{margin:auto;}.d-flex" +
                                       "{display:-webkit-box;display:-ms-flexbox;display:flex}.flex-column{-webkit-box-orient:vertical;-webkit-box-direction:normal;-ms-flex-direction:column;" +
                                       "flex-direction:column}.flex-shrink-0{-ms-flex-negative:0;flex-shrink:0}.line-height-normal{line-height:1}.fs-3{font-size:32px}.fs-4{font-size:28px}.m-0" +
                                       "{margin:0}.py-1{padding-top:.25rem;padding-bottom:.25rem}.py-2r{padding-top:2rem;padding-bottom:2rem}.px-2{padding-left:2rem;padding-right:2rem}.pe-1" +
                                       "{padding-right:.25rem}.pb-3{padding-bottom:1rem}.justify-content-between{-webkit-box-pack:justify!important;-ms-flex-pack:" +
                                       "justify!important;justify-content:space-between!important}.border-bottom{border-bottom:1px solid #dee2e6!important}.text-start{text-align:left!important}.text-end" +
                                       "{text-align:right!important}.text-wrap-all{overflow-wrap:anywhere}.text-wrap{overflow-wrap:break-word}.text-nowrap{white-space:nowrap}" +
                                       ".col-1{width:8.3333333%}.col-4{width:33.33333333%}.col-5{width:41.66666667%}.col-2{width:16.66666667%}.fw-bold{font-weight:500!important}" +
                                       ".text-primary-light{color:#76969e}.text-primary{color:#0e333a}.text-info{color:#76969e}.text-center{text-align:center!important}.word-break{word-break:break-all}" +
                                       ".template-preview{width:100%!important;overflow-x:hidden;font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Oxygen,Ubuntu,Cantarell,'Open Sans','Helvetica Neue',sans-serif}" +
                                       ".template-preview-bg:before{content:'';background-image:url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAIMAAACDCAYAAACunahmAAAAAXNSR0IArs4c6QAADiBJREFUeF7tnU1oXNcVgO9oNCPp6df2yLLkwAjX" +
                                       "YEal6ULZtNDgrBwK3djIELBDS8O4pDSV29JSefM2Mm76IxGT0BnaukQuhQlOCd4kdOGQltCFtXFJtYgXVjGufyaWZEdPmhmNprw38zR/b94759x73/z4zsZgnft3znfPPfe++xNg6qc0UNJAQGlCacDWgIJBsbCnAQWDgkHBoBio14DyDIoK5RkUA8o" +
                                       "zKAZcNKCGCYVHZw0TsWufjGtM88WsBmN7JS3/e/oB0wO7vhTsQyFt7RmOffDPwYGd3gHGTBM152eWvHLqxf81p3SxpbYtDN9Ifdq3HtwZEasOWm6mT1ruACDaFgZzaKCZTk6qlZPfus8CgYKc3P3JtS1hmEp9Fi4Evzjgj4rgpbTKcDERj2tZgw0Xa1" +
                                       "6OpdJXF12Hs7aEIbbUWl7BxmXlrP+xQ+TMLNhDKhjgHZtb0i8YIvHZcUrM3MEwNG8G4UyNxmTDQIXArm+HwvAh2DVyd3dEBitnX5YyxYyciY9Xjv2IKpVFNcbSyY6MGZ4RGGZmgpGewwdJxq9J5OUVTPE2DSA7H4bDr/7wQGY3FBYBgpmHgkGUJoH5i" +
                                       "BomDr7247Hd7UAXsFiQWOfCkGhRz3COP2YY+v75/eFMoQdkYaBQ+urifcaY54JYew4TiWutGUCeO8UXQOp6V+T2+hjQxmAxiFdo35ihQ2HALCBBSdgt5Dcf/+XyE4g82jOMzrw+UOgJD+6tdFZM94OB7icPln6zCSmYRybWgTAImT46KBXqFcCegUys" +
                                       "ObdlRpolkzke49em7UwYvJaVK3dSALUZDn+Z/tObT4HSsKmlEGo1g6WTSb4xtdSqToOB3Nk8rIzxCmDPYAqKqjBmDGvU1tjCtXGfNjZBO5Ult0IMIBvq1vzgSFx1x4LQFBhs7VIqa6eNLSy15mzi/Fm852uwypjWjDSbmMhTZhd5zTDWkskNFMnYFUh" +
                                       "R3oEXiE6CwUmn6bsjj9jH+g5V39SOhppNUCvnRiil4p0Mgw3C6HdfP1TYCaPswzjjMlxhAmOHSkCwQHQqDDYI1BgNq8faToqGgRFWycL5YC4bzIcaeQhsIzoRBlNH9/7627QFAnbzCuDzNCR+wMNA9A6mwd0aiQHChIEYZEN0QpZZJQSQ9tBb2X7McF" +
                                       "zpTcgVLyUkwUB2Y0dHHjBd33UMmjw2a1Y2NDafGDf8OTOD0u/q+XPo2YSpi6qOgNjDgOlAkIaQYSgCgd+B0yhACga28w+Wfv8QUmkLBoigm4ymsdyGsdHby0L5vKZpmsEMzkxXL+BhqK0iRKeiIbDrwAUD2UOUpk616aGNjM4nSOsMpjNZcTNYoRCIX" +
                                       "kweonImBgaHZWkzJlgb+YK9p2epdYOk44aBDETFN3ancdOt8hQYArnw1h39e+sQpVDyN/MVAkM8Pr6T1bbW/7wIqiukPVAZITCYhY2d/cHBfKE3CC3YlKvyBLreZcYTkPRYY+Vzwcxd/bXHkLxtGWwZomDA1FG0rDAY7IphIuE6IICtwxqK2mP9KgfY" +
                                       "bOliwmGwanxc7448tz4KqX26NMOAyJZ77QJw67jBVi+cR0f4djlT+tsDm6Hu4t4NwI8KHSBrX0TkwFBR9YlXfhpxW3Aa2Zd/fPvy5QymtdF5EwbIT+Mex4veATbN4AEP0hrZMtJh4G2A1Tuz2cHVi+UebsEA3E9VmY5Sl+gcFDzGeMui1E9kmpaHwWy" +
                                       "sbZDV8MYjpus7fhrIz7KYXuhq5k0wbQFDJRDYnsDbW/2CYfraJ+Pb+e7MZ6e/iZr1YPXhJt82MDBWCETnFtELQn7CUKvoQHhj7Y6ub3sZbPr6Tc3IGsPNvvCDBMPxGze6n2bGRl23npX2b+a2ck9unfg6cIR3V9t0PBFKR4yIl3LLfzfY6sUL5NkEj0" +
                                       "cy00JBbHwLTfEDjOdnmH7GzBhq+dQLXG1FwzD94X+AkbyDyTTGll+c4qrwkV9cGs4H8576sUvPb/Wt3134yRYcoLJkdG6e3FYohNbFI66tAQBhwlD6LZ+YJl8nhIKBC4Qaayz/K0a+Ni86izCSZvZQmndAlVPRvtVFWHnWJWWZ0iVlnnh7XG5YCcTLN" +
                                       "A8BhmH6w5vkXuLWKwd7nj76+KWXdrA9F2soqIHsemDzt9Nhyqm7jkgQENZ1hAQg4DBckwODqUSjn1Z5rMGghsLmKwQEOxMRQBDjBxQMsHU4bB8vy2thtrH8nRdQxRz9kT6UC4YqnKR7+Zl8YOf+5blHTlJUCAq9Xdv/vfTLNWjLY0v2KfIGlucFwrqX" +
                                       "Ej9UgGHw895F9BV6M6lg9PDnQm44gRq07A3m7jMGv//x+Xc/OpgLFCq+7soBQh4MeqEr9pV/CD8q7qZ4ymVZ5Z4tZCbrykUgGy7ceUc37z0A/44k/j7coznNhMQDIQ2G6cTNkKFh5vdg/bgIGox4E0ogMjt3CDxuIKtqYpZevIieHk8nrmuGFipd1Ol" +
                                       "UqMvYQJh6SoNh8sqN3r7uzD6k3oSIE4Fg5pb+6Hp2DPjB0buu5hR18SLoBpTazKZSqXAhMwS40VYcENJgeP7dj/pzW4Uhb43JkVjhvB4nOjs3vokKS6vbkZ4IW7u6Ka07rt/ofjCeKe7t8AwMPYQQHkIaDMf++MFg187OAEUZItLkMn2Pb7/xbdSeh4" +
                                       "blmh7jXtaKfyoji9KKLrP/TSdpXqCq3FQqGFsbqg5spQJRhKl4y72k2cTklb+N9GV3+0QYlpoH9bg7tTzudLreFRv/2hiz8Kr5SQeC9gYGaGr51d+l9u/2B4XeQIZVdi6ztXn7jTOgu4mweQuXLxQCseT7FV9Y/QcCPT2HHsk/+lZqNJTPdAtXGjLDF" +
                                       "cLxNWQR/OJ1INhZUoFwiSNcPIw0GCbfvHKoL9QN8iL82mycw8rhnofs9Om8zDK48m4Igv9ASIOheLYRNNBx6dIrsXUiqlW9gycI/gIhFwYvS/n095bcjm7NGoLI5fCaYQPc12BrEdJgwB4mkclFJMLSy+fOCb1KkKe+04lEyGARxO6rytKocYQ3EM8E" +
                                       "DKYqW8U7TOmpcGE4c2BvNQncu+UDIREG+DkFnl4GTQvdWwjNjyI3kbiuDRvrNd8aNOAqY22J4j0E5UMfaIaA2S5OUSw2TbNhmFy4MtLHuhsswrUGEJJh4Fjcx1rbQ341nCN/K+CtCmya3Xwg5MGA2YDKq21Ieo5NrpDsG8nEFswbY6CvbvsNRPXi1LM" +
                                       "DgxlEAncg8xi/Mq0Ngv1/sFixeUBIgyEyOz/eDz3pKkr7XkMFYYMJtWrR+SXHsw1gIKo7LbAa1KCyWJhEGOZIj2oCW00SSyfxu40oBe2tsViWrze/XCDMMmmLU5RNQaDZRCQ+J+XMBMU4dho/YHBcbHNYlm9FICTC4PUwBo9ZaWm9Huyk5VpKlUoFo5" +
                                       "+vNV5ebgUgPIYeBQMXAcXEk/qV3kIo673fkwxEyYowd1LTIngcIQ8GwuWfAuzimgX0zkhMPZ7T/7A/GMrDN/G0MBCSYcCoVb5s+qqYJ47smk7qCyOFkIbf2tfg0z6s48ubekqDwVRY8Rrb1vmJhOHIpcRw/gnTnLYrglrMO9OAkYMaMuTCcLLFYHhfj" +
                                       "Gc4qr81lMvm+4sgkKxSNFIrAWGY72XhX98FTS1LSAYiJ+Poa3RAPYsglBYAw7Gf/2pwuztcPgLQIUBQQDBNgIHBOqUUuXXP1zOXjTjhhWFUf3tAy2brL/xsNhBkB1WcaYT6Ak9uvXqCdNgUB0PJMvtm4sPBPI9PJbiCmiQ8MHjeDcULhGVQ2JDTcE0T" +
                                       "lrykleLRH96zJSQY+E1JzyFSil14YADvz+iHW6RVdl/RNYsdJnhKEpjWBIIKAxgEu75AIBQMAg2Mycp8nP3Re+98iUljyqJB2APCeyBXMGCt0UT58jV+cNdfVV2POELB0ETjYoq2QKh6ZF48EAoGjEWaJDv2s1/39+5ky3dLVHFAgKKBh1AwNMnAmGI" +
                                       "db3CTAISCAWOVJsi6XuUnGAgFQxMMDC0SdKejQCAUDFDL+CxXFye4lc8LhJl3P//zRz6ryLG4tluBhCgN5BUqM+IFot88/0l/GA3SJj9kOg4G82a3suKQt0HuQYGcZSgY/GAVV8ZEXNdCWrbmMKw/QDT7/CdOU87SHeUZ7C399Z8T5AOhYBCBo6A8as" +
                                       "92iAPC+7uEKaFgEGRIEdk4HfTxEwgFgwgrCsjD9cSXVntAjTpkuHsIBYMAQ4rIwvP4Xx0Q1uIAvGjA1FPBAFenNMlI3Dz6B5gKSgZCwSDNxPCMwTCUvHy9PxDjIRQMcJtJkZya0cMP961XvOPgg4eoCh3K5SkYpJgYnmnRK9T+AEAUPyfU/BAewgEIB" +
                                       "QPcblIknWGArQtYIaRAIBQMUkwMy7R+iGiuh6C+qgtrrT9Sbbsc3dgrVCrOvyFDweAPsI6lRM6U4gVPe3sKFPPnXJzy+/Y5GapvX89gwwAKEahAWJEFSO8KBpCa5AjteQY7e097ewo08BAwIBQMcuwMyTUQOTPrfD2Aq83lAaFggJhNgsxEPK5lDa3x" +
                                       "67GeNvcUcIgh3D2EgkGCoSFZTrwSj2SDWshV1tPengJW9tC1CAUDxHISZDw9g+A4AgKEgkGCoaFZ1gWQjRJ6OgBPAZCHUDBALSdBrnj7HMyQ3mKwfBp5iJyR27iX1FvnQQ6ivtt2ncFsbzOAqF2c2mT9LO3zcwdEW3sma2sY7NaVoag6d1/deFDnBwn" +
                                       "tzTRWR8JNexHH07IEgY6AgdBulcRBAwoGhcWeBhQMCgYFg2KgXgPKMygqlGdQDCjPoBhw0YAaJhQeaphQDKhhQjGghgnFAEQDKmaAaOkZkfk/9r7WwLL9hiEAAAAASUVORK5CYII=');background-repeat:no-repeat;background-attachment:local;" +
                                       "background-position:center;background-size:300px;position:absolute;top:0;right:0;bottom:0;left:0;z-index:-1}.doctor-details{display:-webkit-box;display:-ms-flexbox;display:flex;border-bottom:1px solid #e7f2f5}" +
                                       ".patient-details{-webkit-box-pack:justify;-ms-flex-pack:justify;justify-content:space-between;padding-top:1rem}.w-400px{width:400px}.w-50{width:50%}}" +
                                    "</style>" +
                                 $"</head>" +

                                 $"<body class=\"text-primary template-preview-bg\">" +
                                    $"<div class=\"template-preview d-flex flex-column\">" +
                                        $"<div class=\"doctor-details justify-content-between pt-5 pb-3\">" +
                                            $"<div class=\"text-start w-50\">" +
                                                $"<h2 class=\"text-wrap-all line-height-normal fs-3\">{response.Response.InstituteName}</h2>" +
                                                $"<div class=\"text-wrap-all text-info py-1 word-break\">{response.Response.InstituteAddress}" +
                                                $"</div>" +
                                                $"<div class=\"text-info\">" +
                                                    $"<small class=\"pe-1\">{response.Response.InstituteCity},</small>" +
                                                    $"<small> {response.Response.InstituteState}</small>" +
                                                $"</div>" +
                                            $"</div>" +
                                            $"<div class=\"text-end flex-shrink-0 w-50\">" +
                                                $"<h2 class=\"text-wrap-all fs-4\">Dr.{response.Response.DoctorName}</h2>" +
                                                $"<p class=\"d-block py-1 text-info m-0\">{response.Response.DoctorMobileNumber}</p>" +
                                                $"<p class=\"d-block py-1 text-info m-0\">{Convert.ToDateTime(response.Response.CreatedDateTime):dd MMM yyyy}</p>" +
                                            $"</div>" +
                                        $"</div>" +
                                        $"<div class=\"patient-details d-flex\">" +
                                            $"<div class=\"text-start\">" +
                                                $"<div class=\"d-flex py-1\">" +
                                                    $"<div class=\"flex-shrink-0 text-info text-nowrap pe-1\">Patient Name: </div>" +
                                                    $"<div class=\"fw-bold text-wrap w-400px word-break\">  {response.Response.PatientName}</div>" +
                                                $"</div>" +
                                                $"<div class=\"d-flex py-1\">" +
                                                    $"<div class=\"flex-shrink-0 text-info pe-1\">Email ID: </div>" +
                                                    $"<div class=\"fw-bold text-wrap w-400px word-break\"> {response.Response.Email}</div>" +
                                                $"</div>" +
                                                $"<div class=\"py-1\"><span class=\"text-info\">Mobile No: </span><span class=\"fw-bold\">{response.Response.PatientMobileNumber}</span></div>" +
                                            $"</div>" +
                                            $"<div class=\"text-end col-12 col-md-6\">" +
                                                $"<div class=\"py-1\"><span class=\"text-info\">Gender: </span><span class=\"fw-bold\"> {response.Response.Gender}</span></div>" +
                                                $"<div class=\"py-1\"><span class=\"text-info\">Weight: </span><span class=\"fw-bold\"> {response.Response.Weight}kg</span></div>" +
                                                $"<div class=\"py-1\"><span class=\"text-info\">Age: </span><span class=\"fw-bold\"> {response.Response.Age} yrs</span></div>" +
                                            $"</div>" +
                                        $"</div>" +
                                        $"<div class=\"py-2r\">" +
                                            $"<span class=\"text-info\">Diseases: </span>" +
                                            $"<span class=\"fw-bold w-400px word-break\">{string.Join(", ", response.Response.Diseases)} " +

                                            $"</span>" +
                                        $"</div>" +
                                        $"<div class=\"flex-grow-1 d-flex flex-column template-preview-table\">" +
                                            $"<div>" +
                                                $"<div class=\"d-flex col-12\">" +
                                                    $"<div class=\"text-info text-start pb-3 col-5\"><small>Medicine Name</small></div>" +
                                                    $"<div class=\"text-info text-center pb-3 px-4 col-2\"><small>Dosage</small></div>" +
                                                    $"<div class=\"text-info pb-3 px-4 col-4\"><small>Timing</small></div>" +
                                                    $"<div class=\"text-info text-center pb-3 col-1\"><small>Days</small></div>" +
                                                $"</div>" +
                                            $"</div>" +
                                            $"<div class=\"fw-bold\">" +
                                                $"{PrescriptionMedicineList(response.Response.PrescriptionMedicine)}" +
                                            $"</div>" +
                                        $"</div>" +
                                        $"<div class=\"text-start text-info py-2r\">" +
                                            $"<small>Prescription Expiry: {Convert.ToDateTime(response.Response.PrescriptionExpiryDate):dd MMM yyyy}</small>" +
                                        $"</div>" +
                                    $"</div>" +
                                 $"</body>" +

                                 $"</html>";
            return htmlString;
        }

        public string GenerateUploadedTemplate(Prescription prescription, List<string> diseases)
        {
            var uploadedHtmlString = $"<!DOCTYPE html>" +
                                 $"<html lang = \"en\">" +

                                 $"<head>" +
                                    $"<meta charset=\"UTF-8\">" +
                                    $"<meta http-equiv=\"X-UA-Compatible\" content = \"IE=edge\">" +
                                    $"<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">" +
                                    $"<title>Prescription</title>" +
                                     "<style>" +
                                       "@import url(https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500&display=swap);@media all{body" +
                                       "{font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Oxygen,Ubuntu,Cantarell,'Open Sans','Helvetica Neue'," +
                                       "sans-serif}img{max-width:100%;height:auto}.d-flex{display:-webkit-box;display:-ms-flexbox;display:flex}.flex-grow-1" +
                                       "{-webkit-box-flex:1;-ms-flex-positive:1;flex-grow:1}.flex-shrink-0{-ms-flex-negative:0;flex-shrink:0}.line-height-normal" +
                                       "{line-height:1}.fs-4{font-size:24px}.m-0{margin:0}.mx-auto{margin-left:auto;margin-right:auto}.py-2r" +
                                       "{padding-top:2rem;padding-bottom:2rem}.text-wrap-all{overflow-wrap:anywhere}.text-wrap{overflow-wrap:break-word}.text-start" +
                                       "{text-align:start!important}.text-info{color:#76969e}.text-primary{color:#0e333a}.w-100{width:100%}}" +
                                    "</style>" +
                                 $"</head>" +

                                 $"<body class=\"text-primary\">" +
                                         $"<h2 class=\"text-wrap-all line-height-normal fs-4\">{string.Join(", ", diseases)}</h2>" +
                                         $"<figure class=\"d-flex m-0 mx-auto\">" +
                                               $"<img class=\"mx-auto\" src= {prescription.PrescriptionUpload.PrescriptionFilePath} alt=\"\">" +
                                         $"</figure>" +
                                         $"<div class=\"text-info py-2r\">Prescription Expiry: {Convert.ToDateTime(prescription.ExpiryDate):dd MMM yyyy} </div>" +
                                 $"</body>" +

                                 $"</html>";
            return uploadedHtmlString;
        }
    }
}