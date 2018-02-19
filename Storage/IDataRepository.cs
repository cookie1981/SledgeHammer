using System.Threading.Tasks;

namespace profiling.Storage
{
    public interface IDataRepository<T>
    {
        Task SaveAsync(T objectToSave);
        Task<T> FetchAsync(string wipId);
        Task<T> UpdateAsync(T objectToUpdate);
    }
}