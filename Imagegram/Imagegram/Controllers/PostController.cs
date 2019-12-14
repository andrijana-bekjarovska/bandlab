using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Imagegram.Models.Account;
using Imagegram.Models.Comment;
using Imagegram.Models.Post;
using Imagegram.Services.Contracts;
using Imagegram.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Imagegram.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;
        private readonly string[] allowedExtenstions = new string[] {".jpg", ".png", ".bmp"};

        public PostController(IPostService postService, ICommentService commentService)
        {
            _postService = postService;
            _commentService = commentService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<ActionResult<PostResponse>> Get([FromRoute] long id)
        {
            var post = await _postService.GetById(id);
            var response = new PostResponse
            {
                Id = post.Id,
                Creator = post.Creator,
                CreatedAt = post.CreatedAt,
                ImageUrl = post.ImageUrl
            };
            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public ActionResult<IEnumerable<PostWithCommentsResponse>> Get([FromQuery] int? start = null, [FromQuery] int? count= null,
            [FromQuery] int commentCount = 3)
        {
            var postList = _postService.GetAll(commentCount, start, count);

            var response = postList.Select(x => new PostWithCommentsResponse
            {
                Id = x.Id,
                Creator = x.Creator,
                CreatedAt = x.CreatedAt,
                ImageUrl = x.ImageUrl,
                CommentResponses = x.Comments.Select(z => new CommentResponse
                {
                    Id = z.Id,
                    Content = z.Content,
                    Creator = z.Creator,
                    CreatedAt = z.CreatedAt,
                    PostId = z.PostId
                })
            });

            return Ok(response);
        }

        [HttpGet("{id}/Comments")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<CommentResponse>>> GetComments([FromRoute] long id, [FromQuery] int? start = null, [FromQuery] int? count= null)
        {
            await _postService.GetById(id);

            var commentList = _commentService.GetByPostId(id, start, count);

            var response = commentList.Select(x => new CommentResponse
            {
                Id = x.Id,
                Content = x.Content,
                Creator = x.Creator,
                CreatedAt = x.CreatedAt,
                PostId = x.PostId
            });

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<ActionResult<PostResponse>> Post([FromForm(Name = "uploadedFile")] IFormFile file,
            [FromHeader(Name = "X-Account-Id")] string accountId)
        {
            byte[] fileBytes;
            if (!allowedExtenstions.Contains(Path.GetExtension(file.FileName).ToLower()))
                throw new Exception(ErrorCodes.UnsupportedFile);

            await using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
            }

            var post = new Post
            {
                Creator = accountId,
                ImageStream = fileBytes
            };

            var response = await _postService.CreatePost(post);
            var actualResponse = new PostResponse
            {
                Creator = response.Creator,
                Id = response.Id,
                CreatedAt = response.CreatedAt,
                ImageUrl = response.ImageUrl
            };
            return CreatedAtAction(nameof(Get), new {id = response.Id}, actualResponse);
        }
    }
}