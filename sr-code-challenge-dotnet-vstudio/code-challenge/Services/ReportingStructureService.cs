using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using challenge.Repositories;

namespace challenge.Services
{
    /// <summary>
    /// Computes number of direct and indirect reports of an Employee, returns in a ReportingStructure class instance.
    /// Uses an instance of EmployeeRepository to query the employee reporting structure data.
    /// </summary>
    public class ReportingStructureService : IReportingStructureService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<ReportingStructureService> _logger;

        public ReportingStructureService(ILogger<ReportingStructureService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        /// <summary>
        /// For a given Employee Id, return a List<string> containing that employee's direct reports.
        /// </summary>
        /// <param name="employeeId">Id of employee to query </param>
        /// <returns>List<string> - list of direct report employee Id's </returns>
        protected List<string> GetReportIds(string employeeId)
        {
            List<string> reportIds = new List<string>();
            Employee emp = _employeeRepository.GetById(employeeId, true);

            if (emp == null)
            {
                return reportIds;
            }
                
            foreach (Employee e in emp.DirectReports)
            {
                reportIds.Add(e.EmployeeId);
            }

            return reportIds;
        }

        /// <summary>
        /// Recursively traverses the structure of reports of Employees, returnin a list of 
        /// unique employee Id for each direct and indirect report of the Employee with id 
        /// supplied at the first call of this function.
        /// </summary>
        /// <param name="empId">Employee Id to retrieve report IDs from</param>
        /// <param name="allReports">HashSet containing list of direct and indirect employee ID's</param>
        protected void AccumulateEmployeeReports(string empId, HashSet<string> allReports)
        {
            var reports = this.GetReportIds(empId);

            foreach (string e in reports)
            {
                allReports.Add(e); //HashSet rejects duplicates, does not throw exception for them.
                this.AccumulateEmployeeReports(e, allReports);
            }

        }

        /// <summary>
        /// Return an instance of ReportingStructure containing the number of direct and indirect reports for the given employee Id.
        /// </summary>
        /// <param name="id">Id of Employee to query</param>
        /// <returns>ReportingStructure instance</returns>
        public ReportingStructure GetByEmployeeId(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return null;
            }

            Employee root = _employeeRepository.GetById(id, true);

            if (root == null)
            {
                return null;
            }

            HashSet<string> allReports = new HashSet<string>();
            this.AccumulateEmployeeReports(id, allReports);

            ReportingStructure result = new ReportingStructure { EmployeeId = id, NumberOfReports = allReports.Count };

            return result;
        }
    }
}
