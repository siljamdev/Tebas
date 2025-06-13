using System;
using AshLib.AshFiles;

public static class ChannelHandler{
	
	static string[] prohibitedNames = new string[]{"rename", "delete", "list", "info"};
	
	static bool channelInit;
	
	static AshFile channels;

	static void initialize(){
		if(channelInit){
			return;
		}
		
		channels = Tebas.dep.ReadAshFile("channels.ash");
		
		if(!channels.CanGetCamp("default", out string s)){
			channels.SetCamp("default", Tebas.dep.path + "/projects");
		}
		
		channels.format = 3;
		
		channels.Save();
		
		channelInit = true;
	}
	
	public static string[] allNames(){
		initialize();
		
		List<string> names = new List<string>();
		
		foreach(KeyValuePair<string, object> kvp in channels.data){
			if(kvp.Value is string){
				names.Add(kvp.Key);
			}
		}
		
		return names.ToArray();
	}
	
	public static bool canGetPath(string name, out string path){
		initialize();
		
		if(channels.CanGetCamp(name, out path)){
			return true;
		}else{
			path = null;
			return false;
		}
	}
	
	public static string getPath(string name){
		initialize();
		
		if(channels.CanGetCamp(name, out string path)){
			return path;
		}else{
			return null;
		}
	}
	
	public static void set(string name, string path){
		initialize();
		
		if(!isNameValid(name)){
			return;
		}
		
		channels.SetCamp(name, path);
		
		Tebas.consoleOutput("Channel set succesfully");
		
		channels.Save();
	}
	
	public static void rename(string oldName, string newName){
		initialize();
		
		if(oldName == "default"){
			Tebas.consoleError("#default channel cannot be renamed, only changed");
			return;
		}
		
		if(!isNameValid(newName)){
			return;
		}
		
		if(channels.CanRenameCamp(oldName, newName)){
			Tebas.consoleOutput("Name changed succesfully");
		}else{
			Tebas.consoleError("The specified channel doesn't exist");
		}
		
		channels.Save();
	}
	
	public static void delete(string name){
		initialize();
		
		if(name == "default"){
			Tebas.consoleError("default channel cannot be deleted, only changed");
			return;
		}
		
		if(channels.ExistsCamp(name)){
			if(Tebas.askDeletionConfirmation()){
				channels.DeleteCamp(name);
				Tebas.consoleOutput("Channel deleted succesfully");
			}else{
				Tebas.consoleOutput("Deletion cancelled");
			}
		}else{
			Tebas.consoleError("The specified channel doesn't exist");
		}
		
		channels.Save();
	}
	
	public static void list(){
		initialize();
		
		List<string> c = new List<string>();
		
		foreach(KeyValuePair<string, object> kvp in channels.data){
			if(kvp.Value is string){
				c.Add(kvp.Key);
			}
		}
		
		if(c.Count > 0){
			Tebas.consoleOutput("List of channels:");
		}
		
		foreach(string s in c){
			Tebas.consoleOutput("    #" + s + ": " + (string) channels.GetCamp(s));
		}
	}
	
	public static void info(string name){
		initialize();
		
		if(channels.CanGetCamp(name, out string path)){
			Tebas.consoleOutput("Channel name: #" + name);
			Tebas.consoleOutput("Channel path: '" + path + "'");
			
			List<string> projects = allProjects(name);
			
			if(projects.Count > 0){
				Tebas.consoleOutput("Number of projects in this channel: " + projects.Count);
				Tebas.consoleOutput("Project list:");
			}else{
				Tebas.consoleOutput("There are no projects in this channel");
			}
			
			foreach(string s in projects){
				Tebas.consoleOutput("    " + s);
			}
		}else{
			Tebas.consoleError("The specified channel doesn't exist");
		}
	}
	
	public static List<string> allProjects(string name){
		initialize();
		
		if(channels.CanGetCamp(name, out string path)){
			if(!Directory.Exists(path)){
				return new List<string>();
			}
			string[] directories = Directory.GetDirectories(path);
			List<string> projects = new List<string>();
			
			foreach(string s in directories){
				if(File.Exists(s + "/project.tebas")){
					projects.Add(s);
				}
			}
			
			return projects;
		}else{
			return new List<string>();
		}
	}
	
	public static int stats(string name){
		initialize();
		
		if(channels.CanGetCamp(name, out string path)){
			List<string> projects = allProjects(name);
			
			if(projects.Count > 0){
				Tebas.consoleOutput("Number of projects in this channel: " + projects.Count);
				Tebas.consoleOutput("Project list:");
			}else{
				Tebas.consoleOutput("There are no projects in this channel");
			}
			
			int l = 0;
			
			foreach(string s in projects){
				Tebas.workingDirectory = s;
				int n = Tebas.getNumberOfLinesOfCode();
				l += n;
				Tebas.consoleOutput("    " + Path.GetFileName(s) + ": " + n);
			}
			
			if(projects.Count > 0){
				Tebas.consoleOutput("Total lines of code: " + l);
			}
			return l;
		}else{
			Tebas.consoleError("The specified channel doesn't exist");
			return 0;
		}
	}
	
	public static int statsShort(string name){
		initialize();
		
		if(channels.CanGetCamp(name, out string path)){
			List<string> projects = allProjects(name);
			
			int l = 0;
			
			foreach(string s in projects){
				Tebas.workingDirectory = s;
				int n = Tebas.getNumberOfLinesOfCode();
				l += n;
			}
			
			if(projects.Count > 0){
				Tebas.consoleOutput("    #" + name + ": " + l);
			}
			return l;
		}else{
			return 0;
		}
	}
	
	static bool isNameValid(string name){
		if(prohibitedNames.Contains(name)){
			Tebas.consoleError("A channel cannot be named #" + name);
			return false;
		}
		
		if(name.Any(StringHelper.isWhitespace)){
			Tebas.consoleError("A channel name cannot have spaces");
			return false;
		}
		
		if(name.Contains('#')){
			Tebas.consoleError("A channel name cannot have '#'");
			return false;
		}
		
		if(name == ""){
			Tebas.consoleError("A channel name cannot be empty");
			return false;
		}
		
		return true;
	}
	
	public static string fixName(string name){
		if(name.StartsWith("#")){
			return name.Substring(1).ToLower();
		}
		return name.ToLower();
	}
}