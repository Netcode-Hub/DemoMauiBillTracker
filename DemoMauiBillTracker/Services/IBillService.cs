
using DemoMauiBillTracker.Models;

namespace DemoMauiBillTracker.Services
{
    public interface IBillService
    {
        // Bill Interface
        Task<(int code, string message)> AddBillAsync(Bill bill);
        Task<(int code, string message)> UpdateBillAsync(Bill bill);
        Task<Bill> GetBillAsync(int id);
        Task<List<Bill>> GetBillsAsync();
        Task<(int code, string message)> DeleteBillAsync(Bill bill);

        //Bill period interface
        Task<List<BillPeriod>> GetPeriodsAsync();
        Task AddPeriodAsync(BillPeriod period);
        Task<BillPeriod> GetPeriodAsync(string name);

        // Manage Bill
        Task<List<DueBill>> GetDueBillsAsync();
        Task<List<BillHistory>> GetBillsHistoryAsync();
        Task<bool> PayBillAsync(int id);
    }
}
