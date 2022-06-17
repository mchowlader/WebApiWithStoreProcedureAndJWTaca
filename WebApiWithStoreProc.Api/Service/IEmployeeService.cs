using System.Collections.Generic;
using WebApiWithStoreProc.Api.Model;

namespace WebApiWithStoreProc.Api.Service
{
    public interface IEmployeeService
    {
        List<Employee> GetByEmployee(int id);
        ServiceResponse<Employee> AddEmployee(Employee emp);
        ServiceResponse<Employee> APREST_UpdateEmployee(Employee emp, int id);
    }
}