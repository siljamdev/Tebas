using System.Text;
using System.Diagnostics;
using AshLib.AshFiles;

static class CommandLineHandler{
	static int Main(string[] args){
		try{
			Tebas.initCore();
		}catch(Exception e){
			Tebas.reportAlways(e.ToString());
			return 11;
		}
		
		int n = 0;
		
		//Update
		if(args.Length == 2 && args[0] == "-u1"){
			try{
				waitUntilFileUnlocked(args[1]);
				
				string o = Environment.ProcessPath;
				File.Copy(o, args[1], true);
				
				Process.Start(args[1], "-u2 \"" + o + "\"");
				Environment.Exit(0);
				return 0;
			}catch(Exception e){
				Tebas.reportAlways(e.ToString());
				return 50;
			}
		}
		
		//Update
		if(args.Length == 2 && args[0] == "-u2"){
			try{
				waitUntilFileUnlocked(args[1]);
				
				File.Delete(args[1]);
				
				return 0;
			}catch(Exception e){
				Tebas.reportAlways(e.ToString());
				return 50;
			}
		}
		
		//Version, help, double clicking
		if(args.Length == 1){
			switch(args[0].ToLower()){
				case "--help":
				case "-h":
					printHelp();
					return 0;
				
				case "--version":
				case "-v":
					printVersion();
					return 0;
			}
			
			int doAction(Func<string, int> act){
				try{
					Tebas.initSecond();
				}catch(Exception e){
					Tebas.reportAlways(e.ToString());
					return 12;
				}
				
				try{
					return act(args[0]);
				}catch(Exception e){
					Tebas.reportAlways(e.ToString());
					return 30;
				}
			}
			
			if(Path.GetExtension(args[0]).ToLower() == ".tbtem"){
				return doAction(path => Template.installLocal(path) ? 0 : 25);
			}else if(Path.GetExtension(args[0]).ToLower() == ".tbplg"){
				return doAction(path => Plugin.installLocal(path) ? 0 : 25);
			}else if(Path.GetExtension(args[0]).ToLower() == ".tebas" && File.Exists(args[0])){
				return doAction(path => {
					string dir = Path.GetDirectoryName(path);
					Project p = Project.get(dir);
					if(p == null){
						Tebas.report("There is no project in '" + dir + "'");
						return 21;
					}
					p.info();
					return 0;
				});
			}
		}
		
		string pluginName = null;
		string pluginScript = null;
		
		bool flags = true;
		
		//Flags
		while(flags && n < args.Length){
			switch(args[n].ToLower()){
				case "--quiet":
				case "-q":
					if(Tebas.quiet){
						Tebas.reportAlways("Quiet already specified");
					}else{
						Tebas.quiet = true;
					}
				break;
				
				case "--forced":
				case "-f":
					if(Tebas.forced){
						Tebas.reportAlways("Forced already specified");
					}else{
						Tebas.forced = true;
					}
				break;
				
				case "--no-hints":
				case "-nh":
					if(Tebas.noHints){
						Tebas.reportAlways("No hints already specified");
					}else{
						Tebas.noHints = true;
					}
				break;
				
				case "--help":
				case "-h":
				case "--version":
				case "-v":
					Tebas.reportAlways("Help or version flags are expected to be the first and only ones");
					return 2;
				break;
				
				default:
					flags = false;
					continue; //Avoid n++
				break;
			}
			
			n++;
		}
		
		//No args
		if(n >= args.Length){
			printHelp();
			return 0;
		}
		
		//Commands
		try{
			return getRoot().handle(args.Skip(n).ToArray(), Tebas.reportAlways,
			() => {
				Console.OutputEncoding = Encoding.UTF8; //Russian chars didnt work
				try{
					Tebas.initSecond();
					return 0;
				}catch(Exception e){
					Tebas.reportAlways(e.ToString());
					return 12;
				}
			});
		}catch(Exception e){
			Tebas.reportAlways(e.ToString());
			return 30;
		}
	}
	
