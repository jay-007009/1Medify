using Microsoft.EntityFrameworkCore;
using OneMedify.DTO.Doctor;
using OneMedify.DTO.Prescription;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Data;
using OneMedify.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneMedify.Infrastructure.Repositories
{
    public class DoctorActionLogRepository : IDoctorActionLogRepository
    {
        private enum PrescriptionStatus
        {
            Approved = 1,
            Pending,
            Rejected
        }

        private readonly OneMedifyDbContext _oneMedifyDbContext;

        public DoctorActionLogRepository(OneMedifyDbContext oneMedifyDbContext)
        {
            _oneMedifyDbContext = oneMedifyDbContext;
        }

        public DoctorActionLog GetPendingPrescription(int prescriptionId, int doctorId)
        {
            try
            {
                return _oneMedifyDbContext.DoctorActionLogs.Where(p => p.PrescriptionId == prescriptionId && p.DoctorId == doctorId).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Get Pending prescriptions DoctorActivityLogs with latest assigned doctor.
        /// </summary>
        public List<DoctorActionLog> GetPendingDoctorActionLogs()
        {
            try
            {
                var prescriptionsToAssignNextDoctor = _oneMedifyDbContext.DoctorActionLogs.Where(x => x.PrescriptionStatus == 2
                                                      && x.Prescription.PrescriptionStatus == 2).ToList()
                                                      .GroupBy(x => x.PrescriptionId, (x, y) => new DoctorActionLog
                                                      {
                                                            PrescriptionId = x,
                                                            DoctorActionLogId = y.OrderByDescending(z => z.ActionTimeStamp)
                                                                .Select(s => s.DoctorActionLogId).FirstOrDefault(),
                                                            ActionTimeStamp = y.OrderByDescending(z => z.ActionTimeStamp)
                                                                .Select(s => s.ActionTimeStamp).FirstOrDefault(),
                                                            DoctorId = y.OrderByDescending(z => z.ActionTimeStamp)
                                                                .Select(s => s.DoctorId).FirstOrDefault(),
                                                            PrescriptionStatus = y.OrderByDescending(z => z.ActionTimeStamp)
                                                                .Select(s => s.PrescriptionStatus).FirstOrDefault(),
                                                            DoctorList = y.OrderByDescending(z => z.ActionTimeStamp)
                                                                .Select(s => s.DoctorList).FirstOrDefault()
                                                      }).Where(dal => dal.ActionTimeStamp.AddMinutes(10) < DateTime.Now).ToList();

                return prescriptionsToAssignNextDoctor;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Create doctor action log
        /// </summary>
        public async Task<DoctorActionLog> Create(DoctorActionLog doctorActionLog)
        {
            try
            {
                await _oneMedifyDbContext.DoctorActionLogs.AddAsync(doctorActionLog);
                await _oneMedifyDbContext.SaveChangesAsync();
                return doctorActionLog;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<Doctorcount10> DoctorIdCount()
        {
            try
            {
                var actionLog = _oneMedifyDbContext.DoctorActionLogs
                    .Where(status => status.PrescriptionStatus == 1 && status.PrescriptionStatus == 3).GroupBy(d => d.DoctorId)
                    .Select(d => new Doctorcount10
                    {
                        DoctorId = (d.Key),
                        Count = d.Count()
                    }).OrderByDescending(max => max.Count).Take(10).ToList();
                return actionLog;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public async Task<List<DoctorActionLog>> GetPendingPrescriptionById(int prescriptionId)
        {
            try
            {
                var result = await _oneMedifyDbContext.DoctorActionLogs.Where(p => p.PrescriptionId == prescriptionId && p.PrescriptionStatus == 2).ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Method to Check If Doctor's Mobile Number Exist In Database Or Not.
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        public async Task<bool> IsDoctorMobileNumberExistsAsync(string mobileNumber)
        {
            try
            {
                var doctor = await _oneMedifyDbContext.Doctors.Where(doctor => doctor.MobileNumber == mobileNumber && doctor.IsDisable == false).FirstOrDefaultAsync();
                if (doctor != null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public void Delete(DoctorActionLog doctorActionLog)
        {
            try
            {
                _oneMedifyDbContext.DoctorActionLogs.Remove(doctorActionLog);
                _oneMedifyDbContext.SaveChanges();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Get Pending Prescriptions
        /// </summary>
        public async Task<List<DoctorActionLog>> GetPendingPrescriptions()
        {
            try
            {
                var doctorActionLogs = _oneMedifyDbContext.DoctorActionLogs.Where(x => x.PrescriptionStatus == (int)PrescriptionStatus.Pending
                                                     && x.Prescription.PrescriptionStatus == (int)PrescriptionStatus.Pending).ToList()
                                                      .GroupBy(x => x.PrescriptionId, (x, y) => new DoctorActionLog
                                                      {
                                                          PrescriptionId = x,
                                                          DoctorActionLogId = y.OrderByDescending(z => z.ActionTimeStamp)
                                                                .Select(s => s.DoctorActionLogId).FirstOrDefault(),
                                                          ActionTimeStamp = y.OrderByDescending(z => z.ActionTimeStamp)
                                                                .Select(s => s.ActionTimeStamp).FirstOrDefault(),
                                                          DoctorId = y.OrderByDescending(z => z.ActionTimeStamp)
                                                                .Select(s => s.DoctorId).FirstOrDefault(),
                                                          PrescriptionStatus = y.OrderByDescending(z => z.ActionTimeStamp)
                                                                .Select(s => s.PrescriptionStatus).FirstOrDefault(),
                                                          DoctorList = y.OrderByDescending(z => z.ActionTimeStamp)
                                                                .Select(s => s.DoctorList).FirstOrDefault()
                                                      }).ToList();
                return doctorActionLogs;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }



        /// <summary>
        /// Get Doctor List For Created Prescription by Doctor Id
        /// </summary>
        /// <param name="doctorId"></param>
        /// <returns></returns>

        public async Task<List<Dto>> GetDoctorListForCreatedPrescription(int doctorId)
        {
            try
            {
                return await _oneMedifyDbContext.DoctorActionLogs.Where(x => x.DoctorId != doctorId).GroupBy(x => x.DoctorId)
                            .Select(group => new Dto { DoctorId = group.Key, Count = group.Count() })
                            .OrderByDescending(x => x.Count).Take(9).ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// Get Doctor List For Uploaded Prescription by Doctor Id
        /// </summary>
        /// <param name="doctorId"></param>
        /// <returns></returns>

        public async Task<List<Dto>> GetDoctorListForUploadedPrescription()
        {
            try
            {
                return await _oneMedifyDbContext.DoctorActionLogs.GroupBy(x => x.DoctorId)
                            .Select(group => new Dto
                            {
                                DoctorId = group.Key,
                                Count = group.Count()
                            })
                            .OrderByDescending(x => x.Count).Take(10).ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}