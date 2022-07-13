using FluentValidation;
using OneMedify.DTO.Prescription;
using OneMedify.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OneMedify.Shared.Services
{
    public class PrescriptionValidator : AbstractValidator<PrescriptionCreateDto>
    {
        public PrescriptionValidator()
        {
            RuleFor(x => x.DiseaseIds).Cascade(CascadeMode.StopOnFirstFailure)
                                      .NotEmpty().WithMessage(PrescriptionResource.FieldRequired)
                                      .Must(IsDiseaseListValid).WithMessage(PrescriptionResource.SameDiseaseIds);

            RuleFor(x => x.DoctorMobileNumber).Cascade(CascadeMode.StopOnFirstFailure)
                                              .NotEmpty().WithMessage(PrescriptionResource.FieldRequired)
                                              .Must(IsValidMobileNumber).WithMessage(PrescriptionResource.InvalidMobileFormat);

            RuleFor(x => x.PatientMobileNumber).Cascade(CascadeMode.StopOnFirstFailure)
                                               .NotEmpty().WithMessage(PrescriptionResource.FieldRequired)
                                               .Must(IsValidMobileNumber).WithMessage(PrescriptionResource.InvalidMobileFormat);

            RuleFor(x => x.ExpiryDate).Cascade(CascadeMode.StopOnFirstFailure)
                                      .NotEmpty().WithMessage(PrescriptionResource.FieldRequired)
                                      .Must(IsValidExpiryDate).WithMessage(PrescriptionResource.InvalidExpiryDate);

            RuleFor(x => x.Medicines).Cascade(CascadeMode.StopOnFirstFailure)
                                     .NotEmpty().WithMessage(PrescriptionResource.FieldRequired)
                                     .Must(IsMedicineListValid).WithMessage(PrescriptionResource.SameMedicineIds);

            RuleForEach(x => x.Medicines).SetValidator(new PrescriptionMedicineValidator());
        }


        //To validate mobile number regex
        private bool IsValidMobileNumber(string mobileNumber)
        {
            if (!Regex.IsMatch(mobileNumber, @"^[0-9]{10}$"))
            {
                return false;
            }         
            return true;
        }

        //To validate expiry date on accept future date and in format: yyyy-MM-dd
        private bool IsValidExpiryDate(string expiryDate)
        {
            if (!Regex.IsMatch(expiryDate, @"^\d{4}-\d{2}-\d{2}$"))
            {
                return false;
            }
            if (!(Convert.ToDateTime(expiryDate) > DateTime.Now && Convert.ToDateTime(expiryDate) <= DateTime.Now.AddDays(180)))
            {
                return false;
            }
            return true;
        }

        //Check same medicine id's are not passed multiple time.
        private bool IsMedicineListValid(List<PrescriptionMedicineDto> medicines)
        {
            foreach (var medicine in medicines)
            {
                if (medicines.Where(x => x.MedicineId == medicine.MedicineId).Count() > 1)
                {
                    return false;
                }
            }
            return true;
        }

        //Check same disease id's are not passed multiple time.
        private bool IsDiseaseListValid(List<int> diseaseIds)
        {
            List<int> disases = new List<int>();
            foreach (var diseaseId in diseaseIds)
            {
                if (disases.Contains(diseaseId))
                {
                    return false;
                }
                disases.Add(diseaseId);
            }
            return true;
        }
    }
}
