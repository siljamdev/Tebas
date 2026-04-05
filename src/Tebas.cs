using System.Text.Json;
using System.Diagnostics;
using AshLib;
using AshLib.AshFiles;
using AshLib.Folders;
using AshLib.Formatting;
using TabScript;

static class Tebas{
	public static bool quiet = false;
	public static bool forced = false;
	public static bool noHints = false;
	
	public static Dependencies dep;
	public static AshFile config;
	
	public static void initCore(){
		string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		dep = new Dependencies(appDataPath + "/ashproject/tebasDEV", true, new string[]{"templates", "plugins"}, null);
		
		initConfig();
		
		Palette.init();
		
		//Console.WriteLine(fetchLatestVersion());
	}
	
	public static void initSecond(){
		Project.init();
		Template.init();
		SharedHandler.init();
	}
	
	public static readonly (string key, string description)[] validPermissions = new (string, string)[]{
		("skipProcessConfirmation", "Skip the confirmation to run a process"),
		("skipFileConfirmation", "Skip the confirmation to modify project files"),
		("skipInstallationConfirmation", "Skip the confirmation to install local templates or plugins"),
	};
	
	#region config
	//These appear in options
	public static readonly (string key, object value, string description)[] configurableOptions = new (string key, object value, string description)[]{
		("searchProjectInParent", true, "Search for local project file in parent directories recursively"),
		("useColors", true, "Use colored console output when possible"),
		("scriptShowLabel", true, "Show script name in its output"),
		("processShowLabel", true, "Show process label (its name) in its output"),
		("scriptAllowAllProcesses", false, "Allow all templates and plugins to execute whatever process without confirmation. DANGEROUS"),
		("scriptAllowAllFileOperations", false, "Allow all templates and plugins to modify project files without confirmation. DANGEROUS"),
	};
	
	//These do not appear in options
	static readonly AshFileModel nonConfigurableConfigModel = new AshFileModel(
		
	);
	
	static void initConfig(){
		config = dep.config;
		
		//Model
		AshFileModel afm = new AshFileModel(configurableOptions.Select(o => new ModelInstance(ModelInstanceOperation.Type, o.key, o.value)).ToArray());
		afm.Merge(nonConfigurableConfigModel);
		afm.deleteNotMentioned = true;
		
		config.ApplyModel(afm);
		
		//Set current version and path. Might be needed by someone (maybe)
		config.Set("version", BuildInfo.Version);
		try{ //Might not work on linux
			config.Set("path", Environment.ProcessPath);
		}catch{}
		
		config.Save();
		
		//Load config if needed goes here
	}
	
	public static void resetConfig(){
		AshFileModel r = new AshFileModel(configurableOptions.Select(h => new ModelInstance(ModelInstanceOperation.Value, h.key, h.value)).ToArray());
		config.ApplyModel(r);
		
		config.Save();
		
		//Load config if needed goes here
		
		output("Reset config");
	}
	
	public static void listConfig(){
		output("List of config options:");
		foreach((string key, object value, string description) in configurableOptions){
			output("  " + key + ": " + description);
		}
	}
	
	public static void seeConfig(){
		output("Current config values:");
		foreach((string key, _, _) in configurableOptions){
			output("  " + key + ": " + config.GetValue(key));
		}
	}
	
	public static bool setConfig(string key, string value){
		if(configurableOptions.Any(o => o.key == key)){
			Type t = config.GetValueType(key);
			if(t == typeof(bool)){
				if(!setConfigBool(key, value)){
					return false;
				}
			}else if(t == typeof(string)){
				config.Set(key, value);
			}else if(t == typeof(int)){
				if(!setConfigInt(key, value)){
					return false;
				}
			}else{
				report("Unknown option type");
				return false;
			}
			
			config.Save();
			return true;
		}else{
			report("Unknown config key");
			hint("Do 'config list' to see the full list");
			return false;
		}
	}
	
	static bool setConfigBool(string key, string value){
		value = value.ToUpper();
		if(value == "T" || value == "TRUE"){
			config.Set(key, true);
			return true;
		}else if(value == "F" || value == "FALSE"){
			config.Set(key, false);
			return true;
		}else{
			report("The value '" + value + "' must be either 'true' or 'false'");
			return false;
		}
	}
	
	static bool setConfigInt(string key, string value){
		if(!int.TryParse(value, out int i)){
			report("The value '" + value + "' is not a valid integer number");
			return false;
		}
		
		config.Set(key, i);
		return true;
	}
	#endregion
	
