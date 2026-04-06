# tebas Import

The `tebas` import is available in absolutely all scripts, globals, utils and properties.  
It provides base functionality, and also an API to talk with the Tebas app.  
The functions it provides are:  

### Output & input functions
```
export function tebas::print(t){ EXTERN; } //Print to Standard Output
export function tebas::printColor(t, color){ EXTERN; } //Print to Standard Output with color(hexadecimal)
export function tebas::error(t){ EXTERN; } //Print to Standard Error
export function tebas::input(prompt){ EXTERN; } //Read from Standard Input
```
These functions have no additional complexity.  

### Project functions
```
export function tebas::getAllProjectsPaths(){ EXTERN; } //Get the paths to all project directories
```
Returns a table with the path to all the project directories (not the .tebas file).  
```
export function tebas::projectExists(directory){ EXTERN; } //Check if project exists in a directory
export function tebas::getProjectTemplateName(directory){ EXTERN; } //Get the name of the template used in a project, based on its directory
export function tebas::getProjectProperty(directory, key){ EXTERN; } //Get a property of a project, based on its directory
```
These functions have no additional complexity.  
```
export function tebas::projectsCleanup(){ EXTERN; } //Cleanup projects
```
Removes projects that dont exist from the internal list.  

### Template functions
```
export function tebas::getAllTemplateNames(){ EXTERN; } //Get the names of all installed templates
export function tebas::templateInstalled(name){ EXTERN; } //Check if template is installed
```
These functions have no additional complexity.  
```
export function tebas::templateRunGlobal(name, global, args){ EXTERN; } //Attempt to run a global script of a template
```
Returns true if it was possible to run.  
```
export function tebas::templatesCleanup(){ EXTERN; } //Cleanup templates
```
Removes internal folders of templates that are not installed.  

### Plugin functions
```
export function tebas::getAllPluginNames(){ EXTERN; } //Get the names of all installed plugins
export function tebas::pluginInstalled(name){ EXTERN; } //Check if plugin is installed
export function tebas::pluginRunGlobal(name, global, args){ EXTERN; } //Attempt to run a global script of a plugin
export function tebas::pluginsCleanup(){ EXTERN; } //Cleanup plugins
```
These functions do exactly the same as the template ones, but for plugins.  

### Shared resources functions
Shared resources are app-wise resources to avoid duplicate values and speed up installations.  
Check the [design guidelines](../designGuidelines.md) for a list of standard values.  
```
export function tebas::getShared(key){ EXTERN; } //Get shared resource
export function tebas::setShared(key, value){ EXTERN; } //Set shared resource
```
These functions have no additional complexity.  
```
export function tebas::appendShared(key, value){ EXTERN; } //Append to a shared resource
```
This function appends to the end of the value.  
```
export function tebas::getAllSharedKeys(){ EXTERN; } //Get all keys with a value in shared resources
```
These functions have no additional complexity.  
```
export function tebas::sharedCleanup(){ EXTERN; } //Cleanup shared resources
```
This function deletes internal invalid or empty values.  

### Path functions
These functions provide base functionality for some path operations.  
In general, it is safe to always use `/` as separator.  
```
export function tebas::getPathExtension(path){ EXTERN; } //Get extension of a file path
export function tebas::getPathFilename(path){ EXTERN; } //Get file name with extension of a file path
export function tebas::getPathFilenameNoExtension(path){ EXTERN; } //Get file name without extension of a file path
export function tebas::getPathDirectory(path){ EXTERN; } //Get parent directory of a path
export function tebas::getPathSeparator(){ EXTERN; } //Get default OS separator of paths
```
These functions have no additional complexity.  

### Other functions
```
export function tebas::getAllPermissionKeys(){ EXTERN; } //Get all valid permission keys
export function tebas::getAllConfigKeys(){ EXTERN; } //Get all valid config keys
export function tebas::getConfigValue(key){ EXTERN; } //Get value for a config key
export function tebas::getVersion(){ EXTERN; } //Get Tebas version
```
These functions have no additional complexity.  
```
export function tebas::cleanupAll(){ EXTERN; } //Cleanup everything in Tebas
```
This function does the same as running `tebas cleanup`.  
