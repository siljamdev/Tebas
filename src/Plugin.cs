using System.Text;
using AshLib.Dates;
using AshLib.AshFiles;
using TabScript;

class Plugin{
	#region static
	static string directory => Tebas.dep.path + "/plugins/";
	
	static readonly List<Plugin> cached = new();
	
	static readonly HashSet<string> forbiddenGlobals = new(){
		"install",
		"uninstall",
		"cleanup",
		"info",
	};
	
	public static void cleanup(){
		string[] directories = Directory.GetDirectories(directory);
		
		foreach(string d in directories){
			if(!File.Exists(d + "/p.tbplg")){
				Directory.Delete(d, true);
			}
		}
	}
	
	public static bool installed(string name){
		return File.Exists(directory + name + "/p.tbplg");
	}
	
	public static string[] getAllNames(){
		string[] directories = Directory.GetDirectories(directory);
		
		return directories.Where(d => File.Exists(d + "/p.tbplg")).Select(d => Path.GetFileName(d)).ToArray();
	}
	
	public static Plugin[] getAll(){
		return getAllNames().Select(n => get(n)).Where(t => t != null).ToArray();
	}
	
	public static Plugin get(string name){
		Plugin c = cached.Find(t => t.name == name);
		if(c != null){
			return c;
		}
		
		if(!installed(name)){
			return null;
		}
		
		Plugin n = new Plugin(name);
		cached.Add(n);
		return n;
	}
	
	public static void list(){
		string[] all = getAllNames();
		
		if(all.Length == 0){
			Tebas.output(all.Length + " plugins installed");
		}else{
			Tebas.output(all.Length + " plugins installed:");
		}
		
		foreach(string t in all){
			Tebas.output("  " + t);
		}
	}
	
	public static bool installLocal(string path){
		if(!File.Exists(path)){
			Tebas.report("File does not exist: '" + path + "'");
			return false;
		}
		AshFile t = new AshFile(path);
		
		return install(t);
	}
	
	//Return false on error
	public static bool install(AshFile t){
		string name;
		if(!t.TryGetValue("name", out name)){
			Tebas.report("Incorrectly formatted plugin file, missing name");
			return false;
		}
		
		if(!isValidPluginName(name)){
			return false;
		}
		
		string author = null;
		if(t.TryGetValue("author", out string auth)){
			author = auth;
			Tebas.output("Plugin author: " + auth);
		}
		
		if(t.TryGetValue("description", out string desc)){
			Tebas.output("Plugin description: " + desc);
		}
		
		if(t.TryGetValue("version", out string ver)){			
			int c = Tebas.compareVersion(ver);
			
			if(c == -1){
				Tebas.output("Tebas version plugin was built for: " + ver);
				
				if(!Tebas.askConfirmationAllowForced("The version this plugin was built for is older. Do you want to proceed?")){
					return true;
				}
			}else if(c == 1){
				Tebas.output("Tebas version plugin was built for: " + ver);
				Tebas.report("Tebas version plugin was built for is newer. Please update your client (or check for time travellers)");
				return false;
			}
		}
		
		Plugin previous = null;
		
		if(installed(name)){
			if(!Tebas.askConfirmationAllowForced("A plugin called '" + name + "' is already installed, do you want to update it?")){
				return true;
			}
			previous = get(name);
		}
		
		//Continue
		
		Directory.CreateDirectory(directory + name);
		t.path = directory + name + "/p.tbplg";
		
		bool keepResources = t.GetValue<bool>("keepResources");
		
		foreach(KeyValuePair<string, object> keyVal in t.Where(kvp => !(kvp.Key == "author" || kvp.Key == "description"
			|| (kvp.Key.StartsWith("resources.") && kvp.Value is string)
			|| (kvp.Key.StartsWith("scripts.") && kvp.Value is string)
			|| (kvp.Key.StartsWith("globals.") && kvp.Value is string)
			|| (kvp.Key.StartsWith("utils.") && kvp.Value is string))).ToList()){
			t.Remove(keyVal.Key);
		}
		
		//Keep permissions
		if(previous != null && previous.getAuthor() == author){
			foreach(KeyValuePair<string, object> keyVal in previous.file.Where(kvp => kvp.Key.StartsWith("permissions.") && kvp.Value is bool b && b).ToList()){
				t.Set(keyVal.Key, keyVal.Value);
			}
		}
		
		//Keep resources
		if(previous != null && previous.getAuthor() == author && keepResources){
			foreach(KeyValuePair<string, object> keyVal in previous.file.Where(kvp => kvp.Key.StartsWith("resources.") && kvp.Value is string).ToList()){
				t.Set(keyVal.Key, keyVal.Value);
			}
		}
		
		t.Save();
		
		cached.RemoveAll(p => p.name == name);
		
		Plugin plg = get(name);
		if(plg != null){
			plg.tryRunGlobal("install");
			return true;
		}
		return false;
	}
	
