using Feedback.APIs.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Feedback.APIs.Persistence.EfConfigurations;

public class SubjectEfConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ToTable(FeedbackDbContextSchema.SubjectTableName);
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Locked)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode(true);

        builder.Property(x => x.CreatedOn)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");


        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.ExpiredOn)
            .IsRequired(false);

        builder.OwnsMany(x => x.Reviews, reviewBuilder =>
        {
            reviewBuilder.ToTable(FeedbackDbContextSchema.ReviewTableName);
            reviewBuilder.HasKey(x => x.Id);

            reviewBuilder.Property(x => x.Comment)
            .IsRequired()
            .HasMaxLength(400)
            .IsUnicode(true);

            reviewBuilder.Property(x => x.ReviewerName)
            .IsRequired()
            .HasMaxLength(50);

            reviewBuilder.Property(x => x.SubjectId)
            .IsRequired();

            reviewBuilder.Property(x => x.Date)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        });
    }

}
