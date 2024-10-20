using Autofac;
using SaveChangesEventHandlers.Example.Models;
using Microsoft.EntityFrameworkCore;
using SaveChangesEventHandlers.Core.Abstraction;

namespace SaveChangesEventHandlers.Example
{
    public class DataContext : DbContext
    {
        private readonly ISaveChangesEventsDispatcher saveChangesEventsDispatcher;

        public DataContext(DbContextOptions<DataContext> options, ISaveChangesEventsDispatcher saveChangesEventsDispatcher) : base(options)
        {
            this.saveChangesEventsDispatcher = saveChangesEventsDispatcher;
        }

        //neded so ef core can create migrations
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ContactTag> ContactTags { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<Number> Numbers { get; set; }

        public override int SaveChanges()
        {
            return saveChangesEventsDispatcher.SaveChangesWithEventsDispatcher(this, base.SaveChanges);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //ContactTags
            modelBuilder.Entity<ContactTag>()
                .HasOne(c => c.Contact)
                .WithMany(ct => ct.ContactTags)
                .HasForeignKey(c => c.ContactId);

            modelBuilder.Entity<ContactTag>()
                .HasOne(t => t.Tag)
                .WithMany(ct => ct.ContactTags)
                .HasForeignKey(t => t.TagId);

            modelBuilder.Entity<ContactTag>()
            .Navigation(t => t.Tag)
            .AutoInclude();

            //Contacts
            modelBuilder.Entity<Contact>()
                .HasMany(c => c.Emails)
                .WithOne(e => e.Contact)
                .HasForeignKey(c => c.ContactId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Contact>()
                .HasMany(c => c.Numbers)
                .WithOne(e => e.Contact)
                .HasForeignKey(c => c.ContactId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Contact>()
                .Navigation(e => e.ContactTags)
                .AutoInclude();

            modelBuilder.Entity<Contact>()
                .Navigation(e => e.Emails)
                .AutoInclude();

            modelBuilder.Entity<Contact>()
                .Navigation(e => e.Numbers)
                .AutoInclude();

            //Tags
            modelBuilder.Entity<Tag>()
                .HasMany(t => t.ContactTags)
                .WithOne(ct => ct.Tag)
                .HasForeignKey(t => t.TagId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
