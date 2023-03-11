using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Pack);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution]
    private readonly Solution Solution;

    [GitVersion]
    private readonly GitVersion GitVersion;

    public AbsolutePath ArtifactsDir => RootDirectory / "artifacts";

    private const string _author = "Martin Wheatley";
    private const string _projectName = "CodeZero.Core";

    Target Clean => _ => _
        .Executes(() =>
        {
            DotNetTasks.DotNetClean(settings =>
                settings
                    .SetProject(Solution)
                    .SetConfiguration(Configuration));

            FileSystemTasks.EnsureCleanDirectory(ArtifactsDir);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetTasks.DotNetRestore(settings => settings
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild(settings => settings
                .SetNoRestore(true)
                .SetAuthors(_author)
                .SetPackageId(_projectName)
                .SetTreatWarningsAsErrors(true)
                .SetVersion(GitVersion.NuGetVersionV2)
                .SetAssemblyVersion(GitVersion.MajorMinorPatch)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetProperty("Commit", GitVersion.ShortSha)
                .SetConfiguration(Configuration)
                .SetProjectFile(Solution));
        });

    Target Pack => _ => _
        .DependsOn(Test)
        .Executes(() =>
        {
            DotNetTasks.DotNetPack(settings => settings
                .SetProject(Solution.GetProject(_projectName))
                .SetConfiguration(Configuration)
                .SetPackageId(_projectName)
                .SetVersion(GitVersion.NuGetVersionV2)
                .SetAuthors(_author)
                .SetNoBuild(true)
                .SetNoRestore(true)
                .SetOutputDirectory(ArtifactsDir));
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTasks.DotNetTest(settings => settings
                .SetNoBuild(true)
                .SetNoRestore(true)
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration));
        });
}
