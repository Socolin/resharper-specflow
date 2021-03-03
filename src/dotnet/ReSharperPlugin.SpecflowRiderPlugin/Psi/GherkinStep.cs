using System.Collections.Generic;
using System.Text;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharperPlugin.SpecflowRiderPlugin.Caching.SpecflowJsonSettings;
using ReSharperPlugin.SpecflowRiderPlugin.Daemon.ParameterHighlighting;
using ReSharperPlugin.SpecflowRiderPlugin.References;

namespace ReSharperPlugin.SpecflowRiderPlugin.Psi
{
    public class GherkinStep : GherkinElement
    {
        private SpecflowStepDeclarationReference _reference;

        public GherkinStep() : base(GherkinNodeTypes.STEP)
        {
        }

        protected override void PreInit()
        {
            base.PreInit();
            _reference = new SpecflowStepDeclarationReference(this);
        }

        public GherkinStepKind GetStepKind()
        {
            var stepKeyword = this.FindChild<GherkinToken>(o => o.NodeType == GherkinTokenTypes.STEP_KEYWORD);
            if (stepKeyword == null) return GherkinStepKind.Unknown;

            var psiServices = GetPsiServices();
            var settings = psiServices.GetComponent<SpecflowSettingsProvider>();
            var gherkinKeywordProvider = psiServices.GetComponent<ILanguageManager>().GetService<GherkinKeywordProvider>(Language);
            var project = this.GetProject();
            var specflowSettingsLanguage = settings.GetSettings(project).Language.Feature;

            var gherkinStepKind = gherkinKeywordProvider.GetStepKind(specflowSettingsLanguage, stepKeyword.GetText());
            if (gherkinStepKind == GherkinStepKind.And)
            {
                gherkinStepKind = GetPreviousGherkinStepKind();
                if (gherkinStepKind == GherkinStepKind.Unknown)
                    return GherkinStepKind.Given;
            }

            return gherkinStepKind;
        }

        private GherkinStepKind GetPreviousGherkinStepKind()
        {
            var node = PrevSibling;
            while (node != null)
            {
                if (node is GherkinStep gherkinStep)
                    return gherkinStep.GetStepKind();
                node = node.PrevSibling;
            }

            return GherkinStepKind.Unknown;
        }

        public string GetStepText(bool withStepKeyWord = false)
        {
            var sb = new StringBuilder();
            for (var te = (TreeElement) FirstChild; te != null; te = te.nextSibling)
            {
                switch (te)
                {
                    case GherkinStepParameter p:
                        sb.Append(p.GetText());
                        break;
                    case GherkinToken token:
                    {
                        if (withStepKeyWord)
                        {
                            sb.Append(token.GetText());
                        }
                        else
                        {
                            if (token.NodeType != GherkinTokenTypes.STEP_KEYWORD)
                            {
                                sb.Append(token.GetText());
                            }
                        }

                        break;
                    }
                }
            }
            return sb.ToString().Trim();
        }

        public string GetStepTextForExample(IDictionary<string, string> exampleData)
        {
            var sb = new StringBuilder();
            var previousTokenWasAParameter = false;
            for (var te = (TreeElement) FirstChild; te != null; te = te.nextSibling)
            {
                switch (te)
                {
                    case GherkinStepParameter p:
                        previousTokenWasAParameter = true;
                        sb.Length--; // Remove `<`

                        if (exampleData.TryGetValue(p.GetParameterName(), out var value))
                            sb.Append(value);
                        else
                            sb.Append(p.GetText());

                        break;
                    case GherkinToken token when token.NodeType != GherkinTokenTypes.STEP_KEYWORD:
                        sb.Append(token.GetText());

                        // Remove `>`
                        if (previousTokenWasAParameter)
                            sb.Length--;
                        previousTokenWasAParameter = false;
                        break;
                }
            }
            return sb.ToString().Trim();
        }

        public SpecflowStepDeclarationReference GetStepReference()
        {
            return _reference;
        }

        public override ReferenceCollection GetFirstClassReferences()
        {
            return new ReferenceCollection(_reference);
        }

        public void Accept(ParameterHighlightingProcessor parameterHighlighterProcess, IHighlightingConsumer consumer)
        {
            parameterHighlighterProcess.VisitNode(this, consumer);
        }
    }
}