using System.Threading.Tasks;
using _3_Scripts.Data.Structure;

namespace _3_Scripts.Data.Services
{
    /// <summary>
    /// Defines a contract for a save system that manages saving and loading progress data of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of progress data to manage, must implement <see cref="ISavedProgress"/>.</typeparam>
    public interface ISaveSystem<T> where T : ISavedProgress
    {
        /// <summary>
        /// Saves the specified progress data asynchronously.
        /// </summary>
        /// <param name="progress">The progress data to save.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        Task Save(T progress);
        
        /// <summary>
        /// Loads the saved progress data asynchronously.
        /// </summary>
        /// <returns>A task that resolves to the loaded progress data of type <typeparamref name="T"/>.</returns>
        Task<T> Load();
        
        /// <summary>
        /// Checks if a save file exists.
        /// </summary>
        /// <returns>True if a save exists; otherwise, false.</returns>
        bool SaveExists();
    }
}