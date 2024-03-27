namespace ClinicalTrials.Core
{
    public interface IDataUtility<T>
    {
        Task<List<String>> GetAllKeys();
        Task<List<T>> GetAllItems();
        void DeleteItem(string key);
        void RenameItem(string key, string newName);
        Task<T> Load(string key);
        Task Save(string? key, T? obj);
        Task Save(string? key, string json);
        Task<string> GetData(string key);
        Task ClearAll();
    }
}