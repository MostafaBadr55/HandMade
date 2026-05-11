using HandMade.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HandMade.Infrastructure.Identity.IdentityModels
{
    public class IdentityAppUser: IdentityUser<Guid>
    {
        public bool IsSeller { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        #region nav Prop
        // Navigation Properties
        public ICollection<IdentityAppUserRole> UserRoles { get; set; }
        public ICollection<Shop> Shops { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
        public ICollection<UserNotification> UserNotifications { get; set; }
        public ICollection<Address> Addresses { get; set; }
        public ICollection<Dispute> Disputes { get; set; }
        public ICollection<ShopFollower> ShopFollowers { get; set; }
        public ICollection<Review> WrittenReviews { get; set; }  // reviews written by the user
        public ICollection<Review> ReceivedReviews { get; set; } // reviews about a buyer

        public Cart Cart { get; set; }
        public ICollection<CartItem> CartItems { get; set; }

        #endregion
    }
}
