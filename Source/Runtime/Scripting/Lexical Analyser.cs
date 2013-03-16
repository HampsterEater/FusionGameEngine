/* 
 * File: Lexical analyser.cs
 *
 * This source file contains the declarations of the lexer class
 * which is used to convert a script to a list of tokens.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Collections;

namespace BinaryPhoenix.Fusion.Runtime.Scripting
{

	/// <summary>
	///		Used to describe the data that a token instance
	///		stores.
	/// </summary>
	public enum TokenID
	{
		KeywordState,
		KeywordEvent,
		KeywordDefault,
		KeywordSwitch,
		KeywordCase,
		KeywordIf,
		KeywordElse,
		KeywordEnum,
		KeywordConst,
		KeywordStatic,
		KeywordRef,
		KeywordFor,
		KeywordReturn,
		KeywordWhile,
		KeywordDo,
		KeywordNew,
		KeywordDelete,
		KeywordLock,
		KeywordAtom,
		KeywordObject,
		KeywordString,
		KeywordBool,
		KeywordByte,
		KeywordInt,
		KeywordShort,
		KeywordFloat,
		KeywordDouble,
		KeywordLong,
		KeywordVoid,
		KeywordThread,
		KeywordGoto,
		KeywordBreak,
		KeywordContinue,
		KeywordIndexer,
		KeywordGotoState,
		KeywordBreakpoint,
		KeywordProperty,
		KeywordPrivate,
		KeywordPublic,
		KeywordProtected,
		KeywordStruct,
		KeywordClass,
		KeywordEngine,
		KeywordEditor,
        KeywordNamespace,
        KeywordConsole,
        KeywordImport,
        KeywordExport,

		TypeIdentifier,
		TypeString,
		TypeByte,
		TypeShort,
		TypeFloat,
		TypeDouble,
		TypeLong,
		TypeInteger,
		TypeBoolean,
		TypeVoid,
		TypeNull,
		TypeObject,

		OpModulus,

		OpLogicalAnd,
		OpLogicalOr,
		OpLogicalNot,

		OpBitwiseSHL,
		OpBitwiseSHR,
		OpBitwiseAnd,
		OpBitwiseOr,
		OpBitwiseXOr,
		OpBitwiseNot,

		OpMultiply,
		OpDivide,
		OpAdd,
		OpSub,

		OpLess,
		OpGreater,
		OpLessEqual,
		OpGreaterEqual,
		OpNotEqual,
		OpEqual,

		OpAssign,
		OpAssignAdd,
		OpAssignSub,
		OpAssignMultiply,
		OpAssignDivide,
		OpAssignModulus,
		OpAssignBitwiseAnd,
		OpAssignBitwiseOr,
		OpAssignBitwiseXOr,
		OpAssignBitwiseNot,
		OpAssignBitwiseSHL,
		OpAssignBitwiseSHR,
		OpIncrement,
		OpDecrement,

        OpMemberResolver,

		CharOpenBrace,
		CharCloseBrace,
		CharOpenBracket,
		CharCloseBracket,
		CharOpenParenthesis,
		CharCloseParenthesis,
		CharComma,
		CharColon,
		CharSemiColon,
		CharPeriod,
		CharDirective
	}

	/// <summary>
	///		Used to describe a token extracted from a script.
	/// </summary>
	public sealed class Token
	{
		#region Members
		#region Variables

		private TokenID _id;
		private string _ident;
		private int _line, _offset;
		private string _file;

		#endregion
		#region Properties

		/// <summary>
		///		Sets or gets the token id used to describe this token.
		/// </summary>
		public TokenID ID
		{
			get { return _id; }
			set { _id = value; }
		}

		/// <summary>
		///		Sets or gets the identifier used to store extra data about this token.
		/// </summary>
		public String Ident
		{
			get { return _ident; }
			set { _ident = value; }
		}

		/// <summary>
		///		Sets or gets the file this token was extracted from.
		/// </summary>
		public String File
		{
			get { return _file; }
			set { _file = value; }
		}

		/// <summary>
		///		Sets or gets the line this token was extracted from.
		/// </summary>
		public int Line
		{
			get { return _line; }
			set { _line = value; }
		}

		/// <summary>
		///		Sets or gets the offset on line this token was extracted from.
		/// </summary>
		public int Offset
		{
			get { return _offset; }
			set { _offset = value; }
		}

		/// <summary>
		///		Returns true if this token is a logical operator.
		/// </summary>
		public bool IsLogical
		{
			get 
			{ 
				// Don't check for the logical not as that is technically a bitwise unary operator.
				if (_id == TokenID.OpLogicalAnd || _id == TokenID.OpLogicalOr)
					return true;
				return false;
			}
		}

		/// <summary>
		///		Returns true if this token is a relational operator.
		/// </summary>
		public bool IsRelational
		{
			get 
			{ 
				if (_id == TokenID.OpEqual		  || _id == TokenID.OpGreater ||
					_id == TokenID.OpGreaterEqual || _id == TokenID.OpLess	  ||
					_id == TokenID.OpLessEqual	  || _id == TokenID.OpNotEqual)
					return true;
				return false;
			}
		}

        /// <summary>
        ///		Returns true if this token is a data type definer.
        /// </summary>
        public bool IsDataType
        {
            get
            {
 				if (_id == TokenID.KeywordObject  || _id == TokenID.KeywordString ||
					_id == TokenID.KeywordBool    || _id == TokenID.KeywordByte	  ||
					_id == TokenID.KeywordInt	  || _id == TokenID.KeywordShort ||
                    _id == TokenID.KeywordFloat	  || _id == TokenID.KeywordDouble || 
                    _id == TokenID.KeywordLong	  || _id == TokenID.KeywordVoid)
					return true;
				return false;
            }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Returns a string describing this token.
		/// </summary>
		/// <returns>A string describing this token.</returns>
		public override string ToString()
		{
			return IdentFromID(_id);
		}

		/// <summary>
		///		Initializes a new instance of this class.
		/// </summary>
		/// <param name="id">ID used to describe this token.</param>
		/// <param name="ident">Identifier used to store extra data about this token.</param>
		/// <param name="line">Line index this token was extracted from.</param>
		/// <param name="offset">Offset on line this token was extracted from.</param>		
		/// <param name="file">File this token was extracted from.</param>
		public Token(TokenID id, string ident, int line, int offset, string file)
		{
			_id = id;
			_ident = ident;
			_line = line;
			_offset = offset;
			_file = file;
		}

		/// <summary>
		///		Returns an identifier for a token from an id.
		/// </summary>
		/// <param name="id">ID to work out identifier from.</param>
		public static string IdentFromID(TokenID id)
		{
			switch (id)
			{
				case TokenID.KeywordState:
					return "state";
				case TokenID.KeywordEvent:
					return "event";
				case TokenID.KeywordDefault:
					return "default";
				case TokenID.KeywordSwitch:
					return "switch";
				case TokenID.KeywordCase:
					return "case";
				case TokenID.KeywordIf:
					return "if";
				case TokenID.KeywordElse:
					return "else";
				case TokenID.KeywordEnum:
					return "enum";
				case TokenID.KeywordConst:
					return "const";
				case TokenID.KeywordStatic:
					return "static";
				case TokenID.KeywordRef:
					return "ref";
				case TokenID.KeywordReturn:
					return "return";
				case TokenID.KeywordWhile:
					return "while";
				case TokenID.KeywordDo:
					return "do";
				case TokenID.KeywordNew:
					return "new";
				case TokenID.KeywordDelete:
					return "delete";
				case TokenID.KeywordLock:
					return "lock";
				case TokenID.KeywordObject:
					return "Object";
				case TokenID.KeywordString:
					return "string";
				case TokenID.KeywordBool:
					return "bool";
				case TokenID.KeywordByte:
					return "byte";
				case TokenID.KeywordInt:
					return "int";
				case TokenID.KeywordShort:
					return "short";
				case TokenID.KeywordFloat:
					return "float";
				case TokenID.KeywordLong:
					return "long";
				case TokenID.KeywordVoid:
					return "void";
				case TokenID.KeywordThread:
					return "thread";
				case TokenID.KeywordGoto:
					return "goto";
				case TokenID.KeywordBreak:
					return "break";
				case TokenID.KeywordContinue:
					return "continue";
				case TokenID.KeywordIndexer:
					return "indexer";
				case TokenID.KeywordGotoState:
					return "gotostate";
				case TokenID.KeywordBreakpoint:
					return "breakpoint";
				case TokenID.KeywordProperty:
					return "property";
				case TokenID.KeywordClass:
					return "class";
				case TokenID.KeywordStruct:
					return "struct";
				case TokenID.KeywordPublic:
					return "public";
				case TokenID.KeywordPrivate:
					return "private";
				case TokenID.KeywordProtected:
					return "protected";
				case TokenID.KeywordEngine:
					return "engine";
				case TokenID.KeywordEditor:
					return "editor";
                case TokenID.KeywordConsole:
                    return "console";

				case TokenID.TypeIdentifier:
					return "identifier";
				case TokenID.TypeString:
					return "string literal";
				case TokenID.TypeFloat:
					return "floating-point literal";
				case TokenID.TypeDouble:
					return "double precision floating-point literal";
				case TokenID.TypeInteger:
					return "integer literal";
				case TokenID.TypeBoolean:
					return "boolean literal";
				case TokenID.TypeVoid:
					return "void";

				case TokenID.OpModulus:
					return "%";

				case TokenID.OpLogicalAnd:
					return "&&";
				case TokenID.OpLogicalOr:
					return "||";
				case TokenID.OpLogicalNot:
					return "!";

				case TokenID.OpBitwiseSHL:
					return "<<";
				case TokenID.OpBitwiseSHR:
					return ">>";
				case TokenID.OpBitwiseAnd:
					return "&";
				case TokenID.OpBitwiseOr:
					return "|";
				case TokenID.OpBitwiseXOr:
					return "^";
				case TokenID.OpBitwiseNot:
					return "~";

				case TokenID.OpMultiply:
					return "*";
				case TokenID.OpDivide:
					return "/";
				case TokenID.OpAdd:
					return "+";
				case TokenID.OpSub:
					return "-";

				case TokenID.OpLess:
					return "<";
				case TokenID.OpGreater:
					return ">";
				case TokenID.OpLessEqual:
					return "<=";
				case TokenID.OpGreaterEqual:
					return ">=";
				case TokenID.OpNotEqual:
					return "!=";
				case TokenID.OpEqual:
					return "==";

				case TokenID.OpAssign:
					return "=";
				case TokenID.OpAssignAdd:
					return "+=";
				case TokenID.OpAssignSub:
					return "-=";
				case TokenID.OpAssignMultiply:
					return "*=";
				case TokenID.OpAssignDivide:
					return "/=";
				case TokenID.OpAssignModulus:
					return "%=";
				case TokenID.OpAssignBitwiseAnd:
					return "&=";
				case TokenID.OpAssignBitwiseOr:
					return "|=";
				case TokenID.OpAssignBitwiseNot:
					return "~=";
				case TokenID.OpAssignBitwiseXOr:
					return "^=";
				case TokenID.OpAssignBitwiseSHL:
					return "=<<";
				case TokenID.OpAssignBitwiseSHR:
					return "=>>";

                case TokenID.OpMemberResolver:
                    return "->";

				case TokenID.CharOpenBrace:
					return "{";
				case TokenID.CharCloseBrace:
					return "}";
				case TokenID.CharOpenBracket:
					return "[";
				case TokenID.CharCloseBracket:
					return "]";
				case TokenID.CharOpenParenthesis:
					return "(";
				case TokenID.CharCloseParenthesis:
					return ")";
				case TokenID.CharComma:
					return ",";
				case TokenID.CharColon:
					return ":";
				case TokenID.CharSemiColon:
					return ";";
				case TokenID.CharPeriod:
					return ".";
				case TokenID.CharDirective:
					return "#";

				default:
					return "*"+id.ToString();
			}
		}

		#endregion
	}

	/// <summary>
	///		Used to lexically analyse a script and convert it to	
	///		an array of tokens.
	/// </summary>
	public sealed class Lexer
	{
		#region Members
		#region Variables

		private ArrayList _errorList = new ArrayList(), _tokenList = new ArrayList();

		private CompileFlags _compileFlags;

		private int _lexerPosition, _lexerLine = 1, _lexerOffset = 0;
		private Token _currentToken;
		private string _currentChar;

		private string _scriptText;

		private string _file;

		#endregion
		#region Properties

		/// <summary>
		///		Returns a list containing an error that have occured while 
		///		lexing the current script.
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
			get { return _tokenList; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Analayse's the given script text and converts it to a list of tokens.
		/// </summary>
		/// <param name="scriptText">Text of script to analyse.</param>
		/// <param name="flags">Bitmask of flags explaining how to analyse script.</param>
		/// <param name="file">File this script was extracted from.</param>
		/// <returns>Number of error that occured.</returns>
		public int Analyse(string scriptText,CompileFlags flags,string file)
		{
			_tokenList.Clear();
			_errorList.Clear();
			_scriptText = scriptText.Trim();
			_compileFlags = flags;
			_file = file;

			try 
			{
				NextCharacter();
				while (!EndOfTokenStream())
				{
					try
					{
						_tokenList.Add(NextToken());
					}
					catch (CompilePanicModeException)
					{
						// Don't do anything here, just allow the error to be
						// forgoten and carry on as normal.
					}
				}
			}
			catch (CompileBreakException)
			{
				// Ignore this exception, it is used to jump out of the compiling loop.
			}

			return _errorList.Count;
		}

		/// <summary>
		///		Creates an error and adds it to the error list.
		/// </summary>
		/// <param name="errorCode">Code ID of error.</param>
		/// <param name="errorMsg">Description of error.</param>
		private void Error(ErrorCode errorCode, string errorMsg, bool fatal)
		{
			CompileError error = new CompileError(errorCode, errorMsg, fatal ? ErrorAlertLevel.FatalError : ErrorAlertLevel.Error, _lexerLine, _lexerOffset, _file);
			_errorList.Add(error);
			if (fatal == true)
				throw new CompileBreakException();
			else
				ErrorPanicMode();
		}
		private void Error(ErrorCode errorCode, string errorMsg)
		{
			Error(errorCode, errorMsg, false);
		}

		/// <summary>
		///		Preforms a rather crude implementation off a panic
		///		mode error recovery algorithem.
		/// </summary>
		private void ErrorPanicMode()
		{
			while (NextCharacter() != " " && NextCharacter() != "\n" && EndOfTokenStream() == false)
			{
				NextCharacter();
			}
			if (_lexerPosition > 0) _lexerPosition--;
			throw new CompilePanicModeException();
		}

		/// <summary>
		///		Creates an warning and adds it to the error list.
		/// </summary>
		/// <param name="errorCode">Code ID of warning.</param>
		/// <param name="errorMsg">Description of warning.</param>
		private void Warning(ErrorCode errorCode, string errorMsg)
		{
			CompileError error = new CompileError(errorCode, errorMsg, ErrorAlertLevel.Warning, _lexerLine, _lexerOffset, _file);
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
			CompileError error = new CompileError(errorCode, errorMsg, ErrorAlertLevel.Message, _lexerLine, _lexerOffset, _file);
			_errorList.Add(error);
			if ((_compileFlags & CompileFlags.TreatMessagesAsErrors) != 0) throw new CompileBreakException();
		}

		/// <summary>
		///		Returns the next character in the script
		///		and advances the lexers position.
		/// </summary>
		/// <returns>The next character in the script.</returns>
		private string NextCharacter()
		{
			if (_lexerPosition < _scriptText.Length)
				_currentChar = _scriptText.Substring(_lexerPosition, 1);
			else
				_currentChar = "";

			_lexerPosition++;
			_lexerOffset++;
			if (_currentChar == "\n")
			{
				_lexerLine++;
				_lexerOffset = 1;
			}
			return _currentChar;
		}

		/// <summary>
		///		Returns true if the lexer has read all token in the current script.
		/// </summary>
		/// <returns>True if the lexer has read all tokens in the current script.</returns>
		private bool EndOfTokenStream()
		{
			return (_lexerPosition > _scriptText.Length);
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
			Token tok = LexToken();

			// This loop is used to ignore error tokens (ie. no token XD),
			// and keep lexing until it gets a usable one.
			while (tok == null && EndOfTokenStream() == false)
				tok = LexToken();

			return tok;
		}

		/// <summary>
		///		Lexically analyses and returns the next token in the script.
		/// </summary>
		/// <returns>Next token in script.</returns>
		private Token LexToken()
		{
			_currentToken = null;
			switch (_currentChar)
			{
				case "=":
					switch (NextCharacter())
					{
						case "=": NextCharacter(); _currentToken = new Token(TokenID.OpEqual, "==", _lexerLine, _lexerOffset, _file); break;
						default: _currentToken = new Token(TokenID.OpAssign, "=", _lexerLine, _lexerOffset, _file); break;
					}
					break;

				case "^":
					switch (NextCharacter())
					{
						case "=": NextCharacter(); _currentToken = new Token(TokenID.OpAssignBitwiseXOr, "^=", _lexerLine, _lexerOffset, _file); break;
						default: _currentToken = new Token(TokenID.OpBitwiseXOr, "^", _lexerLine, _lexerOffset, _file); break;
					}
					break;

				case "~":
					switch (NextCharacter())
					{
						case "=": NextCharacter(); _currentToken = new Token(TokenID.OpAssignBitwiseNot, "~=", _lexerLine, _lexerOffset, _file); break;
						default: _currentToken = new Token(TokenID.OpBitwiseNot, "~", _lexerLine, _lexerOffset, _file); break;
					}
					break;

				case "|":
					switch (NextCharacter())
					{
						case "=": NextCharacter(); _currentToken = new Token(TokenID.OpAssignBitwiseOr, "|=", _lexerLine, _lexerOffset, _file); break;
						case "|": NextCharacter(); _currentToken = new Token(TokenID.OpLogicalOr, "||", _lexerLine, _lexerOffset, _file); break;
						default: _currentToken = new Token(TokenID.OpBitwiseOr, "|", _lexerLine, _lexerOffset, _file); break;
					}
					break;

				case "&":
					switch (NextCharacter())
					{
						case "=": NextCharacter(); _currentToken = new Token(TokenID.OpAssignBitwiseAnd, "&=", _lexerLine, _lexerOffset, _file); break;
						case "&": NextCharacter(); _currentToken = new Token(TokenID.OpLogicalAnd, "&&", _lexerLine, _lexerOffset, _file); break;
						default: _currentToken = new Token(TokenID.OpBitwiseAnd, "&", _lexerLine, _lexerOffset, _file); break;
					}
					break;

				case "%":
					switch (NextCharacter())
					{
						case "=": NextCharacter(); _currentToken = new Token(TokenID.OpAssignModulus, "%=", _lexerLine, _lexerOffset, _file); break;
						default: _currentToken = new Token(TokenID.OpModulus, "%", _lexerLine, _lexerOffset, _file); break;
					}
					break;

				case "-":
					switch (NextCharacter())
					{
                        case ">": NextCharacter(); _currentToken = new Token(TokenID.OpMemberResolver, "->", _lexerLine, _lexerOffset, _file); break;
						case "=": NextCharacter(); _currentToken = new Token(TokenID.OpAssignSub, "-=", _lexerLine, _lexerOffset, _file); break;
						case "-": NextCharacter(); _currentToken = new Token(TokenID.OpDecrement, "--", _lexerLine, _lexerOffset, _file); break;
						default: _currentToken = new Token(TokenID.OpSub, "-", _lexerLine, _lexerOffset, _file); break;
					}
					break;

				case "+":
					switch (NextCharacter())
					{
						case "=": NextCharacter(); _currentToken = new Token(TokenID.OpAssignAdd, "+=", _lexerLine, _lexerOffset, _file); break;
						case "+": NextCharacter(); _currentToken = new Token(TokenID.OpIncrement, "++", _lexerLine, _lexerOffset, _file); break;
						default: _currentToken = new Token(TokenID.OpAdd, "+", _lexerLine, _lexerOffset, _file); break;
					}
					break;

				case "/":
					switch (NextCharacter())
					{
						case "*": //Parse through multiple layers of block comments.
							#region Block Comment Lexing

							int commentDepth = 1;
							while (commentDepth > 0)
							{
								switch (NextCharacter())
								{
									case "*":
										if (NextCharacter() == "/")
										{
											commentDepth--;
											NextCharacter();
										}
										break;
									case "/":
										if (NextCharacter() == "*")
										{
											commentDepth++;
											NextCharacter();
										}
										break;
								}
							}

							#endregion
							break;
						case "/":
							while (_currentChar != "\n" && !EndOfTokenStream()) 
                                NextCharacter();
							break;
						case "=": NextCharacter(); _currentToken = new Token(TokenID.OpAssignDivide, "/=", _lexerLine, _lexerOffset, _file); break;
						default: _currentToken = new Token(TokenID.OpDivide, "/", _lexerLine, _lexerOffset, _file); break;
					}
					break;

				case "*":
					switch (NextCharacter())
					{
						case "=": NextCharacter(); _currentToken = new Token(TokenID.OpAssignMultiply, "*=", _lexerLine, _lexerOffset, _file); break;
						default: _currentToken = new Token(TokenID.OpMultiply, "*", _lexerLine, _lexerOffset, _file); break;
					}
					break;

				case "<":
					switch (NextCharacter())
					{
						case "=": NextCharacter(); _currentToken = new Token(TokenID.OpLessEqual, "<=", _lexerLine, _lexerOffset, _file); break;
						case "<":
							switch (NextCharacter())
							{
								case "=": NextCharacter(); _currentToken = new Token(TokenID.OpAssignBitwiseSHL, "<<=", _lexerLine, _lexerOffset, _file); break;
								default: _currentToken = new Token(TokenID.OpBitwiseSHL, "<<", _lexerLine, _lexerOffset, _file); break;
							}
							break;
						default: _currentToken = new Token(TokenID.OpLess, "<", _lexerLine, _lexerOffset, _file); break;
					}
					break;

				case ">":
					switch (NextCharacter())
					{
						case "=": NextCharacter(); _currentToken = new Token(TokenID.OpGreaterEqual, ">=", _lexerLine, _lexerOffset, _file); break;
						case ">":
							switch (NextCharacter())
							{
								case "=": NextCharacter(); _currentToken = new Token(TokenID.OpAssignBitwiseSHR, ">>=", _lexerLine, _lexerOffset, _file); break;
								default: _currentToken = new Token(TokenID.OpBitwiseSHR, ">>", _lexerLine, _lexerOffset, _file); break;
							}
							break;
						default: _currentToken = new Token(TokenID.OpGreater, ">", _lexerLine, _lexerOffset, _file); break;
					}
					break;

				case "!":
					switch (NextCharacter())
					{
						case "=": NextCharacter(); _currentToken = new Token(TokenID.OpNotEqual, "!=", _lexerLine, _lexerOffset, _file); break;
						default: _currentToken = new Token(TokenID.OpLogicalNot, "!", _lexerLine, _lexerOffset, _file); break;
					}
					break;

				case "{": NextCharacter(); _currentToken = new Token(TokenID.CharOpenBrace, "{", _lexerLine, _lexerOffset, _file); break;
				case "}": NextCharacter(); _currentToken = new Token(TokenID.CharCloseBrace, "}", _lexerLine, _lexerOffset, _file); break;
				case "[": NextCharacter(); _currentToken = new Token(TokenID.CharOpenBracket, "[", _lexerLine, _lexerOffset, _file); break;
				case "]": NextCharacter(); _currentToken = new Token(TokenID.CharCloseBracket, "]", _lexerLine, _lexerOffset, _file); break;
				case "(": NextCharacter(); _currentToken = new Token(TokenID.CharOpenParenthesis, "(", _lexerLine, _lexerOffset, _file); break;
				case ")": NextCharacter(); _currentToken = new Token(TokenID.CharCloseParenthesis, ")", _lexerLine, _lexerOffset, _file); break;
				case ",": NextCharacter(); _currentToken = new Token(TokenID.CharComma, ",", _lexerLine, _lexerOffset, _file); break;
				case ":": NextCharacter(); _currentToken = new Token(TokenID.CharColon, ":", _lexerLine, _lexerOffset, _file); break;
				case ";": NextCharacter(); _currentToken = new Token(TokenID.CharSemiColon, ";", _lexerLine, _lexerOffset, _file); break;
				case ".": NextCharacter(); _currentToken = new Token(TokenID.CharPeriod, ".", _lexerLine, _lexerOffset, _file); break;
				case "#": NextCharacter(); _currentToken = new Token(TokenID.CharDirective, "#", _lexerLine, _lexerOffset, _file); break;

				case " ":
				case "\t":
				case "\n":
				case "\r":
					// We can safely ignore any whitespace characters.
					NextCharacter();
					break;

				case "'":
				case "\"": // String literals
					#region String Literal Lexing

					string stringLiteral = "";
					string stringStartChar = _currentChar;
					while (true)
					{
						if (NextCharacter() == stringStartChar) break;
						if (EndOfTokenStream() == true || _currentChar == "\n")
						{
							Error(ErrorCode.UnfinishedStringLiteral, "Unfinished string literal");
							break;
						}

						// Check for escape sequence
						if (_currentChar == "\\")
						{
							NextCharacter();
							switch (_currentChar)
							{
								case "'":
								case "\"": stringLiteral += stringStartChar; break;
								case "a": stringLiteral += "\a"; break;
								case "b": stringLiteral += "\b"; break;
								case "f": stringLiteral += "\f"; break;
								case "n": stringLiteral += "\n"; break;
								case "r": stringLiteral += "\r"; break;
								case "t": stringLiteral += "\t"; break;
								case "v": stringLiteral += "\v"; break;
								case "0": stringLiteral += "\0"; break;
								case "\\": stringLiteral += "\\"; break;
								default:
									Error(ErrorCode.InvalidEscapeSequence, "Found invalid escape sequence '\\" + _currentChar + "' in string literal.");
									break;
							}
						}
						else
							stringLiteral += _currentChar;
					}

					NextCharacter();
					_currentToken = new Token(TokenID.TypeString, stringLiteral, _lexerLine, _lexerOffset, _file);

					#endregion
					break;

				default:
					if ((_currentChar[0] >= 'A' && _currentChar[0] <= 'Z') || 
						(_currentChar[0] >= 'a' && _currentChar[0] <= 'z') || 
						 _currentChar[0] == '_') // Check if its a identifier.
					{
						#region Identiier Lexing

						TokenID identType = TokenID.TypeIdentifier;
						string identLiteral = "";
						int index = 0;

						while (true)
						{
							identLiteral += _currentChar;
							if (_currentChar[0] >= '0' && _currentChar[0] <= '9' && index == 0)
							{
								Error(ErrorCode.MalformedIdentifier, "Found malformed identifier.");
								break;
							}

							// Go to next character.
							NextCharacter();
							if (EndOfTokenStream() || StringMethods.IsStringIdentifier(_currentChar) == false) break;
							index++;
						}

						// Check if its a keyword or not.
						switch (identLiteral.ToLower())
						{
							case "state":	identType = TokenID.KeywordState;	break;
							case "event":	identType = TokenID.KeywordEvent;	break;
							case "default": identType = TokenID.KeywordDefault; break;
							case "switch":	identType = TokenID.KeywordSwitch;	break;
							case "case":	identType = TokenID.KeywordCase;	break;
							case "if":		identType = TokenID.KeywordIf;		break;
							case "else":	identType = TokenID.KeywordElse;	break;
							case "enum":	identType = TokenID.KeywordEnum;	break;
							case "const":	identType = TokenID.KeywordConst;	break;
							case "ref":		identType = TokenID.KeywordRef;		break;
							case "for":		identType = TokenID.KeywordFor;		break;
							case "return":	identType = TokenID.KeywordReturn;	break;
							case "while":	identType = TokenID.KeywordWhile;	break;
							case "do":		identType = TokenID.KeywordDo;		break;
							case "new":		identType = TokenID.KeywordNew;		break;
							case "delete":	identType = TokenID.KeywordDelete;	break;
							case "lock":	identType = TokenID.KeywordLock;	break;
							case "atom":	identType = TokenID.KeywordAtom;	break;
							case "object":	identType = TokenID.KeywordObject;	break;
							case "string":	identType = TokenID.KeywordString;	break;
							case "bool":	identType = TokenID.KeywordBool;	break;
							case "byte":	identType = TokenID.KeywordByte;	break;
							case "int":		identType = TokenID.KeywordInt;		break;
							case "short":	identType = TokenID.KeywordShort;	break;
							case "float":	identType = TokenID.KeywordFloat;	break;
							case "double":	identType = TokenID.KeywordDouble;	break;
							case "long":	identType = TokenID.KeywordLong;	break;
							case "void":	identType = TokenID.KeywordVoid;	break;
							case "thread":	identType = TokenID.KeywordThread;	break;
							case "goto":	identType = TokenID.KeywordGoto;	break;
							case "break":   identType = TokenID.KeywordBreak;	break;
							case "continue":identType = TokenID.KeywordContinue;break;
							case "true":	identType = TokenID.TypeBoolean;	break;
							case "false":	identType = TokenID.TypeBoolean;	break;
							case "null":	identType = TokenID.TypeNull;		identLiteral = "0"; break;
							case "indexer": identType = TokenID.KeywordIndexer; break;
							case "gotostate": identType = TokenID.KeywordGotoState; break;
							case "static": identType = TokenID.KeywordStatic; break;
							case "breakpoint": identType = TokenID.KeywordBreakpoint; break;
							case "property": identType = TokenID.KeywordProperty; break;
							case "private": identType = TokenID.KeywordPrivate; break;
							case "public": identType = TokenID.KeywordPublic; break;
							case "protected": identType = TokenID.KeywordProtected; break;
							case "struct": identType = TokenID.KeywordStruct; break;
							case "class": identType = TokenID.KeywordClass; break;
							case "engine": identType = TokenID.KeywordEngine; break;
							case "editor": identType = TokenID.KeywordEditor; break;
                            case "namespace": identType = TokenID.KeywordNamespace; break;
                            case "console": identType = TokenID.KeywordConsole; break;
                            case "import": identType = TokenID.KeywordImport; break;
                            case "export": identType = TokenID.KeywordExport; break;
						}

						_currentToken = new Token(identType, identLiteral, _lexerLine, _lexerOffset, _file);

						#endregion
					}
					else if (StringMethods.IsStringNumeric(_currentChar)) // Check if we are dealing with a number.
					{
						#region Numeric Lexing

						TokenID numberType = TokenID.TypeInteger;
						string numberLiteral = "";
						bool foundRadix = false, isHex = false, foundTypeSpecifier = false;
						int index = 0;

						while (true)
						{
							numberLiteral += _currentChar;
							if (_currentChar == "x")
								if (index == 1 && numberLiteral[0] == '0' && isHex == false)
									isHex = true;
								else
								{
									Error(ErrorCode.MalformedHex, "Found malformed hexidecimal literal.");
									break;
								}

							if ((_currentChar[0] >= 'A' && _currentChar[0] <= 'F') ||
								(_currentChar[0] >= 'a' && _currentChar[0] <= 'f'))
							{
								if (isHex == false && foundTypeSpecifier == true)
								{
									Error(ErrorCode.MalformedHex, "Found malformed floating point literal.");
									break;
								}
							}

							if (_currentChar == ".")
								if (foundRadix == false)
								{
									foundRadix = true;
									numberType = TokenID.TypeFloat;
								}
								else
								{
									Error(ErrorCode.MalformedFloat, "Found malformed floating point literal.");
									break;
								}

							if (_currentChar == "-")
								if (!(index == 0))
								{
									Error(ErrorCode.MalformedInteger, "Found malformed integer literal.");
									break;
								}

							if (_currentChar == "f")
							{
								foundTypeSpecifier = true;
								numberType = TokenID.TypeFloat;
							}

							if (_currentChar == "d")
							{
								foundTypeSpecifier = true;
								numberType = TokenID.TypeDouble;
							}

							if (_currentChar == "l")
							{
								if (foundRadix == true) Error(ErrorCode.MalformedDataTypeSpecifier, "Long type specifier can't be applied to float-point values.");
								foundTypeSpecifier = true;
								numberType = TokenID.TypeLong;
							}

							if (_currentChar == "s")
							{
								if (foundRadix == true) Error(ErrorCode.MalformedDataTypeSpecifier, "Short type specifier can't be applied to float-point values.");
								foundTypeSpecifier = true;
								numberType = TokenID.TypeShort;
							}

							if (_currentChar == "b")
							{
								if (foundRadix == true) Error(ErrorCode.MalformedDataTypeSpecifier, "Byte type specifier can't be applied to float-point values.");
								foundTypeSpecifier = true;
								numberType = TokenID.TypeByte;
							}

							// Go to next character.
							NextCharacter();
							if (EndOfTokenStream() || StringMethods.IsStringNumeric(_currentChar) == false) break;

							// Make sure that the type specifier (as in 0.00f) is on the end.
							if (foundTypeSpecifier == true)
							{
								Error(ErrorCode.MalformedDataTypeSpecifier, "Found malformed data type specifier.");
								break;
							}
							index++;
						}

						// If there is a type specifier attached to the end of this number
						// then remove it.
						if (foundTypeSpecifier == true) numberLiteral = numberLiteral.Substring(0, numberLiteral.Length - 1);

						// Create the numeric token.
						_currentToken = new Token(numberType, numberLiteral, _lexerLine, _lexerOffset, _file);

						#endregion
					}
					else
					{
						Error(ErrorCode.InvalidCharacter, "Found invalid character '" + _currentChar + "' while analysing tokens.");
						NextCharacter();
						break;
					}
					break;

			}

			return _currentToken;
		}

		#endregion
	}

}