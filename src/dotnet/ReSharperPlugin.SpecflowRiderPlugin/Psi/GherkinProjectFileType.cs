using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace ReSharperPlugin.SpecflowRiderPlugin.Psi
{
    [ProjectFileTypeDefinition(Name)]
    public class GherkinProjectFileType : KnownProjectFileType
    {
        public new const string Name = "GHERKIN";
        public const string FEATURE_EXTENSION = ".features";
        
        [CanBeNull]
        [UsedImplicitly]
        public new static GherkinProjectFileType Instance { get; private set; }
        
        private GherkinProjectFileType() : base(Name, "GherkinDotnet", new[] {FEATURE_EXTENSION})  {  }
        
        protected GherkinProjectFileType([NotNull] string name) : base(name)
        {
        }

        protected GherkinProjectFileType([NotNull] string name, [NotNull] string presentableName) : base(name, presentableName)
        {
        }

        protected GherkinProjectFileType([NotNull] string name, [NotNull] string presentableName, [NotNull] IEnumerable<string> extensions) : base(name, presentableName, extensions)
        {
        }
    }
}