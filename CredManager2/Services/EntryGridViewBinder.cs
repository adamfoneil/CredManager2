using CredManager2.Models;
using Postulate.SqlCe.IntKey;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinForms.Library.Abstract;

namespace CredManager2.Services
{
    public class EntryGridViewBinder : GridViewBinder<Entry>
    {
        private readonly CredManagerDb _db;

        public EntryGridViewBinder(CredManagerDb db, DataGridView dataGridView) : base(dataGridView)
        {
            _db = db;
        }

        protected override bool SupportsAsync => false;

        protected override void OnDelete(Entry model)
        {
            using (var cn = _db.GetConnection())
            {
                cn.Delete<Entry>(model.Id);
            }
        }

        protected override void OnSave(Entry model)
        {
            using (var cn = _db.GetConnection())
            {
                cn.Save(model);
            }
        }

        protected override Task OnSaveAsync(Entry model) => throw new NotImplementedException();
        protected override Task OnDeleteAsync(Entry model) => throw new NotImplementedException();
    }
}
