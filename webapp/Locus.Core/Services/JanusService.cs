using Locus.Core.DTO;
using Locus.Core.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Configuration;

namespace Locus.Core.Services
{

    public interface IJanusService
    {

        EmployeeInfoDTO GetEmployeeInfoDTO(string user);
        EmployeesDTO GetEmployees();

    }


    public class JanusService : IJanusService
    {


        public EmployeeInfoDTO GetEmployeeInfoDTO(string userName)
        {

            string imagePath = "/Content/img/default-user.png";
            string API = WebConfigurationManager.AppSettings["Janus"];
            APIRequestor myRequest = new APIRequestor(String.Format("{0}{1}", API, "Employee/GetEmployeeByUserName?"), "GET", "UserName=" + userName);
            var json = myRequest.GetResponse();
            if (json == null)
            {
                EmployeeInfoDTO employee = new EmployeeInfoDTO
                {
                    ValueObject = new ValueObject
                    {
                        PhotoUrl = imagePath,
                        FullName = userName,
                        FirstName= userName
                    }
                };
                return employee;
            }

            try
            {
                EmployeeInfoDTO user = JsonConvert.DeserializeObject<EmployeeInfoDTO>(json);
                if (user.ValueObject.PhotoUrl == null)
                {
                    user.ValueObject.PhotoUrl = imagePath;
                }
                return user;
            }
            catch
            {

                EmployeeInfoDTO employee = new EmployeeInfoDTO
                {
                    ValueObject = new ValueObject
                    {
                        PhotoUrl = imagePath,
                        FullName = userName,
                        FirstName = userName
                    }
                };
                return employee;
            }

        }

        public EmployeesDTO GetEmployees()
        {
            string API = WebConfigurationManager.AppSettings["Janus"];
            APIRequestor myRequest = new APIRequestor(String.Format("{0}{1}", API, "Employee/GetActiveEmployees"), "GET");
            var json = myRequest.GetResponse();
            EmployeesDTO employees = JsonConvert.DeserializeObject<EmployeesDTO>(json);
            return employees;
        }
    }
}


