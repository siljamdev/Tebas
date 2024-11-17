using System;
using AshLib;

public static class GitHelper{
	public static void init(){
		if(!Directory.Exists(".git")){
			ProcessExecuter.runProcess("GIT", getGitPath(), "init -b " + getBranch(), Tebas.workingDirectory);
		}else{
			Tebas.consoleOutput("Git has already been initilized here");
		}
		
		Tebas.project.SetCamp("git.initialized", true);
	}
	
	static void tryInit(){
		if(Tebas.project.CanGetCampAsBool("git.initialized", out bool b) && !b){
			init();
		}
	}
	
	public static void status(){
		tryInit();
		ProcessExecuter.runProcess("GIT", getGitPath(), "status", Tebas.workingDirectory);
	}
	
	public static void add(){
		tryInit();
		ProcessExecuter.runProcess("GIT", getGitPath(), "add .", Tebas.workingDirectory);
	}
	
	public static void commit(string m){
		Tebas.initializeConfig();
		tryInit();
		if(Tebas.config.CanGetCampAsBool("git.autoAddOnCommit", out bool b) && b){
			add();
		}
		
		ProcessExecuter.runProcess("GIT", getGitPath(), "commit -m \"" + m + "\"", Tebas.workingDirectory);
	}
	
	public static void push(string r){
		if(getNumberOfRemotes() < 1){
			Tebas.consoleOutput("There are no remotes in this project");
			return;
		}
		if(getNumberOfRemotes() == 1){
			r = getSingleRemote();
		}else if(r == null){
			Tebas.consoleOutput("There is more than one remote, it needs to be specified");
			return;
		}
		
		if(!remoteExists(r)){
			Tebas.consoleOutput("That remote does not exsist");
			return;
		}
		
		tryInit();
		ProcessExecuter.runProcess("GIT", getGitPath(), "push " + r + " " + getBranch(), Tebas.workingDirectory);
	}
	
	public static void pull(string r){
		if(getNumberOfRemotes() < 1){
			Tebas.consoleOutput("There are no remotes in this project");
			return;
		}
		if(getNumberOfRemotes() == 1){
			r = getSingleRemote();
		}else if(r == null){
			Tebas.consoleOutput("There is more than one remote, it needs to be specified");
			return;
		}
		
		if(!remoteExists(r)){
			Tebas.consoleOutput("That remote does not exsist");
			return;
		}
		
		tryInit();
		ProcessExecuter.runProcess("GIT", getGitPath(), "pull " + r + " " + getBranch(), Tebas.workingDirectory);
	}
	
	static string getGitPath(){
		Tebas.initializeConfig();
		
		string gitPath = "git";
		if(Tebas.config.CanGetCampAsString("git.path", out string s)){
			gitPath = s;
		}
		return gitPath;
	}
	
	static string getBranch(){
		Tebas.initializeConfig();
		
		string branch = "main";
		if(Tebas.config.CanGetCampAsString("git.defaultBranch", out string s)){
			branch = s;
		}
		return branch;
	}
	
	static bool remoteExists(string name){
		if(Tebas.project.CanGetCampAsString("git.remote." + name, out string s)){
			return true;
		}
		return false;
	}
	
	public static void remoteSet(string name, string url){
		if(!Tebas.initializeLocal()){
			return;
		}
		
		bool h = remoteExists(name);
		
		Tebas.project.SetCamp("git.remote." + name, url);
		Tebas.project.Save();
		
		tryInit();
		if(h){
			ProcessExecuter.runProcess("GIT", getGitPath(), "remote set-url " + name + " \"" + url + "\"", Tebas.workingDirectory);
		}
		ProcessExecuter.runProcess("GIT", getGitPath(), "remote add " + name + " \"" + url + "\"", Tebas.workingDirectory);
		
		Tebas.consoleOutput("Remote set successfully");
	}
	
	public static void remoteDelete(string name){
		if(!Tebas.initializeLocal()){
			return;
		}
		
		if(!remoteExists(name)){
			Tebas.consoleOutput("That remote does not exsist");
			return;
		}
		
		if(!Tebas.askDeletionConfirmation()){
			Tebas.consoleOutput("Deletion cancelled");
			return;
		}
		
		tryInit();
		Tebas.project.DeleteCamp("git.remote." + name);
		Tebas.project.Save();
		
		ProcessExecuter.runProcess("GIT", getGitPath(), "remote remove " + name, Tebas.workingDirectory);
		
		Tebas.consoleOutput("Remote deleted successfully");
	}
	
	public static void remoteRename(string oldname, string newname){
		if(!Tebas.initializeLocal()){
			return;
		}
		
		if(!remoteExists(oldname)){
			Tebas.consoleOutput("That remote does not exsist");
			return;
		}
		
		tryInit();
		Tebas.project.RenameCamp("git.remote." + oldname, "git.remote." + newname);
		Tebas.project.Save();
		
		ProcessExecuter.runProcess("GIT", getGitPath(), "remote rename " + oldname + " " + newname, Tebas.workingDirectory);
		
		Tebas.consoleOutput("Remote renamed successfully");
	}
	
	public static void printAllRemotes(){
		if(!Tebas.initializeLocal()){
			return;
		}
		
		Tebas.consoleOutput("Complete list of git remotes:");
		foreach(KeyValuePair<string, CampValue> kvp in Tebas.project.data){
			if(kvp.Key.StartsWith("git.remote.") && kvp.Value.CanGetString(out string s)){
				Tebas.consoleOutput(kvp.Key.Substring(11) + ": " + s);
			}
		}
	}
	
	static int getNumberOfRemotes(){
		if(!Tebas.initializeLocal()){
			return -1;
		}
		int i = 0;
		foreach(KeyValuePair<string, CampValue> kvp in Tebas.project.data){
			if(kvp.Key.StartsWith("git.remote.") && kvp.Value.CanGetString(out string s)){
				i++;
			}
		}
		return i;
	}
	
	static string getSingleRemote(){
		if(!Tebas.initializeLocal()){
			return null;
		}
		
		foreach(KeyValuePair<string, CampValue> kvp in Tebas.project.data){
			if(kvp.Key.StartsWith("git.remote.") && kvp.Value.CanGetString(out string s)){
				return kvp.Key.Substring(11);
			}
		}
		return null;
	}
}