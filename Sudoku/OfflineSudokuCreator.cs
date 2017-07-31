using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class OfflineSudokuCreator
    {
        private static SqlConnection connection = new SqlConnection();
        public const string CONNECTION_STRING = @"workstation id = SudokuSolver.mssql.somee.com; packet size = 4096; user id = Cecosam_SQLLogin_1; pwd = zok2ocsnoo; data source = SudokuSolver.mssql.somee.com; persist security info = False; initial catalog = SudokuSolver";
        public static void CreateOfflineSudokusFile()
        {
            connection.ConnectionString = CONNECTION_STRING;
            using (File.Create("\\SudokuCases.txt"))
            {

            }
            StringBuilder result = new StringBuilder();
            result.AppendLine("switch (number)");
            result.AppendLine("{");

            var sudokuAsString = string.Empty;
            try
            {
                connection.Open();
                var queryString = string.Format(string.Format("SELECT TOP 100 SudokuValues,Difficulty FROM Sudoku where Difficulty = {0}", 8));
                SqlCommand query = new SqlCommand(queryString, connection);
                SqlDataReader reader = query.ExecuteReader();
                var counter = 0;

                while (reader.Read())
                {
                    sudokuAsString = reader[0] as string;
                    result.AppendLine("case " + counter + ": return " + "\"" + sudokuAsString + "\";");
                    counter++;
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
            finally
            {
                connection.Close();
            }

            result.AppendLine("default: return null;");
            result.AppendLine("}");

            File.WriteAllLines("\\SudokuCases.txt", new string[] { result.ToString() });
        }
    }
}
