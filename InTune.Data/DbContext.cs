using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InTune.Data
{
    public class DbContext : IDisposable
    {
        IDbConnection _con = null;
        IDbTransaction _trans = null;

        public DbContext()
        {
            _con = openConnection();
        }

        private IDbConnection openConnection()
        {
            var con = new SqlConnection(@"Server=3622e3bc-2b75-45d8-a892-a5dd00d62baa.sqlserver.sequelizer.com;Database=db3622e3bc2b7545d8a892a5dd00d62baa;User ID=jzralyffsomsvdnu;Password=i27HrvMAWiiEmJyJG5doc7AtQ4xGU8vTHNchtwharnmsWQ8hk4j4erVkzMZw7fPX;");
            con.Open();
            return con;
        }

        public void BeginTransaction()
        {
            if (_trans != null)
                throw new Exception("Cannot start new transaction while another transaction is in progress");

            _trans = _con.BeginTransaction();
        }

        public void Commit()
        {
            if (_trans == null)
                return;

            _trans.Commit();
            _trans = null;
        }

        public void Rollback()
        {
            if (_trans == null)
                return;

            _trans.Rollback();
            _trans = null;
        }

        public IDbCommand CreateCommand()
        {
            return CreateCommand(null);
        }

        public IDbCommand CreateCommand(string commandText)
        {
            var command = _con.CreateCommand();
            command.Transaction = _trans;
            command.CommandText = commandText;
            return command;
        }

        public void AddParameterWithValue(IDbCommand cmd, string name, object value)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = name;
            param.Value = convertToDbValue(value);
            cmd.Parameters.Add(param);
        }

        private object convertToDbValue(object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return DBNull.Value;

            return value;
        }

        public IDataReader ExecuteReader(string sql)
        {
            var cmd = _con.CreateCommand();
            cmd.Transaction = _trans;
            cmd.CommandText = sql;
            return cmd.ExecuteReader();
        }

        public object ExecuteScalar(string sql)
        {
            var cmd = _con.CreateCommand();
            cmd.Transaction = _trans;
            cmd.CommandText = sql;
            return cmd.ExecuteScalar();
        }

        public void ExecuteCommand(string sql)
        {
            var cmd = _con.CreateCommand();
            cmd.Transaction = _trans;
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        public int GetGeneratedIdentityValue()
        {
            var cmd = CreateCommand("select @@identity");
            var value = cmd.ExecuteScalar();
            return value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }

        public void Dispose()
        {
            if (_con == null)
                return;

            Rollback();
            _con.Close();
            _con.Dispose();
        }
    }
}
