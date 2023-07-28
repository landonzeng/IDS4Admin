using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace IDS4Admin.Tool
{
	internal class Utility
	{
		public static string CreateConfigureFiles(IDS4Config config)
		{
			if (!Directory.Exists("Output")) Directory.CreateDirectory("Output");
			var path = "Output";
			var templatepath = "templates";
			var folders = Directory.GetDirectories(templatepath);
			foreach (var folder in folders)
			{
				var files = Directory.GetFiles(folder);
				foreach (var file in files)
				{
					var content = File.ReadAllText(file);
					var result = TreateToken(content, config);
					var outpath = "";
					if (folder.Contains("Admin"))
					{
						outpath = Path.Combine(path, "Admin");
					}
					else
					{
						outpath = Path.Combine(path, "STS");
					}
					if (!Directory.Exists(outpath)) Directory.CreateDirectory(outpath);
					var filename = Path.GetFileName(file).Replace("template", "json");
					var temPath = outpath;
					outpath = Path.Combine(outpath, filename);
					File.WriteAllText(outpath, result);

					DirectoryInfo copyPath = new DirectoryInfo(Directory.GetCurrentDirectory());
					var parentPath = copyPath.Parent;
					if (parentPath.FullName.Split('\\').LastOrDefault().Contains("IDS4AdminPublish"))
					{
						var strPath = "";
						if (folder.Contains("Admin"))
						{
							strPath = parentPath.FullName + "\\Admin";
						}
						else
						{
							strPath = parentPath.FullName + "\\STS";
						}
						Console.WriteLine($"{Path.Combine(copyPath.FullName, temPath, filename)}文件拷贝到：{strPath}");
						CopyFileToDir(Path.Combine(copyPath.FullName, temPath), filename, strPath);
					}
				}
			}
			return "Configure files are created. You can find them on Output folder.";
		}


		/// <summary>
		/// 复制文件夹下的所有文件、目录到指定的文件夹
		/// </summary>
		/// <param name="dir">源文件夹地址</param>
		/// <param name="desDir">指定的文件夹地址</param>
		private static void CopyFileToDir(string dir, string fileName, string desDir)
		{
			if (!Directory.Exists(desDir))
			{
				Directory.CreateDirectory(desDir);
			}

			string desPath = Path.Combine(desDir, Path.GetFileName(fileName));
			string socPath = Path.Combine(dir, fileName);
			//如果是文件
			var fileExist = File.Exists(socPath);
			if (fileExist)
			{
				//复制文件到指定目录下                     
				System.IO.File.Copy(socPath, desPath, true);
			}

		}

		private static string TreateToken(string content, IDS4Config config)
		{
			var res = content;
			if (!string.IsNullOrEmpty(content))
			{
				res = res.Replace("[ADMINURI]", config.AdminUri);
				res = res.Replace("[STSURI]", config.STSUri);
				res = res.Replace("[DBTYPE]", config.DbType);
				res = res.Replace("[CONNECTIONSTRING]", config.DbConnString);
				res = res.Replace("[GETSTSURLS]", GetUrls(config.STSUri, config.IsDocker));
				res = res.Replace("[GETADMINURLS]", GetUrls(config.AdminUri, config.IsDocker));
				res = res.Replace("[STSBASEPATH]", GetBasePath(config.STSUri));
				res = res.Replace("[ADMINBASEPATH]", GetBasePath(config.AdminUri));
				res = res.Replace("[ADMINURICORS]", GetCorsPath(config.AdminUri));

				//res = res.Replace("[GETSTSBAEPATH]", GetBasePathCong(config.STSUri));
				//res = res.Replace("[GETADMINBAEPATH]", GetBasePath(config.AdminUri));
			}
			return res;
		}

		private static string GetBasePath(string sTSUri)
		{
			var res = "";
			if (!string.IsNullOrEmpty(sTSUri))
			{
				var arr = sTSUri.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				if (arr.Length > 2) res = @"/" + arr[2];
			}
			return res;
		}
		private static string GetBasePathCong(string sTSUri)
		{
			var res = "";
			if (!string.IsNullOrEmpty(sTSUri))
			{
				var arr = sTSUri.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				if (arr.Length > 2) res = @", ""BasePath"":""/" + arr[2] + @"""";
			}
			return res;
		}
		private static string GetCorsPath(string sTSUri)
		{
			var res = "";
			if (!string.IsNullOrEmpty(sTSUri))
			{
				var arr = sTSUri.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				res = arr[0] + "//" + arr[1];
			}
			return res;
		}

		private static string GetUrls(string sTSUri, string isdocker)
		{
			var res = "";
			if (isdocker == "y" || isdocker == "Y") return res;
			if (!string.IsNullOrEmpty(sTSUri))
			{
				var arr = sTSUri.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				if (arr.Length >= 2) res = @"""urls"":""" + arr[0] + "//" + arr[1] + @"""";
			}
			return res;
		}
	}
}
