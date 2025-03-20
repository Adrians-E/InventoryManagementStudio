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
    public partial class InventoryManagementSystem : Form
    {
        private string connectionString = "Data Source=loginPanel.db;Version=3;";
        private string username;
        private string password;
        public InventoryManagementSystem()
        {
            InitializeComponent();
        }
        private void Form2_Load(object sender, EventArgs e)
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
                //inventoryGridView.DataSource = dt; // Atjauno datus
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

                // SQL Query to check if the username and password exist
                string query = "SELECT COUNT(*) FROM loginPanel WHERE Username = @username AND Password = @password";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    int userCount = Convert.ToInt32(cmd.ExecuteScalar());

                    if (userCount > 0)
                    {
                        //MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Form1 mainForm = new Form1();
                        mainForm.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                conn.Close();
            }
        }
    }
}
