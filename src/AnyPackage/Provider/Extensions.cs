// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Collections;

namespace AnyPackage.Provider;

internal static class Extensions
{
    internal static Dictionary<string, object?> ToDictionary(this Hashtable hashtable)
    {
        var dictionary = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        foreach (DictionaryEntry entry in hashtable)
        {
            var key = entry.Key.ToString();

            if (key is not null)
            {
                dictionary.Add(key, entry.Value);
            }
        }

        return dictionary;
    }
}
