# Compression Algorithms Evaluation Tool

A .NET 8.0 application for evaluating and comparing five lossless compression algorithms: RLE, GZip, Brotli, LZ4 and Zstd.

## Requirements

- .NET 8.0 Runtime or SDK
- Windows, Linux or macOS

## Installation

git clone https://github.com/AlexandraMarcu13/CompressionEvaluation.git
cd CompressionEvaluation
dotnet build

## How to Use

Run the application:

dotnet run --project CompressionEvaluation

The tool will generate 11 sample test files if the user chooses it, compress each file from the Input directory using all 5 algorithms, measure compression time, ratio and space saving, verify data integrity
by comparing hash values, and generate an HTML report. The generated .html file will open in browser to view the results after the project was executed successfully.

## Custom Test Files

Place your own files in the TestFiles folder. The tool will process any file that is not already compressed.

## Algorithms Summary

Brotli - Best for text, JSON, HTML, XML. Slowest but highest compression ratio.
Zstd - Best for general purpose and databases. Medium speed, high ratio.
LZ4 - Best for real-time, streaming and games. Very fast, low ratio.
GZip - Best for compatibility. Medium speed and ratio.
RLE - Best only for extremely repetitive data. Fast but very limited use.

