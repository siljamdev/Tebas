using System;

internal class TebasError : Exception{
	internal TebasError(string message) : base(message){}
	
	public override string ToString(){
		string f = "Error:\n";
		f += $"Message: {this.Message}\n";
        f += $"Source: {this.Source}\n";
        f += $"Stack Trace: {this.StackTrace}\n";
        f += $"Target Site: {this.TargetSite}\n";
		
		return f;
	}
}

internal class TebasScriptError : TebasError{
	internal TebasScriptError(string message) : base(message){}
	
	public override string ToString(){
		string f = "Script Error:\n";
		f += $"Message: {this.Message}\n";
        f += $"Source: {this.Source}\n";
        f += $"Stack Trace: {this.StackTrace}\n";
        f += $"Target Site: {this.TargetSite}\n";
		
		return f;
	}
}

internal class TebasCriticalError : Exception{
	internal TebasCriticalError(string message) : base(message){}
	
	public override string ToString(){
		string f = "Critical Error:\n";
		f += $"Message: {this.Message}\n";
        f += $"Source: {this.Source}\n";
        f += $"Stack Trace: {this.StackTrace}\n";
        f += $"Target Site: {this.TargetSite}\n";
		
		return f;
	}
}