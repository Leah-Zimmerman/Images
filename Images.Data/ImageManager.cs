using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;

namespace Images.Data
{
    public class ImageManager
    {
        private string _connectionString;
        public ImageManager(string connectionString)
        {
            _connectionString = connectionString;
        }
        public int AddAndGetId(string imagePath, string password)
        {
            using var conn = new SqlConnection(_connectionString);
            using var comm = conn.CreateCommand();
            comm.CommandText = "INSERT INTO Images (ImagePath,Password) VALUES (@imagePath, @password); SELECT SCOPE_IDENTITY()";
            comm.Parameters.AddWithValue("@imagePath", imagePath);
            comm.Parameters.AddWithValue("@password", password);
            conn.Open();
            int id = (int)(decimal)comm.ExecuteScalar();
            conn.Close();
            return id;
        }
        public List<Image> GetImages()
        {
            using var conn = new SqlConnection(_connectionString);
            using var comm = conn.CreateCommand();
            comm.CommandText = "SELECT * FROM Images";
            conn.Open();
            var reader = comm.ExecuteReader();
            var images = new List<Image>();
            while (reader.Read())
            {
                var image = new Image
                {
                    Id = (int)reader["Id"],
                    ImagePath = (string)reader["ImagePath"],
                    Password = (string)reader["Password"]
                };
                images.Add(image);
            }
            conn.Close();
            return images;
        }
        public Image GetImageById(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var comm = conn.CreateCommand();
            comm.CommandText = "SELECT * FROM Images WHERE Id = @id";
            comm.Parameters.AddWithValue("@id", id);
            conn.Open();
            var reader = comm.ExecuteReader();
            var image = new Image();
            while (reader.Read())
            {
                image = new Image
                {
                    Id = (int)reader["Id"],
                    ImagePath = (string)reader["ImagePath"],
                    Password = (string)reader["Password"]
                };
                return image;
            }
            conn.Close();
            return image;
        }
    }
}