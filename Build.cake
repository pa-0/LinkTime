#tool "nuget:?package=NUnit.ConsoleRunner"
#tool "nuget:?package=OpenCover"
#tool "nuget:?package=ReportGenerator"

#addin "nuget:?package=Cake.Git"

using System.IO;
using System.Text.RegularExpressions;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var buildDirectory = Directory("./Build") + Directory(configuration);
var projectName = "LinkTime";
var solutionFile = projectName + ".sln";
var solutionInfoFile = "./" + projectName + "/SolutionInfo.cs";
var testReportFile = projectName + ".TestReport.xml";
var coverageReportFile = projectName + ".Coverage.xml";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

var version = "0.0.0";

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

Task("Set-Version")
    .IsDependentOn("Restore-Packages")
    .Does(() =>
{
    try
    {
        string major = "0";
        string minor = "0";
        string buildNumber = "0";
        string revision = "0";
        string shasum = "X";

        if (configuration == "Release")
        {
            var gitDescription = GitDescribe("./", true, GitDescribeStrategy.Default);

            Regex query = new Regex(@"v(?<major>\d+).(?<minor>\d+)-(?<revision>\d+)-(?<shasum>.*)");
            MatchCollection matches = query.Matches(gitDescription);

            foreach (Match match in matches)
            {
                major = match.Groups["major"].Value;
                minor = match.Groups["minor"].Value;
                revision = match.Groups["revision"].Value;
                shasum = match.Groups["shasum"].Value;
            }

            buildNumber = GetPersistentBuildNumber(MakeAbsolute(new DirectoryPath("./")).FullPath).ToString();
        }

        version = string.Format("{0}.{1}.{2}", major, minor, revision);
        string versionString = string.Format("{0}.{1}.{2}.{3}", major, minor, buildNumber, revision);
        string longVersionString = string.Format("{0}.{1}.{2}.{3}-{4}", major, minor, buildNumber, revision, shasum);

        Information("Version: " + versionString + " (" + longVersionString + ")");

        CreateAssemblyInfo(solutionInfoFile, new AssemblyInfoSettings {
            Version = versionString,
            FileVersion = versionString,
            InformationalVersion = longVersionString,
        });
    }
    catch (Exception)
    {
    }
});

Task("Build")
    .IsDependentOn("Set-Version")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
        MSBuild(solutionFile, settings => settings.SetConfiguration(configuration).WithProperty("OutDir", MakeAbsolute(buildDirectory).FullPath));
    }
    else
    {
        XBuild(solutionFile, settings => settings.SetConfiguration(configuration).WithProperty("OutDir", MakeAbsolute(buildDirectory).FullPath + "/"));
    }
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    if(IsRunningOnWindows() && configuration == "Debug")
    {
        OpenCover(tool => {
            tool.NUnit3(GetFiles(MakeAbsolute(buildDirectory).FullPath + "/*.Test.dll"), new NUnit3Settings {
                OutputFile = MakeAbsolute(buildDirectory).CombineWithFilePath(testReportFile),
                Process = NUnit3ProcessOption.InProcess,
                WorkingDirectory = MakeAbsolute(buildDirectory)
            });
            },
            MakeAbsolute(buildDirectory).CombineWithFilePath(coverageReportFile),
            new OpenCoverSettings() {
                WorkingDirectory = MakeAbsolute(buildDirectory)
            }
                .WithFilter("+[*]*")
                .WithFilter("-[" + projectName + ".Test]*"));

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

Task("Create-Archive")
    .IsDependentOn("Test")
    .WithCriteria(configuration == "Release")
    .Does(() =>
{
    var files = new [] {
        buildDirectory.ToString() + "/LinkTime.exe",
        buildDirectory.ToString() + "/AUTHORS.txt",
        buildDirectory.ToString() + "/CHANGELOG.md",
        buildDirectory.ToString() + "/CONTRIBUTING.md",
        buildDirectory.ToString() + "/LICENSE.md",
        buildDirectory.ToString() + "/README.md"
    };

    Zip(MakeAbsolute(buildDirectory), "./" + projectName + "-" + version + ".zip", files);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Create-Archive");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);

//////////////////////////////////////////////////////////////////////
// FUNCTIONS
//////////////////////////////////////////////////////////////////////

public static int GetPersistentBuildNumber(string baseDirectory)
{
    int buildNumber;
    string persistentPathName = System.IO.Path.Combine(baseDirectory, ".cache");
    string persistentFileName = System.IO.Path.Combine(persistentPathName, "build-number");

    try
    {
        if (!System.IO.Directory.Exists(persistentPathName))
        {
            System.IO.Directory.CreateDirectory(persistentPathName);
        }

        buildNumber = int.Parse(System.IO.File.ReadAllText(persistentFileName).Trim());
        buildNumber++;
    }
    catch
    {
        buildNumber = 1;
    }

    System.IO.File.WriteAllText(persistentFileName, buildNumber.ToString());

    return buildNumber;
}