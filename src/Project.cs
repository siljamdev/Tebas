using System.Text;
using AshLib.Dates;
using AshLib.AshFiles;
using TabScript;

class Project{
	#region static
	static string projectsFilePath => Tebas.dep.path + "/projects.txt";
	static List<string> projects = new();
	
	static readonly List<Project> cached = new();
	
	public static void init(){
		if(!File.Exists(projectsFilePath)){
			File.Create(projectsFilePath).Dispose();
		}
		
		projects = File.ReadAllLines(projectsFilePath).ToList();
	}
	
	static void save(){
		File.WriteAllLines(projectsFilePath, projects);
	}
	
	//"fix" them
	public static void cleanup(){
		projects = projects.Distinct().Where(d => exists(d)).ToList();
		
		save();
	}
	
	public static string[] getAllDirectoryPaths(){
		return projects.ToArray();
	}
	
	public static Project[] getAll(){
		return projects.Select(d => get(d)).Where(p => p != null).ToArray();
	}
	
	public static bool exists(string dirPath){
		return File.Exists(dirPath + "/.tebas");
	}
	
	//Null if it doesnt exist
	public static Project get(string dirPath){
		Project c = cached.Find(p => p.path == dirPath);
		if(c != null){
			return c;
		}
		
		if(!exists(dirPath)){
			return null;
		}
		
		Project n = new Project(dirPath);
		cached.Add(n);
		
		if(!projects.Contains(dirPath)){
			projects.Add(dirPath);
			save();
		}
		
		return n;
	}
	
	//Command
	public static void list(){
		string[] all = getAllDirectoryPaths();
		
		if(all.Length == 0){
			Tebas.output("No projects found");
		}else{
			Tebas.output(all.Length + " projects found:");
		}
		
		foreach(string p in all){
			Tebas.output("  " + Path.GetFileName(p) + ": " + p);
		}
	}
	
	public static bool create(string dirPath, Template temp){
		if(exists(dirPath)){
			Tebas.report("Cannot init project in '" + dirPath + "' because it already exists");
			return false;
		}
		
		if(!Directory.Exists(dirPath)){
			Tebas.report("Cannot init project in '" + dirPath + "' because directory does not exists");
			return false;
		}
		
		AshFile projFile = new AshFile(dirPath + "/.tebas");
		
		projFile.Set("template", temp.name);
		projFile.Set("creationDate", (Date) DateTime.Now);
		
		projFile.Save();
		
		Project p = get(dirPath);
		
		p.tryRunScript("init");
		
		return true;
	}
	#endregion
	
	public string path {get;}
	public string filePath => path + "/.tebas";
	public string name => Path.GetFileName(path);
	
	public string templateName {get;}
	public Template template {get;}
	
	Dictionary<string, TableScript> cachedScripts = new();
	TableScript cachedProperties = null;
	
	public TebasProjectImportGenerator importGenerator {get;}
	TemplateScriptImportResolver scriptsImportResolver;
	
	public Date creationDate => file.GetValue<Date>("creationDate");
	
	AshFile file;
	
	private Project(string dirPath){
		path = dirPath;
		
		file = new AshFile(filePath);
		
		AshFileModel pm = new AshFileModel(
			new ModelInstance(ModelInstanceOperation.Type, "template", "blank"),
			new ModelInstance(ModelInstanceOperation.Type, "creationDate", (Date) DateTime.Now)
		);
		
		file.ApplyModel(pm);
		file.Save();
		
		templateName = file.GetValue<string>("template");
		template = Template.get(templateName);
		
		importGenerator = new TebasProjectImportGenerator(this);
		if(template != null){
			scriptsImportResolver = new TemplateScriptImportResolver(this, template); //Will never be used if template is null, so no worries
		}
	}
	
	public bool checkTemplate(){
		if(template == null){
			Tebas.report("The template '" + templateName +"' is not installed", Palette.warn);
			return false;
		}
		return true;
	}
	
