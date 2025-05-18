using System;

public static class CommandLineHandler{
	
	static int clp; //Command line pointer
	
	public static void process(string[] args){
		
		clp = 0;
		
		if(args.Length < 1){
			commandHelp(args);
			return;
		}
		
		while(clp < args.Length && args[clp].StartsWith("-")){
			string f = args[clp];
			switch(f){
				case "-q":
				case "--quiet":
					Tebas.quiet = true;
					break;
				case "-f":
				case "--force":
					Tebas.forced = true;
					break;
				case "-h":
				case "--help":
					commandHelp(args);
					return;
				case "-v":
				case "--version":
					commandVersion(args);
					return;
			}
			clp++;
		}
		
		/* for(int i = 0; i < args.Length; i++){
			args[i] = StringHelper.removeQuotesSingle(args[i]); //If i search it, this is the line
		} */
		
		if(clp < args.Length && Path.GetExtension(args[clp]) == ".tbtem"){
			TemplateHandler.install(args[clp], args.Skip(clp + 1));
			waitAnyKey();
			return;
		}else if(clp < args.Length && Path.GetExtension(args[clp]) == ".tebas"){
			Tebas.workingDirectory = Path.GetDirectoryName(args[clp]);
			Tebas.localInfo();
			waitAnyKey();
			return;
		}else if(clp < args.Length && Path.GetExtension(args[clp]) == ".tbplg"){
			PluginHandler.install(args[clp], args.Skip(clp + 1));
			waitAnyKey();
			return;
		}else if(clp < args.Length && Path.GetExtension(args[clp]) == ".tbscr"){
			Tebas.runStandaloneScript(args[clp], args.Skip(clp + 1));
			return;
		}
		
		commands(args);
	}
	
	//Body of the program, where it actually does things
	static void commands(string[] args){		
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		switch(args[clp]){
			case "project":
			clp++;
			commandsProject(args);
			break;
			
			case "channel":
			clp++;
			commandsChannel(args);
			break;
			
			case "template":
			clp++;
			commandsTemplate(args);
			break;
			
			case "plugin":
			clp++;
			commandsPlugin(args);
			break;
			
			case "global":
			clp++;
			commandsGlobal(args);
			break;
			
			case "local":
			clp++;
			commandsLocal(args);
			break;
			
			case "script": //Standalone script
			clp++;
			commandScript(args);
			break;
			
			case "loop": //Run loop
			clp++;
			commandLoop(args);
			break;
			
			case "help":
			clp++;
			commandHelp(args);
			break;
			
			case "version":
			clp++;
			commandVersion(args);
			break;
			
			default:
			commandsProjectOrLocal(args);
			break;
		}
	}
	
	static void commandsProject(string[] args){
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		switch(args[clp]){
			case "new":
			clp++;
			commandProjectNew(args);
			break;
			
			case "rename":
			clp++;
			commandProjectRename(args);
			break;
			
			case "delete":
			clp++;
			commandProjectDelete(args);
			break;
			
			default:
			Tebas.consoleSevereError("Unknown command: " + args[clp]);
			break;
		}
	}
	
	static void commandsChannel(string[] args){
		if(!determineIfEnoughLength(1, args.Length)){
			commandChannelList(args);
			return;
		}
		
		switch(args[clp]){
			case "set":
			clp++;
			commandChannelSet(args);
			break;
			
			case "rename":
			clp++;
			commandChannelRename(args);
			break;
			
			case "delete":
			clp++;
			commandChannelDelete(args);
			break;
			
			case "list":
			clp++;
			commandChannelList(args);
			break;
			
			case "stats":
			clp++;
			commandChannelStats(args);
			break;
			
			case "info":
			clp++;
			commandChannelInfo(args);
			break;
			
			default:
			commandChannelSet(args);
			break;
		}
	}
	
	static void commandsTemplate(string[] args){
		if(!determineIfEnoughLength(1, args.Length)){
			commandTemplateList(args);
			return;
		}
		
		switch(args[clp]){
			case "install":
			clp++;
			commandTemplateInstall(args);
			break;
			
			case "uninstall":
			clp++;
			commandTemplateUninstall(args);
			break;
			
			case "list":
			clp++;
			commandTemplateList(args);
			break;
			
			case "info":
			clp++;
			commandTemplateInfo(args);
			break;
			
			case "create":
			clp++;
			commandTemplateCreate(args);
			break;
			
			default:
			commandTryTemplateGlobalScript(args);
			break;
		}
	}
	
