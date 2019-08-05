using CredManager2.Models;
using CredManager2.Queries;
using CredManager2.Services;
using JsonSettings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinForms.Library;
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
                if (_settings.Recent == null) _settings.Recent = new HashSet<string>();
                _settings.FormPosition?.Apply(this);

                FillRecentItems();

                cbFilterActive.Fill(new Dictionary<bool, string>()
                {
                    { true, "Active" },
                    { false, "Inactive" }
                });
                cbFilterActive.SetValue(true);

                if (File.Exists(_settings.DatabaseFile))
                {
                    _db = OpenDatabase(_settings.DatabaseFile);
                }
                else
                {
                    _db = CreateDatabase();
                }
                
                _binder = new EntryGridViewBinder(_db, dgvEntries);

                await FillRecordsAsync();

                new GridCellAutoComplete(colUserName, _binder.GetRows().GroupBy(row => row.UserName).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).ToArray());

                Text = $"CredManager - {_settings.DatabaseFile}";
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void FillRecentItems()
        {
            btnCopyPwd.DropDownItems.Clear();
            foreach (string item in _settings.Recent)
            {
                var button = new ToolStripButton(item);
                button.Click += ItemClicked;
                btnCopyPwd.DropDownItems.Add(button);
            }
        }

        private void ItemClicked(object sender, EventArgs e)
        {
            var button = sender as ToolStripButton;
            var items = _binder.GetRows().ToDictionary(row => row.Name);
            Clipboard.SetText(items[button.Text].Password);
        }

        private CredManagerDb OpenDatabase(string fileName)
        {
            string pwd;
            while (frmEnterPwd.Prompt(fileName, out pwd))
            {
                if (TryOpenConnection(fileName, pwd))
                {
                    return new CredManagerDb(fileName, pwd);
                }
            }

            return null;
        }

        private async Task FillRecordsAsync()
        {
            if (_db == null) return;

            using (var cn = _db.GetConnection())
            {
                var qry = new Entries()
                {
                    IsActive = cbFilterActive.GetValue<bool>(),
                    Search = "%" + tbSearch.Text + "%" // need to do wildcards like this in SqlCe
                };

                var records = await qry.ExecuteAsync(cn);
                _binder.Fill(records);
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _settings.FormPosition = FormPosition.FromForm(this);
            _settings.Save();
        }

        private bool TryOpenConnection(string fileName, string pwd)
        {
            try
            {
                _db = new CredManagerDb(fileName, pwd);
                using (var cn = _db.GetConnection())
                {
                    cn.Open();                    
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
                }
                return db;
            }

            return null;
        }

        private async void CbFilterActive_SelectedIndexChanged(object sender, EventArgs e)
        {
            await FillRecordsAsync();
        }

        private async void TbSearch_TextChanged(object sender, EventArgs e)
        {
            await FillRecordsAsync();
        }

        private async void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "SDF Files|*.sdf|All Files|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (frmEnterPwd.Prompt(dlg.FileName, out string pwd))
                    {
                        if (TryOpenConnection(dlg.FileName, pwd))
                        {
                            _settings.DatabaseFile = dlg.FileName;
                            _settings.Save();
                            await FillRecordsAsync();
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void DgvEntries_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var row = dgvEntries.Rows[e.RowIndex];
                var entry = row.DataBoundItem as Entry;

                if (e.ColumnIndex == colGoTo.Index)
                {
                    string url = row.Cells["colUrl"].Value.ToString();
                    string[] mayStartWith = new string[] { "https://", "http://" };
                    if (!mayStartWith.Any(item => url.ToLower().StartsWith(item))) url = mayStartWith[0] + url;
                    FileSystem.OpenDocument(url);
                }

                if (e.ColumnIndex == colCopyUserName.Index)
                {
                    Clipboard.SetText(row.Cells["colUserName"].Value.ToString());
                }

                if (e.ColumnIndex == colCopyPwd.Index)
                {
                    Clipboard.SetText(row.Cells["colPassword"].Value.ToString());
                    _settings.Recent.Add(entry.Name);
                    /* the position of added items doesn't follow a pattern I can tell, so I pick the item first or last
                    to remove based on whatever what you didn't just add */
                    string remove = (_settings.Recent.First().Equals(entry.Name)) ? _settings.Recent.Last() : _settings.Recent.First();
                    if (_settings.Recent.Count > 5) _settings.Recent.Remove(remove);
                    FillRecentItems();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }

        }

        private void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
            {
                tbSearch.Focus();
                e.Handled = true;
            }
        }
    }
}
