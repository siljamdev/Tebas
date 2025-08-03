using System;
using System.Text;

public static class StringHelper{	
	//remove surrounding quotes
	public static string removeQuotesSingle(this string p){
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
	
	public static string[] splitLines(this string s){
		return s.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
	}
	
	//split string like cli does it
	public static string[] splitSentence(this string l){
		bool stringOpened = false;
		bool previousEscapeCode = false;
		bool previousSpace = false;
		
		StringBuilder c = new StringBuilder();
		
		List<string> a = new List<string>();
		
		for(int i = 0; i < l.Length; i++){
			if(l[i] == '\"'){
				if(!previousEscapeCode){
					stringOpened = !stringOpened;
					continue;
				}
			}
			
			if(l[i] == '\\' && !previousEscapeCode){
				previousEscapeCode = true;
				continue;
			}else if(previousEscapeCode){
				c.Append('\\');
				previousEscapeCode = false;
			}
			
			if(l[i].isWhitespace() && !stringOpened){
				if(!previousSpace){
					previousSpace = true;
					a.Add(c.ToString());
					c.Clear();
				}
				continue;
			}else{
				previousSpace = false;
			}
			
			c.Append(l[i]);
		}
		
		if(c.Length > 0){
			a.Add(c.ToString());
		}
		
		return a.ToArray();
	}
	
	public static bool isWhitespace(this char c){
		if(char.IsWhiteSpace(c)){
			return true;
		}
		return false;
	}
}