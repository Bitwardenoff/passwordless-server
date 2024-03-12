using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Passwordless.Service.Storage.Ef.ValueComparers;

public sealed class ArrayValueComparer<T>() : ValueComparer<T[]>(
    (c1, c2) => c1!.SequenceEqual(c2!),
    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
    c => c.ToArray());