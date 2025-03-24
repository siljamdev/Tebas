using System;
using System.Text;

public static class StringHelper{
	
	static readonly char[] whitespace = {' ', '\n', '\r', '\t'};
	
	//remove surrounding quotes
	public static string removeQuotesSingle(string p){
		p = p.Trim();
		
		if(p.Length < 1){
			return p;
		}
		char[] c = p.ToCharArray();
		if(c[0] == '\"' && c[c.Length - 1] == '\"'){
			if(c.Length < 2){
				return "";
			}
			return p.Substring(1, p.Length - 2);
		}
		return p;
	}
	
	//split strinbg like cli does it
	public static string[] splitSentence(string l){
		bool stringOpened = false;
		bool previousEscapeCode = false;
		bool previousSpace = false;
		int startIndex = 0;
		List<string> a = new List<string>();
		
		l += " ";
		
		for(int i = 0; i < l.Length; i++){
			if(l[i] == '\"'){
				if(!previousEscapeCode){
					stringOpened = !stringOpened;
				}
			}
			
			if(l[i] == '\\' && !previousEscapeCode){
				previousEscapeCode = true;
			}else{
				previousEscapeCode = false;
			}
			
			if(l[i].isWhitespace() && !stringOpened){
				if(!previousSpace){
					previousSpace = true;
					a.Add(l.Substring(startIndex, i - startIndex));
				}
				startIndex = i + 1;
			}else{
				previousSpace = false;
			}
		}
		
		return a.ToArray();
	}
	
	public static bool isWhitespace(this char c){
		if(whitespace.Contains(c)){
			return true;
		}
		return false;
	}
}