	public static bool build(string path, string outPath){
		if(!buildGetFile(path, "name.txt", out string name)){
			Tebas.report("'name.txt' missing");
			return false;
		}
		
		if(!isValidPluginName(name)){
			return false;
		}
		
		AshFile t = new AshFile(Path.Combine(outPath, name + ".tbplg"));
		
		t.Set("name", name);
		t.Set("version", BuildInfo.Version);
		
		if(buildGetFile(path, "author.txt", out string auth)){
			t.Set("author", auth.Trim());
		}
		
		if(buildGetFile(path, "description.txt", out string desc)){
			t.Set("description", desc.Trim());
		}
		
		if(File.Exists(path + "/KEEPRESOURCES")){
			t.Set("keepResources", true);
		}
		
		bool hadError = false;
		
		Dictionary<string, ResolvedImport> imports = new();
		
		//Globals
		if(Directory.Exists(path + "/globals")){
			string[] scripts = Directory.GetFiles(path + "/globals", "*.tbs", SearchOption.TopDirectoryOnly);
			
			foreach(string s in scripts){
				string n = Path.GetFileNameWithoutExtension(s);
				if(!Tebas.isValidScriptName(n)){
					hadError = true;
					continue;
				}
				
				string code = File.ReadAllText(s);
				
				try{
					ResolvedImport r = TableScript.SourceAsImport("plugins/BUILD/globals/" + n, code, Tebas.pluginReport);
					
					t.Set("globals." + n, r.Optimize().ToCompactString());
					imports["globals." + n] = r;
				}catch(TabScriptException x){
					hadError = true;
					continue;
				}
			}
		}
		
		//Scripts
		if(Directory.Exists(path + "/scripts")){
			string[] scripts = Directory.GetFiles(path + "/scripts", "*.tbs", SearchOption.TopDirectoryOnly);
			
			foreach(string s in scripts){
				string n = Path.GetFileNameWithoutExtension(s);
				if(!Tebas.isValidScriptName(n)){
					hadError = true;
					continue;
				}
				
				string code = File.ReadAllText(s);
				
				try{
					ResolvedImport r = TableScript.SourceAsImport("plugins/BUILD/scripts/" + n, code, Tebas.pluginReport);
					
					t.Set("scripts." + n, r.Optimize().ToCompactString());
					imports["scripts." + n] = r;
				}catch(TabScriptException x){
					hadError = true;
					continue;
				}
			}
		}
		
		//Utils
		if(Directory.Exists(path + "/utils")){
			string[] scripts = Directory.GetFiles(path + "/utils", "*.tbs", SearchOption.TopDirectoryOnly);
			
			foreach(string s in scripts){
				string n = Path.GetFileNameWithoutExtension(s);
				if(!Tebas.isValidScriptName(n)){
					hadError = true;
					continue;
				}
				
				string code = File.ReadAllText(s);
				
				try{
					ResolvedImport r = TableScript.SourceAsImport("plugins/BUILD/utils/" + n, code, Tebas.pluginReport);
					
					t.Set("utils." + n, r.Optimize().ToCompactString());
					imports["utils." + n] = r;
				}catch(TabScriptException x){
					hadError = true;
					continue;
				}
			}
		}
		
		//Globals
		PluginDummyImportResolver gres = new(imports);
		foreach(ResolvedImport r in imports.Where(kvp => kvp.Key.StartsWith("globals.")).Select(kvp => kvp.Value)){
			try{
				TableScript s = TableScript.FromImport(r, gres, Tebas.pluginReport);
			}catch(TabScriptException x){
				hadError = true;
			}
		}
		
		//Scripts
		PluginScriptDummyImportResolver sres = new(imports);
		foreach(ResolvedImport r in imports.Where(kvp => kvp.Key.StartsWith("scripts.")).Select(kvp => kvp.Value)){
			try{
				TableScript s = TableScript.FromImport(r, sres, Tebas.pluginReport);
			}catch(TabScriptException x){
				hadError = true;
			}
		}
		
		if(hadError){
			return false;
		}
		
		if(Directory.Exists(path + "/resources")){
			string[] res = Directory.GetFiles(path + "/resources", "*", SearchOption.TopDirectoryOnly);
			
			foreach(string s in res){
				string n = Path.GetFileNameWithoutExtension(s);
				t.Set("resources." + n, File.ReadAllText(s));
			}
		}
		
		string parent = Path.GetDirectoryName(t.path);
		if(parent != null){
			Directory.CreateDirectory(parent);
		}
		
		t.Save();
		Tebas.output("Built plugin saved to '" + t.path + "'");
		return true;
	}
	
