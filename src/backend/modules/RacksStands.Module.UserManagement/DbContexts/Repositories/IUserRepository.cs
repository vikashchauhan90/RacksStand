using RacksStands.Module.UserManagement.DbContexts.Entities;

namespace RacksStands.Module.UserManagement.DbContexts.Repositories;

internal interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
}
