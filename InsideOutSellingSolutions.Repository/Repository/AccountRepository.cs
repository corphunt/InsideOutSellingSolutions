using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using InsideOutSellingSolutions.DTOs.AccountDTO;
using InsideOutSellingSolutions.DTOs.CommonDTO;
using InsideOutSellingSolutions.Repository.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace InsideOutSellingSolutions.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IConfiguration _config;
        private readonly ICommonRepository _icommonRepository;
        public AccountRepository(IConfiguration config, ICommonRepository icommonRepository)
        {
            this._config = config;
            _icommonRepository = icommonRepository;
        }

        public string SqlConnection()
        {
            return _config.GetConnectionString("InsideOut_Database").ToString();
        }

        #region --- Registration ---
        public void Registration(RegisterDTO registerDTOobj)
        {
            try
            {
                using (var connection = new SqlConnection(SqlConnection()))
                {
                    SqlCommand cmd = new SqlCommand("Register_SP", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FullName", registerDTOobj.FullName);
                    cmd.Parameters.AddWithValue("@MobileNo", registerDTOobj.MobileNo);
                    cmd.Parameters.AddWithValue("@Address", registerDTOobj.Address);                
                    cmd.Parameters.AddWithValue("@RoleId", registerDTOobj.RoleId);
                    cmd.Parameters.AddWithValue("@EmailId", registerDTOobj.EmailId);
                    cmd.Parameters.AddWithValue("@UserName", registerDTOobj.UserName);
                    cmd.Parameters.AddWithValue("@Password", ComputeSha256Hash(registerDTOobj.Password));            

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                _icommonRepository.Errorlog("Register_SP", ex.StackTrace, ex.Message);
                throw;
            }
        }

        #endregion

        #region --- Login ---
        public LoginResponseDTO Login(LoginDTO loginDTOobj)
        {
            LoginResponseDTO loginresponseDTO = new LoginResponseDTO();
            try
            {
                using (var connection = new SqlConnection(SqlConnection()))
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand("Login_SP", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_LoginId", loginDTOobj.LoginId);
                    cmd.Parameters.AddWithValue("p_Password", ComputeSha256Hash(loginDTOobj.Password));

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        loginresponseDTO.FullName = dt.Rows[0]["FullName"].ToString();
                        loginresponseDTO.Message = dt.Rows[0]["Status"].ToString();
                        loginresponseDTO.UserName = dt.Rows[0]["UserName"].ToString();
                        loginresponseDTO.Role = dt.Rows[0]["RoleName"].ToString();
                        loginresponseDTO.RoleId = dt.Rows[0]["RoleId"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["RoleId"]) : (int?)null;

                        return loginresponseDTO;
                    }
                    else
                    {
                        loginresponseDTO.Message = "Oops! Some error occured during Login...!! ";
                        return loginresponseDTO;
                    }
                }
            }
            catch (Exception ex)
            {
                _icommonRepository.Errorlog("Login_SP", ex.StackTrace, ex.Message);
                throw;
            }
        }
        #endregion

        #region --- Users Listing ---
        public List<ListOfUsersDTO> GetAllUsers()
        {
            try
            {
                using (var connection = new SqlConnection(SqlConnection()))
                {
                    List<ListOfUsersDTO> UserList = new List<ListOfUsersDTO>();
                    SqlCommand cmd = new SqlCommand("ListOfUsers_SP", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    DataTable dt = new DataTable();
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        ListOfUsersDTO list = new ListOfUsersDTO();
                        list.RegId = Convert.ToInt32(dr["RegId"]);
                        list.FullName = Convert.ToString(dr["FullName"]);
                        list.MobileNo = Convert.ToString(dr["MobileNo"]);
                        list.Address = Convert.ToString(dr["Address"]);                        
                        list.Role = Convert.ToString(dr["RoleName"]);
                        list.EmailId = Convert.ToString(dr["EmailId"]);
                        list.UserName = Convert.ToString(dr["UserName"]);
                        list.Password = Convert.ToString(dr["Password"]);
                        UserList.Add(list);
                    }
                    return UserList;
                }
            }
            catch (Exception ex)
            {
                _icommonRepository.Errorlog("ListOfUsers_SP", ex.StackTrace, ex.Message);
                throw;
            }
        }
        #endregion

        #region --- Roles Listing ---
        public List<Dropdown> GetAllRoles()
        {
            try
            {
                using (var connection = new SqlConnection(SqlConnection()))
                {
                    List<Dropdown> RoleList = new List<Dropdown>();
                    SqlCommand cmd = new SqlCommand("ListOfMasterRoles_SP", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    DataTable dt = new DataTable();
                    SqlDataAdapter adp = new SqlDataAdapter(cmd);
                    adp.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        Dropdown ddp = new Dropdown();
                        ddp.ID = Convert.ToInt32(dr["RoleId"]);
                        ddp.Name = Convert.ToString(dr["RoleName"]);
                        RoleList.Add(ddp);
                    }
                    return RoleList;
                }
            }
            catch (Exception ex)
            {
                _icommonRepository.Errorlog("ListOfRoles_SP", ex.StackTrace, ex.Message);
                throw;
            }
        }
        #endregion

        #region --- Plain Text To Hashed Password Conversion Code ---
        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256 hash object
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute the hash as a byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a hex string
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
        #endregion
    }
}
