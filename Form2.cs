using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace InventoryManagementSystem
{
    public partial class Form2 : Form
    {
        private string connectionString = "Data Source=loginPanel.db;Version=3;";
        private string username;
        private string password;

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=loginPanel.db;Version=3;"))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS loginPanel (ID INTEGER PRIMARY KEY AUTOINCREMENT, Username TEXT, Password TEXT)", conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            LoadData();
        }

        private void LoadData()
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                SQLiteDataAdapter adapter = new SQLiteDataAdapter("SELECT * FROM loginPanel", conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                conn.Close();
            }
        }

        private void usernameTextBox_TextChanged(object sender, EventArgs e)
        {
            username = usernameTextBox.Text;
        }

        private void passwordTextBox_TextChanged(object sender, EventArgs e)
        {
            password = passwordTextBox.Text;
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ID FROM loginPanel WHERE Username = @username";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    int ID = Convert.ToInt32(cmd.ExecuteScalar());

                    if (ID >= 0)
                    {
                        if (Hashing.VerifyPassword(ID, password))
                        {
                            MessageBox.Show("Pieslēgšanās veiksmīga!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Form1 mainForm = new Form1();
                            mainForm.Show();
                            mainForm.saveUser(username);
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Parole nav pareiza!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Nav atrasts šāds lietotājs!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                conn.Close();
            }
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            registrationForm registrationForm = new registrationForm();
            registrationForm.Show();
        }
    }
}


