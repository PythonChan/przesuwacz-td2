using System.Globalization;

Console.WriteLine("Nazwa pliku wejściowego: ");
string? fileName = Console.ReadLine();

if (!File.Exists(fileName) || fileName is "output.sc" or null)
{
    throw new ArgumentNullException();
}

Console.WriteLine("Skala w procentach: ");
string? skalaStr = Console.ReadLine();

if (skalaStr is null)
{
    throw new ArgumentNullException();
}

float skala;

try
{
    skala = float.Parse(skalaStr);
}
catch (FormatException)
{
    try
    {
        skala = float.Parse(skalaStr, new CultureInfo("en-US").NumberFormat);
    }
    catch
    {
        throw new ArgumentException();
    }
}

foreach (string line in File.ReadLines(fileName))
{
    using StreamWriter file = new("output.sc", append: true);
    string[]? lineArr = line.Split(';');

    if (lineArr is null)
    {
        await file.WriteLineAsync(line);
        continue;
    }

    if (lineArr[0] is "Track" or "TrackObject" or "Misc" or "MiscGroup" or "TerrainPoint" or "Wires")
    {
        var ci = new CultureInfo("en-US").NumberFormat;
        lineArr[3] = (float.Parse(lineArr[3], ci) * (1 + skala / 100)).ToString("F4", ci);
        lineArr[5] = (float.Parse(lineArr[5], ci) * (1 + skala / 100)).ToString("F4", ci);
    }

    await file.WriteLineAsync(string.Join(';', lineArr));
}

Console.WriteLine("Done.");
Console.ReadLine();