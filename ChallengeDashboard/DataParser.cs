using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FLRC.ChallengeDashboard
{
    public static class DataParser
    {
        private static readonly TimeSpan MinimumDuration = TimeSpan.FromMinutes(4);

        public static IEnumerable<Result> ParseCourse(Course course, JsonElement json)
        {
            var results = json.GetProperty("Racers");

            return results.GetArrayLength() > 0
                ? ParseResults(course, results)
                : new List<Result>();
        }

        public static double ParseDistance(string value)
        {
            var split = Regex.Match(value, @"([\d\.]+)(.*)").Groups;
            if (split.Count < 2)
                return 0;

            var digits = double.Parse(split[1].Value.Trim());
            var units = split[2].Value.Trim();

            switch (units.ToLowerInvariant())
            {
                case "k":
                case "km":
                case "kms":
                    return digits * 1000;
                case "mi":
                case "mile":
                case "miles":
                    return digits * Course.MetersPerMile;
            }

            return digits;
        }

        private static IEnumerable<Result> ParseResults(Course course, JsonElement results)
            => results.EnumerateArray()
            .Where(r => r.GetProperty("Finished").GetByte() == 1)
            .Select(r => new Result
            {
                Course = course,
                Athlete = ParseAthlete(r),
                StartTime = ParseStart(r.GetProperty("StartTime").GetString()),
                Duration = ParseDuration(r.GetProperty("RaceTime").GetDouble())
            })
            .Where(r => r.Duration.Value >= MinimumDuration);

        public static Time ParseDuration(double seconds)
            => new(TimeSpan.FromSeconds(Math.Ceiling(Math.Round(seconds, 1, MidpointRounding.ToZero))));

        private static readonly IDictionary<uint, Athlete> athletes = new ConcurrentDictionary<uint, Athlete>();

        public static Athlete ParseAthlete(JsonElement result)
        {
            var id = result.GetProperty("UserId").GetUInt32();
            if (id == 0)
                id = result.GetProperty("RacerId").GetUInt32();

            if (!athletes.ContainsKey(id))
                athletes.Add(id, new Athlete
                {
                    ID = id,
                    Name = result.GetProperty("Name").GetString(),
                    Age = result.GetProperty("Age").GetByte(),
                    Category = ParseCategory(result.GetProperty("Gender").GetString()),
                    DateOfBirth = DateTime.Parse(result.GetProperty("Info1").GetString() ?? string.Empty)
                });

            return athletes[id];
        }

        public static Category ParseCategory(string value)
            => Enum.TryParse<AgeGradeCalculator.Category>(value, true, out var category)
                ? new Category(category)
                : null;

        private static Date ParseStart(string value) => new Date(DateTime.Parse(value ?? string.Empty));
    }
}