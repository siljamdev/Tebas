# tebasproject Import

The `tebasproject` import is available in all template/plugin scripts, utils (only when imported in from somewhere that has access to it) and template properties.  
It provides functionality related to that specific project.  

## Globals
```
export global path;
```
Project path

```
export global name;
```
Project name

```
export global templateName;
```
Name of the template used in the project


## Functions

### General project functions
```
export function tebasproject::getPath();
```
Takes 0 arguments. Returns table as string. Get the project path

```
export function tebasproject::getName();
```
Takes 0 arguments. Returns table as string. Get the project name

```
export function tebasproject::getCreationDate();
```
Takes 0 arguments. Returns table as table. Get the date and hour of creation in [yy, MM, dd, hh, mm, ss] format

```
export function tebasproject::getTemplateName();
```
Takes 0 arguments. Returns table as string. Get the name of the template used in the project

```
export function tebasproject::runScriptOrGlobal(script, args);
```
Takes 2 arguments: table script as string table args as table. Returns table as bool. Run a template script or global. Returns true if the operation was successful

```
export function tebasproject::runScript(script, args);
```
Takes 2 arguments: table script as string table args as table. Returns table as bool. Get all script names

```
export function tebasproject::runPluginScriptOrGlobal(plugin, script, args);
```
Takes 3 arguments: table plugin as string table script as string table args as table. Returns table as bool. Get all script names

```
export function tebasproject::runPluginScript(plugin, script, args);
```
Takes 3 arguments: table plugin as string table script as string table args as table. Returns table as bool. Run a template script. Returns true if the operation was successful

```
export function tebasproject::getProperty(key);
```
Takes 1 argument: table key as string. Returns table as table. Get a project property or an empty table if not implemented

```
export function tebasproject::cleanup();
```
Takes 0 arguments. Returns an empty table. Cleanup this project: cleans internal invalid or empty values

### Resource functions
All these functions access template level resources. When you reinstall(update) a template, they can be conserved if the designer wants to (KEEPRESOURCES file).  
```
export function tebasproject::getResource(key);
```
Takes 1 argument: table key as string. Returns table as string. Get a project resource

```
export function tebasproject::setResource(key, value);
```
Takes 2 arguments: table key as string table value as string. Returns an empty table. Set a project resource

```
export function tebasproject::appendResource(key, value);
```
Takes 2 arguments: table key as string table value as string. Returns an empty table. Append to the end of a project resource

```
export function tebasproject::getAllResourceKeys();
```
Takes 0 arguments. Returns table as table. Get all keys with a value in project resources

### Build & install functions
All paths of these functions are in the project directory, the one `tebasproject::getPath()` gives. The argument is appended to that path.  
```
export function tebasproject::templateBuild(sourceDirectory, outDirectory);
```
Takes 2 arguments: table sourceDirectory as string table outDirectory as string. Returns table as bool. Build a template from source in a directory in the project path. Returns true if the operation was successful

```
export function tebasproject::pluginBuild(sourceDirectory, outDirectory);
```
Takes 2 arguments: table sourceDirectory as string table outDirectory as string. Returns table as bool. Build a plugin from source in a directory in the project path. Returns true if the operation was successful

### Process functions
All processes are run in the project directory, the one `tebasproject::getPath()` gives. The `directory` argument is appended to that path.  
That way, a process called like `tebasproject::runProcess("git", "repo", ["-h"]);` will be run in `{tebasproject::getPath()}/repo`.  
Before running, the user must confirm to run the process. The user can skip this and allow always with the `skipProcessConfirmation` template/plugin permission.  
```
export function tebasproject::runProcess(command, directory, arguments);
```
Takes 3 arguments: table command as string table directory as string table arguments as table. Returns table as string. Run a process in the project path, printing its output. Returns its exit code as a stdnum num. If any error occurred, an empty table will be returned

```
export function tebasproject::runProcessInteractive(command, directory, arguments);
```
Takes 3 arguments: table command as string table directory as string table arguments as table. Returns table as string. Run a process interactively in the project path, printing its output. Returns its exit code as a stdnum num. If any error occurred, an empty table will be returned

```
export function tebasproject::runProcessDetached(command, directory, arguments);
```
Takes 3 arguments: table command as string table directory as string table arguments as table. Returns table as bool. Run a process detached in the project path, not printing its output. Returns false if any error occurred

```
export function tebasproject::runProcessWithOutput(command, directory, arguments);
```
Takes 3 arguments: table command as string table directory as string table arguments as table. Returns table as table. Run a process in the project path, and get its output as a stdlist list [stdout, stderr, exitcode]. Exitcode is a stdnum num. If any error occurred, an empty table will be returned

