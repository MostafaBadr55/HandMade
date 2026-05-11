using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Shared
{
    public enum ErrorCode
    {
        None= 0,
        
        //Authuntication
        
        UserNotFound = 1001,
        InvalidPassword = 1002,
        InvalidUserName = 1003,
        InvalidEmail = 1004,
        UsernameAlreadyExists = 1005,

        NoRolesFound= 2001,

        //Registration
        EmailAlreadyExists = 3001,
        UserNameAlreadyExists = 3002,
        UserNotCreated= 3003,
    }
}
