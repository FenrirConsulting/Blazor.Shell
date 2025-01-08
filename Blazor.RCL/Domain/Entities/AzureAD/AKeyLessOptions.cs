using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.RCL.Domain.Entities.AzureAD
{
    /// <summary>
    /// Represents the AKeyLess configuration options.
    /// </summary>
    public class AKeyLessOptions
    {
        /// <summary>
        /// Gets or sets the Access ID.
        /// </summary>
        public string? AccessID { get; set; }

        /// <summary>
        /// Gets or sets the Secret.
        /// </summary>
        public string? Secret { get; set; }

        /// <summary>
        /// Gets or sets the ACM1 Value
        /// </summary>
        public string? ACM1 { get; set; }

        /// <summary>
        /// Gets or sets the ACM2 Value
        /// </summary>
        public string? ACM2 { get; set; }

        /// <summary>
        /// Gets or sets the ACM1 Path Value
        /// </summary>
        public string? ACM1Path { get; set; }

        /// <summary>
        /// Gets or sets the ACM2 Path Value
        /// </summary>
        public string? ACM2Path { get; set; }
    }
}
