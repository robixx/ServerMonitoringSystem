using System;
using System.Collections.Generic;
using System.Text;

namespace SysMonitor.Application.IInterface
{
    public interface IAuth
    {
        Task<WebUserResponse> LoginWebUser(AuthenticationRequest request);
    }
}
