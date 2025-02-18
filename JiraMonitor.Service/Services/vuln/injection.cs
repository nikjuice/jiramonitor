using System;
using System.Data.SqlClient;

class Program
{
    static void Main()
    {
        Console.Write("Enter your username: ");
        string username = Console.ReadLine();
        string password = "Hardcodeones";
        string connectionString = "your_connection_string_here";
        string query = "SELECT * FROM Users WHERE Username = '" + username + "'";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
                Console.WriteLine("User found!");
            else
                Console.WriteLine("User not found.");

            reader.Close();
        }
    }
}