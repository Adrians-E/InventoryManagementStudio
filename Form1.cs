using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Windows.Forms;

namespace InventoryManagementSystem
{
    public partial class Form1 : Form
    {
        private string connectionString = "Data Source=inventory.db;Version=3;";
        string[] admins = {"admin", "admins"};
        private string currentUser;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=inventory.db;Version=3;"))
            {
                // Atver datubāzi. Ja tādas nav, tad izveido
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS inventory (ID INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Origin TEXT, Category TEXT, Price TEXT, Description TEXT, Quantity TEXT)", conn);
                cmd.ExecuteNonQuery();
                conn.Close();

            }
                LoadData(); // Ielādē datus
        }

        private void LoadData()
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=inventory.db;Version=3;"))
            {
                conn.Open();
                SQLiteDataAdapter adapter = new SQLiteDataAdapter("SELECT * FROM inventory", conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                inventoryGridView.DataSource = dt; // Atjauno datus
                conn.Close();
            }
        }
        public void saveUser(string user)
        {
           currentUser = user;
        }
        private void saveButton_Click(object sender, EventArgs e)
        {
            string name = nameTextBox.Text;
            string origin = originTextBox.Text;
            string price = priceTextBox.Text;
            string description = descriptionTextBox.Text;
            string quantity = quantityTextBox.Text;
            string category = categoryBox.SelectedItem != null ? categoryBox.SelectedItem.ToString() : "";

            int id = nameTextBox.Tag != null ? Convert.ToInt32(nameTextBox.Tag) : 0; // Atrod ID priekš datiem

            using (SQLiteConnection conn = new SQLiteConnection("Data Source=inventory.db;Version=3;"))
            {
                conn.Open();
                SQLiteCommand cmd;

                if (id > 0) // Ja atjauno esošos datus
                {
                    cmd = new SQLiteCommand("UPDATE inventory SET Name=@name, Origin=@origin, Category=@category, Price=@price, Description=@description, Quantity=@quantity WHERE ID=@id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                }
                else // Ja pievieno jaunus datus
                {
                    cmd = new SQLiteCommand("INSERT INTO inventory (Name, Origin, Category, Price, Description, Quantity) VALUES (@name, @origin, @category, @price, @description, @quantity)", conn);
                }

                // Savieno parametrus
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@origin", origin);
                cmd.Parameters.AddWithValue("@category", category);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@quantity", quantity);

                cmd.ExecuteNonQuery();
                conn.Close();
            }

            // Atjauno tabulu
            LoadData();
            newButton_Click(sender, e); // Notīra lodziņus, lai ievadītu jaunus datus
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (!admins.Contains(currentUser.ToLower()))
            {
                MessageBox.Show("You don't have permissions for this command!", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (inventoryGridView.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(inventoryGridView.SelectedRows[0].Cells["Id"].Value);

                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string deleteQuery = "DELETE FROM Inventory WHERE Id = @Id";

                    using (SQLiteCommand cmd = new SQLiteCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Item deleted successfully!", "Deleted");
                LoadData(); 
            }
            else
            {
                MessageBox.Show("Please select an item to delete.", "Error");
            }
        }

        private void inventoryGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Pārliecinās, ka uzspiestā vieta eksistē
            {
                DataGridViewRow row = inventoryGridView.Rows[e.RowIndex];

                // Ielādē datus
                nameTextBox.Text = row.Cells["Name"].Value.ToString();
                originTextBox.Text = row.Cells["Origin"].Value.ToString();
                priceTextBox.Text = row.Cells["Price"].Value.ToString();
                descriptionTextBox.Text = row.Cells["Description"].Value.ToString();
                quantityTextBox.Text = row.Cells["Quantity"].Value.ToString();
                categoryBox.SelectedItem = row.Cells["Category"].Value.ToString();

                // Patur ID, lai datubāzei vieglāk strādāt
                nameTextBox.Tag = row.Cells["ID"].Value;
            }
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            // Notīra lodziņus no datiem
            nameTextBox.Text = "";
            originTextBox.Text = "";
            priceTextBox.Text = "";
            descriptionTextBox.Text = "";
            quantityTextBox.Text = "";
            categoryBox.SelectedIndex = -1;
        }
    }
}