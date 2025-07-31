using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsideOutSellingSolutions.DTOs.AccountDTO
{
    public class LoginDTO
    {
        [DisplayName("LoginId(UserName/Email)")]
        [Required]
        public string LoginId { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
