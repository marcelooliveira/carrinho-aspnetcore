using System.Collections.Generic;

namespace Carrinho.Core.DTOs
{
    public class CheckoutSummaryDTO
    {
        public string OrderNumber { get; set; }
        //[DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Total { get; set; }
        public int DeliveryUpToNWorkingDays { get; set; }

        public string DeliveryUpTo
        {
            get
            {
                return string.Format("{0} {1}", DeliveryUpToNWorkingDays, 
                    DeliveryUpToNWorkingDays == 1 ? " dia útil" : " dias úteis");
            }
        }

        public CustomerInfoDTO CustomerInfo { get; set; }
        public List<CartItemDTO> CartItems { get; set; }
    }
}
