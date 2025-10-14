namespace MilkTeaShop.Domain.Interfaces;

public interface IRepository<T>
{
    T? GetById(string id);
    IEnumerable<T> GetAll();
    void Add(T entity);
    void Update(T entity);
    void Remove(string id);
}
