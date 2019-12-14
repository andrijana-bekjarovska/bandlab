namespace Imagegram.Models.Comment
{
    public abstract class CommentBase
    {
        public string Content { get; set; }
        public long PostId { get; set; }
    }
}