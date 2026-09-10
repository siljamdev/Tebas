using System;
using System.Text;
using System.Diagnostics;
using TableScript;
using TableScript.Generator;
using TableScript.StandardLibraries;
using Porta.Pty;

[TableScriptLibrary("processexecuter.cs")]
partial class ProcessExecuter{
	#region static
	static ProcessExecuter _dummy = null;
	public static ProcessExecuter Dummy{get{
		if(_dummy == null){
			_dummy = new ProcessExecuter(null, null, false, k => false);
		}
		return _dummy;
	}}
	
	
	static bool hasSeenProcessHint = false;
	
	static void displayProcesshint(){
		if(!hasSeenProcessHint){
			Tebas.hint("To skip this, do 'tebas <template|plugin> permission <name> skipProcessConfirmation allow'");
			hasSeenProcessHint = true;
		}
	}
	
	public static void openUrl(string url){
		try{
			if(OperatingSystem.IsWindows()){
				Process.Start(new ProcessStartInfo{
					FileName = url,
					UseShellExecute = true
				});
			}else if(OperatingSystem.IsLinux()){
				Process.Start("xdg-open", url);
			}else if(OperatingSystem.IsMacOS()){
				Process.Start("open", url);
			}
		}catch(Exception e){
			Tebas.report(e.ToString());
		}
	}
	#endregion
	
	Predicate<string> hasPermission;
	Action<Exception> report;
	
	readonly string basePath;
	readonly string pathName;
	
	public FunctionStmt[] allFuncs => TableScriptFunctions.Select(s => {
		if(s is FunctionExtStmt e){
			return new FunctionExtStmt(e.identifier, e.pars, e.body, e.description?.Replace("PATHNAME", pathName), e.line);
		}
		return s;
	}).ToArray();
	
	public ProcessExecuter(string path, string name, bool isPlugin, Predicate<string> hp){
		basePath = path;
		pathName = name;
		hasPermission = hp;
		
		if(Tebas.config.GetValue<bool>("script.showLabel")){
			report = x => Tebas.labelReport("PROCESS", isPlugin ? Palette.plugin : Palette.template, x.GetType() + ": " + x.Message);
		}else{
			report = x => Tebas.report(x.GetType() + ": " + x.Message);
		}
	}
	
	string getFinalPath(string directory){
		return basePath + "/" + directory;
	}
	
	bool processAllowed(string command, string directory, Table arguments){
		string full = Path.GetFullPath(getFinalPath(directory));
		string normalizedBase = Path.GetFullPath(basePath);
		
		if(!full.StartsWith(normalizedBase + Path.DirectorySeparatorChar) && full != normalizedBase){
			report(new UnauthorizedAccessException("Process directory escape attempt: '" + directory + "'"));
			return false;
		}
		
		if(Tebas.config.GetValue<bool>("script.allowAllProcesses") || (hasPermission != null && hasPermission("skipProcessConfirmation"))){
			return true;
		}
		
		displayProcesshint();
		
		string n = command + (arguments != null ? (" " + string.Join(" ", arguments.contents.Select(a => "\"" + a + "\""))) : "");
		return Tebas.askConfirmation("Do you want to run '" + n + "'?");
	}
	
