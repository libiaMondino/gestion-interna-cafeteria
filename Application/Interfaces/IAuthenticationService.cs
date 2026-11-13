using System;
using System.Collections.Generic;
using System.Text;
using Application.Models;
using Application.Models.Users;
namespace Application.Interfaces
{
    public interface IAuthenticationService
    {
        string Authenticate(CredentialsLoginDto credentials);
    }
}
