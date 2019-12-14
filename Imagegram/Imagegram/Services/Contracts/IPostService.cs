using System.Collections.Generic;
using System.Threading.Tasks;
using Imagegram.Services.Models;

namespace Imagegram.Services.Contracts
{
    public interface IPostService
    {
        Task<Post> GetById(long id);
        IEnumerable<Post> GetAll(int commentCount, int? start, int? count);
        Task<Post> CreatePost(Post post);
    }
}