using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace StoreClassLibrary
{
    [DataContract(Name = "cartItemDisplays")]
    public class CartItemDisplay
    {
        [DataMember(Name = "item")]
        public CartItem Item { get; set; }

        [DataMember(Name = "itemDescription")]
        [Display(Name = "Product")]
        public string ItemDescription { get; set; }

        public CartItemDisplay()
        {
        }

        public CartItemDisplay(CartItem items, string itemDescriptions)
        {
            Item = items;
            ItemDescription = itemDescriptions;
        }
    }
}