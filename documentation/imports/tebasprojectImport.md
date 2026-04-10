# tebasproject Import

The `tebasproject` import is available in all template/plugin scripts, utils (only when imported in from somewhere that has access to it) and template properties.  
It provides functionality related to that specific project.  
The functions it provides are:  

### General project functions
```
export function tebasproject::getPath()
```
Returns string as table, takes no arguments. Returns the project directory path, where the `.tebas` file is located
```
export function tebasproject::getName()
```
Returns string as table, takes no arguments. Get the project name
```
export function tebasproject::getCreationDate()
```
Returns table, takes no arguments. Get the date and hour of creation in [yy, MM, dd, hh, mm, ss] format
```
export function tebasproject::getTemplateName()
```
Returns string as table, takes no arguments. Get the name of the template used in the project
```
export function tebasproject::runScriptOrGlobal(script, args)
```
Returns bool as table, takes as arguments: table as string, table. Run a template script or global. Returns true if the operation was successful
```
export function tebasproject::runScript(script, args)
```
Returns bool as table, takes as arguments: table as string, table. Run a template script. Returns true if the operation was successful
```
export function tebasproject::runPluginScriptOrGlobal(plugin, script, args)
```
Returns bool as table, takes as arguments: table as string, table as string, table. Run a plugin script or global. Returns true if the operation was successful
```
export function tebasproject::runPluginScript(plugin, script, args)
```
Returns bool as table, takes as arguments: table as string, table as string, table. Run a plugin script. Returns true if the operation was successful
```
export function tebasproject::getProperty(key)
```
Returns table, takes as arguments: table as string. Get a project property or an empty table if not implemented
```
export function tebasproject::cleanup()
```
Returns empty table, takes no arguments. Cleanup this project: cleans internal invalid or empty values

### Resource functions
All these functions access template level resources. When you reinstall(update) a template, they can be conserved if the designer wants to (KEEPRESOURCES file).  
```
export function tebasproject::getResource(key)
```
Returns string as table, takes as arguments: table as string. Get a project resource
```
export function tebasproject::setResource(key, value)
```
Returns empty table, takes as arguments: table as string, table as string. Set a project resource
```
export function tebasproject::appendResource(key, value)
```
Returns empty table, takes as arguments: table as string, table as string. Append to the end of a project resource
```
export function tebasproject::getAllResourceKeys()
```
Returns table, takes no arguments. Get all keys with a value in project resources

### Build & install functions
All paths of these functions are in the project directory, the one `tebasproject::getPath()` gives. The argument is appended to that path.  
```
export function tebasproject::templateBuild(sourceDirectory, outDirectory)
```
Returns bool as table, takes as arguments: table as string, table as string. Build a template from source in a directory in the project path. Returns true if the operation was successful
```
export function tebasproject::pluginBuild(sourceDirectory, outDirectory)
```
Returns bool as table, takes as arguments: table as string, table as string. Build a plugin from source in a directory in the project path. Returns true if the operation was successful
```
export function tebasproject::templateInstallLocal(path)
```
Returns bool as table, takes as arguments: table as string. Install a template from a file in the project path. Requires user confirmation. The user can skip this and allow always with the `skipInstallationConfirmation` template/plugin permission
```
export function tebasproject::pluginInstallLocal(path)
```
Returns bool as table, takes as arguments: table as string. Install a plugin from a file in the project path. Requires user confirmation. The user can skip this and allow always with the `skipInstallationConfirmation` template/plugin permission

### Process functions
All processes are run in the project directory, the one `tebasproject::getPath()` gives. The `directory` argument is appended to that path.  
That way, a process called like `tebasproject::runProcess("git", "repo", ["-h"]);` will be run in `{tebasproject::getPath()}/repo`.  
Before running, the user must confirm to run the process. The user can skip this and allow always with the `skipProcessConfirmation` template/plugin permission.  
```
export function tebasproject::runProcess(command, directory, arguments)
```
Returns bool as table, takes as arguments: table as string, table as string, table. Run a process in the template path, printing its output. Returns false if any error occurred
```
export function tebasproject::runProcessDetached(command, directory, arguments)
```
Returns bool as table, takes as arguments: table as string, table as string, table. Run a process detached in the template path, not printing its output. Returns false if any error occurred
```
export function tebasproject::runProcessWithOutput(command, directory, arguments)
```
Returns table, takes as arguments: table as string, table as string, table. Run a process in the template path, and get its output as a [stdlist](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdList.html) list [stdout, stderr, exitcode]. Exitcode is a [stdnum](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdNum.html) num. If any error occurred, an empty table will be returned
```
export function tebasproject::runProcessWithExitCode(command, directory, arguments)
```
Returns table, takes as arguments: table as string, table as string, table. Run a process in the template path, and get its exit code as a [stdnum](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdNum.html) num. If any error occurred, an empty table will be returned
```
export function tebasproject::open(target)
```
Returns bool as table, takes as arguments: table as string. Opens a browser url, folder or file using the target as command (windows), using `xdg-open` (linux) or `open` (macos). Returns false if any error occurred

