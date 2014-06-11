using System.Collections.Generic;
namespace Diebold.Domain.Entities
{
    public enum PreferredContact
    {
        Phone,
        Office,
        Mobile
    }

    public class User : TrackeableEntity
    {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Username { get; set; }
        public virtual string Email { get; set; }
        public virtual string Phone { get; set; }
        public virtual string OfficePhone { get; set; }       
        public virtual string Extension { get; set; }
        public virtual string Mobile { get; set; }
        public virtual PreferredContact PreferredContact { get; set; }
        public virtual string Title { get; set; }
        public virtual string Text1 { get; set; }
        public virtual string Text2 { get; set; }
        public virtual string TimeZone { get; set; }
        public virtual string UserPin { get; set; }

        public virtual Role Role { get; set; }
        public virtual Company Company { get; set; }
        public virtual IList<Link> Links { get; set; }
        public virtual IList<RSSFeed> RSSFeeds { get; set; }
        public virtual IList<UserPortletsPreferences> userPortletsPreferences { get; set; }

        public User()
        {
            Links = new List<Link>();
            RSSFeeds = new List<RSSFeed>();
            userPortletsPreferences = new List<UserPortletsPreferences>();
        }

        public virtual string Name
        {
            get { return FirstName + " " + LastName; }
            set { }
        }

        public override string ToString()
        {
            return "User " + Username;
        }
    }
}
