# paths Import

The `paths` import is available in absolutely all scripts, globals, utils and properties.  
It provides base functionality for working with file/directory paths.  
In general, it is safe and recommended to always use `/` as separator.  
The functions it provides are:  

## Functions
```
export function paths::getExtension(path)
```
Returns string as table, takes as arguments: table as string. Get extension of a file path
```
export function paths::getFilename(path)
```
Returns string as table, takes as arguments: table as string. Get file name with extension of a file path
```
export function paths::getFilenameNoExtension(path)
```
Returns string as table, takes as arguments: table as string. Get file name without extension of a file path
```
export function paths::getDirectory(path)
```
Returns string as table, takes as arguments: table as string. Get parent directory of a path
```
export function paths::getSeparator()
```
Returns string as table, takes no arguments. Get default OS separator of paths