	static void commandsPlugin(string[] args){
		if(!determineIfEnoughLength(1, args.Length)){
			commandPluginList(args);
			return;
		}
		
		switch(args[clp]){
			case "install":
			clp++;
			commandPluginInstall(args);
			break;
			
			case "uninstall":
			clp++;
			commandPluginUninstall(args);
			break;
			
			case "info":
			clp++;
			commandPluginInfo(args);
			break;
			
			case "list":
			clp++;
			commandPluginList(args);
			break;
			
			case "create":
			clp++;
			commandPluginCreate(args);
			break;
			
			default:
			commandTryPluginScript(args);
			break;
		}
	}
	
	static void commandsGlobal(string[] args){
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		switch(args[clp]){
			case "config":
			clp++;
			commandGlobalConfig(args);
			break;
			
			case "list":
			clp++;
			commandGlobalList(args);
			break;
			
			case "stats":
			clp++;
			commandGlobalStats(args);
			break;
			
			default:
			Tebas.consoleSevereError("Unknown command: " + args[clp]);
			break;
		}
	}
	
	static void commandsLocal(string[] args){
		if(!determineIfEnoughLength(1, args.Length)){
			commandLocalInfo(args);
			return;
		}
		
		switch(args[clp]){			
			case "info":
			clp++;
			commandLocalInfo(args);
			break;
			
			case "git":
			clp++;
			commandLocalGit(args);
			break;
			
			case "usegit":
			clp++;
			commandLocalUseGit(args);
			break;
			
			case "stopgit":
			clp++;
			commandLocalStopGit(args);
			break;
			
			case "setbranch":
			clp++;
			commandLocalSetBranch(args);
			break;
			
			case "stats":
			clp++;
			commandLocalStats(args);
			break;
			
			case "remote":
			clp++;
			commandsLocalRemote(args);
			break;
			
			case "commit":
			clp++;
			commandLocalCommit(args);
			break;
			
			case "add":
			clp++;
			commandLocalAdd(args);
			break;
			
			case "push":
			clp++;
			commandLocalPush(args);
			break;
			
			case "pull":
			clp++;
			commandLocalPull(args);
			break;
			
			case "init":
			clp++;
			commandLocalInit(args);
			break;
			
			default:
			commandTryScript(args);
			break;
		}
	}
	
	static void commandsProjectOrLocal(string[] args){
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		switch(args[clp]){
			case "new":
			clp++;
			commandProjectNew(args);
			break;
			
			case "rename":
			clp++;
			commandProjectRename(args);
			break;
			
			case "delete":
			clp++;
			commandProjectDelete(args);
			break;
			
			case "info":
			clp++;
			commandLocalInfo(args);
			break;
			
			case "git":
			clp++;
			commandLocalGit(args);
			break;
			
			case "usegit":
			clp++;
			commandLocalUseGit(args);
			break;
			
			case "stopgit":
			clp++;
			commandLocalStopGit(args);
			break;
			
			case "setbranch":
			clp++;
			commandLocalSetBranch(args);
			break;
			
			case "stats":
			clp++;
			commandLocalStats(args);
			break;
			
			case "remote":
			clp++;
			commandsLocalRemote(args);
			break;
			
			case "commit":
			clp++;
			commandLocalCommit(args);
			break;
			
			case "add":
			clp++;
			commandLocalAdd(args);
			break;
			
			case "push":
			clp++;
			commandLocalPush(args);
			break;
			
			case "pull":
			clp++;
			commandLocalPull(args);
			break;
			
			case "init":
			clp++;
			commandLocalInit(args);
			break;
			
			default:
			commandTryScriptOrPluginOrGlobal(args);
			break;
		}
	}
	
