using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private void frmEnterPwd_Load(object sender, EventArgs e)
        {
            lblFilename.Text = $"Enter password for database: {Filename}";
        }

        internal static bool Prompt(string databaseFile, out string pwd)
        {            
            frmEnterPwd dlg = new frmEnterPwd();
            dlg.Filename = databaseFile;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                pwd = dlg.tbPassword.Text;
                return true;
            }

            pwd = null;
            return false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
