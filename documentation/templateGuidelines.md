# Design guidelines for template and plugin creation

**Disclaimer**: This guidelines are the ones I followed designing the useful templates avaiable [in this repo](../templates)

## Help script
Templates should have a `thelp` global script and plugins `phelp`. These commands will explain all other avaialable commands, what they do, and how to use them.

## Arguments
It is preferent for scripts to have CLI arguments than interactive prompts, when possible. This makes it possible to automate many tasks.

## Workspace bounds
You should NEVER touch or change files that fall outside of the project/template working area. W/ and T/ prefixes for paths make it difficult, but using external commands it is possible to access any file.  
The template folder is for any template related files, and the project folder is for all stuff in relation to a project. Dont go outside

## Deletion
If the script is going to delete a user-made file or remove something, the user should know it is going to happen and be able to stop it in some form.  
Also, be aware than for example when creating a new project, if the user already has a project there, the files might be overriden and the user's progress lost. Check for files first!

## Keep script names descriptive
Make them short and descriptive
