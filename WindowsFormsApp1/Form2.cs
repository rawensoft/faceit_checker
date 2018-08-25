using System;

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
    }
}
