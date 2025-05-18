using System;
using AshLib.AshFiles;

public static class PluginHandler{
	static AshFile plugins;
	
	static bool init;
	
	public static string runningPlugin;
	
	public static void initialize(){
		if(init){
			return;
		}
		
		Tebas.initializeLocalSilent();
		
		plugins = Tebas.dep.ReadAshFile("plugins.ash");
		
		plugins.format = 3;
		
		init = true;
	}
	
	public static bool exists(string name){
		initialize();
		
		foreach(KeyValuePair<string, object> kvp in plugins.data){
			if(kvp.Value is string){
				if(kvp.Key.Split(".")[0] == name){
					return true;
				}
			}
		}
		
		return false;
	}
	
	public static List<string> getList(){
		initialize();
		
		List<string> p = new List<string>();
		
		foreach(KeyValuePair<string, object> kvp in plugins.data){
			string[] n = kvp.Key.Split(".");
			if(kvp.Value is string && n[1] != "resources"){
				if(!p.Contains(n[0])){
					p.Add(n[0]);
				}
			}
		}
		
		return p;
	}
	
	public static void list(){
		initialize();
		
		List<string> p = new List<string>();
		
		foreach(KeyValuePair<string, object> kvp in plugins.data){
			string[] n = kvp.Key.Split(".");
			if(kvp.Value is string && n[1] != "resources"){
				if(!p.Contains(n[0])){
					p.Add(n[0]);
				}
			}
		}
		
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
	
	public static bool runScript(string plugin, string name, IEnumerable<string> args){
		initialize();
		
		string code = "";
		if(!plugins.CanGetCamp(plugin + ".script." + name, out code)){
			return false;
		}
		Script s = new Script(plugin + " " + name, code);
		runningPlugin = plugin;
		s.run(args);
		runningPlugin = null;
		return true;
	}
	
	public static bool runScript(string plugin, string name){
		initialize();
		
		string code = "";
		if(!plugins.CanGetCamp(plugin + ".script." + name, out code)){
			return false;
		}
		Script s = new Script(plugin + " " + name, code);
		runningPlugin = plugin;
		s.run(null);
		runningPlugin = null;
		return true;
	}
	
	public static void install(string path, IEnumerable<string> args){
		initialize();
		
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
		}
		
		if(!Tebas.forced && Tebas.isConsoleInteractive() && exists(name)){
			Console.WriteLine("A plugin called " + name + " is already installed, do you want to update it? (Y/N)");
			string ans = Console.ReadLine();
			
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
				
				if(ans.ToLower() != "y"){
					return;
				}
			}
		}
		
		foreach(KeyValuePair<string, object> kvp in plugins.data){
			if(kvp.Key.Split(".")[0] == name){
				plugins.DeleteCamp(kvp.Key);
			}
		}
		
		plugin.DeleteCamp("name");
		plugin.DeleteCamp("version");
		
		List<string> keysList = plugin.data.Keys.ToList();
		
		foreach(string s in keysList){
			plugin.RenameCamp(s, name + "." + s);
		}
		
		plugins = plugins + plugin;
		
		plugins.Save();
		
		runScript(name, "install", args);
		
		Tebas.consoleOutput("Plugin succesfully installed: " + name);
	}
	
	public static void info(string name){
		initialize();
		
		if(!exists(name)){
			Tebas.consoleError("That plugin is not installed");
			return;
		}
		
		Tebas.consoleOutput("Plugin name: " + name);
		
		string desc = plugins.GetCampOrDefault<string>(name + ".description", null);
		if(desc != null){
			Tebas.consoleOutput("Plugin description: " + desc);
		}
		
		List<string> scripts = new List<string>();
		
		foreach(string k in plugins.data.Keys){
			if(k.StartsWith(name + ".script.")){
				scripts.Add(k.Substring(8 + name.Length));
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
		initialize();
		
		if(!exists(name)){
			Console.WriteLine("That plugin is not installed");
			return;
		}
		
		if(Tebas.forced || Tebas.askDeletionConfirmation()){
			runScript(name, "uninstall");
			foreach(KeyValuePair<string, object> kvp in plugins.data){
				if(kvp.Key.Split(".")[0] == name){
					plugins.DeleteCamp(kvp.Key);
				}
			}
			plugins.Save();
			Tebas.consoleOutput("Plugin uninstalled succesfully");
		}else{
			Tebas.consoleOutput("Uninstallation cancelled");
		}
	}
	
	public static void writeResource(string plugin, string name, string content){
		initialize();
		
		if(!exists(plugin)){
			return;
		}
		
		plugins.SetCamp(plugin + ".resources." + name, content);
		
		plugins.Save();
	}
	
	public static void appendResource(string plugin, string name, string content){
		initialize();
		
		if(!exists(plugin)){
			return;
		}
		
		if(plugins.CanGetCamp(plugin + ".resources." + name, out string v)){
			plugins.SetCamp(plugin + ".resources." + name, v + content);
		}else{
			plugins.SetCamp(plugin + ".resources." + name, content);
		}
		
		plugins.Save();
	}
	
	public static string readResource(string plugin, string name){
		initialize();
		if(!exists(plugin)){
			return "";
		}
		
		if(plugins.CanGetCamp(plugin + ".resources." + name, out string v)){
			return v;
		}else{
			return "";
		}
	}
	
	public static bool isNameValid(string name){		
		if(name.Contains(' ')){
			Tebas.consoleError("A plugin name cannot have spaces, plugin name was: '" + name + "'");
			return false;
		}
		
		if(name.Contains('*')){
			Tebas.consoleError("A plugin name cannot have '*', plugin name was: '" + name + "'");
			return false;
		}
		
		if(name.Contains('@')){
			Tebas.consoleOutput("A plugin name cannot have '@', plugin name was: '" + name + "'");
			return false;
		}
		
		return true;
	}
	
	public static bool isScriptNameValid(string name){		
		if(name.Contains(' ') || name.Contains('*') || name.Contains('@')){
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