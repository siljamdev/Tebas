using System;
using System.Text;
using System.Diagnostics;

public static class ProcessExecuter{
    public static void runProcess(string name, string command, string arguments, string workingDirectory)
    {
        Tebas.initializeConfig();
		
		ProcessStartInfo processInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false
        };

        using (Process process = new Process { StartInfo = processInfo })
        {			
			process.ErrorDataReceived += (sender, args) => {
                if(!string.IsNullOrEmpty(args.Data)){
                    // Display the standard error
                    Console.WriteLine(getErrorName(name) + args.Data);
                }
            };
			
            // Set up event handlers for capturing output and error asynchronously
            process.OutputDataReceived += (sender, args) => {
                if(!string.IsNullOrEmpty(args.Data)){
                    // Display the standard output
                    Console.WriteLine(getName(name) + args.Data);
                }
            };
			
			// Start the process
            process.Start();
			
            // Begin asynchronous reading of the output and error streams
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Wait for the process to exit
            process.WaitForExit();
        }
    }
	
	public static void runProcessNewWindow(string command, string arguments, string workingDirectory)
    {
		ProcessStartInfo processInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            UseShellExecute = true
        };

        using (Process process = new Process { StartInfo = processInfo })
        {
			// Start the process
            process.Start();
        }
    }
	
	public static bool isExecutableInPath(string exeName, out string p){
		// If no extension is provided, add .exe on Windows
		if(!exeName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)){
			exeName += ".exe";
		}
		
		// Get all PATH directories
		string? pathEnv = Environment.GetEnvironmentVariable("PATH");
		if(pathEnv == null){
			p = "";
			return false;
		}
		
		string windowsAppsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Local", "Microsoft", "WindowsApps");
		
		foreach(string path in pathEnv.Split(Path.PathSeparator)){
			string fullPath = path.Trim();
			
			if(fullPath.Equals(windowsAppsDir, StringComparison.OrdinalIgnoreCase)){
				continue;
			}
			
			fullPath = Path.Combine(fullPath, exeName);
			
			if(File.Exists(fullPath)){
				p = fullPath;
				return true;
			}
		}
		
		p = "";
		return false;
	}
	
	static string getName(string n){
		if(Tebas.config.CanGetCamp("processShowsName", out bool b) && b){
			return "[" + n + "] ";
		}
		return "";
	}
	
	static string getErrorName(string n){
		if(Tebas.config.CanGetCamp("processShowsName", out bool b) && b){
			return "[" + n + " ERROR] ";
		}
		return "[ERROR]";
	}
	
	public static void openLink(string url){
		Process.Start(new ProcessStartInfo{
			FileName = url,
			UseShellExecute = true
		});
	}
	
	public static void runProcessWithOutput(string name, string command, string arguments, string workingDirectory, List<string> output, List<string> error)
    {		
		ProcessStartInfo processInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false,
			StandardOutputEncoding = Encoding.Default,
			StandardErrorEncoding = Encoding.Default
        };

        using (Process process = new Process { StartInfo = processInfo })
        {			
			// Start the process
            process.Start();
			
			string o = process.StandardOutput.ReadToEnd();
			output.AddRange(o.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None));
			
			string e = process.StandardError.ReadToEnd();
			error.AddRange(e.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None));

            // Wait for the process to exit
            process.WaitForExit();
        }
    }
}