using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PDM.Model
{
    public class Order : Base
    {
        public int OrderId { get; set; }
        public int CompanyId { get; set; }
        public int BranchId { get; set; }
        public int CustomerId { get; set; }
        public int OrderNo { get; set; }
        [DisplayName("Order ID")]
        public string OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        [DisplayName("Delivery Date")]
        [Required(ErrorMessage = "Delivery Date is required")]
        public DateTime? InputDeliveryDate { get; set; }
        [DisplayName("Title")]
        [Required(ErrorMessage = "Title is required")]
        public int OrderTypeId { get; set; }
        [DisplayName("Pickup Type")]
        [Required(ErrorMessage = "Pickup Type is required")]
        public int PickupTypeId { get; set; }
        [DisplayName("Delivery Note")]
        public string DeliveryNote { get; set; }
        [DisplayName("Branch Note")]
        public string BranchNote { get; set; }
        Customer _customer;
        public Customer Customer
        {
            get
            {
                return _customer;
            }
            set
            {
                _customer = value;
            }

        }
    }
}
