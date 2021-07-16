using System.Collections.Generic;
using JetBrains.Application;

namespace ReSharperPlugin.SpecflowRiderPlugin.Guidance
{
    [ShellComponent]
    public class GuidanceConfiguration : IGuidanceConfiguration
    {
        public GuidanceConfiguration()
        {
            UsageSequence = new[]
            {
                new GuidanceStep(GuidanceNotification.AfterInstall, 1, @"https://specflow.org/tools/specflow/onboarding-day-1/", "Ready to start using SpecFlow?", "Get your personalized learning material", "Start now"),
                new GuidanceStep(GuidanceNotification.TwoDayUsage, 2, "https://specflow.org/tools/specflow/onboarding-day-2/", "Curious how to write better Gherkin feature files?", "Take a look into our helpful list of tips and tricks", "Learn more"),
                new GuidanceStep(GuidanceNotification.FiveDayUsage, 5, "https://specflow.org/tools/specflow/onboarding-day-5/", "How was your first experience with SpecFlow?", "Help us to further improve your automation experience", "Leave feedback"),
                new GuidanceStep(GuidanceNotification.TenDayUsage, 10, "https://specflow.org/tools/specflow/onboarding-day-10/", "Ready to become a SpecFlow expert?", "Join one of our online courses or trainings", "Learn more"),
                new GuidanceStep(GuidanceNotification.TwentyDayUsage, 20, null, "","",""),
                new GuidanceStep(GuidanceNotification.HundredDayUsage, 100, "https://specflow.org/tools/specflow/onboarding-day-100/", "Congrats, you are now a SpecFlow expert!", "Help us to make it easier to get started as beginner", "Leave feedback"),
                new GuidanceStep(GuidanceNotification.TwoHundredDayUsage, 200, "https://specflow.org/tools/specflow/onboarding-day-200/", "Congrats, you are now a SpecFlow expert!", "Help us to make it easier to get started as beginner", "Leave feedback")
            };
        }

        public IEnumerable<GuidanceStep> UsageSequence { get; }
    }
}