using System;

namespace Diebold.Domain.Entities
{
    public class CompanySubscription : IEquatable<CompanySubscription>
    {
        public virtual Subscription Subscription { get; set; }

        public bool Equals(CompanySubscription other)
        {
            return this.Subscription == other.Subscription;
        }
    }
}