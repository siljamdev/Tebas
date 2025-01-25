using System;
using System.Text;

public class Script{
	static readonly char[] invalidNameChars = "[]\"(),{}%".ToCharArray();
	string name;
	
	string[] lines;
	Sentence[] sentences;
	Dictionary<string, int> functions;
	
	Dictionary<string, List<string>> tables;
	
	int lp; //linepointer;
	
	Random rand;
	
	public Script(string name, string code){
		//code = code.Replace("\\n", "\n");
		
		this.name = name;
		
		Tebas.initializeConfig();
		
		lines = code.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
		
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
						functions.Add(sentences[i].getArg(1), i);
					}
				}
			}catch(Exception){
				
			}
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
					ProcessExecuter.runProcess(name + " CMD", "cmd",  "/c " + getString(1), Tebas.workingDirectory);
					break;
					
					case "process.run":
					ProcessExecuter.runProcess(name + " " + getString(1).ToUpper(), getString(1),  getString(2), Tebas.workingDirectory);
					break;
					
					case "process.runOutput":
					tables[getTableRef(3)] = new List<string>();
					tables[getTableRef(4)] = new List<string>();
					ProcessExecuter.runProcessWithOutput(name + " " + getString(1).ToUpper(), getString(1),  getString(2), Tebas.workingDirectory, tables[getTableRef(3)], tables[getTableRef(4)]);
					break;
					
					case "console.print":
					outputLine(expandString(getString(1)));
					break;
					
					case "console.printNoExpand":
					outputLine(getString(1));
					break;
					
					case "console.ask":
					output(getString(2));
					setString(getStringRef(1), Console.ReadLine());
					break;
					
					case "console.pause":
					output(getString(1));
					Console.ReadKey();
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
					
					case "string.substring":
					setString(getStringRef(1), (int.TryParse(getString(3), out int u) && int.TryParse(getString(4), out int v) && u > 0 && u < getString(2).Length && v > 0 && u + v < getString(2).Length ? getString(2).Substring(u, v) : ""));
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
					setString(getStringRef(1), (int.TryParse(getString(2), out u) && int.TryParse(getString(3), out v) && u > 0 && u < getString(2).Length && v > 0 && u + v < getString(2).Length ? getString(1).Substring(u, v) : ""));
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
					if(tables.ContainsKey(getTableRef(2)) && getIndex(getString(3), getTableRef(2), out u)){
						setString(getStringRef(1), tables[getTableRef(2)][u]);
					}else{
						setString(getStringRef(1), "");
					}
					break;
					
					case "table.setAt":
					if(getIndex(getString(2), getTableRef(1), out u)){
						setString(new stringRef(getTableRef(1), u), getString(3));
					}
					break;
					
					case "table.delete":
					if(tables.ContainsKey(getStringRef(1).table) && getIndex(getStringRef(1).index.ToString(), getStringRef(1).table, out u) && u > 0 && u < tables[getStringRef(1).table].Count){
						tables[getStringRef(1).table].RemoveAt(u);
					}
					break;
					
					case "table.deleteAt":
					if(tables.ContainsKey(getTableRef(1)) && getIndex(getString(2), getTableRef(1), out u) && u > 0 && u < tables[getStringRef(1).table].Count){
						tables[getTableRef(1)].RemoveAt(u);
					}
					break;
					
					case "table.insert":
					if(tables.ContainsKey(getStringRef(1).table) && getIndex(getStringRef(1).index.ToString(), getStringRef(1).table, out u) && u > 0 && u < tables[getStringRef(1).table].Count){
						tables[getStringRef(1).table].Insert(u, getString(2));
					}else{
						setString(getStringRef(1), getString(2));
					}
					break;
					
					case "table.insertAt":
					if(tables.ContainsKey(getTableRef(1)) && getIndex(getString(2), getTableRef(1), out u) && u > 0 && u < tables[getStringRef(1).table].Count){
						tables[getTableRef(1)].Insert(u, getString(3));
					}else if(getIndex(getString(2), getTableRef(1), out u)){
						setString(new stringRef(getTableRef(1), u), getString(3));
					}
					break;
					
					case "table.contains":
					setString(getStringRef(1), (getTable(2).Contains(getString(3)) ? tables["true"][0] : tables["false"][0]));
					break;
					
					case "table.clear":
					if(tables.ContainsKey(getTableRef(1))){
						tables[getTableRef(1)].Clear();
					}
					break;
					
					case "table.add":
					if(tables.ContainsKey(getTableRef(1))){
						tables[getTableRef(1)].Add(getString(2));
					}else{
						setString(new stringRef(getTableRef(1), 0), getString(2));
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
					setString(getStringRef(1), ((tables["true"].Contains(getString(2)) && tables["true"].Contains(getString(3)) ? tables["true"][0] : tables["false"][0])));
					break;
					
					case "bool.or":
					setString(getStringRef(1), ((tables["true"].Contains(getString(2)) || tables["true"].Contains(getString(3)) ? tables["true"][0] : tables["false"][0])));
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
					
					case "folder.create":
					if(!Directory.Exists(getPath(getString(1)))){
						Directory.CreateDirectory(getPath(getString(1)));
					}
					break;
					
					case "folder.delete":
					if(Directory.Exists(getPath(getString(1)))){
						Directory.Delete(getPath(getString(1)));
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
					setString(getStringRef(1), TemplateHandler.resourceRead(getString(2)));
					break;
					
					case "template.write":
					TemplateHandler.resourceWrite(getString(1), getString(2));
					break;
					
					case "template.append":
					TemplateHandler.resourceAppend(getString(1), getString(2));
					break;
					
					case "template.run":
					TemplateHandler.runScript(getString(1), StringHelper.splitSentence(getString(2)));
					break;
					
					case "template.installed":
					setString(getStringRef(1), (TemplateHandler.exists(getString(2)) ? tables["true"][0] : tables["false"][0]));
					break;
					
					case "template.list":
					tables[getTableRef(1)] = TemplateHandler.getList();
					break;
					
					case "template.create":
					CreatorUtility.template(getString(1));
					break;
					
					case "plugin.read":
					setString(getStringRef(1), PluginHandler.readResource(getString(2), getString(3)));
					break;
					
					case "plugin.write":
					PluginHandler.writeResource(getString(1), getString(2), getString(3));
					break;
					
					case "plugin.append":
					PluginHandler.appendResource(getString(1), getString(2), getString(3));
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
					CreatorUtility.plugin(getString(1));
					break;
					
					case "project.read":
					setString(getStringRef(1), projectRead(getString(2)));
					break;
					
					case "project.write":
					projectWrite(getString(1), getString(2));
					break;
					
					case "project.append":
					projectAppend(getString(1), getString(2));
					break;
					
					case "tebas.commit":
					Tebas.localCommit(getString(1));
					break;
					
					case "tebas.push":
					Tebas.localPush(getString(1));
					break;
					
					case "tebas.pull":
					Tebas.localPull(getString(1));
					break;
					
					case "tebas.add":
					Tebas.localAdd();
					break;
					
					case "tebas.remoteSet":
					Tebas.localRemoteSet(getString(1), getString(2));
					break;
					
					case "tebas.remoteDelete":
					Tebas.localRemoteDelete(getString(1));
					break;
					
					case "tebas.remoteList":
					tables[getTableRef(1)] = GitHelper.getAllRemotes();
					break;
					
					case "tebas.remoteExists":
					setString(getStringRef(1), (GitHelper.remoteExists(getString(2)) ? tables["true"][0] : tables["false"][0]));
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
					if(s.getArg(2) != "{"){
						throw new TebasScriptError("Incorrect while loop");
					}
					if(tables["true"].Contains(getString(1))){
						flow.Push(new FlowComponent(FlowType.While, lp, flowLevel));
						flowLevel++;
					}else{
						lp++;
						searchEndBlock();
					}
					break;
					
					case "while!":
					if(s.getArg(2) != "{"){
						throw new TebasScriptError("Incorrect while! loop");
					}
					if(!tables["true"].Contains(getString(1))){
						flow.Push(new FlowComponent(FlowType.While, lp, flowLevel));
						flowLevel++;
					}else{
						lp++;
						searchEndBlock();
					}
					break;
					
					case "do":
					if(s.getArg(2) != "{"){
						throw new TebasScriptError("Incorrect do loop");
					}
					
					flow.Push(new FlowComponent(FlowType.Do, lp, flowLevel));
					flowLevel++;
					break;
					
					case "do!":
					if(s.getArg(2) != "{"){
						throw new TebasScriptError("Incorrect do! loop");
					}
					
					flow.Push(new FlowComponent(FlowType.DoNeg, lp, flowLevel));
					flowLevel++;
					break;
					
					case "continue":
					while(true){
						FlowComponent fc = flow.Pop();
						if(fc.ft == FlowType.While || fc.ft == FlowType.Do || fc.ft == FlowType.DoNeg){
							flow.Push(fc);
							lp = fc.line;
							lp++;
							searchEndBlock();
							lp--;
							break;
						}
					}
					break;
					
					case "break":
					while(true){
						FlowComponent fc = flow.Pop();
						if(fc.ft == FlowType.While || fc.ft == FlowType.Do || fc.ft == FlowType.DoNeg){
							lp = fc.line;
							lp++;
							searchEndBlock();
							break;
						}
					}
					break;
					
					case "if":
					if(s.getArg(2) != "{"){
						throw new TebasScriptError("Incorrect if statement");
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
					break;
					
					case "if!":
					if(s.getArg(2) != "{"){
						throw new TebasScriptError("Incorrect if! statement");
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
					}
					break;
					
					case "exit":
					return;
					
					case "function": //Only execute body
					return;
					
					case "return":
					while(true){
						FlowComponent fc = flow.Pop();
						if(fc.ft == FlowType.Function){
							Scope sco = scope.Pop();
							tables[sco.table] = sco.list;
							lp = fc.line;
							break;
						}
					}
					break;
					
					case "}":
					if(flow.Peek().ft == FlowType.While){
						lp = flow.Peek().line - 1;
						flow.Pop();
						flowLevel--;
					}else if(flow.Peek().ft == FlowType.Do){
						FlowComponent fc = flow.Peek();
						if(tables["true"].Contains(getStringAll(sentences[fc.line].getArg(1)))){
							lp = fc.line;
						}else{
							flow.Pop();
							flowLevel--;
						}
					}else if(flow.Peek().ft == FlowType.DoNeg){
						FlowComponent fc = flow.Peek();
						if(!tables["true"].Contains(getStringAll(sentences[fc.line].getArg(1)))){
							lp = fc.line;
						}else{
							flow.Pop();
							flowLevel--;
						}
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
						
						lp = flow.Pop().line;
					}
					break;
					
					/* case "ask":
					output(replace(s.getContinuous()));
					table.Add(Console.ReadLine());
					break;
					
					case "print":
					outputLine(replace(s.getContinuous()));
					break;
					
					case "run":
					checkIfEnoughArgs(2);
					ProcessExecuter.runProcess(name + " " + Path.GetFileName(getPath(0)), getPath(0),  getLiteral(1), Tebas.workingDirectory);
					break;
					
					case "load":
					checkIfEnoughArgs(1);
					sentenceLoad();
					break;
					
					case "set.literal":
					checkIfEnoughArgs(1);
					table.Add(getLiteral(0));
					break;
					
					case "set.lower":
					checkIfEnoughArgs(1);
					table.Add(getTableArg(0).ToLower());
					break;
					
					case "set.upper":
					checkIfEnoughArgs(1);
					table.Add(getTableArg(0).ToUpper());
					break;
					
					case "set.choose":
					checkIfEnoughArgs(4);
					string h = getTableArg(0);
					string[] vals = s.getList(1);
					if(vals.Contains(h)){
						table.Add(getLiteral(2));
					}else{
						table.Add(getLiteral(3));
					}
					break;
					
					case "set.sumup":
					checkIfEnoughArgs(1);
					h = getTableArg(0);
					if(int.TryParse(h, out int b)){
						table.Add((b+1).ToString());
						break;
					}
					table.Add("");
					break;
					
					case "set.sumdown":
					checkIfEnoughArgs(1);
					h = getTableArg(0);
					if(int.TryParse(h, out b)){
						table.Add((b-1).ToString());
						break;
					}
					table.Add("");
					break;
					
					case "set.replace":
					checkIfEnoughArgs(1);
					table.Add(replace(getTableArg(0)));
					break;
					
					case "set.copy":
					checkIfEnoughArgs(1);
					table.Add(getTableArg(0));
					break;
					
					case "goto":
					checkIfEnoughArgs(1);
					jumpToLabel(getLiteral(0));
					break;
					
					case "branch":
					checkIfEnoughArgs(3);
					h = getTableArg(0);
					vals = s.getList(1);
					if(vals.Contains(h)){
						jumpToLabel(getLiteral(2));
					}
					break;
					
					case "pause":
					Console.ReadKey();
					break;
					
					case "exit":
					return;
					
					case "table.empty":
					table = new List<string>();
					break;
					
					case "file.read":
					checkIfEnoughArgs(1);
					string p = getPath(0);
					if(p != "" && File.Exists(p)){
						table.Add(File.ReadAllText(p));
					}else{
						table.Add("");
					}
					break;
					
					case "file.create":
					checkIfEnoughArgs(1);
					p = getPath(0);
					if(p != "" && !File.Exists(p)){
						File.Create(p).Dispose();
					}
					break;
					
					case "file.delete":
					checkIfEnoughArgs(1);
					p = getPath(0);
					if(p != "" && File.Exists(p)){
						File.Delete(p);
					}
					break;
					
					case "file.rename":
					checkIfEnoughArgs(2);
					p = getPath(0);
					string p2 = p = getPath(1);
					if(p != "" && p2 != "" && File.Exists(p) && !File.Exists(p2)){
						File.Move(p, p2);
					}
					break;
					
					case "file.write":
					checkIfEnoughArgs(2);
					p = getPath(0);
					if(p != "" && File.Exists(p)){
						File.WriteAllText(p, getTableArg(1));
					}
					break;
					
					case "file.append":
					checkIfEnoughArgs(2);
					p = getPath(0);
					if(p != "" && File.Exists(p)){
						File.AppendAllText(p, getTableArg(1));
					}
					break;
					
					case "file.exists":
					checkIfEnoughArgs(1);
					p = getPath(0);
					if(p != "" && File.Exists(p)){
						table.Add("1");
						break;
					}
					table.Add("0");
					break;
					
					case "folder.create":
					checkIfEnoughArgs(1);
					p = getPath(0);
					if(p != "" && !Directory.Exists(p)){
						Directory.CreateDirectory(p);
					}
					break;
					
					case "folder.delete":
					checkIfEnoughArgs(1);
					p = getPath(0);
					if(p != "" && Directory.Exists(p)){
						Directory.Delete(p);
					}
					break;
					
					case "folder.rename":
					checkIfEnoughArgs(2);
					p = getPath(0);
					p2 = getPath(1);
					if(p != "" && p2 != "" && Directory.Exists(p) && !Directory.Exists(p2)){
						Directory.Move(p, p2);
					}
					break;
					
					case "folder.exists":
					checkIfEnoughArgs(1);
					p = getPath(0);
					if(p != "" && Directory.Exists(p)){
						table.Add("1");
						break;
					}
					table.Add("0");
					break;
					
					case "template.read":
					checkIfEnoughArgs(1);
					sentenceTemplateRead();
					break;
					
					case "template.write":
					checkIfEnoughArgs(2);
					sentenceTemplateWrite();
					break;
					
					case "template.append":
					checkIfEnoughArgs(2);
					sentenceTemplateAppend();
					break;
					
					case "template.run":
					checkIfEnoughArgs(1);
					TemplateHandler.runScript(getLiteral(0));
					break;
					
					case "project.read":
					checkIfEnoughArgs(1);
					sentenceProjectRead();
					break;
					
					case "project.write":
					checkIfEnoughArgs(2);
					sentenceProjectWrite();
					break;
					
					case "project.append":
					checkIfEnoughArgs(2);
					sentenceProjectAppend();
					break; */
				}
			}catch(TebasScriptError tse){
				Console.WriteLine("The scripts contained an error in line " + lp + ": \"" + sentences[lp].ToString() + "\"");
				Console.WriteLine(tse);
				
				setError(tse.ToString());
			}catch(Exception e){
				Console.WriteLine("An error occured in line " + lp + ": \"" + sentences[lp].ToString() + "\"");
				Console.WriteLine(e);
				
				setError(e.ToString());
			}finally{
				lp++;
			}	
		}
	}
	
	void searchEndBlock(){
		int c = 1;
		while(true){
			if(sentences[lp].command == "if" || sentences[lp].command == "while" || sentences[lp].command == "do" || sentences[lp].command == "if!" || sentences[lp].command == "while!" || sentences[lp].command == "do!"){
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
		}else if(s.StartsWith("T")){
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
		}else if(s.StartsWith("T")){
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
		Console.Write(getName() + s);
	}
	
	void outputLine(string s){
		Console.WriteLine(getName() + s);
	}
	
	string getName(){
		if(Tebas.config.CanGetCampAsBool("scriptShowsName", out bool b) && b){
			return "[" + name + "] ";
		}
		return "";
	}
	
	string getErrorName(){
		if(Tebas.config.CanGetCampAsBool("scriptShowsName", out bool b) && b){
			return "[" + name + " ERROR] ";
		}
		return "[ERROR]";
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
			return Tebas.templateDirectory;
			
			case "pl":
			return PluginHandler.runningPlugin;
			
			case "d":
			return DateTime.Now.ToString();
			
			default:
			return "";
		}
	}
	
	string projectRead(string n){
		if(!(Tebas.project is null)){
			if(Tebas.project.CanGetCampAsString("resources." + n, out string v)){
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
			Tebas.project.SetCamp("resources." + n, c);
			Tebas.project.Save();
		}
	}
	
	void projectAppend(string n, string c){
		if(!(Tebas.project is null)){
			if(Tebas.project.CanGetCampAsString("resources." + n, out string v)){
				Tebas.project.SetCamp("resources." + n, v + c);
				Tebas.project.Save();
			}else{
				Tebas.project.SetCamp("resources." + n, c);
				Tebas.project.Save();
			}
		}
	}
	
	/* void jumpToLabel(string b){
		
		if(b.Length > 0 && b[0] == ':'){
			b = b.Substring(1, b.Length - 1);
		}
		
		if(labels.ContainsKey(b)){
			lp = labels[b];
		}
	}
	
	//Other thingiesssss, not individual sentences
	
	string replace(string s){
		StringBuilder sb = new StringBuilder();
		
		bool previousEscapeCode = false;
		bool readingC = false;
		
		StringBuilder c = new StringBuilder();
		
		for(int i = 0; i < s.Length; i++){
			if(readingC){
				if(s[i] != ']'){
					c.Append(s[i]);
				}else{
					readingC = false;
					if(int.TryParse(c.ToString(), out int u)){
						sb.Append(getTableValue(u));
					}
					c = new StringBuilder();
				}
			}else{
				if(s[i] == '[' && !previousEscapeCode){
					readingC = true;
					continue;
				}
				
				if(s[i] == '\\' && !previousEscapeCode){
					previousEscapeCode = true;
					continue;
				}else{
					previousEscapeCode = false;
				}
				sb.Append(s[i]);
			}
		}
		
		return sb.ToString();
	}
	
	string getLiteral(int n){
		string s = sentences[lp].getLiteral(n);
		
		return replace(s);
	}
	
	string getPath(int n){
		string s = sentences[lp].getLiteral(n);
		s = replace(s);
		
		if(s.StartsWith("W/")){
			if(Tebas.workingDirectory != null){
				return Tebas.workingDirectory + s.Substring(1);
			}
			return "";
		}else if(s.StartsWith("T/")){
			if(Tebas.templateDirectory != null){
				return Tebas.templateDirectory + s.Substring(1);
			}
			return "";
		}
		
		return s;
	}
	
	string getTableValue(int u){
		if(u < 0){
			u = table.Count + u;
		}
		
		if(u > -1 && u < table.Count){
			return table[u];
		}
		return "";
	}
	
	string getTableArg(int i){
		int u = sentences[lp].getIndex(i, lp);
		return getTableValue(u);
	}
	
	//==================00
	void output(string s){
		Console.Write(getName() + s);
	}
	
	void outputLine(string s){
		Console.WriteLine(getName() + s);
	}
	
	
	string getName(){
		if(Tebas.config.CanGetCampAsBool("scriptShowsName", out bool b) && b){
			return "[" + name + "] ";
		}
		return "";
	}
	
	string getErrorName(){
		if(Tebas.config.CanGetCampAsBool("scriptShowsName", out bool b) && b){
			return "[" + name + " ERROR] ";
		}
		return "[ERROR]";
	} */
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
	If, Else, While, Do, DoNeg, Function
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