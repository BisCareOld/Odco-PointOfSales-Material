using Abp.AutoMapper;
using Odco.PointOfSales.Application.Finance.Payments.PaymentTypes;
using Odco.PointOfSales.Application.Inventory.NonInventoryProducts;
using Odco.PointOfSales.Application.Sales.SalesProducts;
using Odco.PointOfSales.Core.Enums;
using Odco.PointOfSales.Core.Sales;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Odco.PointOfSales.Application.Sales.Sales
{
    [AutoMapTo(typeof(Sale))]
    public class CreateOrUpdateSaleDto
    {
        public CreateOrUpdateSaleDto()
        {
            SalesProducts = new HashSet<CreateSalesProductDto>();
            NonInventoryProducts = new HashSet<CreateNonInventoryProductDto>();
            //PaymentDtos
            Cashes = new HashSet<CashDto>();
            Cheques = new HashSet<ChequeDto>();
            Outstandings = new HashSet<CustomerCreditOutstandingDto>();
            DebitCards = new HashSet<DebitCardDto>();
            GiftCards = new HashSet<GiftCardDto>();
        }

        /// <summary>
        /// Exist: Update
        /// Not Exist: Create
        /// </summary>
        public Guid? Id { get; set; }

        [StringLength(15)]
        public string SalesNumber { get; set; }

        [StringLength(15)]
        public string ReferenceNumber { get; set; }

        public Guid? CustomerId { get; set; }

        [StringLength(10)]
        public string CustomerCode { get; set; }

        [StringLength(100)]
        public string CustomerName { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TaxRate { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal GrossAmount { get; set; }

        public decimal NetAmount { get; set; }

        public decimal ReceivedAmount { get; set; }

        public decimal BalanceAmount { get; set; }

        [StringLength(100)]
        public string Remarks { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public bool IsActive { get; set; }

        public ICollection<CreateSalesProductDto> SalesProducts { get; set; }

        public ICollection<CreateNonInventoryProductDto> NonInventoryProducts { get; set; }

        #region Payment Types
        public ICollection<CashDto> Cashes { get; set; }
        public ICollection<ChequeDto> Cheques { get; set; }
        public ICollection<CustomerCreditOutstandingDto> Outstandings { get; set; }
        public ICollection<DebitCardDto> DebitCards { get; set; }
        public ICollection<GiftCardDto> GiftCards { get; set; }
        #endregion
    }
}
