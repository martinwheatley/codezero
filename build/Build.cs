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
    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution]
    private readonly Solution Solution;

    [GitVersion]
    private readonly GitVersion GitVersion;

    public AbsolutePath ArtifactsDir => RootDirectory / "artifacts";

    private const string _author = "Martin Wheatley";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            
        });

    Target Restore => _ => _
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
                .SetPackageId("Wheatley.Prelude")
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
                .SetProject(Solution.GetProject("category-theory"))
                .SetConfiguration(Configuration)
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
            ;
            // DotNetTasks.DotNetTest(settings =>
            //     settings.SetProject)
        });
}
