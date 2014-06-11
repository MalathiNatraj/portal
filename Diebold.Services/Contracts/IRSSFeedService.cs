using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diebold.Domain.Entities;

namespace Diebold.Services.Contracts
{
    public interface IRSSFeedService : ICRUDTrackeableService<RSSFeed>
    {
        IList<RSSFeed> GetAllRSSFeedsByUser(int UserId);
        IList<RSSFeed> GetAllActiveRSSFeedsByUser(int UserId);
    }
}
