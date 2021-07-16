# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## 1.9.1
### Added
- Support for Rider 2021.2 EAP build 212.4416.21

## 1.9.0
### Added
- Support for Rider 2021.2 EAP build 212.4037.61

## 1.8.0

### Added
- Further enhance the gherkin code inspections [#98](https://github.com/SpecFlowOSS/SpecFlow.Rider/issues/98)

### Fixed
- Fix smart indentation when adding newline after a table [#87](https://github.com/SpecFlowOSS/SpecFlow.Rider/issues/87)
- Сode completion shows irrelevant results in feature files [#86](https://github.com/SpecFlowOSS/SpecFlow.Rider/issues/86)
- Don't show the test gutter marks when the scenario does not have a title [#136](https://github.com/SpecFlowOSS/SpecFlow.Rider/pull/138)
- Fix exceptions when no or an invalid language is provided [#138](https://github.com/SpecFlowOSS/SpecFlow.Rider/pull/138)
- Fix NullReferenceException when the createNewFileTarget cannot create the file [#135](https://github.com/SpecFlowOSS/SpecFlow.Rider/pull/135)
- Make the create step quick fix bulb yellow [#90](https://github.com/SpecFlowOSS/SpecFlow.Rider/pull/90)
- Fix exception when adding tag after the last row in the feature file [#126](https://github.com/SpecFlowOSS/SpecFlow.Rider/pull/126)
- Fix folding when adding tags before a Background [#125](https://github.com/SpecFlowOSS/SpecFlow.Rider/pull/125)

## 1.7.0

### Added
- After running tests, if a step failed it will be marked with an icon in the gutter. For this to works, you may need to enable `traceTimings`
- Allow to create a new binding class from the Create Step quickfix
- Format table while typing `|` character
- Syntax analysis, highlight some syntax errors
- Add run tests action in the feature file context menu [#122](https://github.com/SpecFlowOSS/SpecFlow.Rider/pull/122)

### Fixed
- Fix smart indentation when adding newline after a table [#87](https://github.com/SpecFlowOSS/SpecFlow.Rider/issues/87)
- Fix crash in unresolved step daemon [#100](https://github.com/SpecFlowOSS/SpecFlow.Rider/issues/100)
- `.feature.cs` is not nested after creation [#99](https://github.com/SpecFlowOSS/SpecFlow.Rider/issues/99)
- Arguments are not noticeable [#118](https://github.com/SpecFlowOSS/SpecFlow.Rider/pull/118)
- Сode completion shows irrelevant results in feature files [#86](https://github.com/SpecFlowOSS/SpecFlow.Rider/issues/86)
- Fix the "phantom" errors in Xunit projects [#115](https://github.com/SpecFlowOSS/SpecFlow.Rider/pull/115)
- Fix the test gutter mark for the old style csprojects [#116](https://github.com/SpecFlowOSS/SpecFlow.Rider/pull/116)
- Make SpecFlow project template consistent with other templates [#121](https://github.com/SpecFlowOSS/SpecFlow.Rider/pull/121)

## 1.6.0

### Added
- Add project template for SpecFlow
- Add item templates for SpecFlow artifacts (feature files, step definition files, etc.)

## 1.5.1

### Fixed
- Fix "Extend Selection" in 2021.1

## 1.5.0

### Added
- Support for Rider 2021.1

### Fixed
- Create step quickfix should generate same code as SpecFlow or the Visual Studio extension.
- Show onboarding information after opening a SpecFlow solution [#66](https://github.com/SpecFlowOSS/SpecFlow.Rider/issues/66)


## 1.4.0
### Added 
- Add autocompletion for Gherkin keywords [#46](https://github.com/SpecFlowOSS/SpecFlow.Rider/issues/49)
- Add support for Extend Selection in feature files [#56](https://github.com/SpecFlowOSS/SpecFlow.Rider/issues/56)
- Ability to fold Feature/Scenario/Background [#15](https://github.com/SpecFlowOSS/SpecFlow.Rider/issues/15)
- Add support for placing breakpoints in feature files [#2](https://github.com/SpecFlowOSS/SpecFlow.Rider/issues/2)

### Fixed
- Should be able to Configure Inspection of "Undefined step" [#62](https://github.com/SpecFlowOSS/SpecFlow.Rider/issues/62)
- Add separate indentation rule for 'And' steps [#52](https://github.com/SpecFlowOSS/SpecFlow.Rider/issues/52)

## 1.3.7
### Added
- "Comment Line" action now works [#20](https://github.com/SpecFlowOSS/SpecFlow.Rider/issues/20)
- Add syntax highlighting for step arguments [#30](https://github.com/SpecFlowOSS/SpecFlow.Rider/issues/30)
- Support for formatting in feature files [#10](https://github.com/SpecFlowOSS/SpecFlow.Rider/issues/10) and [#31](https://github.com/SpecFlowOSS/SpecFlow.Rider/issues/31)
- Add autocompletion for steps [#36](https://github.com/SpecFlowOSS/SpecFlow.Rider/pull/36)
### Fixed
- Fix step navigation for non english feature files [#27](https://github.com/SpecFlowOSS/SpecFlow.Rider/issues/27)
- Remove unused actions in 'Go to' menu [#5](https://github.com/SpecFlowOSS/SpecFlow.Rider/issues/5)

## 1.3.6
### Fixed
- Fix: Create steps when pattern contains a $ with a number like `Given someone pays $50`
- Support `But` [#26](https://github.com/SpecFlowOSS/SpecFlow.Rider/issues/26) and `*`[#25](https://github.com/SpecFlowOSS/SpecFlow.Rider/issues/25) step keyword
- When resolving reference from a step to its definition, and the step use example parameters, replace the text with the first example [#24](https://github.com/SpecFlowOSS/SpecFlow.Rider/issues/24)

## 1.3.5
### Fixed
- Fix: Check reference for steps inside `Rule:`
- Fix: Escape step before using them as pattern when creating a step with the quick fix.
- Fix: Keep special character in pattern when creating a step with the quick fix

## 1.3.4
### Fixed
- Correctly detect steps from assemblies (#14)
- Improve how tests are detected for NUnit / XUnit to display run test icon in the gutter

## 1.3.3
### Fixed
- Fix reference / find usage for step inside `Scenario Outline`
- Fix _Create Step_ for step inside `Scenario Outline`

## 1.3.2
### Fixed
- Detect step definition using `[StepDefinition]`
- Detect step definitions from NuGet / Assemblies

## 1.3.1
### Fixed
- "Run test" button was not displayed in gutter for XUnit test
- "Create step" quick fix now handle partial class correctly and file with multiple classes.

## 1.3.0
### Added
- Add buttons to run tests in gutter of specflow files

## 1.2.3
### Fixed
- Step definitions were not found when `using` were inside namsepace
- Step definitions were not found when they were inside a `partial class`

## 1.2.2
### Fixed
- Cache was not built in some scenario the first time the project is open with the plugin.
- The cache process was crashing when using specflow with other locales
- Fix a bug when specialized locales was used and not found in locales list

## 1.2.0
### Added
- Add highlighting: Warning when method name does not match with the pattern in the attribute.
- Add quickfix to rename method when name does not match the pattern. 

## 1.1.1
### Fixed
- Errors were disappearing from _Error in solution_ window

## 1.1.0
### Added
- Find usage on a step definition now list all usage of a step and let you navigate to it.
### Fixed
- Fix bug: The quick fix to create step was proposing to create the new step in some files not accessible by the current project

## 1.0.0
### Added
- Initial version
- Syntax highlight
- Go to declaration
- Quick-fix create missing steps
