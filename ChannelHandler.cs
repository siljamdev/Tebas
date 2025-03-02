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
		
		channels.SetCamp(name, path);
		
		Tebas.consoleOutput("Channel set succesfully");
		
		channels.Save();
	}
	
	public static void rename(string oldName, string newName){
		initialize();
		
		if(oldName == "default"){
			Tebas.consoleOutput("default channel cannot be renamed, only changed");
			return;
		}
		
		if(prohibitedNames.Contains(newName)){
			Tebas.consoleOutput("A channel cannot be named " + newName);
			return;
		}
		
		if(channels.CanRenameCamp(oldName, newName)){
			Tebas.consoleOutput("Name changed succesfully");
		}else{
			Tebas.consoleOutput("The specified channel doesn't exist");
		}
		
		channels.Save();
	}
	
	public static void delete(string name){
		initialize();
		
		if(name == "default"){
			Tebas.consoleOutput("default channel cannot be deleted, only changed");
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
			Tebas.consoleOutput("The specified channel doesn't exist");
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
			Tebas.consoleOutput("    " + s + ": " + (string) channels.GetCamp(s));
		}
	}
	
	public static void info(string name){
		initialize();
		
		if(channels.CanGetCamp(name, out string path)){
			Tebas.consoleOutput("Channel name: " + name);
			Tebas.consoleOutput("Channel path: " + path);
			
			string[] directories = Directory.GetDirectories(path);
			List<string> projects = new List<string>();
			
			foreach(string s in directories){
				if(File.Exists(s + "/project.tebas")){
					projects.Add(Path.GetFileName(s));
				}
			}
			
			if(projects.Count > 0){
				Tebas.consoleOutput("Number of projects in this channel: " + projects.Count);
				Tebas.consoleOutput("Project list:");
			}
			
			foreach(string s in projects){
				Tebas.consoleOutput("    " + s);
			}
		}else{
			Tebas.consoleOutput("The specified channel doesn't exist");
		}
	}
}