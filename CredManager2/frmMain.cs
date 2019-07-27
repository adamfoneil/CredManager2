using CredManager2.Services;
using JsonSettings;
using System;
using System.IO;
using System.Windows.Forms;
using WinForms.Library.Models;

namespace CredManager2
{
    public partial class frmMain : Form
    {
        private Settings _settings = null;
        private CredManagerDb _db = null;

        public frmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {
                _settings = JsonSettingsBase.Load<Settings>();
                _settings.FormPosition?.Apply(this);

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
                    CreateDatabase();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
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

        private void CreateDatabase()
        {
            try
            {
                string databaseFile;
                string pwd;
                string hint;
                if (frmCreateDb.Prompt(out databaseFile, out pwd, out hint))
                {
                    _db = new CredManagerDb(databaseFile, pwd);
                    using (var cn = _db.GetConnection())
                    {
                        _settings.DatabaseFile = databaseFile;
                        _settings.PasswordHint = hint;
                        _settings.Save();
                        Text = $"CredManager - {_settings.DatabaseFile}";
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
    }
}
