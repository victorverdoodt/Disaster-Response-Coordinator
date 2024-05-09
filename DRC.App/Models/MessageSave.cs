using System.Data;

namespace DRC.App.Models
{
    public class MessageSave
    {
        public string? Prompt { get; set; }
        public int Role { get; set; }
        public int Tokens { get; set; }
    }
}
