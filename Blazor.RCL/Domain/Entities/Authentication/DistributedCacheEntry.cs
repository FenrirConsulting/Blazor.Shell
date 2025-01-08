using System;
using System.ComponentModel.DataAnnotations;

namespace Blazor.RCL.Domain.Entities.Authentication
{
    /// <summary>
    /// Represents an entry in a distributed cache system.
    /// This entity defines the structure for the DistributedCacheEntry table in the database.
    /// </summary>
    public class DistributedCacheEntry
    {
        #region Properties

        /// <summary>
        /// Unique identifier for the cache entry. This is typically a key used to retrieve the cached item.
        /// </summary>
        [Key]
        [StringLength(449)]
        public string? Id { get; set; }

        /// <summary>
        /// The cached data stored as a byte array.
        /// </summary>
        [Required]
        public byte[]? Value { get; set; }

        /// <summary>
        /// The exact time when this cache entry expires.
        /// </summary>
        [Required]
        public DateTimeOffset ExpiresAtTime { get; set; }

        /// <summary>
        /// The sliding expiration time in seconds. If set, the expiration of the entry 
        /// is extended by this amount each time it's accessed.
        /// </summary>
        public long? SlidingExpirationInSeconds { get; set; }

        /// <summary>
        /// The absolute expiration time of the cache entry, regardless of access.
        /// </summary>
        public DateTimeOffset? AbsoluteExpiration { get; set; }

        #endregion
    }
}