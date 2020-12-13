using System;
using FLRC.AgeGradeCalculator;

namespace FLRC.ChallengeDashboard
{
    public class Athlete
    {
        public uint ID { get; set; }
        public string Name { get; set; }
        
        public Category? Category { get; set; }
        public string CategoryDisplay => Category?.ToString();

        public byte Age { get; set; }
        public DateTime DateOfBirth { get; set; }

        public byte AgeAsOf(DateTime date) => (byte) (date.Subtract(DateOfBirth).TotalDays / 365.2425);

        public byte Team => (byte)(Age / 10);
        public string TeamDisplay => $"{Team}0–{Team}9";
    }
}