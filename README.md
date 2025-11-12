# Steganography Tool

A command-line steganography application written in C# that allows you to hide and retrieve text messages in BMP images using LSB (Least Significant Bit) technique.

## Project Structure

The solution consists of three main projects:

- **Steganography** - CLI application (contains `Main` entry point and `Parser`)
- **SteganographyImplementation** - Class library (contains `SteganographyUtils`, `Hidder`, `Finder`, `BitPairEnumerator`, `BitPairCollector`)
- **SteganographyInterfaces** - Class library (contains `IHidder` and `IFinder` interfaces)

## Features

- Hide text messages in BMP images using LSB steganography
- Retrieve hidden messages using a generated key
- Support for both overriding original images and creating new copies
- Random pixel distribution for enhanced security

## Installation

### Prerequisites
- .NET 6.0 or later
- Windows OS (due to System.Drawing dependencies)

### Build
```bash
dotnet build
```

## Usage

### Hide a message in an image

**Override the original image:**
```bash
Steganography.exe -h -s "source.bmp" "Your secret message here"
```

**Create a new image with hidden message:**
```bash
Steganography.exe -h -s "source.bmp" -d "destination.bmp" "Your secret message here"
```

### Find a hidden message in an image

```bash
Steganography.exe -f -s "image.bmp" -k <key>
```

## Command Line Options

| Option | Description | Required For |
|--------|-------------|--------------|
| `-h` | Hide mode - hide text in image | Hide operations |
| `-f` | Find mode - find hidden text in image | Find operations |
| `-s filepath` | Source image path | Both modes |
| `-d filepath` | Destination image path (optional for hide mode) | Hide mode (optional) |
| `-k key` | Key for finding hidden text | Find mode |

## Examples

1. **Hide a message by overriding the original image:**
   ```bash
   Steganography.exe -h -s "vacation.bmp" "Meet me at the usual place at 5 PM"
   ```

2. **Hide a message by creating a copy:**
   ```bash
   Steganography.exe -h -s "original.bmp" -d "secret.bmp" "The password is 12345"
   ```

3. **Find a hidden message:**
   ```bash
   Steganography.exe -f -s "secret.bmp" -k 4294967298
   ```

## How It Works

### LSB Steganography
The tool uses Least Significant Bit steganography, where:
- Each byte of the secret message is split into 4 pairs of 2 bits
- These 2-bit pairs are embedded into the least significant bits of the RGB channels of randomly selected pixels
- Each pixel can store up to 3 pairs (6 bits) of data across its R, G, and B channels

### Security Features
- Random pixel selection using a seed based on input length
- Data distribution across multiple pixels
- Minimal visual impact on the carrier image

## Technical Details

- **Supported formats**: BMP (other formats may work but are not guaranteed)
- **Image requirements**: 24-bit or 32-bit BMP without compression
- **Message capacity**: Depends on image size (approximately 6 bits per pixel)
- **Key format**: 64-bit integer combining message length and random seed

## Limitations

- Works best with lossless formats (BMP, PNG without compression)
- JPEG and other lossy formats may corrupt hidden data due to compression
- Limited to text messages (UTF-8 encoding)
- Requires sufficient image size for the message length
