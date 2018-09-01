using System;
using System.ComponentModel.DataAnnotations.Schema;
using Bidster.Entities.Products;
using Bidster.Entities.Users;

namespace Bidster.Entities.Bids 
{
    public class Bid 
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        
        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }

        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public DateTime Timestamp { get; set; }

        public decimal Amount { get; set; }
    }
}