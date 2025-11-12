using SteganographyImplementation;

Parser parser = new Parser();

string mode, sourceImage;
string? input, destinationImage;
long? key;

try
{
    parser.Parse(args, out mode, out input, out key, out sourceImage, out destinationImage);
}
catch (ArgumentException e)
{
    Console.WriteLine($"Error: {e.Message}");
    Console.WriteLine(Parser.HelpInfo);
    return;
}

switch (mode)
{
    case "hide":
        long seed = destinationImage != null
            ? SteganographyUtils.HideMessage(input!, sourceImage, destinationImage)
            : SteganographyUtils.HideMessage(input!, sourceImage);

        Console.WriteLine($"Message hidden successfully. Key for finder: {seed}");
        break;

    case "find":
        string foundText = SteganographyUtils.FindMessage(key!.GetValueOrDefault(), sourceImage);
        Console.WriteLine("Found message:");
        Console.WriteLine(foundText);
        break;
}
