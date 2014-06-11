using System;

namespace Diebold.Domain.Entities
{
    public class Note : TrackeableEntity
    {
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public Device Device { get; set; }
        public User User { get; set; }
        
        public override string ToString()
        {
            return "Note \"" + (Text.Length > 30 ? Text.Substring(0, 30) : Text) + "...\" for " + Device.ToString();
        }
    }
}
