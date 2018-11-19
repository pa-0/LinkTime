#tool "nuget:?package=NUnit.ConsoleRunner"
#tool "nuget:?package=OpenCover"
#tool "nuget:?package=ReportGenerator"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var buildDirectory = Directory("./Build") + Directory(configuration);
var solutionFile = "LinkTime.sln";
var testReportFile = "LinkTime.TestReport.xml";
var coverageReportFile = "LinkTime.Coverage.xml";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(MakeAbsolute(buildDirectory));
});

Task("Restore-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(solutionFile);
});

Task("Build")
    .IsDependentOn("Restore-Packages")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
        MSBuild(solutionFile, settings => settings.SetConfiguration(configuration).WithProperty("OutDir", MakeAbsolute(buildDirectory).FullPath));
    }
    else
    {
        XBuild(solutionFile, settings => settings.SetConfiguration(configuration).WithProperty("OutDir", MakeAbsolute(buildDirectory).FullPath));
    }
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
        OpenCover(tool => {
            tool.NUnit3(GetFiles(MakeAbsolute(buildDirectory).FullPath + "/*.Test.dll"), new NUnit3Settings {
                OutputFile = MakeAbsolute(buildDirectory).CombineWithFilePath(testReportFile),
                Process = NUnit3ProcessOption.InProcess,
                WorkingDirectory = MakeAbsolute(buildDirectory)
            });
            },
            MakeAbsolute(buildDirectory).CombineWithFilePath(coverageReportFile),
            new OpenCoverSettings()
                .WithFilter("+[*]*")
                .WithFilter("-[LinkTime.Test]*"));

        ReportGenerator(MakeAbsolute(buildDirectory).CombineWithFilePath(coverageReportFile), MakeAbsolute(buildDirectory).Combine("Coverage"));
    }
    else
    {
        NUnit3(GetFiles(MakeAbsolute(buildDirectory).FullPath + "/*.Test.dll"), new NUnit3Settings {
            OutputFile = MakeAbsolute(buildDirectory).CombineWithFilePath(testReportFile),
            Process = NUnit3ProcessOption.InProcess,
            WorkingDirectory = MakeAbsolute(buildDirectory)
        });
    }
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Test");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);