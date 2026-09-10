using System;
using TableScript;
using TableScript.Generator;

[TableScriptLibrary("installtool.cs")]
partial class InstallTool{
	static InstallTool _dummy = null;
	public static InstallTool Dummy{get{
		if(_dummy == null){
			_dummy = new InstallTool(null, k => false);
		}
		return _dummy;
	}}
	
	static bool hasSeenInstallHint = false;
	static void displayInstallhint(){
		if(!hasSeenInstallHint){
			Tebas.hint("To skip this, do 'tebas <template|plugin> permission <name> skipInstallationConfirmation allow'");
			hasSeenInstallHint = true;
		}
	}
	
	Predicate<string> hasPermission;
	string path;
	
	public InstallTool(string p, Predicate<string> hp){
		hasPermission = hp;
		path = p;
	}
	
	//Helper
	bool installAllowed(string type, string path){
		if(hasPermission("skipInstallationConfirmation")){
			return true;
		}
		
		displayInstallhint();
		
		return Tebas.askConfirmation("Do you want to install a " + type + " from '" + path + "'?");
	}
	
	/// <summary>
	/// Install a template from a file in the project path
	/// </summary>
	[TableScriptFunction]
	public bool templateInstallLocal(string path){
		string path2 = path + "/" + path;
		
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
	
	/// <summary>
	/// Install a plugin from a file in the project path
	/// </summary>
	[TableScriptFunction]
	public bool pluginInstallLocal(string path){
		string path2 = path + "/" + path;
		
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
}