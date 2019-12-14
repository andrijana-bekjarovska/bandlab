using System.Collections.Generic;
using System.Threading.Tasks;
using Imagegram.Services.Models;

namespace Imagegram.Services.Contracts
{
    public interface ICommentService
    {
        Task<Comment> GetById(long id);
        IEnumerable<Comment> GetByPostId(long postId, int? start, int? count);
        Task<Comment> CreateComment(Comment post);

        Task DeleteComment(long id, string creator);
    }
}