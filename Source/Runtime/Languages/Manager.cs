/* 
 * File: Manager.cs
 *
 * This source file contains the declaration of the LanguageManager class
 * which is responsible for loading, saving and minipulating the current 
 * language pack.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Collections;
using System.Xml;
using BinaryPhoenix.Fusion.Runtime.Debug;

namespace BinaryPhoenix.Fusion.Runtime.Languages
{

	/// <summary>
	///		Responsible for loading, saving and minipulating the current 
	///		language pack.
	/// </summary>
	public static class LanguageManager
	{
		#region Members
		#region Variables

		private static string _name = "";
		private static Hashtable _captionTable = new Hashtable();

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the name of the current language pack.
		/// </summary>
		public static string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		///		Gets or sets the list of captions contained in the current language pack.
		/// </summary>
		public static Hashtable CaptionTable
		{
			get { return _captionTable; }
			set { _captionTable = value; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Loads the current language pack from a given file.
		/// </summary>
		/// <param name="url">Url to load language pack from.</param>
		/// <param name="name">Name of language pack.</param>
		public static void LoadLanguagePack(object url, string name)
		{
			Stream stream = StreamFactory.RequestStream(url, StreamMode.Open);
			if (stream == null) return;

			_captionTable.Clear();

			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(stream);

			XmlNode rootNode = xmlDocument["captions"];
			foreach (XmlNode childNode in rootNode.ChildNodes)
				if (childNode.NodeType == XmlNodeType.Element)
					if (childNode.Name.ToLower() == "caption")
					{
						string captionName = "";
						foreach (XmlAttribute attribute in childNode.Attributes)
							if (attribute.Name.ToLower() == "name")
							{
								captionName = attribute.Value;
								DebugLogger.WriteLog("Found language caption with name '"+captionName+"'.");
								break;
							}
						if (captionName == "") continue;

                        // Trim the sides of the text.
                        string[] lines = childNode.InnerText.Split(new char[] { '\n' });
                        string finalText = "";
                        for (int i = 0; i < lines.Length; i++)
                            finalText += lines[i].Trim('\t') +"\n";
                        finalText = finalText.Trim(new char[] { '\n' });

                        if (_captionTable.ContainsKey(captionName) == true)
                        {
                            DebugLogger.WriteLog("Language caption uses a name that is already in use.", LogAlertLevel.Warning);
                            continue;
                        }

						_captionTable.Add(captionName, finalText);
					}

			stream.Close();
		}

		/// <summary>
		///		Gets a caption contained within the language pack.
		/// </summary>
		/// <param name="name">Name of caption to retrieve.</param>
		public static string GetCaption(string name)
		{
            if (_captionTable.Contains(name) == false) return "";
			return _captionTable[name] as string;
		}

		/// <summary>
		///		Returns true if a caption with the given name exists.
		/// </summary>
		/// <param name="name">Name of caption to look for.</param>
		public static bool CaptionExists(string name)
		{
			return _captionTable.Contains(name);
		}

		/// <summary>
		///		Sets a captions contained within the language pack.
		/// </summary>
		/// <param name="name">Name of caption to retrieve.</param>
		/// <param name="value">Value to set caption as.</param>
		public static void SetCaption(string name, string value)
		{
			if (_captionTable.Contains(name) == false)
			{
				_captionTable.Add(name, value);
				return;
			}
			_captionTable[name] = value;
		}

		#endregion
	}

}