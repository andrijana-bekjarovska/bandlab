using System.Threading.Tasks;
using Imagegram.Services.Models;

namespace Imagegram.Services.Contracts
{
    public interface IAccountService
    {
        Task<Account> GetById(string id);
        Task<Account> CreateAccount(Account account);
        Task DeleteAccount(string id);
    }
}