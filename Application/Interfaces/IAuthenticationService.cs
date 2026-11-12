using System;
using System.Collections.Generic;
using System.Text;
using Application.Models;
using Application.Models.Users;
namespace Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<string> Authenticate(CredentialsLoginDto credentials);
    }
}
