using System.Globalization;

Console.WriteLine("Nazwa pliku wejściowego: ");
string? fileName = Console.ReadLine();

if (!File.Exists(fileName) || fileName is "output.sc" or null)
{
    throw new ArgumentNullException();
}

Console.WriteLine("Skalowanie [S] czy przesuwanie [P]");
string? selection = Console.ReadLine();

if (selection is null or not ("S" or "P"))
{
    throw new ArgumentNullException();
}

float skala = 1;
float przesuniecieX = 0;
float przesuniecieZ = 0;
float przesuniecieY = 0;

if (selection is "S")
{
    Console.WriteLine("Skala w procentach: ");
    string? skalaStr = Console.ReadLine();

    if (skalaStr is null)
    {
        throw new ArgumentNullException();
    }

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
}
else
{
    Console.WriteLine("Przesunięcie X: ");
    string? przesuniecieXStr = Console.ReadLine();
    Console.WriteLine("Przesunięcie Z: ");
    string? przesuniecieZStr = Console.ReadLine();
    Console.WriteLine("Przesunięcie Y: ");
    string? przesuniecieYStr = Console.ReadLine();

    if (przesuniecieXStr is null || przesuniecieZStr is null || przesuniecieYStr is null)
    {
        throw new ArgumentNullException();
    }

    try
    {
        przesuniecieX = float.Parse(przesuniecieXStr);
        przesuniecieZ = float.Parse(przesuniecieZStr);
        przesuniecieY = float.Parse(przesuniecieYStr);
    }
    catch (FormatException)
    {
        try
        {
            przesuniecieX = float.Parse(przesuniecieXStr, new CultureInfo("en-US").NumberFormat);
            przesuniecieZ = float.Parse(przesuniecieZStr, new CultureInfo("en-US").NumberFormat);
            przesuniecieY = float.Parse(przesuniecieYStr, new CultureInfo("en-US").NumberFormat);
        }
        catch
        {
            throw new ArgumentException();
        }
    }
}

bool grupaObiektow = false;
foreach (string line in File.ReadLines(fileName))
{
    using StreamWriter file = new("output.sc", append: true);
    string[]? lineArr = line.Split(';');

    if (lineArr is null)
    {
        await file.WriteLineAsync(line);
        continue;
    }

    if (lineArr[0] is "EndMiscGroup")
    {
        grupaObiektow = false;
    }

    if ((lineArr[0] is "Track" or "TrackObject" or "TrackStructure" or "Misc" or "MiscGroup" or "TerrainPoint" or "Wires") && !grupaObiektow)
    {
        if (lineArr[2] is not "#Geoportal")
        {
            var ci = new CultureInfo("en-US").NumberFormat;
            lineArr[3] = (float.Parse(lineArr[3], ci) * (1 + skala / 100) + przesuniecieX).ToString("F4", ci);
            lineArr[4] = (float.Parse(lineArr[4], ci) + przesuniecieZ).ToString("F4", ci);
            lineArr[5] = (float.Parse(lineArr[5], ci) * (1 + skala / 100) + przesuniecieY).ToString("F4", ci);
        }
    }

    await file.WriteLineAsync(string.Join(';', lineArr));

    if (lineArr[0] is "MiscGroup")
    {
        grupaObiektow = true;
    }
}

Console.WriteLine("Done.");
Console.ReadLine();