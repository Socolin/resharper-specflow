using System;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using ReSharperPlugin.SpecflowRiderPlugin.Daemon.UnresolvedReferenceHighlight;
using ReSharperPlugin.SpecflowRiderPlugin.Psi;
using ReSharperPlugin.SpecflowRiderPlugin.Utils.Steps;

namespace ReSharperPlugin.SpecflowRiderPlugin.Daemon.ParameterHighlighting
{
    public class ParameterHighlightingDaemonStageProcess : IDaemonStageProcess
    {
        private readonly GherkinFile _file;
        private readonly ParameterHighlightingProcessor _elementProcessor;
        public IDaemonProcess DaemonProcess { get; }

        public ParameterHighlightingDaemonStageProcess(IDaemonProcess daemonProcess, GherkinFile file, ResolveHighlighterRegistrar resolveHighlighterRegistrar,
                                                       IStepDefinitionBuilder stepDefinitionBuilder)
        {
            DaemonProcess = daemonProcess;
            _file = file;
            _elementProcessor = new ParameterHighlightingProcessor(DaemonProcess, resolveHighlighterRegistrar);
        }

        public void Execute(Action<DaemonStageResult> committer)
        {
            var psiSourceFile = _file.GetSourceFile();
            if (psiSourceFile == null)
                return;
            var consumer = new FilteringHighlightingConsumer(psiSourceFile, _file, DaemonProcess.ContextBoundSettingsStore);
            _file.ProcessDescendants(_elementProcessor, consumer);
            committer(new DaemonStageResult(consumer.Highlightings));
        }
    }
}