using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IPasswordHasherService
    {
        string HashUserPassword(string password);
        bool VerifyUserPassword(string password, string hashedPassword);
    }
}
