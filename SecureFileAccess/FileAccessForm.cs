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
using System.Windows.Forms;

namespace SecureFileAccess
{
    public partial class FileAccessForm : Form
    {
            private int userID;
            private string userName;
            private string userDepartment;

            public FileAccessForm(int userID, string userName, string userDepartment)
            {
                InitializeComponent();
                this.userID = userID;
                this.userName = userName;
                this.userDepartment = userDepartment;
                LoadAccessibleFiles();
            }

            // Load only files accessible to the user's department
            private void LoadAccessibleFiles()
            {
                listBoxFiles.Items.Clear();

                using (SQLiteConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                    SELECT FileName 
                    FROM Files 
                    WHERE 
                        AllowedDepartments LIKE @exact 
                        OR AllowedDepartments LIKE @start 
                        OR AllowedDepartments LIKE @end 
                        OR AllowedDepartments LIKE @any 
                        OR AllowedDepartments = @dept";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        // More accurate department matching
                        cmd.Parameters.AddWithValue("@exact", userDepartment);
                        cmd.Parameters.AddWithValue("@start", userDepartment + ",%");
                        cmd.Parameters.AddWithValue("@end", "%," + userDepartment);
                        cmd.Parameters.AddWithValue("@any", "%," + userDepartment + ",%");
                        cmd.Parameters.AddWithValue("@dept", userDepartment);

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listBoxFiles.Items.Add(reader["FileName"].ToString());
                            }
                        }
                    }
                }
            }

            private void btnOpenFile_Click(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedItem == null) return;

            string fileName = listBoxFiles.SelectedItem.ToString();
            string decryptedFilePath = DBHelper.GetFilePath(fileName, userDepartment);

            if (!string.IsNullOrEmpty(decryptedFilePath))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = decryptedFilePath,
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show("Access Denied!");
            }
        }
    }
}
