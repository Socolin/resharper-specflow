using System.Collections.Generic;
using System.Text.RegularExpressions;
using JetBrains.Application.changes;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using ReSharperPlugin.SpecflowRiderPlugin.References;

namespace ReSharperPlugin.SpecflowRiderPlugin.Psi
{
    public static class GherkinPsiUtil
    {
        public static List<TextRange> BuildParameterRanges(GherkinStep step, SpecflowStepDeclarationReference reference, DocumentRange documentRange)
        {
            var stepText = step.GetStepText();
            var stepTextWithKeyword = step.GetStepText(true);
            var stepKeywordLengthWithWhiteSpace = stepTextWithKeyword.Length - stepText.Length;

            List<TextRange> parameterRanges = new List<TextRange>();
            reference.ResolveWithoutCache();
            
            var regex = reference.RegexPattern;
            if (regex == null) return parameterRanges;

            var match = regex.Match(stepText);

            if (match.Success)
            {
                foreach (Group matchGroup in match.Groups)
                {
                    if(matchGroup.Value == stepText)
                        continue;
                    
                    var start = stepKeywordLengthWithWhiteSpace + matchGroup.Index;

                    var parameterStart = documentRange.StartOffset.Offset+start;
                    var parameterEnd = parameterStart + matchGroup.Length;
                    TextRange range = new TextRange( parameterStart, parameterEnd);
                    
                    if (!parameterRanges.Contains(range))
                    {
                        parameterRanges.Add(range);
                    }
                }
            }

            return parameterRanges;
        }


    }
}