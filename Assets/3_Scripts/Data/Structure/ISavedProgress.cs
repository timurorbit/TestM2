namespace _3_Scripts.Data.Structure
{
    /// <summary>
    /// Represents a contract for data that can be saved and validated during the save/load process.
    /// Implementing types must provide logic to determine if their state is valid.
    /// </summary>
    public interface ISavedProgress
    {
        public bool isValid();
    }
}