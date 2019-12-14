using System;

namespace Imagegram.Models.Post
{
    public class PostResponse
    {
        public long Id { get; set; }
        public string ImageUrl { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}