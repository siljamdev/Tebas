# Template creation tutorial

In this tutorial, we will create a template called `html`, for web-based projects. You can find the full template [here](https://github.com/siljamdev/Tebas-Registry/tree/main/templates/html).  

## 1. Establishing our goal
We want to create a template that will allow us to create and "run" web projects based on html files.  
To open them, we will use a browser executable.  

## 2. Setting up the environment
The steps to follow are:  
- Installing Tebas if it is not installed
- Running `tebas template install tbtem` to install the [tbtem](https://github.com/siljamdev/Tebas-Registry/tree/main/templates/tbtem) template from the [registry](https://github.com/siljamdev/Tebas-Registry)
- Creating a folder, in this case called `html`
- Inside that folder, running `tebas init html`

Now, we will have our folder populated with a bunch of files. Let's modify some of them.  
In `name.txt`, we should write `html`, because its the name of the template.  
In `author.txt`, we should write our author name (in my case siljam).  
In `description.txt`, we should write a meaningful description for our template. I will write `Template that helps you create web projects with html files.`.  

Now, we have succesfully set everything up and we are ready to start writing code.

## 3. Install script
As we established earlier, we will use a browser to open the html files.  
To run the browser, we will need its path. We will get it in the install script, and save in the template resources with the key `browserPath`.  
We go to the `globals` folder and create a new file, `install.tbs`.  
Lets start with its code:
```
import stdlib;
import tebas;
import tebastemplate;

//User input
tab m = input("Enter browser executable path: ").removeQuotes();
tebastemplate::setResource("browserPath", m);
```

Here, we ask the user for the browser executable path and save it in the template resources. But we can do better!
We can test if a process is actually runnable:
```
function testProcess(path){
	tab num = tebastemplate::runProcessWithExitCode(path, "", ["-h"]);
	
	if num.length == 0{
		error("'" + path + "' does not seem to be a runnable process");
		return 0;
	}
	
	return 1;
}
```

And then update the code correspondingly:
```
//User input
tab m = input("Enter browser executable path: ").removeQuotes();
tebastemplate::setResource("browserPath", m);
testProcess(m);
```

This is pretty good, but the user has to input the valid path every time they install the template. We can fix this by telling tebas to keep the previous template resources and then checking those.  
First, run `tebas add-kr`. This will add an empty `KEEPRESOURCES` files, that tells tebas to keep the previous resources when installing.  
We have to update our install script:
```
import stdlib;
import tebas;
import tebastemplate;

//Previous template resources
tab pr = tebastemplate::getResource("browserPath");
if pr > 0{
	print("Browser executable path gotten from previous template resources");
	testProcess(pr);
	exit;
}

//User input
tab m = input("Enter browser executable path: ").removeQuotes();
tebastemplate::setResource("browserPath", m);
testProcess(m);
```

If the resource exists(`length > 0`), we dont need to ask the user, and exit prematurely. We also log what we did so the user knows what happened.  

If we consult the [desing guidelines](./designGuidelines.md), we can see it tells us to try and get data from the shared resources, and there is a standard for executable paths. So we can try getting the browser path from there:
```
//Shared resources
tab sr = tebas::getShared("paths.browser");
if sr > 0{
	tebastemplate::setResource("browserPath", sr);
	print("Browser executable path gotten from shared resources");
	testProcess(sr);
	exit;
}
```

And if we successfully get it, it should also be saved there. This is the full script:
```
import stdlib;
import tebas;
import tebastemplate;

//Previous template resources
tab pr = tebastemplate::getResource("browserPath");
if pr > 0{
	print("Browser executable path gotten from previous template resources");
	testProcess(pr);
	exit;
}

//Shared resources
tab sr = tebas::getShared("paths.browser");
if sr > 0{
	tebastemplate::setResource("browserPath", sr);
	print("Browser executable path gotten from shared resources");
	testProcess(sr);
	exit;
}

//User input
tab m = input("Enter browser executable path: ").removeQuotes();
tebastemplate::setResource("browserPath", m);
if testProcess(m){
	tebas::setShared("paths.browser", m); //Only save if it was succesfully tested
}

function testProcess(path){
	tab num = tebastemplate::runProcessWithExitCode(path, "", ["-h"]);
	
	if num.length == 0{
		error("'" + path + "' does not seem to be a runnable process");
		return 0;
	}
	
	return 1;
}
```

With this, we have a working install script.  

## 4. Config script
But the user might want to change this path without having to reinstall the template(even though its impossible). Thats why we will create a `tconfig` global script as defined by the [desing guidelines](./designGuidelines.md).  
We can run `tebas add-tconfig` to create this script. After it is created, we just have to fill in the gaps(keys and description):
```
import stdlib;
import tebas;
import tebastemplate;

tab validKeys = ["browserPath"]; //List of valid configurable keys
tab trueFalseKeys = []; //Add here keys that can only take true/false values
tab keysDescription = ["Path to a browser executable"]; //In the same order as validKeys

if args.length == 1{
	if args.0 == "list"{
		print("Here is a list of valid keys and their description:");
		tab i = 0;
		while(i < validKeys){
			print("  " + validKeys[i] + ": " + keysDescription[i]);
			i += 1;
		}
		exit;
	}else if args.0 == "see"{
		print("Current config:");
		foreach key @ validKeys{
			print("  " + key + ": " + tebastemplate::getResource(key));
		}
		exit;
	}
}

if args.length != 2{
	error("Expected 2 arguments: 'key' 'value'");
	exit;
}

if !(args.0 @ validKeys){
	error("Invalid key");
	exit;
}

if args.0 @ trueFalseKeys{
	args.1 = args.1.lower();
	if !(args.1 == "true" | args.1 == "t" | args.1 == "false" | args.1 == "f"){
		error("Invalid value: Use true/false");
		exit;
	}
	
	if args.1 == "t"{
		args.1 = "true";
	}else if args.1 == "f"{
		args.1 = "false";
	}
}

tebastemplate::setResource(args.0, args.1);
```

This config is pretty solid and enough for our template. We could even delete the true/false values part, but we will not in case we need to expand the template in the future.  

## 5. Init script
Now lets create the script that will run when we do `tebas init html`. We want to create all the necessary files so the user can start coding directly, so we will add some files to the project: `index.html`, `style.css`, `script.js`.  
The contents of these file will be a blank and basic web page ready to be modified.  
We need to add this empty content to the template resources. We go to the `resources` folder and create a `index.html` file containg:
```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
	
	<link rel="stylesheet" href="style.css">
	<script src="script.js"></script>
	
    <title>Hello World</title>
</head>
<body>
	Hello World!
</body>
</html>
```

We also create a css file, `style.css`, containing:
```css
body{
	font-family: Arial, sans-serif;
	margin: 20px;
	padding: 5px;
	background-color: black;
	color: white;
}
```

Finally, we create a JavaScript file, `script.js`, containing:
```js
console.log("Hello World!");
```

These 3 resources will be available to as with the keys `index`, `style` and `script`.  
Lets create the init script. We go into the `scripts` folder and create the file `init.tbs`.  
In it, we will write:
```
import stdlib;
import tebas;
import tebasproject;
import tebastemplate;

createFile("index.html", tebastemplate::getResource("index"));
createFile("style.css", tebastemplate::getResource("style"));
createFile("script.js", tebastemplate::getResource("script"));

function createFile(path, content){
	if(!tebasproject::fileExists(path)){
		tebasproject::fileWrite(path, content);
	}
}
```

We are helped by the `createFile` function, that does not write to a file if it already exists. This follows the [desing guidelines](./designGuidelines.md).  
But what if the user already has a html project there? We should check for the existance of html files before creating everything new:

```
tab files = tebasproject::folderListFiles("", "*.html");
if files.length > 0{
	exit;
}
```

The final script looks like this:
```
import stdlib;
import tebas;
import tebasproject;
import tebastemplate;

tab files = tebasproject::folderListFiles("", "*.html");
if files.length > 0{
	exit;
}

createFile("index.html", tebastemplate::getResource("index"));
createFile("style.css", tebastemplate::getResource("style"));
createFile("script.js", tebastemplate::getResource("script"));

function createFile(path, content){
	if(!tebasproject::fileExists(path)){
		tebasproject::fileWrite(path, content);
	}
}
```

## 6. Run script
To conform with the standard of the [unirun](https://github.com/siljamdev/Tebas-Registry/tree/main/plugins/unirun) we will add a run plugin.  
It will either take an argument to the file we want to run or no arguments.
We will create, inside the `scripts` folder, a file called `run.tbs`, containg:
```
import stdlib;
import tebas;
import tebastemplate;
import tebasproject;

if args.length == 1{
	tebasproject::runProcess(tebastemplate::getResource("browserPath"), "", args.0);
}else if args.length == 0{
	tab files = tebasproject::folderListFiles("", "*.html");
	if files > 0{
		tab path = tebasproject::getPath() + "/" + files.0;
		tebasproject::runProcess(tebastemplate::getResource("browserPath"), "", path^);
	}else{
		error("No html files found for running");
	}
}else{
	error("Expected 0 arguments");
}
```

This script will run a file if passed, or search for it instead.  

## 7. Help script
When we did the init, a global script was added in `globals/help.tbs`, as the [desing guidelines](./designGuidelines.md) stipulates.  
Lets modify it so it shows everything correctly:
```
import stdlib;
import tebas;
import tebastemplate;

if args.length != 0{
	error("Expected 0 arguments");
	exit;
}

print(getName() + " template help");
print("");
print("Scripts:");
print("  run [file]    Runs the web page in the browser");
print("Globals:");
print("  tconfig <key> <value>    Change the config. Use list as key to get all valid keys, and see to see the current config");
print("  help    This help menu");
print("");
print("This template helps you with local web page development (html files)");
```

## 8. Properties
The [desing guidelines](./designGuidelines.md) recommend implementing properties for common plugins. We will implement those for the [stats](https://github.com/siljamdev/Tebas-Registry/tree/main/plugins/stats) plugin.  
We need to modify the `properties.tbs` file that was created on init. The key we need to implement is `stats.codeLines`:
```
import stdlib;
import stdnum;
import tebas;
import tebasproject;
import tebastemplate;

getProperty(""); //Make sure the compiler does not remove the function

export function getProperty(key){
	if key == "stats.codeLines"{
		return "0";
	}
}
```

We need to return a [stdnum](https://siljamdev.github.io/TableScript/api/TabScript.StandardLibraries.StdNum.html) num. We will count the lines of html, css, js and ts files:
```
export function getProperty(key){
	if key == "stats.codeLines"{
		tab total = "0";
		
		tab files = tebasproject::folderListChildFiles("", "*.html");
		files += tebasproject::folderListChildFiles("", "*.css");
		files += tebasproject::folderListChildFiles("", "*.js");
		files += tebasproject::folderListChildFiles("", "*.ts");
		
		foreach file @ files{
			total = sum(total, max("0", tebasproject::fileReadLines(file).length.toNum()));
		}
		return total;
	}
}
```

This way, everything works.

## 9. Building
Lets ensure everything works well. Running `tebas build` should tell you if any errors happened.  
If the build is succesfull, congratulations! You have succesfully created a template. Now, try creating whatever you want.  
This html template can be found in the [Official Tebas Registry](https://github.com/siljamdev/Tebas-Registry), [here](https://github.com/siljamdev/Tebas-Registry/tree/main/templates/html).  
