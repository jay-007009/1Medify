using System;
using System.Collections.Generic;

namespace OneMedify.Infrastructure.Entities
{
        public partial class State
        {
            public State()
            {
                City = new HashSet<City>();
            }

            public int StateId { get; set; }
            public string StateName { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime? ModifiedDate { get; set; }
            public bool? IsDisable { get; set; }

            public virtual ICollection<City> City { get; set; }
        }
}
