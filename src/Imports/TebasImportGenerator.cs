using System;
using AshLib;
using AshLib.Formatting;
using TabScript;
using TabScript.StandardLibraries;

class TebasImportGenerator{	
	static (Delegate func, string description)[] staticFunctions => new (Delegate, string)[]{
		(getAllProjectsPaths, "Get the paths to all projects"),
		(projectExists, "Check if project exists in a path"),
		(getProjectName, "Get the name of the project in a path if it exists"),
		(getProjectTemplateName, "Get the name of the template used in a project, based on its path"),
		(getProjectProperty, "Get a property of a project, based on its path"),
		(projectsCleanup, "Cleanup projects"),
		
		(getAllTemplateNames, "Get the names of all installed templates"),
		(templateInstalled, "Check if template is installed"),
		(templateRunGlobal, "Attempt to run a global script of a template"),
		(templateBuild, "Build a template from source in a directory"),
		(templatesCleanup, "Cleanup templates"),
		
		(getAllPluginNames, "Get the names of all installed plugins"),
		(pluginInstalled, "Check if plugin is installed"),
		(pluginRunGlobal, "Attempt to run a global script of a plugin"),
		(pluginBuild, "Build a plugin from source in a directory"),
		(pluginsCleanup, "Cleanup plugins"),
		
		(getShared, "Get shared resource"),
		(setShared, "Set shared resource"),
		(appendShared, "Append to a shared resource"),
		(getAllSharedKeys, "Get all keys with a value in shared resources"),
		(sharedCleanup, "Cleanup shared resources"),
		
		(getPathExtension, "Get extension of a file path"),
		(getPathFilename, "Get file name with extension of a file path"),
		(getPathFilenameNoExtension, "Get file name without extension of a file path"),
		(getPathDirectory, "Get parent directory of a path"),
		
		(getAllPermissionKeys, "Get all valid permission keys"),
		(getAllConfigKeys, "Get all valid config keys"),
		(getConfigValue, "Get value for a config key"),
		(cleanupAll, "Cleanup everything in Tebas"),
	};
	
	static FunctionStmt[] _compiledStaticFuncs;
	static FunctionStmt[] compiledStaticFuncs {get{
		if(_compiledStaticFuncs == null){
			_compiledStaticFuncs = Library.BuildLibrary("tebas", staticFunctions).functions;
		}
		return _compiledStaticFuncs;
	}}
	
	(Delegate func, string description)[] instanceFunctions => new (Delegate, string)[]{
		(print, "Print to Standard Output"),
		(printColor, "Print to Standard Output with color"),
		(error, "Print to Standard Error"),
		(input, "Read from Standard Input"),
	};
	
	string label;
	bool isPlugin;
	
	bool showLabel;
	
	ResolvedImport _generated;
	
	public TebasImportGenerator(bool isP, string n){
		label = n.ToUpper();
		isPlugin = isP;
		
		showLabel = Tebas.config.GetValue<bool>("scriptShowLabel");
	}
	
	public ResolvedImport Generate(){
		if(_generated == null){
			_generated = new ResolvedImport("tebas", null, null, compiledStaticFuncs.Concat(Library.BuildLibrary("tebas", instanceFunctions).functions).ToArray());
		}
		
		return _generated;
	}
	
	void print(string t){
		if(showLabel){
			Tebas.labelOutput(label, isPlugin ? Palette.plugin : Palette.template, t);
		}else{
			Tebas.output(t);
		}
	}
	
	void printColor(string t, string color){
		CharFormat? f = Color3.TryParse(color, out Color3 c) ? new CharFormat(c) : null;
		
		if(showLabel){
			Tebas.labelOutput(label, isPlugin ? Palette.plugin : Palette.template, t, f);
		}else{
			Tebas.output(t, f);
		}
	}
	
	void error(string t){
		if(showLabel){
			Tebas.labelReport(label, isPlugin ? Palette.plugin : Palette.template, t);
		}else{
			Tebas.report(t);
		}
	}
	
	string input(string prompt){
		if(showLabel){
			Tebas.labelOutputNoLineAlways(label, isPlugin ? Palette.plugin : Palette.template, prompt);
		}else{
			Tebas.outputNoLineAlways(prompt);
		}
		
		if(!Environment.UserInteractive){
			return "";
		}
		return Console.ReadLine();
	}
	
	//Staticcc
	
	static Table getAllProjectsPaths(){
		return new Table(Project.getAllDirectoryPaths());
	}
	
	static bool projectExists(string directory){
		return Project.exists(directory);
	}
	
	static string getProjectName(string directory){
		return Project.exists(directory) ? Path.GetFileName(directory) : "";
	}
	
	static string getProjectTemplateName(string directory){
		return Project.get(directory)?.templateName ?? "";
	}
	
	static Table getProjectProperty(string directory, string key){
		return Project.get(directory)?.getProperty(key) ?? new Table(0);
	}
	
	static void projectsCleanup(){
		Project.cleanup();
	}
	
	static Table getAllTemplateNames(){
		return new Table(Template.getAllNames());
	}
	
	static bool templateInstalled(string name){
		return Template.installed(name);
	}
	
	static bool templateRunGlobal(string name, string global, Table args){
		return Template.get(name)?.tryRunGlobal(global, args.contents) ?? false;
	}
	
	static bool templateBuild(string sourceDirectory, string outDirectory){
		return Template.build(sourceDirectory, outDirectory);
	}
	
	static void templatesCleanup(){
		Template.cleanup();
	}
	
	static Table getAllPluginNames(){
		return new Table(Plugin.getAllNames());
	}
	
	static bool pluginInstalled(string name){
		return Plugin.installed(name);
	}
	
	static bool pluginRunGlobal(string name, string global, Table args){
		return Plugin.get(name)?.tryRunGlobal(global, args.contents) ?? false;
	}
	
	static bool pluginBuild(string sourceDirectory, string outDirectory){
		return Plugin.build(sourceDirectory, outDirectory);
	}
	
	static void pluginsCleanup(){
		Plugin.cleanup();
	}
	
	static string getShared(string key){
		return SharedHandler.get(key);
	}
	
	static void setShared(string key, string value){
		SharedHandler.set(key, value);
	}
	
	static void appendShared(string key, string value){
		SharedHandler.append(key, value);
	}
	
	static Table getAllSharedKeys(){
		return new Table(SharedHandler.getAll());
	}
	
	static void sharedCleanup(){
		SharedHandler.cleanup();
	}
	
	static string getPathExtension(string path){
		return Path.GetExtension(path);
	}
	
	static string getPathFilename(string path){
		return Path.GetFileName(path);
	}
	
	static string getPathFilenameNoExtension(string path){
		return Path.GetFileNameWithoutExtension(path);
	}
	
	static string getPathDirectory(string path){
		return Path.GetDirectoryName(path);
	}
	
	static Table getAllPermissionKeys(){
		return new Table(Tebas.validPermissions.Select(t => t.key).ToArray());
	}
	
	static Table getAllConfigKeys(){
		return new Table(Tebas.configurableOptions.Select(c => c.key).ToArray());
	}
	
	static string getConfigValue(string key){
		if(Tebas.configurableOptions.Any(o => o.key == key)){
			return Tebas.config.GetValue(key).ToString();
		}
		return "";
	}
	
	static void cleanupAll(){
		Tebas.cleanupAll();
	}
}