using System;
using System.Text;

//Tokenized sentence
class Sentence{
	
	public string command {get; private set;}
	List<string> args;
	
	int cpi; //Current parsing index
	
	string line;
	bool commentFound;
	
	public Sentence(string s){
		line = s;
		
		command = "";
		args = new List<string>();
		
		parseCommand();
		parseArgs();
	}
	
	//From first non whitespace char till first whitespace
	void parseCommand(){
		cpi = 0;
		commentFound = false;
		
		line = line.TrimStart();
		
		StringBuilder c = new StringBuilder();
		
		bool previousSlash = false;
		
		while(true){
			if(cpi >= line.Length){
				command = c.ToString();
				break;
			}
			
			if(line[cpi].isWhitespace()){
				command = c.ToString();
				break;
			}
			
			if(line[cpi] == '/'){
				if(previousSlash){
					c.Remove(c.Length - 1, 1);
					command = c.ToString();
					cpi--;
					commentFound = true;
					break;
				}
				previousSlash = true;
			}else{
				previousSlash = false;
			}
			
			c.Append(line[cpi]);
			cpi++;
		}
	}
	
	int parseArgs(){
		int c = cpi;
		while(!commentFound){
			if(cpi >= line.Length){
				break;
			}
			
			if(parsePart(out string p)){
				c = cpi;
				args.Add(p);
			}
		}
		return c;
	}
	
	bool parsePart(out string ret){
		if(cpi >= line.Length){
			ret = null;
			return false;
		}
		
		if(line[cpi].isWhitespace()){
			parseWhitespace();
			ret = null;
			return false;
		}
		
		switch(line[cpi]){
			case '\"':
			cpi++;
			ret = parseQuote();
			searchWhitespace();
			return true;
			
			case '[':
			cpi++;
			ret = parseSquare();
			searchWhitespace();
			return true;
			
			case '{':
			cpi++;
			ret = parseCurly();
			searchWhitespace();
			return true;
			
			default:
			if(parseText(out ret)){
				return true;
			}
			return false;
		}
		
		ret = null;
		return false;
	}
	
	void parseWhitespace(){
		while(true){
			if(cpi >= line.Length){
				break;
			}
			
			if(!line[cpi].isWhitespace()){
				break;
			}
			
			cpi++;
		}
		
		return;
	}
	
	bool parseText(out string ret){
		StringBuilder c = new StringBuilder();
		
		bool previousSlash = false;
		bool previousEscapeCode = false;
		bool something = false;
		
		while(true){
			if(cpi >= line.Length){
				break;
			}
			
			if(line[cpi].isWhitespace()){
				break;
			}
			
			if(line[cpi] == '/'){
				if(previousSlash){
					c.Remove(c.Length - 1, 1);
					if(c.Length == 0){
						something = false;
					}
					cpi--;
					commentFound = true;
					break;
				}
				previousSlash = true;
			}else{
				previousSlash = false;
			}
			
			if(line[cpi] == 'n' && previousEscapeCode){
				previousEscapeCode = false;
				c.Append('\n');
				cpi++;
				continue;
			}
			
			if(line[cpi] == '\\' && !previousEscapeCode){
				previousEscapeCode = true;
				cpi++;
				continue;
			}else{
				previousEscapeCode = false;
			}
			
			something = true;
			c.Append(line[cpi]);
			cpi++;
		}
		
		if(something){
			ret = c.ToString();
			return true;
		}
		ret = null;
		return false;
	}
	
	string parseQuote(){
		StringBuilder c = new StringBuilder();
		
		bool previousEscapeCode = false;
		
		while(true){
			if(cpi >= line.Length){
				break;
			}
			
			if(line[cpi] == '\"' && !previousEscapeCode){
				cpi++;
				break;
			}
			
			if(line[cpi] == 'n' && previousEscapeCode){
				previousEscapeCode = false;
				c.Append('\n');
				cpi++;
				continue;
			}
			
			if(line[cpi] == '\\' && !previousEscapeCode){
				previousEscapeCode = true;
				cpi++;
				continue;
			}else{
				previousEscapeCode = false;
			}
			
			c.Append(line[cpi]);
			cpi++;
		}
		
		return c.ToString();
	}
	
