﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Text.Json.JsonElement;

namespace ChallengeDashboard
{
    public class API
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseURL;

        public API(HttpClient httpClient, string baseURL)
        {
            _httpClient = httpClient;
            _baseURL = baseURL;
        }

        public async Task<Course> GetCourse(uint id)
        {
            var url = $"{_baseURL}/results?raceid={id}";
            var response = await _httpClient.GetStreamAsync(url);
            var json = await JsonDocument.ParseAsync(response);
            var root = json.RootElement;
            var distances = root.GetProperty("Distances");
            var results = root.GetProperty("Racers");
            return new Course
            {
                ID = root.GetProperty("RaceId").GetUInt32(),
                Name = root.GetProperty("Name").GetString(),
                Type = root.GetProperty("SportType").GetString(),
                Distance = distances.GetArrayLength() > 0
                    ? string.Join(", ", distances.EnumerateArray().Select(r => r.GetProperty("Name").GetString()))
                    : null,
                Results = results.GetArrayLength() > 0
                    ? ParseResults(results.EnumerateArray())
                    : null
            };
        }

        private IEnumerable<Result> ParseResults(ArrayEnumerator results)
        {
            return results.Select(r => new Result
            {
                Athlete = ParseAthlete(r),
                StartTime = ParseStart(r.GetProperty("StartTime").GetString()),
                Duration = r.GetProperty("RaceTime").GetDecimal()
            });
        }

        private Athlete ParseAthlete(JsonElement result)
        {
            return new Athlete
            {
                ID = result.GetProperty("RacerId").GetUInt32(),
                Name = result.GetProperty("Name").GetString(),
                Age = result.GetProperty("Age").GetByte()
            };
        }

        private DateTime? ParseStart(string value)
        {
            return value != null ? DateTime.Parse(value) : null;
        }
    }
}