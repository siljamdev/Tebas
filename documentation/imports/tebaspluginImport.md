# tebasplugin Import

The `tebasplugin` import is available in all plugin scripts, globals and utils.  
It provides functionality related to that specific plugin.  

## Globals
```
export global path;
```
Plugin path

```
export global name;
```
Plugin name

```
export global author;
```
Plugin author if possible, or an empty string

```
export global description;
```
Plugin description if possible, or an empty string


## Functions

### General plugin functions
```
export function tebasplugin::getPath();
```
Takes 0 arguments. Returns table as string. Get the plugin path

```
export function tebasplugin::getName();
```
Takes 0 arguments. Returns table as string. Get the plugin name

```
export function tebasplugin::getAuthor();
```
Takes 0 arguments. Returns table as string. Get the plugin author if possible, or an empty string

```
export function tebasplugin::getDescription();
```
Takes 0 arguments. Returns table as string. Get the plugin description if possible, or an empty string

```
export function tebasplugin::getAllScripts();
```
Takes 0 arguments. Returns table as table. Get all script names

```
export function tebasplugin::getAllGlobals();
```
Takes 0 arguments. Returns table as table. Get all global script names

```
export function tebasplugin::runGlobal(global, args);
```
Takes 2 arguments: table global as string table args as table. Returns table as bool. Run a global script. Returns true if the operation was successful

```
export function tebasplugin::hasPermission(key);
```
Takes 1 argument: table key as string. Returns table as bool. Check if the plugin has a permission

```
export function tebasplugin::cleanup();
```
Takes 0 arguments. Returns an empty table. Cleanup this plugin: cleans internal invalid or empty values

### Resource functions
All these functions access plugin level resources. When you reinstall(update) a plugin, they can be conserved if the designer wants to (KEEPRESOURCES file).  
```
export function tebasplugin::getResource(key);
```
Takes 1 argument: table key as string. Returns table as string. Get a plugin resource

```
export function tebasplugin::setResource(key, value);
```
Takes 2 arguments: table key as string table value as string. Returns an empty table. Set a plugin resource

```
export function tebasplugin::appendResource(key, value);
```
Takes 2 arguments: table key as string table value as string. Returns an empty table. Append to the end of a plugin resource

```
export function tebasplugin::getAllResourceKeys();
```
Takes 0 arguments. Returns table as table. Get all keys with a value in plugin resources

### Process functions
All processes are run in the directory that `tebasplugin::getPath()` gives. The `directory` argument is appended to that path.  
That way, a process called like `tebasplugin::runProcess("git", "repo", ["-h"]);` will be run in `{tebasplugin::getPath()}/repo`.  
Before running, the user must confirm to run the process. The user can skip this and allow always with the `skipProcessConfirmation` plugin permission.  
```
export function tebasplugin::runProcess(command, directory, arguments);
```
Takes 3 arguments: table command as string table directory as string table arguments as table. Returns table as string. Run a process in the plugin path, printing its output. Returns its exit code as a stdnum num. If any error occurred, an empty table will be returned

```
export function tebasplugin::runProcessInteractive(command, directory, arguments);
```
Takes 3 arguments: table command as string table directory as string table arguments as table. Returns table as string. Run a process interactively in the plugin path, printing its output. Returns its exit code as a stdnum num. If any error occurred, an empty table will be returned

```
export function tebasplugin::runProcessDetached(command, directory, arguments);
```
Takes 3 arguments: table command as string table directory as string table arguments as table. Returns table as bool. Run a process detached in the plugin path, not printing its output. Returns false if any error occurred

```
export function tebasplugin::runProcessWithOutput(command, directory, arguments);
```
Takes 3 arguments: table command as string table directory as string table arguments as table. Returns table as table. Run a process in the plugin path, and get its output as a stdlist list [stdout, stderr, exitcode]. Exitcode is a stdnum num. If any error occurred, an empty table will be returned

```
export function tebasplugin::runProcessSilent(command, directory, arguments);
```
Takes 3 arguments: table command as string table directory as string table arguments as table. Returns table as string. Run a process in the plugin path, not printing its output. Returns its exit code as a stdnum num. If any error occurred, an empty table will be returned

