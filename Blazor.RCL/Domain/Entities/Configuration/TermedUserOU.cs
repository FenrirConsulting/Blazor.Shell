using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.RCL.Domain.Entities.Configuration
{
    public static class TermedUserOU
    {
        public static readonly Dictionary<string, string> DomainToPurgeUsersOU = new Dictionary<string, string>
        {
            { "Companyrx.net", "OU=Terminated Users,DC=Companyrx,DC=net" },
            { "corp.CompanyCompany.com", "OU=DeprovisionedAccounts,DC=Corp,DC=CompanyCompany,DC=com" },
            { "corp.Company.com", "OU=Disabled Users and Computers,DC=Corp,DC=Company,DC=com" },
            { "minclinic.local", "OU=TerminatedUsers,DC=minclinic,DC=local" },
            { "coe.Companyrx.net", "OU=Terminated Users,DC=coe,DC=Companyrx,DC=net" },
            { "corp.omnicare.com", "OU=Terminated Users,DC=corp,DC=omnicare,DC=com" },
            { "test.CompanyCompany.com", "OU=Terminated Users,DC=test,DC=CompanyCompany,DC=com" },
            { "aeth.Company.com", "OU=Terminated Users,DC=aeth,DC=Company,DC=com" }
        };

        public static string GetPurgeUsersOU(string domain)
        {
            return DomainToPurgeUsersOU.TryGetValue(domain, out var purgeOU) ? purgeOU : "DefaultOU"; // DefaultOU can be a fallback
        }
    }
}
