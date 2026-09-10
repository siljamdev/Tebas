# tebastemplate Import

The `tebastemplate` import is available in all template scripts, globals, utils and properties.  
It provides functionality related to that specific template.  

## Globals
```
export global path;
```
emplate path

```
export global name;
```
Template name

```
export global author;
```
Template author if possible, or an empty string

```
export global description;
```
Template description if possible, or an empty string


## Functions

### General template functions
```
export function tebastemplate::getPath();
```
Takes 0 arguments. Returns table as string. Get the template path

```
export function tebastemplate::getName();
```
Takes 0 arguments. Returns table as string. Get the template name

```
export function tebastemplate::getAuthor();
```
Takes 0 arguments. Returns table as string. Get the template author if possible, or an empty string

```
export function tebastemplate::getDescription();
```
Takes 0 arguments. Returns table as string. Get the template description if possible, or an empty string

```
export function tebastemplate::getAllScripts();
```
Takes 0 arguments. Returns table as table. Get all script names

```
export function tebastemplate::getAllGlobals();
```
Takes 0 arguments. Returns table as table. Get all global script names

```
export function tebastemplate::runGlobal(global, args);
```
Takes 2 arguments: table global as string table args as table. Returns table as bool. Run a global script. Returns true if the operation was successful

```
export function tebastemplate::hasPermission(key);
```
Takes 1 argument: table key as string. Returns table as bool. Check if the template has a permission

```
export function tebastemplate::cleanup();
```
Takes 0 arguments. Returns an empty table. Cleanup this template: cleans internal invalid or empty values

### Resource functions
All these functions access template level resources. When you reinstall(update) a template, they can be conserved if the designer wants to (KEEPRESOURCES file).  
```
export function tebastemplate::getResource(key);
```
Takes 1 argument: table key as string. Returns table as string. Get a template resource

```
export function tebastemplate::setResource(key, value);
```
Takes 2 arguments: table key as string table value as string. Returns an empty table. Set a template resource

```
export function tebastemplate::appendResource(key, value);
```
Takes 2 arguments: table key as string table value as string. Returns an empty table. Append to the end of a template resource

```
export function tebastemplate::getAllResourceKeys();
```
Takes 0 arguments. Returns table as table. Get all keys with a value in template resources

### Process functions
All processes are run in the directory that `tebastemplate::getPath()` gives. The `directory` argument is appended to that path.  
That way, a process called like `tebastemplate::runProcess("git", "repo", ["-h"]);` will be run in `{tebastemplate::getPath()}/repo`.  
Before running, the user must confirm to run the process. The user can skip this and allow always with the `skipProcessConfirmation` template permission.  
```
export function tebastemplate::runProcess(command, directory, arguments);
```
Takes 3 arguments: table command as string table directory as string table arguments as table. Returns table as string. Run a process in the template path, printing its output. Returns its exit code as a stdnum num. If any error occurred, an empty table will be returned

```
export function tebastemplate::runProcessInteractive(command, directory, arguments);
```
Takes 3 arguments: table command as string table directory as string table arguments as table. Returns table as string. Run a process interactively in the template path, printing its output. Returns its exit code as a stdnum num. If any error occurred, an empty table will be returned

```
export function tebastemplate::runProcessDetached(command, directory, arguments);
```
Takes 3 arguments: table command as string table directory as string table arguments as table. Returns table as bool. Run a process detached in the template path, not printing its output. Returns false if any error occurred

```
export function tebastemplate::runProcessWithOutput(command, directory, arguments);
```
Takes 3 arguments: table command as string table directory as string table arguments as table. Returns table as table. Run a process in the template path, and get its output as a stdlist list [stdout, stderr, exitcode]. Exitcode is a stdnum num. If any error occurred, an empty table will be returned

```
export function tebastemplate::runProcessSilent(command, directory, arguments);
```
Takes 3 arguments: table command as string table directory as string table arguments as table. Returns table as string. Run a process in the template path, not printing its output. Returns its exit code as a stdnum num. If any error occurred, an empty table will be returned

