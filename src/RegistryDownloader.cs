using System.Text;
using AshLib.AshFiles;

static class RegistryDownloader{
	public static bool use => Tebas.config.GetValue<bool>("registry.use"); 
	static string registryUrl => Tebas.config.GetValue<string>("registry.url").TrimEnd('/');
	
	static HttpClient getClient(){
		HttpClient client = new HttpClient();
		client.DefaultRequestHeaders.UserAgent.ParseAdd("TebasRegistryDownload");
		client.Timeout = TimeSpan.FromSeconds(15);
		
		return client;
	}
	
	public static AshFile downloadTemplate(string name){
		try{
			string url = registryUrl + "/releases/latest/download/" + name + ".tbtem";
			
			using var client = getClient();
			
			using var response = client.GetAsync(url).GetAwaiter().GetResult();
			if(response.StatusCode == System.Net.HttpStatusCode.NotFound){
				Tebas.report("Template not found in registry: '" + name + "'");
				return null;
			}
			response.EnsureSuccessStatusCode();
			
			using var stream = response.Content.ReadAsStream();
			using var ms = new MemoryStream();
			stream.CopyTo(ms);
			byte[] bytes = ms.ToArray();
			
			AshFile d = AshFile.ReadFromBytes(bytes);
			
			return d;
		}catch(Exception e){
			Tebas.report("A problem occured attempting to download template '" + name + "' from registry: " + e.GetType() + ": " + e.Message);
			return null;
		}
	}
	
	public static AshFile downloadPlugin(string name){
		try{
			string url = registryUrl + "/releases/latest/download/" + name + ".tbplg";
			
			using var client = getClient();
			
			using var response = client.GetAsync(url).GetAwaiter().GetResult();
			if(response.StatusCode == System.Net.HttpStatusCode.NotFound){
				Tebas.report("Plugin not found in registry: '" + name + "'");
				return null;
			}
			response.EnsureSuccessStatusCode();
			
			using var stream = response.Content.ReadAsStream();
			using var ms = new MemoryStream();
			stream.CopyTo(ms);
			byte[] bytes = ms.ToArray();
			
			AshFile d = AshFile.ReadFromBytes(bytes);
			
			return d;
		}catch(Exception e){
			Tebas.report("A problem occured attempting to download plugin '" + name + "' from registry: " + e.GetType() + ": " + e.Message);
			return null;
		}
	}
}