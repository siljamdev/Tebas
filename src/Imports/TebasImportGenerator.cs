using System;
using AshLib;
using AshLib.Formatting;
using TabScript;
using TabScript.StandardLibraries;

class TebasImportGenerator{
	static TebasImportGenerator _dummy = null;
	public static TebasImportGenerator Dummy{get{
		if(_dummy == null){
			_dummy = new TebasImportGenerator(false, "");
		}
		return _dummy;
	}}
	
	static (Delegate func, string description)[] staticFunctions => new (Delegate, string)[]{
		(getAllProjectsPaths, "Get the directory paths to all projects"),
		(projectExists, "Check if project exists in a directory"),
		(getProjectTemplateName, "Get the name of the template used in a project, based on its directory. Returns an empty string if no project exists in that directory"),
		(getProjectProperty, "Get a property of a project, based on its directory. Returns an empty table if no project exists in that directory"),
		(projectsCleanup, "Cleanup projects"),
		
		(getAllTemplateNames, "Get the names of all installed templates"),
		(templateInstalled, "Check if a template is installed"),
		(templateRunGlobal, "Attempt to run a global script of a template. Returns true if the operation was successful"),
		(templatesCleanup, "Cleanup templates"),
		
		(getAllPluginNames, "Get the names of all installed plugins"),
		(pluginInstalled, "Check if a plugin is installed"),
		(pluginRunGlobal, "Attempt to run a global script of a plugin. Returns true if the operation was successful"),
		(pluginsCleanup, "Cleanup plugins"),
		
		(getShared, "Get shared resource"),
		(setShared, "Set shared resource"),
		(appendShared, "Append to the end of a shared resource"),
		(getAllSharedKeys, "Get all keys with a value in shared resources"),
		(sharedCleanup, "Cleanup shared resources: cleans internal invalid or empty values"),
		
		(getAllPermissionKeys, "Get all valid permission keys"),
		(getAllConfigKeys, "Get all valid config keys"),
		(getConfigValue, "Get value for a config key"),
		(getVersion, "Get Tebas version"),
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
		(printFormat, "Print to Standard Output with color(hexadecimal)"),
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
		
		showLabel = Tebas.config.GetValue<bool>("script.showLabel");
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
	
	void printFormat(string t){
		FormatString fs = new FormatString(t);
		
		if(showLabel){
			Tebas.labelOutput(label, isPlugin ? Palette.plugin : Palette.template, fs);
		}else{
			Tebas.output(fs);
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
	
	static string getVersion(){
		return "v" + BuildInfo.Version;
	}
	
	static void cleanupAll(){
		Tebas.cleanupAll();
	}
}