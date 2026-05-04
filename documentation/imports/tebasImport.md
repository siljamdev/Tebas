# tebas Import

The `tebas` import is available in absolutely all scripts, globals, utils and properties.  
It provides base functionality, and also an API to talk with the Tebas app.  
The functions it provides are:  

### Output & input functions
```
export function tebas::print(t)
```
Returns empty table, takes as arguments: table as string. Print to Standard Output
```
export function tebas::printFormat(t)
```
Returns empty table, takes as arguments: table as string. Print to Standard Output a [FormatString](https://github.com/siljamdev/AshLib/blob/main/documentation/formatstrings.md) with string formatting
```
export function tebas::error(t)
```
Returns empty table, takes as arguments: table as string. Print to Standard Error
```
export function tebas::input(prompt)
```
Returns string as table, takes as arguments: table as string. Read from Standard Input

### Project functions
```
export function tebas::getAllProjectsPaths()
```
Returns table, takes no arguments. Get the directory paths to all projects (not the `.tebas` file)
```
export function tebas::projectExists(directory)
```
Returns bool as table, takes as arguments: table as string. Check if project exists in a directory
```
export function tebas::getProjectTemplateName(directory)
```
Returns string as table, takes as arguments: table as string. Get the name of the template used in a project, based on its directory. Returns an empty string if no project exists in that directory
```
export function tebas::getProjectProperty(directory, key)
```
Returns table, takes as arguments: table as string, table as string. Get a property of a project, based on its directory. Returns an empty table if no project exists in that directory
```
export function tebas::projectsCleanup()
```
Returns empty table, takes no arguments. Removes projects that dont exist from the internal list

### Template functions
```
export function tebas::getAllTemplateNames()
```
Returns table, takes no arguments. Get the names of all installed templates
```
export function tebas::templateInstalled(name)
```
Returns bool as table, takes as arguments: table as string. Check if a template is installed
```
export function tebas::templateRunGlobal(name, global, args)
```
Returns bool as table, takes as arguments: table as string, table as string, table. Attempt to run a global script of a template. Returns true if the operation was successful
```
export function tebas::templatesCleanup()
```
Returns empty table, takes no arguments. Removes internal folders of templates that are not installed

### Plugin functions
```
export function tebas::getAllPluginNames()
```
Returns table, takes no arguments. Get the names of all installed plugins
```
export function tebas::pluginInstalled(name)
```
Returns bool as table, takes as arguments: table as string. Check if a plugin is installed
```
export function tebas::pluginRunGlobal(name, global, args)
```
Returns bool as table, takes as arguments: table as string, table as string, table. Attempt to run a global script of a plugin. Returns true if the operation was successful
```
export function tebas::pluginsCleanup()
```
Returns empty table, takes no arguments. Removes internal folders of plugins that are not installed

### Shared resources functions
Shared resources are app-wise resources to avoid duplicate values and speed up installations. 
Check the [design guidelines](../designGuidelines.md) for a list of standard values.  
```
export function tebas::getShared(key)
```
Returns string as table, takes as arguments: table as string. Get shared resource
```
export function tebas::setShared(key, value)
```
Returns empty table, takes as arguments: table as string, table as string. Set shared resource
```
export function tebas::appendShared(key, value)
```
Returns empty table, takes as arguments: table as string, table as string. Append to a shared resource
```
export function tebas::getAllSharedKeys()
```
Returns table, takes no arguments. Get all keys with a value in shared resources
```
export function tebas::sharedCleanup()
```
Returns empty table, takes no arguments. Cleanup shared resources: cleans internal invalid or empty values

### Other functions
```
export function tebas::getAllPermissionKeys()
```
Returns table, takes no arguments. Get all valid permission keys
```
export function tebas::getAllConfigKeys()
```
Returns table, takes no arguments. Get all valid config keys
```
export function tebas::getConfigValue(key)
```
Returns string as table, takes as arguments: table as string. Get value for a config key
```
export function tebas::getVersion()
```
Returns string as table, takes no arguments. Get Tebas version
```
export function tebas::cleanupAll()
```
Returns empty table, takes no arguments. Cleanup everything in Tebas. This function does the same as running `tebas cleanup`
