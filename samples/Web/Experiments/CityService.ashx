<%@ WebHandler Language="C#" Class="CityService" %>

using System;
using System.Collections.Generic;
using System.Linq;
using SilverlightFX.Services;

public class CityInfo {

    public string City {
        get;
        set;
    }

    public string ZipCode {
        get;
        set;
    }

    public string State {
        get;
        set;
    }
}

public static class CityDB {

    private static readonly List<CityInfo> cities =
        new List<CityInfo> {
            new CityInfo { City = "Redmond", ZipCode = "98052", State = "WA", },
            new CityInfo { City = "Seattle", ZipCode = "98101", State = "WA", },
            new CityInfo { City = "Bellevue", ZipCode = "98005", State = "WA", },
            new CityInfo { City = "Sammamish", ZipCode = "98074", State = "WA", },
            new CityInfo { City = "Portland", ZipCode = "97200", State = "OR", },
            new CityInfo { City = "Roseville", ZipCode = "95678", State = "CA" },
            new CityInfo { City = "San Francisco", ZipCode = "94101", State = "CA" },
            new CityInfo { City = "San Diego", ZipCode = "92101", State = "CA" },
            new CityInfo { City = "San Jose", ZipCode = "95101", State = "CA" },
            new CityInfo { City = "San Mateo", ZipCode = "94401", State = "CA" },
            new CityInfo { City = "Los Angeles", ZipCode = "90002", State = "CA" },
            new CityInfo { City = "San Antonio", ZipCode = "78201", State = "TX" },
            new CityInfo { City = "Las Vegas", ZipCode = "89044", State = "NV" },
            new CityInfo { City = "New York", ZipCode = "10110", State = "NY" },
            new CityInfo { City = "Irvine", ZipCode = "92626", State = "CA" }
        };

    public static CityInfo[] GetCities(string prefix) {
        IEnumerable<CityInfo> matchingCities =
            from c in cities
            where c.City.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            select c;

        return matchingCities.ToArray();
    }
}

public class CityService : CompletionService<CityInfo> {

    protected override IEnumerable<CityInfo> GetCompletionItems(string prefix) {
        return CityDB.GetCities(prefix);
    }
}
