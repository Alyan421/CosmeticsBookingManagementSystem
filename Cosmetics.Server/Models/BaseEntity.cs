namespace Cosmetics.Server.Models
{
    public abstract class BaseEntity<TId>
    {
        public TId Id { get; set; }
    }
}
