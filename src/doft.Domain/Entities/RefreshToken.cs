using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doft.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } // Store securely (hashed)
        public string UserId { get; set; } // FK to User
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
        public string PreviousToken { get; set; } // Optional: FK to previous refresh token
        
    }

}