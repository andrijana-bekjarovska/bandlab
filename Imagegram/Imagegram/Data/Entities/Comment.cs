using System;

namespace Imagegram.Data.Entities
{
    public class Comment
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedAt { get; set; }
        public long PostId { get; set; }
        public Account Account { get; set; }
        public Post Post { get; set; }
    }
}