using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

public partial class InsertToSql : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            InsertDataIntoSqlServer();
        }
    }

    private void InsertDataIntoSqlServer()
    {
        string connectionString = "my connection string";
        string csvFilePath = @"my path to file\cities.csv";

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string[] lines = File.ReadAllLines(csvFilePath);
                List<string> errors = new List<string>();

                foreach (string line in lines)
                {
                    string[] columns = line.Split(',');

                    if (columns.Length >= 6)
                    {
                        if (int.TryParse(columns[0], out int id) &&
                            int.TryParse(columns[2], out int stateId) &&
                            int.TryParse(columns[4], out int countryId))
                        {
                            string name = columns[1];
                            string stateCode = columns[3];
                            string countryCode = columns[5];

                            try
                            {
                                InsertRow(connection, id, name, stateId, stateCode, countryId, countryCode);
                            }
                            catch (Exception ex)
                            {
                                errors.Add($"Error inserting row with ID {id}: {ex.Message}");
                            }
                        }
                        else
                        {
                            errors.Add("Error: Unable to parse numeric values in the CSV file.");
                        }
                    }
                    else
                    {
                        errors.Add("Error: The CSV file does not have the expected number of columns.");
                    }
                }

                connection.Close();

                if (errors.Count > 0)
                {
                    ResultLiteral.Text = string.Join("<br/>", errors);
                }
                else
                {
                    ResultLiteral.Text = "Data inserted successfully!";
                }
            }
        }
        catch (Exception ex)
        {
            ResultLiteral.Text = $"Error: {ex.Message}";
        }
    }

    private void InsertRow(SqlConnection connection, int id, string name, int stateId, string stateCode, int countryId, string countryCode)
    {
        using (SqlCommand cmd = new SqlCommand("INSERT INTO world.dbo.cities (id, name, state_id, state_code, country_id, country_code) VALUES (@id, @name, @stateId, @stateCode, @countryId, @countryCode)", connection))
        {
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@stateId", stateId);
            cmd.Parameters.AddWithValue("@stateCode", stateCode);
            cmd.Parameters.AddWithValue("@countryId", countryId);
            cmd.Parameters.AddWithValue("@countryCode", countryCode);

            cmd.ExecuteNonQuery();
        }
    }
}
