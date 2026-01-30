using System.Text;
using System.Text.Json;

if (args.Length != 2)
{
    Console.WriteLine("Invalid arguments. Expected (inputFile.json, outputFile.cs)");
    return;
}
var inputFile = args[0];
var outputFile = args[1];

var localization = ReadJson(inputFile);
WriteCs(outputFile, localization);

static Dictionary<string, string> ReadJson(string inputFile)
{
    using var inputStream = File.OpenRead(inputFile);
    using var json = JsonDocument.Parse(inputStream);
    using var jsonEnumerator = json.RootElement.EnumerateObject();
    var localization = new Dictionary<string, string>();
    while (jsonEnumerator.MoveNext())
    {
        localization[jsonEnumerator.Current.Name] = jsonEnumerator.Current.Value.ToString();
    }
    return localization;
}

static void WriteCs(string outputFile, Dictionary<string, string> localization)
{
    static string Tab(int count)
    {
        return new string([.. Enumerable
            .Range(0, count * 4)
            .Select(x => ' ')]);
    }

    var builder = new StringBuilder()
        .AppendLine("// WARNING: Auto-generated file, do not edit!")
        .AppendLine()
        .AppendLine("namespace Authi.App.Logic.Localization")
        .AppendLine("{");

    string? currentBlock = null;
    foreach (var pair in localization.OrderBy(x => x.Key))
    {
        var keySplit = pair.Key.Split('.');
        var block = keySplit[0];
        var name = keySplit[1];
        var value = pair.Value;
        if (!string.IsNullOrEmpty(currentBlock) && block != currentBlock)
        {
            builder.AppendLine($"{Tab(1)}}}");
        }
        if (block != currentBlock)
        {
            builder
                .AppendLine($"{Tab(1)}public static class {block}")
                .AppendLine($"{Tab(1)}{{");
            currentBlock = block;
        }
        builder.AppendLine($"{Tab(2)}public const string {name} = \"{value}\";");
    }
    builder
        .AppendLine($"{Tab(1)}}}")
        .AppendLine("}");
    if (File.Exists(outputFile))
    {
        File.Delete(outputFile);
    }
    File.WriteAllText(outputFile, builder.ToString());
}