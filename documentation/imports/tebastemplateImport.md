# tebastemplate Import

The `tebastemplate` import is available in all template scripts, globals, utils and properties.  
It provides functionality related to that specific template.  
The functions it provides are:  

### General template functions
```
export function tebastemplate::getPath()
```
Returns string as table, takes no arguments. Gets the path to the internal template directory, where all the template files are located
```
export function tebastemplate::getName()
```
Returns string as table, takes no arguments. Get the template name
```
export function tebastemplate::getAuthor()
```
Returns string as table, takes no arguments. Get the template author if possible, or an empty string
```
export function tebastemplate::getDescription()
```
Returns string as table, takes no arguments. Get the template description if possible, or an empty string
```
export function tebastemplate::getAllScripts()
```
Returns table, takes no arguments. Get all script names
```
export function tebastemplate::getAllGlobals()
```
Returns table, takes no arguments. Get all global script names
```
export function tebastemplate::runGlobal(global, args)
```
Returns bool as table, takes as arguments: table as string, table. Run a global script. Returns true if the operation was successful
```
export function tebastemplate::hasPermission(key)
```
Returns bool as table, takes as arguments: table as string. Check if the template has a permission
```
export function tebastemplate::cleanup()
```
Returns empty table, takes no arguments. Cleanup this template: cleans internal invalid or empty values

### Resource functions
All these functions access template level resources. When you reinstall(update) a template, they can be conserved if the designer wants to (KEEPRESOURCES file).  
```
export function tebastemplate::getResource(key)
```
Returns string as table, takes as arguments: table as string. Get a template resource
```
export function tebastemplate::setResource(key, value)
```
Returns empty table, takes as arguments: table as string, table as string. Set a template resource
```
export function tebastemplate::appendResource(key, value)
```
Returns empty table, takes as arguments: table as string, table as string. Append to the end of a template resource

### Process functions
All processes are run in the directory that `tebastemplate::getPath()` gives. The `directory` argument is appended to that path.  
That way, a process called like `tebastemplate::runProcess("git", "repo", ["-h"]);` will be run in `{tebastemplate::getPath()}/repo`.  
Before running, the user must confirm to run the process. The user can skip this and allow always with the `skipProcessConfirmation` template permission.  
```
export function tebastemplate::runProcess(command, directory, arguments)
```
Returns bool as table, takes as arguments: table as string, table as string, table. Run a process in the template path, printing its output. Returns false if any error occurred
```
export function tebastemplate::runProcessDetached(command, directory, arguments)
```
Returns bool as table, takes as arguments: table as string, table as string, table. Run a process detached in the template path, not printing its output. Returns false if any error occurred
```
export function tebastemplate::runProcessWithOutput(command, directory, arguments)
```
Returns table, takes as arguments: table as string, table as string, table. Run a process in the template path, and get its output as a [stdlist](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdList.html) list [stdout, stderr, exitcode]. Exitcode is a [stdnum](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdNum.html) num. If any error occurred, an empty table will be returned
```
export function tebastemplate::runProcessWithExitCode(command, directory, arguments)
```
Returns table, takes as arguments: table as string, table as string, table. Run a process in the template path, and get its exit code as a [stdnum](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdNum.html) num. If any error occurred, an empty table will be returned
```
export function tebastemplate::open(target)
```
Returns bool as table, takes as arguments: table as string. Opens a browser url, folder or file using the target as command (windows), using `xdg-open` (linux) or `open` (macos). Returns false if any error occurred

### File functions
All files in the directory that `tebastemplate::getPath()` gives. The `path` argument is appended to that path.  
That way, checking if a file exists like `tebastemplate::fileExists("list.txt")` will check if a file exists in `{tebastemplate::getPath()}/list.txt`.  
Modifying these files and folders does not require user confirmation, because they are in an internal folder.  
```
export function tebastemplate::fileExists(path)
```
Returns bool as table, takes as arguments: table as string. Returns true if a file exists in the template path
```
export function tebastemplate::fileRead(path)
```
Returns table, takes as arguments: table as string. Reads whole text of a file in the template path
```
export function tebastemplate::fileReadLines(path)
```
Returns table, takes as arguments: table as string. Returns lines of text of a file in the template path. Returns a table containg all lines
```
export function tebastemplate::fileWrite(path, content)
```
Returns bool as table, takes as arguments: table as string, table as string. Writes whole content to a file in the template path. Returns true if the operation was successful
```
export function tebastemplate::fileWriteLines(path, content)
```
Returns bool as table, takes as arguments: table as string, table. Writes whole lines to a file in the template path. Each element of `content` should represent a line. Returns true if the operation was successful
```
export function tebastemplate::fileAppend(path, content)
```
Returns bool as table, takes as arguments: table as string, table as string. Appends content to the end of a file in the template path. Returns true if the operation was successful
```
export function tebastemplate::fileAppendLines(path, content)
```
Returns bool as table, takes as arguments: table as string, table. Appends lines to the end of a file in the template path. Returns true if the operation was successful
```
export function tebastemplate::fileDelete(path)
```
Returns bool as table, takes as arguments: table as string. Deletes a file in the template path. Returns true if the operation was successful
```
export function tebastemplate::fileMove(path, newPath)
```
Returns bool as table, takes as arguments: table as string, table as string. Moves a file to a new location in the template path. Returns true if the operation was successful
```
export function tebastemplate::fileCopy(path, copyPath)
```
Returns bool as table, takes as arguments: table as string, table as string. Copies a file to another location in the template path. Returns true if the operation was successful
```
export function tebastemplate::fileSize(path)
```
Returns string as table, takes as arguments: table as string. Get the size in bytes as a [stdnum](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdNum.html) num of a file in the template path. Returns an empty string if any error occurred
```
export function tebastemplate::folderExists(path)
```
Returns bool as table, takes as arguments: table as string. Returns true if a folder exists in the template path
```
export function tebastemplate::folderCreate(path)
```
Returns bool as table, takes as arguments: table as string. Create a folder in the template path. Returns true if the operation was successful
```
export function tebastemplate::folderDelete(path)
```
Returns bool as table, takes as arguments: table as string. Delete a folder in the template path. Returns true if the operation was successful
```
export function tebastemplate::folderMove(path, newPath)
```
Returns bool as table, takes as arguments: table as string, table as string. Move a folder to a new location in the template path. Returns true if the operation was successful
```
export function tebastemplate::folderListFiles(path, pattern)
```
Returns table, takes as arguments: table as string, table as string. Returns a table containing paths to all files that matched the pattern in the `path` directory. Pattern can use `*` and `?` wildcards. Returns a table with length -1 if any error occurred
```
export function tebastemplate::folderListChildFiles(path, pattern)
```
Returns table, takes as arguments: table as string, table as string. Returns a table containing paths to all files that matched the pattern in all directories of a directory in the template path. Pattern can use `*` and `?` wildcards. Returns a table with length -1 if any error occurred
```
export function tebastemplate::folderListFolders(path)
```
Returns table, takes as arguments: table as string. Get all subfolder paths in a folder in the template path. Returns a table with length -1 if any error occurred
