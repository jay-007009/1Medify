using OneMedify.Infrastructure.Data;
using OneMedify.Shared.Contracts;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace OneMedify.Shared.Services
{
    public class UserValidations : IUserValidations
    {
        private readonly OneMedifyDbContext _oneMedifyDbContext;

        public UserValidations(OneMedifyDbContext oneMedifyDbContext)
        {
            _oneMedifyDbContext = oneMedifyDbContext;
        }

        /// <summary>
        /// Method for Checking Email in Pharmacy,Doctor and Patient Table
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public virtual bool IsEmailAlreadyExists(string email)
        {
            try
            {
                var pharmacy = _oneMedifyDbContext.Pharmacies.Where(pharmacy => pharmacy.Email == email.ToLower() && pharmacy.IsDisable == false).FirstOrDefault();
                var doctor = _oneMedifyDbContext.Doctors.Where(doctor => doctor.Email == email.ToLower() && doctor.IsDisable == false).FirstOrDefault();
                var patient = _oneMedifyDbContext.Patients.Where(patient => patient.Email == email.ToLower() && patient.IsDisable == false).FirstOrDefault();
                return !(pharmacy == null && doctor == null && patient == null);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        /// Method for Checking Mobile Number in Pharmacy,Doctor and Patient Table
        /// </summary>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        public bool IsMobileNumberAlreadyExists(string mobileNumber)
        {
            try
            {
                var pharmacy = _oneMedifyDbContext.Pharmacies.Where(pharmacy => pharmacy.MobileNumber == mobileNumber && pharmacy.IsDisable == false).FirstOrDefault();
                var doctor = _oneMedifyDbContext.Doctors.Where(doctor => doctor.MobileNumber == mobileNumber && doctor.IsDisable == false).FirstOrDefault();
                var patient = _oneMedifyDbContext.Patients.Where(patient => patient.MobileNumber == mobileNumber && patient.IsDisable == false).FirstOrDefault();
                return !(pharmacy == null && doctor == null && patient == null);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public bool IsMobileNumberValid(string mobileNumber)
        {
            if (mobileNumber == null)
            {
                return true;
            }
            var pharmacy = _oneMedifyDbContext.Pharmacies.Where(pharmacy => pharmacy.MobileNumber == mobileNumber && pharmacy.IsDisable == false).Select(x => x.MobileNumber).FirstOrDefault();
            if (pharmacy == null)
            {
                return true;
            }
            return false;
        }

        public bool IsEmailValid(string email)
        {
            Regex regex = new Regex(@"^([a-zA-Z0-9]+[\.]*[a-zA-Z0-9]*)@([a-zA-Z0-9]+)((\.([org|net|com|in]){2,3})+)$");
            return regex.IsMatch(email.ToLower());
        }
        
    }
}