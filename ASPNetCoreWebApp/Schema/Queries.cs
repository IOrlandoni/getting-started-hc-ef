using EFGetStarted;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using System.Linq;

namespace ASPNetCoreWebApp.Schema
{
    public partial class Queries
    {
        // The order of these HC annotations matters!
        [UseDbContext(typeof(BloggingContext))]
        [UsePaging]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Blog> GetBlogs([ScopedService] BloggingContext dbContext)
            => dbContext.Blogs;

        // The order of these HC annotations matters!
        [UseDbContext(typeof(BloggingContext))]
        [UsePaging]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Post> GetPosts([ScopedService] BloggingContext dbContext)
            => dbContext.Posts;
    }
}
