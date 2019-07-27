using CredManager2.Models;
using Postulate.SqlCe.IntKey;
using System.Data.SqlServerCe;
using System.IO;

namespace CredManager2.Services
{
    public class CredManagerDb
    {
        private readonly string _fileName;
        private readonly string _connectionString;

        private SqlCeEngine _engine;

        public CredManagerDb(string fileName, string password)
        {
            _fileName = fileName;
            _connectionString = $"Data Source='{fileName}';LCID=1033;Password={password};Encryption Mode=Platform Default";
        }

        public SqlCeConnection GetConnection()
        {
            _engine = new SqlCeEngine(_connectionString);

            if (!File.Exists(_fileName))
            {
                string folder = Path.GetDirectoryName(_fileName);
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                _engine.CreateDatabase();

                using (var cn = new SqlCeConnection(_connectionString))
                {
                    cn.CreateTable<Entry>();
                }
            }

            return new SqlCeConnection(_connectionString);
        }
    }
}
