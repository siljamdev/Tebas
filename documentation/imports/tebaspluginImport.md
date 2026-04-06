# tebasplugin Import

The `tebasplugin` import is available in all plugin scripts, globals and utils.  
It provides functionality related to that specific plugin.  
The functions it provides are:  

### General plugin functions
```
export function tebasplugin::getPath(){ EXTERN; } //Get the plugin path
```
This functions returns the path to the internal template directory, where all the template files are located.  
```
export function tebasplugin::getName(){ EXTERN; } //Get the plugin name
export function tebasplugin::getAuthor(){ EXTERN; } //Get the plugin author if possible
export function tebasplugin::getDescription(){ EXTERN; } //Get the plugin description if possible
export function tebasplugin::getAllScripts(){ EXTERN; } //Get all script names
export function tebasplugin::getAllGlobals(){ EXTERN; } //Get all global script names
export function tebasplugin::hasPermission(key){ EXTERN; } //Get if the plugin has a permission
```
These functions have no additional complexity.  
```
export function tebasplugin::runGlobal(global, args){ EXTERN; } //Run a global script
```
Returns true if it was possible to run.  
```
export function tebasplugin::cleanup(){ EXTERN; } //Cleanup this plugin
```
This function deletes internal invalid or empty values.  

### Resource functions
```
export function tebasplugin::getResource(key){ EXTERN; } //Get a plugin resource
export function tebasplugin::setResource(key, value){ EXTERN; } //Set a plugin resource
export function tebasplugin::appendResource(key, value){ EXTERN; } //Append to a plugin resource
```
These functions have no additional complexity.  

### Process functions
All processes are run in the directory that `tebasplugin::getPath()` gives. The `directory` argument is appended to that path.  
That way, a process called like `tebasplugin::runProcess("git", "repo", ["-h"]);` will be run in `{tebasplugin::getPath()}/repo`.  
Before running, the user must confirm to run the process. The user can skip this and allow always with the `skipProcessConfirmation` plugin permission.  
```
export function tebasplugin::runProcess(command, directory, arguments){ EXTERN; } //Run a process in the plugin path, printing its output
```
Returns true if the process was run without an issue. Prints The process output to Tebas output.  
```
export function tebasplugin::runProcessDetached(command, directory, arguments){ EXTERN; } //Run a process detached in the plugin path, not printing its output
```
Returns true if the process was run without an issue.  
```
export function tebasplugin::runProcessWithOutput(command, directory, arguments){ EXTERN; } //Run a process in the plugin path, and get its output as a stdlist list [stdout, stderr, exitcode]. Exitcode is a stdnum num
```
Returns a [stdlist](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdList.html) list containg 3 tables.  
The first one, contains all stdout output of the process. The second one, contains all stderr output of the process. The third one, contains the process exit code as a [stdnum](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdNum.html) num.  
```
export function tebasplugin::runProcessWithExitCode(command, directory, arguments){ EXTERN; } //Run a process in the plugin path, and get its exit code
```
Returns a table of length equal to the exit code of the process.  
```
export function tebasplugin::open(target){ EXTERN; } //Open a url, folder or file in the plugin path
```
Opens a browser url, folder or file using the target as command (windows), using `xdg-open` (linux) or `open` (macos).  

### File functions
All files in the directory that `tebasplugin::getPath()` gives. The `path` argument is appended to that path.  
That way, checking if a file exists like `tebasplugin::fileExists("list.txt")` will check if a file exists in `{tebasplugin::getPath()}/list.txt`.  
Modifying these files and folders does not require user confirmation, because they are in an internal folder.  
```
export function tebasplugin::fileExists(path){ EXTERN; } //Checks if a file exists in the plugin path
```
These functions have no additional complexity.  
```
export function tebasplugin::fileRead(path){ EXTERN; } //Reads whole text of a file in the plugin path
```
Reads text as a single string. Return a length 1 table.  
```
export function tebasplugin::fileReadLines(path){ EXTERN; } //Reads text lines of a file in the plugin path
```
Reads text as lines. Returns a table containg all lines.  
```
export function tebasplugin::fileWrite(path, content){ EXTERN; } //Writes whole content to a file in the plugin path
```
Writes text as a string.  
```
export function tebasplugin::fileWriteLines(path, content){ EXTERN; } //Writes whole lines to a file in the plugin path
```
Writes text lines. `content` should have elements representing lines.  
```
export function tebasplugin::fileAppend(path, content){ EXTERN; } //Appends content to the end of a file in the plugin path
export function tebasplugin::fileAppendLines(path, content){ EXTERN; } //Appends lines to the end of a file in the plugin path
export function tebasplugin::fileDelete(path){ EXTERN; } //Deletes a file in the plugin path
export function tebasplugin::fileMove(path, newPath){ EXTERN; } //Moves a file to a new location in the plugin path
export function tebasplugin::fileCopy(path, copyPath){ EXTERN; } //Copies a file to another location in the plugin path
```
These functions have no additional complexity.  
```
export function tebasplugin::fileSize(path){ EXTERN; } //Get the size in bytes as a stdnum num of a file in the plugin path
```
Returns the file size in bytes as a [stdnum](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdNum.html) num.  
```
export function tebasplugin::folderExists(path){ EXTERN; } //Checks if a folder exists in the plugin path
export function tebasplugin::folderCreate(path){ EXTERN; } //Create a folder in the plugin path
export function tebasplugin::folderDelete(path){ EXTERN; } //Delete a folder in the plugin path
export function tebasplugin::folderMove(path, newPath){ EXTERN; } //Move a folder to a new location in the plugin path
```
These functions have no additional complexity.  
```
export function tebasplugin::folderListFiles(path, pattern){ EXTERN; } //Get all file paths in the top directory of a folder in the plugin path
```
Returns a table containing paths to all files that matched the pattern in the `path` directory. Pattern can use `*` and `?` wildcards.  
For example, you can do `tebasplugin::folderListFiles("", "*.txt")` to get a tebas-usable path to all `.txt` files in the base plugin path(`tebasplugin::getPath()`).  
```
export function tebasplugin::folderListChildFiles(path, pattern){ EXTERN; } //Get all file paths in all directories of a folder in the plugin path
```
Returns a table containing paths to all files that matched the pattern in the `path` directory and its subfolders. Pattern can use `*` and `?` wildcards.  
```
export function tebasplugin::folderListFolders(path){ EXTERN; } //Get all subfolder paths in a folder in the plugin path
```
Returns a table containing paths to all subfolders in the `path` directory.  
