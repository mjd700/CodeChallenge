using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace challenge.Models
{
    public class Compensation
    {
        [System.ComponentModel.DataAnnotations.Key]
        public String EmployeeId { get; set; }
        public float Salary { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }
}
