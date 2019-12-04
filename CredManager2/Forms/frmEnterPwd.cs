using CredManager2.Services;
using Dapper;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CredManager2
{
    public partial class frmEnterPwd : Form
    {
        public string Filename { get; private set; }

        public frmEnterPwd()
        {
            InitializeComponent();
        }

        public bool GetSinglePassword
        {
            get { return chkGetOnePwd.Checked; }
        }

        public string GetSinglePasswordEntry
        {
            get { return tbSearchEntryName.Text; }
        }

        private void frmEnterPwd_Load(object sender, EventArgs e)
        {
            lblFilename.Text = $"Enter password for database: {Filename}";
        }

        internal static bool Prompt(string databaseFile, out string pwd, out string getSinglePassword)
        {
            getSinglePassword = null;

            frmEnterPwd dlg = new frmEnterPwd();
            dlg.Filename = databaseFile;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                pwd = dlg.tbPassword.Text;
                if (dlg.GetSinglePassword) getSinglePassword = dlg.GetSinglePasswordEntry;                
                return true;
            }

            pwd = null;
            
            return false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void chkGetOnePwd_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkGetOnePwd.Checked && File.Exists(Filename))
                {
                    var db = new CredManagerDb(Filename, tbPassword.Text);
                    var validPwd = db.TryOpenConnection(out _);

                    if (validPwd) InitEntryAutoComplete(db);

                    btnOK.Enabled = validPwd;
                }

                tbSearchEntryName.Enabled = chkGetOnePwd.Checked;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void InitEntryAutoComplete(CredManagerDb db)
        {
            using (var cn = db.GetConnection())
            {
                var items = cn.Query<string>("SELECT [Name] FROM [Entry] WHERE [IsActive]=1 ORDER BY [Name]").ToArray();
                var autoComplete = new AutoCompleteStringCollection();
                autoComplete.AddRange(items);
                tbSearchEntryName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                tbSearchEntryName.AutoCompleteCustomSource = autoComplete;
                tbSearchEntryName.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
        }
    }
}
