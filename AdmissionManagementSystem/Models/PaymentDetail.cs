using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AdmissionManagementSystem.Models
{
    public class PaymentDetail
    {
        [Key]
        public int Id { get; set; }
        public int StudentId { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal BalanceAmount { get; set; }

        public decimal PayingAmount { get; set; }


        public DateTime PaymentDate { get; set; }
    }
}