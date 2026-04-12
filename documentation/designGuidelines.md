# Design guidelines for template & plugin creation

In this document, the recommended design guidelines for creating Tebas templates and plugins will be exposed.  
**Disclaimer**: These guidelines are the ones I follow designing the templates and plugins available in the [Official Tebas Registry](https://github.com/siljamdev/Tebas-Registry).  

## Workspace bounds
You should NEVER touch or change files that fall outside of the project/template/plugin working area.  
The security of Tebas itself prohibits it, but using external commands it is possible to access any file. Please refrain from doing this.  

## Deletion
If a script is going to delete a user-made file or do any other destructive operation, the user should know it is going to happen and be able to stop it.  
Also, be aware that for example on init, if the user already has files there, they might be overridden and the user's progress lost. Check for existing files first!  

## Arguments
It is preferred for scripts/globals to have CLI arguments than interactive prompts. This makes it possible to automate many tasks.  
Also, argument length enforcing is encouraged. If `args` has an incorrect length, the scriot should display an error to the user(using `tebas::error`) and then `exit;`.  

## Script names
Keep script/global names **short** and **descriptive**. If two words are necessary, a *verb-noun* name is preferred, such as: `set-remote` or `list-tags`.  

## Shared resources
If your template/plugin needs some data, like the path to an executable, try getting it from shared resources. Here is a list of standards:  
|Shared resources key|Value|
|---|---|
|`author`|Author name|
|`paths.<name>`|Path to an executable. For example, `paths.git` or `paths.browser`|

## Help global
Templates and plugins should have a `help` global script. The [tbtem](https://github.com/siljamdev/Tebas-Registry/tree/main/templates/tbtem) and [tbplg](https://github.com/siljamdev/Tebas-Registry/tree/main/templates/tbplg) templates automatically add it to the globals folder on init.  
This global will explain all other available scripts and globals, what they do, and how to use them.  
Also, it will explain how the template/plugin works and what it does.  

## Config global
If some configuration needs to occur at the template/plugin level, templates should have a `tconfig` global script (called like this because config would be blocked by `tebas config` command) and plugins should have a `config` global script.  
It should take either 1 or 2 arguments: `<key> <value>`:  
|key|value|Operation|
|---|---|---|
|list||Display a list of valid keys and what they do|
|see||Display the current config values|
|key|value|Set the config|

For keys that represent booleans, accept as value true, t, false and f.  
The [tbtem](https://github.com/siljamdev/Tebas-Registry/tree/main/templates/tbtem) template has a script (`add-tconfig`) that adds a tconfig global ready to be used.  
The [tbplg](https://github.com/siljamdev/Tebas-Registry/tree/main/templates/tbplg) template has a script (`add-config`) that adds a config global ready to be used.  

## Compatibility with default plugins
When designing a template, keep in mind compatibility with plugins in the [Official Tebas Registry](https://github.com/siljamdev/Tebas-Registry).  
Compatibility makes the tool much easier to be used smoothly.  
Here is a list of things to implement if appropriate:

### [git](https://github.com/siljamdev/Tebas-Registry/tree/main/plugins/git) plugin
Properties to implement:
- `git.defaultBranch`: Table with length 1. The default git branch to use. Default is configured in the git plugin itself
- `git.addTarget`: Table with length 1. What to target on `git add <target>`. Default is `-A`
- `git.defaultGitignore`: Table with **lines** to add to the initial `.gitignore` file. Default is empty

### [stats](https://github.com/siljamdev/Tebas-Registry/tree/main/plugins/stats) plugin
Properties to implement:
- `stats.codeLines`: Lines of code in the project as a [stdnum](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdNum.html) num. The template is responsible for counting them

Scripts to implement:
- `stats`: Script that will be called when the plugin script `stats` is called

### [unirun](https://github.com/siljamdev/Tebas-Registry/tree/main/plugins/unirun) plugin
Scripts to implement:
- `run [file]`: Script that runs the project/file
- `debug [file]`: Script that debugs the project/file
- `build [file]`: Script that builds the project/file
