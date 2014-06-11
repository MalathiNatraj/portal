namespace Diebold.Domain.Entities
{
    public abstract class TrackeableEntity : IntKeyedEntity
    {
        public virtual bool IsDisabled { get; set; }
        public virtual int? DeletedKey { get; set; }

        public virtual void Enable()
        {
            this.IsDisabled = false;
        }

        public virtual void Disable()
        {
            this.IsDisabled = true;
        }

        public virtual void LogicalDelete()
        {
            this.DeletedKey = this.Id;
        }
    }
}
