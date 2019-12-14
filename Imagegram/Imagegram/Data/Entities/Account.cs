﻿using System.Collections.Generic;

namespace Imagegram.Data.Entities
{
    public class Account
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public ICollection<Post> Posts { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
