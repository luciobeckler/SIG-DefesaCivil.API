using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;

namespace SIG_DefesaCivil.API.Helper
{
    public static class EntityTypeBuilderExtensions
    {
        // 1. Método original: Para Entidades Raízes (EntityTypeBuilder)
        public static void ConfigureEnumList<TEntity, TEnum>(
            this EntityTypeBuilder<TEntity> builder,
            Expression<Func<TEntity, List<TEnum>>> propertyExpression)
            where TEntity : class
            where TEnum : struct, Enum
        {
            var comparer = CriarValueComparer<TEnum>();

            builder.Property(propertyExpression)
                .HasConversion(
                    v => string.Join(',', v.Select(e => e.ToString())),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(s => Enum.Parse<TEnum>(s)).ToList())
                .Metadata.SetValueComparer(comparer);
        }

        // 2. NOVO Método: Para Value Objects / Complex Types (OwnedNavigationBuilder)
        public static void ConfigureEnumList<TOwnerEntity, TOwnedEntity, TEnum>(
            this OwnedNavigationBuilder<TOwnerEntity, TOwnedEntity> builder,
            Expression<Func<TOwnedEntity, List<TEnum>>> propertyExpression)
            where TOwnerEntity : class
            where TOwnedEntity : class
            where TEnum : struct, Enum
        {
            var comparer = CriarValueComparer<TEnum>();

            builder.Property(propertyExpression)
                .HasConversion(
                    v => string.Join(',', v.Select(e => e.ToString())),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(s => Enum.Parse<TEnum>(s)).ToList())
                .Metadata.SetValueComparer(comparer);
        }

        // Método privado para não repetirmos a lógica do Comparer
        private static ValueComparer<List<TEnum>> CriarValueComparer<TEnum>() where TEnum : struct, Enum
        {
            return new ValueComparer<List<TEnum>>(
                (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());
        }
    }
}