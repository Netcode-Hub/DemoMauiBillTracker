
using DemoMauiBillTracker.Models;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;
using System.Net;

namespace DemoMauiBillTracker.Services
{
    public class BillService : IBillService
    {
        private SQLiteAsyncConnection connection;
        public BillService()
        {
            SetupDatabase();
        }

        private async void SetupDatabase()
        {
            if (connection is null)
            {
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DemoBill.db3");
                connection = new SQLiteAsyncConnection(dbPath);
                await connection.CreateTableAsync<Bill>();
                await connection.CreateTableAsync<BillPeriod>();
                await connection.CreateTableAsync<DueBill>();
                await connection.CreateTableAsync<BillHistory>();
            }
        }

        //Bill service
        public async Task<(int code, string message)> AddBillAsync(Bill bill)
        {
            if (bill is null)
                return ((int)HttpStatusCode.BadRequest, "Bads Request");

            int result = await connection.InsertAsync(bill);
            if (result > 0)
                return ((int)HttpStatusCode.Created, "Created Successfully");
            return ((int)HttpStatusCode.InternalServerError, "Internal Server Error");
        }

        public async Task<(int code, string message)> DeleteBillAsync(Bill bill)
        {
            var result = await connection?.DeleteAsync(bill);
            if (result > 0)
                return ((int)HttpStatusCode.OK, "Deleted Successfully");
            return ((int)HttpStatusCode.InternalServerError, "Internal Server Error");
        }

        public async Task<(int code, string message)> UpdateBillAsync(Bill bill)
        {
            var result = await connection.UpdateAsync(bill);
            if (result > 0)
                return ((int)HttpStatusCode.OK, "Updated Successfully");
            return ((int)HttpStatusCode.InternalServerError, "Internal Server Error");
        }

        public async Task<Bill> GetBillAsync(int id)
        {
            var result = await connection.GetAllWithChildrenAsync<Bill>(_ => _.Id == id, recursive: true);
            return result.FirstOrDefault();
        }

        public async Task<List<Bill>> GetBillsAsync()
        {
            var result = await connection.GetAllWithChildrenAsync<Bill>(recursive: true);
            if (result.Count == 0) return null;
            return result.ToList();
        }

        // Bill history service
        public async Task<List<BillHistory>> GetBillsHistoryAsync()
        {
            var result = await connection.Table<BillHistory>().ToListAsync();
            if (result.Count == 0) return null;
            return result.ToList();
        }

        public async Task<List<DueBill>> GetDueBillsAsync()
        {
            // Prepare all due bills into DueBills Table which is awaiting for payments.
            var getAlActivelBills = await connection.GetAllWithChildrenAsync<Bill>(_ => _.Active, recursive: true);
            if (getAlActivelBills.Any())
            {
                // Get and prepare Monthly bills.
                var getAllMonthlyBills = getAlActivelBills.Where(_ => _.BillPeriod!.PeriodName!.Equals("Monthly")).ToList();
                if (getAllMonthlyBills.Any())
                {
                    foreach (var bill in getAllMonthlyBills)
                    {
                        bool checkIfDatesMeet = DateTime.Now.Date.ToShortDateString() == bill.StartingDate.Value.AddMonths(1).Date.ToShortDateString();
                        if (checkIfDatesMeet)
                        {
                            // this means the bill is due to be paid, so prepare the bill and save it to the DueBills Table
                            //First check and see if the bill has being already prepared from the DueBills Table.
                            try
                            {
                                DueBill checkBill = await connection.Table<DueBill>().Where(_ => _.BillId == bill.Id).FirstOrDefaultAsync();
                                bool checkIfDateMeets = false;
                                if (checkBill != null)
                                    checkIfDateMeets = checkBill.DueDate == bill.StartingDate.Value.AddMonths(1);


                                if (checkIfDateMeets == false)
                                {
                                    var newDueBill = new DueBill()
                                    {
                                        BillId = bill.Id,
                                        Name = bill.Name,
                                        Amount = bill.Amount,
                                        DueDate = bill.StartingDate.Value.AddMonths(1),
                                        BillType = bill.BillPeriod!.PeriodName,
                                        Paid = false
                                    };
                                    await connection.InsertAsync(newDueBill);

                                    // update the default bill starting date to next month.

                                    var defaultBill = await connection.Table<Bill>().FirstOrDefaultAsync(_ => _.Id == bill.Id);
                                    if (defaultBill is not null)
                                    {
                                        var newDefaultBill = new Bill()
                                        {
                                            Id = defaultBill.Id,
                                            Name = defaultBill.Name,
                                            Amount = defaultBill.Amount,
                                            StartingDate = defaultBill.StartingDate.Value.AddMonths(1),
                                            BillPeriodId = defaultBill.BillPeriodId,
                                            Active = defaultBill.Active
                                        };
                                        await connection.InsertAsync(newDefaultBill);
                                        await connection.DeleteAsync(defaultBill);
                                    }
                                }
                            }
                            catch (Exception ex) { throw new Exception(ex.Message); }
                        }
                    }
                }
            }
            return await connection.Table<DueBill>().ToListAsync();
        }
        public async Task<List<BillPeriod>> GetPeriodsAsync()
        {
            var result = await connection.Table<BillPeriod>().ToListAsync();
            if (result.Count == 0) return null;
            return result.ToList();
        }

        public async Task<bool> PayBillAsync(int id)
        {
            var result = await connection.Table<DueBill>().Where(_ => _.Id == id).FirstOrDefaultAsync();
            if (result is not null)
            {
                // move to the history table
                var newHistory = new BillHistory()
                {
                    Id = id,
                    BillId = result.BillId,
                    Amount = result.Amount,
                    DueDate = result.DueDate,
                    Paid = true,
                    BillType = result.BillType,
                    Name = result.Name
                };
                await connection.DeleteAsync(result);
                await connection.InsertAsync(newHistory);
            }
            return true;
        }

        //Add period
        public async Task AddPeriodAsync(BillPeriod period)
        {
            await connection.InsertAsync(period);
        }

        public async Task<BillPeriod> GetPeriodAsync(string name)
        {
            var result = await connection.Table<BillPeriod>().FirstOrDefaultAsync(_ => _.PeriodName.ToLower().Equals(name.ToLower()));
            return result;
        }
    }
}
