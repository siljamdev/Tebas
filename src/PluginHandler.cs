using System;
using AshLib.AshFiles;

public static class PluginHandler{
	static AshFile currentPlugin;
	
	public static string runningPlugin;
	
	public static bool exists(string name){
		return File.Exists(Tebas.dep.path + "/plugins/" + name + ".tbplg");
	}
	
	public static bool load(string name){
		if(name != null && exists(name)){
			currentPlugin = new AshFile(Tebas.dep.path + "/plugins/" + name + ".tbplg");
			return true;
		}
		currentPlugin = null;
		return false;
	}
	
	public static List<string> getList(){
		if(!Directory.Exists(Tebas.dep.path + "/plugins")){
			return new List<string>();
		}
		string[] pls = Directory.GetFiles(Tebas.dep.path + "/plugins", "*.tbplg");
		
		return pls.Select(Path.GetFileNameWithoutExtension).ToList();
	}
	
	public static void list(){
		List<string> p = getList();
		
		if(p.Count > 0){
			Tebas.consoleOutput("Number of plugins installed: " + p.Count);
			Tebas.consoleOutput("Plugin list:");
		}else{
			Tebas.consoleOutput("No plugins installed");
		}
		
		foreach(string s in p){
			Tebas.consoleOutput("    " + s);
		}
	}
	
	public static bool runScript(string plugin, string name, IEnumerable<string> args = null){
		Tebas.initializeLocalSilent();
		
		if(!load(plugin)){
			return false;
		}
		
		string code = "";
		if(!currentPlugin.CanGetCamp("script." + name, out code)){
			return false;
		}
		Script s = new Script(plugin + " " + name, ScriptType.Plugin, code);
		runningPlugin = plugin;
		s.run(args);
		runningPlugin = null;
		return true;
	}
	
	public static void install(string path, IEnumerable<string> args){
		path = StringHelper.removeQuotesSingle(path);
		
		if(!File.Exists(path)){
			Tebas.consoleError("That file does not exist");
			return;
		}
		
		AshFile plugin = new AshFile(path);
		
		string name;
		if(!plugin.CanGetCamp("name", out name)){
			Tebas.consoleError("Plugin is incorrectly formatted: name missing");
			return;
		}
		
		if(!isNameValid(name)){
			return;
		}
		
		string desc = plugin.GetCampOrDefault<string>("description", null);
		if(desc != null){
			Tebas.consoleOutput("Plugin description: " + desc);
			Tebas.consoleOutput("");
		}
		
		if(!Tebas.forced && Tebas.isConsoleInteractive() && exists(name)){
			Console.WriteLine("A plugin called " + name + " is already installed, do you want to update it? (Y/N)");
			string ans = Console.ReadLine();
			Tebas.consoleOutput("");
			
			if(ans.ToLower() != "y"){
				return;
			}
		}
		
		if(plugin.CanGetCamp("version", out string vs)){
			int v = Tebas.isVersionNewer(vs);
			if(v == -1){
				Console.WriteLine("The plugin version(" + vs + ") is newer than the current tebas version(" + Tebas.currentVersion + "). Please update your client");
				return;
			}else if(v == 1 && !Tebas.forced){
				Console.WriteLine("The plugin version(" + vs + ") is older than the current tebas version(" + Tebas.currentVersion + "), do you want to install it? (Y/N)");
				string ans = Console.ReadLine();
				Tebas.consoleOutput("");
				
				if(ans.ToLower() != "y"){
					return;
				}
			}
		}
		
		plugin.Save(Tebas.dep.path + "/plugins/" + name + ".tbplg");
		
		runScript(name, "install", args);
		
		Tebas.consoleOutput("");
		Tebas.consoleOutput("Plugin succesfully installed: " + name);
	}
	
	public static void info(string name){
		if(!load(name)){
			Tebas.consoleError("That plugin is not installed");
			return;
		}
		
		Tebas.consoleOutput("Plugin name: " + name);
		
		string desc = currentPlugin.GetCampOrDefault<string>("description", null);
		if(desc != null){
			Tebas.consoleOutput("Plugin description: " + desc);
		}
		
		List<string> scripts = new List<string>();
		
		foreach(string k in currentPlugin.data.Keys){
			if(k.StartsWith("script.")){
				scripts.Add(k.Substring(7));
			}
		}
		
		if(scripts.Count > 0){
			Tebas.consoleOutput("This plugin has " + scripts.Count + " scripts:");
			
			foreach(string h in scripts){
				Tebas.consoleOutput("    " + h);
			}
		}
		
		runScript(name, "info");
	}
	
	public static void uninstall(string name){
		if(!load(name)){
			Console.WriteLine("That plugin is not installed");
			return;
		}
		
		if(Tebas.forced || Tebas.askDeletionConfirmation()){
			runScript(name, "uninstall");
			File.Delete(Tebas.dep.path + "/plugins/" + name + ".tbplg");
			Tebas.consoleOutput("");
			Tebas.consoleOutput("Plugin uninstalled succesfully");
		}else{
			Tebas.consoleOutput("Uninstallation cancelled");
		}
	}
	
	public static void writeResource(string name, string content){
		if(currentPlugin == null){
			return;
		}
		
		if(content == ""){
			currentPlugin.DeleteCamp("resources." + name);
		}else{
			currentPlugin.SetCamp("resources." + name, content);
		}
		
		currentPlugin.Save();
	}
	
	public static void appendResource(string name, string content){
		if(currentPlugin == null){
			return;
		}
		
		if(currentPlugin.CanGetCamp("resources." + name, out string v)){
			currentPlugin.SetCamp("resources." + name, v + content);
		}else if(content != ""){
			currentPlugin.SetCamp("resources." + name, content);
		}
		
		currentPlugin.Save();
	}
	
	public static string readResource(string name){
		if(currentPlugin == null){
			return "";
		}
		
		if(currentPlugin.CanGetCamp("resources." + name, out string v)){
			return v;
		}else{
			return "";
		}
	}
	
	public static bool isNameValid(string name){		
		if(name.Any(StringHelper.isWhitespace)){
			Tebas.consoleError("A plugin name cannot have spaces, plugin name was: '" + name + "'");
			return false;
		}
		
		if(name.Contains('*')){
			Tebas.consoleError("A plugin name cannot have '*', plugin name was: '" + name + "'");
			return false;
		}
		
		if(name.Contains('@')){
			Tebas.consoleError("A plugin name cannot have '@', plugin name was: '" + name + "'");
			return false;
		}
		
		if(name == ""){
			Tebas.consoleError("A plugin name cannot be empty, plugin name was: '" + name + "'");
			return false;
		}
		
		return true;
	}
	
	public static bool isScriptNameValid(string name){		
		if(name == "" || name.Any(StringHelper.isWhitespace) || name.Contains('*') || name.Contains('@')){
			return false;
		}
		return true;
	}
	
	public static string fixName(string name){
		if(name.StartsWith("*")){
			return name.Substring(1);
		}
		return name;
	}
}