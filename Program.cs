using System.Formats.Tar;
using static System.Console;

using System.IO.Compression;

// 🎸 Tar Issue that I wish you saw... 🎶

await using var s = File.OpenRead("./my-file.tar.gz");
await using var gz = new GZipStream(s, CompressionMode.Decompress, true);

// Change to true to see what should happen
var copyToMemoryStream = false;

var tarStream = copyToMemoryStream ? await CopyToMemoryStream(gz) : gz;
await using var tarReader = new TarReader(tarStream, true);

var temp = Directory.CreateDirectory("temp").FullName;

var entriesCount = 0;
while (await tarReader.GetNextEntryAsync() is { } entry)
{
	WriteLine(entry.Name);
	entriesCount++;
	
	var destination = Path.Combine(temp, entry.Name);
	if (entry.EntryType is TarEntryType.Directory)
	{
		Directory.CreateDirectory(destination);
	}
	else
	{
		await entry.ExtractToFileAsync(destination, true);
	}
}

WriteLine();
WriteLine("Should have encountered 140 tar entries");
WriteLine($"Actual: {entriesCount}");

async Task<Stream> CopyToMemoryStream(Stream source)
{
	var destination = new MemoryStream();
	await source.CopyToAsync(destination);
	destination.Seek(0, SeekOrigin.Begin);
	return destination;
}