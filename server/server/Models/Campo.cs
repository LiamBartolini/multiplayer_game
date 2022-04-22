using Newtonsoft.Json;

namespace server.Models
{
    public class Campo
    {
        public string segno { get; set; }
        public string[,] field { get; set; }
    }
}