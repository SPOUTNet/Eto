﻿using Eto.Designer;
using Eto.Designer.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Eto.Addin.VisualStudio
{
	public class BuilderInfo
	{
		public string Extension { get; set; }

		public string DesignExtension { get; set; }

		public Func<IInterfaceBuilder> CreateBuilder { get; set; }

		public Func<string, string> GetCodeFile { get; set; }
		public Func<string, string> GetDesignFile { get; set; }

		static List<BuilderInfo> builders = new List<BuilderInfo>
		{
			new BuilderInfo
			{ 
				Extension = ".eto.cs", 
				DesignExtension = "^.+(?<![jx]eto)[.]cs$",
				CreateBuilder = () => new CSharpInterfaceBuilder { InitializeAssembly = Assembly.GetExecutingAssembly().Location },
				GetCodeFile = fileName => Regex.Replace(fileName, @"(.+)[.]eto([.]cs)", "$1$2"),
				GetDesignFile = fileName => Regex.Replace(fileName, @"(.+)([.]cs)", "$1.eto$2")
			},
			new BuilderInfo
			{ 
				Extension = ".eto.vb", 
				DesignExtension = "^.+(?<![jx]eto)[.]vb$",
				CreateBuilder = () => new VbInterfaceBuilder { InitializeAssembly = Assembly.GetExecutingAssembly().Location },
				GetCodeFile = fileName => Regex.Replace(fileName, @"(.+)[.]eto([.]vb)", "$1$2"),
				GetDesignFile = fileName => Regex.Replace(fileName, @"(.+)([.]vb)", "$1.eto$2")
			},
			new BuilderInfo
			{
				Extension = ".xeto",
				DesignExtension = "^.+[.]xeto[.]cs$",
				CreateBuilder = () => new XamlInterfaceBuilder(),
				GetCodeFile = fileName => fileName + ".cs",
				GetDesignFile = fileName => Regex.Replace(fileName, @"(.+[.]xeto)[.]cs", "$1")
			},
			new BuilderInfo
			{
				Extension = ".jeto",
				DesignExtension = "^.+[.]jeto[.]cs$",
				CreateBuilder = () => new JsonInterfaceBuilder(),
				GetCodeFile = fileName => fileName + ".cs",
				GetDesignFile = fileName => Regex.Replace(fileName, @"(.+[.]jeto)[.]cs", "$1")
			}
		};

		public static bool Supports(string fileName)
		{
			return builders.Any(r => fileName.EndsWith(r.Extension, StringComparison.OrdinalIgnoreCase));
		}

		public static BuilderInfo Find(string fileName)
		{
			foreach (var item in builders)
			{
				if (fileName.EndsWith(item.Extension, StringComparison.OrdinalIgnoreCase))
					return item;
			}
			return null;
		}
		public static BuilderInfo FindCodeBehind(string fileName)
		{
			foreach (var item in builders)
			{
				if (Regex.IsMatch(fileName, item.DesignExtension, RegexOptions.IgnoreCase))
					return item;
			}
			return null;
		}

		public static bool IsCodeBehind(string fileName)
		{
			return builders.Any(r => Regex.IsMatch(fileName, r.DesignExtension, RegexOptions.IgnoreCase));
		}
	}
}
