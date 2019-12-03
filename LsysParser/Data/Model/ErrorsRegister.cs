using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LsysParser.Data.Model
{
    class ErrorsRegister
    {
        string connectionString;

        public ErrorsRegister(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void RemoveAll()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    using (var command = conn.CreateCommand())
                    {
                        command.Transaction = trans;
                        command.CommandText = @"DELETE FROM error";
                        command.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
            }
        }

        public void RegisterError(ErrorRuntimeObj err)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    using (var command = conn.CreateCommand())
                    {
                        command.Transaction = trans;
                        command.CommandText = @"INSERT INTO error (url, code, error_source_id, message)
                             VALUES (@url, @code, @errorSourceId, @message)";

                        command.Parameters.Add(new SqlParameter("@url", err.Url));
                        command.Parameters.Add(new SqlParameter("@code", err.Code ?? -1));
                        command.Parameters.Add(new SqlParameter("@errorSourceId", err.ErrorSourceId));
                        command.Parameters.Add(new SqlParameter("@message", err.Message));

                        command.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
            }
        }
    }
}
