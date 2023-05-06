namespace FinalIS.Models
{
    public class Token
    {
        public string token { get; set; }
        public DateTime expirationTime { get; set; }
        public string username { get; set; }
        public int id { get; set; }
    }
}