	/// <summary>
	/// Run a process in the PATHNAME path, printing its output. Returns its exit code as a stdnum num. If any error occurred, an empty table will be returned
	/// </summary>
	[TableScriptFunction]
	public string runProcess(string command, string directory, Table arguments){
		if(!processAllowed(command, directory, arguments)){
			return null;
		}
		
		directory = getFinalPath(directory);
		
		try{
			string name = Path.GetFileNameWithoutExtension(command).ToUpper();
			ProcessStartInfo processInfo = new ProcessStartInfo{
				FileName = command,
				WorkingDirectory = directory,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = false,
				StandardOutputEncoding = Encoding.UTF8,
				StandardErrorEncoding = Encoding.UTF8
			};
			
			foreach(string arg in arguments.contents){
				processInfo.ArgumentList.Add(arg);
			}
			
			using Process process = new Process{StartInfo = processInfo};
			
			//Figure out actions
			Action<string> stdout;
			Action<string> stderr;
			if(Tebas.config.GetValue<bool>("process.showLabel")){
				stdout = t => Tebas.labelOutput(name, Palette.process, t);
				stderr = t => Tebas.labelReport(name, Palette.process, t);
			}else{
				stdout = t => Tebas.output(t);
				stderr = t => Tebas.report(t);
			}
			
			process.OutputDataReceived += (sender, args) => {
				if(!string.IsNullOrEmpty(args.Data)){
					stdout(args.Data);
				}
			};
			
			// Set up event handlers for capturing output and error asynchronously
			process.ErrorDataReceived += (sender, args) => {
				if(!string.IsNullOrEmpty(args.Data)){
					stderr(args.Data);
				}
			};
			
			// Start the process
			process.Start();
			
			// Begin asynchronous reading of the output and error streams
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
		
			// Wait for the process to exit
			process.WaitForExit();
			return process.ExitCode.ToString();
		}catch(Exception e){
			report(e);
			return null;
		}
	}
	
	/// <summary>
	/// Run a process interactively in the PATHNAME path, printing its output. Returns its exit code as a stdnum num. If any error occurred, an empty table will be returned
	/// </summary>
	[TableScriptFunction]
	public string runProcessInteractive(string command, string directory, Table arguments){
		if(!processAllowed(command, directory, arguments)){
			return null;
		}
		
		directory = getFinalPath(directory);
		
		try{
			string name = Path.GetFileNameWithoutExtension(command).ToUpper();
			
			PtyOptions options = new PtyOptions{
				Cols = 120,
				Rows = 30,
				Cwd = directory,
				App = command,
				CommandLine = arguments.contents.ToArray()
			};
			
			using IPtyConnection terminal = PtyProvider.SpawnAsync(options, CancellationToken.None).GetAwaiter().GetResult();
			
			//Figure out actions
			Action<string> stdout;
			if(Tebas.config.GetValue<bool>("process.showLabel")){
				stdout = t => Tebas.labelOutputNoLine(name, Palette.process, t);
			}else{
				stdout = t => Tebas.outputNoLine(t);
			}
			
			Task outputTask = Task.Run(() =>{
				using StreamReader reader = new StreamReader(terminal.ReaderStream, Encoding.UTF8);
				bool first = true;
				
				while (true){
					int value = reader.Read();
					if(value == -1)
						break;
			
					char c = (char)value;
					if (first){
						first = false;
			
						stdout(c.ToString());
					}else{
						Tebas.outputNoLine(c.ToString());
					}
			
					if(c == '\n')
						first = true;
				}
			});
			
			CancellationTokenSource cts = new CancellationTokenSource();
			Task inputTask = Task.Run(() =>{
				Stream input = Console.OpenStandardInput();
				byte[] buffer = new byte[4096];
				while (!cts.Token.IsCancellationRequested){
					int count = input.Read(buffer, 0, buffer.Length);
					if (count == 0)
						break;
					terminal.WriterStream.Write(buffer, 0, count);
				}
			});
		
			// Wait for the process to exit
			terminal.WaitForExit(-1);
			int exitCode = terminal.ExitCode;
			
			cts.Cancel();
			terminal.Dispose();
			//outputTask.GetAwaiter().GetResult();
			
			Tebas.output(""); //Ensure spacing is correct
			return exitCode.ToString();
		}catch(Exception e){
			report(e);
			return null;
		}
	}
	
