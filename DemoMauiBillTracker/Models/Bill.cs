
using SQLite;
using SQLiteNetExtensions.Attributes;
using System.ComponentModel.DataAnnotations;
namespace DemoMauiBillTracker.Models
{
    [Table("Bills")]
    public class Bill
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }
        [Required]
        public DateTime? StartingDate { get; set; }
        public bool Active { get; set; }
        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public BillPeriod BillPeriod { get; set; }
        [ForeignKey(typeof(BillPeriod))]
        public int BillPeriodId { get; set; }
    }
}