	string parseSquare(){
		StringBuilder c = new StringBuilder();
		
		while(true){
			if(cpi >= line.Length){
				break;
			}
			
			if(line[cpi] == ']'){
				cpi++;
				break;
			}
			
			c.Append(line[cpi]);
			cpi++;
		}
		
		return c.ToString();
	}
	
	string parseCurly(){
		StringBuilder c = new StringBuilder();
		
		bool previousEscapeCode = false;
		
		while(true){
			if(cpi >= line.Length){
				break;
			}
			
			if(line[cpi] == '}' && !previousEscapeCode){
				cpi++;
				break;
			}
			
			if(line[cpi] == 'n' && previousEscapeCode){
				previousEscapeCode = false;
				c.Append('\n');
				cpi++;
				continue;
			}
			
			if(line[cpi] == '\\' && !previousEscapeCode){
				previousEscapeCode = true;
				cpi++;
				continue;
			}else{
				previousEscapeCode = false;
			}
			
			c.Append(line[cpi]);
			cpi++;
		}
		
		return c.ToString();
	}
	
	void searchWhitespace(){		
		bool previousSlash = false;
		
		while(true){
			if(cpi >= line.Length){
				break;
			}
			
			if(line[cpi].isWhitespace()){
				break;
			}
			
			if(line[cpi] == '/'){
				if(previousSlash){
					cpi--;
					commentFound = true;
					break;
				}
				previousSlash = true;
			}else{
				previousSlash = false;
			}
			
			cpi++;
		}
	}
	
	public int getNumberOfArgs(){
		return args.Count;
	}
	
	public bool checkEnoughArgs(int r){
		if(r > args.Count){
			return false;
		}
		return true;
	}
	
	public int getIndex(int i){
		string s = args[i];
		
		if(int.TryParse(s, out int u)){
			return u;
		}
		return -1;
	}
	
	public int getIndex(int i, int lp){
		string s = args[i];
		
		if(int.TryParse(s, out int u)){
			return u;
		}
		throw new TebasScriptError("Failed to parse table index(" + s + ") in line " + lp);
	}
	
	public string getLiteral(int i){
		string s = args[i];
		
		return s;
	}
	
	public string[] getList(int i){
		string s = args[i];
		
		List<string> f = new List<string>();
		
		bool stringOpened = false;
		bool previousEscapeCode = false;
		bool whiteReading = false;
		
		StringBuilder c = new StringBuilder();
		StringBuilder w = new StringBuilder();
		
		for(int j = 0; j < s.Length; j++){
			if(s[j] == '\"' && !previousEscapeCode){
				stringOpened = !stringOpened;
				if(stringOpened){
					w = new StringBuilder();
				}
				continue;
			}
			
			if(s[j] == ',' && !stringOpened){
				f.Add(c.ToString());
				c = new StringBuilder();
				w = new StringBuilder();
				previousEscapeCode = false;
				whiteReading = false;
				continue;
			}
			
			if(s[j] == '\\'){
				previousEscapeCode = true;
				continue;
			}else{
				previousEscapeCode = false;
			}
			
			if(s[j].isWhitespace() && !stringOpened){
				if(whiteReading){
					w.Append(s[j]);
				}
				continue;
			}
			
			if(w.Length > 0 && whiteReading && !stringOpened){
				c.Append(w.ToString());
				w = new StringBuilder();
			}
			
			c.Append(s[j]);
			whiteReading = true;
		}
		
		if(c.Length > 0){
			f.Add(c.ToString());
		}
		
		return f.ToArray();
	}
	
	public string getContinuous(){
		cpi = 0;
		parseCommand();
		
		parseWhitespace();
		int argStart = cpi;
		int argFinish = parseArgs();
		
		return StringHelper.removeQuotesSingle(line.Substring(argStart, argFinish - argStart));
	}
	
	public override string ToString(){
		string s = "Line: ";
		s += "\"" + line + "\"";
		s += "\nCommand: ";
		s += "\"" + command + "\"";
		s += "\nArgs (" + args.Count + "):";
		
		foreach(string t in args){
			s += "\n\"" + t + "\"";
		}
		
		return s;
	}
}