using System;

namespace Microsoft
{
    internal class Data
    {
        internal class Sqlite
        {
            internal class SqliteConnection : IDisposable
            {
                private string connectionString;

                public SqliteConnection(string connectionString)
                {
                    this.connectionString = connectionString;
                }

                public void Dispose()
                {
                    throw new NotImplementedException();
                }

                internal void Open()
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}