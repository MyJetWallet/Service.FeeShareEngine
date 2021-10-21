using System;

namespace Service.FeeShareEngine.Postgres.Models
{
    public class ShareStatEntity
    {
        public decimal Amount { get; set; }
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public DateTime CalculationTimestamp { get; set; }
    }
}