using System;
using AshLib;

public static class GitHelper{
	public static void init(){
		if(!Directory.Exists(".git")){
			ProcessExecuter.runProcess("GIT", getGitPath(), "init -b " + getBranch(), Tebas.workingDirectory);
		}else{
			Tebas.consoleError("Git has already been initilized here");
		}
		
		Tebas.project.SetCamp("git.initialized", true);
	}
	
	public static void tryInit(){
		if(!Tebas.project.CanGetCamp("git.initialized", out bool b) || !b){
			init();
		}
	}
	
	public static void addGitignore(){
		if(File.Exists(Tebas.workingDirectory + "/.gitignore")){
			return;
		}
		
		string gitIgnore = "";
		if(Tebas.config.CanGetCamp("git.defaultGitignore", out string k)){
			gitIgnore = k;
		}
		
		if(Tebas.template.CanGetCamp("git.gitignore", out string l)){
			gitIgnore += Environment.NewLine + l;
		}
		
		if(gitIgnore.Length > 0){
			File.WriteAllText(Tebas.workingDirectory + "/.gitignore", gitIgnore);
		}
	}
	
	public static void status(){
		tryInit();
		Tebas.consoleOutput("Current working branch: " + getBranch());
		ProcessExecuter.runProcess("GIT", getGitPath(), "status", Tebas.workingDirectory);
	}
	
	public static void add(){
		tryInit();
		ProcessExecuter.runProcess("GIT", getGitPath(), "add -A", Tebas.workingDirectory);
	}
	
	public static void commit(string m){
		Tebas.initializeConfig();
		tryInit();
		if(Tebas.config.CanGetCamp("git.autoAddOnCommit", out bool b) && b){
			add();
		}
		
		ProcessExecuter.runProcess("GIT", getGitPath(), "commit -m \"" + m + "\"", Tebas.workingDirectory);
	}
	
	public static void push(string r){
		if(getNumberOfRemotes() < 1){
			Tebas.consoleError("There are no remotes in this project");
			return;
		}
		if(getNumberOfRemotes() == 1){
			r = getSingleRemote();
		}else if(r == null){
			Tebas.consoleError("There is more than one remote, it needs to be specified");
			return;
		}
		
		if(!remoteExists(r)){
			Tebas.consoleError("That remote does not exsist");
			return;
		}
		
		tryInit();
		ProcessExecuter.runProcess("GIT", getGitPath(), "push " + r + " " + getBranch(), Tebas.workingDirectory);
	}
	
	public static void pull(string r){
		if(getNumberOfRemotes() < 1){
			Tebas.consoleError("There are no remotes in this project");
			return;
		}
		if(getNumberOfRemotes() == 1){
			r = getSingleRemote();
		}else if(r == null){
			Tebas.consoleError("There is more than one remote, it needs to be specified");
			return;
		}
		
		if(!remoteExists(r)){
			Tebas.consoleError("That remote does not exsist");
			return;
		}
		
		tryInit();
		ProcessExecuter.runProcess("GIT", getGitPath(), "pull " + r + " " + getBranch(), Tebas.workingDirectory);
	}
	
	static string getGitPath(){
		Tebas.initializeConfig();
		
		string gitPath = "git";
		if(Tebas.config.CanGetCamp("git.path", out string s)){
			gitPath = s;
		}
		return gitPath;
	}
	
	public static string getBranch(){
		if(!Tebas.initializeLocal()){
			return getDefaultBranch();
		}
		if(Tebas.project.CanGetCamp("git.branch", out string s)){
			return s;
		}
		return getDefaultBranch();
	}
	
	public static string getDefaultBranch(){
		Tebas.initializeConfig();
		
		string branch = "main";
		if(Tebas.config.CanGetCamp("git.defaultBranch", out string s)){
			branch = s;
		}
		return branch;
	}
	
