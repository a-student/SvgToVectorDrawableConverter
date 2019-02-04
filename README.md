# SVG to VectorDrawable Converter
Batch converter of SVG images to Android VectorDrawable XML resource files.

Put the output XML files into the ‘res/drawable’ directory of your app and reference to them in XML / Java code as to ordinary drawables.

## [The online version of the converter](https://svg2vector.com) completely replaced this one.
_Please use that instead. This project is obsolete!_

### Not supported SVG features
These SVG elements are not supported by VectorDrawable: **patterns, masks, images,** etc.

VectorDrawable **fill-rule** is always **non-zero** and cannot be changed prior to Android 7.0 (Nougat).
If you end up with areas filled that should not be filled, that is because the SVG image was created using even-odd rule instead.
There are three ways to deal with this problem: try specifying the `--fix-fill-type` option, manually edit SVGs in vector graphics software or convert for Android 7.0+.

#### Specifying the `--fix-fill-type` option
This option is experimental, but it is worth trying first. The option uses complex mathematics to rebuild paths in such a way that rendering with any fill-rule leads to the same result.

#### Manual editing
If you are using Inkscape, open *Object* → *Fill and Stroke…* and in the *Fill* tab choose *Fill is solid unless a subpath is counterdirectional.*
If you see improperly filled area after this operation, using F2 tool select subpath around that area and apply *Path* → *Reverse* command from the main menu.

If you are using Sketch, select the path, right to the *Fills* property title there is settings icon, click it and choose *Non-Zero* option.
To reverse the path direction apply *Layer* → *Paths* → *Reverse Order* command from the main menu.

#### Converting for Android 7.0
This option does not fit all, because it requires setting `minSdkVersion` to 24 at least (Android 7.0 or later) in ‘build.gradle’.
Alternatively, you can try AndroidX VectorDrawableCompat.
