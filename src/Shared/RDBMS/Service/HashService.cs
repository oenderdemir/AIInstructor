namespace AIInstructor.src.Shared.RDBMS.Service
{
    public class HashService : IHashService
    {
        public async Task<string> ComputeHash(string data)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(data);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }

        }
    }
}
