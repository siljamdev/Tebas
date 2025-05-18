# Tebas template & plugin structure

Templates and plugins are [AshFiles](https://github.com/Dumbelfo08/AshLib).  
These template AshFiles have the following structure:  
```
Root
├──git
│  ├──defaultUse: <bool>
│  ├──gitignore: <string, gitignore content>
├──addReadme: <bool>
├──readme: <string, additional readme content>
├──name: <string>
├──version: <string, Tebas version on which it was created>
├──description: <string>
├──codeExtensions: <string, file extensions that count for code line count>
├──codeFilesFolderBlacklist: <string, folders that wont count for code line counts>
├──script
│  ├──[name]: <string, script content>
│  ├──[name2]: <string, script content>
├──global
│  ├──[name]: <string, script content>
│  ├──[name2]: <string, script content>
├──resources
│  ├──[name]: <string, content>
│  ├──[name2]: <string, content>
```

Here, codeExtensions and codeFilesFolderBlacklist are expected to separate extensions in different lines and codeExtensions to include the .  
In `script` there are normal scripts that can only be executed in projects, and in `global` there are global scripts that can be executed anywhere.

Plugin AshFiles have the same structure, minus some elements:  
```
Root
├──name: <string>
├──version: <string, Tebas version on which it was created>
├──description: <string>
├──scripts
│  ├──[name]: <string, script content>
│  ├──[name2]: <string, script content>
├──resources
│  ├──[name]: <string, content>
│  ├──[name2]: <string, content>
```

Here, all scripts are global.

Remember, the tree structure is a visual representation of a flat camp list. The separator is '.'.


## Using the creator utility
The creator utility is what is used when you use `tebas template create [...]` and `tebas plugin create [...]`.  
It creates a template based on a folder and its subfolders:
```
Root
├──default.gitignore
├──readme.md
├──description.txt
├──extensions.txt
├──blacklist.txt
├──scripts
│  ├──[name].tbscr
│  ├──[name2].tbscr
│  ├──global
│  │  ├──[name]: <string, script content>
│  │  ├──[name2]: <string, script content>
├──resources
│  ├──[name].*
│  ├──[name2].*
```

From the scripts folder, only `.tbscr` files will be loaded. From the resources folder, all files will be loaded, but they will lose their extension: *program.js => resources.program*  
In the `scripts` folder there are normal scripts, and in `scripts/global` there are global scripts. Plugin scripts are only in `scripts`.  
See the structure [here](../templates/c).

## Special named scripts
Some scripts will be called at special times.  
For templates:
|Script|Activation|
|---|---|
|install|On template installation. Allows cli args|
|uninstall|On template uninstallation|
|new|On project creation|
|rename|On project succesful renaming|
|info|On project, not template, info command|
|git|On git command|
|setbranch|On changing working git branch|
|stats|On stats command|
|add|On add command|
|commit|On commit command|
|push|On push command|
|pull|On pull command|

For plugins:
|Script|Activation|
|---|---|
|install|On plugin installation. Allows cli args|
|uninstall|On plugin uninstallation|
|info|On plugin info|