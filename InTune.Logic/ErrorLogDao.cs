using InTune.Data;
using InTune.Domain;
using System;
using System.Data;

namespace InTune.Logic
{
    public class ErrorLogDao
    {
        DbContext _dbc = null;

        public ErrorLogDao(DbContext dbc)
        {
            _dbc = dbc;
        }

        public void LogError(string message)
        {
            var sql = "insert into [ErrorLog] (message) values (@message)";

            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@message", message);
            cmd.ExecuteNonQuery();
        }
    }
}
