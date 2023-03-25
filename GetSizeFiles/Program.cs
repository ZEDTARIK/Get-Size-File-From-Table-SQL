using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace GetSizeFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectiontring = ConfigurationManager.ConnectionStrings["ConnectionData"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(connectiontring);

            try
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = new SqlCommand("SELECT Systemfield3, PathFile FROM udvw_GetListPathDocs ;", sqlConnection);
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                while (sqlDataReader.Read())
                {
                    string query = "INSERT INTO FileWeights (Systemfield3,SizeFile) values (@systemfield3,@SizeFile)";
                    SqlCommand sqlCommandInsert = new SqlCommand(query, sqlConnection);
                    string[] filePaths = Directory.GetFiles(Path.GetDirectoryName(sqlDataReader.GetString(1)), Path.GetFileName(sqlDataReader.GetString(1)));

                    FileInfo fileInfo = new FileInfo(filePaths[0]);

                    if(fileInfo.Exists)
                    {
                        long filesize = fileInfo.Length;

                        sqlCommandInsert.Parameters.AddWithValue("systemfield3", sqlDataReader.GetInt32(0));
                        sqlCommandInsert.Parameters.AddWithValue("@SizeFile", filesize);
                        sqlCommandInsert.ExecuteNonQuery();
                    }
                }

                sqlConnection.Close();
                Console.WriteLine("operation Success");

            }
            catch (System.Data.SqlClient.SqlException ex )
            {

                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }
    }
}
