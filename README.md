# Compression Algorithms Evaluation Tool

A .NET 8.0 application for evaluating and comparing **five lossless compression algorithms** - RLE, GZip, Brotli, LZ4 and Zstd - in **both sequential and parallel (multi-core) modes**, measuring compression ratio, speed, and speedup, then generating an interactive HTML report.

**Parallel compression works by splitting each input file into chunks, compressing each chunk on a separate CPU core using `Parallel.For`, then concatenating the results - trading a small compression-ratio loss for multi-core speedup.**

## Requirements

- .NET 8.0 Runtime or SDK
- Windows, Linux or macOS

## Installation

- git clone https://github.com/AlexandraMarcu13/CompressionEvaluation.git
- cd CompressionEvaluation
- dotnet build

## How to Use

Run the application:

- dotnet run --project CompressionEvaluation

The tool will generate 11 sample test files if the user chooses it, compress each file from the Input directory using all 5 algorithms(sequential+parallel), measure compression time, ratio and space saving, verify data integrity
by comparing hash values, and generate an HTML report. The generated .html file will open in browser to view the results after the project was executed successfully.

## Custom Test Files

Place your own files in the TestFiles folder, which will be created after the first execution of the program. The tool will process any file that is not already compressed.Also the application can generate test files to use if no files are provided beforehand.

## Algorithms Summary

| Algorithm | Best for | Speed | Ratio |
|---|---|---|---|
| **Brotli** | Text, JSON, HTML, XML | Slowest | Best |
| **Zstd** | General purpose, databases, filesystems | Medium | High |
| **LZ4** | Real-time, streaming, games | Fastest | Low |
| **GZip** | Universal compatibility (HTTP, email, archives) | Medium | Medium |
| **RLE** | Extremely repetitive data only | Fast | Very low (expands most files) |

### How each algorithm works

**Brotli** - LZ77 matching + context-modelled Huffman coding + a built-in static dictionary of ~13,000 common web/text words. Best ratio on text-like data, but the slowest of the five.

**Zstd** - Smart LZ77 matching (searches multiple candidates per position) + repeat-offset memory (last 3 match distances stored as tiny codes) + FSE entropy coding. Near-Brotli ratio at near-LZ4 speed — the best overall balance.

**LZ4** - Minimal-effort LZ77 matching with a trivial token format. Compresses and decompresses at memory-bandwidth speed. Lowest ratio but by far the fastest.

**GZip** - LZ77 with a 32 KB sliding window + Huffman coding. Universal compatibility (every browser, OS, and library supports it). Medium speed and ratio.

**RLE** - Replaces consecutive identical bytes with `[count][byte]` pairs. Only useful on data with long runs of the same byte (bitmaps, simple graphics, pure repetitive text). Expands most real-world data.

