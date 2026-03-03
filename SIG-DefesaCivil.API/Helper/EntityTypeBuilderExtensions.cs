using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;

namespace SIG_DefesaCivil.API.Helper
{
    public static class EntityTypeBuilderExtensions
    {
        /// <summary>
        /// Método auxiliar genérico para configurar listas de Enums (List<T>) como strings separadas por vírgula no banco.
        /// </summary>
        public static void ConfigureEnumList<TEntity, TEnum>(
            this EntityTypeBuilder<TEntity> builder,
            Expression<Func<TEntity, List<TEnum>>> propertyExpression)
            where TEntity : class
            where TEnum : struct, Enum
        {
            var comparer = new ValueComparer<List<TEnum>>(
                (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());

            builder.Property(propertyExpression)
                .HasConversion(
                    v => string.Join(',', v.Select(e => e.ToString())),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(s => Enum.Parse<TEnum>(s)).ToList())
                .Metadata.SetValueComparer(comparer);
        }
    }
}