```
export function tebasproject::runProcessSilent(command, directory, arguments);
```
Takes 3 arguments: table command as string table directory as string table arguments as table. Returns table as string. Run a process in the project path, not printing its output. Returns its exit code as a stdnum num. If any error occurred, an empty table will be returned

```
export function tebasproject::open(target);
```
Takes 1 argument: table target as string. Returns table as bool. Open a url, folder or file in the project path. Returns false if any error occurred

### File functions
All files in the project directory, the one `tebasproject::getPath()` gives. The `path` argument is appended to that path.  
That way, checking if a file exists like `tebasproject::fileExists("list.txt")` will check if a file exists in `{tebasproject::getPath()}/list.txt`.  
Some file operations, like writing, moving, or deleting, require user confirmation because these files are user project files. The user can skip this and allow always with the `skipFileConfirmation` template/plugin permission.  
```
export function tebasproject::fileExists(path);
```
Takes 1 argument: table path as string. Returns table as bool. Returns true if a file exists in the project path

```
export function tebasproject::fileRead(path);
```
Takes 1 argument: table path as string. Returns table as string. Reads whole text of a file in the project path

```
export function tebasproject::fileReadLines(path);
```
Takes 1 argument: table path as string. Returns table as table. Returns lines of text of a file in the project path

```
export function tebasproject::fileWrite(path, content);
```
Takes 2 arguments: table path as string table content as string. Returns table as bool. Writes whole content to a file in the project path. Returns true if the operation was successful

```
export function tebasproject::fileWriteLines(path, content);
```
Takes 2 arguments: table path as string table content as table. Returns table as bool. Writes whole lines to a file in the project path. Returns true if the operation was successful. Each element of `content` represents a line

```
export function tebasproject::fileAppend(path, content);
```
Takes 2 arguments: table path as string table content as string. Returns table as bool. Appends content to the end of a file in the project path. Returns true if the operation was successful

```
export function tebasproject::fileAppendLines(path, content);
```
Takes 2 arguments: table path as string table content as table. Returns table as bool. Appends lines to the end of a file in the project path. Returns true if the operation was successful. Each element of `content` represents a line

```
export function tebasproject::fileDelete(path);
```
Takes 1 argument: table path as string. Returns table as bool. Deletes a file in the project path. Returns true if the operation was successful

```
export function tebasproject::fileMove(path, newPath);
```
Takes 2 arguments: table path as string table newPath as string. Returns table as bool. Moves a file to a new location in the project path. Returns true if the operation was successful

```
export function tebasproject::fileCopy(path, copyPath);
```
Takes 2 arguments: table path as string table copyPath as string. Returns table as bool. Copies a file to another location in the project path. Returns true if the operation was successful

```
export function tebasproject::fileSize(path);
```
Takes 1 argument: table path as string. Returns table as string. Get the size in bytes as a stdnum num of a file in the project path. Returns an empty string if any error occurred

```
export function tebasproject::folderExists(path);
```
Takes 1 argument: table path as string. Returns table as bool. Returns true if a folder exists in the project path

```
export function tebasproject::folderCreate(path);
```
Takes 1 argument: table path as string. Returns table as bool. Create a folder in the project path. Returns true if the operation was successful

```
export function tebasproject::folderDelete(path);
```
Takes 1 argument: table path as string. Returns table as bool. Delete a folder in the project path. Returns true if the operation was successful

```
export function tebasproject::folderMove(path, newPath);
```
Takes 2 arguments: table path as string table newPath as string. Returns table as bool. Move a folder to a new location in the project path. Returns true if the operation was successful

```
export function tebasproject::folderListFiles(path, pattern);
```
Takes 2 arguments: table path as string table pattern as string. Returns table as table. Get all file paths in the top directory of a folder in the project path. Returns a table with length -1 if any error occurred. Pattern can use `*` and `?` wildcards.

```
export function tebasproject::folderListChildFiles(path, pattern);
```
Takes 2 arguments: table path as string table pattern as string. Returns table as table. Get all file paths in all directories of a folder in the project path. Returns a table with length -1 if any error occurred. Pattern can use `*` and `?` wildcards.

```
export function tebasproject::folderListFolders(path);
```
Takes 1 argument: table path as string. Returns table as table. Get all subfolder paths in a folder in the project path. Returns a table with length -1 if any error occurred

```
export function tebasproject::templateInstallLocal(path);
```
Takes 1 argument: table path as string. Returns table as bool. Install a template from a file in the project path

```
export function tebasproject::pluginInstallLocal(path);
```
Takes 1 argument: table path as string. Returns table as bool. Install a plugin from a file in the project path