	static CLINode getRoot(){
		CLINode root = new CLINode("tebas");
		
		root.setArgs("script").setOptionalArgs().setAction(args => {
			Project p = Tebas.tryGetLocal();
			if(p == null){
				Plugin t = Plugin.get(args[0]);
				if(t == null){
					Tebas.report("There is no local project in this folder, and plugin '" + args[0] + "' is not installed. Use -h to see a list of commands if you meant something else");
					return 21;
				}
				if(args.Length < 2){
					Tebas.report("Too few arguments. At least 1 extra ('script') was expected after 'tebas <plugin>'");
					return 1;
				}
				if(!t.tryRunGlobal(args[1], args.Skip(2), true)){
					Tebas.report("Unknown plugin script or global");
					return 23;
				}
				return 0;
			}
			if(!p.tryRunScriptOrGlobal(args[0], args.Skip(1), true)){
				Plugin t = Plugin.get(args[0]);
				if(t == null){
					Tebas.report("Unknown template script or global, and plugin '" + args[0] + "' is not installed. Use -h to see a list of commands if you meant something else");
					return 23;
				}
				if(args.Length < 2){
					Tebas.report("Too few arguments. At least 1 extra ('script') was expected after 'tebas <plugin>'");
					return 1;
				}
				if(!p.tryRunPluginScriptOrGlobal(t, args[1], args.Skip(2), true)){
					Tebas.report("Unknown plugin script or global");
					return 23;
				}
				return 0;
			}
			return 0;
		}).hideHelp();
		
		root.chain("projects").setAction(args => {
			Project.list();
			return 0;
		}).setDescription("See a list of all recorded projects");
		
		#region template
		root.chain("template").chain("list").setAction(args => {
			Template.list();
			return 0;
		}).setDescription("See a list of installed templates");
		
		root.chain("template").chain("info").setArgs("name").setAction(args => {
			Template t = Template.get(args[0]);
			if(t == null){
				Tebas.report("The template '" + args[0] + "' is not installed");
				return 22;
			}
			t.info();
			return 0;
		}).setDescription("Get info on a template");
		
		root.chain("template").chain("install").setArgs("path").setAction(args => {
			if(!File.Exists(args[0])){
				Tebas.report("That file does not exist: '" + args[0] + "'");
				return 25;
			}
			AshFile t = new AshFile(args[0]);
			return Template.install(t) ? 0 : 25;
		}).setDescription("Install a template from a file");
		
		root.chain("template").chain("uninstall").setArgs("name").setAction(args => {
			Template t = Template.get(args[0]);
			if(t == null){
				Tebas.report("The template '" + args[0] + "' is not installed");
				return 22;
			}
			return t.uninstall() ? 0 : 25;
		}).setDescription("Uninstall a template");
		
		root.chain("template").chain("permission").setAction(args => {
			Tebas.output("Permission key list:");
			foreach((string p, string desc) in Tebas.validPermissions){
				Tebas.output("  " + p + ": " + desc);
			}
			return 0;
		}).setDescription("See a list of valid permission keys");
		
		root.chain("template").chain("permission").chain("set").setArgs("name", "key", "allow|disallow").setAction(args => {
			Template t = Template.get(args[0]);
			if(t == null){
				Tebas.report("The template '" + args[0] + "' is not installed");
				return 22;
			}
			if(args[2].ToUpper() != "ALLOW" && args[2].ToUpper() != "DISALLOW"){
				Tebas.reportAlways("Please use 'allow' or 'disallow' for value");
				return 8;
			}
			return t.setPermission(args[1], args[2].ToUpper() == "ALLOW") ? 0 : 24;
		}).setDescription("Set a permission for a template");
		
		root.chain("template").chain("permission").chain("reset").setArgs("name").setAction(args => {
			Template t = Template.get(args[0]);
			if(t == null){
				Tebas.report("The template '" + args[0] + "' is not installed");
				return 22;
			}
			
			t.resetPermissions();
			return 0;
		}).setDescription("Reset all permissions for a template");
		
		root.chain("template").chain("build").setArgs("directory").setAction(args => {
			return Template.build(args[0], args[0]) ? 0 : 25;
		}).setDescription("Build a template from source from a directory");
		
		root.chain("template").chain("global").setArgs("name", "script").setOptionalArgs().setAction(args => {
			Template t = Template.get(args[0]);
			if(t == null){
				Tebas.report("The template '" + args[0] + "' is not installed");
				return 22;
			}
			if(!t.tryRunGlobal(args[1], args.Skip(2), true)){
				Tebas.report("Unknown template global");
				return 23;
			}
			return 0;
		}).setDescription("Run a global script");
		#endregion
		
		#region plugin
		root.chain("plugin").chain("list").setAction(args => {
			Plugin.list();
			return 0;
		}).setDescription("See a list of installed plugins");
		
		root.chain("plugin").chain("info").setArgs("name").setAction(args => {
			Plugin t = Plugin.get(args[0]);
			if(t == null){
				Tebas.report("The plugin '" + args[0] + "' is not installed");
				return 22;
			}
			t.info();
			return 0;
		}).setDescription("Get info on a plugin");
		
		root.chain("plugin").chain("install").setArgs("path").setAction(args => {
			if(!File.Exists(args[0])){
				Tebas.report("That file does not exist: '" + args[0] + "'");
				return 25;
			}
			AshFile t = new AshFile(args[0]);
			return Plugin.install(t) ? 0 : 25;
		}).setDescription("Install a plugin from a file");
		
		root.chain("plugin").chain("uninstall").setArgs("name").setAction(args => {
			Plugin t = Plugin.get(args[0]);
			if(t == null){
				Tebas.report("The plugin '" + args[0] + "' is not installed");
				return 22;
			}
			return t.uninstall() ? 0 : 25;
		}).setDescription("Uninstall a plugin");
		
		root.chain("plugin").chain("permission").setAction(args => {
			Tebas.output("Permission key list:");
			foreach((string p, string desc) in Tebas.validPermissions){
				Tebas.output("  " + p + ": " + desc);
			}
			return 0;
		}).setDescription("See a list of valid permission keys");
		
		root.chain("plugin").chain("permission").chain("set").setArgs("name", "key", "allow|disallow").setAction(args => {
			Plugin t = Plugin.get(args[0]);
			if(t == null){
				Tebas.report("The plugin '" + args[0] + "' is not installed");
				return 22;
			}
			if(args[2].ToUpper() != "ALLOW" && args[2].ToUpper() != "DISALLOW"){
				Tebas.reportAlways("Please use 'allow' or 'disallow' for value");
				return 8;
			}
			return t.setPermission(args[1], args[2].ToUpper() == "ALLOW") ? 0 : 24;
		}).setDescription("Set a permission for a plugin");
		
		root.chain("plugin").chain("permission").chain("reset").setArgs("name").setAction(args => {
			Plugin t = Plugin.get(args[0]);
			if(t == null){
				Tebas.report("The plugin '" + args[0] + "' is not installed");
				return 22;
			}
			
			t.resetPermissions();
			return 0;
		}).setDescription("Reset all permissions for a plugin");
		
		root.chain("plugin").chain("build").setArgs("directory").setAction(args => {
			return Plugin.build(args[0], args[0]) ? 0 : 25;
		}).setDescription("Build a plugin from source from a directory");
		
		root.chain("plugin").chain("global").setArgs("name", "script").setOptionalArgs().setAction(args => {
			Plugin t = Plugin.get(args[0]);
			if(t == null){
				Tebas.report("The plugin '" + args[0] + "' is not installed");
				return 22;
			}
			if(!t.tryRunGlobal(args[1], args.Skip(2), true)){
				Tebas.report("Unknown plugin global");
				return 23;
			}
			return 0;
		}).setDescription("Run a global script");
		
		root.chain("plugin").chain("run").setArgs("name", "script").setOptionalArgs().setAction(args => {
			Project p = Tebas.getLocal();
			if(p == null){
				return 21;
			}
			
			Plugin t = Plugin.get(args[0]);
			if(t == null){
				Tebas.report("The plugin '" + args[0] + "' is not installed");
				return 22;
			}
			
			if(!p.tryRunPluginScript(t, args[1], args.Skip(2))){
				Tebas.report("Unknown plugin script");
				return 23;
			}
			return 0;
		}).setDescription("Run a plugin script locally");
		#endregion
		
		root.chain("init").setArgs("template").setAction(args => {
			Template t = Template.get(args[0]);
			if(t == null){
				Tebas.report("The template '" + args[0] + "' is not installed");
				return 22;
			}
			return Project.create(Directory.GetCurrentDirectory(), t) ? 0 : 26;
		}).setDescription("Initialize a new project in the current directory");
		
		root.chain("info").setAction(args => {
			Project p = Tebas.getLocal();
			if(p == null){
				return 21;
			}
			p.info();
			return 0;
		}).setDescription("Get info on the local project");
		
		#region config
		root.chain("config").setAction(args => {
			Tebas.listConfig();
			return 0;
		}).setDescription("See a list of config options");
		
		root.chain("config").chain("set").setArgs("key", "value").setAction(args => {
			return Tebas.setConfig(args[0], args[1]) ? 0 : 24;
		}).setDescription("Set config values");
		
		root.chain("config").chain("see").setAction(args => {
			Tebas.seeConfig();
			return 0;
		}).setDescription("See current config values");
		
		root.chain("config").chain("reset").setAction(args => {
			Tebas.resetConfig();
			return 0;
		}).setDescription("Reset the config");
		#endregion
		
		root.chain("update").setAction(args => {
			return Tebas.update() ? 0 : 50;
		}).setDescription("Attempt to update Tebas");
		
		root.chain("cleanup").setAction(args => {
			Tebas.cleanupAll();
			return 0;
		}).setDescription("Cleanup and update everything");
		
		return root;
	}
	
