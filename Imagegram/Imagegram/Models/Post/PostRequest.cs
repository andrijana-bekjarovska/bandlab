using Microsoft.AspNetCore.Http;

namespace Imagegram.Models.Post
{
    public class PostRequest
    {
        public IFormFile Image { get; set; }
    }
}