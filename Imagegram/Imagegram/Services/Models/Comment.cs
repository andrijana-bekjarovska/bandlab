using System;

namespace Imagegram.Services.Models
{
    public class Comment
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedAt { get; set; }
        public long PostId { get; set; }
    }
}