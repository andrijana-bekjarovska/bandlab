using System;

namespace Imagegram.Models.Comment
{
    public class CommentResponse : CommentBase
    {
        public long Id { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}