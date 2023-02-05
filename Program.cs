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

float scale = 1;
float translationX = 0;
float translationZ = 0;
float translationY = 0;

if (selection is "S")
{
    Console.WriteLine("Skala w procentach: ");
    string? scaleString = Console.ReadLine();

    if (scaleString is null)
    {
        throw new ArgumentNullException();
    }

    try
    {
        scale = float.Parse(scaleString);
    }
    catch (FormatException)
    {
        try
        {
            scale = float.Parse(scaleString, new CultureInfo("en-US").NumberFormat);
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
    string? translationXString = Console.ReadLine();
    Console.WriteLine("Przesunięcie Z: ");
    string? translationZString = Console.ReadLine();
    Console.WriteLine("Przesunięcie Y: ");
    string? translationYString = Console.ReadLine();

    if (translationXString is null || translationZString is null || translationYString is null)
    {
        throw new ArgumentNullException();
    }

    try
    {
        translationX = float.Parse(translationXString);
        translationZ = float.Parse(translationZString);
        translationY = float.Parse(translationYString);
    }
    catch (FormatException)
    {
        try
        {
            translationX = float.Parse(translationXString, new CultureInfo("en-US").NumberFormat);
            translationZ = float.Parse(translationZString, new CultureInfo("en-US").NumberFormat);
            translationY = float.Parse(translationYString, new CultureInfo("en-US").NumberFormat);
        }
        catch
        {
            throw new ArgumentException();
        }
    }
}

Console.WriteLine();

bool objectGroup = false;
int createdLines = 0;
int lastDisplayedHashtag = 0;

int fileLength = File.ReadLines(fileName).Count();
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
        objectGroup = false;
    }

    if ((lineArr[0] is "Track" or "TrackObject" or "TrackStructure" or "Misc" or "MiscGroup" or "TerrainPoint"
            or "Wires") && !objectGroup)
    {
        if (lineArr[2] is not "#Geoportal")
        {
            translationX = 0;
            translationZ = 0;
            translationY = 0;
        }

        NumberFormatInfo ci = new CultureInfo("en-US").NumberFormat;
        lineArr[3] = (float.Parse(lineArr[3], ci) * (1 + scale / 100) + translationX).ToString("F4", ci);
        lineArr[4] = (float.Parse(lineArr[4], ci) + translationZ).ToString("F4", ci);
        lineArr[5] = (float.Parse(lineArr[5], ci) * (1 + scale / 100) + translationY).ToString("F4", ci);

        if (lineArr[2] is "BTrack")
        {
            lineArr[9] = (float.Parse(lineArr[9], ci) * (1 + scale / 100) + translationX).ToString("F4", ci);
            lineArr[9] = (float.Parse(lineArr[9], ci) + translationZ).ToString("F4", ci);
            lineArr[11] = (float.Parse(lineArr[11], ci) * (1 + scale / 100) + translationY).ToString("F4", ci);
        }
    }

    await file.WriteLineAsync(string.Join(';', lineArr));
    ++createdLines;

    // Displaying progress bar
    if (createdLines * 30 / fileLength > lastDisplayedHashtag)
    {
        ClearCurrentConsoleLine();
        lastDisplayedHashtag = createdLines * 30 / fileLength;
        Console.Write($"{createdLines * 100 / fileLength}%   ");
        for (int i = 0; i <= lastDisplayedHashtag; ++i)
        {
            Console.Write("#");
        }
    }

    if (lineArr[0] is "MiscGroup")
    {
        objectGroup = true;
    }
}

Console.WriteLine("\n\nDone.");
Console.ReadLine();

void ClearCurrentConsoleLine()
{
    int currentLineCursor = Console.CursorTop;
    Console.SetCursorPosition(0, Console.CursorTop);
    Console.Write(new string(' ', Console.WindowWidth));
    Console.SetCursorPosition(0, currentLineCursor);
}