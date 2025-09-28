using MilkTeaShop.Domain.Interfaces;

namespace MilkTeaShop.Infrastructure.Repositories;

public class InMemoryRepository<T> : IRepository<T> where T : class
{
    private readonly Dictionary<string, T> _store = new();
    private readonly Func<T, string> _keySelector;

    public InMemoryRepository(Func<T, string> keySelector) => _keySelector = keySelector;

    public void Add(T entity) => _store[_keySelector(entity)] = entity;
    public IEnumerable<T> GetAll() => _store.Values;
    public T? GetById(string id) => _store.TryGetValue(id, out var e) ? e : null;
    public void Remove(string id) => _store.Remove(id);
    public void Update(T entity) => _store[_keySelector(entity)] = entity;
}
