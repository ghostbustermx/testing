using System;
using System.Collections.Generic;

namespace Locus.Core.DTO
{
    public class ValueObject
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public object MiddleName { get; set; }
        public bool IsActive { get; set; }
        public int EmployeeNumber { get; set; }
        public DateTime? HireDate { get; set; }
        public string Gender { get; set; }
        public string JobTitle { get; set; }
        public string Division { get; set; }
        public string PhotoUrl { get; set; }
        public string Department { get; set; }
        public int BambooHrId { get; set; }
        public string Alias { get; set; }
        public DateTime? PerformanceDate { get; set; }
        public string FullName { get; set; }
        public List<string> PermissionRoles { get; set; }
        public object Roles { get; set; }
    }

    public class EmployeeInfoDTO
    {
        public int StatusCode { get; set; }
        public ValueObject ValueObject { get; set; }
    }

}
