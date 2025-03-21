using System.Threading.Tasks;
using _3_Scripts.Data.Structure;

namespace _3_Scripts.Data.Services
{
    public interface ISaveSystem<T> where T : ISavedProgress
    {
        Task Save(T progress);
        
        Task<T> Load();
        
        bool SaveExists();

        Task<T> Reset();
        
    }
}