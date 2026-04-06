# tebastemplate Import

The `tebastemplate` import is available in all template scripts, globals, utils and properties.  
It provides functionality related to that specific template.  
The functions it provides are:  

### General template functions
```
export function tebastemplate::getPath(){ EXTERN; } //Get the template path
```
This functions returns the path to the internal template directory, where all the template files are located.  
```
export function tebastemplate::getName(){ EXTERN; } //Get the template name
export function tebastemplate::getAuthor(){ EXTERN; } //Get the template author if possible
export function tebastemplate::getDescription(){ EXTERN; } //Get the template description if possible
export function tebastemplate::getAllScripts(){ EXTERN; } //Get all script names
export function tebastemplate::getAllGlobals(){ EXTERN; } //Get all global script names
export function tebastemplate::hasPermission(key){ EXTERN; } //Get if the template has a permission
```
These functions have no additional complexity.  
```
export function tebastemplate::runGlobal(global, args){ EXTERN; } //Run a global script
```
Returns true if it was possible to run.  
```
export function tebastemplate::cleanup(){ EXTERN; } //Cleanup this template
```
This function deletes internal invalid or empty values.  

### Resource functions
```
export function tebastemplate::getResource(key){ EXTERN; } //Get a template resource
export function tebastemplate::setResource(key, value){ EXTERN; } //Set a template resource
export function tebastemplate::appendResource(key, value){ EXTERN; } //Append to a template resource
```
These functions have no additional complexity.  

### Process functions
All processes are run in the directory that `tebastemplate::getPath()` gives. The `directory` argument is appended to that path.  
That way, a process called like `tebastemplate::runProcess("git", "repo", ["-h"]);` will be run in `{tebastemplate::getPath()}/repo`.  
Before running, the user must confirm to run the process. The user can skip this and allow always with the `skipProcessConfirmation` template permission.  
```
export function tebastemplate::runProcess(command, directory, arguments){ EXTERN; } //Run a process in the template path, printing its output
```
Returns true if the process was run without an issue. Prints The process output to Tebas output.  
```
export function tebastemplate::runProcessDetached(command, directory, arguments){ EXTERN; } //Run a process detached in the template path, not printing its output
```
Returns true if the process was run without an issue.  
```
export function tebastemplate::runProcessWithOutput(command, directory, arguments){ EXTERN; } //Run a process in the template path, and get its output as a stdlist list [stdout, stderr, exitcode]. Exitcode is a stdnum num
```
Returns a [stdlist](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdList.html) list containg 3 tables.  
The first one, contains all stdout output of the process. The second one, contains all stderr output of the process. The third one, contains the process exit code as a [stdnum](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdNum.html) num.  
```
export function tebastemplate::runProcessWithExitCode(command, directory, arguments){ EXTERN; } //Run a process in the template path, and get its exit code
```
Returns a table of length equal to the exit code of the process.  
```
export function tebastemplate::open(target){ EXTERN; } //Open a url, folder or file in the template path
```
Opens a browser url, folder or file using the target as command (windows), using `xdg-open` (linux) or `open` (macos).  

### File functions
All files in the directory that `tebastemplate::getPath()` gives. The `path` argument is appended to that path.  
That way, checking if a file exists like `tebastemplate::fileExists("list.txt")` will check if a file exists in `{tebastemplate::getPath()}/list.txt`.  
Modifying these files and folders does not require user confirmation, because they are in an internal folder.  
```
export function tebastemplate::fileExists(path){ EXTERN; } //Checks if a file exists in the template path
```
These functions have no additional complexity.  
```
export function tebastemplate::fileRead(path){ EXTERN; } //Reads whole text of a file in the template path
```
Reads text as a single string. Return a length 1 table.  
```
export function tebastemplate::fileReadLines(path){ EXTERN; } //Reads text lines of a file in the template path
```
Reads text as lines. Returns a table containg all lines.  
```
export function tebastemplate::fileWrite(path, content){ EXTERN; } //Writes whole content to a file in the template path
```
Writes text as a string.  
```
export function tebastemplate::fileWriteLines(path, content){ EXTERN; } //Writes whole lines to a file in the template path
```
Writes text lines. `content` should have elements representing lines.  
```
export function tebastemplate::fileAppend(path, content){ EXTERN; } //Appends content to the end of a file in the template path
export function tebastemplate::fileAppendLines(path, content){ EXTERN; } //Appends lines to the end of a file in the template path
export function tebastemplate::fileDelete(path){ EXTERN; } //Deletes a file in the template path
export function tebastemplate::fileMove(path, newPath){ EXTERN; } //Moves a file to a new location in the template path
export function tebastemplate::fileCopy(path, copyPath){ EXTERN; } //Copies a file to another location in the template path
```
These functions have no additional complexity.  
```
export function tebastemplate::fileSize(path){ EXTERN; } //Get the size in bytes as a stdnum num of a file in the template path
```
Returns the file size in bytes as a [stdnum](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdNum.html) num.  
```
export function tebastemplate::folderExists(path){ EXTERN; } //Checks if a folder exists in the template path
export function tebastemplate::folderCreate(path){ EXTERN; } //Create a folder in the template path
export function tebastemplate::folderDelete(path){ EXTERN; } //Delete a folder in the template path
export function tebastemplate::folderMove(path, newPath){ EXTERN; } //Move a folder to a new location in the template path
```
These functions have no additional complexity.  
```
export function tebastemplate::folderListFiles(path, pattern){ EXTERN; } //Get all file paths in the top directory of a folder in the template path
```
Returns a table containing paths to all files that matched the pattern in the `path` directory. Pattern can use `*` and `?` wildcards.  
For example, you can do `tebastemplate::folderListFiles("", "*.txt")` to get a tebas-usable path to all `.txt` files in the base template path(`tebastemplate::getPath()`).  
```
export function tebastemplate::folderListChildFiles(path, pattern){ EXTERN; } //Get all file paths in all directories of a folder in the template path
```
Returns a table containing paths to all files that matched the pattern in the `path` directory and its subfolders. Pattern can use `*` and `?` wildcards.  
```
export function tebastemplate::folderListFolders(path){ EXTERN; } //Get all subfolder paths in a folder in the template path
```
Returns a table containing paths to all subfolders in the `path` directory.  
