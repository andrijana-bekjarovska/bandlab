using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Imagegram.Data;
using Imagegram.Services.Contracts;
using Imagegram.Services.Models;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Imagegram.Services
{
    public class PostService : IPostService
    {
        private readonly IAccountService _accountService;
        private readonly ImagegramContext _imagegramContext;
        private readonly ICommentService _commentService;

        public PostService(ImagegramContext imagegramContext, IAccountService accountService,
            ICommentService commentService)
        {
            _imagegramContext = imagegramContext;
            _accountService = accountService;
            _commentService = commentService;
        }

        public async Task<Post> GetById(long id)
        {
            var postEntity = await _imagegramContext.Posts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);

            if (postEntity == null)
                throw new Exception(ErrorCodes.ResourceNotFound);

            return new Post
            {
                Id = postEntity.Id,
                Creator = postEntity.Creator,
                CreatedAt = postEntity.CreatedAt,
                ImageUrl = postEntity.ImageUrl
            };
        }

        public IEnumerable<Post> GetAll(int commentCount, int? start, int? count)
        {
            IQueryable<Data.Entities.Post> postEntities;
            if (start.HasValue && count.HasValue)
            {
                postEntities = _imagegramContext.Posts.AsNoTracking()
                    .OrderByDescending(x => x.Comments.Count)
                    .Skip(start.Value)
                    .Take(count.Value);
            }
            else
            {
                postEntities = _imagegramContext.Posts.AsNoTracking()
                    .OrderByDescending(x => x.Comments.Count);
            }
         
            
            var posts = new List<Post>();

            foreach (var post in postEntities)
            {
                var comments = _commentService.GetByPostId(post.Id, null, null);
                posts.Add(new Post
                {
                    Id = post.Id,
                    Comments = comments.Take(commentCount),
                    Creator = post.Creator,
                    CreatedAt = post.CreatedAt,
                    ImageUrl = post.ImageUrl
                });
            }

            return posts;
        }


        public async Task<Post> CreatePost(Post post)
        {
            await _accountService.GetById(post.Creator);

            var imageGuid = Guid.NewGuid();
            var postEntity = new Data.Entities.Post
            {
                Creator = post.Creator,
                CreatedAt = DateTime.UtcNow,
                ImageUrl = $"/Image/{imageGuid}"
            };
            await _imagegramContext.Posts.AddAsync(postEntity);

            // Load the image.
            using (var imageToResize = Image.Load(post.ImageStream))
            {
                const int newWidth = 640;
                var newHeight = newWidth * imageToResize.Height / imageToResize.Width;

                imageToResize.Mutate(x => x
                    .Resize(newWidth, newHeight));
                imageToResize.Save($@"UploadedImages\{imageGuid}.jpg", new JpegEncoder());
            }

            await _imagegramContext.SaveChangesAsync();

            post.ImageUrl = postEntity.ImageUrl;
            post.Id = postEntity.Id;
            post.CreatedAt = postEntity.CreatedAt;
            return post;
        }
    }
}