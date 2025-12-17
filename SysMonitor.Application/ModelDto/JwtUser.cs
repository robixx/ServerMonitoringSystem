using System;
using System.Collections.Generic;
using System.Text;

namespace SysMonitor.Application
{
    public class JwtUser
    {
        public string? JWTId { get; set; }
        public long UserId { get; set; }
        public long EmployeeId { get; set; }
        public string? DispalyName { get; set; }
        public string? RoleName { get; set; }
        public int RoleId { get; set; }
        public DateTime? TokenExpired { get; set; }
    }
}
