namespace EasyPersist.Core.IFaces
{
    /// <summary>
    /// Interface for objects stored in db
    /// </summary>
    public interface IPersistent
    {
        int Id
        {
            get;
            set;
        }
    }
}