	static void printVersion(){
		Console.WriteLine("Tebas project manager, created by Siljam");
		Console.WriteLine("  Version: v" + BuildInfo.Version);
		Console.WriteLine("  Built on: " + BuildInfo.BuildDate);
		Console.WriteLine("  GitHub repo: " + BuildInfo.RepoUrl);
	}
	
	static void printHelp(){
		Console.WriteLine("Tebas CLI help");
		Console.WriteLine();
		Console.WriteLine("Usage: tebas [flags] <command>");
		Console.WriteLine();
		Console.WriteLine("Flags:");
		Console.WriteLine("  -q");
		Console.WriteLine("  --quiet       Show no output");
		Console.WriteLine("  -f");
		Console.WriteLine("  --forced      Skip confirmations");
		Console.WriteLine("  -nh");
		Console.WriteLine("  --no-hints    Show no hints");
		Console.WriteLine("  -v");
		Console.WriteLine("  --version     Show current version");
		Console.WriteLine("  -h");
		Console.WriteLine("  --help        Show help");
		Console.WriteLine();
		Console.WriteLine("Commands:");
		Console.Write(getRoot().help());
		Console.WriteLine("      <script> [args]    Run a template script or global locally");
		Console.WriteLine("      <plugin> <script> [args]    Run a plugin script or global locally");
	}
	
