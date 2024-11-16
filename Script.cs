using System;
using System.Text;

public class Script{
	string name;
	
	string[] lines;
	Sentence[] sentences;
	Dictionary<string, int> labels;
	
	List<string> table;
	
	int lp; //linepointer;
	
	int lastError;
	
	public Script(string name, string code){
		//code = code.Replace("\\n", "\n");
		
		this.name = name;
		
		Tebas.initializeConfig();
		
		lines = code.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
		
		sentences = new Sentence[lines.Length];
		
		for(int i = 0; i < lines.Length; i++){
			sentences[i] = new Sentence(lines[i]);
		}
		
		labels = new Dictionary<string, int>();
		
		for(int i = 0; i < sentences.Length; i++){
			if(sentences[i].command.Length > 0 && sentences[i].command[0] == ':'){
				labels.Add(sentences[i].command.Substring(1), i);
			}
		}
	}
	
	public void run(){
		table = new List<string>();
		
		lp = 0;
		lastError = 0;
		
		while(lp < lines.Length){
			try{
				Sentence s =  sentences[lp];
				
				switch(s.command){
					case "cmd":
					checkIfEnoughArgs(1);
					ProcessExecuter.runProcess(name + " CMD", "cmd",  "/c " + replace(s.getContinuous()), Tebas.workingDirectory);
					break;
					
					case "ask":
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
					break;
				}
				lastError = 0;
			}catch(TebasScriptError tse){
				Console.WriteLine("The scripts contained an error:");
				Console.WriteLine(tse);
				
				if(lastError == 0){
					lastError = 2;
				}
			}catch(Exception e){
				Console.WriteLine("An error occured:");
				Console.WriteLine(e);
				
				lastError = 3;
			}finally{
				lp++;
			}	
		}
	}
	
	void sentenceLoad(){
		string s = getLiteral(0);
		
		if(s.Length > 0 && s[0] == '%'){
			s = s.Substring(1, s.Length - 1);
		}
		
		switch(s){
			case "p":
			case "pn":
			table.Add(Tebas.pn);
			break;
			
			case "tn":
			table.Add(Tebas.tn);
			break;
			
			case "w":
			case "wd":
			table.Add(Tebas.workingDirectory);
			break;
			
			case "t":
			case "td":
			table.Add(Tebas.templateDirectory);
			break;
			
			case "lt":
			table.Add(table.Count.ToString());
			break;
			
			case "d":
			table.Add(DateTime.Now.ToString());
			break;
			
			case "e":
			table.Add(lastError.ToString());
			break;
			
			default:
			table.Add("");
			break;
		}
	}
	
	void sentenceTemplateRead(){
		string n = getLiteral(0);
		
		if(!(Tebas.template is null)){
			if(Tebas.template.CanGetCampAsString("resources." + n, out string v)){
				table.Add(v);
			}else{
				table.Add("");
			}
		}else{
			table.Add("");
		}
	}
	
	void sentenceTemplateWrite(){
		string n = getLiteral(0);
		
		if(!(Tebas.template is null)){
			Tebas.template.SetCamp("resources." + n, getTableArg(1));
			Tebas.template.Save();
		}
	}
	
	void sentenceTemplateAppend(){
		string n = getLiteral(0);
		
		if(!(Tebas.template is null)){
			if(Tebas.template.CanGetCampAsString("resources." + n, out string v)){
				Tebas.template.SetCamp("resources." + n, v + getTableArg(1));
				Tebas.template.Save();
			}else{
				Tebas.template.SetCamp("resources." + n, getTableArg(1));
				Tebas.template.Save();
			}
		}
	}
	
	void sentenceProjectRead(){
		string n = getLiteral(0);
		
		if(!(Tebas.project is null)){
			if(Tebas.project.CanGetCampAsString("resources." + n, out string v)){
				table.Add(v);
			}else{
				table.Add("");
			}
		}else{
			table.Add("");
		}
	}
	
	void sentenceProjectWrite(){
		string n = getLiteral(0);
		
		if(!(Tebas.project is null)){
			Tebas.project.SetCamp("resources." + n, getTableArg(1));
			Tebas.project.Save();
		}
	}
	
	void sentenceProjectAppend(){
		string n = getLiteral(0);
		
		if(!(Tebas.project is null)){
			if(Tebas.project.CanGetCampAsString("resources." + n, out string v)){
				Tebas.project.SetCamp("resources." + n, v + getTableArg(1));
				Tebas.project.Save();
			}else{
				Tebas.project.SetCamp("resources." + n, getTableArg(1));
				Tebas.project.Save();
			}
		}
	}
	
	void jumpToLabel(string b){
		
		if(b.Length > 0 && b[0] == ':'){
			b = b.Substring(1, b.Length - 1);
		}
		
		if(labels.ContainsKey(b)){
			lp = labels[b];
		}
	}
	
	//Other thingiesssss, not individual sentences
	
	void checkIfEnoughArgs(int n){
		if(!sentences[lp].checkEnoughArgs(n)){
			lastError = 1;
			throw new TebasScriptError("Not enough arguments(" + sentences[lp].getNumberOfArgs + "/" + n + ") for sentence in line " + lp);
		}
	}
	
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
	}
}