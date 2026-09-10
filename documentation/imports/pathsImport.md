# paths Import

The `paths` import is available in absolutely all scripts, globals, utils and properties.  
It provides base functionality for working with file/directory paths.  
In general, it is safe and recommended to always use `/` as separator.  

## Globals
```
export global separator;
```
Default OS separator of paths


## Functions
```
export function paths::getExtension(path);
```
Takes 1 arguments: table path as string. Returns table as string. Get extension of a file path

```
export function paths::getFilename(path);
```
Takes 1 arguments: table path as string. Returns table as string. Get file name with extension of a file path

```
export function paths::getFilenameNoExtension(path);
```
Takes 1 arguments: table path as string. Returns table as string. Get file name without extension of a file path

```
export function paths::getDirectory(path);
```
Takes 1 arguments: table path as string. Returns table as string. Get parent directory of a path

```
export function paths::getSeparator();
```
Takes 0 arguments. Returns table as string. Get default OS separator of paths