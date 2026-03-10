namespace SIG_DefesaCivil.API.Helper
{
    public static class EnumMapperHelper
    {
        public static List<TEnum> ToEnumList<TEnum>(this IEnumerable<string>? source) where TEnum : struct, Enum
        {
            if (source == null || !source.Any()) return new List<TEnum>();

            return source
                .Where(s => Enum.TryParse<TEnum>(s, true, out _))
                .Select(s => Enum.Parse<TEnum>(s, true))
                .ToList();
        }

        public static List<string> ToStringList<TEnum>(this IEnumerable<TEnum>? source) where TEnum : struct, Enum
        {
            if (source == null || !source.Any()) return new List<string>();
            return source.Select(e => e.ToString()).ToList();
        }
    }
}
