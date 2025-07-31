using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsideOutSellingSolutions.Repository.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace InsideOutSellingSolutions.Repository.Repository
{
    public class CommonRepository : ICommonRepository
    {
        private readonly IConfiguration _config;
        public CommonRepository(IConfiguration config)
        {
            this._config = config;
        }
        public string SqlConnection()
        {
            return _config.GetConnectionString("InsideOut_Database").ToString();
        }
        public void Errorlog(string ProcedureName, string StackTrace, string ErrorMessage)
        {
            try
            {
                using (var connection = new SqlConnection(SqlConnection()))
                {
                    SqlCommand cmd = new SqlCommand("ErrorLog_SP", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("p_StoredProcedure", ProcedureName);
                    cmd.Parameters.AddWithValue("p_StackTrace", StackTrace);
                    cmd.Parameters.AddWithValue("p_Message", ErrorMessage);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Errorlog("ErrorLog_SP", ex.StackTrace, ex.Message);
                throw;
            }
        }
    }
}