```
export function tebasplugin::open(target);
```
Takes 1 argument: table target as string. Returns table as bool. Open a url, folder or file in the plugin path. Returns false if any error occurred

### File functions
All files in the directory that `tebasplugin::getPath()` gives. The `path` argument is appended to that path.  
That way, checking if a file exists like `tebasplugin::fileExists("list.txt")` will check if a file exists in `{tebasplugin::getPath()}/list.txt`.  
Modifying these files and folders does not require user confirmation, because they are in an internal folder.  
```
export function tebasplugin::fileExists(path);
```
Takes 1 argument: table path as string. Returns table as bool. Returns true if a file exists in the plugin path

```
export function tebasplugin::fileRead(path);
```
Takes 1 argument: table path as string. Returns table as string. Reads whole text of a file in the plugin path

```
export function tebasplugin::fileReadLines(path);
```
Takes 1 argument: table path as string. Returns table as table. Returns lines of text of a file in the plugin path

```
export function tebasplugin::fileWrite(path, content);
```
Takes 2 arguments: table path as string table content as string. Returns table as bool. Writes whole content to a file in the plugin path. Returns true if the operation was successful

```
export function tebasplugin::fileWriteLines(path, content);
```
Takes 2 arguments: table path as string table content as table. Returns table as bool. Writes whole lines to a file in the plugin path. Returns true if the operation was successful. Each element of `content` represents a line

```
export function tebasplugin::fileAppend(path, content);
```
Takes 2 arguments: table path as string table content as string. Returns table as bool. Appends content to the end of a file in the plugin path. Returns true if the operation was successful

```
export function tebasplugin::fileAppendLines(path, content);
```
Takes 2 arguments: table path as string table content as table. Returns table as bool. Appends lines to the end of a file in the plugin path. Returns true if the operation was successful. Each element of `content` represents a line

```
export function tebasplugin::fileDelete(path);
```
Takes 1 argument: table path as string. Returns table as bool. Deletes a file in the plugin path. Returns true if the operation was successful

```
export function tebasplugin::fileMove(path, newPath);
```
Takes 2 arguments: table path as string table newPath as string. Returns table as bool. Moves a file to a new location in the plugin path. Returns true if the operation was successful

```
export function tebasplugin::fileCopy(path, copyPath);
```
Takes 2 arguments: table path as string table copyPath as string. Returns table as bool. Copies a file to another location in the plugin path. Returns true if the operation was successful

```
export function tebasplugin::fileSize(path);
```
Takes 1 argument: table path as string. Returns table as string. Get the size in bytes as a stdnum num of a file in the plugin path. Returns an empty string if any error occurred

```
export function tebasplugin::folderExists(path);
```
Takes 1 argument: table path as string. Returns table as bool. Returns true if a folder exists in the plugin path

```
export function tebasplugin::folderCreate(path);
```
Takes 1 argument: table path as string. Returns table as bool. Create a folder in the plugin path. Returns true if the operation was successful

```
export function tebasplugin::folderDelete(path);
```
Takes 1 argument: table path as string. Returns table as bool. Delete a folder in the plugin path. Returns true if the operation was successful

```
export function tebasplugin::folderMove(path, newPath);
```
Takes 2 arguments: table path as string table newPath as string. Returns table as bool. Move a folder to a new location in the plugin path. Returns true if the operation was successful

```
export function tebasplugin::folderListFiles(path, pattern);
```
Takes 2 arguments: table path as string table pattern as string. Returns table as table. Get all file paths in the top directory of a folder in the plugin path. Returns a table with length -1 if any error occurred. Pattern can use `*` and `?` wildcards.

```
export function tebasplugin::folderListChildFiles(path, pattern);
```
Takes 2 arguments: table path as string table pattern as string. Returns table as table. Get all file paths in all directories of a folder in the plugin path. Returns a table with length -1 if any error occurred. Pattern can use `*` and `?` wildcards.

```
export function tebasplugin::folderListFolders(path);
```
Takes 1 argument: table path as string. Returns table as table. Get all subfolder paths in a folder in the plugin path. Returns a table with length -1 if any error occurred