	/// <summary>
	/// Run a process detached in the PATHNAME path, not printing its output. Returns false if any error occurred
	/// </summary>
	[TableScriptFunction]
	public bool runProcessDetached(string command, string directory, Table arguments){
		if(!processAllowed(command, directory, arguments)){
			return false;
		}
		
		directory = getFinalPath(directory);
		
		try{
			ProcessStartInfo processInfo = new ProcessStartInfo{
				FileName = command,
				WorkingDirectory = directory,
				UseShellExecute = true
			};
			
			foreach(string arg in arguments.contents){
				processInfo.ArgumentList.Add(arg);
			}
	
			using Process process = new Process{StartInfo = processInfo};
			process.Start();
			return true;
		}catch(Exception e){
			report(e);
			return false;
		}
	}
	
	/// <summary>
	/// Run a process in the PATHNAME path, and get its output as a stdlist list [stdout, stderr, exitcode]. Exitcode is a stdnum num. If any error occurred, an empty table will be returned
	/// </summary>
	[TableScriptFunction]
	public Table runProcessWithOutput(string command, string directory, Table arguments){
		if(!processAllowed(command, directory, arguments)){
			return new Table(0);
		}
		
		directory = getFinalPath(directory);
		
		try{
			List<string> output = new();
			List<string> error = new();
			
			ProcessStartInfo processInfo = new ProcessStartInfo{
				FileName = command,
				WorkingDirectory = directory,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = false,
				StandardOutputEncoding = Encoding.UTF8,
				StandardErrorEncoding = Encoding.UTF8
			};
			
			foreach(string arg in arguments.contents){
				processInfo.ArgumentList.Add(arg);
			}
	
			using Process process = new Process{StartInfo = processInfo};
			process.Start();
			
			string[] o = process.StandardOutput.ReadToEnd().Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
			string[] e = process.StandardError.ReadToEnd().Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
			
			// Wait for the process to exit
			process.WaitForExit();
			return StdList.Build(new Table(o), new Table(e), new Table(process.ExitCode.ToString()));
		}catch(Exception e){
			report(e);
			return new Table(0);
		}
	}
	
	/// <summary>
	/// Run a process in the PATHNAME path, not printing its output. Returns its exit code as a stdnum num. If any error occurred, an empty table will be returned
	/// </summary>
	[TableScriptFunction]
	public string runProcessSilent(string command, string directory, Table arguments){
		if(!processAllowed(command, directory, arguments)){
			return null;
		}
		
		directory = getFinalPath(directory);
		
		try{
			ProcessStartInfo processInfo = new ProcessStartInfo{
				FileName = command,
				WorkingDirectory = directory,
				RedirectStandardOutput = true, // Output is redirected but ignored
				RedirectStandardError = true,  // Error is redirected but ignored
				UseShellExecute = false,
				CreateNoWindow = true
			};
			
			foreach(string arg in arguments.contents){
				processInfo.ArgumentList.Add(arg);
			}
			
			using Process process = new Process{StartInfo = processInfo};
			process.Start();
			
			// Wait for the process to exit
			process.WaitForExit();
			return process.ExitCode.ToString();
		}catch(Exception e){
			report(e);
			return null;
		}
	}
	
	/// <summary>
	/// Open a url, folder or file in the PATHNAME path. Returns false if any error occurred
	/// </summary>
	[TableScriptFunction]
	public bool open(string target){
		if(!processAllowed(target, basePath, null)){
			return false;
		}
		
		try{
			if(OperatingSystem.IsWindows()){
				Process.Start(new ProcessStartInfo{
					FileName = target,
					WorkingDirectory = basePath,
					UseShellExecute = true
				});
				return true;
			}else if(OperatingSystem.IsLinux()){
				Process.Start(new ProcessStartInfo{
					FileName = "xdg-open",
					Arguments = target,
					WorkingDirectory = basePath,
				});
				return true;
			}else if(OperatingSystem.IsMacOS()){
				Process.Start(new ProcessStartInfo{
					FileName = "open",
					Arguments = target,
					WorkingDirectory = basePath,
				});
				return true;
			}
			return false;
		}catch(Exception e){
			report(e);
			return false;
		}
	}
}