	public bool tryRunScriptOrGlobal(string name, IEnumerable<string> args = null, bool checkForForbidden = false){
		if(template == null){
			return false;
		}
		
		if(!tryRunScript(name, args, checkForForbidden)){
			return template.tryRunGlobal(name, args, checkForForbidden);
		}
		
		return true; //tryRunScript succeeded
	}
	
	public bool tryRunScript(string name, IEnumerable<string> args = null, bool checkForForbidden = false){
		if(template == null){
			return false;
		}
		
		if(checkForForbidden && Template.forbiddenScripts.Contains(name)){
			return false;
		}
		
		if(cachedScripts.TryGetValue(name, out TableScript c) && c != null){
			c.Run(args);
			
			return true;
		}else{
			try{
				ResolvedImport r = template.getScriptAsImport(name);
				if(r == null){
					return false;
				}
				TableScript s = TableScript.FromImport(r, scriptsImportResolver, Tebas.templateReport);
				cachedScripts[name] = s;
				
				s.Run(args);
				
				return true;
			}catch(TabScriptException x){
				Tebas.templateReport(x);
			}
		}
		return false;
	}
	
	//Never null
	public Table getProperty(string key){
		if(template == null){
			return new Table(0);
		}
		
		if(cachedProperties != null){
			return cachedProperties.CallFunction(null, "getProperty", new Table(key));
		}else{
			try{
				ResolvedImport r = template.getPropertiesAsImport();
				if(r == null){
					return new Table(0);
				}
				TableScript s = TableScript.FromImport(r, scriptsImportResolver, Tebas.templateReport);
				s.Run(); //Needed to then use CallFunction
				cachedProperties = s;
				
				return cachedProperties.CallFunction(null, "getProperty", new Table(key));
			}catch(TabScriptException x){
				
			}
		}
		return new Table(0);
	}
	
	public bool tryRunPluginScriptOrGlobal(Plugin plugin, string name, IEnumerable<string> args = null, bool checkForForbidden = false){
		if(plugin == null){
			return false;
		}
		
		if(!tryRunPluginScript(plugin, name, args)){
			return plugin.tryRunGlobal(name, args, checkForForbidden);
		}
		
		return true; //tryRunPluginScript succeeded
	}
	
	public bool tryRunPluginScript(Plugin plugin, string name, IEnumerable<string> args = null){
		if(plugin == null){
			return false;
		}
		
		try{
			ResolvedImport r = plugin.getScriptAsImport(name);
			if(r == null){
				return false;
			}
			TableScript s = TableScript.FromImport(r, new PluginScriptImportResolver(this, plugin), Tebas.pluginReport);
			
			s.Run(args);
			
			return true;
		}catch(TabScriptException x){
			Tebas.pluginReport(x);
		}
		return false;
	}
	
	public void info(){
		Tebas.output("Project name: " + name);
		Tebas.output("Project directory: " + path);
		Tebas.output("Date of creation: " + creationDate);
		
		Tebas.output("");
		
		if(!checkTemplate()){ //Error text if needed
			return;
		}
		
		Tebas.output("Template in use: " + templateName);
		
		tryRunScript("info");
	}
	
	public string getResource(string key){
		if(file.TryGetValue("resources." + key, out string s)){
			return s;
		}
		return "";
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
	
	public string[] getAllResourceKeys(){
		return file.Keys.Where(k => k.StartsWith("resources.")).Select(k => k.Substring(10)).ToArray();
	}
	
	public void cleanupInstance(){
		foreach(KeyValuePair<string, object> keyVal in file.Where(kvp => !(kvp.Key == "template" || kvp.Key == "creationDate"
			|| (kvp.Key.StartsWith("resources.") && kvp.Value is string))).ToList()){
			file.Remove(keyVal.Key);
		}
		
		file.Save();
		
		tryRunScript("cleanup");
	}
}