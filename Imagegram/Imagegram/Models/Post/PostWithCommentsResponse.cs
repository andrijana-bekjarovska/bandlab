using System.Collections;
using System.Collections.Generic;
using Imagegram.Models.Comment;

namespace Imagegram.Models.Post
{
    public class PostWithCommentsResponse : PostResponse
    {
        public IEnumerable<CommentResponse> CommentResponses { get; set; }
    }
}