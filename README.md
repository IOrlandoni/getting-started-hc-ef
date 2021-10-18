# Get started with Hot Chocolate and Entity Framework
### Steps on how to implement a basic GraphQL query endpoint with filtering, paging, and sorting, using HotChocolate11 and EFCore

See original post here: https://www.softwareis.cool/hotchocolate-and-efcore/

- - - - - 
Going from a `DbContext` to an API could not be easier with [Hot Chocolate](https://github.com/ChilliCream/hotchocolate). You'll have a simple [GraphQL](https://graphql.org/) endpoint to query (including filtering, paging and sorting) faster than you can say *"representational state transfer"*. Let's start learning by doing!

<sub><sup>*2021, May 19th - Updated to HotChocolate12*</sup></sub>

### Contents
1. [Requirements](#requirements)
2. [Model tweaks](#model-tweaks)
3. [Install HotChocolate](#install-hotchocolate)
4. [Service configurations](#service-configurations)
5. [Adding root queries](#adding-root-queries)
6. [Manually test](#manually-test)
7. [Next steps](#next-steps)

## Requirements
You can get [here](https://github.com/IOrlandoni/getting-started-hc-ef) our starting project, or create your own, and follow along the tutorial. All our starting project has is:

##### ASP.NET Web Application
We're making a web API so you'll need a web API project... not exactly rocket science. We're using an empty `ASP.NET Core Web Application`.

##### Entity Framework Model
You should have an EF model already existing *within* the solution. If you are not quite sure what I'm talking about, check [how to get started with EF Core](https://docs.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli).

In our starting project, we are using the small model that you get from the "how to get started" sample, with `Blogs` and `Posts`. If you're using your own model, adapt our steps to fit your entities! It really is not that hard!

## Model tweaks
Because we will configure our DbContext when we register it for dependency injection, our context should not contain any hard-coded configuration and should pass any options during construction to the base class.

In our case this means deleting, within our `DbContext`, the `OnConfiguring(...)` method, and create a new empty constructor that allows for `DbContextOptions` like so:

```csharp
public BloggingContext(DbContextOptions options) : base(options)
{}
```

<sub><sup>*Resulting changes after this step, can be seen [here](https://github.com/IOrlandoni/getting-started-hc-ef/compare/steps%2F1-model-tweaks#toc).*</sup></sub>
## Install HotChocolate
Install packages `HotChocolate.Data.EntityFramework` and `HotChocolate.AspNetCore` in your project. You can do this from the Visual Studio package management UI, dotnet CLI, or package manager console. However you prefer!

<sub><sup>*Resulting changes after this step, can be seen [here](https://github.com/IOrlandoni/getting-started-hc-ef/compare/steps%2F1-model-tweaks...steps%2F2-installing-hotchocolate#toc).*</sup></sub>
## Service configurations

We'll need to register two services with our dependency injection container: GraphQL and our database context. For that, we go to our `Startup` class and within our `ConfigureServices(..)` method we add the following snippet:
```csharp
// Register our DbContextPool
services.AddPooledDbContextFactory<BloggingContext>(b => b
    //Context options
    .UseSqlite("Data Source=blogging.db")
);

// Build our schema and register it.
services
    .AddGraphQLServer();
```

For our last service configuration, we need to map our GraphQL endpoints.
Within our `Startup` class, and within our `Configure(...)` method, we configure the GraphQL endpoints by adding it to our route builder's `UseEndpoints(...)` call:
```csharp
app.UseEndpoints(endpoints =>
{
    // Other stuff might already exist here...
   
    // We map our GraphQL endpoint
    endpoints.MapGraphQL();
});
```

For those things to work, you'll need to add the correct `using` statements, which intellisense should suggest: `using Microsoft.EntityFrameworkCore;`

If we've done everything correctly, we should be able to start the project, navigate to `/graphql` and see `Banana Cake Pop`. This will be our GraphQL IDE!
(Also... cool people call it `BCP`. Now you know.)

<sub><sup>*Resulting changes after this step, can be seen [here](https://github.com/IOrlandoni/getting-started-hc-ef/compare/steps/2-installing-hotchocolate...steps/3-service-configurations#toc).*</sup></sub>

---

We now have the base for our project. We're serving up an empty schema through our endpoint, and we have a GraphQL IDE that we can use to explore and interact with our endpoint. Time to actually define our schema!

## Adding root queries

This is where the magic happens. Let's add a root query, so our users can enter the schema through it. In our case, we're adding both `blog` and `post`:

In a new folder called `Schema`, define your query class as follows:

```csharp
using EFGetStarted;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using System.Linq;

public class Queries
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
```

Your namespaces might differ from ours and you can always adapt it to your own entities.

Last step, is modifying the schema builder in our `Startup` to add the queries, and the middleware for filtering and sorting.

In our Startup class, in the ConfigureServices method, modify the section that adds the GraphQLServer so it looks like the following:
```csharp
// Build our schema and register it.
services
    .AddGraphQLServer()
    .AddQueryType<Queries>()
    .AddFiltering()
    .AddSorting();
```

<sub><sup>*Resulting changes after this step, can be seen [here](https://github.com/IOrlandoni/getting-started-hc-ef/compare/steps%2F3-service-configurations...steps%2F4-adding-queries#toc).*</sup></sub>

---

## Manually test
If everything works out, you should be able to see your queries in your IDE. 
Run your project, navigate to `BCP`, go to `Schema` and `Refresh`.

That is it. We now how a queryable GraphQL endpoint with sorting, paging and filtering that depends only on our data model. How easy was that? And how easy will it be to maintain?

## Next steps
What now? Well... a lot of things! You might want to look into how to add mutations, subscriptions, data loader, relay support...

Don't worry, it sounds a lot harder that it is. There is a workshop explaining the details, steps and anything else you might be interested in [right here](https://github.com/ChilliCream/graphql-workshop/).
