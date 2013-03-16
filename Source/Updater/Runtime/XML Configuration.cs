/* 
 * File: XML Configuration.cs
 *
 * This source file contains the declaration of the XMLConfigFile class which
 * is responsible for loading, saving and minipulating XML configuration files.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.IO;
using System.Xml;

namespace BinaryPhoenix.Fusion.Runtime
{

	/// <summary>
	///		This class is used to store configuration files in an 
	///		save/load-able XML format.
	/// </summary>
	public class XmlConfigFile
	{
		#region Members
		#region Variables

		XmlDocument _xmlDocument = new XmlDocument();

		#endregion
		#region Properties

		/// <summary>
		///		Gets the value of a setting in this configuration file.
		/// </summary>
		/// <param name="setting">Path to setting to get or set.</param>
		/// <param name="def">If the setting is not found this value will be returned.</param>
		/// <returns>Value of setting.</returns>
		public string this[string setting, string def]
		{
			get { return GetSetting(setting, def); }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Sets the value of a given configuration setting within this xml document.
		/// </summary>
		/// <param name="setting">Setting to set, namespaces can be split up with the : scoping character.</param>
		/// <param name="value">Value to set setting to.</param>
		public void SetSetting(string setting, string value)
		{
			XmlNode node = FindNode(setting);
			if (node == null)
			{
				CreateNode(setting, value);
			}
			else
				foreach (XmlNode attribute in node.Attributes)
					if (attribute.Name.ToLower() == "value")
					{
						attribute.Value = value;
					}
		}

		/// <summary>
		///		Sets the value of a given setting in this xml document.
		/// </summary>
		/// <param name="setting">Setting to get, namespaces can be split up with the : scoping character.</param>
		/// <param name="def">If no setting is found this value will be returned.</param>
		public string GetSetting(string setting, string def)
		{
			XmlNode node = FindNode(setting);
			if (node == null)
			{
				return def;
			}
			else
				foreach (XmlNode attribute in node.Attributes)
					if (attribute.Name.ToLower() == "value")
					{
						return attribute.Value;
					}

			return def;
		}
		public string GetSetting(string setting)
		{
			return GetSetting(setting, "");
		}

		/// <summary>
		///		Gets all the setting residing in the given group. 
		/// </summary>
		/// <param name="group">Group that settings should reside in.</param>
		/// <returns>Array of settings contained in the given group.</returns>
		public string[] GetSettings(string group)
		{
			XmlNode node = _xmlDocument["settings"];
			string[] groups = group.Split(new char[] { ':' });
			for (int i = 0; i < groups.Length; i++)
				node = node[groups[i]];
			if (node == null) return null;
			
			ArrayList settings = new ArrayList();
			foreach (XmlNode child in node.ChildNodes)
			{
				if (child.NodeType != XmlNodeType.Element) continue;
				foreach (XmlNode attribute in child.Attributes)
					if (attribute.Name.ToLower() == "name")
						settings.Add(group + ":" + attribute.Value);
			}

			if (settings.Count == 0) return null;
			return (string[])settings.ToArray(typeof(string));
		}

		/// <summary>
		///		Finds a node in the XML document with the given name. 
		///		Names can be deliminated with the : scoping character. 
		/// </summary>
		/// <param name="name">Name of node to find.</param>
		/// <returns>An XmlNode if one is found with the correct name.</returns>
		private XmlNode FindNode(string name)
		{
			string[] splitNames = name.Split(new char[1] { ':' });
			XmlNode node = _xmlDocument["settings"];

			// Go through any namespace's.
			for (int i = 0; i < splitNames.Length; i++)
			{
				// Find the actuall node and value by its name attribute.
				if (i == splitNames.Length - 1)
				{
					foreach (XmlNode subNode in node.ChildNodes)
					{
						if (subNode.Name.ToLower() == "setting")
						{	
							// Find the name/value attributes of this setting.
							foreach (XmlNode attribute in subNode.Attributes)
							{
								if (attribute.Name.ToLower() == "name" && attribute.Value.ToLower() == splitNames[splitNames.Length - 1].ToLower())
									return subNode;
							}
						}
					}
					break;
				}

				// Find the namespace element this node is in.
				else
				{
					foreach (XmlNode subNode in node.ChildNodes)
						if (subNode.NodeType == XmlNodeType.Element && subNode.Name.ToLower() == splitNames[i].ToLower())
							node = subNode;
				}
			}

			return null;
		}

		/// <summary>
		///		Creates a node in the XML document with the given name and value.
		///		Names can be deliminated with the : scoping character. 
		/// </summary>
		/// <param name="name">Name of the node to create.</param>
		/// <param name="value">Value to store in node.</param>
		/// <returns>The newly created XmlNode instance.</returns>
		private XmlNode CreateNode(string name, string value)
		{
			if (FindNode(name) != null) return null;

			bool foundNode = false;
			string[] splitNames = name.Split(new char[1] { ':' });
			XmlNode node = _xmlDocument["settings"];

			// Go through any namespace's.
			for (int i = 0; i < splitNames.Length; i++)
			{
				// Find the actuall node and value by its name attribute.
				if (i == splitNames.Length - 1)
				{
					foundNode = false;
					foreach (XmlNode subNode in node.ChildNodes)
					{
						if (subNode.NodeType == XmlNodeType.Element && subNode.Name.ToLower() == "setting")
						{
							// Find the name/value attributes of this setting.
							foreach (XmlNode attribute in subNode.Attributes)
							{
								if (attribute.Name.ToLower() == "name" && attribute.Value.ToLower() == splitNames[splitNames.Length - 1].ToLower())
									foundNode = true;
							}
						}
					}
					if (foundNode == false)
					{
						XmlNode newNode = _xmlDocument.CreateElement("setting");
						node.AppendChild(newNode);

						XmlAttribute nameAttribute = _xmlDocument.CreateAttribute("name");
						nameAttribute.Value = splitNames[splitNames.Length - 1];
						newNode.Attributes.Append(nameAttribute);

						XmlAttribute valueAttribute = _xmlDocument.CreateAttribute("value");
						valueAttribute.Value = value;
						newNode.Attributes.Append(valueAttribute);
					}
					break;
				}

				// Find the namespace element this node is in.
				else
				{
					foundNode = false;
					foreach (XmlNode subNode in node.ChildNodes)
						if (subNode.NodeType == XmlNodeType.Element && subNode.Name.ToLower() == splitNames[i].ToLower())
						{
							node = subNode;
							foundNode = true;
						}
					if (foundNode == false)
					{
						XmlNode newNode = _xmlDocument.CreateElement(splitNames[i]);
						node.AppendChild(newNode);
						node = newNode;
					}
				}
			}

			return null;
		}

		/// <summary>
		///		Saves this XML configuration file into a file.
		/// </summary>
		/// <param name="url">Url of file to write data into.</param>
		public void Save(object url)
		{
            if (url is BinaryWriter)
                _xmlDocument.Save((url as BinaryWriter).BaseStream);
            else
            {
                if (File.Exists(url as string) == false) File.Create(url as string);
                Stream stream = new FileStream(url as string, FileMode.Truncate, FileAccess.Write);
                if (stream == null) return;
                _xmlDocument.Save(stream);
                stream.Close();
            }
        }

		/// <summary>
		///		Loads this XML configuration file's data from a file .
		/// </summary>
		/// <param name="url">Url of file to read data from.</param>
		public void Load(object url)
		{
            if (url is BinaryReader)
                _xmlDocument.Load((url as BinaryReader).BaseStream);
            else
            {
                Stream stream = new FileStream(url as string, FileMode.Open, FileAccess.Read);
			    if (stream == null) return;
			    _xmlDocument.Load(stream);
			    stream.Close();
            }
		}

		/// <summary>
		///		Initializes a new instance of this class from a given file.
		/// </summary>
		/// <param name="url">Path of Xml file to load configuration from.</param>
		public XmlConfigFile(object url)
		{
			Load(url);
		}

		#endregion
	}

}