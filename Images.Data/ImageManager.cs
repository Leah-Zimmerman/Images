using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace Images.Data
{

    public class ImageManager
    {
        private string _connectionString;
        private static Dictionary<string, int> _imageDictionary = new Dictionary<string, int>();
        public ImageManager(string connectionString)
        {
            _connectionString = connectionString;
        }
        public int AddAndGetId(string imagePath, string password, string fileName)
        {
            using var conn = new SqlConnection(_connectionString);
            using var comm = conn.CreateCommand();
            comm.CommandText = "INSERT INTO Images (ImagePath,Password,FileName,Views) VALUES (@imagePath, @password, @fileName, 1); SELECT SCOPE_IDENTITY()";
            comm.Parameters.AddWithValue("@imagePath", imagePath);
            comm.Parameters.AddWithValue("@password", password);
            comm.Parameters.AddWithValue("@fileName", fileName);
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
                    Password = (string)reader["Password"],
                    FileName = (string)reader["FileName"],
                    Views = (int)reader["Views"]
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
                    Password = (string)reader["Password"],
                    FileName = (string)reader["FileName"],
                    Views = (int)reader["Views"]
                };
            }
            conn.Close();
            return image;
        }
        public void IncreaseImageViews(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var comm = conn.CreateCommand();
            comm.CommandText = "UPDATE Images SET Views = Views + 1 WHERE Id=@id;";
            comm.Parameters.AddWithValue("@id", id);
            conn.Open();
            comm.ExecuteNonQuery();
            conn.Close();
        }
        //public void AddToImageDictionary(string fileName)
        //{
        //    _imageDictionary.Add(fileName, 1);
        //}
        //public int GetValueOfImageDictionary(string fileName)
        //{
        //    return _imageDictionary[fileName];
        //}
        //public void IncreaseValueOfImageDictionary(string fileName)
        //{
        //    _imageDictionary[fileName]++;
        //}
        //public Dictionary<string,int>GetImageDictionary()
        //{
        //    var d = _imageDictionary;
        //    return d;
        //}
    }

}