#nullable enable
#r "nuget: CliWrap, 3.5.0"
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using CliWrap;
#if false
{
#endif

static async ValueTask BuildUnityPackageAssemblies(string packageName, string packageUrl, string[] assemblies)
{
    var buildDir = Path.Combine(Path.GetTempPath(), "unitybuild-" + Guid.NewGuid().ToString("N"));

    TryDeleteAll(buildDir);
    Directory.CreateDirectory(buildDir);
    Directory.CreateDirectory($"{buildDir}/Assets");
    Directory.CreateDirectory($"{buildDir}/Packages");
    Directory.CreateDirectory($"{buildDir}/ProjectSettings");
    var sb = new StringBuilder();
    sb.Append("{\n");
    sb.Append("  \"dependencies\": {\n");
    sb.Append("    \"");
    sb.Append(packageName);
    sb.Append("\": \"");
    sb.Append(packageUrl);
    sb.Append("\"\n");
    sb.Append("  }\n");
    sb.Append("}\n");
    File.WriteAllText($"{buildDir}/Packages/manifest.json", sb.ToString());
    sb.Clear();

    var latestUnity = await GetLatestUnity();
    var changeset = GetChangeset(latestUnity.path);
    if (!string.IsNullOrEmpty(changeset))
    {
        sb.Append($"m_EditorVersion: ");
        sb.Append(latestUnity.ver);
        sb.Append("\nm_EditorVersionWithRevision: ");
        sb.Append(latestUnity.ver);
        sb.Append(" (");
        sb.Append(changeset);
        sb.Append(")\n");
        File.WriteAllText($"{buildDir}/ProjectSettings/ProjectVersion.txt", sb.ToString());
    }

    var stdout = Console.OpenStandardOutput();
    var stderr = Console.OpenStandardError();

    try
    {
        await Cli.Wrap(latestUnity.path)
                .WithArguments($"-quit -batchmode -projectPath \"{buildDir}\" -nographics -logFile - -executeMethod UnityEditor.SyncVS.SyncSolution")
                .WithStandardOutputPipe(PipeTarget.ToStream(stdout))
                .WithStandardErrorPipe(PipeTarget.ToStream(stderr))
                .ExecuteAsync();

        foreach (var assembly in assemblies)
        {
            File.Copy($"{buildDir}/Library/ScriptAssemblies/{assembly}", assembly, true);
        }
    }
    finally
    {
        TryDeleteAll(buildDir);
    }
}

static string GetExecutingScriptPath([System.Runtime.CompilerServices.CallerFilePath] string path = "")
    => path;

static string GetExecutingScriptDirectory([System.Runtime.CompilerServices.CallerFilePath] string path = "")
    => Path.GetDirectoryName(path) ?? "";

static string? GetChangeset(string unityPath)
{
    var cacheDir = Path.Combine(Path.GetDirectoryName(unityPath)!, "Data/Resources/PackageManager/ProjectTemplates/libcache");
    if (string.IsNullOrEmpty(cacheDir) || !Directory.Exists(cacheDir))
    {
        return null;
    }

    var templateDir = Directory.GetDirectories(cacheDir)
                        .FirstOrDefault(x => Path.GetFileName(x).StartsWith("com.unity.template.3d"));
    if (string.IsNullOrEmpty(templateDir))
    {
        return null;
    }

    var lines = File.ReadAllLines(templateDir + "/PackageManager/ProjectCache");
    var line = lines.FirstOrDefault(x => x.StartsWith("m_EditorVersion:"));
    if (string.IsNullOrEmpty(line))
    {
        return null;
    }

    // e.g.) "m_EditorVersion: 2021.3.0f1 (6eacc8284459)"
    var changeset = line.Split(' ').Last().TrimStart('(').TrimEnd(')');
    if (Regex.IsMatch(changeset, @"^[0-9a-f]+$"))
    {
        return changeset;
    }

    return null;
}

static async ValueTask<(string ver, string path)> GetLatestUnity()
{
    var hubBin = Path.DirectorySeparatorChar == '\\'
                    ? @"C:\Program Files\Unity Hub\Unity Hub.exe"
                    : @"/Applications/Unity Hub.app/Contents/MacOS/Unity Hub";

    var stdout = new StringBuilder();
    await Cli.Wrap(hubBin)
        .WithArguments("-- --headless editors --installed")
        .WithValidation(CommandResultValidation.None)
        .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdout))
        .ExecuteAsync();
    var lines = stdout.ToString().Split('\n');
    var pairs = lines.Select(x => x.Split(", installed at"))
                    .Where(x => x.Length == 2)
                    .Select(x => (ver: x[0].Trim(), path: x[1].Trim()))
                    .OrderBy(x => x.ver)
                    .ToArray();
    return pairs.Last();
}

static void TryDeleteAll(string path)
{
    if (!Directory.Exists(path))
    {
        return;
    }

    foreach (var dir in Directory.GetDirectories(path))
    {
        TryDeleteAll(dir);
    }

    foreach (var file in Directory.GetFiles(path))
    {
        for (var i = 0; i < 10; i++)
        {
            try
            {
                File.Delete(file);
                break;
            }
            catch (Exception)
            {
                Thread.Sleep(300);
            }
        }
    }

    for (var i = 0; i < 10; i++)
    {
        try
        {
            Directory.Delete(path);
            break;
        }
        catch (Exception)
        {
            Thread.Sleep(300);
        }
    }
}
