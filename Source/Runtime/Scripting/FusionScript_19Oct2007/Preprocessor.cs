/* 
 * File: Preprocessor.cs
 *
 * This source file contains the declarations of the preprocessor class
 * which is used to go through a token list and act upon any preprocessor
 * directives (eg. #if, #define, ...)
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Collections;
using BinaryPhoenix.Fusion.Runtime.Debug;

namespace BinaryPhoenix.Fusion.Runtime.Scripting
{

	/// <summary>
	///		Used to store details on a single definition (eg. #define ME 0) used in the preprocessing step.
	/// </summary>
	public sealed class Define
	{
		#region Members
		#region Variables

		private string _ident, _value;
		private TokenID _valueID;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the identifier used to identify this deine.
		/// </summary>
		public string Ident
		{
			get { return _ident;  }
			set { _ident = value; }
		}

		/// <summary>
		///		Used to get the value stored in this define.
		/// </summary>
		public string Value
		{
			get { return _value;  }
			set { _value = value; }
		}

		/// <summary>
		///		Used to get the value ID stored in this define.
		/// </summary>
		public TokenID ValueID
		{
			get { return _valueID; }
			set { _valueID = value; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Initializes a new instance of this class.
		/// </summary>
		/// <param name="ident">Identifier used to identify this define.</param>
		/// <param name="value">Value this define should store.</param>
		/// <param name="valueID">Value ID thsi define should store.</param>
		public Define(string ident, string value, TokenID valueID)
		{
			_ident = ident;
			_value = value;
			_valueID = valueID;
		}

		#endregion
	}

	/// <summary>
	///		Used to go through a token list and act upon any preprocessor
	///		directives (eg. #if, #define, ...)
	/// </summary>
	public sealed class PreProcessor
	{
		#region Members
		#region Variables

		private Token _currentToken;
		private int _tokenIndex = 0;

		private ArrayList _errorList = new ArrayList();
		private ArrayList _tokenList = null, _newTokenList = new ArrayList();
		private ArrayList _defineList = new ArrayList();
        private ArrayList _includedFiles = new ArrayList();
		private CompileFlags _compileFlags;
		private object[] _includePaths;
		
		#endregion
		#region Properties

		/// <summary>
		///		Returns a list containing all the values defined in this script.
		/// </summary>
		public ArrayList DefineList
		{
			get { return _defineList; }
		}

		/// <summary>
		///		Returns a list containing an error that have occured while 
		///		processing the current token list.
		/// </summary>
		public ArrayList ErrorList
		{
			get { return _errorList; }
		}

		/// <summary>
		///		Returns a list of token obtained while analysing the current
		///		script.
		/// </summary>
		public ArrayList TokenList
		{
			get { return _newTokenList; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Processes the given token list and acts upon any pre processor directives (eg. #if, #define, ...).
		/// </summary>
		/// <param name="tokenList">Text of script to analyse.</param>
		/// <param name="flags">Bitmask of flags explaining how to process tokens.</param>
		/// <param name="defineList">A list of defines to use while preprocessing script.</param>
		/// <param name="includePaths">A list of directory paths to use when looking for include files.</param>
		/// <returns>Number of error that occured.</returns>
		public int Process(ArrayList tokenList, CompileFlags flags, Define[] defineList, object[] includePaths)
		{
			_tokenList = tokenList;
			_newTokenList.Clear();
			_errorList.Clear();
			_defineList.Clear();
            _includedFiles.Clear();
			_includePaths = includePaths;
			_currentToken = null;
			_tokenIndex = 0;
			_compileFlags = flags;

			// Create a new define for every definition passed.
			if (defineList != null)
			{
				foreach (Define def in defineList)
					_defineList.Add(def);
			}
			try
			{
				while (!EndOfTokenStream())
				{
					NextToken();
					ProcessToken();
				}
			}
			catch (CompileBreakException)
			{
				// Ignore this exception, it is used to jump out of the compiling loop.
			}

			return _errorList.Count;
		}

		/// <summary>
		///		Processes the next token in the token list.
		/// </summary>
		private void ProcessToken()
		{
			// Process the next token.
			#region Token Processing
            if (_currentToken == null)
            {
                NextToken();
                return;
            }
			switch (_currentToken.ID)
			{
				case TokenID.CharDirective:
					switch (NextToken().ID)
					{
						case TokenID.KeywordIf:
							ParseIf();
							break;

						case TokenID.TypeIdentifier:
							switch (_currentToken.Ident)
							{
								case "define":
									NextToken(); // Get the identifier.
									string ident = _currentToken.Ident;

									NextToken(); // Get the value.
									string value = _currentToken.Ident;

									_defineList.Add(new Define(ident, value, _currentToken.ID));

									break;
								case "undefine":
									NextToken(); // Get the identifier.

									foreach(Define def in _defineList)
										if (def.Ident.ToLower() == _currentToken.Ident.ToLower())
										{
											_defineList.Remove(def);
											break;
										}

									break;
								case "warning":
									Warning(ErrorCode.DirectiveError,NextToken().Ident);
									break;
								case "error":
									Error(ErrorCode.DirectiveError, NextToken().Ident);
									break;
								case "message":
									Message(ErrorCode.DirectiveError, NextToken().Ident);
									break;
								case "include":
									ExpectToken(TokenID.TypeString);
									string filePath = _currentToken.Ident;

									if (File.Exists(filePath) == false)
									{
                                        // See if its in one of the include paths.
                                        if (_includePaths != null)
                                        {
                                            foreach (object obj in _includePaths)
                                            {
                                                string path = obj as string;
                                                string newFilePath = path + "\\" + filePath;
                                                if (File.Exists(newFilePath) == true)
                                                {
                                                    filePath = newFilePath;
                                                    break;
                                                }
                                            }
                                        }

                                        // Nope? Check the tokens file name and see if its relative to that.
                                        if (_currentToken.File != "" && File.Exists(filePath) == false)
                                        {
                                            string includePath = Path.GetDirectoryName(_currentToken.File);
                                            string newFilePath = includePath + "\\" + filePath;
                                            if (File.Exists(newFilePath) == true)
                                                filePath = newFilePath;
                                        }
									}

                                    // Already included?
                                    if (_includedFiles.Contains(filePath))
                                    {
                                        Message(ErrorCode.DuplicateSymbol, "Ignoring include file \"" + _currentToken.Ident + "\", file has already been included.");
                                        break;
                                    }

									if (File.Exists(filePath) == true)
									{
										// Open a stream so we can read in the script.
										Stream stream = StreamFactory.RequestStream(filePath, StreamMode.Open);
										if (stream == null) return;
										StreamReader reader = new StreamReader(stream);

										// Load in script and setup variables.				
                                        string scriptText = reader.ReadToEnd();

										// Create a lexer and convert script into a list
										// of tokens.
										Lexer lexer = new Lexer();
                                        if (lexer.Analyse(scriptText, _compileFlags, filePath) > 0)
										{
											foreach (CompileError error in lexer.ErrorList)
												_errorList.Add(error);
										}

										// Insert the newly lexed tokens into this token list.
										_tokenList.InsertRange(_tokenIndex, lexer.TokenList);
                                        
                                        // Free up the stream.
										stream.Close();
										reader.Close();

                                        _includedFiles.Add(filePath);

                                        //DebugLogger.WriteLog("Included \"" + filePath + "\" into script.");
                                    }
									else
									{
										Warning(ErrorCode.NonExistantIncludeFile,"Unable to process include file \""+_currentToken.Ident+"\", file dosen't exist.");
									}

									break;
							}
							break;

						default:
							Error(ErrorCode.InvalidDirective, "Found invalid or unexpected directive while pre processing script.");
							break;
					}

					break;

				default:

					_newTokenList.Add(LookupDefineToken(_currentToken));

					break;
			}
			#endregion
		}

		/// <summary>
		///		Parses an #if directive.
		/// </summary>
		private void ParseIf()
		{
			bool ignoringTokens = !ProcessIf();
			bool exitLoop = false;
			while (EndOfTokenStream() == false && exitLoop == false)
			{
				NextToken();
				switch (_currentToken.ID)
				{
					case TokenID.CharDirective:
						switch (LookAheadToken().ID)
						{
							case TokenID.KeywordElse:
								NextToken();
								ignoringTokens = !ignoringTokens;
								break;
							case TokenID.TypeIdentifier:
								switch (LookAheadToken().Ident.ToLower())
								{
									case "elseif":
										NextToken(); 
										ignoringTokens = !ProcessIf();
										break;
									case "endif":
										NextToken(); 
										exitLoop = true;
										break;
									default:
										if (ignoringTokens == false) ProcessToken();
										break;
								}
								break;
							default:
								if (ignoringTokens == false) ProcessToken();
								break;
						}
						break;

					default:
						if (ignoringTokens == false) ProcessToken();
						break;
				}
			}
		}

		/// <summary>
		///		Looks up the give token in the define list and changes
		///		its ident if a define is found.
		/// </summary>
		/// <param name="token">Token to lookup.</param>
		/// <returns>The original token but changed to the given define.</returns>
		private Token LookupDefineToken(Token token)
		{
			foreach (Define def in _defineList)
				if (def.Ident.ToLower() == token.Ident.ToLower())
				{
					token.Ident = def.Value;
					token.ID    = def.ValueID;
					break;
				}
			return token;
		}

		/// <summary>
		///		Processes an if expression.
		/// </summary>
		/// <returns>Result of if expression</returns>
		private bool ProcessIf()
		{
			bool result = false;

			NextToken(); // Skip the If token.

			// Check for the not operator
			bool notExists = false;
			if (_currentToken.ID == TokenID.OpLogicalNot)
			{
				notExists = true;
				NextToken();
			}

			Define define = null;
			foreach (Define def in _defineList)
				if (def.Ident.ToLower() == _currentToken.Ident.ToLower())
				{
					define = def;
					break;
				}

			if (LookAheadToken().ID == TokenID.OpEqual ||
				LookAheadToken().ID == TokenID.OpNotEqual ||
				LookAheadToken().ID == TokenID.OpGreater ||
				LookAheadToken().ID == TokenID.OpGreaterEqual ||
				LookAheadToken().ID == TokenID.OpLess ||
				LookAheadToken().ID == TokenID.OpLessEqual)
			{
				NextToken(); // Get operator.
				TokenID opID = _currentToken.ID;
				NextToken(); // Get value.
				string value = _currentToken.Ident;

				if (define != null)
				{
					// Evaluate expression based on op.
					switch (opID)
					{
						case TokenID.OpEqual:
							if (define.Value == value) result = true;
							break;
						case TokenID.OpNotEqual:
							if (define.Value != value) result = true;
							break;
						case TokenID.OpGreater:
							if (int.Parse(define.Value) > int.Parse(value)) result = true;
							break;
						case TokenID.OpGreaterEqual:
							if (int.Parse(define.Value) >= int.Parse(value)) result = true;
							break;
						case TokenID.OpLess:
							if (int.Parse(define.Value) < int.Parse(value)) result = true;
							break;
						case TokenID.OpLessEqual:
							if (int.Parse(define.Value) <= int.Parse(value)) result = true;
							break;
					}
				}
			}
			else
			{
				if (define != null) result = true;
			}

			if (notExists == true) result = !result;
			return result;
		}

		/// <summary>
		///		Creates an error and adds it to the error list.
		/// </summary>
		/// <param name="errorCode">Code ID of error.</param>
		/// <param name="errorMsg">Description of error.</param>
		private void Error(ErrorCode errorCode, string errorMsg, bool fatal)
		{
			CompileError error = new CompileError(errorCode, errorMsg, fatal ? ErrorAlertLevel.FatalError : ErrorAlertLevel.Error, _currentToken.Line, _currentToken.Offset, _currentToken.File);
			_errorList.Add(error);
			if (fatal == true) throw new CompileBreakException();
		}
		private void Error(ErrorCode errorCode, string errorMsg)
		{
			Error(errorCode, errorMsg, false);
		}

		/// <summary>
		///		Creates an warning and adds it to the error list.
		/// </summary>
		/// <param name="errorCode">Code ID of warning.</param>
		/// <param name="errorMsg">Description of warning.</param>
		private void Warning(ErrorCode errorCode, string errorMsg)
		{
            CompileError error = new CompileError(errorCode, errorMsg, ErrorAlertLevel.Warning, _currentToken.Line, _currentToken.Offset, _currentToken.File);
			_errorList.Add(error);
			if ((_compileFlags & CompileFlags.TreatWarningsAsErrors) != 0) throw new CompileBreakException();
		}

		/// <summary>
		///		Creates an message and adds it to the error list.
		/// </summary>
		/// <param name="errorCode">Code ID of error.</param>
		/// <param name="errorMsg">Description of message.</param>
		private void Message(ErrorCode errorCode, string errorMsg)
		{
            CompileError error = new CompileError(errorCode, errorMsg, ErrorAlertLevel.Message, _currentToken.Line, _currentToken.Offset, _currentToken.File);
			_errorList.Add(error);
			if ((_compileFlags & CompileFlags.TreatMessagesAsErrors) != 0) throw new CompileBreakException();
		}

		/// <summary>
		///		Returns true if the lexer has read all token in the current script.
		/// </summary>
		/// <returns>True if the lexer has read all tokens in the current script.</returns>
		private bool EndOfTokenStream()
		{
			if (_tokenIndex >= _tokenList.Count) return true;
			return false;
		}

		/// <summary>
		///		Returns the next token in the script and advances the stream, this method
		///		will keep on lexing the stream if an invalid token is returned, the purpose of
		///		this is to gracefully handle errors.
		/// </summary>
		/// <returns>Next token in script.</returns>
		private Token NextToken()
		{
			if (EndOfTokenStream() == true) return null;
			_currentToken = (Token)_tokenList[_tokenIndex];
			_tokenIndex++;
			return _currentToken;
		}


		/// <summary>
		///		Returns the next token in the script but dosen't advance the stream.
		/// </summary>
		/// <param name="count">How many tokens to look ahead.</param>
		/// <returns>Token 'count' tokens ahead of current token.</returns>
		private Token LookAheadToken(int index)
		{
			int position = _tokenIndex;
			Token LAToken = null;
			for (int i = 0; i < index; i++)
				LAToken = NextToken();
			if (position != 0)
			{
				_tokenIndex = position - 1;
				NextToken();
			}
			else
				_tokenIndex = position;
			return LAToken;
		}
		private Token LookAheadToken()
		{
			return LookAheadToken(1);
		}

		/// <summary>
		///		Reads in the next token and advances the stream, it also checks
		///		if the next token's id is equal to the given id if not an error
		///		will be generated.
		/// </summary>
		/// <param name="id">ID that you wish to check against the current token's id.</param>
		/// <returns>Next token in script.</returns>
		private Token ExpectToken(TokenID id)
		{
			NextToken();
			CheckToken(id);
			return _currentToken;
		}

		/// <summary>
		///		Checks if the current token's id is equal to the given id,
		///		if not an error will be generated.
		/// </summary>
		/// <param name="id">ID that you wish to check against the current token's id.</param>
		private void CheckToken(TokenID id)
		{
			if (_currentToken.ID != id)
				Error(ErrorCode.ExpectingToken, "Expecting \"" + Token.IdentFromID(id) + "\" token.");
		}

		#endregion
	}

}