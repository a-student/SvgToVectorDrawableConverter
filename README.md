# SVG to VectorDrawable Converter
Batch converter of SVG images to Android VectorDrawable XML resource files.

Put the output XML files into the ‘res/drawable’ directory of your app and reference to them in XML / Java code as to ordinary drawables.

## Requirements
Inkscape (free and open-source vector graphics software) must be installed. Download it from https://inkscape.org/en/download

The converter can be downloaded from https://github.com/a-student/SvgToVectorDrawableConverter/releases

## Running on Windows
.NET Framework 4.5 or higher is required. Usually it is already installed on your operating system (enabled by default in Windows 8). If you have older OS, please search the Microsoft web site for .NET Framework, download and install it.

Open command prompt and run
```
svg2vd.exe --help
```
This command prints usage and option explanations.

Typical usage:
```
svg2vd.exe -i *
```
(Converts all SVG files in current directory.)

## Running on OS X
OS X 10.7 or better and Mono are required. If you do not have Mono installed, please download and install it from http://www.mono-project.com/download

Open terminal and run
```
mono svg2vd.exe --help
```
This command prints usage and option explanations.

Typical usage:
```
mono svg2vd.exe -i *
```
(Converts all SVG files in current directory.)

## Not supported SVG elements
These SVG elements are not supported by VectorDrawable: patterns, masks, gradients. You need to manually eliminate them from the input files before conversion.

## Issues
If you have any problems with the converter, please create an issue on GitHub (https://github.com/a-student/SvgToVectorDrawableConverter/issues/new), explain the reproducing steps, and add link to the SVG file (link is optional but highly recommended).

## Alternatives
Less powerful but online converter http://inloop.github.io/svg2android
