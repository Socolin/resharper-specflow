using System.Collections.Generic;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Calculated.Interface;
using JetBrains.Application.Threading;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using ReSharperPlugin.SpecflowRiderPlugin.Psi;

namespace ReSharperPlugin.SpecflowRiderPlugin.Formatting
{
    [Language(typeof(GherkinLanguage))]
    public class GherkinFormatterInfoProvider : FormatterInfoProviderWithFluentApi<CodeFormattingContext, GherkinFormatSettingsKey>
    {
        public GherkinFormatterInfoProvider(ISettingsSchema settingsSchema, ICalculatedSettingsSchema calculatedSettingsSchema, IThreading threading, Lifetime lifetime)
            : base(settingsSchema, calculatedSettingsSchema, threading, lifetime)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            Indenting();
            Aligning();
            Formatting();
        }

        public override ProjectFileType MainProjectFileType => GherkinProjectFileType.Instance;

        private void Indenting()
        {
            var indentBetweenNodeAndChild = new List<(string name, GherkinNodeType parent, GherkinNodeType child)>
            {
                ("FeatureScenario", GherkinNodeTypes.FEATURE, GherkinNodeTypes.SCENARIO),
                ("FeatureScenarioOutline", GherkinNodeTypes.FEATURE, GherkinNodeTypes.SCENARIO_OUTLINE),
                ("FeatureRule", GherkinNodeTypes.FEATURE, GherkinNodeTypes.RULE),
                ("ScenarioStep", GherkinNodeTypes.SCENARIO, GherkinNodeTypes.STEP),
                ("ScenarioOutlineStep", GherkinNodeTypes.SCENARIO_OUTLINE, GherkinNodeTypes.STEP),
                ("RuleScenario", GherkinNodeTypes.RULE, GherkinNodeTypes.SCENARIO),
                ("ScenarioOutlineExampleBlob", GherkinNodeTypes.SCENARIO_OUTLINE, GherkinNodeTypes.EXAMPLES_BLOCK),
            };

            foreach (var rule in indentBetweenNodeAndChild)
            {
                Describe<IndentingRule>()
                    .Name(rule.name + "Indent")
                    .Where(
                        Parent().HasType(rule.parent),
                        Node().HasType(rule.child))
                    .Return(IndentType.External)
                    .Build();
            }
        }

        private void Aligning()
        {
        }

        private void Formatting()
        {
        }
    }
}