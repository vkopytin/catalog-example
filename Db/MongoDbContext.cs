using Db.Records;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Db;

public class MongoDbContext : DbContext
{
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

  public MongoDbContext(MongoClient client)
   : base(new DbContextOptionsBuilder<MongoDbContext>().UseMongoDB(client, "main").Options)
  {

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

    modelBuilder.Entity<ArticleRecord>().HasOne(a => a.Media);
    modelBuilder.Entity<ArticleRecord>().HasMany(a => a.Blocks).WithOne(b => b.Article).HasForeignKey(b => b.ArticleId);
    modelBuilder.Entity<ArticleRecord>().HasMany(a => a.WebSiteArticles).WithOne(w => w.Article).HasForeignKey(w => w.ArticleId);
    modelBuilder.Entity<ArticleBlockRecord>().HasOne(b => b.Article);
    modelBuilder.Entity<WebSiteRecord>().HasOne(w => w.Parent).WithMany(w => w.SubSites).HasForeignKey(w => w.ParentId);
    modelBuilder.Entity<WebSiteArticleRecord>().HasOne(w => w.Article).WithMany(a => a.WebSiteArticles).HasForeignKey(w => w.ArticleId);
    modelBuilder.Entity<WebSiteArticleRecord>().HasOne(w => w.WebSite).WithMany(a => a.WebSiteArticles).HasForeignKey(w => w.WebSiteId);
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
  }
}
