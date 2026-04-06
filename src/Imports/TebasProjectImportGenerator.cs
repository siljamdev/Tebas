using System;
using AshLib;
using AshLib.Dates;
using AshLib.Formatting;
using TabScript;

class TebasProjectImportGenerator{
	static ResolvedImport _dummy = null;
	public static ResolvedImport GenerateDummy(){
		if(_dummy == null){
			TebasProjectImportGenerator d = new TebasProjectImportGenerator(null);
			FunctionStmt[] _compiledIndependentFuncs = Library.BuildLibrary("tebasproject", d.independentFunctions).functions;
			
			//Install
			bool templateInstallLocal(string path){
				return false;
			}
			bool pluginInstallLocal(string path){
				return false;
			}
			
			ProcessExecuter px = ProcessExecuter.Dummy;
			FileUnit fu = FileUnit.Dummy;
			
			(string name, Delegate func, string description)[] nonIndependentFunctions = new (string, Delegate, string)[]{
				("templateInstallLocal", templateInstallLocal, "Install a template from a file in the project path"),
				("pluginInstallLocal", pluginInstallLocal, "Install a plugin from a file in the project path"),
			}.Concat(px.NamedFunctions).Concat(fu.NamedFunctions).ToArray();
			
			_dummy = new ResolvedImport("tebasproject", null, null, _compiledIndependentFuncs.Concat(Library.BuildLibrary("tebasproject", nonIndependentFunctions).functions).ToArray());
		}
		return _dummy;
	}
	
	static bool hasSeenInstallHint = false;
	
	(Delegate func, string description)[] independentFunctions => new (Delegate, string)[]{
		(getPath, "Get the project path"),
		(getName, "Get the project name"),
		(getCreationDate, "Get the date and hour of creation in [yy, MM, dd, hh, mm, ss] format"),
		(getTemplateName, "Get the used template name"),
		
		(runScriptOrGlobal, "Run a script or a template global"),
		(runScript, "Run a script"),
		(runPluginScriptOrGlobal, "Run a plugin script or a plugin global"),
		(runPluginScript, "Run a plugin script"),
		
		(getProperty, "Get a project property"),
		
		(getResource, "Get a project resource"),
		(setResource, "Set a project resource"),
		(appendResource, "Append to a project resource"),
		(getAllResourceKeys, "Get all keys with a value in project resources"),
		
		(templateBuild, "Build a template from source in a directory in the project path"),
		(pluginBuild, "Build a plugin from source in a directory in the project path"),
		
		(cleanup, "Cleanup this project"),
	};
	
	Project proj;
	
	FunctionStmt[] _compiledIndependentFuncs;
	
	public TebasProjectImportGenerator(Project p){
		proj = p;
	}
	
	public ResolvedImport Generate(bool isPlugin, Predicate<string> hasPermission){
		if(_compiledIndependentFuncs == null){
			_compiledIndependentFuncs = Library.BuildLibrary("tebasproject", independentFunctions).functions;
		}
		
		//Non independent
		
		//Install
		bool installAllowed(string type, string path){
			if(hasPermission("skipInstallationConfirmation")){
				return true;
			}
			
			displayInstallhint();
			
			return Tebas.askConfirmation("Do you want to install a " + type + " from '" + path + "'?");
		}
		
		bool templateInstallLocal(string path){
			string path2 = getPath() + "/" + path;
			
			if(installAllowed("template", path2)){
				bool f = Tebas.forced;
				Tebas.forced = false;
				bool r = Template.installLocal(path2);
				Tebas.forced = f;
				return r;
			}else{
				return false;
			}
		}
		
		bool pluginInstallLocal(string path){
			string path2 = getPath() + "/" + path;
			
			if(installAllowed("plugin", path2)){
				bool f = Tebas.forced;
				Tebas.forced = false;
				bool r = Plugin.installLocal(path2);
				Tebas.forced = f;
				return r;
			}else{
				return false;
			}
		}
		
		//Processes
		ProcessExecuter px = new ProcessExecuter(getPath(), "project", isPlugin, hasPermission);
		
		//Files
		FileUnit fu = new FileUnit(getPath(), "project", ".tebas", isPlugin, hasPermission);
		
		(string name, Delegate func, string description)[] nonIndependentFunctions = new (string, Delegate, string)[]{
			("templateInstallLocal", templateInstallLocal, "Install a template from a file in the project path"),
			("pluginInstallLocal", pluginInstallLocal, "Install a plugin from a file in the project path"),
		}.Concat(px.NamedFunctions).Concat(fu.NamedFunctions).ToArray(); //Add process & file funcs
		
		return new ResolvedImport("tebasproject", null, null, _compiledIndependentFuncs.Concat(Library.BuildLibrary("tebasproject", nonIndependentFunctions).functions).ToArray());
	}
	
	string getPath(){
		return proj.path;
	}
	
	string getName(){
		return proj.name;
	}
	
	Table getCreationDate(){
		Date d = proj.creationDate;
		return new Table(d.years.ToString(), d.months.ToString(), d.days.ToString(), d.hours.ToString(), d.minutes.ToString(), d.seconds.ToString());
	}
	
	string getTemplateName(){
		return proj.templateName;
	}
	
	bool runScriptOrGlobal(string script, Table args){
		return proj.tryRunScriptOrGlobal(script, args.contents);
	}
	
	bool runScript(string script, Table args){
		return proj.tryRunScript(script, args.contents);
	}
	
	bool runPluginScriptOrGlobal(string plugin, string script, Table args){
		Plugin p = Plugin.get(plugin);
		if(p == null){
			return false;
		}
		return proj.tryRunPluginScriptOrGlobal(p, script, args.contents);
	}
	
	bool runPluginScript(string plugin, string script, Table args){
		Plugin p = Plugin.get(plugin);
		if(p == null){
			return false;
		}
		return proj.tryRunPluginScript(p, script, args.contents);
	}
	
	Table getProperty(string key){
		return proj.getProperty(key);
	}
	
	string getResource(string key){
		return proj.getResource(key);
	}
	
	void setResource(string key, string value){
		proj.setResource(key, value);
	}
	
	void appendResource(string key, string value){
		proj.appendResource(key, value);
	}
	
	Table getAllResourceKeys(){
		return new Table(proj.getAllResourceKeys());
	}
	
	//Build
	bool templateBuild(string sourceDirectory, string outDirectory){
		return Template.build(getPath() + "/" + sourceDirectory, getPath() + "/" + outDirectory);
	}
	
	bool pluginBuild(string sourceDirectory, string outDirectory){
		return Plugin.build(getPath() + "/" + sourceDirectory, getPath() + "/" + outDirectory);
	}
	
	void cleanup(){
		proj.cleanupInstance();
	}
	
	static void displayInstallhint(){
		if(!hasSeenInstallHint){
			Tebas.hint("To skip this, do 'tebas <template|plugin> permission <name> skipInstallationConfirmation allow'");
			hasSeenInstallHint = true;
		}
	}
}