```
export function tebastemplate::open(target);
```
Takes 1 argument: table target as string. Returns table as bool. Open a url, folder or file in the template path. Returns false if any error occurred

### File functions
All files in the directory that `tebastemplate::getPath()` gives. The `path` argument is appended to that path.  
That way, checking if a file exists like `tebastemplate::fileExists("list.txt")` will check if a file exists in `{tebastemplate::getPath()}/list.txt`.  
Modifying these files and folders does not require user confirmation, because they are in an internal folder.  
```
export function tebastemplate::fileExists(path);
```
Takes 1 argument: table path as string. Returns table as bool. Returns true if a file exists in the template path

```
export function tebastemplate::fileRead(path);
```
Takes 1 argument: table path as string. Returns table as string. Reads whole text of a file in the template path

```
export function tebastemplate::fileReadLines(path);
```
Takes 1 argument: table path as string. Returns table as table. Returns lines of text of a file in the template path

```
export function tebastemplate::fileWrite(path, content);
```
Takes 2 arguments: table path as string table content as string. Returns table as bool. Writes whole content to a file in the template path. Returns true if the operation was successful

```
export function tebastemplate::fileWriteLines(path, content);
```
Takes 2 arguments: table path as string table content as table. Returns table as bool. Writes whole lines to a file in the template path. Returns true if the operation was successful. Each element of `content` represents a line

```
export function tebastemplate::fileAppend(path, content);
```
Takes 2 arguments: table path as string table content as string. Returns table as bool. Appends content to the end of a file in the template path. Returns true if the operation was successful

```
export function tebastemplate::fileAppendLines(path, content);
```
Takes 2 arguments: table path as string table content as table. Returns table as bool. Appends lines to the end of a file in the template path. Returns true if the operation was successful. Each element of `content` represents a line

```
export function tebastemplate::fileDelete(path);
```
Takes 1 argument: table path as string. Returns table as bool. Deletes a file in the template path. Returns true if the operation was successful

```
export function tebastemplate::fileMove(path, newPath);
```
Takes 2 arguments: table path as string table newPath as string. Returns table as bool. Moves a file to a new location in the template path. Returns true if the operation was successful

```
export function tebastemplate::fileCopy(path, copyPath);
```
Takes 2 arguments: table path as string table copyPath as string. Returns table as bool. Copies a file to another location in the template path. Returns true if the operation was successful

```
export function tebastemplate::fileSize(path);
```
Takes 1 argument: table path as string. Returns table as string. Get the size in bytes as a stdnum num of a file in the template path. Returns an empty string if any error occurred

```
export function tebastemplate::folderExists(path);
```
Takes 1 argument: table path as string. Returns table as bool. Returns true if a folder exists in the template path

```
export function tebastemplate::folderCreate(path);
```
Takes 1 argument: table path as string. Returns table as bool. Create a folder in the template path. Returns true if the operation was successful

```
export function tebastemplate::folderDelete(path);
```
Takes 1 argument: table path as string. Returns table as bool. Delete a folder in the template path. Returns true if the operation was successful

```
export function tebastemplate::folderMove(path, newPath);
```
Takes 2 arguments: table path as string table newPath as string. Returns table as bool. Move a folder to a new location in the template path. Returns true if the operation was successful

```
export function tebastemplate::folderListFiles(path, pattern);
```
Takes 2 arguments: table path as string table pattern as string. Returns table as table. Get all file paths in the top directory of a folder in the template path. Returns a table with length -1 if any error occurred. Pattern can use `*` and `?` wildcards.

```
export function tebastemplate::folderListChildFiles(path, pattern);
```
Takes 2 arguments: table path as string table pattern as string. Returns table as table. Get all file paths in all directories of a folder in the template path. Returns a table with length -1 if any error occurred. Pattern can use `*` and `?` wildcards.

```
export function tebastemplate::folderListFolders(path);
```
Takes 1 argument: table path as string. Returns table as table. Get all subfolder paths in a folder in the template path. Returns a table with length -1 if any error occurred