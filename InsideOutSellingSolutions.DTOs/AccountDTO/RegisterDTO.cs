using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsideOutSellingSolutions.DTOs.CommonDTO;

namespace InsideOutSellingSolutions.DTOs.AccountDTO
{
    public class RegisterDTO
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string MobileNo { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public int RoleId { get; set; }
        public IList<Dropdown> Roles { get; set; }
        [Required]
        public string EmailId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
