using System.Collections.Immutable;
using System.IO.Compression;

if (args.Length != 1)
{
    Console.WriteLine("Invalid arguments. Expected (D:\\Authi.BrowserExtension)");
    return;
}
var rootDirectory = args[0];

if (!Directory.Exists(rootDirectory))
{
    Console.WriteLine($"Directory '{rootDirectory}' not found");
    return;
}

var binDirectory = Path.Combine(rootDirectory, "bin");
if (Directory.Exists(binDirectory))
{
    Directory.Delete(binDirectory, true);
}

var bundleFiles = Directory.GetFiles(rootDirectory);
var bundleDirectories = Directory.GetDirectories(rootDirectory);

var platforms = bundleFiles
    .Select(Path.GetFileName)
    .OfType<string>()
    .Where(x => x.StartsWith("manifest.") && x.EndsWith(".json"))
    .Select(x => x.Replace("manifest.", string.Empty).Replace(".json", string.Empty))
    .ToImmutableList();

if (platforms.IsEmpty)
{
    Console.WriteLine($"No manifests found. Expected (manifest.browsername.json)");
    return;
}

Directory.CreateDirectory(binDirectory);

foreach (var platform in platforms)
{
    var platformDirectory = Path.Combine(binDirectory, platform);
    Directory.CreateDirectory(platformDirectory);
    foreach (var bundleDirectory in bundleDirectories)
    {
        var directoryName = Path.GetFileName(bundleDirectory);
        var destination = Path.Combine(platformDirectory, directoryName);
        CopyDirectory(bundleDirectory, destination, true);
    }
    foreach (var bundleFile in bundleFiles)
    {
        var fileName = Path.GetFileName(bundleFile);
        var destination = Path.Combine(platformDirectory, fileName);
        File.Copy(bundleFile, destination, true);
    }

    foreach (var manifestPlatfrom in platforms)
    {
        if (manifestPlatfrom == platform)
        {
            var oldManifestName = $"manifest.{manifestPlatfrom}.json";
            var oldManifestPath = Path.Combine(platformDirectory, oldManifestName);
            var newManifestName = "manifest.json";
            var newManifestPath = Path.Combine(platformDirectory, newManifestName);
            File.Move(oldManifestPath, newManifestPath);
        }
        else
        {
            var oldManifestName = $"manifest.{manifestPlatfrom}.json";
            var oldManifestPath = Path.Combine(platformDirectory, oldManifestName);
            File.Delete(oldManifestPath);
        }
    }

    ZipFile.CreateFromDirectory(platformDirectory, platformDirectory + ".zip");

    Directory.Delete(platformDirectory, true);
}

static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
{
    var dir = new DirectoryInfo(sourceDir);

    if (!dir.Exists)
        throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

    DirectoryInfo[] dirs = dir.GetDirectories();

    Directory.CreateDirectory(destinationDir);

    foreach (FileInfo file in dir.GetFiles())
    {
        string targetFilePath = Path.Combine(destinationDir, file.Name);
        file.CopyTo(targetFilePath);
    }

    if (recursive)
    {
        foreach (DirectoryInfo subDir in dirs)
        {
            string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
            CopyDirectory(subDir.FullName, newDestinationDir, true);
        }
    }
}