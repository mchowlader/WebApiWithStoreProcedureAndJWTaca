using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApiWithStoreProc.Api.Model;

namespace WebApiWithStoreProc.Api.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly string _connectionString;
        public EmployeeService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        public List<Employee> GetByEmployee(int id)
        {
            SqlConnection sql = new SqlConnection(_connectionString);
            sql.Open();

            var paramList = new SqlCommandParameters();
            paramList.Add("@id",id);
            var result_dt = ExecuteStoreProcedure("getById", paramList.List, sql);
            var result = JsonConvert.DeserializeObject<List<Employee>>(JsonConvert.SerializeObject(result_dt));

            return result;
        }

        public static DataTable ExecuteStoreProcedure(string sp_name, List<SqlCommandParameter> paramList, 
            SqlConnection connectionString)
        {
            if(connectionString.State != ConnectionState.Open)
            {
                connectionString.Open();
            }

            var cmd = new SqlCommand(sp_name);
            cmd.Connection = connectionString;

            if(paramList != null)
            {
                if(paramList.Count > 0)
                {
                    foreach(var item in paramList)
                    {
                        cmd.Parameters.AddWithValue(item.ParameterName, item.ParameterValue);
                    }
                }
            }

            DataTable dt = new DataTable();

            try
            {
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = cmd.ExecuteReader();
                dt.Load(rdr);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                cmd.Dispose();
                cmd.Connection.Close();

            }

            return dt;
        }

        public static object ExecuteStoredProcedureScalar(string sp_name, List<SqlCommandParameter> paramList,
            SqlConnection connectionString)
        {
            object value = null;
            var cmd = GetCommand(sp_name, paramList, connectionString);

            try
            {
                cmd.CommandType = CommandType.StoredProcedure;
                value = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                value = ex.Message;
            }
            finally
            {
                cmd.Dispose();
                cmd.Connection.Dispose();
            }

            return value;
        }
        public ServiceResponse<Employee> AddEmployee(Employee emp)
        {
            List<string> msgList = new List<string>();

            if (emp != null)
            {
                SqlConnection sql = new SqlConnection(_connectionString);
                var cmdParamsList = new List<SqlCommandParameter>();
                cmdParamsList.Add(SqlCommandParameter.AddParameters("@Id",emp.Id));
                cmdParamsList.Add(SqlCommandParameter.AddParameters("@Name", emp.Name));
                cmdParamsList.Add(SqlCommandParameter.AddParameters("@Email", emp.Email));
                cmdParamsList.Add(SqlCommandParameter.AddParameters("@Designation", emp.@Designation));

                foreach(var item in cmdParamsList.ToList())
                {
                    if(item.ParameterValue == DBNull.Value)
                    {
                        cmdParamsList.Remove(item);
                    }
                }
                msgList.Add("Employee saved.");

                var execute = ExecuteStoredProcedureScalar("Add_Employee", cmdParamsList, sql);

                emp.Id = Convert.ToInt32(execute);

            }
           
            return new ServiceResponse<Employee>
            {
                Data = emp,
                Message = msgList,
                Success = true
            };
        }

        public class SqlCommandParameter
        {
            public string ParameterName { get; set; }
            public object ParameterValue { get; set; }

            public SqlCommandParameter(string name, object  value)
            {
                ParameterName = name;
                ParameterValue = value == null ? DBNull.Value : value;
            }   

            public static SqlCommandParameter AddParameters(string parameterName, object parameterValue)
            {

                return new SqlCommandParameter(parameterName, parameterValue);
            }
        }

        public class SqlCommandParameters
        {
            public List<SqlCommandParameter> List { get; set; } = new List<SqlCommandParameter>();

            public void Add(string name, object value, bool nullable = false)
            {
                if(nullable || value != null)
                {
                    List.Add(new SqlCommandParameter(name, value));
                }
            }

            public void AddAll(List<(string, object)> parameters)
            {
                parameters.ForEach(param => Add(param.Item1, param.Item2));
            }
        }

        public static SqlCommand GetCommand(string sp_name, List<SqlCommandParameter> paramList,
            SqlConnection connectionString)
        {
            try
            {
                if (connectionString.State != ConnectionState.Open)
                {
                    connectionString.Open();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            SqlCommand cmd = new SqlCommand(sp_name);
            cmd.Connection = connectionString;

            if (paramList != null)
            {
                foreach (var item in paramList)
                {
                    cmd.Parameters.AddWithValue(item.ParameterName, item.ParameterValue);
                }
            }

            return cmd;
        }

        public ServiceResponse<Employee> APREST_UpdateEmployee(Employee emp, int id)
        {
            var msgList = new List<string>();
            if (emp != null)
            {
                var _UpdateEmployee = UpdateEmployee(emp, "UpdateEmployee" , id);

                if (_UpdateEmployee)
                {
                    msgList.Add("Customer updated.");
                }
                else
                {
                    msgList.Add("Customer update failed");
                }
            }
            return new ServiceResponse<Employee>
            {
                Data = emp,
                Message = msgList,
                Success = true
            };
        }

        public bool UpdateEmployee(Employee emp, string sp, int id)
        {
            var cmdParamsList = new List<SqlCommandParameter>();

            cmdParamsList.Add(SqlCommandParameter.AddParameters("Id", id));
            cmdParamsList.Add(SqlCommandParameter.AddParameters("@Name", emp.Name));
            cmdParamsList.Add(SqlCommandParameter.AddParameters("@Email", emp.Email));
            cmdParamsList.Add(SqlCommandParameter.AddParameters("@Designation", emp.Designation));
            
            foreach(var item in cmdParamsList.ToList())
            {
                if(item.ParameterValue == DBNull.Value)
                {
                    cmdParamsList.Remove(item); ;
                }
            }

            SqlConnection sql = new SqlConnection(_connectionString);

            if (sql.State != ConnectionState.Open)
            {
                sql.Open();
            }

            SqlCommand cmd = new SqlCommand(sp);
            cmd.Connection = sql;

            if(cmdParamsList != null)
            {
                if (cmdParamsList.Count > 0)
                {
                    foreach (var item in cmdParamsList)
                    {
                        cmd.Parameters.AddWithValue(item.ParameterName, item.ParameterValue);
                    }
                }
            }
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();

            return true;
        }
    }
}