	static void waitUntilFileUnlocked(string path){
		while(true){
			try{
				using(File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None)){
					break;
				}
			}catch{
				Thread.Sleep(100);
			}
		}
	}
}

//Lil helper class
class CLINode{
	public string command {get;}
	public int extraArgs => extraArgsNames.Length;
	public string[] extraArgsNames;
	public string description {get; private set;} 
	
	public bool optionalArgs {get; private set;}
	
	public List<CLINode> children {get;} = new();
	public Func<string[], int>? action;
	
	public bool showHelp = true;
	
	public CLINode(string n){
		command = n;
		extraArgsNames = Array.Empty<string>();
	}
	
	public CLINode setArgs(params string[] n){
		if(n != null){
			extraArgsNames = n;
		}
		
		return this;
	}
	
	public CLINode setOptionalArgs(){
		optionalArgs = true;
		
		return this;
	}
	
	public CLINode chain(string n){
		CLINode f = children.Find(h => h.command == n);
		if(f != null){
			return f;
		}
		
		CLINode c = new CLINode(n);
		children.Add(c);
		return c;
	}
	
	public CLINode setAction(Func<string[], int>? f){
		action = f;
		
		return this;
	}
	
	public CLINode setDescription(string d){
		description = d;
		
		return this;
	}
	