	static bool buildGetFile(string path, string file, out string content){
		string p = path + "/" + file;
		if(File.Exists(p)){
			content = File.ReadAllText(p);
			return true;
		}
		
		content = null;
		return false;
	}
	
	static bool isValidPluginName(string n){
		if(string.IsNullOrWhiteSpace(n)){
			Tebas.report("A plugin name cannot contain whitespace or be empty: '" + n + "'");
			return false;
		}
		
		if(n.Any(c => !char.IsLetterOrDigit(c))){
			Tebas.report("A plugin name can only contain letters and numbers: '" + n + "'");
			return false;
		}
		
		return true;
	}
	#endregion
	
	public string name {get;}
	public string path => directory + name;
	public string filePath => directory + name + "/p.tbplg";
	
	Dictionary<string, TableScript> cachedGlobals = new();
	Dictionary<string, ResolvedImport> cachedGlobalsImports = new();
	Dictionary<string, ResolvedImport> cachedScriptsImports = new();
	Dictionary<string, ResolvedImport> cachedUtilsImports = new();
	
	AshFile file;
	
	public TebasPluginImportGenerator importGenerator {get;}
	public TebasImportGenerator tebasImportGenerator {get;}
	PluginImportResolver globalsImportResolver;
	
	private Plugin(string name){
		this.name = name;
		
		file = new AshFile(filePath);
		
		file.Save();
		
		importGenerator = new TebasPluginImportGenerator(this);
		tebasImportGenerator = new TebasImportGenerator(true, this.name);
		
		globalsImportResolver = new PluginImportResolver(this);
	}
	
	public string getAuthor(){
		return file.GetValue<string>("author");
	}
	
	public string getDescription(){
		return file.GetValue<string>("description");
	}
	
	public void info(){
		Tebas.output("Plugin name: " + name);
		
		if(file.TryGetValue("author", out string auth)){
			Tebas.output("Plugin author: " + auth);
		}
		
		if(file.TryGetValue("description", out string desc)){
			Tebas.output("Plugin description: " + desc);
		}
		
		string[] scripts = getAllScriptNames();
		if(scripts.Length == 0){
			Tebas.output("This plugin has no scripts");
		}else{
			Tebas.output(scripts.Length + " scripts:");
			
			foreach(string s in scripts){
				Tebas.output("  " + s);
			}
		}
		
		string[] globals = getAllGlobalNames();
		if(globals.Length == 0){
			Tebas.output("This plugin has no global scripts");
		}else{
			Tebas.output(globals.Length + " global scripts:");
			
			foreach(string s in globals){
				Tebas.output("  " + s);
			}
		}
		
		tryRunGlobal("info");
	}
	
	public string[] getAllScriptNames(){
		return file.Where(kvp => kvp.Key.StartsWith("scripts.") && kvp.Value is string).Select(kvp => kvp.Key.Substring(8)).ToArray();
	}
	
	public string[] getAllGlobalNames(){
		return file.Where(kvp => kvp.Key.StartsWith("globals.") && kvp.Value is string).Select(kvp => kvp.Key.Substring(8)).ToArray();
	}
	
	public bool tryRunGlobal(string name, IEnumerable<string> args = null, bool checkForForbidden = false){
		if(checkForForbidden && forbiddenGlobals.Contains(name)){
			return false;
		}
		
		if(cachedGlobals.TryGetValue(name, out TableScript c) && c != null){
			c.Run(args);
			
			return true;
		}else{
			try{
				ResolvedImport r = getGlobalAsImport(name);
				if(r == null){
					return false;
				}
				TableScript g = TableScript.FromImport(r, globalsImportResolver, Tebas.pluginReport);
				cachedGlobals[name] = g;
				
				g.Run(args);
				
				return true;
			}catch(TabScriptException x){
				Tebas.pluginReport(x);
			}
		}
		return false;
	}
	
