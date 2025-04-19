
using FreelanceBirga.Models.DB;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FreelanceBirga.Models.VM
{
    public class OrdersModerationViewModel
    {
        public List<TempOrder> Orders { get; set; }
    }
}
