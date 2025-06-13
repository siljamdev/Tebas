using System;
using System.Text;
using AshLib.Formatting;

public class Script{
	static readonly char[] invalidNameChars = "[]\"(),{}%".ToCharArray();
	string name;
	
	ScriptType type;
	
	string[] lines;
	Sentence[] sentences;
	Dictionary<string, int> functions;
	
	Dictionary<string, List<string>> tables;
	
	bool useColors = true;
	
	byte allowProcesses; //0 is never, 1 is ask and 2 is always
	
	FormatString formattedName;
	FormatString formattedErrorName;
	
	int lp; //linepointer;
	
	Random rand;
	
	//Checking if file is executable on linux needs the access function from the libc, main c library
	[System.Runtime.InteropServices.DllImport("libc", SetLastError = true)]
	static extern int access(string pathname, int mode);
	
	public Script(string name, ScriptType t, string code){
		//code = code.Replace("\\n", "\n");
		
		this.name = name;
		this.type = t;
		this.useColors = Tebas.useColors();
		
		bool showName = (Tebas.config.CanGetCamp("script.showName", out bool b) && b);
		
		allowProcesses = Tebas.config.GetCampOrDefault("script.allowProcesses", (byte) 1); 
		
		formattedName = showName ? new FormatString("[" + name + "] ", Tebas.blueCharFormat) : new FormatString();
		formattedErrorName = showName ? new FormatString("[" + name + " ERROR] ", Tebas.errorCharFormat) : new FormatString();
		
		Tebas.initializeConfig();
		
		List<string> l = code.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None).ToList();
		
		for(int i = 0; i < l.Count; i++){
			if(l[i].EndsWith("/+")){
				l[i] = l[i].Substring(0, l[i].Length - 2);
				if(i + 1 < l.Count){
					l[i] = l[i] + l[i + 1];
					l.RemoveAt(i + 1);
					i--;
				}
			}
		}
		
		lines = l.ToArray();
		
		sentences = new Sentence[lines.Length];
		
		for(int i = 0; i < lines.Length; i++){
			sentences[i] = new Sentence(lines[i]);
		}
		
		functions = new Dictionary<string, int>();
		
