using System;
using System.Collections.Generic;

namespace Imagegram.Data.Entities
{
    public class Post
    {
        public long Id { get; set; }
        public string ImageUrl { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedAt { get; set; }

        public Account Account { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}