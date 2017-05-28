using InTune.Data;
using InTune.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InTune.Logic
{
    public class EntryDao
    {
        DbContext _dbc = null;
        Entry _entry = null;

        public EntryDao(DbContext dbc, Entry entry)
            : this(dbc)
        {
            _entry = entry;
        }

        public EntryDao(DbContext dbc)
        {
            _dbc = dbc;
        }

        public void InsertEntry()
        {
            var sql = "insert into Entry (userId, accountId, txnType, notes, txnDate, quantity, amount, voidId) values (@userId, @accountId, @txnType, @notes, @txnDate, @quantity, @amount, @voidId)";

            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@userId", _entry.UserId);
            _dbc.AddParameterWithValue(cmd, "@accountId", _entry.AccountId);
            _dbc.AddParameterWithValue(cmd, "@txnType", _entry.TxnType);
            _dbc.AddParameterWithValue(cmd, "@notes", _entry.Notes);
            _dbc.AddParameterWithValue(cmd, "@txnDate", _entry.TxnDate);
            _dbc.AddParameterWithValue(cmd, "@quantity", _entry.Quantity);
            _dbc.AddParameterWithValue(cmd, "@amount", _entry.Amount);
            _dbc.AddParameterWithValue(cmd, "@voidId", _entry.VoidId);
            cmd.ExecuteNonQuery();

            _entry.Id = _dbc.GetGeneratedIdentityValue();

            sql = string.Format("update Entry set voidId={0} where id={1}", _entry.Id, _entry.VoidId);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        public Entry ReadAccountEntry(int entryId)
        {
            var sql = string.Format("select * from Entry where id={0}", entryId);
            using (var rdr = _dbc.ExecuteReader(sql))
            {

                if (rdr.Read())
                {
                    return new Entry
                    {
                        Id = Convert.ToInt32(rdr["id"]),
                        UserId = Convert.ToInt32(rdr["UserId"]),
                        AccountId = Convert.ToInt32(rdr["AccountId"]),,
                        TxnType = (TxnType)Convert.ToInt32(rdr["TxnType"]),
                        Notes = rdr["Notes"].ToString(),
                        TxnDate = Convert.ToDateTime(rdr["TxnDate"]),
                        Quantity = Convert.ToDouble(rdr["Quantity"]),
                        Amount = Convert.ToDecimal(rdr["Amount"]),
                        VoidId = Convert.ToInt32(rdr["VoidId"]),
                    };
                }
            }

            return new Entry();
        }

        public IList<Entry> ReadAccountEntries(int accountId)
        {
            var result = new List<Entry>();
            var sql = string.Format("select * from Entry where accountId={0} ", accountId);
            var rdr = _dbc.ExecuteReader(sql);
            while (rdr.Read())
            {
                result.Add(new Entry
                {
                    Id = Convert.ToInt32(rdr["id"]),
                    UserId = Convert.ToInt32(rdr["userId"]),
                    AccountId = accountId,
                    TxnType = (TxnType)Convert.ToInt32(rdr["TxnType"]),
                    Notes = rdr["Notes"].ToString(),
                    TxnDate = Convert.ToDateTime(rdr["TxnDate"]),
                    Quantity = Convert.ToDouble(rdr["Quantity"]),
                    Amount = Convert.ToDecimal(rdr["Amount"]),
                    VoidId = Convert.ToInt32(rdr["VoidId"]),
                });
            }

            rdr.Close();
            return result;
        }
    }
}
