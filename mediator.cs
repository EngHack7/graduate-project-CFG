using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawCFGraph
{
    public partial class mediator: Form
    {
        public mediator()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(richTextBox1.Text))
            {
                MainWindow main = new MainWindow();
                main.displayProgram(richTextBox1.Text);
                this.Hide();
                main.Show();
            }
            else
            {
                MessageBox.Show("Enter Text, or select next button to read file ");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.File_Reader();
            this.Hide();mainWindow.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void mediator_Load(object sender, EventArgs e)
        {

        }
    }
}
