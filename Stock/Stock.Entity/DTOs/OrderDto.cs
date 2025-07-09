using Stock.Entity.Entities;
using Stock.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Entity.DTOs
{
    public class OrderDto
    {
        public DateTime OrderDate { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? ShipDate { get; set; }

        public OrderStatus Status { get; set; }

        public bool OnlineOrderFlag { get; set; }

        public string SalesOrderNumber { get; set; } = null!;

        public string? PurchaseOrderNumber { get; set; }

        public string? AccountNumber { get; set; }

        public int CustomerId { get; set; }

        public int? SalesPersonId { get; set; }

        public int? TerritoryId { get; set; }

        public int BillToAddressId { get; set; }

        public int ShipToAddressId { get; set; }

        public int ShipMethodId { get; set; }

        public int? CreditCardId { get; set; }

        public string? CreditCardApprovalCode { get; set; }

        public int? CurrencyRateId { get; set; }

        public decimal SubTotal { get; set; }

        public decimal TaxAmt { get; set; }

        public decimal Freight { get; set; }

        public decimal TotalDue { get; set; }

        public string? Comment { get; set; }

        public Guid Rowguid { get; set; }

        public DateTime ModifiedDate { get; set; }
        public IList<OrderDetailDto> OrderDetails { get; set; } = [];
    }

    public class OrderDetailDto
    {
        public int SalesOrderId { get; set; }
        public int SalesOrderDetailId { get; set; }
        public string? CarrierTrackingNumber { get; set; }
        public short OrderQty { get; set; }
        public int ProductId { get; set; }
        public int SpecialOfferId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitPriceDiscount { get; set; }
        public decimal LineTotal { get; set; }
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