	static void commandsLocalRemote(string[] args){
		if(!determineIfEnoughLength(1, args.Length)){
			commandLocalRemoteList(args);
			return;
		}
		
		switch(args[clp]){
			case "list":
			clp++;
			commandLocalRemoteList(args);
			break;
			
			case "set":
			clp++;
			commandLocalRemoteSet(args);
			break;
			
			case "delete":
			clp++;
			commandLocalRemoteDelete(args);
			break;
			
			case "rename":
			clp++;
			commandLocalRemoteRename(args);
			break;
			
			default:
			Tebas.consoleSevereError("Unknown command: " + args[clp]);
			break;
		}
	}
	
	
	//Individual commands
	static void commandProjectNew(string[] args){
		string channelName;
		string templateName;
		string projectName;
		
		if(!determineIfEnoughLength(2, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		if(args[clp].StartsWith("#")){ //get channel
			channelName = ChannelHandler.fixName(args[clp]);
			clp++;
			
			if(!determineIfEnoughLength(2, args.Length)){
				Tebas.consoleSevereError("Not enough arguments");
				return;
			}
		}else{
			channelName = "default";
		}
		
		templateName = TemplateHandler.fixName(args[clp]);
		clp++;
		
		projectName = args[clp];
		clp++;
		
		Tebas.projectNew(channelName, templateName, projectName);
	}
	
	static void commandProjectRename(string[] args){
		string channelName;
		string oldName;
		string newName;
		
		if(!determineIfEnoughLength(2, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		if(args[clp].StartsWith("#")){ //get channel
			channelName = ChannelHandler.fixName(args[clp]);
			clp++;
			
			if(!determineIfEnoughLength(2, args.Length)){
				Tebas.consoleSevereError("Not enough arguments");
				return;
			}
		}else{
			channelName = "default";
		}
		
		oldName = args[clp];
		clp++;
		
		newName = args[clp];
		clp++;
		
		Tebas.projectRename(channelName, oldName, newName);
	}
	
	static void commandProjectDelete(string[] args){
		string channelName;
		string name;
		
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		if(args[clp].StartsWith("#")){ //get channel
			channelName = ChannelHandler.fixName(args[clp]);
			clp++;
			
			if(!determineIfEnoughLength(2, args.Length)){
				Tebas.consoleSevereError("Not enough arguments");
				return;
			}
		}else{
			channelName = "default";
		}
		
		name = args[clp];
		clp++;
		
		Tebas.projectDelete(channelName, name);
	}
	
	//Channel
	static void commandChannelSet(string[] args){
		string name;
		string v;
		
		if(!determineIfEnoughLength(2, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		name = ChannelHandler.fixName(args[clp]);
		clp++;
		
		v = args[clp];
		clp++;
		
		ChannelHandler.set(name, v);
	}
	
	static void commandChannelRename(string[] args){
		string oldName;
		string newName;
		
		if(!determineIfEnoughLength(2, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		oldName = ChannelHandler.fixName(args[clp]);
		clp++;
		
		newName = ChannelHandler.fixName(args[clp]);
		clp++;
		
		ChannelHandler.rename(oldName, newName);
	}
	
	static void commandChannelDelete(string[] args){
		string name;
		
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		name = ChannelHandler.fixName(args[clp]);
		clp++;
		
		ChannelHandler.delete(name);
	}
	
	static void commandChannelList(string[] args){		
		ChannelHandler.list();
	}
	
	static void commandChannelInfo(string[] args){
		string name;
		
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		name = ChannelHandler.fixName(args[clp]);
		clp++;
		
		ChannelHandler.info(name);
	}
	
	static void commandChannelStats(string[] args){
		string name;
		
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		name = ChannelHandler.fixName(args[clp]);
		clp++;
		
		ChannelHandler.stats(name);
	}
	
	//Template
	static void commandTemplateInstall(string[] args){
		string path;
		
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		path = StringHelper.removeQuotesSingle(args[clp]);
		clp++;
		
		TemplateHandler.install(path, args.Skip(clp));
	}
	
	static void commandTemplateUninstall(string[] args){
		string name;
		
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		name = TemplateHandler.fixName(args[clp]);
		clp++;
		
		TemplateHandler.uninstall(name);
	}
	
	static void commandTemplateList(string[] args){
		TemplateHandler.list();
	}
	
	static void commandTemplateInfo(string[] args){
		string name;
		
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		name = TemplateHandler.fixName(args[clp]);
		clp++;
		
		TemplateHandler.info(name);
	}
	
	static void commandTemplateCreate(string[] args){
		string path;
		
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		path = StringHelper.removeQuotesSingle(args[clp]);
		clp++;
		
		CreatorUtility.template(path);
	}
	
	static void commandTryTemplateGlobalScript(string[] args){
		string p;
		string s;
		
		if(!determineIfEnoughLength(2, args.Length)){
			Tebas.consoleSevereError("Unknown command: " + args[clp - 1]);
			return;
		}
		
		p = TemplateHandler.fixName(args[clp]);
		clp++;
		
		s = args[clp];
		clp++;
		
		if(!TemplateHandler.runGlobal(p, s, args.Skip(clp))){
			Tebas.consoleSevereError("Unknown command or script: " + s);
			return;
		}
	}
	
	//Plugin
	static void commandPluginInstall(string[] args){
		string path;
		
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		path = StringHelper.removeQuotesSingle(args[clp]);
		clp++;
		
		PluginHandler.install(path, args.Skip(clp));
	}
	
	static void commandPluginUninstall(string[] args){
		string name;
		
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		name = PluginHandler.fixName(args[clp]);
		clp++;
		
		PluginHandler.uninstall(name);
	}
	
	static void commandPluginInfo(string[] args){
		string name;
		
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		name = PluginHandler.fixName(args[clp]);
		clp++;
		
		PluginHandler.info(name);
	}
	
	static void commandPluginList(string[] args){
		PluginHandler.list();
	}
	
	static void commandTryPluginScript(string[] args){
		string p;
		string s;
		
		if(!determineIfEnoughLength(2, args.Length)){
			Tebas.consoleSevereError("Unknown command: " + args[clp - 1]);
			return;
		}
		
		p = PluginHandler.fixName(args[clp]);
		clp++;
		
		s = args[clp];
		clp++;
		
		if(!PluginHandler.runScript(p, s, args.Skip(clp))){
			Tebas.consoleSevereError("Unknown command or script: " + s);
			return;
		}
	}
	
	static void commandPluginCreate(string[] args){
		string path;
		
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		path = StringHelper.removeQuotesSingle(args[clp]);
		clp++;
		
		CreatorUtility.plugin(path);
	}
	
	//Global
	static void commandGlobalConfig(string[] args){
		string key;
		string v;
		
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		key = args[clp];
		clp++;
		
		if(key == "list" || key == "see"){
			Tebas.globalConfig(key, null);
			return;
		}
		
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		v = args[clp];
		clp++;
		
		Tebas.globalConfig(key, v);
	}
	
	static void commandGlobalList(string[] args){
		Tebas.globalList();
	}
	
	static void commandGlobalStats(string[] args){
		Tebas.globalStats();
	}
	
	//Local
	static void commandLocalInfo(string[] args){
		Tebas.localInfo();
	}
	
	static void commandLocalGit(string[] args){
		Tebas.localGit();
	}
	
	static void commandLocalUseGit(string[] args){
		Tebas.localGitStartUsing();
	}
	
	static void commandLocalStopGit(string[] args){
		Tebas.localGitStopUsing();
	}
	
	static void commandLocalStats(string[] args){
		Tebas.localStats();
	}
	
	static void commandLocalAdd(string[] args){
		Tebas.localAdd();
	}
	
	static void commandLocalCommit(string[] args){
		string m;
		
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		m = args[clp];
		clp++;
		
		Tebas.localCommit(m);
	}
	
	static void commandLocalPush(string[] args){
		if(args.Length > clp){
			string m = args[clp];
			clp++;
		
			Tebas.localPush(m);
			return;
		}
		
		Tebas.localPush(null);
	}
	
	static void commandLocalPull(string[] args){
		if(args.Length > clp){
			string m = args[clp];
			clp++;
		
			Tebas.localPull(m);
			return;
		}
		
		Tebas.localPull(null);
	}
	
	static void commandLocalSetBranch(string[] args){
		string branch;
		
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		branch = args[clp];
		clp++;
		
		Tebas.localSetBranch(branch);
	}
	
	static void commandLocalInit(string[] args){
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		string t = TemplateHandler.fixName(args[clp]);
		clp++;
		
		Tebas.localInitNew(t);
	}
	
	static void commandTryScript(string[] args){
		string v;
		
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		v = args[clp];
		clp++;
		
		Tebas.tryScript(v, args.Skip(clp));
	}
	
	static void commandTryScriptOrPluginOrGlobal(string[] args){
		string name;
		
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		name = args[clp];
		clp++;
		
		if(name.StartsWith("*")){
			if(!determineIfEnoughLength(1, args.Length)){
				Tebas.consoleSevereError("Missing script name");
				return;
			}
			
			name = PluginHandler.fixName(name);
			
			string s = args[clp];
			clp++;
			
			if(!PluginHandler.runScript(name, s, args.Skip(clp))){
				Tebas.consoleSevereError("Unknown script");
				return;
			}
		}else if(name.StartsWith("@")){
			if(!determineIfEnoughLength(1, args.Length)){
				Tebas.consoleSevereError("Missing script name");
				return;
			}
			
			name = TemplateHandler.fixName(name);
			
			string s = args[clp];
			clp++;
			
			if(!TemplateHandler.runGlobal(name, s, args.Skip(clp))){
				Tebas.consoleSevereError("Unknown global script");
				return;
			}
		}else{
			Tebas.tryScript(name, args.Skip(clp));
		}
	}
	
	//local config remote
	
	static void commandLocalRemoteList(string[] args){
		Tebas.localRemoteList();
	}
	
	static void commandLocalRemoteSet(string[] args){
		string n;
		string u;
		
		if(!determineIfEnoughLength(2, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		n = args[clp];
		clp++;
		
		u = args[clp];
		clp++;
		
		Tebas.localRemoteSet(n, u);
	}
	
	static void commandLocalRemoteDelete(string[] args){
		string n;
		
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		n = args[clp];
		clp++;
		
		Tebas.localRemoteDelete(n);
	}
	
	static void commandLocalRemoteRename(string[] args){
		string n;
		string u;
		
		if(!determineIfEnoughLength(2, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		n = args[clp];
		clp++;
		
		u = args[clp];
		clp++;
		
		Tebas.localRemoteRename(n, u);
	}
	
	static void commandScript(string[] args){
		string f;
		if(!determineIfEnoughLength(1, args.Length)){
			Tebas.consoleSevereError("Not enough arguments");
			return;
		}
		
		f = StringHelper.removeQuotesSingle(args[clp]);
		clp++;
		
		Tebas.runStandaloneScript(f, args.Skip(clp));
	}
	
	static void commandLoop(string[] args){
		Tebas.loop();
	}
	
	static void commandVersion(string[] args){
		Tebas.version();
	}
	
	static void commandHelp(string[] args){
		Tebas.consoleOutput("Help for CLI:");
		Tebas.consoleOutput("");
		Tebas.consoleOutput("Usage:");
		Tebas.consoleOutput("  tebas [flags] <section> <subcommand1> <subcommand2>...");
		Tebas.consoleOutput("Flags:");
		Tebas.consoleOutput("  --quiet, -q      Make app quiet, where the console output is minimal");
		Tebas.consoleOutput("  --force, -f      Make it forced. This only affects certain actions that usually need confirmation, like deletions");
		Tebas.consoleOutput("  --help, -h       Show this menu");
		Tebas.consoleOutput("  --version, -v    Show the current tebas version");
		Tebas.consoleOutput("");
		Tebas.consoleOutput("Sections:");
		Tebas.consoleOutput("");
		Tebas.consoleOutput("version  Shows the current Tebas version");
		Tebas.consoleOutput("");
		Tebas.consoleOutput("loop  Enters a state where you can write as many commands as wanted");
		Tebas.consoleOutput("");
		Tebas.consoleOutput("channel");
		Tebas.consoleOutput(" Manages channels(folders where projects are). This section can be executed anywhere");
		Tebas.consoleOutput("        <name> <path>            Followed by any name and a folder path, will create or set that channel");
		Tebas.consoleOutput("        delete <name>            Deletes that channel");
		Tebas.consoleOutput("        rename <name> <newname>  Renames that channel");
		Tebas.consoleOutput("        list                     Shows a list of all channels");
		Tebas.consoleOutput("        info <name>              Shows info on a specific channel");
		Tebas.consoleOutput("        stats <name>             Shows stats on a specific channel. Mainly total number of code lines");
		Tebas.consoleOutput("");
		Tebas.consoleOutput("template");
		Tebas.consoleOutput(" Manages the templates (templates for projects). This section can be executed anywhere");
		Tebas.consoleOutput("         install <path> [args]   Installs the template from a file (.tbtem)");
		Tebas.consoleOutput("         uninstall <name>        Deletes that template");
		Tebas.consoleOutput("         list                    Shows list of all templates installed");
		Tebas.consoleOutput("         info <name>             Shows info on a specific template");
		Tebas.consoleOutput("         create <path>           Create a template using the creator utility");
		Tebas.consoleOutput("         <name> <script> [args]  Attempts to run a global script of that template");
		Tebas.consoleOutput("");
		Tebas.consoleOutput("<@template name> <script> [args]   Another way to run a global template script");
		Tebas.consoleOutput("");
		Tebas.consoleOutput("plugin");
		Tebas.consoleOutput(" Manages the plugins. This section can be executed anywhere");
		Tebas.consoleOutput("       install <path> [args]   Installs the plugin from a file (.tbplg)");
		Tebas.consoleOutput("       uninstall <name>        Deletes that plugin");
		Tebas.consoleOutput("       list                    Shows list of all plugins installed");
		Tebas.consoleOutput("       info <name>             Shows info on a specific plugin");
		Tebas.consoleOutput("       create <path>           Create a plugin using the creator utility");
		Tebas.consoleOutput("       <name> <script> [args]  Attempts to run a script of that plugin");
		Tebas.consoleOutput("");
		Tebas.consoleOutput("<*plugin name> <script> [args]   Another way to run a plugin script");
		Tebas.consoleOutput("");
		Tebas.consoleOutput("script");
		Tebas.consoleOutput(" Lets you run a plugin, just for the fun of it. All enviroment variables are empty but the working directory");
		Tebas.consoleOutput("       <script src path> [args]   Attempts to run a script from a file");
		Tebas.consoleOutput("");
		Tebas.consoleOutput("global");
		Tebas.consoleOutput(" Manages global things. This section can be executed anywhere");
		Tebas.consoleOutput("       config                Manages the global config. This has more options:");
		Tebas.consoleOutput("              <key> <value>  Changes the global config");
		Tebas.consoleOutput("              list           Shows a list of all keys");
		Tebas.consoleOutput("              see            Shows the config current values");
		Tebas.consoleOutput("       list                  Shows list of all of the projects");
		Tebas.consoleOutput("       stats                 Shows the stats of all the projects");
		Tebas.consoleOutput("");
		Tebas.consoleOutput("project");
		Tebas.consoleOutput(" Manages projects. This section can be executed anywhere, and the project keyword can be skipped entirely (For example, instead of 'tebas project new', 'tebas new'");
		Tebas.consoleOutput("        new [#channel] <template> <name>    Creates a new project. The channel can be skipped, and the default one will be used. To specify the channel, precede its name with '#'");
		Tebas.consoleOutput("        delete [#channel] <name>            Deletes a project. The channel can be skipped, and the default one will be used. To specify the channel, precede its name with '#'");
		Tebas.consoleOutput("        rename [#channel] <name> <newname>  Renames a project. The channel can be skipped, and the default one will be used. To specify the channel, precede its name with '#'");
		Tebas.consoleOutput("");
		Tebas.consoleOutput("local");
		Tebas.consoleOutput(" Manages the local project. This section must be executed in a folder containing a project, and the local keyword can be skipped entirely (For example, instead of 'tebas local info', 'tebas info'");
		Tebas.consoleOutput("      git                 The equivalent of doing 'git status'");
		Tebas.consoleOutput("      usegit              Starts using git in the current project");
		Tebas.consoleOutput("      stopgit             Stops using git in the current project");
		Tebas.consoleOutput("      setbranch <branch>  Sets the git working branch");
		Tebas.consoleOutput("      push [remote]       The equivalent of doing 'git push'. If there is only one remote, you dont need to specify it");
		Tebas.consoleOutput("      pull [remote]       The equivalent of doing 'git pull'. If there is only one remote, you dont need to specify it");
		Tebas.consoleOutput("      add                 The equivalent of doing 'git add .'");
		Tebas.consoleOutput("      commit <message>    The equivalent of doing 'git commit'");
		Tebas.consoleOutput("      remote              Manages git remotes. This has more options:");
		Tebas.consoleOutput("             set <name> <URL>         Sets or creates a remote");
		Tebas.consoleOutput("             delete <name>            Deletes a remote");
		Tebas.consoleOutput("             rename <name> <newname>  Deletes a remote");
		Tebas.consoleOutput("             list                     Shows a list of all remotes");
		Tebas.consoleOutput("      init <template>     Creates a new project directly in the local folder. Useful for creating projects outside of channels");
		Tebas.consoleOutput("      info                Shows info on the current project");
		Tebas.consoleOutput("      stats               Shows stats on the current project");
		Tebas.consoleOutput("      <script> [args]     Attempts to run a script of the current template");
	}
	
	//utilitie
	static bool determineIfEnoughLength(int n, int l){
		if(clp + n - 1 >= l){
			return false;
		}
		return true;
	}
	
	static void waitAnyKey(){
		if(Tebas.isConsoleInteractive()){
			Console.WriteLine("Press any key to continue");
			Console.ReadKey();
		}
	}
}