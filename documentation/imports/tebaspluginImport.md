# tebasplugin Import

The `tebasplugin` import is available in all plugin scripts, globals and utils.  
It provides functionality related to that specific plugin.  
The functions it provides are:  

### General plugin functions
```
export function tebasplugin::getPath()
```
Returns string as table, takes no arguments. Gets the path to the internal plugin directory, where all the plugin files are located
```
export function tebasplugin::getName()
```
Returns string as table, takes no arguments. Get the plugin name
```
export function tebasplugin::getAuthor()
```
Returns string as table, takes no arguments. Get the plugin author if possible, or an empty string
```
export function tebasplugin::getDescription()
```
Returns string as table, takes no arguments. Get the plugin description if possible, or an empty string
```
export function tebasplugin::getAllScripts()
```
Returns table, takes no arguments. Get all script names
```
export function tebasplugin::getAllGlobals()
```
Returns table, takes no arguments. Get all global script names
```
export function tebasplugin::runGlobal(global, args)
```
Returns bool as table, takes as arguments: table as string, table. Run a global script. Returns true if the operation was successful
```
export function tebasplugin::hasPermission(key)
```
Returns bool as table, takes as arguments: table as string. Check if the plugin has a permission
```
export function tebasplugin::cleanup()
```
Returns empty table, takes no arguments. Cleanup this plugin: cleans internal invalid or empty values

### Resource functions
All these functions access plugin level resources. When you reinstall(update) a plugin, they can be conserved if the designer wants to (KEEPRESOURCES file).  
```
export function tebasplugin::getResource(key)
```
Returns string as table, takes as arguments: table as string. Get a plugin resource
```
export function tebasplugin::setResource(key, value)
```
Returns empty table, takes as arguments: table as string, table as string. Set a plugin resource
```
export function tebasplugin::appendResource(key, value)
```
Returns empty table, takes as arguments: table as string, table as string. Append to the end of a plugin resource

### Process functions
All processes are run in the directory that `tebasplugin::getPath()` gives. The `directory` argument is appended to that path.  
That way, a process called like `tebasplugin::runProcess("git", "repo", ["-h"]);` will be run in `{tebasplugin::getPath()}/repo`.  
Before running, the user must confirm to run the process. The user can skip this and allow always with the `skipProcessConfirmation` plugin permission.  
```
export function tebasplugin::runProcess(command, directory, arguments)
```
Returns string as table, takes as arguments: table as string, table as string, table. Run a process in the plugin path, printing its output. Returns its exit code as a [stdnum](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdNum.html) num. If any error occurred, an empty table will be returned
```
export function tebasplugin::runProcessDetached(command, directory, arguments)
```
Returns bool as table, takes as arguments: table as string, table as string, table. Run a process detached in the plugin path, not printing its output. Returns false if any error occurred
```
export function tebasplugin::runProcessWithOutput(command, directory, arguments)
```
Returns table, takes as arguments: table as string, table as string, table. Run a process in the plugin path, and get its output as a [stdlist](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdList.html) list [stdout, stderr, exitcode]. Exitcode is a [stdnum](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdNum.html) num. If any error occurred, an empty table will be returned
```
export function tebastemplate::runProcessSilent(command, directory, arguments)
```
Returns string as table, takes as arguments: table as string, table as string, table. Run a process in the plugin path, not printing its output. Returns its exit code as a [stdnum](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdNum.html) num. If any error occurred, an empty table will be returned
```
export function tebasplugin::open(target)
```
Returns bool as table, takes as arguments: table as string. Opens a browser url, folder or file using the target as command (windows), using `xdg-open` (linux) or `open` (macos). Returns false if any error occurred

### File functions
All files in the directory that `tebasplugin::getPath()` gives. The `path` argument is appended to that path.  
That way, checking if a file exists like `tebasplugin::fileExists("list.txt")` will check if a file exists in `{tebasplugin::getPath()}/list.txt`.  
Modifying these files and folders does not require user confirmation, because they are in an internal folder.  
```
export function tebasplugin::fileExists(path)
```
Returns bool as table, takes as arguments: table as string. Returns true if a file exists in the plugin path
```
export function tebasplugin::fileRead(path)
```
Returns string as table, takes as arguments: table as string. Reads whole text of a file in the plugin path. Returns an empty table if any error occurred
```
export function tebasplugin::fileReadLines(path)
```
Returns table, takes as arguments: table as string. Returns lines of text of a file in the plugin path. Returns a table containg all lines
```
export function tebasplugin::fileWrite(path, content)
```
Returns bool as table, takes as arguments: table as string, table as string. Writes whole content to a file in the plugin path. Returns true if the operation was successful
```
export function tebasplugin::fileWriteLines(path, content)
```
Returns bool as table, takes as arguments: table as string, table. Writes whole lines to a file in the plugin path. Each element of `content` should represent a line. Returns true if the operation was successful
```
export function tebasplugin::fileAppend(path, content)
```
Returns bool as table, takes as arguments: table as string, table as string. Appends content to the end of a file in the plugin path. Returns true if the operation was successful
```
export function tebasplugin::fileAppendLines(path, content)
```
Returns bool as table, takes as arguments: table as string, table. Appends lines to the end of a file in the plugin path. Returns true if the operation was successful
```
export function tebasplugin::fileDelete(path)
```
Returns bool as table, takes as arguments: table as string. Deletes a file in the plugin path. Returns true if the operation was successful
```
export function tebasplugin::fileMove(path, newPath)
```
Returns bool as table, takes as arguments: table as string, table as string. Moves a file to a new location in the plugin path. Returns true if the operation was successful
```
export function tebasplugin::fileCopy(path, copyPath)
```
Returns bool as table, takes as arguments: table as string, table as string. Copies a file to another location in the plugin path. Returns true if the operation was successful
```
export function tebasplugin::fileSize(path)
```
Returns string as table, takes as arguments: table as string. Get the size in bytes as a [stdnum](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdNum.html) num of a file in the plugin path. Returns an empty table if any error occurred
```
export function tebasplugin::folderExists(path)
```
Returns bool as table, takes as arguments: table as string. Returns true if a folder exists in the plugin path
```
export function tebasplugin::folderCreate(path)
```
Returns bool as table, takes as arguments: table as string. Create a folder in the plugin path. Returns true if the operation was successful
```
export function tebasplugin::folderDelete(path)
```
Returns bool as table, takes as arguments: table as string. Delete a folder in the plugin path. Returns true if the operation was successful
```
export function tebasplugin::folderMove(path, newPath)
```
Returns bool as table, takes as arguments: table as string, table as string. Move a folder to a new location in the plugin path. Returns true if the operation was successful
```
export function tebasplugin::folderListFiles(path, pattern)
```
Returns table, takes as arguments: table as string, table as string. Returns a table containing paths to all files that matched the pattern in the `path` directory. Pattern can use `*` and `?` wildcards. Returns a table with length -1 if any error occurred
```
export function tebasplugin::folderListChildFiles(path, pattern)
```
Returns table, takes as arguments: table as string, table as string. Returns a table containing paths to all files that matched the pattern in all directories of a directory in the plugin path. Pattern can use `*` and `?` wildcards. Returns a table with length -1 if any error occurred
```
export function tebasplugin::folderListFolders(path)
```
Returns table, takes as arguments: table as string. Get all subfolder paths in a folder in the plugin path. Returns a table with length -1 if any error occurred
