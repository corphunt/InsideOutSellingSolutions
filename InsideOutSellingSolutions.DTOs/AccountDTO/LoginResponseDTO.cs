using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsideOutSellingSolutions.DTOs.AccountDTO
{
    public class LoginResponseDTO
    {
        public string Message { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public int? RoleId { get; set; } // <-- Newly Added

    }
}