	public static Project getLocal(){
		Project l = tryGetLocal();
		if(l == null){
			report("There is no local project in this folder");
		}
		return l;
	}
	
	public static Project tryGetLocal(){
		string dir = Directory.GetCurrentDirectory();
		
		if(config.GetValue<bool>("searchProjectInParent")){
			while(dir != null){
				if(Project.exists(dir)){
					Directory.SetCurrentDirectory(dir); //set workingDir
					return Project.get(dir);
				}
				
				dir = Path.GetDirectoryName(dir);
			}
			
			return null;
		}else{
			return Project.get(dir);
		}
	}
	
	public static void cleanupAll(){
		Project.cleanup();
		foreach(Project p in Project.getAll()){
			p.cleanupInstance();
		}
		
		Template.cleanup();
		foreach(Template t in Template.getAll()){
			t.cleanupInstance();
		}
		
		Plugin.cleanup();
		foreach(Plugin pg in Plugin.getAll()){
			pg.cleanupInstance();
		}
		
		SharedHandler.cleanup();
	}
	
	public static int compareVersion(string vs){ //-1: older, 0: same, 1: newer
		string appVersion;
		string[] avp = BuildInfo.Version.Split("-");
		if(avp.Length == 3){
			appVersion = avp[0] + ".0." + avp[2];
		}else{
			appVersion = avp[0] + ".1";
		}
		
		if(appVersion.StartsWith("v")){ //Little correction
			appVersion = appVersion.Substring(1);
		}
		
		string[] vp = vs.Split("-");
		if(vp.Length == 3){
			vs = vp[0] + ".0." + vp[2];
		}else{
			vs = vp[0] + ".1";
		}
		
		if(vs.StartsWith("v")){ //Little correction
			vs = vs.Substring(1);
		}
		
		int[] c = appVersion.Split(".").Select(h => {
			if(int.TryParse(h, out int v)){
				return v;
			}
			return 0;
		}).ToArray();
		
		int[] n = vs.Split(".").Select(h => {
			if(int.TryParse(h, out int v)){
				return v;
			}
			return 0;
		}).ToArray();
		
		int i = 0;
		
		while(true){
			if(i >= c.Length && i >= n.Length){
				return 0; //Equal
			}
			
			int c2 = i < c.Length ? c[i] : 0;
			int n2 = i < n.Length ? n[i] : 0;
			
			if(n2 > c2){
				return 1; //Newer
			}
			if(n2 < c2){
				return -1; //Older
			}
			i++;
		}
		return 0;
	}
	
	static string fetchLatestVersion(){
		using var client = new HttpClient();
		client.DefaultRequestHeaders.UserAgent.ParseAdd("TebasFetchLatest");
		client.Timeout = TimeSpan.FromSeconds(15);
		
		try{
			//string url = "https://api.github.com/repos/siljamdev/Tebas/releases/latest";
			
			string repoPath = new Uri(BuildInfo.RepoUrl).AbsolutePath;
			string url = "https://api.github.com/repos" + repoPath.TrimEnd('/') + "/releases/latest";
			
			string json = client.GetStringAsync(url).GetAwaiter().GetResult();
			var doc = JsonDocument.Parse(json);
			string latestTag = doc.RootElement.GetProperty("tag_name").GetString()!;
			
			return latestTag;
		}catch(Exception e){
			report(e.ToString());
			return null;
		}
	}
	
	//Returns false if something goes wrong
	public static bool update(){
		string ver = fetchLatestVersion();
		if(ver == null){
			report("Impossible to fetch latest version");
			return false;
		}
		
		int c = compareVersion(ver);
		if(c == 0){
			output("Up to date");
			return true;
		}else if(c == -1){
			output("Your are ahead! Please, check for time travellers");
			return true;
		}
		
		try{
			string assetName = "tebas_" + BuildInfo.Runtime;
			string url = BuildInfo.RepoUrl.TrimEnd('/') + "/releases/latest/download/" + assetName;
			
			string currentExe = Environment.ProcessPath!;		
			string tempExe = Path.Combine(Path.GetDirectoryName(currentExe), Path.GetFileNameWithoutExtension(currentExe) + ".new" + Path.GetExtension(currentExe));
			
			using var client = new HttpClient();
			client.DefaultRequestHeaders.UserAgent.ParseAdd("TebasUpdate");
			
			using var stream = client.GetStreamAsync(url).GetAwaiter().GetResult();
			using var file = File.Create(tempExe);
			stream.CopyTo(file);
			
			if(!OperatingSystem.IsWindows()){
				File.SetUnixFileMode(tempExe, UnixFileMode.UserExecute | UnixFileMode.UserRead | UnixFileMode.UserWrite);
			}
			
			output("Updated succesfully");
			Process.Start(tempExe, "-u1 \"" + currentExe + "\"");
			Environment.Exit(0);
			return true;
		}catch(Exception e){
			report(e.ToString());
			return false;
		}
	}
	