### File functions
All files in the project directory, the one `tebasproject::getPath()` gives. The `path` argument is appended to that path.  
That way, checking if a file exists like `tebasproject::fileExists("list.txt")` will check if a file exists in `{tebasproject::getPath()}/list.txt`.  
Some file operations, like writing, moving, or deleting, require user confirmation because these files are user project files. The user can skip this and allow always with the `skipFileConfirmation` template/plugin permission.  
```
export function tebasproject::fileExists(path)
```
Returns bool as table, takes as arguments: table as string. Returns true if a file exists in the template path
```
export function tebasproject::fileRead(path)
```
Returns table, takes as arguments: table as string. Reads whole text of a file in the template path
```
export function tebasproject::fileReadLines(path)
```
Returns table, takes as arguments: table as string. Returns lines of text of a file in the template path. Returns a table containg all lines
```
export function tebasproject::fileWrite(path, content)
```
Returns bool as table, takes as arguments: table as string, table as string. Writes whole content to a file in the template path. Returns true if the operation was successful
```
export function tebasproject::fileWriteLines(path, content)
```
Returns bool as table, takes as arguments: table as string, table. Writes whole lines to a file in the template path. Each element of `content` should represent a line. Returns true if the operation was successful
```
export function tebasproject::fileAppend(path, content)
```
Returns bool as table, takes as arguments: table as string, table as string. Appends content to the end of a file in the template path. Returns true if the operation was successful
```
export function tebasproject::fileAppendLines(path, content)
```
Returns bool as table, takes as arguments: table as string, table. Appends lines to the end of a file in the template path. Returns true if the operation was successful
```
export function tebasproject::fileDelete(path)
```
Returns bool as table, takes as arguments: table as string. Deletes a file in the template path. Returns true if the operation was successful
```
export function tebasproject::fileMove(path, newPath)
```
Returns bool as table, takes as arguments: table as string, table as string. Moves a file to a new location in the template path. Returns true if the operation was successful
```
export function tebasproject::fileCopy(path, copyPath)
```
Returns bool as table, takes as arguments: table as string, table as string. Copies a file to another location in the template path. Returns true if the operation was successful
```
export function tebasproject::fileSize(path)
```
Returns string as table, takes as arguments: table as string. Get the size in bytes as a [stdnum](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdNum.html) num of a file in the template path. Returns an empty string if any error occurred
```
export function tebasproject::folderExists(path)
```
Returns bool as table, takes as arguments: table as string. Returns true if a folder exists in the template path
```
export function tebasproject::folderCreate(path)
```
Returns bool as table, takes as arguments: table as string. Create a folder in the template path. Returns true if the operation was successful
```
export function tebasproject::folderDelete(path)
```
Returns bool as table, takes as arguments: table as string. Delete a folder in the template path. Returns true if the operation was successful
```
export function tebasproject::folderMove(path, newPath)
```
Returns bool as table, takes as arguments: table as string, table as string. Move a folder to a new location in the template path. Returns true if the operation was successful
```
export function tebasproject::folderListFiles(path, pattern)
```
Returns table, takes as arguments: table as string, table as string. Returns a table containing paths to all files that matched the pattern in the `path` directory. Pattern can use `*` and `?` wildcards. Returns a table with length -1 if any error occurred
```
export function tebasproject::folderListChildFiles(path, pattern)
```
Returns table, takes as arguments: table as string, table as string. Returns a table containing paths to all files that matched the pattern in all directories of a directory in the template path. Pattern can use `*` and `?` wildcards. Returns a table with length -1 if any error occurred
```
export function tebasproject::folderListFolders(path)
```
Returns table, takes as arguments: table as string. Get all subfolder paths in a folder in the template path. Returns a table with length -1 if any error occurred
