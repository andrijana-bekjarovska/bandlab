using System;
using System.Collections;
using System.Collections.Generic;

namespace Imagegram.Services.Models
{
    public class Post
    {
        public long Id { get; set; }
        public string ImageUrl { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedAt { get; set; }
        public byte[] ImageStream { get; set; }

        public IEnumerable<Comment> Comments { get; set; }
    }
}