	#region naming
	public static bool isValidScriptName(string n){
		if(string.IsNullOrWhiteSpace(n)){
			Tebas.report("A script name cannot contain whitespace or be empty: '" + n + "'");
			return false;
		}
		
		if(n.StartsWith("-")){
			Tebas.report("A script name cannot start with '-': '" + n + "'");
			return false;
		}
		
		if(n.Any(c => !char.IsLetterOrDigit(c) && c != '-')){
			Tebas.report("A script name can only contain letters, numbers and hyphens: '" + n + "'");
			return false;
		}
		
		return true;
	}
	#endregion
	
	#region console output
	//Normal
	public static void output(string e, CharFormat? f = null){
		if(!quiet){
			outputAlways(e, f);
		}
	}
	
	public static void outputAlways(string e, CharFormat? f = null){
		if(Palette.useColors && f != null){
			Console.WriteLine(new FormatString(e, f));
		}else{
			Console.WriteLine(e);
		}
	}
	
	public static void outputNoLineAlways(string e, CharFormat? f = null){
		if(Palette.useColors && f != null){
			Console.Write(new FormatString(e, f));
		}else{
			Console.Write(e);
		}
	}
	
	public static void report(string e, CharFormat? f = null){
		if(!quiet){
			reportAlways(e, f);
		}
	}
	
	public static void reportAlways(string e, CharFormat? f){
		if(Palette.useColors){
			f ??= Palette.error;
			Console.Error.WriteLine(new FormatString(e, f));
		}else{
			Console.Error.WriteLine(e);
		}
	}
	
	public static void reportAlways(string e){
		reportAlways(e, null);
	}
	
	//Labels
	public static void labelOutput(string label, CharFormat lf, string t, CharFormat? f = null){
		if(quiet){
			return;
		}
		
		if(Palette.useColors){
			f ??= CharFormat.ResetAll;
			Console.WriteLine(new FormatString(("[" + label + "] ", lf), (t, f)));
		}else{
			Console.WriteLine("[" + label + "] " + t);
		}
	}
	
	public static void labelOutputNoLineAlways(string label, CharFormat lf, string t, CharFormat? f = null){
		if(Palette.useColors){
			f ??= CharFormat.ResetAll;
			Console.Write(new FormatString(("[" + label + "] ", lf), (t, f)));
		}else{
			Console.Write("[" + label + "] " + t);
		}
	}
	
	public static void labelReport(string label, CharFormat lf, string t, CharFormat? f = null){
		if(quiet){
			return;
		}
		
		if(Palette.useColors){
			f ??= Palette.error;
			Console.Error.WriteLine(new FormatString(("[" + label + "] ", lf), (t, f)));
		}else{
			Console.Error.WriteLine("[" + label + "] " + t);
		}
	}
	
	//Scripts
	
	//Compilation
	public static void templateReport(TabScriptException x){
		labelReport("COMPILATION", Palette.template, x.ToShortString());
	}
	
	public static void pluginReport(TabScriptException x){
		labelReport("COMPILATION", Palette.plugin, x.ToShortString());
	}
	
	//Confirmations
	public static bool askConfirmationAllowForced(string question){
		if(forced){
			return true;
		}
		
		return askConfirmation(question);
	}
	
	public static bool askConfirmation(string question){
		while(true){
			outputNoLineAlways(question + " [Y/N] ", Palette.confirmation);
			string ans = Console.ReadLine().Trim().ToUpper();
			
			if(ans == "Y"){
				return true;
			}else if(ans == "N"){
				return false;
			}else{
				reportAlways("Please, write only 'Y' or 'N'");
			}
		}
	}
	
	//Hints
	public static void hint(string t){
		if(noHints){
			return;
		}
		
		output(t, Palette.warn);
	}
	#endregion
}