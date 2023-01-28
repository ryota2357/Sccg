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

## Builtin Support

- Vim
  - Editor/Syntax highlight
- Neovim
  - Editor/Syntax highlight
  - Treesitter highlight
- Terminal
  - Iterm2

## Examples

TODO
