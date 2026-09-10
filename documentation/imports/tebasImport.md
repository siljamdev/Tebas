# tebas Import

The `tebas` import is available in absolutely all scripts, globals, utils and properties.  
It provides base functionality, and also an API to talk with the Tebas app.  

## Globals
```
export global version;
```
Tebas version


## Functions

### Output & input functions
```
export function tebas::print(t);
```
Takes 1 argument: table t as string. Returns an empty table. Print to Standard Output

```
export function tebas::printFormat(t);
```
Takes 1 argument: table t as string. Returns an empty table. Print to Standard Output with format (AshFile FormatString)

```
export function tebas::error(t);
```
Takes 1 argument: table t as string. Returns an empty table. Print to Standard Error

```
export function tebas::input(prompt);
```
Takes 1 argument: table prompt as string. Returns table as string. Read from Standard Input

### Project functions
```
export function tebas::getAllProjectsPaths();
```
Takes 0 arguments. Returns table as table. Get the directory paths to all projects

```
export function tebas::projectExists(directory);
```
Takes 1 argument: table directory as string. Returns table as bool. Check if project exists in a directory

```
export function tebas::getProjectName(directory);
```
Takes 1 argument: table directory as string. Returns table as string. Get the name of the project in a path. Returns an empty string if no project exists in that directory

```
export function tebas::getProjectTemplateName(directory);
```
Takes 1 argument: table directory as string. Returns table as string. Get the name of the template used in a project, based on its directory. Returns an empty string if no project exists in that directory

```
export function tebas::getProjectProperty(directory, key);
```
Takes 2 arguments: table directory as string table key as string. Returns table as table. Get a property of a project, based on its directory. Returns an empty table if no project exists in that directory

```
export function tebas::projectsCleanup();
```
Takes 0 arguments. Returns an empty table. Cleanup projects

### Template functions
```
export function tebas::getAllTemplateNames();
```
Takes 0 arguments. Returns table as table. Get the names of all installed templates

```
export function tebas::templateInstalled(name);
```
Takes 1 argument: table name as string. Returns table as bool. Check if a template is installed

```
export function tebas::templateRunGlobal(name, global, args);
```
Takes 3 arguments: table name as string table global as string table args as table. Returns table as bool. Attempt to run a global script of a template. Returns true if the operation was successful

```
export function tebas::templatesCleanup();
```
Takes 0 arguments. Returns an empty table. Cleanup templates

### Plugin functions
```
export function tebas::getAllPluginNames();
```
Takes 0 arguments. Returns table as table. Get the names of all installed plugins

```
export function tebas::pluginInstalled(name);
```
Takes 1 argument: table name as string. Returns table as bool. Check if a plugin is installed

```
export function tebas::pluginRunGlobal(name, global, args);
```
Takes 3 arguments: table name as string table global as string table args as table. Returns table as bool. Attempt to run a global script of a plugin. Returns true if the operation was successful

```
export function tebas::pluginsCleanup();
```
Takes 0 arguments. Returns an empty table. Cleanup plugins

### Shared resources functions
Shared resources are app-wise resources to avoid duplicate values and speed up installations. 
Check the [design guidelines](../designGuidelines.md) for a list of standard values.  
```
export function tebas::getShared(key);
```
Takes 1 argument: table key as string. Returns table as string. Get shared resource

```
export function tebas::setShared(key, value);
```
Takes 2 arguments: table key as string table value as string. Returns an empty table. Set shared resource

```
export function tebas::appendShared(key, value);
```
Takes 2 arguments: table key as string table value as string. Returns an empty table. Append to the end of a shared resource

```
export function tebas::getAllSharedKeys();
```
Takes 0 arguments. Returns table as table. Get all keys with a value in shared resources

```
export function tebas::sharedCleanup();
```
Takes 0 arguments. Returns an empty table. Cleanup shared resources: cleans internal invalid or empty values

### Other functions
```
export function tebas::getAllPermissionKeys();
```
Takes 0 arguments. Returns table as table. Get all valid permission keys

```
export function tebas::getAllConfigKeys();
```
Takes 0 arguments. Returns table as table. Get all valid config keys

```
export function tebas::getConfigValue(key);
```
Takes 1 argument: table key as string. Returns table as string. Get value for a config key

```
export function tebas::getVersion();
```
Takes 0 arguments. Returns table as string. Get Tebas version

```
export function tebas::cleanupAll();
```
Takes 0 arguments. Returns an empty table. Cleanup everything in Tebas. This function does the same as running `tebas cleanup`