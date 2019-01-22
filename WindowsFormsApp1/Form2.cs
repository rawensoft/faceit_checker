using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form2 : MetroFramework.Forms.MetroForm
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void Quit1_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2();
            form.Close();
        }

        private void errorMsg_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(errorMsg.Text);
        }
    }
}
