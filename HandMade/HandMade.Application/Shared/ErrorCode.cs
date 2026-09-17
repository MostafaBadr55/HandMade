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

        //Role
        NoRolesFound= 2001,
        ThisUserAlreadyHasThisRole = 2002,
        RoleAddingFaild=2003,
        FailedToUpdateSellerBool = 2004,
        RoleAlreadyExist= 2005,
        InvalidRoleName=2006,

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
        DefaultAddressNotAdded = 4003,
        AddressNotFound = 4004,
        AddressNotOwnedByUser = 4005,

        // File Upload
        NoFileProvided = 6001,
        FileTooLarge = 6002,
        InvalidFileExtension = 6003,
        FileNotFound = 6004,

        //Shop
        ShopNotFound = 7001,
        ShopNotPending = 7002,
        ThisOwnerAlreadyHasAShop = 7003,
        UserDoesNotOwnThisShop = 7004,

        //Product
        ProductNotFound = 8001,
        productNotPending = 8002,
        ProductDoesNotBelongToThisShop = 8003,
        ProductNotApproved = 8004,
        ProductAccessDenied = 8005,
        //ProductImages
        FailedToLoadProductImages = 9001,

        //Category & Subcategory 
        CategoryNotFound = 10001,
        SubCategoryNotFound = 10002,
        CategoryNameAlreadyExists = 10003,
        SubCategoryNameAlreadyExistsInCategory = 10004,
        //Cart
        CartNotFound = 11001,
        CartItemNotFound = 11002,
        CartIsEmpty = 11003,
        ProductNotPurchasable = 11004,
        InvalidQuantity = 11005,
        CartItemAlreadyConverted = 11006,

        //Payment & Escrow
        PaymentNotFound = 13001,
        PaymentFailed = 13002,
        PaymentAlreadyCompleted = 13003,
        EscrowNotHeld = 13004,
        EscrowAlreadyReleased = 13005,

    }
}