	public static void setWorkingBranch(string branch){
		if(!Tebas.initializeLocal()){
			return;
		}
		Tebas.project.SetCamp("git.branch", branch);
		Tebas.project.Save();
		
		int exitCode = ProcessExecuter.runProcessExitCode(getGitPath(), "rev-parse --verify refs/heads/" + branch, Tebas.workingDirectory);
		
		if(exitCode == 0){
			ProcessExecuter.runProcess("GIT", getGitPath(), "switch " + branch, Tebas.workingDirectory);
		}else{
			ProcessExecuter.runProcess("GIT", getGitPath(), "switch -c " + branch, Tebas.workingDirectory);
		}
	}
	
	public static bool remoteExists(string name){
		if(Tebas.project.CanGetCamp("git.remote." + name, out string s)){
			return true;
		}
		return false;
	}
	
	public static string getRemoteUrl(string name){
		if(Tebas.project.CanGetCamp("git.remote." + name, out string s)){
			return s;
		}
		return "";
	}
	
	public static void remoteSet(string name, string url){
		if(!Tebas.initializeLocal()){
			return;
		}
		
		Tebas.project.SetCamp("git.remote." + name, url);
		Tebas.project.Save();
		
		tryInit();
		
		int exitCode = ProcessExecuter.runProcessExitCode(getGitPath(), "remote get-url " + name, Tebas.workingDirectory);
		if(exitCode == 0){
			ProcessExecuter.runProcess("GIT", getGitPath(), "remote set-url " + name + " \"" + url + "\"", Tebas.workingDirectory);
		}else{
			ProcessExecuter.runProcess("GIT", getGitPath(), "remote add " + name + " \"" + url + "\"", Tebas.workingDirectory);
		}
		
		Tebas.consoleOutput("Remote set successfully");
	}
	
	public static void remoteDelete(string name){
		if(!Tebas.initializeLocal()){
			return;
		}
		
		if(!remoteExists(name)){
			Tebas.consoleError("That remote does not exsist");
			return;
		}
		
		if(!Tebas.forced && !Tebas.askDeletionConfirmation()){
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
			Tebas.consoleError("That remote does not exsist");
			return;
		}
		
		tryInit();
		Tebas.project.RenameCamp("git.remote." + oldname, "git.remote." + newname);
		Tebas.project.Save();
		
		ProcessExecuter.runProcess("GIT", getGitPath(), "remote rename " + oldname + " " + newname, Tebas.workingDirectory);
		
		Tebas.consoleOutput("Remote renamed successfully");
	}
	
	public static List<string> getAllRemotes(){
		if(!Tebas.initializeLocal()){
			return null;
		}
		
		List<string> r = new List<string>();
		
		foreach(KeyValuePair<string, object> kvp in Tebas.project.data){
			if(kvp.Key.StartsWith("git.remote.") && kvp.Value is string){
				r.Add(kvp.Key.Substring(11));
			}
		}
		
		return r;
	}
	
	public static void printAllRemotes(){
		if(!Tebas.initializeLocal()){
			return;
		}
		
		Tebas.consoleOutput("Complete list of git remotes:");
		foreach(KeyValuePair<string, object> kvp in Tebas.project.data){
			if(kvp.Key.StartsWith("git.remote.") && kvp.Value is string s){
				Tebas.consoleOutput(kvp.Key.Substring(11) + ": " + s);
			}
		}
	}
	
	static int getNumberOfRemotes(){
		if(!Tebas.initializeLocal()){
			return -1;
		}
		int i = 0;
		foreach(KeyValuePair<string, object> kvp in Tebas.project.data){
			if(kvp.Key.StartsWith("git.remote.") && kvp.Value is string){
				i++;
			}
		}
		return i;
	}
	
	static string getSingleRemote(){
		if(!Tebas.initializeLocal()){
			return null;
		}
		
		foreach(KeyValuePair<string, object> kvp in Tebas.project.data){
			if(kvp.Key.StartsWith("git.remote.") && kvp.Value is string){
				return kvp.Key.Substring(11);
			}
		}
		return null;
	}
}