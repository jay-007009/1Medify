using FluentValidation;
using OneMedify.DTO.Prescription;
using OneMedify.Resources;
using System.Collections.Generic;
using System.Linq;

namespace OneMedify.Shared.Services
{
    public class PrescriptionMedicineValidator : AbstractValidator<PrescriptionMedicineDto>
    {
        public PrescriptionMedicineValidator()
        {
            RuleFor(x => x.MedicineId).NotEmpty().WithMessage(PrescriptionResource.FieldRequired);

            RuleFor(x => x.AfterBeforeMeal).NotNull().WithMessage(PrescriptionResource.FieldRequired);

            RuleFor(x => x.MedicineDays).Cascade(CascadeMode.StopOnFirstFailure)
                                        .NotNull().WithMessage(PrescriptionResource.FieldRequired)
                                        .InclusiveBetween(2, 180).WithMessage(PrescriptionResource.InvalidMedicineDays);

            RuleFor(x => x.MedicineDosage).Cascade(CascadeMode.StopOnFirstFailure)
                                          .NotNull().WithMessage(PrescriptionResource.FieldRequired)
                                          .InclusiveBetween(1, 2).WithMessage(PrescriptionResource.InvalidMedicineDosage);

            RuleFor(x => x.MedicineTiming).Cascade(CascadeMode.StopOnFirstFailure)
                                          .NotEmpty().WithMessage(PrescriptionResource.FieldRequired)
                                          .Must(IsValidMedicineTime).WithMessage(PrescriptionResource.InvalidMedicineTiming);
        }


        //To check passed medicine timing are in acceptable format.
        private bool IsValidMedicineTime(List<string> medicineTimings)
        {
            List<string> sequencedTiming = new List<string> { "Mo", "Af", "Ev", "Ni" };

            foreach (var medicineTiming in medicineTimings)
            {
                if (!sequencedTiming.Contains(medicineTiming))
                {
                    return false;
                }
            }

            //Check there is atleast min: one timmimg and max:four timming
            if (medicineTimings.Count < 1 && medicineTimings.Count > 4)
            {
                return false;
            }

            //Check same timmings are not passed multiple time.
            List<string> timings = new List<string>();
            foreach (var time in medicineTimings)
            {
                if (timings.Contains(time))
                {
                    return false;
                }
                timings.Add(time);
            }

            //Check given item are in sequence as mentioned in "sequencedTiming" list.
            int i = 0, j = 0;
            while (i < sequencedTiming.Count() && j < medicineTimings.Count())
            {
                if (sequencedTiming[i] == medicineTimings[j])
                {
                    i++;
                    j++;
                } 
                else
                {
                    i++;
                }
            }

            return medicineTimings.Count() == j;
        }
    }
}
