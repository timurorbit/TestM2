using _3_Scripts.Data.Structure;

namespace _3_Scripts.Data.Services
{
    public interface ISaveSystem<T> where T : ISavedProgress
    {
        void Save(T progress);
        
        T Load();
        
        bool SaveExists();

        T Reset();
        
    }
}