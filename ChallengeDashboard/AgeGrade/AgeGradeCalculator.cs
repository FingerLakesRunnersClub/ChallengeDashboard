﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FLRC.ChallengeDashboard.AgeGrade
{
    public class AgeGradeCalculator
    {
        private readonly IDictionary<Identifier, uint> _ageGradeLookup;

        public static readonly IList<double> StandardDistances = new[] { 1609.344, 5000, 6000, 6437.376, 8000, 8046.7000, 10000, 12000, 15000, 16093.44, 20000, 21097.5, 25000, 30000, 42195, 50000, 80467.2, 100000, 150000, 160934.4, 200000 };

        public AgeGradeCalculator(IDictionary<Identifier, uint> ageGradeLookup) => _ageGradeLookup = ageGradeLookup;

        public double GetAgeGrade(Athlete athlete, Course course, TimeSpan time)
        {
            if (athlete.Category == null || course.Distance == 0)
                return 0;

            var identifier = new Identifier(athlete.Category, athlete.Age, course.Distance);
            var best = StandardDistances.Contains(course.Distance)
                ? _ageGradeLookup[identifier]
                : Interpolate(identifier);

            return 100 * best / time.TotalSeconds;
        }

        private double Interpolate(Identifier identifier)
        {
            var distance = identifier.Distance;
            var prev = StandardDistances.Where(d => d <= distance).Max();
            var next = StandardDistances.Where(d => d >= distance).Min();
            var factor = (distance - prev) / (next - prev);

            var prevAgeGrade = _ageGradeLookup[new Identifier(identifier.Category, identifier.Age, prev)];
            var nextAgeGrade = _ageGradeLookup[new Identifier(identifier.Category, identifier.Age, next)];

            return prevAgeGrade * (1 - factor) + nextAgeGrade * factor;
        }
    }
}
