# tebasproject Import

The `tebasproject` import is available in all template/plugin scripts, utils (only when imported in from somewhere that has access to it) and template properties.  
It provides functionality related to that specific project.  
The functions it provides are:  

### General project functions
```
export function tebasproject::getPath(){ EXTERN; } //Get the project path
```
This functions returns the project directory path, where the `.tebas` file is located.  
```
export function tebasproject::getName(){ EXTERN; } //Get the project name
export function tebasproject::getCreationDate(){ EXTERN; } //Get the date and hour of creation in [yy, MM, dd, hh, mm, ss] format
```
These functions have no additional complexity.  
```
export function tebasproject::getTemplateName(){ EXTERN; } //Get the used template name
```
This function returns the name of the template used in the project.  
```
export function tebasproject::runScriptOrGlobal(script, args){ EXTERN; } //Run a script or a template global
export function tebasproject::runScript(script, args){ EXTERN; } //Run a script
export function tebasproject::runPluginScriptOrGlobal(plugin, script, args){ EXTERN; } //Run a plugin script or a plugin global
export function tebasproject::runPluginScript(plugin, script, args){ EXTERN; } //Run a plugin script
```
These functions return true if the script/global was run succesfully.  
```
export function tebasproject::getProperty(key){ EXTERN; } //Get a project property
```
This function returns the value of the property implemented by the template.  
```
export function tebasproject::cleanup(){ EXTERN; } //Cleanup this project
```
This function deletes internal invalid or empty values.  

### Resource functions
```
export function tebasproject::getResource(key){ EXTERN; } //Get a project resource
export function tebasproject::setResource(key, value){ EXTERN; } //Set a project resource
export function tebasproject::appendResource(key, value){ EXTERN; } //Append to a project resource
export function tebasproject::getAllResourceKeys(){ EXTERN; } //Get all keys with a value in project resources
```
These functions have no additional complexity.  

### Build & install functions
All paths of these functions are in the project directory, the one `tebasproject::getPath()` gives. The argument is appended to that path.  
```
export function tebasproject::templateBuild(sourceDirectory, outDirectory){ EXTERN; } //Build a template from source in a directory in the project path
export function tebasproject::pluginBuild(sourceDirectory, outDirectory){ EXTERN; } //Build a plugin from source in a directory in the project path
```
These functions have no additional complexity. 
```
export function tebasproject::templateInstallLocal(path){ EXTERN; } //Install a template from a file in the project path
export function tebasproject::pluginInstallLocal(path){ EXTERN; } //Install a plugin from a file in the project path
```
These functions require user confirmation. The user can skip this and allow always with the `skipInstallationConfirmation` template/plugin permission.  

### Process functions
All processes are run in the project directory, the one `tebasproject::getPath()` gives. The `directory` argument is appended to that path.  
That way, a process called like `tebasproject::runProcess("git", "repo", ["-h"]);` will be run in `{tebasproject::getPath()}/repo`.  
Before running, the user must confirm to run the process. The user can skip this and allow always with the `skipProcessConfirmation` template/plugin permission.  
```
export function tebasproject::runProcess(command, directory, arguments){ EXTERN; } //Run a process in the project path, printing its output
```
Returns true if the process was run without an issue. Prints The process output to Tebas output.  
```
export function tebasproject::runProcessDetached(command, directory, arguments){ EXTERN; } //Run a process detached in the project path, not printing its output
```
Returns true if the process was run without an issue.  
```
export function tebasproject::runProcessWithOutput(command, directory, arguments){ EXTERN; } //Run a process in the project path, and get its output as a stdlist list [stdout, stderr, exitcode]. Exitcode is a stdnum num
```
Returns a [stdlist](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdList.html) list containg 3 tables.  
The first one, contains all stdout output of the process. The second one, contains all stderr output of the process. The third one, contains the process exit code as a [stdnum](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdNum.html) num.  
```
export function tebasproject::runProcessWithExitCode(command, directory, arguments){ EXTERN; } //Run a process in the project path, and get its exit code
```
Returns a table of length equal to the exit code of the process.  
```
export function tebasproject::open(target){ EXTERN; } //Open a url, folder or file in the project path
```
Opens a browser url, folder or file using the target as command (windows), using `xdg-open` (linux) or `open` (macos).  

### File functions
All files in the project directory, the one `tebasproject::getPath()` gives. The `path` argument is appended to that path.  
That way, checking if a file exists like `tebasproject::fileExists("list.txt")` will check if a file exists in `{tebasproject::getPath()}/list.txt`.  
Some file operations, like writing, moving, or deleting, require user confirmation because these files are user project files. The user can skip this and allow always with the `skipFileConfirmation` template/plugin permission.  
```
export function tebasproject::fileExists(path){ EXTERN; } //Checks if a file exists in the project path
```
These functions have no additional complexity.  
```
export function tebasproject::fileRead(path){ EXTERN; } //Reads whole text of a file in the project path
```
Reads text as a single string. Return a length 1 table.  
```
export function tebasproject::fileReadLines(path){ EXTERN; } //Reads text lines of a file in the project path
```
Reads text as lines. Returns a table containg all lines.  
```
export function tebasproject::fileWrite(path, content){ EXTERN; } //Writes whole content to a file in the project path
```
Writes text as a string.  
```
export function tebasproject::fileWriteLines(path, content){ EXTERN; } //Writes whole lines to a file in the project path
```
Writes text lines. `content` should have elements representing lines.  
```
export function tebasproject::fileAppend(path, content){ EXTERN; } //Appends content to the end of a file in the project path
export function tebasproject::fileAppendLines(path, content){ EXTERN; } //Appends lines to the end of a file in the project path
export function tebasproject::fileDelete(path){ EXTERN; } //Deletes a file in the project path
export function tebasproject::fileMove(path, newPath){ EXTERN; } //Moves a file to a new location in the project path
export function tebasproject::fileCopy(path, copyPath){ EXTERN; } //Copies a file to another location in the project path
```
These functions have no additional complexity.  
```
export function tebasproject::fileSize(path){ EXTERN; } //Get the size in bytes as a stdnum num of a file in the project path
```
Returns the file size in bytes as a [stdnum](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdNum.html) num.  
```
export function tebasproject::folderExists(path){ EXTERN; } //Checks if a folder exists in the project path
export function tebasproject::folderCreate(path){ EXTERN; } //Create a folder in the project path
export function tebasproject::folderDelete(path){ EXTERN; } //Delete a folder in the project path
export function tebasproject::folderMove(path, newPath){ EXTERN; } //Move a folder to a new location in the project path
```
These functions have no additional complexity.  
```
export function tebasproject::folderListFiles(path, pattern){ EXTERN; } //Get all file paths in the top directory of a folder in the project path
```
Returns a table containing paths to all files that matched the pattern in the `path` directory. Pattern can use `*` and `?` wildcards.  
For example, you can do `tebasproject::folderListFiles("", "*.txt")` to get a tebas-usable path to all `.txt` files in the base project path(`tebasproject::getPath()`).  
```
export function tebasproject::folderListChildFiles(path, pattern){ EXTERN; } //Get all file paths in all directories of a folder in the project path
```
Returns a table containing paths to all files that matched the pattern in the `path` directory and its subfolders. Pattern can use `*` and `?` wildcards.  
```
export function tebasproject::folderListFolders(path){ EXTERN; } //Get all subfolder paths in a folder in the project path
```
Returns a table containing paths to all subfolders in the `path` directory.  
