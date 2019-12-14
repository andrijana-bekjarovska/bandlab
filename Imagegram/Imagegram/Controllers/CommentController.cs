﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IPostService _postService;

        public CommentController(ICommentService commentService, IPostService postService)
        {
            _commentService = commentService;
            _postService = postService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<ActionResult<CommentResponse>> Get([FromRoute] long id)
        {
            try
            {
                var comment = await _commentService.GetById(id);
                var response = new CommentResponse
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    Creator = comment.Creator,
                    CreatedAt = comment.CreatedAt,
                    PostId = comment.PostId
                };
                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }


        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CommentResponse>> Post([FromBody] CommentRequest request,
            [FromHeader(Name = "X-Account-Id")] string accountId)
        {
            try
            {
                await _postService.GetById(request.PostId);

                var comment = new Comment
                {
                    Content = request.Content,
                    Creator = accountId,
                    PostId = request.PostId
                };

                var response = await _commentService.CreateComment(comment);

                var actualResponse = new CommentResponse
                {
                    Id = response.Id,
                    Creator = response.Creator,
                    CreatedAt = response.CreatedAt,
                    Content = response.Content,
                    PostId = response.PostId
                };
                return CreatedAtAction(nameof(Get), new {id = response.Id}, actualResponse);
            }
            catch (Exception ex)
            {
                return ex.Message == ErrorCodes.ResourceNotFound ? BadRequest() : StatusCode(500);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<ActionResult> Delete([FromRoute] long id,
            [FromHeader(Name = "X-Account-Id")] string accountId)
        {
            try
            {
                await _commentService.DeleteComment(id, accountId);

                return Ok();
            }
            catch (Exception ex)
            {
                return ex.Message == ErrorCodes.ResourceNotFound ? BadRequest() : StatusCode(500);
            }
        }
    }
}