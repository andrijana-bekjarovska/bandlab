using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Imagegram.Data;
using Imagegram.Services.Contracts;
using Imagegram.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Services
{
    public class CommentService : ICommentService
    {
        private readonly ImagegramContext _imagegramContext;
        private readonly IAccountService _accountService;

        public CommentService(ImagegramContext imagegramContext, IAccountService accountService)
        {
            _imagegramContext = imagegramContext;
            _accountService = accountService;
        }

        public async Task<Comment> GetById(long id)
        {
            var commentEntity = await _imagegramContext.Comments.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);

            if (commentEntity == null)
                throw new Exception(ErrorCodes.ResourceNotFound);

            return new Comment
            {
                Id = commentEntity.Id,
                Creator = commentEntity.Creator,
                CreatedAt = commentEntity.CreatedAt,
                PostId = commentEntity.PostId
            };
        }

        public IEnumerable<Comment> GetByPostId(long postId, int? start, int? count)
        {
            IQueryable<Data.Entities.Comment> commentEntities;
            if (start.HasValue && count.HasValue)
            {
                commentEntities = _imagegramContext.Comments.AsNoTracking()
                    .Where(y => y.PostId == postId).OrderByDescending(x => x.Id)
                    .Skip(start.Value)
                    .Take(count.Value);
            }
            else
            {
                commentEntities = _imagegramContext.Comments.AsNoTracking()
                    .Where(y => y.PostId == postId).OrderByDescending(x => x.Id);
            }


            foreach (var comment in commentEntities)
            {
                yield return new Comment
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    Creator = comment.Creator,
                    CreatedAt = comment.CreatedAt,
                    PostId = comment.PostId
                };
            }
        }

        public async Task<Comment> CreateComment(Comment comment)
        {
            await _accountService.GetById(comment.Creator);

            var commentEntity = new Data.Entities.Comment
            {
                Content = comment.Content,
                Creator = comment.Creator,
                CreatedAt = DateTime.UtcNow,
                PostId = comment.PostId
            };

            await _imagegramContext.Comments.AddAsync(commentEntity);
            await _imagegramContext.SaveChangesAsync();

            comment.Id = commentEntity.Id;
            comment.CreatedAt = commentEntity.CreatedAt;

            return comment;
        }

        public async Task DeleteComment(long id, string creator)
        {
            var commentEntity = await _imagegramContext.Comments.AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id && x.Creator == creator);
            if (commentEntity == null)
                throw new Exception(ErrorCodes.ResourceNotFound);

            _imagegramContext.Comments.Remove(commentEntity);
            await _imagegramContext.SaveChangesAsync();
        }
    }
}