# Sccg

[![GitHub Pages](https://github.com/ryota2357/Sccg/actions/workflows/gh-pages.yml/badge.svg)](https://github.com/ryota2357/Sccg/actions/workflows/gh-pages.yml)
[![Test](https://github.com/ryota2357/Sccg/actions/workflows/dotnet-test.yml/badge.svg)](https://github.com/ryota2357/Sccg/actions/workflows/dotnet-test.yml)

> **S**criptable **C**olor **C**onfiguration **G**enerator.

Sccg is a tool to generate color schemes for your terminal, editor, etc.

## Goals

- Fully scriptable
  - Separate the each generation logic, `Source`, `Formatter`, `Converter` and `Writer`.
  - You can write your own `Source`, `Formatter`, `Converter` and `Writer` with C#.
- Unified API and Cross-platform
  - Each platform has its own color scheme format.
  - Sccg provides a unified API to generate color schemes for each platform.
  - You can set the color with `Set` and can link to other color with `Link`.
- Foolproof
  - There are may color groups, such as 'Comment', '@type.builtin', 'Ansi 0 Color', etc.
  - All color groups are defined in the `Source`, so you get completion, type check and so on.
  - Cycle reference will be detected by `Source`.

## Usage

TODO

## Builtin

### Sources

- Vim
  - [VimEditorHighlightSource](./Sccg.Builtin/Sources/VimEditorHighlightSource.cs)
  - [VimSyntaxGroupSource](./Sccg.Builtin/Sources/VimSyntaxGroupSource.cs)
  - [VimCustomGroupSource](./Sccg.Builtin/Sources/VimCustomGroupSource.cs)
- Neovim
  - [NeovimEditorHighlightSource](./Sccg.Builtin/Sources/NeovimEditorHighlightSource.cs)
  - [VimSyntaxGroupSource](./Sccg.Builtin/Sources/VimSyntaxGroupSource.cs)
  - [NeovimTreesitterHighlightSource](./Sccg.Builtin/Sources/NeovimTreesitterHighlightSource.cs)
  - [NeovimLspDiagnosticHighlightSource](./Sccg.Builtin/Sources/NeovimLspDiagnosticHighlightSource.cs)
  - [VimCustomGroupSource](./Sccg.Builtin/Sources/VimCustomGroupSource.cs)
- Iterm2
  - [Iterm2ColorsSource](./Sccg.Builtin/Sources/Iterm2ColorsSource.cs)
- Alacritty
  - [AlacrittyColorsSource](./Sccg.Builtin/Sources/AlacrittyColorsSource.cs)

### Formatters

- Vim
  - [VimFormatter](./Sccg.Builtin/Formatters/VimFormatter.cs)
- Neovim
  - [NeovimFormatter](./Sccg.Builtin/Formatters/NeovimFormatter.cs)
- Iterm2
  - [Iterm2Formatter](./Sccg.Builtin/Formatters/Iterm2Formatter.cs)
- Alacritty
  - [AlacrittyFormatter](./Sccg.Builtin/Formatters/AlacrittyFormatter.cs)

### Writers

- Vim/Neovim/Iterm2/Alacritty
  - [TextFileWriter](./Sccg.Builtin/Writers/TextFileWriter.cs)

## Examples

TODO