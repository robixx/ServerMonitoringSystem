using System;
using System.Collections.Generic;
using System.Text;

namespace SysMonitor.Application
{
    public class AuthenticationRequest
    {
        public string? UserName { get; set; } = string.Empty;
        public string? Password { get; set; }=string.Empty;
    }
}
