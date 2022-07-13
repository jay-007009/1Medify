using OneMedify.DTO.Response;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Entities;
using OneMedify.Resources;
using OneMedify.Services.Contracts;
using OneMedify.Shared.Contracts;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OneMedify.Services.Services
{
    public class DoctorChangeForPrescriptionService : IDoctorChangeForPrescriptionService
    {
        private readonly IDoctorActionLogRepository _doctorActionLogRepository;
        private readonly IEmailService _emailService;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPrescriptionRepository _prescriptionRepository;

        public DoctorChangeForPrescriptionService(IDoctorActionLogRepository doctorActionLogRepository, IEmailService emailService,
                                                  IDoctorRepository doctorRepository, IPrescriptionRepository prescriptionRepository)
        {
            _doctorActionLogRepository = doctorActionLogRepository;
            _emailService = emailService;
            _doctorRepository = doctorRepository;
            _prescriptionRepository = prescriptionRepository;
        }

        public async Task DoWork(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

                    var prescriptionsToAssignNextDoctor = _doctorActionLogRepository.GetPendingDoctorActionLogs();
                    foreach (var prescription in prescriptionsToAssignNextDoctor)
                    {
                        var checkPrescription = await _prescriptionRepository.ReadById(prescription.PrescriptionId);
                        if (checkPrescription.PrescriptionStatus == 2)
                        {
                            //Remove 1st doctor and add at last position
                            var newdoctorList = prescription.DoctorList.Split(",").Select(int.Parse).ToList();
                            newdoctorList.Remove(prescription.DoctorId);
                            newdoctorList.Add(prescription.DoctorId);
                            prescription.DoctorList = String.Join(",", newdoctorList);

                            //Code to insert action log
                            DoctorActionLog actionLog = new DoctorActionLog
                            {
                                PrescriptionId = prescription.PrescriptionId,
                                PrescriptionStatus = prescription.PrescriptionStatus,
                                DoctorId = newdoctorList[0],
                                ActionTimeStamp = DateTime.Now,
                                DoctorList = String.Join(",", newdoctorList)
                            };
                            var doctorActionLog = await _doctorActionLogRepository.Create(actionLog);

                            var doctor = await _doctorRepository.ReadById(actionLog.DoctorId);
                            await _emailService.SendEmailAsync(doctor.Email, DoctorResources.PrescriptionApprovalEmail, GetEmailMessage(prescription.PrescriptionId).Result);
                        }                                 
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Get EmailMessage for doctor
        /// </summary>
        private async Task<string> GetEmailMessage(int prescriptionId)
        {
            try
            {
                var prescription = await _prescriptionRepository.ReadById(prescriptionId);

                if (prescription.ModifiedByPharmacy != null)
                {
                    return DoctorResources.SentForApprovalByPharmacy;
                }
                return $"Patient({prescription.Patient.FirstName + " " + prescription.Patient.LastName}) has sent prescription for approval.";
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }           
        }
    }
}
