using Db.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Db;

public class MongoDbContext : DbContext
{
    MongoClient Client { get; init; }
    public DbSet<UserRecord> Users { get; init; }
    public DbSet<ClientRecord> AuthClients { get; init; }
    public DbSet<ArticleRecord> Articles { get; init; }
    public DbSet<ArticleBlockRecord> ArticleBlocks { get; set; }
    public DbSet<WebSiteRecord> WebSites { get; init; }
    public DbSet<CategoryRecord> Categories { get; init; }
    public DbSet<WebSiteArticleRecord> WebSiteArticles { get; init; }
    public DbSet<AuthorizationTokenRecord> AuthTokens { get; init; }
    public DbSet<YoutubeChannelRecord> YoutubeChannels { get; init; }
    public DbSet<SecurityGroupRecord> SecurityGroups { get; init; }
    public DbSet<WordBookRecord> WordBooks { get; init; }

    public MongoDbContext(MongoClient client)
     : base(new DbContextOptionsBuilder<MongoDbContext>().UseMongoDB(client, "main").Options)
    {
        Client = client;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CategoryRecord>().ToCollection("categories");
        modelBuilder.Entity<UserRecord>().ToCollection("users");
        modelBuilder.Entity<ClientRecord>().ToCollection("authClients");
        modelBuilder.Entity<ArticleRecord>().ToCollection("articles");
        modelBuilder.Entity<WebSiteRecord>().ToCollection("webSites");
        modelBuilder.Entity<ArticleBlockRecord>().ToCollection("articleBlocks");
        modelBuilder.Entity<WebSiteArticleRecord>().ToCollection("webSiteArticles");
        modelBuilder.Entity<AuthorizationTokenRecord>().ToCollection("authTokens");
        modelBuilder.Entity<YoutubeChannelRecord>().ToCollection("youtubeChannels");
        modelBuilder.Entity<SecurityGroupRecord>().ToCollection("securityGroups");

        modelBuilder.Entity<WebSiteArticleRecord>().HasQueryFilter(w => w.DeletedAt == null);
        modelBuilder.Entity<WebSiteArticleRecord>().HasIndex(w => w.DeletedAt);

        modelBuilder.Entity<ArticleRecord>().HasOne(a => a.Media);
        modelBuilder.Entity<ArticleRecord>().HasMany(a => a.Blocks).WithOne(b => b.Article).HasForeignKey(b => b.ArticleId);
        modelBuilder.Entity<ArticleRecord>().HasMany(a => a.WebSiteArticles).WithOne(w => w.Article).HasForeignKey(w => w.ArticleId);
        modelBuilder.Entity<ArticleBlockRecord>().HasOne(b => b.Article);
        modelBuilder.Entity<WebSiteRecord>().HasOne(w => w.Parent).WithMany(w => w.SubSites).HasForeignKey(w => w.ParentId);
        modelBuilder.Entity<WebSiteRecord>().HasOne(w => w.User).WithMany().HasForeignKey(w => w.UserId);
        modelBuilder.Entity<WebSiteArticleRecord>().HasOne(w => w.Article).WithMany(a => a.WebSiteArticles).HasForeignKey(w => w.ArticleId);
        modelBuilder.Entity<WebSiteArticleRecord>().HasOne(w => w.WebSite).WithMany(a => a.WebSiteArticles).HasForeignKey(w => w.WebSiteId);
        modelBuilder.Entity<WebSiteArticleRecord>().Property(w => w.Id).IsRequired().ValueGeneratedOnAdd();
        modelBuilder.Entity<ArticleRecord>()
          .HasMany(a => a.WebSites)
          .WithMany(e => e.Articles)
          .UsingEntity<WebSiteArticleRecord>(
            r => r.HasOne(e => e.WebSite).WithMany(e => e.WebSiteArticles).HasForeignKey(e => e.WebSiteId),
            l => l.HasOne(e => e.Article).WithMany(e => e.WebSiteArticles).HasForeignKey(e => e.ArticleId)
          );
        modelBuilder.Entity<WebSiteRecord>()
          .HasMany(w => w.Articles)
          .WithMany(e => e.WebSites)
          .UsingEntity<WebSiteArticleRecord>(
              r => r.HasOne(e => e.Article).WithMany(e => e.WebSiteArticles).HasForeignKey(e => e.ArticleId),
              l => l.HasOne(e => e.WebSite).WithMany(e => e.WebSiteArticles).HasForeignKey(e => e.WebSiteId)
          );

        Client.GetDatabase("main").GetCollection<WordBookRecord>("WordBooks").Indexes.CreateOne(
            new CreateIndexModel<WordBookRecord>(
                Builders<WordBookRecord>.IndexKeys.Ascending(w => w.En),
                new CreateIndexOptions { Name = "En" }
            )
        );
        Client.GetDatabase("main").GetCollection<WordBookRecord>("WordBooks").Indexes.CreateOne(
             new CreateIndexModel<WordBookRecord>(
                Builders<WordBookRecord>.IndexKeys.Ascending(w => w.De),
                new CreateIndexOptions { Name = "De" }
            )
        );
        Client.GetDatabase("main").GetCollection<WordBookRecord>("WordBooks").Indexes.CreateOne(
          new CreateIndexModel<WordBookRecord>(
              Builders<WordBookRecord>.IndexKeys.Text(w => w.En).Text(w => w.De),
              new CreateIndexOptions { Name = "TextIndex", DefaultLanguage = "de" }
          )
      );
    }

    public IEnumerable<WordBookRecord> WordBooksSearch(string search, int skip = 0, int limit = 20)
    {
        var collection = this.Client.GetDatabase("main").GetCollection<WordBookRecord>("WordBooks");
        var filter = BsonDocument.Parse($@"{{
        $or: [
             {{ En: {{ $regex: ""{search}"", $options: ""i"" }} }},
             {{ De: {{ $regex: ""{search}"", $options: ""i"" }} }},
             {{ $text: {{ $search: ""{search}"", $caseSensitive: false, $diacriticSensitive: false }} }}
        ]
        }}");
        var query = collection.Find(filter);
        if (string.IsNullOrEmpty(search))
        {
            query = query.Sort("{ CreatedAt: -1 }");
        }
        var result = query.Skip(skip).Limit(limit).ToList();

        return result;
    }
}
