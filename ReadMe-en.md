🌏 [Chinese](./README.md) / English
# Grim Dawn Quest and Dialogue Text Extraction Tool

## Project Introduction
This is a tool developed using .NET 8 and WinForms to extract quest and dialogue text from the game Grim Dawn.
- This project is based on Carson_N's PowerShell script [Original Source](https://forums.crateentertainment.com/t/japanese-community-of-the-rot/128705/71)

## Features

- Extracts text content from .qst and .cnv files and converts it to a txt file.
- Supports multiple output methods:
- Output to a txt file with the same name in the original directory.
- Output to the CnvQstToTxtNoLevel directory in the selected directory (all txt files are in the same directory). This option is recommended and is the default.
- Output to the CnvQstToTxtKeepLevel directory in the selected directory (preserves the directory structure).

## System Requirements

- Windows operating system
- .NET 8.0 runtime

## Usage

1. Browse or drag and drop a directory containing .qst and .cnv files.
2. Select the output method.
3. Click the Start button.
4. Wait for processing to complete and view the log output.

## Notes

- Please ensure the directory path is correct.
- Processing a large number of files may take some time.
- Please submit issues if you encounter any problems.

## Example - How to translate the complete Grim Dawn game or mods

1. Unpack text_en.arc and translate the content after the = sign.
2. Unpack Conversations.arc and Quests.arc (you can use another tool I released, [arzedit-GUI](https://github.com/zhangqi222/arzedit-GUI)).
3. Use GrimDawnCnvQstToTXT to generate a TXT file from the unpacked content.
4. Translate the generated TXT file into your desired language. Note: Do not modify the filename or lines in the file during translation, and do not delete the blank lines at the end (only translate, do not do anything else).
5. Place the translated TXT file in the text_zh folder (Chinese is used as an example here).
6. Also place the translated content from text_en in the text_zh folder.
7. Package it into text_zh.arc using [arzedit-GUI](https://github.com/zhangqi222/arzedit-GUI)
8. Place text_zh.arc in the resources directory.