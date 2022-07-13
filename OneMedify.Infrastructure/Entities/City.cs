using System;
using System.Collections.Generic;

namespace OneMedify.Infrastructure.Entities
{
    public partial class City
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int StateId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDisable { get; set; }
        public virtual State State { get; set; }

        public virtual ICollection<Doctor> Doctor { get; set; }
        public virtual ICollection<Patient> Patient { get; set; }
        public virtual ICollection<Pharmacy> Pharmacy { get; set; }
    }
}
