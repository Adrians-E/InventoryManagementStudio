using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InventoryManagementSystem;


namespace InventoryManagementSystem
{
    public partial class registrationForm : Form
    {
        private string connectionString = "Data Source=loginPanel.db;Version=3;";
        private string username;
        private string password;

        public registrationForm()
        {
            InitializeComponent();
        }

        private void registrationForm_Load(object sender, EventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=loginPanel.db;Version=3;"))
            {
                // Atver datubāzi. Ja tādas nav, tad izveido
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS loginPanel (ID INTEGER PRIMARY KEY AUTOINCREMENT, Username TEXT, Password TEXT)", conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            LoadData(); // Ielādē datus
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

        private void passwordTextBox2_TextChanged(object sender, EventArgs e)
        {
            password = passwordTextBox2.Text;
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            bool usernameTaken = false;
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(*) FROM loginPanel WHERE Username = @username", conn);
                cmd.Parameters.AddWithValue("username", username);
                int result = Convert.ToInt32(cmd.ExecuteScalar());
                if(result == 0)
                {
                    usernameTaken = false;

                } else
                {
                    usernameTaken = true;
                }
                conn.Close();
            }
            if (usernameTaken == false)
            {
                if (passwordTextBox.Text == passwordTextBox2.Text)
                {
                    string hashedPassword = Hashing.HashPassword(passwordTextBox.Text);
                    username = usernameTextBox.Text;
                    using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                    {
                        conn.Open();
                        SQLiteCommand cmd = new SQLiteCommand("INSERT INTO loginPanel (Username, Password) VALUES (@username, @password)", conn);
                        cmd.Parameters.AddWithValue("username", username);
                        cmd.Parameters.AddWithValue("password", hashedPassword);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        MessageBox.Show("Jūs esat reģistrējies!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.Hide();
                        Form2 form2 = new Form2();
                        form2.Show();
                    }
                }
                else
                {
                    MessageBox.Show("Paroles nav vienādas!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } else
            {
                MessageBox.Show("Šis lietotājvārds jau tiek izmantots!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}