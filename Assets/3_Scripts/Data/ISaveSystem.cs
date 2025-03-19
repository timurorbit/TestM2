namespace _3_Scripts.Data
{
    public interface ISaveSystem<T> where T : SavedProgress
    {
        void Save(T progress);
        
        T Load();
        
        bool SaveExists();

        bool Reset();
        
    }
}