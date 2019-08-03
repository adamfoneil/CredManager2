using System;
using System.Windows.Forms;

namespace CredManager2
{
    public partial class frmCreateDb : Form
    {
        public frmCreateDb()
        {
            InitializeComponent();
        }

        internal static bool Prompt(out string databaseFile, out string pwd, out string pwdHint)
        {
            databaseFile = null;
            pwd = null;
            pwdHint = null;

            frmCreateDb dlg = new frmCreateDb();
            if (!dlg.PromptFilename()) return false;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                databaseFile = dlg.tbFilename.Text;
                pwd = dlg.tbPassword.Text;
                pwdHint = dlg.tbHint.Text;
                return true;
            }

            return false;
        }

        public bool PromptFilename()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = "sdf";
            dlg.Filter = "SDF Files|*.sdf|All Files|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                tbFilename.Text = dlg.FileName;
                return true;
            }

            return false;
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            PromptFilename();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();

            if (string.IsNullOrEmpty(tbPassword.Text))
            {
                errorProvider1.SetError(tbPassword, "Password cannot be empty.");
                return;
            }

            if (!tbPassword.Text.Equals(tbConfirm.Text))
            {
                errorProvider1.SetError(tbConfirm, "Passwords don't match.");
                return;
            }

            DialogResult = DialogResult.OK;
        }
    }
}
