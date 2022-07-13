using System;
using System.Collections.Generic;
using System.Text;

namespace OneMedify.Shared.Services
{
    public static class AgeCalculator
    {
        public static string GetAge(DateTime dob)
        {
            DateTime today = DateTime.Today;

            int months = today.Month - dob.Month;
            int years = today.Year - dob.Year;

            if (today.Day < dob.Day)
            {
                months--;
            }

            if (months < 0)
            {
                years--;
                months += 12;
            }

            int days = (today - dob.AddMonths((years * 12) + months)).Days;

            return (years == 0) ? (months == 0) ? string.Format("{0} day{1}", days, (days == 1) ? "" : "s")
            : string.Format("{0} month{1}", months, (months == 1) ? "" : "s")
            : string.Format("{0} year{1}", years, (years == 1) ? "" : "s");
        }
    }
}
