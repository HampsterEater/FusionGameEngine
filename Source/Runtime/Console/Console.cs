/*
 * File: Console.cs
 *
 * Contains the Console class which is used to parse console commands
 * and preform the appropriate actions they specify.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Reflection;
using System.Collections;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Debug;

namespace BinaryPhoenix.Fusion.Runtime.Console
{
	/// <summary>
	///		Used to point to a method that can be called by a
	///		console command.
	/// </summary>
	/// <param name="arguments">Arguments this command was called with.</param>
	public delegate void CommandDelegate(object[] arguments);

    /// <summary>
    ///     Used the define the values a console command accepts.
    /// </summary>
    public enum ConsoleValueType
    {
        Bool,
        Int,
        Float,
        String
    }

	/// <summary>
	///		Used to store details on a single command capable of being invoked
	///		by the console class.
	/// </summary>
	public sealed class ConsoleCommand
	{
		#region Members
		#region Variables

		private string _identifier = "";
        private ConsoleValueType[] _parameters;

		private CommandDelegate _delegate;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the string used to identify this console command.
		/// </summary>
		public string Identifier
		{
			get { return _identifier;  }
			set { _identifier = value; }
		}

		/// <summary>
		///		Gets or sets the delegate that is invoked when this console command is called.
		/// </summary>
		public CommandDelegate Delegate
		{
			get { return _delegate;  }
			set { _delegate = value; }
		}

        /// <summary>
        ///		Gets or sets the value types of this commands parameters.
        /// </summary>
        public ConsoleValueType[] ParameterTypes
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Invokes this console command with the given arguments.
		/// </summary>
		/// <param name="arguments">Arguments to invoke this command with.</param>
		public void Invoke(object[] arguments)
		{
			_delegate(arguments);
		}

		/// <summary>
		///		Initializes a new instance of this class with the 
		///		given identifier and delegate.
		/// </summary>
		/// <param name="identifier">String to identify this console command.</param>
		/// <param name="del">Delegate to method to invoke when this command is called.</param>
        /// <param name="parameters">Array of parameter types describing this commands parameters.</param>
		public ConsoleCommand(string identifier, CommandDelegate del, ConsoleValueType[] parameters)
		{
			_identifier = identifier;
			_delegate = del;
            _parameters = parameters;
		}

		#endregion
	}

    /// <summary>
    ///		Used inside ConsoleCommandSet classes to describe how a consoel command
    ///		should be called.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class ConsoleCommandInfo : Attribute
    {
        #region Members
        #region Variables

		private string _identifier = "";
        private ConsoleValueType[] _parameters;

        #endregion
        #region Properties

        /// <summary>
        ///		Gets or sets the identifier used to call this function.
        /// </summary>
        public string Identifier
        {
            get { return _identifier; }
            set { _identifier = value; }
        }

        /// <summary>
        ///		Gets or sets the parameters that this command accepts.
        /// </summary>
        public ConsoleValueType[] ParameterTypes
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///		Creates a new instance of this class with the given data.
        /// </summary>
        /// <param name="name">Identifier used to call this function</param>
        /// <param name="parameterTypes">Parameters that this function accepts</param>
        public ConsoleCommandInfo(string name, string parameterTypes)
        {
            _identifier = name;

            if (parameterTypes != "")
            {
                string[] parameterTypesSplit = parameterTypes.Split(new Char[] { ',' });
                _parameters = new ConsoleValueType[parameterTypesSplit.Length];
                for (int i = 0; i < parameterTypesSplit.Length; i++)
                {
                    int index = 0;
                    foreach (string valueName in Enum.GetNames(typeof(ConsoleValueType)))
                    {
                        if (valueName.ToLower() == parameterTypesSplit[i])
                        {
                            _parameters[i] = (ConsoleValueType)index;
                            break;
                        }
                        index++;
                    }
                }
            }
            else
                _parameters = new ConsoleValueType[0];
        }

        #endregion
    }

    /// <summary>
    ///     Used to quickly register a large set of console commands.
    /// </summary>
    public abstract class ConsoleCommandSet
    {
		#region Members
		#region Variables

		private static ArrayList _globalCommandSets = new ArrayList();

		private ArrayList _consoleCommands = new ArrayList();
		private bool _prepared = false;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the global pool of command sets being used.
		/// </summary>
		public static ArrayList GlobalCommandSets
		{
			get { return _globalCommandSets; }
			set { _globalCommandSets = value; }
		}

		/// <summary>
		///		Gets or sets the list of console commands this set contains.
		/// </summary>
		public ArrayList ConsoleCommands
		{
			get { return _consoleCommands; }
			set { _consoleCommands = value; }
		}

		#endregion
		#endregion
		#region Register Functions

		/// <summary>
		///		Creates a new instance of this class and calls the PrepareSet method.
		/// </summary>
		public ConsoleCommandSet()
		{
			PrepareSet();
		}

		/// <summary>
		///		Parses through this class to find any methods that can be used as console commands.
		/// </summary>
		public void PrepareSet()
		{
			if (_prepared == true) return;
			_prepared = true;
			
			_globalCommandSets.Add(this);

			Type type = this.GetType();
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
		
			foreach (MethodInfo method in methods)
			{
				// Go through attributes until we find a NativeFunctionInfo attribute
				// if we don't find one then ignore this function.
				ConsoleCommandInfo infoAttribute = method.GetCustomAttributes(typeof(ConsoleCommandInfo), true)[0] as ConsoleCommandInfo;
				if (infoAttribute == null) continue;

				// Create a native function delegate for this method.
				ConsoleCommand command = new ConsoleCommand(infoAttribute.Identifier, (CommandDelegate)CommandDelegate.CreateDelegate(typeof(CommandDelegate), this, method), infoAttribute.ParameterTypes);
				_consoleCommands.Add(command);
            }

            #endregion
        }

        /// <summary>
		///		Registers all the console commands containing in this set with the console.
		/// </summary>
		public void RegisterCommands()
		{
            foreach (ConsoleCommand command in _consoleCommands)
                Console.RegisterCommand(command);
        }

        /// <summary>
		///		Registers all the console commands containing in all sets with the console.
		/// </summary>
		public static void RegisterCommandSets()
		{
			foreach (ConsoleCommandSet set in _globalCommandSets)
				set.RegisterCommands();
		}
    }

	/// <summary>
	///		This class is used to process console commands and preforms the appropriate 
	///		actions they specify.
	/// </summary>
	public static class Console
	{
		#region Members
		#region Variables

		private static ArrayList _commands = new ArrayList();

		#endregion
		#region Properties

		/// <summary>
		///		Gets the list containing all the console commands registered to this console.
		/// </summary>
		public static ArrayList Commands
		{
			get { return _commands; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Registers a console command to the console.
		/// </summary>
		/// <param name="command">Command to register.</param>
		public static void RegisterCommand(ConsoleCommand command)
		{
			_commands.Add(command);
		}

		/// <summary>
		///		Unregisters a console command to the console.
		/// </summary>
		/// <param name="command">Command to unregister.</param>
		public static void DeregisterCommand(ConsoleCommand command)
		{
			_commands.Remove(command);
		}

		/// <summary>
		///		Processes the given command and preforms the actions it specifys.
		/// </summary>
		/// <param name="command">Command to process.</param>
		public static void ProcessCommand(string command)
		{
            // Clean the command line up a bit.
            command = command.Trim();

			// Find the first space index and use that to split
			// the command from its arguments.
			object[] arguments = new object[0];
			int spaceIndex = command.IndexOf(' ');
			if (spaceIndex != -1)
			{
                string argumentsRaw = command.Substring(spaceIndex + 1);
				command = command.Substring(0, spaceIndex).Trim();

                ArrayList parameterList = new ArrayList();
                string parameter = "";
                for (int i = 0; i < argumentsRaw.Length; i++)
                {
                    switch (argumentsRaw[i])
                    {
                        case ' ': // New parameter.
                            {
                                parameterList.Add(parameter);
                                parameter = "";
                                break;
                            }
                        case '"': 
                        case '\'':
                            {
                                int j = i + 1;
                                string value = "";
                                while (true)
                                {
                                    if (j >= argumentsRaw.Length)
                                    {
                                        DebugLogger.WriteLog("Syntax Error, unfinished string.", LogAlertLevel.Error);
                                        return;
                                    }

                                    if (argumentsRaw[j] == argumentsRaw[i])
                                        break;
                                    else
                                    {
                                        value += argumentsRaw[j];
                                        j++;
                                    }
                                }
                                i = j;
                                parameterList.Add(value);
                                break;
                            }
                        default: // General detritus.
                            {
                                parameter += argumentsRaw[i];
                                break;
                            }
                    }
                }

                if (parameter != "")
                    parameterList.Add(parameter);
                
                arguments = parameterList.ToArray();

                // Replace keywords with appropriate value.
                for (int i = 0; i < arguments.Length; i++)
                {
                    switch ((arguments[i] as string).ToLower())
                    {
                        case "true":
                            arguments[i] = true;
                            break;
                        case "false:":
                            arguments[i] = false;
                            break;
                    }
                }
            }

			// Look through the list of registered console commands
			// and call them if neccessary.
			bool foundCommand = false;
			foreach(ConsoleCommand concmd in _commands)
				if (command.ToLower() == concmd.Identifier.ToLower())
				{
                    // Check arguments are correct.
                    bool argumentsCorrect = (arguments.Length == concmd.ParameterTypes.Length);
                    if (argumentsCorrect == true)
                    {
                        for (int i = 0; i < arguments.Length; i++)
                        {
                            switch (concmd.ParameterTypes[i])
                            {
                                case ConsoleValueType.Bool:
                                    if (arguments[i] is string)
                                    {
                                        string str = arguments[i] as string;
                                        if (str == "1")
                                            arguments[i] = true;
                                        else if (str == "0")
                                            arguments[i] = false;
                                        else
                                            argumentsCorrect = false;
                                    }
                                    else if (!(arguments[i] is bool))
                                        argumentsCorrect = false;
                                    break;
                                case ConsoleValueType.Float:
                                    {
                                        float result = 0.0f;
                                        if (float.TryParse(arguments[i] as string, out result))
                                            arguments[i] = result;
                                        else
                                            argumentsCorrect = false;
                                        break;
                                    }
                                case ConsoleValueType.Int:
                                    {
                                        int result = 1;
                                        if (int.TryParse(arguments[i] as string, out result))
                                            arguments[i] = result;
                                        else
                                            argumentsCorrect = false;
                                        break;
                                    }
                                case ConsoleValueType.String:
                                    argumentsCorrect = (arguments[i] is string);
                                    break;
                            }

                            if (argumentsCorrect == false)
                                break;
                        }
                    }
                    
                    // Nope? Error! Does not compute!
                    if (!argumentsCorrect)
                    {
                        string argumentSyntax = "";
                        for (int i = 0; i < concmd.ParameterTypes.Length; i++)
                        {
                            argumentSyntax += concmd.ParameterTypes[i].ToString().ToLower();
                            if (i < concmd.ParameterTypes.Length - 1)
                                argumentSyntax += " ";
                        }
                        DebugLogger.WriteLog("Syntax Error, invalid parameters. Correct syntax is: " + concmd.Identifier + " " + argumentSyntax, LogAlertLevel.Error);
                        return;
                    }
                    
                    // Invoke
                    concmd.Invoke(arguments);
					foundCommand = true;
					break;
				}

			// Write out a lot explaining if this command was executed successfully.
			if (foundCommand == false)
				DebugLogger.WriteLog("Unable to find command \""+command+"\".", LogAlertLevel.Warning);
		}

		#endregion
	}

}
