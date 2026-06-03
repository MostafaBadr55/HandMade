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
        ThisUserAlreadyHasThisRole = 2002,
        RoleAddingFaild=2003,
        FailedToUpdateSellerBool = 2004,

        //Registration
        EmailAlreadyExists = 3001,
        UserNameAlreadyExists = 3002,
        UserNotCreated= 3003,

        //Login
        InvalidUsernameOrPassword = 5001,
        InActiveAccount = 5002,

        //Address
        LabelMustBeProvided = 4001,
        DetailedAddressNotProvided = 4002,
        DefaultAddressNotAdded = 4003
    }
}
