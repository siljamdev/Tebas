using System;
using AshLib;

public static class PluginHandler{
	static AshFile plugins;
	
	static bool init;
	
	public static string runningPlugin;
	
	public static void initialize(){
		if(init){
			return;
		}
		
		Tebas.initializeLocalSilent();
		
		plugins = new AshFile(Tebas.dep.path + "/plugins.ash");
		init = true;
	}
	
	public static bool exists(string name){
		initialize();
		
		foreach(KeyValuePair<string, CampValue> kvp in plugins.data){
			if(kvp.Value.CanGetString(out string s)){
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
		
		foreach(KeyValuePair<string, CampValue> kvp in plugins.data){
			string[] n = kvp.Key.Split(".");
			if(kvp.Value.CanGetString(out string s) && n[1] != "resources"){
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
		
		foreach(KeyValuePair<string, CampValue> kvp in plugins.data){
			string[] n = kvp.Key.Split(".");
			if(kvp.Value.CanGetString(out string s) && n[1] != "resources"){
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
		if(!plugins.CanGetCampAsString(plugin + ".script." + name, out code)){
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
		if(!plugins.CanGetCampAsString(plugin + ".script." + name, out code)){
			return false;
		}
		Script s = new Script(plugin + " " + name, code);
		runningPlugin = plugin;
		s.run(null);
		runningPlugin = null;
		return true;
	}
	
	public static void install(string path){
		initialize();
		
		path = StringHelper.removeQuotesSingle(path);
		
		if(!File.Exists(path)){
			Tebas.consoleOutput("That file does not exist");
			return;
		}
		
		AshFile plugin = new AshFile(path);
		
		string name;
		if(!plugin.CanGetCampAsString("name", out name)){
			Tebas.consoleOutput("Plugin is incorrectly formatted: name missing");
			return;
		}
		
		if(exists(name)){
			Console.WriteLine("A plugin with that name is already installed, do you want to update it? (Y/N)");
			string ans = Console.ReadLine();
			
			if(ans.ToLower() != "y"){
				return;
			}
		}
		
		foreach(KeyValuePair<string, CampValue> kvp in plugins.data){
			if(kvp.Key.Split(".")[0] == name){
				plugins.DeleteCamp(kvp.Key);
			}
		}
		
		plugin.DeleteCamp("name");
		
		plugins = plugins + plugin;
		
		plugins.Save();
		
		runScript(name, "install");
		
		Tebas.consoleOutput("Plugin succesfully installed: " + name);
	}
	
	public static void uninstall(string name){
		initialize();
		
		if(!exists(name)){
			Console.WriteLine("That plugin is not installed");
			return;
		}
		
		if(Tebas.askDeletionConfirmation()){
			runScript(name, "uninstall");
			foreach(KeyValuePair<string, CampValue> kvp in plugins.data){
				if(kvp.Key.Split(".")[0] == name){
					plugins.DeleteCamp(kvp.Key);
				}
			}
			plugins.Save();
			Tebas.consoleOutput("Template uninstalled succesfully");
		}else{
			Tebas.consoleOutput("Deletion cancelled");
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
		
		if(plugins.CanGetCampAsString(plugin + ".resources." + name, out string v)){
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
		
		if(plugins.CanGetCampAsString(plugin + ".resources." + name, out string v)){
			return v;
		}else{
			return "";
		}
	}
}