		for(int i = 0; i < sentences.Length; i++){
			try{
				if(sentences[i].command == "function"){
					string n = sentences[i].getArg(1);
					if(isValidName(n)){
						if(sentences[i].getArg(2) != "{"){
							outputErrorLine("Invalid function " + n + ": missing {");
						}else{
							functions.Add(n, i);
						}
					}else{
						outputErrorLine("Invalid function name: " + n);
					}
				}
			}catch(Exception){}
		}
	}
	
	public void run(IEnumerable<string> args){
		tables = new Dictionary<string, List<string>>();
		
		if(args != null){
			tables["args"] = new List<string>(args);
		}
		
		tables["true"] = new List<string>();
		tables["true"].Add("1");
		
		tables["false"] = new List<string>();
		tables["false"].Add("0");
		
		lp = 0;
		
		rand = new Random();
		
		int flowLevel = 0;
		Stack<FlowComponent> flow = new Stack<FlowComponent>();
		
		Stack<Scope> scope = new Stack<Scope>();
		
		while(lp < lines.Length){
			try{				
				Sentence s = sentences[lp];
				
				switch(s.command){
					case "process.cmd":
					string pname;
					string prog;
					string args2;
					if(OperatingSystem.IsWindows()){
						pname = name + " CMD";
						prog = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\System32\cmd.exe");
						args2 = "/c \"" + getString(1) + "\"";
					}else{
						pname = name + " SH";
						prog = "sh";
						args2 = "-c \"" + getString(1) + "\"";
					}
					switch(allowProcesses){
						case 0:
						outputLine("Blocked process '" + prog + "' with args: " + args2);
						break;
						case 1:
						outputLine("Do you allow to run process '" + prog + "' with args '" + args2 + "'? (Y/N)");
						string ans = Console.ReadLine().ToLower();
						if(ans == "y"){
							ProcessExecuter.runProcess(pname, prog, args2, Tebas.workingDirectory);
						}
						break;
						case 2:
						ProcessExecuter.runProcess(pname, prog, args2, Tebas.workingDirectory);
						break;
					}
					break;
					
					case "process.link":
					ProcessExecuter.openLink(getString(1));
					break;
					
					case "process.run":
					pname = name + " " + getString(1).ToUpper();
					prog = getString(1);
					args2 = getString(2);
					switch(allowProcesses){
						case 0:
						outputLine("Blocked process '" + prog + "' with args: " + args2);
						break;
						case 1:
						outputLine("Do you allow to run process '" + prog + "' with args '" + args2 + "'? (Y/N)");
						string ans = Console.ReadLine().ToLower();
						if(ans == "y"){
							ProcessExecuter.runProcess(pname, prog, args2, Tebas.workingDirectory);
						}
						break;
						case 2:
						ProcessExecuter.runProcess(pname, prog, args2, Tebas.workingDirectory);
						break;
					}
					break;
					
					case "process.runDetached":
					prog = getString(1);
					args2 = getString(2);
					switch(allowProcesses){
						case 0:
						outputLine("Blocked process '" + prog + "' with args: " + args2);
						break;
						case 1:
						outputLine("Do you allow to run process '" + prog + "' with args '" + args2 + "'? (Y/N)");
						string ans = Console.ReadLine().ToLower();
						if(ans == "y"){
							ProcessExecuter.runProcessNewWindow(prog, args2, Tebas.workingDirectory);
						}
						break;
						case 2:
						ProcessExecuter.runProcessNewWindow(prog, args2, Tebas.workingDirectory);
						break;
					}
					break;
					
					case "process.runOutput":
					tables[getTableRef(3)] = new List<string>();
					tables[getTableRef(4)] = new List<string>();
					prog = getString(1);
					args2 = getString(2);
					
					switch(allowProcesses){
						case 0:
						outputLine("Blocked process '" + prog + "' with args: " + args2);
						break;
						case 1:
						outputLine("Do you allow to run process '" + prog + "' with args '" + args2 + "'? (Y/N)");
						string ans = Console.ReadLine().ToLower();
						if(ans == "y"){
							ProcessExecuter.runProcessWithOutput(prog, args2, Tebas.workingDirectory, tables[getTableRef(3)], tables[getTableRef(4)]);
						}
						break;
						case 2:
						ProcessExecuter.runProcessWithOutput(prog, args2, Tebas.workingDirectory, tables[getTableRef(3)], tables[getTableRef(4)]);
						break;
					}
					break;
					
					case "process.runExitCode":
					prog = getString(1);
					args2 = getString(2);
					switch(allowProcesses){
						case 0:
						outputLine("Blocked process '" + prog + "' with args: " + args2);
						setString(getStringRef(1), "");
						break;
						case 1:
						outputLine("Do you allow to run process '" + prog + "' with args '" + args2 + "'? (Y/N)");
						string ans = Console.ReadLine().ToLower();
						if(ans == "y"){
							setString(getStringRef(1), ProcessExecuter.runProcessExitCode(prog, args2, Tebas.workingDirectory).ToString());
						}else{
							setString(getStringRef(1), "");
						}
						break;
						case 2:
						setString(getStringRef(1), ProcessExecuter.runProcessExitCode(prog, args2, Tebas.workingDirectory).ToString());
						break;
					}
					break;
					
					case "process.isExecutableInPath":
					if(ProcessExecuter.isExecutableInPath(getString(2), out string fullPath)){
						setString(getStringRef(1), tables["true"][0]);
					}else{
						setString(getStringRef(1), tables["false"][0]);
					}
					break;
					
					case "process.getExecutableFullPath":
					ProcessExecuter.isExecutableInPath(getString(2), out fullPath);
					setString(getStringRef(1), fullPath);
					break;
					
					case "console.print":
					outputLine(getString(1));
					break;
					
					case "console.printExpand":
					outputLine(expandString(getString(1)));
					break;
					
					case "console.printFormat":
					FormatString fs = getString(1);
					
					if(Tebas.useColors()){
						outputLine(fs.ToString());
					}else{
						outputLine(fs.content);
					}
					break;
					
					case "console.ask":
					output(getString(2));
					
					if(!Environment.UserInteractive){
						setString(getStringRef(1), "");
						break;
					}
					setString(getStringRef(1), Console.ReadLine());
					break;
					
					case "console.pause":
					output(getString(1));
					if(!Environment.UserInteractive){
						outputLine("");
						break;
					}
					Console.ReadLine();
					outputLine("");
					break;
					
					case "time.wait":
					if(int.TryParse(getString(1), out int w)){
						Thread.Sleep(w);
					}
					break;
					
					case "string.set":
					setString(getStringRef(1), getString(2));
					break;
					
					case "string.expand":
					setString(getStringRef(1), expandString(getString(2)));
					break;
					
					case "string.append":
					setString(getStringRef(1), getString(2) + getString(3));
					break;
					
					case "string.split":
					tables[getTableRef(1)] = getString(2).Split(getTable(3).ToArray(), StringSplitOptions.None).ToList();
					break;
					
					case "string.splitChars":
					tables[getTableRef(1)] = getString(2).Select(c => c.ToString()).ToList();
					break;
					
					case "string.substring":
					if(!int.TryParse(getString(3), out int u)){
						setString(getStringRef(1), "");
						break;
					}
					if(u < 0){
						u = getString(2).Length + u;
					}
					setString(getStringRef(1), (int.TryParse(getString(4), out int v) && u >= 0 && u < getString(2).Length && v >= 0 && u + v <= getString(2).Length ? getString(2).Substring(u, v) : ""));
					break;
					
					case "string.replace":
					setString(getStringRef(1), getString(2).Replace(getString(3), getString(4)));
					break;
					
					case "string.equal":
					setString(getStringRef(1), (getString(2) == getString(3) ? tables["true"][0] : tables["false"][0]));
					break;
					
					case "string.lower":
					setString(getStringRef(1), getString(2).ToLower());
					break;
					
					case "string.upper":
					setString(getStringRef(1), getString(2).ToUpper());
					break;
					
					case "string.contains":
					setString(getStringRef(1), (getString(2).Contains(getString(3)) ? tables["true"][0] : tables["false"][0]));
					break;
					
					case "string.trim":
					setString(getStringRef(1), getString(2).Trim());
					break;
					
					case "string.removeQuotes":
					setString(getStringRef(1), StringHelper.removeQuotesSingle(getString(2)));
					break;
					
					case "string.count":
					setString(getStringRef(1), getString(2).Length.ToString());
					break;
					
					case "self.expand":
					setString(getStringRef(1), expandString(getString(1)));
					break;
					
					case "self.append":
					setString(getStringRef(1), getString(1) + getString(2));
					break;
					
					case "self.substring":
					if(!int.TryParse(getString(2), out u)){
						setString(getStringRef(1), "");
						break;
					}
					if(u < 0){
						u = getString(1).Length + u;
					}
					setString(getStringRef(1), (int.TryParse(getString(3), out v) && u >= 0 && u < getString(1).Length && v >= 0 && u + v <= getString(1).Length ? getString(1).Substring(u, v) : ""));
					break;
					
					case "self.replace":
					setString(getStringRef(1), getString(1).Replace(getString(2), getString(3)));
					break;
					
					case "self.upper":
					setString(getStringRef(1), getString(1).ToUpper());
					break;
					
					case "self.lower":
					setString(getStringRef(1), getString(1).ToLower());
					break;
					
					case "self.trim":
					setString(getStringRef(1), getString(1).Trim());
					break;
					
					case "self.removeQuotes":
					setString(getStringRef(1), StringHelper.removeQuotesSingle(getString(1)));
					break;
					
					case "table.access":
					List<string> l2 = getTable(2);
					if(getIndex(getString(3), l2.Count, out u) && u >= 0 && u < l2.Count){
						setString(getStringRef(1), l2[u]);
					}else{
						setString(getStringRef(1), "");
					}
					break;
					
					case "table.find":
					string v22 = getString(3);
					int index = getTable(2).FindIndex(n => n == v22);
					if(index == -1){
						setString(getStringRef(1), "");
					}else{
						setString(getStringRef(1), index.ToString());
					}
					break;
					
					case "table.setAt":
					if(getIndex(getString(2), getTableRef(1), out u)){
						setString(new stringRef(getTableRef(1), u), getString(3));
					}
					break;
					
					case "table.delete":
					if(tables.ContainsKey(getStringRef(1).table) && getIndex(getStringRef(1).index.ToString(), getStringRef(1).table, out u) && u >= 0 && u < tables[getStringRef(1).table].Count){
						tables[getStringRef(1).table].RemoveAt(u);
					}
					break;
					
					case "table.deleteAt":
					if(tables.ContainsKey(getTableRef(1)) && getIndex(getString(2), getTableRef(1), out u) && u >= 0 && u < tables[getTableRef(1)].Count){
						tables[getTableRef(1)].RemoveAt(u);
					}
					break;
					
					case "table.deleteAll":
					if(tables.ContainsKey(getTableRef(1))){
						string v2 = getString(2);
						tables[getTableRef(1)].RemoveAll(n => n == v2);
					}
					break;
					
					case "table.insert":
					if(tables.ContainsKey(getStringRef(1).table) && getIndex(getStringRef(1).index.ToString(), getStringRef(1).table, out u) && u >= 0 && u < tables[getStringRef(1).table].Count){
						tables[getStringRef(1).table].Insert(u, getString(2));
					}else{
						setString(getStringRef(1), getString(2));
					}
					break;
					
					case "table.insertAt":
					if(tables.ContainsKey(getTableRef(1)) && getIndex(getString(2), getTableRef(1), out u) && u >= 0 && u < tables[getStringRef(1).table].Count){
						tables[getTableRef(1)].Insert(u, getString(3));
					}else if(getIndex(getString(2), getTableRef(1), out u)){
						setString(new stringRef(getTableRef(1), u), getString(3));
					}
					break;
					
					case "table.contains":
					setString(getStringRef(1), (getTable(2).Contains(getString(3)) ? tables["true"][0] : tables["false"][0]));
					break;
					
					case "table.clear":
					tables.Remove(getTableRef(1));
					break;
					
					case "table.range":
					if(!int.TryParse(getString(3), out u)){
						tables.Remove(getTableRef(1));
						break;
					}
					if(u < 0){
						u = getTable(2).Count + u;
					}
					tables[getTableRef(1)] = (int.TryParse(getString(4), out v) && u >= 0 && u < getTable(2).Count && v >= 0 && u + v <= getTable(2).Count ? getTable(2).GetRange(u, v) : new List<string>());
					break;
					
					case "table.add":
					if(tables.ContainsKey(getTableRef(1))){
						tables[getTableRef(1)].Add(getString(2));
					}else{
						setString(new stringRef(getTableRef(1), 0), getString(2));
					}
					break;
					
					case "table.pop":
					if(tables.ContainsKey(getTableRef(2)) && getIndex("-1", getTableRef(2), out u) && u >= 0 && u < tables[getTableRef(2)].Count){
						setString(getStringRef(1), tables[getTableRef(2)][u]);
						tables[getTableRef(2)].RemoveAt(tables[getTableRef(2)].Count - 1);
					}else{
						setString(getStringRef(1), "");
					}
					break;
					
					case "table.peek":
					if(tables.ContainsKey(getTableRef(2)) && getIndex("-1", getTableRef(2), out u) && u >= 0 && u < tables[getTableRef(2)].Count){
						setString(getStringRef(1), tables[getTableRef(2)][u]);
					}else{
						setString(getStringRef(1), "");
					}
					break;
					
					case "table.append":
					if(tables.ContainsKey(getTableRef(1))){
						tables[getTableRef(1)].AddRange(getTable(2));
					}else{
						tables[getTableRef(1)] = new List<string>(getTable(2));
					}
					break;
					
					case "table.set":
					tables[getTableRef(1)] = new List<string>(getTable(2));
					break;
					
					case "table.shuffle":
					tables[getTableRef(1)] = new List<string>(getTable(2));
					
					for (int i = tables[getTableRef(1)].Count - 1; i > 0; i--){
						int j = rand.Next(i + 1); // Random index from 0 to i
						
						// Swap elements
						string temp = tables[getTableRef(1)][i];
						tables[getTableRef(1)][i] = tables[getTableRef(1)][j];
						tables[getTableRef(1)][j] = temp;
					}
					break;
					
					case "table.join":
					setString(getStringRef(1), string.Join(getString(3), getTable(2)));
					break;
					
					case "bool.negate":
					setString(getStringRef(1), (tables["true"].Contains(getString(2)) ? tables["false"][0] : tables["true"][0]));
					break;
					
					case "bool.and":
					setString(getStringRef(1), (tables["true"].Contains(getString(2)) && tables["true"].Contains(getString(3)) ? tables["true"][0] : tables["false"][0]));
					break;
					
					case "bool.or":
					setString(getStringRef(1), (tables["true"].Contains(getString(2)) || tables["true"].Contains(getString(3)) ? tables["true"][0] : tables["false"][0]));
					break;
					
					case "bool.isBool":
					setString(getStringRef(1), (tables["true"].Contains(getString(2)) || tables["false"].Contains(getString(2)) ? tables["true"][0] : tables["false"][0]));
					break;
					
					case "math.isNumber":
					setString(getStringRef(1), (int.TryParse(getString(2), out u) ? tables["true"][0] : tables["false"][0]));
					break;
					
					case "math.isNegative":
					setString(getStringRef(1), (int.TryParse(getString(2), out u) ? (u > -1 ? tables["true"][0] : tables["false"][0]) : ""));
					break;
					
					case "math.equal":
					setString(getStringRef(1), (int.TryParse(getString(2), out u) && int.TryParse(getString(3), out v) ? (u == v ? tables["true"][0] : tables["false"][0]) : ""));
					break;
					
					case "math.getRandom":
					setString(getStringRef(1), (int.TryParse(getString(2), out u) && int.TryParse(getString(3), out v) ? rand.Next(u, v).ToString() : ""));
					break;
					
					case "math.sumUp":
					setString(getStringRef(1), (int.TryParse(getString(2), out u) ? (u + 1).ToString() : ""));
					break;
					
					case "math.sumDown":
					setString(getStringRef(1), (int.TryParse(getString(2), out u) ? (u - 1).ToString() : ""));
					break;
					
					case "math.sum":
					setString(getStringRef(1), (int.TryParse(getString(2), out u) && int.TryParse(getString(3), out v) ? (u + v).ToString() : ""));
					break;
					
					case "math.subtract":
					setString(getStringRef(1), (int.TryParse(getString(2), out u) && int.TryParse(getString(3), out v) ? (u - v).ToString() : ""));
					break;
					
					case "math.multiply":
					setString(getStringRef(1), (int.TryParse(getString(2), out u) && int.TryParse(getString(3), out v) ? (u * v).ToString() : ""));
					break;
					
					case "math.divide":
					setString(getStringRef(1), (int.TryParse(getString(2), out u) && int.TryParse(getString(3), out v) ? (u / v).ToString() : ""));
					break;
					
					case "math.modulus":
					setString(getStringRef(1), (int.TryParse(getString(2), out u) && int.TryParse(getString(3), out v) ? (u % v).ToString() : ""));
					break;
					
					case "math.abs":
					setString(getStringRef(1), (int.TryParse(getString(2), out u) ? (Math.Abs(u)).ToString() : ""));
					break;
					
					case "math.greater":
					setString(getStringRef(1), (int.TryParse(getString(2), out u) && int.TryParse(getString(3), out v) ? (u > v ? tables["true"][0] : tables["false"][0]) : ""));
					break;
					
					case "math.greaterEqual":
					setString(getStringRef(1), (int.TryParse(getString(2), out u) && int.TryParse(getString(3), out v) ? (u >= v ? tables["true"][0] : tables["false"][0]) : ""));
					break;
					
					case "math.less":
					setString(getStringRef(1), (int.TryParse(getString(2), out u) && int.TryParse(getString(3), out v) ? (u < v ? tables["true"][0] : tables["false"][0]) : ""));
					break;
					
					case "math.lessEqual":
					setString(getStringRef(1), (int.TryParse(getString(2), out u) && int.TryParse(getString(3), out v) ? (u <= v ? tables["true"][0] : tables["false"][0]) : ""));
					break;
					
					case "path.extension":
					setString(getStringRef(1), Path.GetExtension(getString(2)));
					break;
					
					case "path.filename":
					setString(getStringRef(1), Path.GetFileName(getString(2)));
					break;
					
					case "path.filenameNoExtension":
					setString(getStringRef(1), Path.GetFileNameWithoutExtension(getString(2)));
					break;
					
					case "path.directory":
					setString(getStringRef(1), Path.GetDirectoryName(getString(2)));
					break;
					
					case "file.create":
					if(!File.Exists(getPath(getString(1)))){
						File.Create(getPath(getString(1))).Dispose();
					}
					break;
					
					case "file.read":
					if(File.Exists(getPath(getString(2)))){
						setString(getStringRef(1), File.ReadAllText(getPath(getString(2))));
					}else{
						setString(getStringRef(1), "");
					}
					break;
					
					case "file.delete":
					if(File.Exists(getPath(getString(1)))){
						File.Delete(getPath(getString(1)));
					}
					break;
					
					case "file.rename":
					if(File.Exists(getPath(getString(1))) && !File.Exists(getPath(getString(2)))){
						File.Move(getPath(getString(1)), getPath(getString(2)));
					}
					break;
					
					case "file.copy":
					if(File.Exists(getPath(getString(1))) && !File.Exists(getPath(getString(2)))){
						File.Copy(getPath(getString(1)), getPath(getString(2)));
					}
					break;
					
					case "file.write":
					if(File.Exists(getPath(getString(1)))){
						File.WriteAllText(getPath(getString(1)), getString(2));
					}
					break;
					
					case "file.append":
					if(File.Exists(getPath(getString(1)))){
						File.AppendAllText(getPath(getString(1)), getString(2));
					}
					break;
					
					case "file.exists":
					setString(getStringRef(1), (fileExistsWildcard(getPathExclusive(getString(2))) ? tables["true"][0] : tables["false"][0]));
					break;
					
					case "file.size":
					setString(getStringRef(1), File.Exists(getPath(getString(2))) ? new FileInfo(getPath(getString(2))).Length.ToString() : "");
					break;
					
					case "file.isExecutable":
					if(OperatingSystem.IsWindows()){
						string p = getPathExclusive(getString(2));
						setString(getStringRef(1), (File.Exists(p) && Path.GetExtension(p).ToLower() == ".exe") ? tables["true"][0] : tables["false"][0]);
					}else{
						setString(getStringRef(1), (access(getPathExclusive(getString(2)), 1) == 0) ? tables["true"][0] : tables["false"][0]);
					}
					break;
					
					case "folder.create":
					if(!Directory.Exists(getPath(getString(1)))){
						Directory.CreateDirectory(getPath(getString(1)));
					}
					break;
					
					case "folder.delete":
					if(Directory.Exists(getPath(getString(1)))){
						Directory.Delete(getPath(getString(1)), recursive: true);
					}
					break;
					
					case "folder.rename":
					if(Directory.Exists(getPath(getString(1))) && !Directory.Exists(getPath(getString(2)))){
						Directory.Move(getPath(getString(1)), getPath(getString(2)));
					}
					break;
					
					case "folder.exists":
					setString(getStringRef(1), (Directory.Exists(getPath(getString(2))) ? tables["true"][0] : tables["false"][0]));
					break;
					
					case "folder.list":
					tables[getTableRef(1)] = new List<string>(Directory.GetFiles(getPath(getString(2)), getString(3)));
					for(int i = 0; i < tables[getTableRef(1)].Count; i++){
						if(tables[getTableRef(1)][i].StartsWith(Tebas.workingDirectory)){
							tables[getTableRef(1)][i] = "W" + tables[getTableRef(1)][i].Substring(Tebas.workingDirectory.Length);
						}else if(tables[getTableRef(1)][i].StartsWith(Tebas.templateDirectory)){
							tables[getTableRef(1)][i] = "T" + tables[getTableRef(1)][i].Substring(Tebas.templateDirectory.Length);
						}
					}
					break;
					
					case "folder.listChild":
					tables[getTableRef(1)] = new List<string>(Directory.GetFiles(getPath(getString(2)), getString(3), SearchOption.AllDirectories));
					for(int i = 0; i < tables[getTableRef(1)].Count; i++){
						if(tables[getTableRef(1)][i].StartsWith(Tebas.workingDirectory)){
							tables[getTableRef(1)][i] = "W" + tables[getTableRef(1)][i].Substring(Tebas.workingDirectory.Length);
						}else if(tables[getTableRef(1)][i].StartsWith(Tebas.templateDirectory)){
							tables[getTableRef(1)][i] = "T" + tables[getTableRef(1)][i].Substring(Tebas.templateDirectory.Length);
						}
					}
					break;
					
					case "template.read":
					if(type == ScriptType.Template || type == ScriptType.TemplateGlobal){
						setString(getStringRef(1), TemplateHandler.resourceRead(getString(2)));
					}else{
						setString(getStringRef(1), "");
					}
					break;
					
					case "template.write":
					if(type == ScriptType.Template || type == ScriptType.TemplateGlobal){
						TemplateHandler.resourceWrite(getString(1), getString(2));
					}
					break;
					
					case "template.append":
					if(type == ScriptType.Template || type == ScriptType.TemplateGlobal){
						TemplateHandler.resourceAppend(getString(1), getString(2));
					}
					break;
					
					case "template.run":
					if(type != ScriptType.Standalone){
						TemplateHandler.runScript(getString(1), StringHelper.splitSentence(getString(2)));
					}
					break;
					
					case "template.global":
					TemplateHandler.runGlobal(getString(1), getString(2), StringHelper.splitSentence(getString(3)));
					break;
					
					case "template.installed":
					setString(getStringRef(1), (TemplateHandler.exists(getString(2)) ? tables["true"][0] : tables["false"][0]));
					break;
					
					case "template.list":
					tables[getTableRef(1)] = TemplateHandler.getList();
					break;
					
					case "template.create":
					CreatorUtility.template(getPathExclusive(getString(1)));
					break;
					
					case "plugin.read":
					if(type == ScriptType.Plugin){
						setString(getStringRef(1), PluginHandler.readResource(getString(2)));
					}else{
						setString(getStringRef(1), "");
					}
					break;
					
					case "plugin.write":
					if(type == ScriptType.Plugin){
						PluginHandler.writeResource(getString(1), getString(2));
					}
					break;
					
					case "plugin.append":
					if(type == ScriptType.Plugin){
						PluginHandler.appendResource(getString(1), getString(2));
					}
					break;
					
					case "plugin.run":
					PluginHandler.runScript(getString(1), getString(2), StringHelper.splitSentence(getString(3)));
					break;
					
					case "plugin.installed":
					setString(getStringRef(1), (PluginHandler.exists(getString(2)) ? tables["true"][0] : tables["false"][0]));
					break;
					
					case "plugin.list":
					tables[getTableRef(1)] = PluginHandler.getList();
					break;
					
					case "plugin.create":
					CreatorUtility.plugin(getPathExclusive(getString(1)));
					break;
					
					case "project.read":
					if(type == ScriptType.Template || type == ScriptType.Plugin){
						setString(getStringRef(1), projectRead(getString(2)));
					}else{
						setString(getStringRef(1), "");
					}
					break;
					
					case "project.write":
					if(type == ScriptType.Template || type == ScriptType.Plugin){
						projectWrite(getString(1), getString(2));
					}
					break;
					
					case "project.append":
					if(type == ScriptType.Template || type == ScriptType.Plugin){
						projectAppend(getString(1), getString(2));
					}
					break;
					
					case "project.gitUsed":
					if(type == ScriptType.Template || type == ScriptType.Plugin){
						if(Tebas.project != null && Tebas.project.CanGetCamp("git.use", out bool b) && b){
							setString(getStringRef(1), tables["true"][0]);
						}else{
							setString(getStringRef(1), tables["false"][0]);
						}
					}else{
						setString(getStringRef(1), "");
					}
					break;
					
					case "project.remoteSet":
					if(type == ScriptType.Template){
						Tebas.localRemoteSet(getString(1), getString(2));
					}
					break;
					
					case "project.remoteUrl":
					if(type == ScriptType.Template || type == ScriptType.Plugin){
						setString(getStringRef(1), GitHelper.getRemoteUrl(getString(2)));
					}
					break;
					
					case "project.remoteDelete":
					if(type == ScriptType.Template){
						Tebas.localRemoteDelete(getString(1));
					}
					break;
					
					case "project.remoteList":
					if(type == ScriptType.Template || type == ScriptType.Plugin){
						tables[getTableRef(1)] = GitHelper.getAllRemotes();
					}
					break;
					
					case "project.remoteExists":
					if(type == ScriptType.Template || type == ScriptType.Plugin){
						setString(getStringRef(1), (GitHelper.remoteExists(getString(2)) ? tables["true"][0] : tables["false"][0]));
					}else{
						setString(getStringRef(1), "");
					}
					break;
					
					case "project.getCurrentGitBranch":
					if(type == ScriptType.Template || type == ScriptType.Plugin){
						setString(getStringRef(1), GitHelper.getBranch());
					}else{
						setString(getStringRef(1), "");
					}
					break;
					
					case "shared.read":
					setString(getStringRef(1), SharedHandler.read(getString(2)));
					break;
					
					case "shared.write":
					SharedHandler.write(getString(1), getString(2));
					break;
					
					case "shared.append":
					SharedHandler.append(getString(1), getString(2));
					break;
					
					case "tebas.commit":
					if(type == ScriptType.Template || type == ScriptType.Plugin){
						Tebas.localCommit(getString(1));
					}
					break;
					
					case "tebas.getDefaultGitBranch":
					setString(getStringRef(1), GitHelper.getDefaultBranch());
					break;
					
					case "tebas.push":
					if(type == ScriptType.Template || type == ScriptType.Plugin){
						Tebas.localPush(getString(1));
					}
					break;
					
					case "tebas.pull":
					if(type == ScriptType.Template || type == ScriptType.Plugin){
						Tebas.localPull(getString(1));
					}
					break;
					
					case "tebas.add":
					if(type == ScriptType.Template || type == ScriptType.Plugin){
						Tebas.localAdd();
					}
					break;
					
					case "tebas.script":
					Tebas.runStandaloneScript(getPathExclusive(getString(1)), StringHelper.splitSentence(getString(2)));
					break;
					
					case "tebas.channelExists":
					setString(getStringRef(1), ChannelHandler.canGetPath(getString(2), out string notused) ? tables["true"][0] : tables["false"][0]);
					break;
					
					case "tebas.channelList":
					tables[getTableRef(1)] = ChannelHandler.allNames().ToList();
					break;
					
					case "tebas.channelPath":
					setString(getStringRef(1), ChannelHandler.canGetPath(getString(2), out notused) ? notused : "");
					break;
					
					case "tebas.isInteractive":
					setString(getStringRef(1), Tebas.isConsoleInteractive() ? tables["true"][0] : tables["false"][0]);
					break;
					
					case "scope":
					if(tables.ContainsKey(getTableRef(1))){
						scope.Pop();
						scope.Push(new Scope(getTableRef(1), tables[getTableRef(1)]));
						tables[getTableRef(1)] = new List<string>();
					}else{
						scope.Pop();
						scope.Push(new Scope(getTableRef(1), new List<string>()));
						tables[getTableRef(1)] = new List<string>();
					}
					break;
					
					case "while":
					try{
						if(s.getArg(2) != "{"){
							throw new TebasScriptError("Incorrect while loop: missing {");
						}
						if(tables["true"].Contains(getString(1))){
							flow.Push(new FlowComponent(FlowType.While, lp, flowLevel));
							flowLevel++;
						}else{
							lp++;
							searchEndBlock();
						}
					}catch(Exception e){
						int max = s.getNumOfArgs() + 1;
						for(int i = 1; i < max; i++){
							if(s.getArg(i) == "{"){
								flow.Push(new FlowComponent(FlowType.Error, lp, flowLevel));
								break;
							}
						}
						throw e;
					}
					break;
					
					case "while!":
					try{
						if(s.getArg(2) != "{"){
							throw new TebasScriptError("Incorrect while! loop: missing {");
						}
						if(!tables["true"].Contains(getString(1))){
							flow.Push(new FlowComponent(FlowType.While, lp, flowLevel));
							flowLevel++;
						}else{
							lp++;
							searchEndBlock();
						}
					}catch(Exception e){
						int max = s.getNumOfArgs() + 1;
						for(int i = 1; i < max; i++){
							if(s.getArg(i) == "{"){
								flow.Push(new FlowComponent(FlowType.Error, lp, flowLevel));
								break;
							}
						}
						throw e;
					}
					break;
					
					case "do":
					try{
						if(s.getArg(1) != "{"){
							int max = s.getNumOfArgs() + 1;
							for(int i = 1; i < max; i++){
								if(s.getArg(i) == "{"){
									flow.Push(new FlowComponent(FlowType.Error, lp, flowLevel));
									break;
								}
							}
							throw new TebasScriptError("Incorrect do loop: missing {");
						}
						
						flow.Push(new FlowComponent(FlowType.Do, lp, flowLevel));
						flowLevel++;
					}catch(Exception e){
						int max = s.getNumOfArgs() + 1;
						for(int i = 1; i < max; i++){
							if(s.getArg(i) == "{"){
								flow.Push(new FlowComponent(FlowType.Error, lp, flowLevel));
								break;
							}
						}
						throw e;
					}
					break;
					
					case "for":
					try{
						if(s.getArg(3) != "{"){
							throw new TebasScriptError("Incorrect for loop: missing {");
						}
						if(int.TryParse(getString(1), out int l)){
							setString(getStringRef(1), (l + 1).ToString());
						}else{
							setString(getStringRef(1), "0");
						}
						
						if(int.TryParse(getString(1), out int f1) && int.TryParse(getString(2), out int f2) && f1 < f2){
							flow.Push(new FlowComponent(FlowType.For, lp, flowLevel));
							flowLevel++;
						}else{
							lp++;
							searchEndBlock();
						}
					}catch(Exception e){
						int max = s.getNumOfArgs() + 1;
						for(int i = 1; i < max; i++){
							if(s.getArg(i) == "{"){
								flow.Push(new FlowComponent(FlowType.Error, lp, flowLevel));
								break;
							}
						}
						throw e;
					}
					break;
					
					case "continue":
					while(flow.Count > 0){
						FlowComponent fc = flow.Pop();
						if(fc.ft == FlowType.While || fc.ft == FlowType.Do || fc.ft == FlowType.For){
							flow.Push(fc);
							flowLevel = fc.level + 1;
							lp = fc.line;
							lp++;
							searchEndBlock();
							lp--;
							break;
						}
					}
					flowLevel = 0;
					break;
					
					case "break":
					while(flow.Count > 0){
						FlowComponent fc = flow.Pop();
						if(fc.ft == FlowType.While || fc.ft == FlowType.Do || fc.ft == FlowType.For){
							lp = fc.line;
							flowLevel = fc.level;
							lp++;
							searchEndBlock();
							break;
						}
					}
					flowLevel = 0;
					break;
					
					case "if":
					try{
						if(s.getArg(2) != "{"){
							throw new TebasScriptError("Incorrect if statement: missing {");
						}
						
						if(tables["true"].Contains(getString(1))){
							flow.Push(new FlowComponent(FlowType.If, lp, flowLevel));
							flowLevel++;
						}else{
							flow.Push(new FlowComponent(FlowType.Else, lp, flowLevel));
							flowLevel++;
							lp++;
							searchEndBlock();
							lp--;
						}
					}catch(Exception e){
						int max = s.getNumOfArgs() + 1;
						for(int i = 1; i < max; i++){
							if(s.getArg(i) == "{"){
								flow.Push(new FlowComponent(FlowType.Error, lp, flowLevel));
								break;
							}
						}
						throw e;
					}
					break;
					
					case "if!":
					try{
						if(s.getArg(2) != "{"){
							throw new TebasScriptError("Incorrect if! statement: missing {");
						}
						
						if(!tables["true"].Contains(getString(1))){
							flow.Push(new FlowComponent(FlowType.If, lp, flowLevel));
							flowLevel++;
						}else{
							flow.Push(new FlowComponent(FlowType.Else, lp, flowLevel));
							flowLevel++;
							lp++;
							searchEndBlock();
							lp--;
						}
					}catch(Exception e){
						int max = s.getNumOfArgs() + 1;
						for(int i = 1; i < max; i++){
							if(s.getArg(i) == "{"){
								flow.Push(new FlowComponent(FlowType.Error, lp, flowLevel));
								break;
							}
						}
						throw e;
					}
					break;
					
					case "call":
					if(functions.ContainsKey(s.getArg(1))){
						flow.Push(new FlowComponent(FlowType.Function, lp, flowLevel));
						flowLevel++;
						if(scope.Count > 0 && scope.Peek().table != null){
							Scope sp = scope.Pop();
							List<string> scoped = tables[sp.table];
							tables[sp.table] = sp.list;
							scope.Push(new Scope(sp.table, scoped));
						}
						scope.Push(new Scope(null, null));
						lp = functions[s.getArg(1)];
					}else{
						throw new TebasScriptError("Incorrect call: not found function " + s.getArg(1));
					}
					break;
					
					case "exit":
					return;
					
					case "function": //Only execute outise program of the script, when found functions, exit
					if(flowLevel == 0){
						return;	
					}else{
						/* int max = s.getNumOfArgs() + 1;
						for(int i = 1; i < max; i++){
							if(s.getArg(i) == "{"){
								flow.Push(new FlowComponent(FlowType.Error, lp, flowLevel));
								break;
							}
						}
						throw new TebasScriptError("Found internal function"); */
						return;
					}
					break;
					
					
					case "return":
					while(flow.Count > 0){
						FlowComponent fc = flow.Pop();
						if(fc.ft == FlowType.Function){
							Scope sco = scope.Pop();
							if(sco.table != null){
								tables[sco.table] = sco.list;
							}
							lp = fc.line;
							flowLevel = fc.level;
							break;
						}
					}
					flowLevel = 0;
					break;
					
					case "}":
					if(flow.Peek().ft == FlowType.While){
						lp = flow.Pop().line - 1;
						flowLevel--;
					}else if(flow.Peek().ft == FlowType.Do){
						FlowComponent fc = flow.Peek();
						if(!(s.getNumOfArgs() > 1) || (s.getArg(1) != "while" && s.getArg(1) != "while!")){
							throw new TebasScriptError("Incorrect do statement: missing while or while!");
						}
						if(s.getArg(1) == "while"){
							if(tables["true"].Contains(getString(2))){
								lp = fc.line;
							}else{
								flow.Pop();
								flowLevel--;
							}
						}else{
							if(!tables["true"].Contains(getString(2))){
							lp = fc.line;
							}else{
								flow.Pop();
								flowLevel--;
							}
						}
					}else if(flow.Peek().ft == FlowType.For){
						lp = flow.Pop().line - 1;
						flowLevel--;
					}else if(flow.Peek().ft == FlowType.If){
						if(s.getNumOfArgs() > 1){
							if(s.getArg(1) == "elseif"){
								lp++;
								searchEndBlock();
								lp--;
							}else if(s.getArg(1) == "elseif!"){
								lp++;
								searchEndBlock();
								lp--;
							}else if(s.getArg(1) == "else"){
								lp++;
								searchEndBlock();
								lp--;
							}
						}else{
							flow.Pop();
							flowLevel--;
						}
					}else if(flow.Peek().ft == FlowType.Else){
						if(s.getNumOfArgs() > 1){
							if(s.getArg(1) == "elseif"){
								if(s.getArg(3) != "{"){
									int max = s.getNumOfArgs() + 1;
									for(int i = 2; i < max; i++){
										if(s.getArg(i) == "{"){
											flow.Push(new FlowComponent(FlowType.Error, lp, flowLevel));
											break;
										}
									}
									throw new TebasScriptError("Incorrect elseif statement: missing {");
								}
								if(tables["true"].Contains(getString(2))){
									flow.Pop();
									flow.Push(new FlowComponent(FlowType.If, lp, flowLevel));
									flowLevel++;
								}else{
									lp++;
									searchEndBlock();
									lp--;
								}
							}else if(s.getArg(1) == "elseif!"){
								if(s.getArg(3) != "{"){
									int max = s.getNumOfArgs() + 1;
									for(int i = 2; i < max; i++){
										if(s.getArg(i) == "{"){
											flow.Push(new FlowComponent(FlowType.Error, lp, flowLevel));
											break;
										}
									}
									throw new TebasScriptError("Incorrect elseif! statement: missing {");
								}
								if(!tables["true"].Contains(getString(2))){
									flow.Pop();
									flow.Push(new FlowComponent(FlowType.If, lp, flowLevel));
									flowLevel++;
								}else{
									lp++;
									searchEndBlock();
									lp--;
								}
							}else if(s.getArg(1) == "else"){
								
							}
						}else{
							flow.Pop();
							flowLevel--;
						}
					}else if(flow.Peek().ft == FlowType.Function){
						Scope sco = scope.Pop();
						if(sco.table != null){
							tables[sco.table] = sco.list;
						}
						
						if(scope.Count > 0 && scope.Peek().table != null){
							Scope sp = scope.Pop();
							List<string> scoped = tables[sp.table];
							tables[sp.table] = sp.list;
							scope.Push(new Scope(sp.table, scoped));
						}
						
						flowLevel = flow.Peek().level;
						lp = flow.Pop().line;
					}else if(flow.Peek().ft == FlowType.Error){
						flow.Pop();
						flowLevel--;
					}
					break;
				}
			}catch(TebasScriptError tse){
				outputErrorLine("The scripts contained an error in line " + lp + ": \"" + sentences[lp].ToString() + "\"");
				outputErrorLine(tse.ToString());
				
				setError(tse.ToString());
			}catch(Exception e){
				outputErrorLine("An error occured in line " + lp + ": \"" + sentences[lp].ToString() + "\"");
				outputErrorLine(e.ToString());
				
				setError(e.ToString());
			}finally{
				lp++;
			}	
		}
	}
	
	void searchEndBlock(){
		int c = 1;
		while(true){
			if(sentences[lp].command == "if" || sentences[lp].command == "while" || sentences[lp].command == "do" || sentences[lp].command == "if!" || sentences[lp].command == "while!" || sentences[lp].command == "for"){
				c++;
			}else if(sentences[lp].command == "}"){
				c--;
				if(c == 0){
					return;
				}
				if(sentences[lp].getNumOfArgs() > 0 && (sentences[lp].getArg(1) == "elseif" || sentences[lp].getArg(1) == "else" || sentences[lp].getArg(1) == "elseif!")){
					c++;
				}
			}
			lp++;
		}
	}
	
	bool isValidName(string n){
		return !n.Any(c => invalidNameChars.Contains(c));
	}
	
	string getString(int i){
		string s = sentences[lp].getArg(i);
		
		if(s.Length > 2 && s[0] == 'f' && s[1] == '"' && s[s.Length - 1] == '"'){
			return expandString(s.Substring(2, s.Length - 3));
		}
		
		if(s.Length > 1 && s[0] == '"' && s[s.Length - 1] == '"'){
			return s.Substring(1, s.Length - 2);
		}
		
		if(s.Length > 0 && s[0] == '%'){
			return specialLoad(s);
		}
		
		if(isValidName(s)){
			string[] h = s.Split(".");
			if(h.Length != 2){
				throw new TebasScriptError("Failed to get string " + i + ", incorrect dot format");
			}
			if(tables.ContainsKey(h[0]) && h[1] == "length"){
				return tables[h[0]].Count.ToString();
			}
			if(tables.ContainsKey(h[0]) && getIndex(h[1], h[0], out int u)){
				if(u < tables[h[0]].Count){
					return tables[h[0]][u];
				}
			}
			return "";
		}
		
		throw new TebasScriptError("Failed to get string " + i);
	}
	
	string getString(string s){
		if(s.Length > 0 && s[0] == '%'){
			return specialLoad(s);
		}
		
		if(isValidName(s)){
			string[] h = s.Split(".");
			if(h.Length != 2){
				throw new TebasScriptError("Failed to get string \"" + s + "\", incorrect dot format");
			}
			if(h[1] == "length"){
				if(tables.ContainsKey(h[0])){
					return tables[h[0]].Count.ToString();
				}else{
					return "0";
				}
			}
			if(tables.ContainsKey(h[0]) && getIndex(h[1], h[0], out int u)){
				if(u < tables[h[0]].Count){
					return tables[h[0]][u];
				}
			}
			return "";
		}
		
		throw new TebasScriptError("Failed to get string \"" + s + "\"");
	}
	
	string getStringAll(string s){
		if(s.Length > 2 && s[0] == 'f' && s[1] == '"' && s[s.Length - 1] == '"'){
			return expandString(s.Substring(2, s.Length - 3));
		}
		
		if(s.Length > 1 && s[0] == '"' && s[s.Length - 1] == '"'){
			return s.Substring(1, s.Length - 2);
		}
		
		if(s.Length > 0 && s[0] == '%'){
			return specialLoad(s);
		}
		
		if(isValidName(s)){
			string[] h = s.Split(".");
			if(h.Length != 2){
				throw new TebasScriptError("Failed to get string \"" + s + "\", incorrect dot format");
			}
			if(h[1] == "length"){
				if(tables.ContainsKey(h[0])){
					return tables[h[0]].Count.ToString();
				}else{
					return "0";
				}
			}
			if(tables.ContainsKey(h[0]) && getIndex(h[1], h[0], out int u)){
				if(u < tables[h[0]].Count){
					return tables[h[0]][u];
				}
			}
			return "";
		}
		
		throw new TebasScriptError("Failed to get string \"" + s + "\"");
	}
	
	List<string> getTable(int i){
		string s = sentences[lp].getArg(i);
		
		if(s.Length > 1 && s[0] == '[' && s[s.Length - 1] == ']'){
			s = s.Substring(1, s.Length - 2);
			List<string> t = new List<string>();
			bool quoteOpen = false;
			
			int j = 0;
			
			bool previousEscapeCode = false;
			
			StringBuilder c = new StringBuilder();
			
			while(j < s.Length){
				if(s[j] == '"'){
					if(quoteOpen && !previousEscapeCode){
						quoteOpen = false;
						t.Add(c.ToString());
						c.Clear();
						j++;
						continue;
					}else if(quoteOpen && previousEscapeCode){
						previousEscapeCode = false;
						c.Append(s[j]);
						j++;
						continue;
					}else{
						quoteOpen = true;
						j++;
						continue;
					}
				}
				
				if(quoteOpen){
					if(s[j] == '\\'){
						if(!previousEscapeCode){
							previousEscapeCode = true;
							j++;
							continue;
						}else{
							previousEscapeCode = false;
						}
					}
					c.Append(s[j]);
				}
				
				j++;
			}
			
			return t;
		}
		
		if(isValidName(s)){
			if(tables.ContainsKey(s)){
				return tables[s];
			}
			return new List<string>();
		}
		
		throw new TebasScriptError("Failed to get table " + i);
	}
	
	stringRef getStringRef(int i){
		string s = sentences[lp].getArg(i);
		
		if(isValidName(s)){
			string[] h = s.Split(".");
			if(h.Length != 2){
				throw new TebasScriptError("Failed to get string ref " + i + ", incorrect dot format");
			}
			if(getIndex(h[1], h[0], out int u)){
				return new stringRef(h[0], u);
			}
			throw new TebasScriptError("Failed to get string ref " + i);
		}
		throw new TebasScriptError("Failed to get string ref " + i);
	}
	
	string getTableRef(int i){
		string s = sentences[lp].getArg(i);
		
		if(isValidName(s)){
			return s;
		}
		throw new TebasScriptError("Failed to get table ref " + i);
	}
	
	bool getIndex(string s, string t, out int n){
		if(int.TryParse(s, out int u)){
			if(u < 0 && tables.ContainsKey(t)){
				u = tables[t].Count + u;
			}else if(u < 0){
				u = 0;
			}
			
			n = u;
			return true;
		}
		
		if(s == "random"){
			if(tables.ContainsKey(t)){
				n = rand.Next(tables[t].Count);
			}else{
				n = 0;
			}
			return true;
		}
		
		if(s == "center"){
			if(tables.ContainsKey(t)){
				n = tables[t].Count;
				n /= 2;
			}else{
				n = 0;
			}
			return true;
		}
		
		n = -1;
		return false;
	}
	
	bool getIndex(string s, int tl, out int n){
		if(int.TryParse(s, out int u)){
			if(u < 0){
				u = tl + u;
			}
			
			n = u;
			return true;
		}
		
		if(s == "random"){
			n = rand.Next(tl);
			return true;
		}
		
		if(s == "center"){
			n = tl;
			n /= 2;
			return true;
		}
		
		n = -1;
		return false;
	}
	
	void setError(string message){
		tables["error"] = new List<string>();
		
		tables["error"].Add(message);
	}
	
	string expandString(string s){
		StringBuilder sb = new StringBuilder();
		
		bool readingC = false;
		bool previousEscapeCode = false;
		
		StringBuilder c = new StringBuilder();
		
		for(int i = 0; i < s.Length; i++){
			if(readingC){
				if(s[i] != '}'){
					c.Append(s[i]);
				}else{
					readingC = false;
					sb.Append(getString(c.ToString().Trim()));
					c.Clear();
				}
			}else{
				if(s[i] == '{'){
					if(!previousEscapeCode){
						readingC = true;
						continue;	
					}else{
						previousEscapeCode = false;
					}
				}
				
				if(s[i] == '\\' && !previousEscapeCode){
					previousEscapeCode = true;
					continue;
				}else if(previousEscapeCode){
					sb.Append('\\');
					previousEscapeCode = false;
				}
				sb.Append(s[i]);
			}
		}
		
		return sb.ToString();
	}
	
	void setString(stringRef sr, string v){
		if(!tables.ContainsKey(sr.table)){
			tables[sr.table] = new List<string>(Enumerable.Repeat("", sr.index));
			tables[sr.table].Add(v);
			return;
		}
		
		while(tables[sr.table].Count <= sr.index){
			tables[sr.table].Add("");
		}
		
		tables[sr.table][sr.index] = v;
	}
	
	string getPath(string s){
		if(s.StartsWith("W")){
			if(Tebas.workingDirectory != null){
				return Tebas.workingDirectory + s.Substring(1);
			}
			return "";
		}else if((type == ScriptType.Template || type == ScriptType.TemplateGlobal) && s.StartsWith("T")){
			if(Tebas.templateDirectory != null){
				return Tebas.templateDirectory + s.Substring(1);
			}
			return "";
		}
		
		return "";
	}
	
	string getPathExclusive(string s){
		if(s.StartsWith("W")){
			if(Tebas.workingDirectory != null){
				return Tebas.workingDirectory + s.Substring(1);
			}
			return "";
		}else if((type == ScriptType.Template || type == ScriptType.TemplateGlobal) && s.StartsWith("T")){
			if(Tebas.templateDirectory != null){
				return Tebas.templateDirectory + s.Substring(1);
			}
			return "";
		}
		
		return s;
	}
	
	bool fileExistsWildcard(string p){
		if(p.Contains("*") || p.Contains("?")){
			return Directory.GetFiles(Path.GetDirectoryName(p), Path.GetFileName(p)).Length > 0;
		}else{
			return File.Exists(p);
		}
	}
	
	void output(string s){
		if(useColors){
			FormatString fs = formattedName + new FormatString(s, CharFormat.ResetAll);
			Console.Write(fs);
		}else{
			Console.Write(formattedName.content + s);
		}
	}
	
	void outputLine(string s){
		if(useColors){
			FormatString fs = formattedName + new FormatString(s, CharFormat.ResetAll);
			Console.WriteLine(fs);
		}else{
			Console.WriteLine(formattedName.content + s);
		}
	}
	
	void outputErrorLine(string s){
		if(useColors){
			FormatString fs = formattedErrorName + new FormatString(s, CharFormat.ResetAll);
			Console.Error.WriteLine(fs);
		}else{
			Console.Error.WriteLine(formattedErrorName.content + s);
		}
	}
	
	string specialLoad(string s){
		s = s.Substring(1, s.Length - 1);
		
		switch(s){
			case "pn":
			return Tebas.pn;
			
			case "tn":
			return Tebas.tn;
			
			case "wd":
			return Tebas.workingDirectory;
			
			case "td":
			return type == ScriptType.Template || type == ScriptType.TemplateGlobal ? Tebas.templateDirectory : "";
			
			case "pl":
			return type == ScriptType.Plugin ? PluginHandler.runningPlugin : "";
			
			case "d":
			return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
			
			case "h":
			return DateTime.Now.ToString("HH:mm:ss");
			
			case "tbx":
			return System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
			
			case "tbv":
			return Tebas.currentVersion;
			
			case "os":
			return OperatingSystem.IsWindows() ? "windows" : OperatingSystem.IsLinux()   ? "linux" : OperatingSystem.IsMacOS()   ? "macos" : "";
			
			default:
			return "";
		}
	}
	
	string projectRead(string n){
		if(!(Tebas.project is null)){
			if(Tebas.project.CanGetCamp("resources." + n, out string v)){
				return v;
			}else{
				return "";
			}
		}else{
			return "";
		}
	}
	
	void projectWrite(string n, string c){
		if(!(Tebas.project is null)){
			if(c == ""){
				Tebas.project.DeleteCamp("resources." + n);
			}else{
				Tebas.project.SetCamp("resources." + n, c);
			}
			Tebas.project.Save();
		}
	}
	
	void projectAppend(string n, string c){
		if(!(Tebas.project is null)){
			if(Tebas.project.CanGetCamp("resources." + n, out string v)){
				Tebas.project.SetCamp("resources." + n, v + c);
			}else if(c != ""){
				Tebas.project.SetCamp("resources." + n, c);
			}
			Tebas.project.Save();
		}
	}
}

struct Scope{
	public string table;
	public List<string> list;
	
	public Scope(string n, List<string> l){
		table = n;
		list = l;
	}
}

struct stringRef{
	public string table;
	public int index;
	
	public stringRef(string t, int i){
		table = t;
		index = i;
	}
}

enum FlowType{
	If, Else, While, Do, For, Function, Error
}

struct FlowComponent{
	public FlowType ft;
	public int line;
	public int level;
	
	public FlowComponent(FlowType f, int s, int l){
		ft = f;
		line = s;
		level = l;
	}
}

public enum ScriptType{
	Template, TemplateGlobal, Plugin, Standalone
}