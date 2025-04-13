using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;

public static class DBHelper
{
    private static string connectionString = "Data Source=C:\\Users\\kilot\\OneDrive\\Desktop\\SecureFiles.db;Version=3;";

    public static string GetUserName(int userID)
    {
        string query = "SELECT Name FROM Users WHERE ID = @UserID";
        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
        {
            conn.Open();
            cmd.Parameters.AddWithValue("@UserID", userID);
            object result = cmd.ExecuteScalar();
            return result?.ToString() ?? string.Empty;
        }
    }

    public static string GetUserDepartment(int userID)
    {
        string query = "SELECT Department FROM Users WHERE ID = @UserID";
        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
        {
            conn.Open();
            cmd.Parameters.AddWithValue("@UserID", userID);
            object result = cmd.ExecuteScalar();
            return result?.ToString() ?? string.Empty;
        }
    }

    public static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }

    public static void RegisterUser(string name, string department, string password)
    {
        string hashedPassword = HashPassword(password);

        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();

            string insertUserQuery = "INSERT INTO Users (Name, Department) VALUES (@name, @department)";
            using (SQLiteCommand cmd = new SQLiteCommand(insertUserQuery, conn))
            {
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@department", department);
                cmd.ExecuteNonQuery();
            }

            long userId = conn.LastInsertRowId;

            string insertPasswordQuery = "INSERT INTO Passwords (UserID, HashedPassword) VALUES (@userId, @hashedPassword)";
            using (SQLiteCommand cmd = new SQLiteCommand(insertPasswordQuery, conn))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@hashedPassword", hashedPassword);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public static bool ValidateUser(int userID, string password)
    {
        string hashedPassword = HashPassword(password);

        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();

            string passwordQuery = "SELECT 1 FROM Passwords WHERE UserID = @userID AND HashedPassword = @hashedPassword";
            using (SQLiteCommand cmd = new SQLiteCommand(passwordQuery, conn))
            {
                cmd.Parameters.AddWithValue("@userID", userID);
                cmd.Parameters.AddWithValue("@hashedPassword", hashedPassword);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    return reader.Read();
                }
            }
        }
    }
    public static List<string> GetAccessibleFiles(string department)
    {
        List<string> files = new List<string>();

        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();

            string query = "SELECT FileName FROM Files WHERE AllowedDepartments LIKE @department";

            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@department", "%" + department + "%");

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        files.Add(reader["FileName"].ToString());
                    }
                }
            }
        }

        return files;
    }

    public static SQLiteConnection GetConnection()
    {
        return new SQLiteConnection(connectionString);
    }

    public static string GetFilePath(string fileName, string userDepartment)
    {
        using (SQLiteConnection conn = GetConnection())
        {
            conn.Open();
            string query = "SELECT FilePath FROM Files WHERE FileName = @fileName AND AllowedDepartments LIKE @dept";

            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@fileName", fileName);
                cmd.Parameters.AddWithValue("@dept", "%" + userDepartment + "%");

                object result = cmd.ExecuteScalar();
                return result?.ToString();
            }
        }
    }
}


