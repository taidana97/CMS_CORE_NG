using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ModelService
{
    /// <summary>
    /// Application Role
    /// </summary>
    public class ApplicationRole : IdentityRole
    {
        /// <summary>Role name</summary>
        public string RoleName { get; set; }
        /// <summary>Role icon</summary>
        public string RoleIcon { get; set; }
        /// <summary>Handle</summary>
        public string Handle { get; set; }
        /// <summary>Is active</summary>
        public bool IsActive { get; set; }
        /// <summary>Permissions</summary>
        public ICollection<RolePermission> Permissions { get; set; }
    }
}
