using EventPlanner.Domain.Entities;
using EventPlanner.Domain.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventPlanner.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id).ValueGeneratedNever();

        builder
            .Property(user => user.FirstName)
            .HasMaxLength(User.MaxFirstNameLength)
            .IsRequired();

        builder
            .Property(user => user.LastName)
            .HasMaxLength(User.MaxLastNameLength)
            .IsRequired();

        builder
            .Property(user => user.Email)
            .HasMaxLength(User.MaxEmailLength)
            .IsRequired();

        builder
            .Property(user => user.Role)
            .HasConversion(role => role.ToString(), value => Enum.Parse<UserRole>(value))
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(user => user.Phone).HasMaxLength(User.MaxPhoneLength);

        builder.Property(user => user.PasswordHash).HasMaxLength(512).IsRequired();

        builder.Property(user => user.IsActive).HasDefaultValue(true);

        builder.Property(user => user.CreatedAt).IsRequired();

        builder.Property(user => user.UpdatedAt).IsRequired();

        builder.HasIndex(user => user.Email).IsUnique();
    }
}
