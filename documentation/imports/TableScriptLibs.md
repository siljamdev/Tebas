# Standard TableScript Libraries

The standard libraries [TableScript](https://github.com/siljamdev/TableScript) provides are available in absolutely all scripts, globals, utils and properties.  
These are:

## stdlib
This library has been modified and no longer has all the function TableScript provides. The console output functions have been moved ot of here to the [tebas](./tebasImport.md) import.  
The remaining functions are:  
```
export function stdlib::join(self, separator){ EXTERN; } //Join all elements of a table with a seperator between them
export function stdlib::split(self, separators){ EXTERN; } //Split all elements of a table by multiple separators
export function stdlib::splitLines(self){ EXTERN; } //Split all elements of a table by all common line endings
export function stdlib::replace(self, originals, replacements){ EXTERN; } //Replace a set of substrings by their replacement
export function stdlib::indexOf(self, element){ EXTERN; } //Find the index of an element
export function stdlib::upper(self){ EXTERN; } //Transform all elements to uppercase
export function stdlib::lower(self){ EXTERN; } //Transform all elements to lowercase
export function stdlib::trim(self){ EXTERN; } //Trim whitespace from all elements
export function stdlib::removeQuotes(self){ EXTERN; } //Remove surrounding double quotes (") from all elements
export function stdlib::deleteAll(self, toDel){ EXTERN; } //Delete all matching elements from a table
export function stdlib::deleteAt(self, index){ EXTERN; } //Delete element at an index
export function stdlib::deleteEmpty(self){ EXTERN; } //Delete all 0-length elements
export function stdlib::reverse(self){ EXTERN; } //Reverse the order of a table
export function stdlib::shuffle(self){ EXTERN; } //Shuffle randomly the order of a table
export function stdlib::repeat(self, times){ EXTERN; } //Repeat some elements x times
export function stdlib::getMaxLength(){ EXTERN; } //Get the maximum table length
export function stdlib::getOS(){ EXTERN; } //Get the operating system, either 'windows', 'linux', 'macos' or ''
export function stdlib::getDate(){ EXTERN; } //Get date and hour in [yy, MM, dd, hh, mm, ss] format
export function stdlib::sleep(ms){ EXTERN; } //Sleep x miliseconds
```

Please check the [TableScript docs](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdLib.html) for extra information on these functions.  

## stdnum
This library is exactly the same. Check its functions in the [TableScript docs](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdNum.html).  
Some Tebas functions use stdnum nums as output, for multiple things such as error codes and sizes.  

## stdlist
This library is exactly the same. Check its functions in the [TableScript docs](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdList.html).  
Some Tebas functions use stdlist lists as output, for having several tables of output.  
