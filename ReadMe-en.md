🌏 [Chinese](./ReadMe.md) / English

# Grim Dawn Quest and Dialogue Text Extraction Tool

## Project Introduction
This is a tool developed using .NET 8 and WinForms to extract quest and dialogue text from the game Grim Dawn.

## Features

- Extracts text content from .qst and .cnv files and converts it to a .txt file.
- Supports multiple output methods:
- Output to a .txt file with the same name in the original directory.
- Output to the CnvQstToTxtNoLevel directory in the selected directory (all .txt files are in the same directory).
- Output to the CnvQstToTxtKeepLevel directory in the selected directory (preserves the directory structure).
- Supports switching between Simplified Chinese and English interfaces.
- Supports drag-and-drop directory functionality.
- Displays processing progress and detailed logs.

## System Requirements

- Windows operating system
- .NET 8.0 runtime

## How to Use

1. Browse or drag and drop a directory containing .qst and .cnv files.
2. Select the output method.
3. Click the Start button.
4. Wait for processing to complete and view the log output.

## Notes

- Ensure the directory path is correct.
- Processing a large number of files may take some time.
- Please submit issues if you encounter any problems.

## Development Environment

- Visual Studio 2022+
- .NET 8.0 SDK

## Build Instructions

1. Open your solution file using Visual Studio.
2. Compile your project.
3. Run the generated executable.