using System.Linq.Dynamic.Core;

namespace Drinks.API.Helpers;

public static class IQueryableExtensions
{
    public static IQueryable<T> ApplySort<T>(
        this IQueryable<T> source,
        string orderBy,
        Dictionary<string, string> mapping)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            return source;

        var orderByClauses = orderBy.Split(',');

        var orderQuery = new List<string>();

        foreach (var clause in orderByClauses)
        {
            var trimmed = clause.Trim();
            var descending = trimmed.EndsWith(" desc", StringComparison.OrdinalIgnoreCase);

            var propertyName = trimmed
                .Replace(" desc", "", StringComparison.OrdinalIgnoreCase)
                .Replace(" asc", "", StringComparison.OrdinalIgnoreCase);

            if (!mapping.ContainsKey(propertyName))
                throw new ArgumentException($"Invalid orderBy field: {propertyName}");

            var entityProperty = mapping[propertyName];

            orderQuery.Add(descending
                ? $"{entityProperty} descending"
                : entityProperty);
        }

        return source.OrderBy(string.Join(", ", orderQuery));
    }
}