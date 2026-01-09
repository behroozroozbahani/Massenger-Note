using PortalCore.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PortalCore.Persistence.Configurations.Identity
{
    public class JwtUserTokenConfiguration : IEntityTypeConfiguration<JwtUserToken>
    {
        public void Configure(EntityTypeBuilder<JwtUserToken> builder)
        {
            builder.HasOne(jwtUserToken => jwtUserToken.User)
                .WithMany(user => user.JwtUserTokens)
                .HasForeignKey(jwtUserToken => jwtUserToken.UserId);

            builder.Property(ut => ut.RefreshTokenIdHash).HasMaxLength(450).IsRequired();
            builder.Property(ut => ut.RefreshTokenIdHashSource).HasMaxLength(450);

            builder.ToTable("AppJwtUserTokens");
        }
    }
}
