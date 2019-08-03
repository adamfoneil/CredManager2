using CredManager2.Queries;
using CredManager2.Services;
using JsonSettings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinForms.Library.Extensions;
using WinForms.Library.Models;

namespace CredManager2
{
    public partial class frmMain : Form
    {
        private Settings _settings = null;
        private CredManagerDb _db = null;
        private EntryGridViewBinder _binder = null;

        public frmMain()
        {
            InitializeComponent();
            dgvEntries.AutoGenerateColumns = false;
        }

        private async void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {
                _settings = JsonSettingsBase.Load<Settings>();
                _settings.FormPosition?.Apply(this);

                cbFilterActive.Fill(new Dictionary<bool, string>()
                {
                    { true, "Active" },
                    { false, "Inactive" }
                });
                cbFilterActive.SetValue<bool>(true);

                if (File.Exists(_settings.DatabaseFile))
                {
                    string pwd;
                    while (frmEnterPwd.Prompt(_settings.DatabaseFile, out pwd))
                    {
                        if (TryOpenConnection(pwd)) break;
                    }

                    _db = new CredManagerDb(_settings.DatabaseFile, pwd);
                }
                else
                {
                    _db = CreateDatabase();
                }

                _binder = new EntryGridViewBinder(_db, dgvEntries);

                await FillRecordsAsync();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private async Task FillRecordsAsync()
        {
            if (_db == null) return;

            using (var cn = _db.GetConnection())
            {
                var records = await new Entries()
                {
                    IsActive = cbFilterActive.GetValue<bool>(),
                    Search = tbSearch.Text
                }.ExecuteAsync(cn);
                _binder.Fill(records);
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _settings.FormPosition = FormPosition.FromForm(this);
            _settings.Save();
        }

        private bool TryOpenConnection(string pwd)
        {
            try
            {
                _db = new CredManagerDb(_settings.DatabaseFile, pwd);
                using (var cn = _db.GetConnection())
                {
                    cn.Open();
                    Text = $"CredManager - {_settings.DatabaseFile}";

                    //entryDataGridView1.Fill(_db, new Entries(_db) { IsActive = true }.Execute(cn));
                    return true;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
                return false;
            }
        }

        private CredManagerDb CreateDatabase()
        {
            string databaseFile;
            string pwd;
            string hint;
            if (frmCreateDb.Prompt(out databaseFile, out pwd, out hint))
            {
                var db = new CredManagerDb(databaseFile, pwd);
                using (var cn = db.GetConnection())
                {
                    _settings.DatabaseFile = databaseFile;
                    _settings.PasswordHint = hint;
                    _settings.Save();
                    Text = $"CredManager - {_settings.DatabaseFile}";
                }
                return db;
            }

            return null;
        }

        private async void CbFilterActive_SelectedIndexChanged(object sender, EventArgs e)
        {
            await FillRecordsAsync();
        }
    }
}
