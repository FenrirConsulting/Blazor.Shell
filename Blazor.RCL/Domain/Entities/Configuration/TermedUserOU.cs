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
            { "caremarkrx.net", "OU=Terminated Users,DC=caremarkrx,DC=net" },
            { "corp.Companycaremark.com", "OU=DeprovisionedAccounts,DC=Corp,DC=Companycaremark,DC=com" },
            { "corp.Company.com", "OU=Disabled Users and Computers,DC=Corp,DC=Company,DC=com" },
            { "minclinic.local", "OU=TerminatedUsers,DC=minclinic,DC=local" },
            { "coe.caremarkrx.net", "OU=Terminated Users,DC=coe,DC=caremarkrx,DC=net" },
            { "corp.omnicare.com", "OU=Terminated Users,DC=corp,DC=omnicare,DC=com" },
            { "test.Companycaremark.com", "OU=Terminated Users,DC=test,DC=Companycaremark,DC=com" },
            { "aeth.aetna.com", "OU=Terminated Users,DC=aeth,DC=aetna,DC=com" }
        };

        public static string GetPurgeUsersOU(string domain)
        {
            return DomainToPurgeUsersOU.TryGetValue(domain, out var purgeOU) ? purgeOU : "DefaultOU"; // DefaultOU can be a fallback
        }
    }
}
