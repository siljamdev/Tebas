using System;
using AshLib;
using AshLib.Formatting;
using TableScript;
using TableScript.StandardLibraries;
using TableScript.Generator;

[TableScriptLibrary("tebas.cs")]
partial class TebasImportGenerator{
	static TebasImportGenerator _dummy = null;
	public static TebasImportGenerator Dummy{get{
		if(_dummy == null){
			_dummy = new TebasImportGenerator(false, "");
		}
		return _dummy;
	}}
	
	string label;
	bool isPlugin;
	
	bool showLabel;
	
	public TebasImportGenerator(bool isP, string n){
		label = n.ToUpper();
		isPlugin = isP;
		
		showLabel = Tebas.config.GetValue<bool>("script.showLabel");
	}
	
	/// <summary>
	/// Print to Standard Output
	/// </summary>
	[TableScriptFunction]
	public void print(string t){
		if(showLabel){
			Tebas.labelOutput(label, isPlugin ? Palette.plugin : Palette.template, t);
		}else{
			Tebas.output(t);
		}
	}
	
	/// <summary>
	/// Print to Standard Output with format (AshFile FormatString)
	/// </summary>
	[TableScriptFunction]
	public void printFormat(string t){
		FormatString fs = new FormatString(t);
		
		if(showLabel){
			Tebas.labelOutput(label, isPlugin ? Palette.plugin : Palette.template, fs);
		}else{
			Tebas.output(fs);
		}
	}
	
	/// <summary>
	/// Print to Standard Error
	/// </summary>
	[TableScriptFunction]
	public void error(string t){
		if(showLabel){
			Tebas.labelReport(label, isPlugin ? Palette.plugin : Palette.template, t);
		}else{
			Tebas.report(t);
		}
	}
	
	/// <summary>
	/// Read from Standard Input
	/// </summary>
	[TableScriptFunction]
	public string input(string prompt){
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
	
	/// <summary>
	/// Tebas version
	/// </summary>
	[TableScriptGlobal]
	public static readonly string version = "v" + BuildInfo.Version;
	
	/// <summary>
	/// Get the directory paths to all projects
	/// </summary>
	[TableScriptFunction]
	public static Table getAllProjectsPaths(){
		return new Table(Project.getAllDirectoryPaths());
	}
	
	/// <summary>
	/// Check if project exists in a directory
	/// </summary>
	[TableScriptFunction]
	public static bool projectExists(string directory){
		return Project.exists(directory);
	}
	
	/// <summary>
	/// Get the name of the project in a path. Returns an empty string if no project exists in that directory
	/// </summary>
	[TableScriptFunction]
	public static string getProjectName(string directory){
		return Project.exists(directory) ? Path.GetFileName(directory) : "";
	}
	
	/// <summary>
	/// Get the name of the template used in a project, based on its directory. Returns an empty string if no project exists in that directory
	/// </summary>
	[TableScriptFunction]
	public static string getProjectTemplateName(string directory){
		return Project.get(directory)?.templateName ?? "";
	}
	
	/// <summary>
	/// Get a property of a project, based on its directory. Returns an empty table if no project exists in that directory
	/// </summary>
	[TableScriptFunction]
	public static Table getProjectProperty(string directory, string key){
		return Project.get(directory)?.getProperty(key) ?? new Table(0);
	}
	
	/// <summary>
	/// Cleanup projects
	/// </summary>
	[TableScriptFunction]
	public static void projectsCleanup(){
		Project.cleanup();
	}
	
	/// <summary>
	/// Get the names of all installed templates
	/// </summary>
	[TableScriptFunction]
	public static Table getAllTemplateNames(){
		return new Table(Template.getAllNames());
	}
	
	/// <summary>
	/// Check if a template is installed
	/// </summary>
	[TableScriptFunction]
	public static bool templateInstalled(string name){
		return Template.installed(name);
	}
	
	/// <summary>
	/// Attempt to run a global script of a template. Returns true if the operation was successful
	/// </summary>
	[TableScriptFunction]
	public static bool templateRunGlobal(string name, string global, Table args){
		return Template.get(name)?.tryRunGlobal(global, args.contents) ?? false;
	}
	
	/// <summary>
	/// Cleanup templates
	/// </summary>
	[TableScriptFunction]
	public static void templatesCleanup(){
		Template.cleanup();
	}
	
	/// <summary>
	/// Get the names of all installed plugins
	/// </summary>
	[TableScriptFunction]
	public static Table getAllPluginNames(){
		return new Table(Plugin.getAllNames());
	}
	
	/// <summary>
	/// Check if a plugin is installed
	/// </summary>
	[TableScriptFunction]
	public static bool pluginInstalled(string name){
		return Plugin.installed(name);
	}
	
	/// <summary>
	/// Attempt to run a global script of a plugin. Returns true if the operation was successful
	/// </summary>
	[TableScriptFunction]
	public static bool pluginRunGlobal(string name, string global, Table args){
		return Plugin.get(name)?.tryRunGlobal(global, args.contents) ?? false;
	}
	
	/// <summary>
	/// Cleanup plugins
	/// </summary>
	[TableScriptFunction]
	public static void pluginsCleanup(){
		Plugin.cleanup();
	}
	
	/// <summary>
	/// Get shared resource
	/// </summary>
	[TableScriptFunction]
	public static string getShared(string key){
		return SharedHandler.get(key);
	}
	
	/// <summary>
	/// Set shared resource
	/// </summary>
	[TableScriptFunction]
	public static void setShared(string key, string value){
		SharedHandler.set(key, value);
	}
	
	/// <summary>
	/// Append to the end of a shared resource
	/// </summary>
	[TableScriptFunction]
	public static void appendShared(string key, string value){
		SharedHandler.append(key, value);
	}
	
	/// <summary>
	/// Get all keys with a value in shared resources
	/// </summary>
	[TableScriptFunction]
	public static Table getAllSharedKeys(){
		return new Table(SharedHandler.getAll());
	}
	
	/// <summary>
	/// Cleanup shared resources: cleans internal invalid or empty values
	/// </summary>
	[TableScriptFunction]
	public static void sharedCleanup(){
		SharedHandler.cleanup();
	}
	
	/// <summary>
	/// Get all valid permission keys
	/// </summary>
	[TableScriptFunction]
	public static Table getAllPermissionKeys(){
		return new Table(Tebas.validPermissions.Select(t => t.key).ToArray());
	}
	
	/// <summary>
	/// Get all valid config keys
	/// </summary>
	[TableScriptFunction]
	public static Table getAllConfigKeys(){
		return new Table(Tebas.configurableOptions.Select(c => c.key).ToArray());
	}
	
	/// <summary>
	/// Get value for a config key
	/// </summary>
	[TableScriptFunction]
	public static string getConfigValue(string key){
		if(Tebas.configurableOptions.Any(o => o.key == key)){
			return Tebas.config.GetValue(key).ToString();
		}
		return "";
	}
	
	/// <summary>
	/// Get Tebas version
	/// </summary>
	[TableScriptFunction]
	public static string getVersion(){
		return "v" + BuildInfo.Version;
	}
	
	/// <summary>
	/// Cleanup everything in Tebas. This function does the same as running `tebas cleanup`
	/// </summary>
	[TableScriptFunction]
	public static void cleanupAll(){
		Tebas.cleanupAll();
	}
}