	public ResolvedImport? getScriptAsImport(string name){
		if(cachedScriptsImports.TryGetValue(name, out ResolvedImport c) && c != null){
			return c;
		}else if(file.TryGetValue("scripts." + name, out string code)){
			try{
				ResolvedImport r = TableScript.SourceAsImport("plugins/" + this.name + "/scripts/" + name, code, Tebas.pluginReport);
				cachedScriptsImports[name] = r;
				
				return r;
			}catch(TabScriptException x){
				
			}
		}
		return null;
	}
	
	public ResolvedImport? getGlobalAsImport(string name){
		if(cachedGlobalsImports.TryGetValue(name, out ResolvedImport c) && c != null){
			return c;
		}else if(file.TryGetValue("globals." + name, out string code)){
			try{
				ResolvedImport r = TableScript.SourceAsImport("plugins/" + this.name + "/globals/" + name, code, Tebas.pluginReport);
				cachedGlobalsImports[name] = r;
				
				return r;
			}catch(TabScriptException x){
				
			}
		}
		return null;
	}
	
	public ResolvedImport? getUtilAsImport(string name){
		if(cachedUtilsImports.TryGetValue(name, out ResolvedImport c) && c != null){
			return c;
		}else if(file.TryGetValue("utils." + name, out string code)){
			try{
				ResolvedImport r = TableScript.SourceAsImport("plugins/" + this.name + "/utils/" + name, code, Tebas.pluginReport);
				cachedUtilsImports[name] = r;
				
				return r;
			}catch(TabScriptException x){
				
			}
		}
		return null;
	}
	
	public string getResource(string key){
		if(file.TryGetValue("resources." + key, out string s)){
			return s;
		}
		return null;
	}
	
	public void setResource(string key, string value){
		if(string.IsNullOrEmpty(value)){
			file.Remove("resources." + key);
		}else{
			file.Set("resources." + key, value);
		}
		file.Save();
	}
	
	public void appendResource(string key, string value){
		if(string.IsNullOrEmpty(value)){
			return;
		}
		
		if(file.TryGetValue("resources." + key, out string s)){
			file.Set("resources." + key, s + value);
		}else{
			file.Set("resources." + key, value);
		}
		file.Save();
	}
	
	public bool hasPermission(string key){
		if(Tebas.validPermissions.Any(t => t.key == key)){
			return file.GetValue<bool>("permissions." + key);
		}
		
		return false;
	}
	
	public bool setPermission(string key, bool value){
		if(Tebas.validPermissions.Any(t => t.key == key)){
			file.Set("permissions." + key, value);
			file.Save();
			return true;
		}else{
			Tebas.report("Unknown permission key");
			Tebas.hint("Do 'tebas plugin permission' to see the full list");
		}
		
		return false;
	}
	
	public void resetPermissions(){
		foreach(string key in file.Keys.Where(k => k.StartsWith("permissions.")).ToList()){
			file.Remove(key);
		}
		
		file.Save();
	}
	
	//false if error
	public bool uninstall(){
		if(!Tebas.askConfirmationAllowForced("Do you want to uninstall the plugin '" + name + "'?")){
			return true;
		}
		
		tryRunGlobal("uninstall");
		Directory.Delete(path, true);
		cached.Remove(this);
		
		return true;
	}
	
	public void cleanupInstance(){
		foreach(KeyValuePair<string, object> keyVal in file.Where(kvp => !(kvp.Key == "author" || kvp.Key == "description"
			|| (kvp.Key.StartsWith("resources.") && kvp.Value is string)
			|| (kvp.Key.StartsWith("scripts.") && kvp.Value is string) || (kvp.Key.StartsWith("globals.") && kvp.Value is string) || (kvp.Key.StartsWith("utils.") && kvp.Value is string)
			|| (kvp.Key.StartsWith("permissions.") && Tebas.validPermissions.Any(t => t.key == kvp.Key.Substring(12)) && kvp.Value is bool b && b == true))).ToList()){
			file.Remove(keyVal.Key);
		}
		
		file.Save();
		
		tryRunGlobal("cleanup");
	}
}