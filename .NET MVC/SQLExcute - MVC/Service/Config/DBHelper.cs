using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Acctrue.CMC.Configuration;
using System.Data;

namespace Acctrue.CMC.Web.WebDBInstall.Config
{
    public class DBHelper : IDisposable
    {
        Dictionary<string, IDbConnection> connectionCache = new Dictionary<string, IDbConnection>();

        public int ExecuteNonQuery(DBObject db, string commandText, bool autoClose = false)
        {
            var connection = CreateConnection(db);

            var command = connection.CreateCommand();
            command.CommandText = commandText;
            if (connection.State != ConnectionState.Open)
                connection.Open();
            command.Connection = connection;
            var affected = command.ExecuteNonQuery();
            command.Dispose();
            return affected;

        }

        public object ExecuteScalar(DBObject db, string commandText, bool autoClose = true)
        {
            var connection = CreateConnection(db);

            var command = connection.CreateCommand();
            command.CommandText = commandText;
            if (connection.State != ConnectionState.Open)
                connection.Open();
            command.Connection = connection;
            var value = command.ExecuteScalar();
            command.Dispose();
            return value;

        }

        IDbConnection CreateConnection(DBObject db)
        {
            IDbConnection connection = null;
            if (connectionCache.ContainsKey(db.DbName))
            {
                connection = connectionCache[db.DbName];
            }
            else
            {
                connection = db.DbConnection;
                connectionCache.Add(db.DbName, connection);
            }
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
            }
            catch
            {
                try
                {
                    if (connection.State != ConnectionState.Closed)
                        connection.Close();
                    if (connection != null)
                        connection.Dispose();
                }
                catch { }
                for (var i = 0; i < 10; i++)
                {
                    try
                    {
                        connection = db.DbConnection;
                        if (connection.State != ConnectionState.Open)
                            connection.Open();
                        connectionCache[db.DbName] = connection;
                        System.Threading.Thread.Sleep(30);
                        break;
                    }
                    catch
                    {
                    }
                }
            }

            return connection;
        }

        public void Dispose()
        {
            if (connectionCache != null)
            {
                foreach (var connection in connectionCache.Values)
                {
                    if (connection.State != ConnectionState.Closed)
                        connection.Close();
                    if (connection != null)
                        connection.Dispose();
                }
                connectionCache.Clear();
                connectionCache = null;
            }
        }

        public void Clear()
        {
            if (connectionCache != null && connectionCache.Count > 0)
            {
                foreach (var connection in connectionCache.Values)
                {
                    if (connection.State != ConnectionState.Closed)
                        connection.Close();
                    if (connection != null)
                        connection.Dispose();
                }
                connectionCache.Clear();
            }
        }

        private string GetDbVersion()
        {
            try
            {
                var archiver = DatabaseArchiver.Create();
                var db = archiver.Read();
                return VersionReader.DatabaseVersion(db);
            }
            catch (Exception ex)
            {
                return "1.00";
            }
        }
    }
}