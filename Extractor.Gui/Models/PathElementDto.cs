using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Extractor.Core.Model;

namespace Extractor.Gui.Models;

public class PathElementDto
{
    public PathElement Value { get; set; }
}

public static class PathElementExtensions
{
    extension(PathElement element)
    {
        public PathElementDto ToDto()
        {
            return new PathElementDto { Value = element };
        }
    }
}

public class PathElementDtoComparer : IEqualityComparer<PathElementDto>
{
    public bool Equals(PathElementDto? x, PathElementDto? y)
    {
        if (ReferenceEquals(x, y)) return true;

        if (x is null || y is null) return false;

        return x.Value.Equals(y.Value);
    }

    public int GetHashCode([DisallowNull] PathElementDto obj)
    {
        return obj.Value.GetHashCode();
    }
}