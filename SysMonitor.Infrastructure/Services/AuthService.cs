using SysMonitor.Application;
using SysMonitor.Application.IInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace SysMonitor.Infrastructure.Services
{
    public class AuthService : IAuth
    {
        private readonly DatabaseConnection _databaseconnection;
        public AuthService(DatabaseConnection databaseconnection)
        {
            _databaseconnection = databaseconnection;
        }
        public async Task<WebUserResponse> LoginWebUser(AuthenticationRequest request)
        {
            try
            {
                if(request.UserName=="robi" && request.Password == "123")
                {
                    var response = new WebUserResponse
                    {
                        EmployeeId=519,
                        DispalyName="Shoriful Islam Robi",
                        UserId=18475,
                        RoleName="SuperAdmin",
                        RoleId=2
                    };
                    return response;
                }
                return new WebUserResponse();

            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
