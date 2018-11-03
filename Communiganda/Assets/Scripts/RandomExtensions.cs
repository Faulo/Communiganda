﻿using System;
using System.Linq;

public static class RandomExtensions {
    private static readonly Random random = new Random();

    public static T RandomElement<T>(this T[] enumerable)
    {
        if (enumerable == null || enumerable.Length == 0)
        {
            throw new ArgumentNullException("enumerable");
        }

        return enumerable.ElementAt(random.Next(enumerable.Length));
    }

    public static bool Contains<T>(this T[] haystack, T needle)
    {
        foreach (T item in haystack)
        {
            if (item.Equals(needle))
            {
                return true;
            }
        }
        return false;
    }
}
