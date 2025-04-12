using System;
using System.Text;

//Tokenized sentence
class Sentence{
	public string command{get; private set;}
	List<string> args;
	
	bool commentFound;
	
	int cpi;
	string line;
	
	public Sentence(string l){
		line = l.TrimStart();
		
		args = new List<string>();
		
		command = parseText();
		parseWhitespace();
		
		while(cpi < line.Length && !commentFound){
			args.Add(parsePart());
			parseWhitespace();
		}
	}
	
	string parseText(){
		bool previousSlash = false;
		
		StringBuilder t = new StringBuilder();
		
		while(cpi < line.Length){
			if(line[cpi].isWhitespace()){
				return t.ToString();
			}
			
			if(line[cpi] == '/'){
				if(previousSlash){
					t.Remove(t.Length - 1, 1);
					commentFound = true;
					return t.ToString();
				}
				previousSlash = true;
			}
			
			t.Append(line[cpi]);
			
			cpi++;
		}
		
		return t.ToString();
	}
	
	void parseWhitespace(){
		while(cpi < line.Length){
			if(!line[cpi].isWhitespace()){
				break;
			}
			
			cpi++;
		}
	}
	
	string parsePart(){
		if(line[cpi] == 'f' && line[cpi + 1] == '"'){
			cpi += 2;
			return "f" + parseQuote();
		}
		
		if(line[cpi] == '"'){
			cpi++;
			return parseQuote();
		}
		
		if(line[cpi] == '['){
			cpi++;
			return parseTable();
		}
		
		return parseText();
	}
	
	string parseQuote(){
		bool previousEscape = false;
		
		StringBuilder t = new StringBuilder();
		
		while(cpi < line.Length){
			if(line[cpi] == '"'){
				if(previousEscape){
					t.Append(line[cpi]);
					previousEscape = false;
					cpi++;
					continue;
				}
				t.Append(line[cpi]);
				cpi++;
				return "\"" + t.ToString();
			}
			
			if(line[cpi] == 'n' && previousEscape){
				t.Append('\n');
				previousEscape = false;
				cpi++;
				continue;
			}
			
			if(line[cpi] == '\\' && !previousEscape){
				previousEscape = true;
				//t.Append(line[cpi]);
				cpi++;
				continue;
			}else if(previousEscape){
				t.Append('\\');
				previousEscape = false;
			}
			
			t.Append(line[cpi]);
			
			cpi++;
		}
		
		return "\"" + t.ToString();
	}
	
	string parseTable(){
		StringBuilder t = new StringBuilder();
		
		while(cpi < line.Length){
			if(line[cpi] == 'f' && line[cpi + 1] == '"'){
				cpi += 2;
				t.Append("f" + parseQuote());
			}else if(line[cpi] == '"'){
				cpi++;
				t.Append(parseQuote());
			}
			
			if(line[cpi] == ','){
				t.Append(line[cpi]);
				cpi++;
				continue;
			}
			
			if(line[cpi] == ']'){
				t.Append(line[cpi]);
				cpi++;
				return "[" + t.ToString();
			}
			
			cpi++;
		}
		
		return "[" + t.ToString();
	}
	
	public int getNumOfArgs(){
		return args.Count;
	}
	
	public string getArg(int i){
		i -= 1;
		if(i < args.Count){
			return args[i];
		}
		
		throw new TebasScriptError("Failed to get argument: Requested " + (i + 1) + " and maximum possible is " + args.Count);
	}
	
	public override string ToString(){
		StringBuilder sb = new StringBuilder();
		sb.Append(command);
		sb.Append(" ");
		
		for(int i = 0; i < args.Count; i++){
			sb.Append(args[i]);
			if(i != args.Count - 1){
				sb.Append(" ");
			}
		}
		
		return sb.ToString();
	}
}