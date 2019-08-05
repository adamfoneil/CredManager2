using CredManager2.Models;
using Dapper;
using Postulate.SqlCe.IntKey;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Threading.Tasks;

namespace CredManager2.Services
{
    public class ImportCommand
    {
        private readonly CredManagerDb _sourceDb;
        private readonly CredManagerDb _destDb;

        public ImportCommand(CredManagerDb sourceDb, CredManagerDb destDb)
        {
            _sourceDb = sourceDb;
            _destDb = destDb;
        }

        public async Task<ImportResult> ExecuteAsync()
        {
            using (var cnSource = _sourceDb.GetConnection())
            {
                using (var cnDest = _destDb.GetConnection())
                {
                    var sourceEntries = await GetEntriesAsync(cnSource);
                    var destEntries = await GetEntriesAsync(cnDest);
                    return await ImportEntriesAsync(sourceEntries, destEntries, cnDest);                    
                }
            }
        }

        private async Task<IEnumerable<Entry>> GetEntriesAsync(SqlCeConnection connection)
        {
            return await connection.QueryAsync<Entry>("SELECT * FROM [Entry]");
        }

        private async Task<ImportResult> ImportEntriesAsync(IEnumerable<Entry> sourceEntries, IEnumerable<Entry> destEntries, IDbConnection cn)
        {
            var result = new ImportResult();

            var newEntries = GetNewEntries(sourceEntries, destEntries);

            foreach (var entry in newEntries)
            {                
                await cn.InsertAsync(entry);
            }

            var updatedEntries = GetUpdatedEntries(sourceEntries, destEntries);

            foreach (var entry in updatedEntries)
            {
                await cn.MergeAsync(entry);
            }

            result.NewEntries = newEntries.Count();
            result.UpdatedEntries = updatedEntries.Count();
            return result;
        }

        public IEnumerable<Entry> GetNewEntries(IEnumerable<Entry> sourceEntries, IEnumerable<Entry> destEntries)
        {
            return sourceEntries.Except(destEntries);
        }

        public IEnumerable<Entry> GetUpdatedEntries(IEnumerable<Entry> sourceEntries, IEnumerable<Entry> destEntries)
        {
            return from src in sourceEntries
                   join dest in destEntries on src equals dest
                   where src.IsNewerThan(dest)
                   select src;
        }
    }
}
