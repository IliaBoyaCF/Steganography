using System.Reflection;
using System.Text;

internal class Parser
{
    private static readonly string _programName = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly()?.Location ?? "stg");

    public Parser()
    {
    }

    public static string HelpInfo { get; internal set; } = GetHelpInfo();

    internal void Parse(string[] args, out string mode, out string? input, out long? key, out string originalImagePath, out string? resultImagePath)
    {
        mode = "";
        originalImagePath = "";
        key = null;
        input = null;
        resultImagePath = null;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-h":
                    if (!string.IsNullOrEmpty(mode))
                    {
                        throw new ArgumentException("Invalid use of flag '-h'");
                    }
                    mode = "hide";
                    break;
                case "-f":
                    if (!string.IsNullOrEmpty(mode))
                    {
                        throw new ArgumentException("Invalid use of flag '-f'");
                    }
                    mode = "find";
                    break;
                case "-s":
                    if (i + 1 < args.Length)
                        originalImagePath = args[++i];
                    else
                        throw new ArgumentException("Source image path expected after -s");
                    break;
                case "-k":
                    if (i + 1 < args.Length)
                        key = long.Parse(args[++i]);
                    else
                        throw new ArgumentException("Key for finding text in picture expected after -k");
                    break;
                case "-d":
                    if (i + 1 < args.Length)
                        resultImagePath = args[++i];
                    else
                        throw new ArgumentException("Destination image path expected after -d");
                    break;
                default:
                    input = args[i];
                    break;
            }
        }

        if (string.IsNullOrEmpty(originalImagePath))
            throw new ArgumentException("Image path is missing.");
        if (string.IsNullOrEmpty(mode))
            throw new ArgumentException("Mode is not selected.");

    }

    private static string GetHelpInfo()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Usage:");
        sb.AppendLine($"  Hide message in image: {_programName} -h -s \"source.bmp\" [-d \"destination.bmp\"] \"input text\"");
        sb.AppendLine($"  Find message in image: {_programName} -f -s \"image.bmp\" -k key");
        sb.AppendLine();
        sb.AppendLine("Modes:");
        sb.AppendLine("  -h            Hide mode - hide text in image");
        sb.AppendLine("  -f            Find mode - find hidden text in image");
        sb.AppendLine();
        sb.AppendLine("Options:");
        sb.AppendLine("  -s filepath   Source image path (required for both modes)");
        sb.AppendLine("  -d filepath   Destination image path (optional for hide mode)");
        sb.AppendLine("                If not specified, overrides the original image");
        sb.AppendLine("  -k key        Key for finding hidden text (required for find mode)");
        sb.AppendLine();
        sb.AppendLine("Examples:");
        sb.AppendLine($"  Hide message by overriding original image:");
        sb.AppendLine($"    {_programName} -h -s \"source.bmp\" \"Hello, world!\"");
        sb.AppendLine();
        sb.AppendLine($"  Hide message by creating a copy:");
        sb.AppendLine($"    {_programName} -h -s \"source.bmp\" -d \"destination.bmp\" \"Hello, world!\"");
        sb.AppendLine();
        sb.AppendLine($"  Find hidden message:");
        sb.AppendLine($"    {_programName} -f -s \"image.bmp\" -k 123456789");
        sb.AppendLine();

        return sb.ToString();
    }
}
