using System.Security.Cryptography;
using System.Text;

namespace ArtCosplay.Models
{
    public class HashedPathGenerator
    { 
        public static string GeneratePath(IFormFile file)
        {
            List<string> acceptableExtensions =
                [".jpg", ".jpeg", ".png", ".webp"];

            string extension = Path.GetExtension(file.FileName);

            string path = string.Empty;
            using (SHA256 mySHA256 = SHA256.Create())
            {
                byte[] hash = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(file.FileName + DateTime.Now.ToString()));
                path = new string(BitConverter.ToString(hash).Where(x => x != '-').ToArray()) + extension;
            }

            return path;
        }
    }
}
