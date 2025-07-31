using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsideOutSellingSolutions.DTOs.AccountDTO;
using InsideOutSellingSolutions.DTOs.CommonDTO;


namespace InsideOutSellingSolutions.Repository
{
    public interface IAccountRepository
    {
        public void Registration(RegisterDTO registerDTOobj);
        public LoginResponseDTO Login(LoginDTO loginDTOobj);
        public List<ListOfUsersDTO> GetAllUsers();
        public List<Dropdown> GetAllRoles();

    }
}
