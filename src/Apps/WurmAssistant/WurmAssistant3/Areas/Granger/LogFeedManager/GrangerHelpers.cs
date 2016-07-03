﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AldursLab.Essentials.Extensions.DotNet;

namespace AldursLab.WurmAssistant3.Areas.Granger.LogFeedManager
{
    public static class GrangerHelpers
    {
        public static class Ages
        {
            public const string
                Young = "Young",
                Adolescent = "Adolescent",
                Mature = "Mature",
                Aged = "Aged",
                Old = "Old",
                Venerable = "Venerable";
        }

        public static class Tags
        {
            public const string
                Starving = "starving",
                Diseased = "diseased",
                Fat = "fat";
        }

        public static readonly TimeSpan BreedingNotInMoodDuration = TimeSpan.FromMinutes(45);

        static readonly IReadOnlyCollection<string> CreatureAges = new List<string>()
        {
            Ages.Young, Ages.Adolescent, Ages.Mature, Ages.Aged, Ages.Old, Ages.Venerable
        };

        public static readonly IReadOnlyCollection<string> CreatureAgesUpcase;

        public static readonly IReadOnlyCollection<string> OtherNamePrefixes = new List<string>()
        {
            Tags.Fat, Tags.Starving, Tags.Diseased
        };

        public static readonly IReadOnlyCollection<string> AllNamePrefixes;

        static GrangerHelpers()
        {
            CreatureAgesUpcase = CreatureAges.Select(x => x.ToUpperInvariant()).ToArray();

            var allnameprefixesBuilder = new List<string>();
            allnameprefixesBuilder.AddRange(CreatureAges);
            allnameprefixesBuilder.AddRange(OtherNamePrefixes);

            AllNamePrefixes = allnameprefixesBuilder.ToArray<string>();
        }

        public static TimeSpan LongestPregnancyPossible { get; } = TimeSpan.FromHours(273);

        public static string RemoveAllPrefixes(string creatureName)
        {
            foreach (string prefix in AllNamePrefixes)
            {
                creatureName = Regex.Replace(creatureName,
                    $@"^{prefix}(\W)|(\W){prefix}(\W)",
                    "$1$2",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            }
            creatureName = creatureName.Trim();
            return creatureName;
        }

        internal static string CapitalizeCreatureName(string lowercasename)
        {
            char firstletter = lowercasename[0];
            firstletter = Char.ToUpperInvariant(firstletter);
            var fixedName = firstletter + lowercasename.Substring(1, lowercasename.Length - 1);
            return fixedName;
        }

        public static CreatureAge ExtractCreatureAge(string prefixedObjectName)
        {
            return CreatureAge.CreateAgeFromRawCreatureNameStartsWith(prefixedObjectName);
        }

        public static bool HasAgeInName(string prefixedObjectName, bool ignoreCase = false)
        {
            foreach (string age in CreatureAges)
            {
                if (ignoreCase)
                {
                    if (prefixedObjectName.Contains(age, StringComparison.OrdinalIgnoreCase)) return true;
                }
                else
                {
                    if (prefixedObjectName.Contains(age)) return true;
                }
            }
            return false;
        }

        public static string ExtractCreatureName(string prefixedObjectName)
        {
            return RemoveAllPrefixes(prefixedObjectName);
        }

        public static string TryParseCreatureNameIfLineContainsDiseased(string inputLine)
        {
            return TryParseCreatureNameIfLineContains(inputLine, Tags.Diseased);
        }

        public static string TryParseCreatureNameIfLineContainsFat(string inputLine)
        {
            return TryParseCreatureNameIfLineContains(inputLine, Tags.Fat);
        }

        public static string TryParseCreatureNameIfLineContainsStarving(string inputLine)
        {
            return TryParseCreatureNameIfLineContains(inputLine, Tags.Starving);
        }

        static string TryParseCreatureNameIfLineContains(string line, string value)
        {
            Match match = Regex.Match(line, value + @" (\w+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            if (match.Success)
            {
                string possibleCreatureName = match.Groups[1].Value;
                return CapitalizeCreatureName(possibleCreatureName);
            }
            else return null;
        }

        public static CreatureTrait[] ParseTraitsFromLine(string line)
        {
            List<CreatureTrait> result = new List<CreatureTrait>();
            foreach (var trait in CreatureTrait.GetAllPossibleTraits())
            {
                if (line.Contains(trait.ToString()))
                {
                    result.Add(trait);
                }
            }
            return result.ToArray();
        }
    }
}