	public CLINode hideHelp(){
		showHelp = false;
		return this;
	}
	
	public string help(int indent = 0, int len = 0){
		string tab = new string(' ', indent);
		StringBuilder sb = new();
		sb.Append(tab);
		sb.Append(command);
		if(showHelp && action != null){
			foreach(string n in extraArgsNames){
				sb.Append(" <" + n + ">");
			}
			
			if(optionalArgs){
				sb.Append(" [args]");
			}
			
			if(description != null){
				sb.Append(new string(' ', Math.Max(0, len - helpLen())) + "    " + description);
			}
		}
		
		sb.Append(Environment.NewLine);
		
		int l = 0;
		foreach(CLINode c in children){
			l = Math.Max(l, c.helpLen());
		}
		
		foreach(CLINode c in children){
			sb.Append(c.help(indent + 6, l));
		}
		
		return sb.ToString();
	}
	
	int helpLen(){
		if(!showHelp){
			return command.Length;
		}
		return command.Length + (action == null ? 0 : extraArgsNames.Select(h => h.Length + 3).Sum() + (optionalArgs ? 7 : 0));
	}
	
	public int handle(string[] args, Action<string> report, Func<int>? onMatch){
		if(args.Length == 0){
			if(action != null){
				if(extraArgs == 0){
					int m1 = onMatch?.Invoke() ?? 0;
					if(m1 != 0){
						return m1;
					}
					return action?.Invoke(Array.Empty<string>()) ?? 0;
				}else{
					report(extraArgs + " extra arguments(" + string.Join(", ", extraArgsNames.Select(h => "'" + h + "'")) + ") were expected after '" + command + "'");
					return 1;
				}
			}else{
				report("Expected another command after '" + command + "'");
				return 1;
			}
		}
		
		CLINode c = children.Find(h => h.command == args[0]);
		if(c != null){
			return c.handle(args.Skip(1).ToArray(), report, onMatch);
		}else if(action != null){
			if(args.Length == extraArgs || (args.Length > extraArgs && optionalArgs)){
				int m1 = onMatch?.Invoke() ?? 0;
				if(m1 != 0){
					return m1;
				}
				return action?.Invoke(args) ?? 0;
			}else if(args.Length < extraArgs){
				report("Too few arguments. At least " + (extraArgs - args.Length) + " extra (" + string.Join(", ", extraArgsNames.Skip(args.Length).Select(h => "'" + h + "'")) + ") were expected after '" + command + "'");
				return 1;
			}else{
				if(extraArgs == 0){
					report("Too many arguments. No more were expected after '" + command + "'");
				}else{
					report("Too many arguments. Only " + extraArgs + " were expected after '" + command + "'");
				}
				return 1;
			}
		}else{
			report("Unknown command: '" + args[0] + "'. Use -h to see a list of commands");
			return 2;
		}
	}
}