using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

#pragma warning disable CS1662

namespace TodoApi.Data;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options) : base(options)
    {
    }

    // Users
    public DbSet<User> Users { get; set; } = null!;

    // Teams
    public DbSet<Team> Teams { get; set; } = null!;
    public DbSet<TeamMember> TeamMembers { get; set; } = null!;

    // Projects
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<ProjectMember> ProjectMembers { get; set; } = null!;

    // Tasks
    public DbSet<TaskItem> Tasks { get; set; } = null!;
    public DbSet<TaskComment> TaskComments { get; set; } = null!;
    public DbSet<TaskAttachment> TaskAttachments { get; set; } = null!;
    public DbSet<TaskHistory> TaskHistory { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Roles).HasColumnType("text[]");
        });

        // Team entity
        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.OwnerId);
            entity.HasIndex(e => e.Name);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasOne(e => e.Owner)
                .WithMany()
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // TeamMember entity
        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TeamId, e.UserId }).IsUnique();
            entity.HasOne(e => e.Team)
                .WithMany(t => t.Members)
                .HasForeignKey(e => e.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User)
                .WithMany(u => u.TeamMemberships)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Project entity
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.OwnerId);
            entity.HasIndex(e => e.TeamId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.HasOne(e => e.Owner)
                .WithMany(u => u.OwnedProjects)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Team)
                .WithMany(t => t.Projects)
                .HasForeignKey(e => e.TeamId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ProjectMember entity
        modelBuilder.Entity<ProjectMember>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ProjectId, e.UserId }).IsUnique();
            entity.HasOne(e => e.Project)
                .WithMany(p => p.Members)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Task entity
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProjectId);
            entity.HasIndex(e => e.AssigneeId);
            entity.HasIndex(e => e.CreatedById);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Priority);
            entity.HasIndex(e => e.DueDate);
            entity.HasIndex(e => e.CreatedAt);
            entity.Property(e => e.Title).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(5000);
            entity.Property(e => e.Tags).HasMaxLength(500);
            entity.HasOne(e => e.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Assignee)
                .WithMany()
                .HasForeignKey(e => e.AssigneeId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.CreatedBy)
                .WithMany()
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.ParentTask)
                .WithMany(p => p.Subtasks)
                .HasForeignKey(e => e.ParentTaskId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // TaskComment entity
        modelBuilder.Entity<TaskComment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TaskId);
            entity.HasIndex(e => e.AuthorId);
            entity.Property(e => e.Content).HasMaxLength(5000).IsRequired();
            entity.HasOne(e => e.Task)
                .WithMany(t => t.Comments)
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(e => e.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(e => e.ParentCommentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // TaskAttachment entity
        modelBuilder.Entity<TaskAttachment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TaskId);
            entity.HasIndex(e => e.UploadedById);
            entity.Property(e => e.FileName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.FileUrl).HasMaxLength(500).IsRequired();
            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.HasOne(e => e.Task)
                .WithMany(t => t.Attachments)
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.UploadedBy)
                .WithMany()
                .HasForeignKey(e => e.UploadedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // TaskHistory entity
        modelBuilder.Entity<TaskHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TaskId);
            entity.HasIndex(e => e.ChangedAt);
            entity.Property(e => e.FieldName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.OldValue).HasMaxLength(500);
            entity.Property(e => e.NewValue).HasMaxLength(500);
            entity.HasOne(e => e.Task)
                .WithMany(t => t.History)
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.ChangedBy)
                .WithMany()
                .HasForeignKey(e => e.ChangedById)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
