using SQLite;
namespace DemoMauiBillTracker.Models
{
    [Table("DueBills")]
    public class DueBill
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int BillId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string DueDateName { get; set; }
        public bool Paid { get; set; }
        public string BillType { get; set; }
    }
}
