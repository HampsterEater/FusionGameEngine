/* 
 * File: Compiler.cs
 *
 * This source file contains the declarations of any and all classes
 * used to compile scripts to byte code.
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
	///		Describes what data type a variable or token is. 
	/// </summary>
	public enum DataType : byte
	{
		Object,
		String,
		Bool,
		Byte,
		Int,
		Short,
		Float,
		Long,
		Double,
		Void,
		Invalid,
		Null,
	}

	/// <summary>
	///		Used to store details on a data type, including array 
	///		and reference details.
	/// </summary>
	public sealed class DataTypeValue
	{
		#region Members
		#region Variables

		private DataType _dataType;
		private bool _isArray;
		private bool _isReference;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the data type this value is storing.
		/// </summary>
		public DataType DataType
		{
			get { return _dataType;  }
			set { _dataType = value; }
		}

		/// <summary>
		///		Gets or sets is this value is an array.
		/// </summary>
		public bool IsArray
		{
			get { return _isArray;  }
			set { _isArray = value; }
		}

		/// <summary>
		///		Gets or sets if this value is a reference.
		/// </summary>
		public bool IsReference
		{
			get { return _isReference;  }
			set { _isReference = value; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Converts this data type to a textural form.
		/// </summary>
		/// <returns>Data type in a textural form.</returns>
		public override string ToString()
		{
			return _dataType.ToString() + (_isArray == true ? "[]" : "") + (_isReference == true ? " *" : "");
		}

		/// <summary>
		///		Returns true if this instance contains the same value as a given one.
		/// </summary>
		/// <param name="obj">Object to check against.</param>
		/// <returns>True if object contains the same value as this instance.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is DataTypeValue)) return false;
			DataTypeValue v1 = obj as DataTypeValue;
			return ((v1.DataType == _dataType) && (v1.IsArray == _isArray) && (v1.IsReference == _isReference));
		}

		/// <summary>
		///		Gets a hash code based on this object.
		/// </summary>
		/// <returns>Hash code based on this object.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		///		Checks if 2 instances of this class contain the same value.
		/// </summary>
		/// <param name="v1">Value to check against.</param>
		/// <param name="v2">Value to check.</param>
		/// <returns>True if values contain the same value.</returns>
		public static bool operator ==(DataTypeValue v1, DataTypeValue v2)
		{
			return v1.Equals(v2);
		}
		public static bool operator !=(DataTypeValue v1, DataTypeValue v2)
		{
			return !v1.Equals(v2);
		}

		/// <summary>
		///		Converts a mnemonic representation of a data type (eg. Void[]) into
		///		a DataTypeValue instance.
		/// </summary>
		/// <param name="mnemonic">Mnemonic to convert</param>
		/// <returns>A new DataTypeValue instance representing the mnemonic, or null if the mnemonic is invalid</returns>
		public static DataTypeValue FromMnemonic(string mnemonic)
		{
			DataType dataType = DataType.Invalid;
			bool isReference = false, isArray = false;

			mnemonic = mnemonic.ToLower().Trim();
			if (mnemonic.EndsWith("*") == true)
			{
				isReference = true;
				mnemonic = mnemonic.Substring(0, mnemonic.Length - 2);
			}
			if (mnemonic.EndsWith("[]") == true)
			{
				isArray = true;
				mnemonic = mnemonic.Substring(0, mnemonic.Length - 3);
			}

			int index = 0;
			foreach (string name in Enum.GetNames(typeof(DataType)))
			{
				if (name.ToLower() == mnemonic)
				{
					dataType = (DataType)index;
					break;
				}
				index++;
			}

			return new DataTypeValue(dataType, isArray, isReference);
		}

		/// <summary>
		///		Initializes a new instance of this class.
		/// </summary>
		/// <param name="dataType">The data type this value should store.</param>
		/// <param name="isArray">If this is an array or not.</param>
		/// <param name="isReference">If this is a reference or not.</param>
		public DataTypeValue(DataType dataType, bool isArray, bool isReference)
		{
			_dataType = dataType;
			_isArray = isArray;
			_isReference = isReference;
		}

		#endregion
	}

	/// <summary>
	///		Lists all the different registers used by the VM.
	/// </summary>
	public enum Register : byte
	{
		Return = 0,
		Arithmetic1,
		Arithmetic2,
		Arithmetic3,
		Reserved1,
		Reserved2,
		Reserved3,
        Reserved4,
		Compare,
        Member
	}

	/// <summary>
	///		Used to create a bitmask describing how a compiler
	///		should compile and individual script.
	/// </summary>
	public enum CompileFlags : byte
	{
		Debug = 0x00000001,
		TreatWarningsAsErrors = 0x00000002,
		TreatMessagesAsErrors = 0x00000004,
		Library = 0x00000008
	}

	/// <summary>
	///		Lists all the different operation codes used by
	///		the VM to preform complex tasks.
	/// </summary>
	public enum OpCode : byte
	{
		INVALID,

        // Pointless really, only used for tricky optimization situations.
        NOP,

		// Execution related op codes.
		EXIT,
		PAUSE,
		GOTO_STATE,

		// Stack frame minipulation op codes.
		CALL,
		RETURN,

		POP_OBJECT,
		POP_BOOL,
		POP_BYTE,
		POP_INT,
		POP_SHORT,
		POP_FLOAT,
		POP_STRING,
		POP_LONG,
		POP_DOUBLE,
		POP_NULL,
		POP_MEMORY_INDEX,
		POP_DESTROY,

		PUSH_OBJECT,
		PUSH_BOOL,
		PUSH_BYTE,
		PUSH_INT,
		PUSH_SHORT,
		PUSH_FLOAT,
		PUSH_STRING,
		PUSH_LONG,
		PUSH_DOUBLE,
		PUSH_NULL,
		PUSH_MEMORY_INDEX,

		// Flow of control op codes.
		CMP_NULL,
		CMP_Object,
		CMP_BOOL,
		CMP_BYTE,
		CMP_INT,
		CMP_SHORT,
		CMP_FLOAT,
		CMP_STRING,
		CMP_LONG,
		CMP_DOUBLE,
		CMP_MEMORY_INDEX,

		JMP_EQ,
		JMP_L,
		JMP_G,
		JMP_LE,
		JMP_GE,
		JMP_NE,
		JMP,

		IS_EQ,
		IS_L,
		IS_G,
		IS_LE,
		IS_GE,
		IS_NE,

		LOGICAL_AND,
		LOGICAL_OR,
		LOGICAL_NOT,

		// Arithmetic op codes.
		MOV_BOOL,
		MOV_BYTE,
		MOV_INT,
		MOV_SHORT,
		MOV_FLOAT,
		MOV_DOUBLE,
		MOV_LONG,
		MOV_OBJECT,
		MOV_NULL,
		MOV_STRING,
		MOV_MEMORY_INDEX,

		MUL_BYTE,
		MUL_INT,
		MUL_SHORT,
		MUL_FLOAT,
		MUL_DOUBLE,
		MUL_LONG,

		DIV_BYTE,
		DIV_INT,
		DIV_SHORT,
		DIV_FLOAT,
		DIV_DOUBLE,
		DIV_LONG,

		ADD_STRING,
		ADD_BYTE,
		ADD_INT,
		ADD_SHORT,
		ADD_FLOAT,
		ADD_DOUBLE,
		ADD_LONG,

		SUB_BYTE,
		SUB_INT,
		SUB_SHORT,
		SUB_FLOAT,
		SUB_DOUBLE,
		SUB_LONG,

		INC_BYTE,
		INC_INT,
		INC_SHORT,
		INC_FLOAT,
		INC_DOUBLE,
		INC_LONG,

		DEC_BYTE,
		DEC_INT,
		DEC_SHORT,
		DEC_FLOAT,
		DEC_DOUBLE,
		DEC_LONG,

		NEG_BYTE,
		NEG_INT,
		NEG_SHORT,
		NEG_FLOAT,
		NEG_DOUBLE,
		NEG_LONG,

		ABS_BYTE,
		ABS_INT,
		ABS_SHORT,
		ABS_FLOAT,
		ABS_DOUBLE,
		ABS_LONG,

		BIT_OR_INT,
		BIT_AND_INT,
		BIT_XOR_INT,
		BIT_NOT_INT,
		BIT_SHL_INT,
		BIT_SHR_INT,

		EXP_INT,

		MOD_INT,

		// System op codes.
		CAST_NULL,
		CAST_BOOL,
		CAST_BYTE,
		CAST_INT,
		CAST_SHORT,
		CAST_FLOAT,
		CAST_DOUBLE,
		CAST_LONG,
		CAST_Object,
		CAST_STRING,

		// Debug op codes.
		BREAKPOINT,
        ENTER_STATEMENT,
        EXIT_STATEMENT,

		// Memory allocation op codes.
		ALLOCATE_HEAP_NULL,
		ALLOCATE_HEAP_BOOL,
		ALLOCATE_HEAP_BYTE,
		ALLOCATE_HEAP_INT,
		ALLOCATE_HEAP_SHORT,
		ALLOCATE_HEAP_FLOAT,
		ALLOCATE_HEAP_DOUBLE,
		ALLOCATE_HEAP_LONG,
		ALLOCATE_HEAP_Object,
		ALLOCATE_HEAP_STRING,
		DEALLOCATE_HEAP,

		// Thread syncronization op codes.
		LOCK,
		UNLOCK,
		ENTER_ATOM,
		LEAVE_ATOM,

        // Method accessing op codes.
        CALL_METHOD, // CALL_METHOD Object, Method Symbol
        GET_MEMBER, // GET_MEMBER Object, Member Symbol 
        GET_MEMBER_INDEXED, // GET_MEMBER_INDEXED Object, Member Symbol, Index
        SET_MEMBER, // SET_MEMBER Object, Member Symbol, Value 
        SET_MEMBER_INDEXED  // SET_MEMBER Object, Member Symbol, Value, Index
    }

	/// <summary>
	///		Describes a data type inspecfic operation code.
	/// </summary>
	public enum OpCodeType : byte
	{
		PUSH,
		POP,
		CMP,
		MOV,
		MUL,
		DIV,
		ADD,
		SUB,
		INC,
		DEC,
		NEG,
		ABS,
		CAST,
		ALLOCATE_HEAP
	}

	/// <summary>
	///		List all the errors that can be emitted by a script.
	/// </summary>
	public enum ErrorCode : byte
	{
		ExpectingToken,
		InvalidCharacter,
		UnfinishedStringLiteral,
		InvalidEscapeSequence,
		MalformedHex,
		MalformedFloat,
		MalformedInteger,
		MalformedDataTypeSpecifier,
		MalformedIdentifier,
		InvalidDirective,
		DirectiveError, // Used when #warning, #error or #message are found.
		UnexpectedToken,
		InvalidScope,
		InvalidFlag,
		DuplicateFlag,
		DuplicateSymbol,
		InvalidConst,
		InvalidDataType,
		IllegalAssignment,
		InvalidCast,
		InvalidFactor,
		NonExistantIncludeFile,
		InvalidArrayIndex,
		ExpectingReturnValue,
		UndeclaredVariable,
		IllegalAssignmentOperator,
		InvalidParameters,
		UnusedVariable,
		MissingDefaultState,
		InvalidFunction,
		InvalidLoopIndex,
		InvalidBreak,
		InvalidContinue,
		InvalidIndexer,
		DuplicateDefault,
		InvalidNesting,
		InternalError,
		UnsupportedFeature,
        IllegalResolve
	}

	/// <summary>
	///		Used by the compiler to break out of the main compilation loop.
	/// </summary>
	public class CompileBreakException : Exception { }

	/// <summary>
	///		Used by the compiler when exiting panic mode to get out of the 
	///		current parsing state.
	/// </summary>
	public class CompilePanicModeException : Exception { }

	/// <summary>
	///		Used by the CompilerError class to describe the priority of a given error.
	/// </summary>
	public enum ErrorAlertLevel
	{
		Message,
		Warning,
		Error,
		FatalError
	}

	/// <summary>
	///		Describes an error that occurs while compiling a script.
	/// </summary>
	public sealed class CompileError
	{
		#region Members
		#region Variables

		private ErrorCode _errorCode;
		private string _message;
		private int _line, _offset;
        private string _file;
		private ErrorAlertLevel _alertLevel;

		#endregion
		#region Properties

		/// <summary>
		///		Gets the error code describing how this error
		///		was caused.
		/// </summary>
		public ErrorCode ErrorCode
		{
			get { return _errorCode; }
		}

		/// <summary>
		///		Gets a string containing a description of this error.
		/// </summary>
		public string Message
		{
			get { return _message; }
		}

		/// <summary>
		///		Gets the line this error occured on.
		/// </summary>
		public int Line
		{
			get { return _line; }
		}

		/// <summary>
		///		Gets the offset in character on the line this error occured on.
		/// </summary>
		public int Offset
		{
			get { return _offset; }
		}

        /// <summary>
        ///		Gets the file this error occured in.
        /// </summary>
        public string File
        {
            get { return _file; }
        }

		/// <summary>
		///		Gets the alert level of this error.
		/// </summary>
		public ErrorAlertLevel AlertLevel
		{
			get { return _alertLevel; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Returns a string describing where and how the error occured.
		/// </summary>
		/// <returns>A string describing where and how the error occured.</returns>
		public override string ToString()
		{
			string errorDescription = "";
			switch (AlertLevel)
			{
				case ErrorAlertLevel.FatalError: errorDescription = "Fatal Error "; break;
				case ErrorAlertLevel.Error: errorDescription = "Error "; break;
				case ErrorAlertLevel.Warning: errorDescription = "Warning "; break;
				case ErrorAlertLevel.Message: errorDescription = "Message "; break;
			}
			return errorDescription + ((int)_errorCode) + " occured on line " + _line + " at offset " + _offset + " in " + Path.GetFileName(_file) + ": " + _message;
		}

		/// <summary>
		///		Initializes a new instance of this class.
		/// </summary>
		/// <param name="code">Error code describing how this error occured.</param>
		/// <param name="msg">String containg a description of how and where this error occured.</param>
		/// <param name="alertLevel">Describes the priority of this error.</param>
		/// <param name="line">The line this error occured on.</param>
		/// <param name="offset">The offset, in character, on the line this error occured on.</param>
		public CompileError(ErrorCode code, string msg, ErrorAlertLevel alertLevel, int line, int offset, string file)
		{
			_errorCode = code;
			_message = msg;
			_line = line;
			_offset = offset;
            _file = file;
			_alertLevel = alertLevel;
		}

		#endregion
	}

	/// <summary>
	///		Encapsulates a single byte code instruction.
	/// </summary>
	public sealed class Instruction
	{
		#region Members
		#region Variables

		private OpCode _opCode;
		private Operand[] _operands = new Operand[5]; // Maximum of 5 ops per instruction.
		private int _line, _offset;
		private string _file;
		private int _operandCount;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the array of operands used by this instruction.
		/// </summary>
		public Operand[] Operands
		{
			get { return _operands; }
			set { _operands = value; }
		}

		/// <summary>
		///		Gets or sets the file this instruction was 
		///		generated from.
		/// </summary>
		public string File
		{
			get { return _file; }
			set { _file = value; }
		}

		/// <summary>
		///		Gets or sets the offset on the line this instruction 
		///		was generated on.
		/// </summary>
		public int Offset
		{
			get { return _offset; }
			set { _offset = value; }
		}

		/// <summary>
		///		Gets or sets the line this instruction was generated on.
		/// </summary>
		public int Line
		{
			get { return _line; }
			set { _line = value; }
		}

		/// <summary>
		///		Gets or sets the operation code of this instruction.
		/// </summary>
		public OpCode OpCode
		{
			get { return _opCode; }
			set { _opCode = value; }
		}

		/// <summary>
		///		Gets or sets the operand at a given index of this instruction operand list.
		/// </summary>
		/// <param name="index">Operand index.</param>
		/// <returns>Operand at the given index.</returns>
		public Operand this[int index]
		{
			get
			{
				if (index + 1 > _operandCount)
				{
					_operandCount = index + 1;
					Array.Resize<Operand>(ref _operands, _operandCount);
				}
				return _operands[index];
			}
			set
			{
				if (index + 1 > _operandCount)
				{
					_operandCount = index + 1;
					Array.Resize<Operand>(ref _operands, _operandCount);
				}
				_operands[index] = value;
			}
		}

		/// <summary>
		///		Returns the amount of operands associated with this instruction.
		/// </summary>
		public int OperandCount
		{
			get { return _operandCount; }
			set { _operandCount = value; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Returns a string representing the values stored in this object.
		/// </summary>
		/// <returns>A string representing the values stored in this object.</returns>
		public override string ToString()
		{
			return _opCode.ToString();
		}

		/// <summary>
		///		Decompiles this instruction to an pusedo-ASM representation of it.
		/// </summary>
		/// <returns>Pusedo-ASM representation of this instruction.</returns>
		public string Decompile()
		{
			string asmString = (_opCode.ToString() + "").PadRight(30);
			for (int i = 0; i < _operandCount; i++)
			{
				Operand op = _operands[i];
				switch (op.OpType)
				{
					case OperandType.BooleanLiteral:
						asmString += op.BooleanLiteral;
						break;
					case OperandType.ByteLiteral:
						asmString += op.ByteLiteral;
						break;
					case OperandType.DirectMemory:
						asmString += "memory[" + op.MemoryIndex + "]";
						break;
					case OperandType.DirectMemoryIndexed:
						asmString += "memory[" + op.MemoryIndex + " + " + op.OffsetRegister.ToString() + "]";
						break;
					case OperandType.IndirectMemory:
						asmString += "memory[" + op.Register.ToString() + "]";
						break;
					case OperandType.IndirectMemoryIndexed:
						asmString += "memory[" + op.Register.ToString() + " + " + op.OffsetRegister.ToString() + "]";
						break;
					case OperandType.DirectStack:
						asmString += "stack[" + op.StackIndex + "]";
						break;
					case OperandType.DirectStackIndexed:
						asmString += "stack[" + op.StackIndex + " + " + op.OffsetRegister.ToString() + "]";
						break;
					case OperandType.IndirectStack:
						asmString += "stack[" + op.Register.ToString() + "]";
						break;
					case OperandType.IndirectStackIndexed:
						asmString += "stack[" + op.Register.ToString() + " + " + op.OffsetRegister.ToString() + "]";
						break;
					case OperandType.FloatLiteral:
						asmString += op.FloatLiteral;
						break;
					case OperandType.DoubleLiteral:
						asmString += op.DoubleLiteral;
						break;
					case OperandType.InstrIndex:
						asmString += "instruction[" + op.InstrIndex + "]";
						break;
					case OperandType.IntegerLiteral:
						asmString += op.IntegerLiteral;
						break;
					case OperandType.LongLiteral:
						asmString += op.LongLiteral;
						break;
					case OperandType.Register:
						asmString += op.Register.ToString();
						break;
					case OperandType.ShortLiteral:
						asmString += op.ShortLiteral;
						break;
					case OperandType.StringLiteral:
						asmString += "\"" + op.StringLiteral + "\"";
						break;
					case OperandType.SymbolIndex:
						asmString += op.SymbolIndexTracker.Identifier;
						break;
				}
				if (i < _operandCount - 1) asmString += ", ";
			}
			return asmString;
		}

		/// <summary>
		///		Initializes a new instruction index with the given operation code.
		/// </summary>
		/// <param name="opCode">Operation code that this instruction should use.</param>
		/// <param name="line">Specifies the line of code this instruction was generated from.</param>
		/// <param name="offset">Specifies the offset of the line of code this instruction was generated from.</param>
		/// <param name="file">Specifies the file from which this code was generated from.</param>
		public Instruction(OpCode opCode, int line, int offset, string file)
		{
			_opCode = opCode;
			_line = line;
			_offset = offset;
			_file = file;
		}
		public Instruction(OpCode opCode, int line, int offset)
		{
			_opCode = opCode;
			_line = line;
			_offset = offset;
		}
		public Instruction(OpCode opCode)
		{
			_opCode = opCode;
		}

		/// <summary>
		///		Initializes a new instruction and attachs it to the given scope.
		/// </summary>
		/// <param name="opCode">Operation code for this instruction.</param>
		/// <param name="scope">Scope this instruction should be added to.</param>
		/// <param name="token">Token to get debug infomation from.</param>
		public Instruction(OpCode opCode, Symbol scope, Token token)
		{
			scope.AddInstruction(this);
			_opCode = opCode;
			if (token != null)
			{
				_line = token.Line;
				_offset = token.Offset;
				_file = token.File;
			}
		}

		#endregion
	}

	/// <summary>
	///		Used by the Operand class to describe what value this operand contains.
	/// </summary>
	public enum OperandType
	{
		Invalid,

		BooleanLiteral,
		ByteLiteral,
		DirectMemory,
		DirectMemoryIndexed,
		IndirectMemory,
		IndirectMemoryIndexed,
		DirectStack,
		DirectStackIndexed,
		IndirectStack,
		IndirectStackIndexed,
		FloatLiteral,
		DoubleLiteral,
		InstrIndex,
		IntegerLiteral,
		LongLiteral,
		Register,
		ShortLiteral,
		StringLiteral,
		SymbolIndex,
		JumpTarget,
		SymbolIndexTracker
	}

	/// <summary>
	///		Used by the Instruction class to descripe a single 
	///		byte code instruction operand.
	/// </summary>
	public sealed class Operand
	{
		#region Members
		#region Variables

		private OperandType _opType;

		private string _stringLiteral;
		private bool _booleanLiteral;
		private byte _byteLiteral;
		private int _integerLiteral;
		private short _shortLiteral;
		private float _floatLiteral;
		private double _doubleLiteral;
		private long _longLiteral;

		private Register _register;
		private Register _offsetRegister;

		private int _stackIndex;
		private int _symbolIndex;

		private int _instrIndex;
		private int _memoryIndex;

		private JumpTargetSymbol _jumpTarget;

		private Symbol _symbolIndexTracker;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the operand type used by this operand.
		/// </summary>
		public OperandType OpType
		{
			get { return _opType; }
			set { _opType = value; }
		}

		/// <summary>
		///		Gets or sets the string literal used by this operand.
		/// </summary>
		public string StringLiteral
		{
			get { return _stringLiteral; }
			set { _stringLiteral = value; }
		}

		/// <summary>
		///		Gets or sets the instruction index used by this operand.
		/// </summary>
		public int InstrIndex
		{
			get { return _instrIndex; }
			set { _instrIndex = value; }
		}

		/// <summary>
		///		Gets or sets the symbol this operand is tracking used by this operand.
		/// </summary>
		public Symbol SymbolIndexTracker
		{
			get { return _symbolIndexTracker; }
			set { _symbolIndexTracker = value; }
		}

		/// <summary>
		///		Gets or sets the jump target used by this operand.
		/// </summary>
		public JumpTargetSymbol JumpTarget
		{
			get { return _jumpTarget; }
			set { _jumpTarget = value; }
		}

		/// <summary>
		///		Gets or sets the boolean literal used by this operand.
		/// </summary>
		public bool BooleanLiteral
		{
			get { return _booleanLiteral; }
			set { _booleanLiteral = value; }
		}

		/// <summary>
		///		Gets or sets the byte literal used by this this operand.
		/// </summary>
		public byte ByteLiteral
		{
			get { return _byteLiteral; }
			set { _byteLiteral = value; }
		}

		/// <summary>
		///		Gets or sets the integer literal used by this this operand.
		/// </summary>
		public int IntegerLiteral
		{
			get { return _integerLiteral; }
			set { _integerLiteral = value; }
		}

		/// <summary>
		///		Gets or sets the short literal used by this this operand.
		/// </summary>
		public short ShortLiteral
		{
			get { return _shortLiteral; }
			set { _shortLiteral = value; }
		}

		/// <summary>
		///		Gets or sets the float literal used by this this operand.
		/// </summary>
		public float FloatLiteral
		{
			get { return _floatLiteral; }
			set { _floatLiteral = value; }
		}

		/// <summary>
		///		Gets or sets the double literal used by this this operand.
		/// </summary>
		public double DoubleLiteral
		{
			get { return _doubleLiteral; }
			set { _doubleLiteral = value; }
		}

		/// <summary>
		///		Gets or sets the long literal used by this this operand.
		/// </summary>
		public long LongLiteral
		{
			get { return _longLiteral; }
			set { _longLiteral = value; }
		}

		/// <summary>
		///		Gets or sets the register used by this this operand.
		/// </summary>
		public Register Register
		{
			get { return _register; }
			set { _register = value; }
		}

		/// <summary>
		///		Gets or sets the offset register used by this this operand.
		/// </summary>
		public Register OffsetRegister
		{
			get { return _offsetRegister; }
			set { _offsetRegister = value; }
		}

		/// <summary>
		///		Gets or sets the stack index used by this this operand.
		/// </summary>
		public int StackIndex
		{
			get { return _stackIndex; }
			set { _stackIndex = value; }
		}

		/// <summary>
		///		Gets or sets the symbol index used by this this operand.
		/// </summary>
		public int SymbolIndex
		{
			get { return _symbolIndex; }
			set { _symbolIndex = value; }
		}

		/// <summary>
		///		Gets or sets the memory index used by this this operand.
		/// </summary>
		public int MemoryIndex
		{
			get { return _memoryIndex; }
			set { _memoryIndex = value; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Checks if this operands values are equal to that of another.
		/// </summary>
		/// <param name="obj">Operand to check against.</param>
		/// <returns>True if this operand and the given operand contain the same values.</returns>
		public override bool Equals(object obj)
		{
			Operand other = obj as Operand;
			if (_opType != other._opType) return false;
			switch (_opType)
			{
				case OperandType.BooleanLiteral:
					return (_booleanLiteral == other._booleanLiteral);
				case OperandType.ByteLiteral:
					return (_byteLiteral == other._byteLiteral);
				case OperandType.DirectMemory:
					return (_memoryIndex == other._memoryIndex);
				case OperandType.DirectMemoryIndexed:
					return (_memoryIndex == other._memoryIndex && _offsetRegister == other._offsetRegister);
				case OperandType.DirectStack:
					return (_stackIndex == other._stackIndex);
				case OperandType.DirectStackIndexed:
					return (_stackIndex == other._stackIndex && _offsetRegister == other._offsetRegister);
				case OperandType.DoubleLiteral:
					return (_doubleLiteral == other._doubleLiteral);
				case OperandType.FloatLiteral:
					return (_floatLiteral == other._floatLiteral);
				case OperandType.IndirectMemory:
					return (_register == other._register);
				case OperandType.IndirectMemoryIndexed:
					return (_register == other._register && _offsetRegister == other._offsetRegister);
				case OperandType.IndirectStack:
					return (_register == other._register);
				case OperandType.IndirectStackIndexed:
					return (_register == other._register && _offsetRegister == other._offsetRegister);
				case OperandType.InstrIndex:
					return (_instrIndex == other._instrIndex);
				case OperandType.IntegerLiteral:
					return (_integerLiteral == other._integerLiteral);
				case OperandType.JumpTarget:
					return (_jumpTarget == other._jumpTarget);
				case OperandType.LongLiteral:
					return (_longLiteral == other._longLiteral);
				case OperandType.Register:
					return (_register == other._register);
				case OperandType.ShortLiteral:
					return (_shortLiteral == other._shortLiteral);
				case OperandType.StringLiteral:
					return (_stringLiteral == other._stringLiteral);
				case OperandType.SymbolIndex:
					return (_symbolIndex == other._symbolIndex);
				case OperandType.SymbolIndexTracker:
					return (_symbolIndexTracker == other._symbolIndexTracker);
			}
			return false;
		}

		/// <summary>
		///		Gets the hash code for this object.
		/// </summary>
		/// <returns>Hash code for this object.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		///		Initializes a new Operand instance with the given instruction as a parent
		///		and a given operand type.
		/// </summary>
		/// <param name="instruction">Instruction this operand is associated with.</param>
		/// <param name="opType">Operand type describing what value this operand holds.</param>
		public Operand(Instruction instruction, OperandType opType)
		{
			_opType = opType;
			instruction[instruction.OperandCount] = this;
		}
		public Operand(Instruction instruction, bool value)
		{
			_opType = OperandType.BooleanLiteral;
			_booleanLiteral = value;
			instruction[instruction.OperandCount] = this;
		}
		public Operand(Instruction instruction, byte value)
		{
			_opType = OperandType.ByteLiteral;
			_byteLiteral = value;
			instruction[instruction.OperandCount] = this;
		}
		public Operand(Instruction instruction, float value)
		{
			_opType = OperandType.FloatLiteral;
			_floatLiteral = value;
			instruction[instruction.OperandCount] = this;
		}
		public Operand(Instruction instruction, int value)
		{
			_opType = OperandType.IntegerLiteral;
			_integerLiteral = value;
			instruction[instruction.OperandCount] = this;
		}
		public Operand(Instruction instruction, long value)
		{
			_opType = OperandType.LongLiteral;
			_longLiteral = value;
			instruction[instruction.OperandCount] = this;
		}
		public Operand(Instruction instruction, double value)
		{
			_opType = OperandType.DoubleLiteral;
			_doubleLiteral = value;
			instruction[instruction.OperandCount] = this;
		}
		public Operand(Instruction instruction, short value)
		{
			_opType = OperandType.ShortLiteral;
			_shortLiteral = value;
			instruction[instruction.OperandCount] = this;
		}
		public Operand(Instruction instruction, string value)
		{
			_opType = OperandType.StringLiteral;
			_stringLiteral = value;
			instruction[instruction.OperandCount] = this;
		}
		public Operand(Instruction instruction, Register value)
		{
			_opType = OperandType.Register;
			_register = value;
			instruction[instruction.OperandCount] = this;
		}
		public Operand(Instruction instruction, JumpTargetSymbol value)
		{
			_opType = OperandType.JumpTarget;
			_jumpTarget = value;
			instruction[instruction.OperandCount] = this;
		}
		public Operand(Instruction instruction, Symbol symbol)
		{
			_opType = OperandType.SymbolIndexTracker;
			_symbolIndexTracker = symbol;
			instruction[instruction.OperandCount] = this;
		}
		public Operand()
		{
		}

		/// <summary>
		///		Creates a new instruction index operand.
		/// </summary>
		/// <param name="instruction">Instruction to add operand to.</param>
		/// <param name="instrIndex">Index of instruction this operand should point to.</param>
		/// <returns>New operand instance.</returns>
		public static Operand InstrIndexOperand(Instruction instruction, int instrIndex)
		{
			Operand op = new Operand(instruction, OperandType.InstrIndex);
			op.InstrIndex = instrIndex;
			return op;
		}

		/// <summary>
		///		Creates a new symbol index operand.
		/// </summary>
		/// <param name="instruction">Instruction to add operand to.</param>
		/// <param name="symbolIndex">Index of symbol this operand should point to.</param>
		/// <returns>New operand instance.</returns>
		public static Operand SymbolIndexOperand(Instruction instruction, int symbolIndex)
		{
			Operand op = new Operand(instruction, OperandType.SymbolIndex);
			op.SymbolIndex = symbolIndex;
			return op;
		}

		/// <summary>
		///		Creates a new direct memory operand.
		/// </summary>
		/// <param name="instruction">Instruction to add operand to.</param>
		/// <param name="memoryIndex">Index of memory slot this operand should point to.</param>
		/// <returns>New operand instance.</returns>
		public static Operand DirectMemoryOperand(Instruction instruction, int memoryIndex)
		{
			Operand op = new Operand(instruction, OperandType.DirectMemory);
			op.MemoryIndex = memoryIndex;
			return op;
		}

		/// <summary>
		///		Creates a new direct memory index operand.
		/// </summary>
		/// <param name="instruction">Instruction to add operand to.</param>
		/// <param name="memoryIndex">Index of memory slot this operand should point to.</param>
		/// <param name="register">Register containing index offset.</param>
		/// <returns>New operand instance.</returns>
		public static Operand DirectMemoryIndexedOperand(Instruction instruction, int memoryIndex, Register regiser)
		{
			Operand op = new Operand(instruction, OperandType.DirectMemoryIndexed);
			op.MemoryIndex = memoryIndex;
			op.OffsetRegister = regiser;
			return op;
		}

		/// <summary>
		///		Creates a new indirect memory operand.
		/// </summary>
		/// <param name="instruction">Instruction to add operand to.</param>
		/// <param name="register">Register containing memory slot this operand should point to.</param>
		/// <returns>New operand instance.</returns>
		public static Operand IndirectMemoryOperand(Instruction instruction, Register register)
		{
			Operand op = new Operand(instruction, OperandType.IndirectMemory);
			op.Register = register;
			return op;
		}

		/// <summary>
		///		Creates a new indirect memory indexed operand.
		/// </summary>
		/// <param name="instruction">Instruction to add operand to.</param>
		/// <param name="register">Register containing memory slot this operand should point to.</param>
		/// <param name="offsetRegister">Register containing index offset this operand should point to.</param>
		/// <returns>New operand instance.</returns>
		public static Operand IndirectMemoryIndexedOperand(Instruction instruction, Register register, Register offsetRegister)
		{
			Operand op = new Operand(instruction, OperandType.IndirectMemoryIndexed);
			op.Register = register;
			op.OffsetRegister = offsetRegister;
			return op;
		}

		/// <summary>
		///		Creates a new direct stack operand.
		/// </summary>
		/// <param name="instruction">Instruction to add operand to.</param>
		/// <param name="stackIndex">Index of stack slot this operand should point to.</param>
		/// <returns>New operand instance.</returns>
		public static Operand DirectStackOperand(Instruction instruction, int stackIndex)
		{
			Operand op = new Operand(instruction, OperandType.DirectStack);
			op.StackIndex = stackIndex;
			return op;
		}

		/// <summary>
		///		Creates a new direct stack index operand.
		/// </summary>
		/// <param name="instruction">Instruction to add operand to.</param>
		/// <param name="stackIndex">Index of stack slot this operand should point to.</param>
		/// <param name="register">Register containing index offset.</param>
		/// <returns>New operand instance.</returns>
		public static Operand DirectStackIndexedOperand(Instruction instruction, int stackIndex, Register regiser)
		{
			Operand op = new Operand(instruction, OperandType.DirectStackIndexed);
			op.StackIndex = stackIndex;
			op.OffsetRegister = regiser;
			return op;
		}

		/// <summary>
		///		Creates a new indirect stack operand.
		/// </summary>
		/// <param name="instruction">Instruction to add operand to.</param>
		/// <param name="register">Register containing stack slot this operand should point to.</param>
		/// <returns>New operand instance.</returns>
		public static Operand IndirectStackOperand(Instruction instruction, Register register)
		{
			Operand op = new Operand(instruction, OperandType.IndirectStack);
			op.Register = register;
			return op;
		}

		/// <summary>
		///		Creates a new indirect stack indexed operand.
		/// </summary>
		/// <param name="instruction">Instruction to add operand to.</param>
		/// <param name="register">Register containing stack slot this operand should point to.</param>
		/// <param name="offsetRegister">Register containing index offset this operand should point to.</param>
		/// <returns>New operand instance.</returns>
		public static Operand IndirectStackIndexedOperand(Instruction instruction, Register register, Register offsetRegister)
		{
			Operand op = new Operand(instruction, OperandType.IndirectStackIndexed);
			op.Register = register;
			op.OffsetRegister = offsetRegister;
			return op;
		}

		#endregion
	}

	/// <summary>
	///		Used to describe what a symbol class contains.
	/// </summary>
	public enum SymbolType
	{
		String,
		Function,
		Variable,
		State,
		Enumeration,
		JumpTarget,
		MetaData,
        Namespace
	}

	/// <summary>
	///		Used as a base for all symbol classes, a symbol can be anything
	///		that can be used in a script, and something that 'normally' has its
	///		own scope, for example a function or a state.
	/// </summary>
	public abstract class Symbol
	{
		#region Members
		#region Variables

		protected int _index;

		protected string _ident;
		protected Symbol _scope;

		protected ArrayList _instructionList = new ArrayList();
		protected ArrayList _symbolList = new ArrayList();

		#endregion
		#region Properties

		public abstract SymbolType Type { get; }

		/// <summary>
		///		Gets or sets the index of this symbol within its scope's list.
		/// </summary>
		public int Index
		{
			get { return _index; }
			set { _index = value; }
		}

		/// <summary>
		///		Gets or sets the indentifier used by this symbol.
		/// </summary>
		public string Identifier
		{
			get { return _ident; }
			set { _ident = value; }
		}

		/// <summary>
		///		Gets or sets the scope that this symbol is declared in.
		/// </summary>
		public Symbol Scope
		{
			get { return _scope; }
			set { _scope = value; }
		}

		/// <summary>
		///		Retrieves the array list containing all instructions
		///		bound to this symbol.
		/// </summary>
		public ArrayList Instructions
		{
			get { return _instructionList; }
			set { _instructionList = value; }
		}

		/// <summary>
		///		Retrieves the array list containing all symbols declared 
		///		within this symbols scope.
		/// </summary>
		public ArrayList Symbols
		{
			get { return _symbolList; }
		}

		#endregion
		#endregion
		#region Methods

        /// <summary>
        ///     Converts this object to a textural form and returns it.
        /// </summary>
        /// <returns>Textural form of this object.</returns>
        public override string ToString()
        {
            return _ident != null ? _ident : base.ToString();
        }

		/// <summary>
		///		Works just like the FindSymbol function except it checks a functions mask as well.
		/// </summary>
		/// <param name="ident">Identifier of function to find.</param>
		/// <param name="mask">Mask of function to find.</param>
		/// <returns>A function symbol with the same indentifier and mask as the one passed or null if one can't be found.</returns>
		public Symbol FindFunctionByMask(string ident, ArrayList mask)
		{
			foreach (Symbol symbol in _symbolList)
			{
				if (symbol.Type != SymbolType.Function || symbol.Identifier.ToLower() != ident.ToLower()) continue;
				FunctionSymbol funcSymbol = (FunctionSymbol)symbol;
				if (funcSymbol.ParameterCount != mask.Count) continue;

				int parameterIndex = 0;
				bool paramsValid = true;
				foreach (DataTypeValue value in mask)
					if (funcSymbol.CheckParameterTypeValid(parameterIndex++, value) == false)
						paramsValid = false;

				if (paramsValid == false) continue;

				return symbol;
			}
			return null;
		}

		/// <summary>
		///		Retrieves a symbol from this symbols scope by checking its identifier 
		///		with the given identifier.
		/// </summary>
		/// <param name="ident">Identifier to find symbol by.</param>
		/// <returns>A symbol with the same indentifier as the one passed or null if one can't be found.</returns>
		public Symbol FindSymbol(string ident)
		{
			foreach (Symbol symbol in _symbolList)
			{
				if (symbol.Identifier == null) continue;
				if (symbol.Identifier.ToLower() == ident.ToLower()) return symbol;
			}
			return null;
		}

		/// <summary>
		///		Retrieves a symbol from this symbols scope by checking its index
		///		with the given index.
		/// </summary>
		/// <param name="index">Identifier to find symbol by.</param>
		/// <returns>A symbol with the same index as the one passed or null if one can't be found.</returns>
		public Symbol FindSymbol(int index)
		{
			foreach (Symbol symbol in _symbolList)
				if (symbol.Index == index) return symbol;
			return null;
		}

		/// <summary>
		///		Retrieves a symbol from this symbols scope by checking its identifier 
		///		with the given identifier, and its type against the given type.
		/// </summary>
		/// <param name="ident">Identifier to find symbol by.</param>
		/// <param name="type">Type of symbol to find symbol by.</param>
		/// <returns>A symbol with the same indentifier as the one passed or null if one can't be found.</returns>
		public Symbol FindSymbol(string ident, SymbolType type)
		{
			foreach (Symbol symbol in _symbolList)
			{
				if (symbol.Identifier == null) continue;
				if (symbol.Identifier.ToLower() == ident.ToLower() && symbol.Type == type) return symbol;
			}
			return null;
		}

        /// <summary>
        ///		Retrieves a symbol from this symbols scope by checking its identifier 
        ///		with the given identifier, its type against the given type and its data
        ///     type against a given data type.
        /// </summary>
        /// <param name="ident">Identifier to find symbol by.</param>
        /// <param name="type">Type of symbol to find symbol by.</param>
        /// <param name="dataType"></param>
        /// <returns>A symbol with the same indentifier as the one passed or null if one can't be found.</returns>
        public Symbol FindVariableSymbol(string ident, DataTypeValue dataType)
        {
            foreach (Symbol symbol in _symbolList)
            {
                if (symbol.Identifier == null) continue;
                if (symbol.Identifier.ToLower() == ident.ToLower() && symbol.Type == SymbolType.Variable && ((VariableSymbol)symbol).DataType == dataType) return symbol;
            }
            return null;
        }

		/// <summary>
		///		Retrieves a symbol from this symbols scope by checking its index
		///		with the given index, and its type against the given type.
		/// </summary>
		/// <param name="index">Identifier to find symbol by.</param>
		/// <param name="type">Type of symbol to find symbol by.</param>
		/// <returns>A symbol with the same index as the one passed or null if one can't be found.</returns>
		public Symbol FindSymbol(int index, SymbolType type)
		{
			foreach (Symbol symbol in _symbolList)
				if (symbol.Index == index && symbol.Type == type) return symbol;
			return null;
		}

		/// <summary>
		///		Retrieves an list containing every symbol of the specified
		///		type in this symbols scope.
		/// </summary>
		/// <param name="type">Type of symbol to filter symbols by.</param>
		/// <returns></returns>
		public ArrayList FindSymbols(SymbolType type)
		{
			ArrayList list = new ArrayList();
			foreach (Symbol symbol in _symbolList)
				if (symbol.Type == type) list.Add(symbol);
			return list;
		}

		/// <summary>
		///		Adds an instruction to this symbols scope.
		/// </summary>
		/// <param name="instruction">Instruction to add to this symbols scope.</param>
		public void AddInstruction(Instruction instruction)
		{
			_instructionList.Add(instruction);
		}

		/// <summary>
		///		Adds a symbol to this symbols scope.
		/// </summary>
		/// <param name="symbol">Symbol to add to this symbols scope.</param>
		public void AddSymbol(Symbol symbol)
		{
			symbol.Index = _symbolList.Count;
			_symbolList.Add(symbol);
		}

		#endregion
	}

	/// <summary>
	///		Used by instruction operands to track a given instruction.
	/// </summary>
	public class JumpTargetSymbol : Symbol
	{
		#region Members
		#region Variables

		private int _instrIndex;

		#endregion
		#region Properties

		/// <summary>
		///		Returns an ID specifying what type of symbol this is.
		/// </summary>
		public override SymbolType Type
		{
			get { return SymbolType.JumpTarget; }
		}

		/// <summary>
		///		Gets or sets this jump target points to.
		/// </summary>
		public int InstrIndex
		{
			get { return _instrIndex; }
			set { _instrIndex = value; }
		}

		#endregion
		#endregion
		#region methods

		/// <summary>
		///		Binds this jump target to the last created instruction in this 
		///		symbols scope.
		/// </summary>
		public void Bind()
		{
			_instrIndex = _scope.Instructions.Count;
		}

		/// <summary>
		///		Initializes a new jump target instance and adds itself to the given
		///		scope's symbol list.
		/// </summary>
		/// <param name="scope">Scope that symbol is in.</param>
		public JumpTargetSymbol(Symbol scope)
		{
			_scope = scope;
			if (scope != null) _instrIndex = scope.Instructions.Count;
			if (scope != null) scope.AddSymbol(this);
		}

		#endregion
	}

	/// <summary>
	///		Used to describe the technical details (such as entry point / parameter count)
	///		of a script's function.
	/// </summary>
	public class FunctionSymbol : Symbol
	{
		#region Members
		#region Variables

		private bool _isThreadSpawner = false;
		private bool _isEvent = false;
        private bool _isConsole = false;
        private bool _isImport = false;
        private bool _isExport = false;

        private bool _isMember = false;

		private int _entryPoint;
		private int _localDataSize;
		private DataTypeValue _returnType = new DataTypeValue(DataType.Void, false, false);
        private SymbolAccessModifier _accessModifier = SymbolAccessModifier.Private;

		private int _parameterCount;

        private bool _isUsed = false;

        private NativeFunction _nativeFunction;

		#endregion
		#region Properties

		/// <summary>
		///		Returns an ID specifying what type of symbol this is.
		/// </summary>
		public override SymbolType Type
		{
			get { return SymbolType.Function; }
		}

        /// <summary>
        ///		Gets or sets if this function is ever used.
        /// </summary>
        public bool IsUsed
        {
            get { return _isUsed; }
            set { _isUsed = value; }
        }

		/// <summary>
		///		Gets or sets if this function spawns a new 
		///		thread when called.
		/// </summary>
		public bool IsThreadSpawner
		{
			get { return _isThreadSpawner; }
			set { _isThreadSpawner = value; }
		}

		/// <summary>
		///		Gets or sets if this function is declared
		///		as an event.
		/// </summary>
		public bool IsEvent
		{
			get { return _isEvent; }
			set { _isEvent = value; }
		}

        /// <summary>
        ///		Gets or sets if this function is declared
        ///		as a console.
        /// </summary>
        public bool IsConsole
        {
            get { return _isConsole; }
            set { _isConsole = value; }
        }

        /// <summary>
        ///		Gets or sets if this function is declared
        ///		as a member.
        /// </summary>
        public bool IsMember
        {
            get { return _isMember; }
            set { _isMember = value; }
        }

        /// <summary>
        ///		Gets or sets if this function is declared
        ///		as an import.
        /// </summary>
        public bool IsImport
        {
            get { return _isImport; }
            set { _isImport = value; }
        }

        /// <summary>
        ///		Gets or sets if this function is declared
        ///		as an export.
        /// </summary>
        public bool IsExport
        {
            get { return _isExport; }
            set { _isExport = value; }
        }

		/// <summary>
		///		Gets or sets the entry point of this function 
		///		in its scope's instruction list.
		/// </summary>
		public int EntryPoint
		{
			get { return _entryPoint; }
			set { _entryPoint = value; }
		}

		/// <summary>
		///		Get or sets the amount of parameters associated with this function.
		/// </summary>
		public int ParameterCount
		{
			get { return _parameterCount; }
			set { _parameterCount = value; }
		}

		/// <summary>
		///		Get or sets the size of the local data.
		/// </summary>
		public int LocalDataSize
		{
			get { return _localDataSize; }
			set { _localDataSize = value; }
		}

		/// <summary>
		///		Get or sets the data type this function returns.
		/// </summary>
		public DataTypeValue ReturnType
		{
			get { return _returnType; }
			set { _returnType = value; }
		}

        /// <summary>
        ///		Gets or sets the value determining how this variable can be accessed.
        /// </summary>
        public SymbolAccessModifier AccessModifier
        {
            get { return _accessModifier; }
            set { _accessModifier = value; }
        }

        /// <summary>
        ///     Gets or sets the native function this function refers to.
        /// </summary>
        public NativeFunction NativeFunction
        {
            get { return _nativeFunction; }
            set { _nativeFunction = value; }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Checks if a given parameter data type can be applied to this parameter.
 		/// </summary>
		/// <param name="index">Index of parameter to check.</param>
		/// <returns>Boolean describing if the data type is valid.</returns>
		public bool CheckParameterTypeValid(int index, DataTypeValue value)
		{
            if (index >= _symbolList.Count) return false;
			VariableSymbol symbol = _symbolList[index] as VariableSymbol;
            return (symbol.DataType == value || value.DataType == DataType.Null);//|| ScriptCompiler.CanImplicitlyCast(symbol.DataType, value));
		}

		/// <summary>
		///		Initializes a new instance of this class with the given identifier,
		///		and add's itself to the given scope's symbol list.
		/// </summary>
		/// <param name="ident">Identifier used to identify this symbol.</param>
		/// <param name="scope">Scope that symbol is in.</param>
		public FunctionSymbol(string ident, Symbol scope)
		{
			_ident = ident;
			_scope = scope;
			if (scope != null) scope.AddSymbol(this);
		}

        /// <summary>
        ///     Converts this instance to a textural form.
        /// </summary>
        /// <returns>Textural form of this instance.</returns>
        public override string ToString()
        {
            DataTypeValue[] parameterTypes = new DataTypeValue[_parameterCount];
            for (int i = 0; i < _parameterCount; i++)
                parameterTypes[i] = (_symbolList[i] as VariableSymbol).DataType;

            string fullName = _ident + "(";
            for (int i = 0; i < parameterTypes.Length; i++)
                fullName += parameterTypes[i] + (i < parameterTypes.Length - 1 ? "," : "");
            fullName += ")";

            return fullName;
        }

		#endregion
	}

	/// <summary>
	///		Describes a single string literal. Storing strings as symbol's
	///		has the advantage of meaning that duplicates get ignored, thus cutting
	///		down the size of the compile byte code.
	/// </summary>
	public class StringSymbol : Symbol
	{
		#region Members
		#region Variables

		#endregion
		#region Properties

		/// <summary>
		///		Returns an ID specifying what type of symbol this is.
		/// </summary>
		public override SymbolType Type
		{
			get { return SymbolType.String; }
		}

		#endregion
		#endregion
		#region methods

		/// <summary>
		///		Initializes a new instance of this class and adds itself to 
		///		the given scope's symbol list.
		/// </summary>
		/// <param name="scope">Scope that this symbol is in.</param>
		/// <param name="ident">Contents of this string.</param>
		public StringSymbol(Symbol scope, string ident)
		{
			_scope = scope;
			if (scope != null) scope.AddSymbol(this);
			_ident = ident;
		}

		#endregion
	}

	/// <summary>
	///		Describes a small chunk of meta data. 
	/// </summary>
	public class MetaDataSymbol : Symbol
	{
		#region Members
		#region Variables

		private string _value = "";

		#endregion
		#region Properties

		/// <summary>
		///		Returns an ID specifying what type of symbol this is.
		/// </summary>
		public override SymbolType Type
		{
			get { return SymbolType.MetaData; }
		}

		/// <summary>
		///		Gets or sets the value this ,eta data should store. 
		/// </summary>
		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}

		#endregion
		#endregion
		#region methods

		/// <summary>
		///		Initializes a new instance of this class and adds itself to 
		///		the given scope's symbol list.
		/// </summary>
		/// <param name="scope">Scope that this symbol is in.</param>
		/// <param name="ident">Name of this meta data.</param>
		/// <param name="value">Value of this meta data.</param>
		public MetaDataSymbol(Symbol scope, string ident, string value)
		{
			_scope = scope;
			if (scope != null) scope.AddSymbol(this);
			_ident = ident;
			_value = value;
		}

		#endregion
	}

	/// <summary>
	///		Used by the VariableSymbol class to describe what purpose 
	///		the variable has.
	/// </summary>
	public enum VariableType
	{
		Parameter,
		Constant,
		Local,
		Global,
        Member
	}

    /// <summary>
    ///     Used to determine how a symbol can be accessed.
    /// </summary>
    public enum SymbolAccessModifier
    {
        Public,
        Private,
        Protected
    }

	/// <summary>
	///		Describes a single variable symbol. Variable is used in a broad term, as it 
	///		encompasses parameters, constants, native variables, 
	/// </summary>
	public class VariableSymbol : Symbol
	{
		#region Members
		#region Variables

		private VariableType _variableType = VariableType.Local;
		private int _stackIndex, _memoryIndex;
		private bool _isArray = false, _isConst = false;
		private DataTypeValue _dataType;
		private bool _isUsed = false;

		private bool _isProperty = false;
        private SymbolAccessModifier _accessModifier = SymbolAccessModifier.Private;

		private Token _constToken = null;

		#endregion
		#region Properties

		/// <summary>
		///		Returns an ID specifying what type of symbol this is.
		/// </summary>
		public override SymbolType Type
		{
			get { return SymbolType.Variable; }
		}

		/// <summary>
		///		Gets or sets the token that contains the constant value of this variable.
		/// </summary>
		public Token ConstToken
		{
			get { return _constToken; }
			set { _constToken = value; }
		}

		/// <summary>
		///		Get or sets the variable type of this variable.
		/// </summary>
		public VariableType VariableType
		{
			get { return _variableType; }
			set { _variableType = value; }
		}

		/// <summary>
		///		Get or sets the stack index of this variable.
		/// </summary>
		public int StackIndex
		{
			get { return _stackIndex; }
			set { _stackIndex = value; }
		}

		/// <summary>
		///		Get or sets the memory index (used for global symbols) of this variable.
		/// </summary>
		public int MemoryIndex
		{
			get { return _memoryIndex; }
			set { _memoryIndex = value; }
		}

		/// <summary>
		///		Gets or sets if this variable is ever used.
		/// </summary>
		public bool IsUsed
		{
			get { return _isUsed; }
			set { _isUsed = value; }
		}

		/// <summary>
		///		Gets or sets if this variable is a property.
		/// </summary>
		public bool IsProperty
		{
			get { return _isProperty; }
			set { _isProperty = value; }
		}

		/// <summary>
		///		Gets or sets if this variable is an array.
		/// </summary>
		public bool IsArray
		{
			get { return _isArray; }
			set { _isArray = value; }
		}

		/// <summary>
		///		Gets or sets if this variable is a constant.
		/// </summary>
		public bool IsConstant
		{
			get { return _isConst; }
			set { _isConst = value; }
		}

        /// <summary>
        ///		Gets or sets the value determining how this variable can be accessed.
        /// </summary>
        public SymbolAccessModifier AccessModifier
        {
            get { return _accessModifier; }
            set { _accessModifier = value; }
        }

		/// <summary>
		///		Gets or sets if the data type of this variable.
		/// </summary>
		public DataTypeValue DataType
		{
			get { return _dataType; }
			set { _dataType = value; }
		}

		#endregion
		#endregion
		#region methods

		/// <summary>
		///		Initializes a new instance of this class and adds itself to 
		///		the given scope's symbol list.
		/// </summary>
		/// <param name="scope">Scope that this symbol is in.</param>
		public VariableSymbol(Symbol scope)
		{
			if (scope == null) return;
			_scope = scope;
			scope.AddSymbol(this);
		}

		#endregion
	}

	/// <summary>
	///		Describes a single state block symbol. 
	/// </summary>
	public class StateSymbol : Symbol
	{
		#region Members
		#region Variables

		private bool _isEngineDefault;
		private bool _isEditorDefault;

		#endregion
		#region Properties

		/// <summary>
		///		Sets or gets if this state should be the engines default one.
		/// </summary>
		public bool IsEngineDefault
		{
			get { return _isEngineDefault; }
			set { _isEngineDefault = value; }
		}

		/// <summary>
		///		Sets or gets if this state should be the editor default one.
		/// </summary>
		public bool IsEditorDefault
		{
			get { return _isEditorDefault; }
			set { _isEditorDefault = value; }
		}

		/// <summary>
		///		Returns an ID specifying what type of symbol this is.
		/// </summary>
		public override SymbolType Type
		{
			get { return SymbolType.State; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Initializes a new instance of this class and adds itself to 
		///		the given scope's symbol list.
		/// </summary>
		/// <param name="scope">Scope that this symbol is in.</param>
		public StateSymbol(Symbol scope)
		{
			_scope = scope;
			if (scope != null) scope.AddSymbol(this);
		}

		#endregion
	}

	/// <summary>
	///		Describes a single enumeration statement symbol.
	/// </summary>
	public class EnumerationSymbol : Symbol
	{
		#region Members
		#region Variables

		#endregion
		#region Properties

		/// <summary>
		///		Returns an ID specifying what type of symbol this is.
		/// </summary>
		public override SymbolType Type
		{
			get { return SymbolType.Enumeration; }
		}

		#endregion
		#endregion
		#region methods

		/// <summary>
		///		Initializes a new instance of this class and adds itself to 
		///		the given scope's symbol list.
		/// </summary>
		/// <param name="scope">Scope that this symbol is in.</param>
		public EnumerationSymbol(Symbol scope)
		{
			_scope = scope;
			if (scope != null) scope.AddSymbol(this);
		}

		#endregion
	}

    /// <summary>
    ///		Describes a single namespace statement symbol.
    /// </summary>
    public class NamespaceSymbol : Symbol
    {
        #region Members
        #region Variables

        #endregion
        #region Properties

        /// <summary>
        ///		Returns an ID specifying what type of symbol this is.
        /// </summary>
        public override SymbolType Type
        {
            get { return SymbolType.Namespace; }
        }

        #endregion
        #endregion
        #region methods

        /// <summary>
        ///		Initializes a new instance of this class and adds itself to 
        ///		the given scope's symbol list.
        /// </summary>
        /// <param name="scope">Scope that this symbol is in.</param>
        public NamespaceSymbol(Symbol scope)
        {
            _scope = scope;
            if (scope != null) scope.AddSymbol(this);
        }

        #endregion
    }

	/// <summary>
	///		Used to track the start and end jump targets of a loop.
	/// </summary>
	public class LoopTracker
	{
		#region Members
		#region Variables

		private JumpTargetSymbol _startJumpTarget;
		private JumpTargetSymbol _endJumpTarget;

		#endregion
		#region Properties

		/// <summary>
		///		Returns the start jump target this instance is tracking.
		/// </summary>
		public JumpTargetSymbol StartJumpTarget
		{
			get {  return _startJumpTarget; }
			set { _startJumpTarget = value; }
		}

		/// <summary>
		///		Returns the end jump target this instance is tracking.
		/// </summary>
		public JumpTargetSymbol EndJumpTarget
		{
			get {  return _endJumpTarget; }
			set { _endJumpTarget = value; }
		}

		#endregion
		#endregion
		#region methods

		/// <summary>
		///		Initializes a new instance of this class and sets it start
		///		and end jump targets as those given.
		/// </summary>
		/// <param name="start">Jump target pointing to the start of the loop.</param>
		/// <param name="end">Jump target pointing to the end of the loop.</param>
		public LoopTracker(JumpTargetSymbol start, JumpTargetSymbol end)
		{
			_startJumpTarget = start;
			_endJumpTarget = end;
		}

		#endregion
	}

	/// <summary>	
	///		Used to compile scripts into an optimized byte code format 
	///		compatible with the script VM.
	/// </summary>
	public sealed class ScriptCompiler
	{
		#region Members
		#region Variables

		private static object[] _defaultIncludes = null;
		private static Define[] _defaultDefines = null;
		private static CompileFlags _defaultFlags = 0;

		private ArrayList _errorList = new ArrayList();
		private ArrayList _tokenList = null;
		private int _tokenIndex = 0;

		private Token _currentToken;

		private Symbol _currentScope;
		private Symbol _globalScope, _memberScope;

		private int _currentPass;

		private int _memorySize;
		private int _internalVariableIndex;

		private CompileFlags _compileFlags;

		private ArrayList _compiledSymbolList = new ArrayList();
		private ArrayList _compiledInstructionList = new ArrayList();
		private ArrayList _compiledDebugFileList = new ArrayList();
		private ArrayList _compiledDefineList = new ArrayList();

		private ArrayList _metaDataList = new ArrayList();
		private Symbol _lastMetaDataSymbol = null;

		private StateSymbol _defaultEngineState, _defaultEditorState;
		private bool _errorsOccured = false;

		private Stack _loopTrackerStack = new Stack();

		private bool _insideIndexerLoop = true;
		private int _indexerLoopIndex = 0;

		private bool _inAtom = false;
		private bool _inLock = false;

        private Symbol _overrideInstructionScope = null;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the list of includes used if none are specified when a compile is requested.
		/// </summary>
		public static object[] DefaultIncludes
		{
			get { return _defaultIncludes; }
			set { _defaultIncludes = value; }
		}

		/// <summary>
		///		Gets or sets the list of defines used if none are specified when a compile is requested.
		/// </summary>
		public static Define[] DefaultDefines
		{
			get { return _defaultDefines; }
			set { _defaultDefines = value; }
		}

		/// <summary>
		///		Gets or sets the flag bitmask used if one is not specified when a compile is requested.
		/// </summary>
		public static CompileFlags DefaultFlags
		{
			get { return _defaultFlags; }
			set { _defaultFlags = value; }
		}

		/// <summary>
		///		Returns an array list containing CompilerError instances 
		///		describing each error that occured while compiling
		///		the last script.
		/// </summary>
		public ArrayList ErrorList
		{
			get { return _errorList; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Compiles a script from a string into byte code.
		/// </summary>
		/// <param name="data">Data of script to compile.</param>
		/// <param name="flags">Bitmask of flags defining how the script should be compiled.</param>
		/// <param name="defineList">A list of defines to use while preprocessing script.</param>
		/// <param name="includePaths">A list of directory paths to use when looking for include files.</param>
		/// <param name="fileUrl">Contains the url of the file this data comes from.</param>
		/// <returns>The number of errors or warning this script has generated duren compilation.</returns>
		public int CompileString(string data, CompileFlags flags, Define[] defineList, object[] includePaths, string fileUrl)
		{
#if !DEBUG
			try
			{
#endif
				// Reset all variables used to store compilation details.
				_errorList.Clear();
				_compiledSymbolList.Clear();
				_compiledInstructionList.Clear();
				_compiledDebugFileList.Clear();
				_compiledDefineList.Clear();
				_loopTrackerStack.Clear();
				_metaDataList.Clear();
				_currentToken = null;
				_currentPass = 0;
				_currentScope = null;
				_globalScope = new FunctionSymbol("$global", null); // Initialize global scope here.
                _memberScope = new FunctionSymbol("$member", null); // Initialize member scope here.
                _compileFlags = flags;
				_tokenList = null;
				_tokenIndex = 0;
				_memorySize = 2; // Reserve space for 'special' globals like 'this'.
				_internalVariableIndex = 0;
				_defaultEngineState = _defaultEditorState = null;
				_errorsOccured = false;
                _overrideInstructionScope = null;

				// Create the 'this' variable.
				VariableSymbol thisSymbol = new VariableSymbol(_globalScope);
				thisSymbol.DataType = new DataTypeValue(DataType.Object, false, false);
				thisSymbol.Identifier = "this";
				thisSymbol.IsConstant = true;
				thisSymbol.IsUsed = true;
				thisSymbol.MemoryIndex = 1;
				thisSymbol.VariableType = VariableType.Constant;

				// Create a lexer and convert script into a list
				// of tokens.
                DebugLogger.WriteLog("Preforming lexical analysis on script.");
				Lexer lexer = new Lexer();
				if (lexer.Analyse(data, _compileFlags, fileUrl) > 0)
				{
					foreach (CompileError error in lexer.ErrorList)
						_errorList.Add(error);
				}
				_tokenList = lexer.TokenList;

                // Add the script directory into the include path array.
                string includePath = Path.GetDirectoryName(fileUrl);
                string[] newIncludePaths = new string[includePaths.Length + 1];
                includePaths.CopyTo(newIncludePaths, 0);
                newIncludePaths[newIncludePaths.Length - 1] = includePath;
                includePaths = newIncludePaths;

				// Create a pre-processor and process the token list
                DebugLogger.WriteLog("Preforming preprocessing on script.");
				PreProcessor preProcessor = new PreProcessor();
				if (preProcessor.Process(_tokenList, _compileFlags, defineList, includePaths) > 0)
				{
					foreach (CompileError error in preProcessor.ErrorList)
						_errorList.Add(error);
				}
				_compiledDefineList = preProcessor.DefineList;
				_tokenList = preProcessor.TokenList;

				try
				{
					// Pass 0: Collect infomation. 
					// Pass 1: Generate byte code.
                    DebugLogger.WriteLog("Compiling script in 2 passes.");

					// Go over the source code in 2 passes.
					for (int pass = 0; pass < 2; pass++)
					{
						_currentPass = pass;
						_currentToken = null;
						_tokenIndex = 0;
						_currentScope = _globalScope;
						_internalVariableIndex = 0; // This needs to be reset so statements using 
						// an internal variables can find them on the second pass.
						while (!EndOfTokenStream())
						{
							try
							{
								ParseStatement();
							}
							catch (CompilePanicModeException)
							{
                                DebugLogger.WriteLog("Panic mode initialized in script.");
								// Don't do anything here, just allow the error to be
								// forgoten and carry on as normal.
							}

							// If there are any errors quit compilation now.
                            if (_errorsOccured == true)
                                throw new CompileBreakException();
						}
					}

					// Yell at user if no default state has been declared.
					if (_defaultEngineState == null && (_compileFlags & CompileFlags.Library) == 0)
						Error(ErrorCode.MissingDefaultState, "Default engine state is missing.");

				}
				catch (CompileBreakException)
				{
                    DebugLogger.WriteLog("Script compilation broken via CompileBreakException.");
				}

                // Check what errors occured.
                int fatalErrorCount = 0;
                int errorCount = 0;
                int warningCount = 0;
                int messageCount = 0;
                foreach (CompileError error in _errorList)
                {
                    switch (error.AlertLevel)
                    {
                        case ErrorAlertLevel.Error: errorCount++; break;
                        case ErrorAlertLevel.FatalError: fatalErrorCount++; break;
                        case ErrorAlertLevel.Message: messageCount++; break;
                        case ErrorAlertLevel.Warning: warningCount++; break;
                    }
                    DebugLogger.WriteLog(error.ToString(), LogAlertLevel.Warning);
                }

                DebugLogger.WriteLog("Script compiled with "+fatalErrorCount+" fatal errors, "+errorCount+" errors, "+warningCount+" warnings and "+messageCount+" messages.");    

				// If there are any errors quit compilation now.
                if (_errorsOccured == true) return _errorList.Count;

				// Append an exit symbol onto the global scopes instruction list.
				CreateInstruction(OpCode.EXIT, _globalScope, _currentToken);

                DebugLogger.WriteLog("Optimizing and tweeking symbols and instructions.");

				// Compile symbol list.
				CompileSymbol(_globalScope);
                CompileSymbol(_memberScope);
			
                // Go through instruction list and replace placeholders with their values.
				int symbolIndex = 0;
				foreach (Symbol symbol in _compiledSymbolList)
				{
					symbol.Index = symbolIndex;
					symbolIndex++;

					// Update the entry point if this is a function or event.
					switch (symbol.Type)
					{
						case SymbolType.Function:
							((FunctionSymbol)symbol).EntryPoint = _compiledInstructionList.Count;
							break;
						case SymbolType.Variable:
							bool check = true;
							if (symbol.Scope.Type == SymbolType.Function)
                                if (((FunctionSymbol)symbol.Scope).IsImport == true) check = false;

                            // Should we remove it rather than warning?
							if (((VariableSymbol)symbol).IsUsed == false && ((VariableSymbol)symbol).IsConstant == false && check == true)
								Warning(ErrorCode.UnusedVariable, "Variable \"" + symbol.Identifier + "\" is declared but never used.");
							break;
					}

					foreach (Instruction instruction in symbol.Instructions)
					{
						// Create a new debug file entry if this instruction 
						// was generated in a previously unknown file.
						if (instruction.File != null && instruction.File != "")
						{
							bool found = false;
							foreach (string file in _compiledDebugFileList)
								if (file == instruction.File) found = true;
							if (found == false) _compiledDebugFileList.Add(instruction.File);
						}

						// Go through each operand attach to this instruction
						// and check for any trackers.
						foreach (Operand operand in instruction.Operands)
						{
							if (operand == null) continue;

							// Update operand based on type.
							switch (operand.OpType)
							{
								case OperandType.JumpTarget:
									operand.OpType = OperandType.InstrIndex;
									switch (symbol.Type)
									{
										case SymbolType.Function:
											operand.InstrIndex = ((FunctionSymbol)symbol).EntryPoint + operand.JumpTarget.InstrIndex;
											break;
									}
									break;
								case OperandType.SymbolIndexTracker:
									operand.OpType = OperandType.SymbolIndex;
									operand.SymbolIndex = _compiledSymbolList.IndexOf(operand.SymbolIndexTracker);
                                    break;
							}
						}
						_compiledInstructionList.Add(instruction);
					}
				}

				// Optimize this symbols instructions. (Currently somewhat error prone)
                int instructionCount = _compiledInstructionList.Count;
				ScriptOptimizer optimizer = new ScriptOptimizer();
				optimizer.Optimize(_compiledInstructionList, _compiledSymbolList);
				_compiledInstructionList = optimizer.OptimizedInstructions;
				_compiledSymbolList = optimizer.OptimizedSymbols;

                // int index = 0;
                //foreach (Instruction instr in _compiledInstructionList)
                //{
                //    System.Console.WriteLine("\t"+instr.Decompile());
                //    index++;
                //}

#if !DEBUG
			}
			catch (Exception)
			{
				_errorList.Add(new CompileError(ErrorCode.InternalError, "An internal compiler error occured.", ErrorAlertLevel.FatalError, 0, 0, ""));
			}
#endif
			return _errorList.Count;
		}
		public int CompileString(string data, CompileFlags flags, Define[] defineList, string fileUrl)
		{
			return CompileString(data, flags, defineList, _defaultIncludes, fileUrl);
		}
		public int CompileString(string data, CompileFlags flags, string fileUrl)
		{
			return CompileString(data, flags, _defaultDefines, _defaultIncludes, fileUrl);
		}
		public int CompileString(string data, string fileUrl)
		{
			return CompileString(data, _defaultFlags, _defaultDefines, _defaultIncludes, fileUrl);
		}

		/// <summary>
		///		Compiles a script from a url into byte code.
		/// </summary>
		/// <param name="url">Url of script to compile.</param>
		/// <param name="flags">Bitmask of flags defining how the script should be compiled.</param>
		/// <param name="defineList">A list of defines to use while preprocessing script.</param>
		/// <param name="includePaths">A list of directory paths to use when looking for include files.</param>
		/// <returns>The number of errors or warning this script has generated duren compilation.</returns>
		public int Compile(object url, CompileFlags flags, Define[] defineList, object[] includePaths)
		{
			// Open a stream so we can read in the script.
			Stream stream = StreamFactory.RequestStream(url, StreamMode.Open);
			if (stream == null) return 0;
			StreamReader reader = new StreamReader(stream);

			// Read in the whole text
            DebugLogger.WriteLog("Loading script into compiler from "+url.ToString()+".");
			CompileString(reader.ReadToEnd().Trim(), flags, defineList, includePaths, url.ToString());

			// Clean up the open stream.
			stream.Close();

			return _errorList.Count;
		}
		public int Compile(object url, CompileFlags flags, Define[] defineList)
		{
			return Compile(url, flags, defineList, _defaultIncludes);
		}
		public int Compile(object url, CompileFlags flags)
		{
			return Compile(url, flags, _defaultDefines, _defaultIncludes);
		}
		public int Compile(object url)
		{
			return Compile(url, _defaultFlags, _defaultDefines, _defaultIncludes);
		}

		/// <summary>
		///		Adds the given symbol and any sub symbols 
		///		into the compiled symbol list.
		/// </summary>
		private void CompileSymbol(Symbol symbol)
		{
			_compiledSymbolList.Add(symbol);
            foreach (Symbol subSymbol in symbol.Symbols)
            {
                // Jump target?
                if (subSymbol.Type == SymbolType.JumpTarget) continue;

                // Dpn't compile unused import functions.
                if (subSymbol.Type == SymbolType.Function && ((FunctionSymbol)subSymbol).IsImport == true && ((FunctionSymbol)subSymbol).IsUsed == false) continue;

                CompileSymbol(subSymbol);
            }
        }

		/// <summary>
		///		Dumps the compiled code as an object file to a given file.
		/// </summary>
		/// <param name="url">Url of file to dump byte code into.</param>
		public void DumpExecutableFile(object url)
		{
			// Open a stream so we can write in the script.
			Stream stream = StreamFactory.RequestStream(url, StreamMode.Truncate);
			if (stream == null) return;
			BinaryWriter writer = new BinaryWriter(stream);

            DebugLogger.WriteLog("Dumping script byte code to "+url.ToString()+".");

			// Dump it into the stream.
			DumpExecutableFile(writer);

            DebugLogger.WriteLog("Script byte code dumped.");

			// Close the stream.
			stream.Close();
			writer.Close();
		}

		/// <summary>
		///		Dumps the compiled code as an object file to a given file.
		/// </summary>
		/// <param name="writer">Binary writer to dump byte code into.</param>
		public void DumpExecutableFile(BinaryWriter writer)
		{
			// Write in data.
			writer.Write(new byte[3] { (byte)'C', (byte)'R', (byte)'X' });
			writer.Write((int)_compileFlags);
			writer.Write(_internalVariableIndex);
			writer.Write(_memorySize);
			writer.Write(_globalScope.Index);
            writer.Write(_memberScope.Index);
			writer.Write(_defaultEngineState == null ? -1 : _defaultEngineState.Index);
			writer.Write(_defaultEditorState == null ? -1 : _defaultEditorState.Index);

			writer.Write(_compiledDefineList.Count);
			writer.Write(_compiledSymbolList.Count);
			writer.Write(_compiledInstructionList.Count);

			// Write out debug file list.
			if ((_compileFlags & CompileFlags.Debug) != 0)
			{
				writer.Write(_compiledDebugFileList.Count);
				foreach (string file in _compiledDebugFileList)
					writer.Write(file);
			}

			// Write out the define list.
			foreach (Define define in _compiledDefineList)
			{
				writer.Write(define.Ident);
				writer.Write((int)define.ValueID);
				writer.Write(define.Value);
			}

			// Write our symbol table.
			foreach (Symbol symbol in _compiledSymbolList)
			{
				writer.Write((byte)symbol.Type);
				writer.Write(symbol.Identifier == null ? "" : symbol.Identifier);

				if (symbol.Scope != null)
					writer.Write(symbol.Scope.Index);
				else
					writer.Write((int)-1);

				switch (symbol.Type)
				{
                    case SymbolType.Namespace:
                        break;

                    case SymbolType.Enumeration:
						break;

					case SymbolType.String:
						break;

                    // These are never compiled to the symbol list.
					//case SymbolType.JumpTarget:
					//	break;

					case SymbolType.Function:
						writer.Write(((FunctionSymbol)symbol).EntryPoint);
						writer.Write((short)((FunctionSymbol)symbol).LocalDataSize);
						writer.Write(((FunctionSymbol)symbol).IsEvent);
                        writer.Write(((FunctionSymbol)symbol).IsConsole);
                        writer.Write(((FunctionSymbol)symbol).IsExport);
                        writer.Write(((FunctionSymbol)symbol).IsImport);
						writer.Write(((FunctionSymbol)symbol).IsThreadSpawner);
                        writer.Write(((FunctionSymbol)symbol).IsMember);
						writer.Write((byte)((FunctionSymbol)symbol).ParameterCount);
						writer.Write(((FunctionSymbol)symbol).ReturnType.IsArray);
						writer.Write(((FunctionSymbol)symbol).ReturnType.IsReference);
						writer.Write((byte)((FunctionSymbol)symbol).ReturnType.DataType);
                        writer.Write((byte)((FunctionSymbol)symbol).AccessModifier); 
						break;

					case SymbolType.State:
						writer.Write(((StateSymbol)symbol).IsEngineDefault);
						writer.Write(((StateSymbol)symbol).IsEditorDefault);
						break;

					case SymbolType.Variable:
						writer.Write((byte)((VariableSymbol)symbol).DataType.DataType);
						writer.Write(((VariableSymbol)symbol).DataType.IsReference);
						writer.Write(((VariableSymbol)symbol).IsArray);
						writer.Write(((VariableSymbol)symbol).IsConstant);
						writer.Write(((VariableSymbol)symbol).MemoryIndex);
						writer.Write(((VariableSymbol)symbol).StackIndex);
						writer.Write((byte)((VariableSymbol)symbol).VariableType);
						writer.Write(((VariableSymbol)symbol).IsProperty);
                        writer.Write((byte)((VariableSymbol)symbol).AccessModifier); 
                        writer.Write(((VariableSymbol)symbol).ConstToken == null ? "" : ((VariableSymbol)symbol).ConstToken.Ident);
						break;

					case SymbolType.MetaData:
						writer.Write(((MetaDataSymbol)symbol).Value);
						break;
				}
			}

			// Write out instruction table.
			foreach (Instruction instr in _compiledInstructionList)
			{
				writer.Write((byte)instr.OpCode);
				writer.Write((byte)instr.OperandCount);

				if ((_compileFlags & CompileFlags.Debug) != 0)
				{
					// Write the index into the debug file list that 
					// coresponds to the file this instruction was extracted from.
					int index = 0;
					if (instr.File == "" || instr.File == null)
						writer.Write((SByte)(-1));
					else
					{
						foreach (string file in _compiledDebugFileList)
							if (file == instr.File)
							{
								writer.Write((SByte)index);
								break;
							}
							else
								index++;
					}

					writer.Write((short)instr.Offset);
					writer.Write((short)instr.Line);
				}

				// Write out operand table.
				foreach (Operand operand in instr.Operands)
				{
					if (operand == null) continue;

					writer.Write((int)operand.OpType);
					switch (operand.OpType)
					{
						case OperandType.BooleanLiteral:
							writer.Write(operand.BooleanLiteral);
							break;
						case OperandType.ByteLiteral:
							writer.Write(operand.ByteLiteral);
							break;
						case OperandType.DirectMemory:
							writer.Write(operand.MemoryIndex);
							break;
						case OperandType.DirectMemoryIndexed:
							writer.Write(operand.MemoryIndex);
							writer.Write((byte)operand.OffsetRegister);
							break;
						case OperandType.DirectStack:
							writer.Write(operand.StackIndex);
							break;
						case OperandType.DirectStackIndexed:
							writer.Write(operand.StackIndex);
							writer.Write((byte)operand.OffsetRegister);
							break;
						case OperandType.DoubleLiteral:
							writer.Write(operand.DoubleLiteral);
							break;
						case OperandType.FloatLiteral:
							writer.Write(operand.FloatLiteral);
							break;
						case OperandType.IndirectMemory:
							writer.Write((byte)operand.Register);
							break;
						case OperandType.IndirectMemoryIndexed:
							writer.Write((byte)operand.Register);
							writer.Write((byte)operand.OffsetRegister);
							break;
						case OperandType.IndirectStack:
							writer.Write((byte)operand.Register);
							break;
						case OperandType.IndirectStackIndexed:
							writer.Write((byte)operand.Register);
							writer.Write((byte)operand.OffsetRegister);
							break;
						case OperandType.InstrIndex:
							writer.Write(operand.InstrIndex);
							break;
						case OperandType.IntegerLiteral:
							writer.Write(operand.IntegerLiteral);
							break;
						case OperandType.LongLiteral:
							writer.Write(operand.LongLiteral);
							break;
						case OperandType.Register:
							writer.Write((byte)operand.Register);
							break;
						case OperandType.ShortLiteral:
							writer.Write(operand.ShortLiteral);
							break;
						case OperandType.SymbolIndex:
							writer.Write(operand.SymbolIndex);
							break;
					}
				}
			}
		}

		/// <summary>
		///		Converts a keyword token to a data type.
		/// </summary>
		/// <param name="token">Token to convert.</param>
		/// <returns>DataType representing keyword.</returns>
		private DataType DataTypeFromKeywordToken(TokenID token)
		{
			switch (token)
			{
				case TokenID.KeywordBool: return DataType.Bool;
				case TokenID.KeywordByte: return DataType.Byte;
				case TokenID.KeywordDouble: return DataType.Double;
				case TokenID.KeywordFloat: return DataType.Float;
				case TokenID.KeywordInt: return DataType.Int;
				case TokenID.KeywordLong: return DataType.Long;
				case TokenID.KeywordShort: return DataType.Short;
				case TokenID.KeywordString: return DataType.String;
				case TokenID.KeywordObject: return DataType.Object;
				case TokenID.KeywordVoid: return DataType.Void;
				default: return DataType.Invalid;
			}
		}

		/// <summary>
		///		Converts a type token to a data type.
		/// </summary>
		/// <param name="token">Token to convert.</param>
		/// <returns>DataType representing type.</returns>
		private DataType DataTypeFromTypeToken(TokenID token)
		{
			switch (token)
			{
				case TokenID.TypeBoolean: return DataType.Bool;
				case TokenID.TypeByte: return DataType.Byte;
				case TokenID.TypeDouble: return DataType.Double;
				case TokenID.TypeFloat: return DataType.Float;
				case TokenID.TypeInteger: return DataType.Int;
				case TokenID.TypeLong: return DataType.Long;
				case TokenID.TypeShort: return DataType.Short;
				case TokenID.TypeString: return DataType.String;
				case TokenID.TypeVoid: return DataType.Void;
				default: return DataType.Invalid;
			}
		}

        /// <summary>
        ///     Creates a new byte code instruction.
        /// </summary>
        /// <param name="opcode">Opcode of instruction.</param>
        /// <param name="scope">Scope that instruction is in.</param>
        /// <param name="token">Token with line / offset data of instruction.</param>
        /// <returns>A new byte code instruction.</returns>
        private Instruction CreateInstruction(OpCode opcode, Symbol scope, Token token)
        {
            return new Instruction(opcode, _overrideInstructionScope != null ? _overrideInstructionScope : scope, token);
        }

		/// <summary>
		///		Creates an error and adds it to the error list.
		/// </summary>
		/// <param name="errorCode">Code ID of error.</param>
		/// <param name="errorMsg">Description of error.</param>
		private void Error(ErrorCode errorCode, string errorMsg, bool fatal, int onlyOnPass)
		{
			if (_currentPass != onlyOnPass && onlyOnPass != -1) return; // Only emit errors on the second pass.
            CompileError error = new CompileError(errorCode, errorMsg, fatal ? ErrorAlertLevel.FatalError : ErrorAlertLevel.Error, _currentToken != null ? _currentToken.Line : 0, _currentToken != null ? _currentToken.Offset : 0, _currentToken != null ? _currentToken.File : "");
			_errorList.Add(error);
			_errorsOccured = true;
			if (fatal == true)
				throw new CompileBreakException();
			else
				ErrorPanicMode();
		}
		private void Error(ErrorCode errorCode, string errorMsg, bool fatal)
		{
			Error(errorCode, errorMsg, fatal, 1);
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
			while (EndOfTokenStream() == false && LookAheadToken().ID != TokenID.CharCloseBrace &&
				   LookAheadToken().ID != TokenID.CharSemiColon)
			{
				NextToken();
			}
			throw new CompilePanicModeException();
		}

		/// <summary>
		///		Creates an warning and adds it to the error list.
		/// </summary>
		/// <param name="errorCode">Code ID of warning.</param>
		/// <param name="errorMsg">Description of warning.</param>
		private void Warning(ErrorCode errorCode, string errorMsg, int onlyOnPass)
		{
			if (_currentPass != onlyOnPass) return; // Only emit errors on the second pass.
			CompileError error = new CompileError(errorCode, errorMsg, ErrorAlertLevel.Warning, _currentToken.Line, _currentToken.Offset, _currentToken.File);
			_errorList.Add(error);
			if ((_compileFlags & CompileFlags.TreatWarningsAsErrors) != 0) throw new CompileBreakException();
		}
		private void Warning(ErrorCode errorCode, string errorMsg)
		{
			Warning(errorCode, errorMsg, 1);
		}

		/// <summary>
		///		Creates an message and adds it to the error list.
		/// </summary>
		/// <param name="errorCode">Code ID of error.</param>
		/// <param name="errorMsg">Description of message.</param>
		private void Message(ErrorCode errorCode, string errorMsg, int onlyOnPass)
		{
			if (_currentPass != onlyOnPass) return; // Only emit errors on the second pass.
			CompileError error = new CompileError(errorCode, errorMsg, ErrorAlertLevel.Message, _currentToken.Line, _currentToken.Offset, _currentToken.File);
			_errorList.Add(error);
			if ((_compileFlags & CompileFlags.TreatMessagesAsErrors) != 0) throw new CompileBreakException();
		}
		private void Message(ErrorCode errorCode, string errorMsg)
		{
			Message(errorCode, errorMsg, 1);
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
		///		Returns the token in 'index' places in the script but dosen't advance the stream.
		/// </summary>
		/// <param name="index">How far ahead to look for token.</param>
		/// <returns>The token in 'index' places.</returns>
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
			if (LAToken == null) Error(ErrorCode.ExpectingToken, "Expecting look-ahead token but encountered end of file.", true, -1);
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
				Error(ErrorCode.ExpectingToken, "Expecting \"" + Token.IdentFromID(id) + "\" token.", false, 0);
		}

		/// <summary>
		///		Checks if 2 data types can be used together without the need
		///		for manual casting.
		/// </summary>
		/// <param name="dest">Destination data type.</param>
		/// <param name="src">Source data type.</param>
		/// <returns>True if types can be used together.</returns>
        private bool CanImplicitlyCast(DataTypeValue dest, DataTypeValue src)
		{
			if (_currentPass == 0) return true; // Assume everything can cast when in the first pass.
			if ((dest.IsArray != src.IsArray || dest.IsReference != src.IsReference) && src.DataType != DataType.Null) return false;
			if (dest == src || dest.DataType == DataType.Null || src.DataType == DataType.Null) return true;
			switch (dest.DataType)
			{
				case DataType.Bool:
                    if (src.DataType == DataType.Bool || src.DataType == DataType.Byte ||
                        src.DataType == DataType.Double || src.DataType == DataType.Float ||
                        src.DataType == DataType.Int || src.DataType == DataType.Long ||
                        src.DataType == DataType.Short || src.DataType == DataType.String) return true;
					break;
				case DataType.Byte:
                    if (src.DataType == DataType.Bool || src.DataType == DataType.Byte ||
                        src.DataType == DataType.Double || src.DataType == DataType.Float ||
                        src.DataType == DataType.Int || src.DataType == DataType.Long ||
                        src.DataType == DataType.Short || src.DataType == DataType.String) return true;
					break;
				case DataType.Float:
                    if (src.DataType == DataType.Bool || src.DataType == DataType.Byte ||
                        src.DataType == DataType.Double || src.DataType == DataType.Float ||
                        src.DataType == DataType.Int || src.DataType == DataType.Long ||
                        src.DataType == DataType.Short || src.DataType == DataType.String) return true;
					break;
                case DataType.Double:
                    if (src.DataType == DataType.Bool || src.DataType == DataType.Byte ||
                        src.DataType == DataType.Double || src.DataType == DataType.Float ||
                        src.DataType == DataType.Int || src.DataType == DataType.Long ||
                        src.DataType == DataType.Short || src.DataType == DataType.String) return true;
                    break;
				case DataType.Int:
                    if (src.DataType == DataType.Bool || src.DataType == DataType.Byte ||
                        src.DataType == DataType.Double || src.DataType == DataType.Float ||
                        src.DataType == DataType.Int || src.DataType == DataType.Long ||
                        src.DataType == DataType.Short || src.DataType == DataType.String) return true;
					break;
				case DataType.Long:
                    if (src.DataType == DataType.Bool || src.DataType == DataType.Byte ||
                        src.DataType == DataType.Double || src.DataType == DataType.Float ||
                        src.DataType == DataType.Int || src.DataType == DataType.Long ||
                        src.DataType == DataType.Short || src.DataType == DataType.String) return true;
					break;
				case DataType.Short:
                    if (src.DataType == DataType.Bool || src.DataType == DataType.Byte ||
                        src.DataType == DataType.Double || src.DataType == DataType.Float ||
                        src.DataType == DataType.Int || src.DataType == DataType.Long ||
                        src.DataType == DataType.Short || src.DataType == DataType.String) return true;
					break;
                case DataType.String:
                    if (src.DataType == DataType.Bool || src.DataType == DataType.Byte ||
                        src.DataType == DataType.Double || src.DataType == DataType.Float ||
                        src.DataType == DataType.Int || src.DataType == DataType.Long ||
                        src.DataType == DataType.Short || src.DataType == DataType.String) return true;
                    break;

				// These type's can't be converted to anything but themselfs.
				case DataType.Object:
					break;
				case DataType.Void:
					break;
			}
			return false;
		}

		/// <summary>
		///		Chooses the correct byte code based on the data type.
		/// </summary>
		/// <param name="type">Type to find operation code from.</param>
		/// <returns>The correct opcode based on the data type.</returns>
        private OpCode OpCodeByType(DataTypeValue type, OpCodeType opCodeType)
		{
			switch (opCodeType)
			{
				case OpCodeType.ALLOCATE_HEAP:
					switch (type.DataType)
					{
						case DataType.Null: return OpCode.ALLOCATE_HEAP_NULL;
						case DataType.Bool: return OpCode.ALLOCATE_HEAP_BOOL;
						case DataType.Byte: return OpCode.ALLOCATE_HEAP_BYTE;
						case DataType.Double: return OpCode.ALLOCATE_HEAP_DOUBLE;
						case DataType.Object: return OpCode.ALLOCATE_HEAP_Object;
						case DataType.Float: return OpCode.ALLOCATE_HEAP_FLOAT;
						case DataType.Int: return OpCode.ALLOCATE_HEAP_INT;
						case DataType.Long: return OpCode.ALLOCATE_HEAP_LONG;
						case DataType.Short: return OpCode.ALLOCATE_HEAP_SHORT;
						case DataType.String: return OpCode.ALLOCATE_HEAP_STRING;
					}
					break;
				case OpCodeType.CAST:
					switch (type.DataType)
					{
						case DataType.Null: return OpCode.CAST_NULL;
						case DataType.Bool: return OpCode.CAST_BOOL;
						case DataType.Byte: return OpCode.CAST_BYTE;
						case DataType.Double: return OpCode.CAST_DOUBLE;
						case DataType.Object: return OpCode.CAST_Object;
						case DataType.Float: return OpCode.CAST_FLOAT;
						case DataType.Int: return OpCode.CAST_INT;
						case DataType.Long: return OpCode.CAST_LONG;
						case DataType.Short: return OpCode.CAST_SHORT;
						case DataType.String: return OpCode.CAST_STRING;
					}
					break;
				case OpCodeType.CMP:
					
					// If its an array or reference we need to compare the 
					// memory index not the value.
					if (type.IsArray == true || type.IsReference == true)
					{
						return OpCode.CMP_MEMORY_INDEX;
					}

					switch (type.DataType)
					{
						case DataType.Null: return OpCode.CMP_NULL;
						case DataType.Bool: return OpCode.CMP_BOOL;
						case DataType.Byte: return OpCode.CMP_BYTE;
						case DataType.Double: return OpCode.CMP_DOUBLE;
						case DataType.Object: return OpCode.CMP_Object;
						case DataType.Float: return OpCode.CMP_FLOAT;
						case DataType.Int: return OpCode.CMP_INT;
						case DataType.Long: return OpCode.CMP_LONG;
						case DataType.Short: return OpCode.CMP_SHORT;
						case DataType.String: return OpCode.CMP_STRING;
					}
					break;
				case OpCodeType.PUSH:
					
					// If its an array or reference we need to push the 
					// memory index not the value.
					if (type.IsArray == true || type.IsReference == true)
					{
						return OpCode.PUSH_MEMORY_INDEX;
					}

					switch (type.DataType)
					{
						case DataType.Null: return OpCode.PUSH_NULL;
						case DataType.Bool: return OpCode.PUSH_BOOL;
						case DataType.Byte: return OpCode.PUSH_BYTE;
						case DataType.Double: return OpCode.PUSH_DOUBLE;
						case DataType.Object: return OpCode.PUSH_OBJECT;
						case DataType.Float: return OpCode.PUSH_FLOAT;
						case DataType.Int: return OpCode.PUSH_INT;
						case DataType.Long: return OpCode.PUSH_LONG;
						case DataType.Short: return OpCode.PUSH_SHORT;
						case DataType.String: return OpCode.PUSH_STRING;
					}
					break;
				case OpCodeType.POP:

					// If its an array or reference we need to pop the 
					// memory index not the value.
					if (type.IsArray == true || type.IsReference == true)
					{
						return OpCode.POP_MEMORY_INDEX;
					}

					switch (type.DataType)
					{
						case DataType.Null: return OpCode.POP_NULL;
						case DataType.Bool: return OpCode.POP_BOOL;
						case DataType.Byte: return OpCode.POP_BYTE;
						case DataType.Double: return OpCode.POP_DOUBLE;
						case DataType.Object: return OpCode.POP_OBJECT;
						case DataType.Float: return OpCode.POP_FLOAT;
						case DataType.Int: return OpCode.POP_INT;
						case DataType.Long: return OpCode.POP_LONG;
						case DataType.Short: return OpCode.POP_SHORT;
						case DataType.String: return OpCode.POP_STRING;
					}
					break;
				case OpCodeType.MOV:

					// If its an array or reference we need to move the 
					// memory index not the value.
					if (type.IsArray == true || type.IsReference == true)
					{
						return OpCode.MOV_MEMORY_INDEX;
					}

					switch (type.DataType)
					{
						case DataType.Bool: return OpCode.MOV_BOOL;
						case DataType.Byte: return OpCode.MOV_BYTE;
						case DataType.Double: return OpCode.MOV_DOUBLE;
						case DataType.Object: return OpCode.MOV_OBJECT;
						case DataType.Null: return OpCode.MOV_NULL;
						case DataType.Float: return OpCode.MOV_FLOAT;
						case DataType.Int: return OpCode.MOV_INT;
						case DataType.Long: return OpCode.MOV_LONG;
						case DataType.Short: return OpCode.MOV_SHORT;
						case DataType.String: return OpCode.MOV_STRING;
					}
					break;
				case OpCodeType.MUL:
					switch (type.DataType)
					{
						case DataType.Byte: return OpCode.MUL_BYTE;
						case DataType.Double: return OpCode.MUL_DOUBLE;
						case DataType.Float: return OpCode.MUL_FLOAT;
						case DataType.Int: return OpCode.MUL_INT;
						case DataType.Long: return OpCode.MUL_LONG;
						case DataType.Short: return OpCode.MUL_SHORT;
					}
					break;
				case OpCodeType.DIV:
					switch (type.DataType)
					{
						case DataType.Byte: return OpCode.DIV_BYTE;
						case DataType.Double: return OpCode.DIV_DOUBLE;
						case DataType.Float: return OpCode.DIV_FLOAT;
						case DataType.Int: return OpCode.DIV_INT;
						case DataType.Long: return OpCode.DIV_LONG;
						case DataType.Short: return OpCode.DIV_SHORT;
					}
					break;
				case OpCodeType.ADD:
					switch (type.DataType)
					{
						case DataType.Byte: return OpCode.ADD_BYTE;
						case DataType.Double: return OpCode.ADD_DOUBLE;
						case DataType.Float: return OpCode.ADD_FLOAT;
						case DataType.Int: return OpCode.ADD_INT;
						case DataType.Long: return OpCode.ADD_LONG;
						case DataType.Short: return OpCode.ADD_SHORT;
						case DataType.String: return OpCode.ADD_STRING;
					}
					break;
				case OpCodeType.SUB:
					switch (type.DataType)
					{
						case DataType.Byte: return OpCode.SUB_BYTE;
						case DataType.Double: return OpCode.SUB_DOUBLE;
						case DataType.Float: return OpCode.SUB_FLOAT;
						case DataType.Int: return OpCode.SUB_INT;
						case DataType.Long: return OpCode.SUB_LONG;
						case DataType.Short: return OpCode.SUB_SHORT;
					}
					break;
				case OpCodeType.INC:
					switch (type.DataType)
					{
						case DataType.Byte: return OpCode.INC_BYTE;
						case DataType.Double: return OpCode.INC_DOUBLE;
						case DataType.Float: return OpCode.INC_FLOAT;
						case DataType.Int: return OpCode.INC_INT;
						case DataType.Long: return OpCode.INC_LONG;
						case DataType.Short: return OpCode.INC_SHORT;
					}
					break;
				case OpCodeType.DEC:
					switch (type.DataType)
					{
						case DataType.Byte: return OpCode.DEC_BYTE;
						case DataType.Double: return OpCode.DEC_DOUBLE;
						case DataType.Float: return OpCode.DEC_FLOAT;
						case DataType.Int: return OpCode.DEC_INT;
						case DataType.Long: return OpCode.DEC_LONG;
						case DataType.Short: return OpCode.DEC_SHORT;
					}
					break;
				case OpCodeType.NEG:
					switch (type.DataType)
					{
						case DataType.Byte: return OpCode.NEG_BYTE;
						case DataType.Double: return OpCode.NEG_DOUBLE;
						case DataType.Float: return OpCode.NEG_FLOAT;
						case DataType.Int: return OpCode.NEG_INT;
						case DataType.Long: return OpCode.NEG_LONG;
						case DataType.Short: return OpCode.NEG_SHORT;
					}
					break;
				case OpCodeType.ABS:
					switch (type.DataType)
					{
						case DataType.Byte: return OpCode.ABS_BYTE;
						case DataType.Double: return OpCode.ABS_DOUBLE;
						case DataType.Float: return OpCode.ABS_FLOAT;
						case DataType.Int: return OpCode.ABS_INT;
						case DataType.Long: return OpCode.ABS_LONG;
						case DataType.Short: return OpCode.ABS_SHORT;
					}
					break;
			}
			return OpCode.INVALID;
		}

		/// <summary>
		///		Returns true if the given data type is valid for the given operator. 
		/// </summary>
		/// <param name="type">Data type to check operator against.</param>
		/// <param name="opToken">Operator to check validitity for.</param>
		/// <returns>True if the given data type is valid for the given operator.</returns>
        private bool OperatorDataTypeValid(DataTypeValue type, TokenID opToken)
		{
			if (type.DataType == DataType.Invalid) return false;
            if (type.IsArray == true && opToken != TokenID.OpAssign && opToken != TokenID.OpEqual && opToken != TokenID.OpNotEqual) return false;
			switch (opToken)
			{
				case TokenID.OpAdd:
					return (type.DataType == DataType.Object ? false : true);
				case TokenID.OpAssign:
					return true;
				case TokenID.OpAssignAdd:
					return (type.DataType == DataType.Object ? false : true);
				case TokenID.OpAssignBitwiseAnd:
					return (type.DataType != DataType.Int ? false : true);
				case TokenID.OpAssignBitwiseOr:
					return (type.DataType != DataType.Int ? false : true);
				case TokenID.OpAssignBitwiseNot:
					return (type.DataType != DataType.Int ? false : true);
				case TokenID.OpAssignBitwiseSHL:
					return (type.DataType != DataType.Int ? false : true);
				case TokenID.OpAssignBitwiseSHR:
					return (type.DataType != DataType.Int ? false : true);
				case TokenID.OpAssignBitwiseXOr:
					return (type.DataType != DataType.Int ? false : true);
				case TokenID.OpAssignDivide:
					return (type.DataType == DataType.Object || type.DataType == DataType.String || type.DataType == DataType.Null ? false : true);
				case TokenID.OpAssignModulus:
					return (type.DataType != DataType.Int ? false : true);
				case TokenID.OpAssignMultiply:
					return (type.DataType == DataType.Object || type.DataType == DataType.String || type.DataType == DataType.Null ? false : true);
				case TokenID.OpAssignSub:
					return (type.DataType == DataType.Object || type.DataType == DataType.String || type.DataType == DataType.Null ? false : true);
				case TokenID.OpBitwiseAnd:
					return (type.DataType != DataType.Int ? false : true);
				case TokenID.OpBitwiseOr:
					return (type.DataType != DataType.Int ? false : true);
				case TokenID.OpBitwiseSHL:
					return (type.DataType != DataType.Int ? false : true);
				case TokenID.OpBitwiseSHR:
					return (type.DataType != DataType.Int ? false : true);
				case TokenID.OpBitwiseXOr:
					return (type.DataType != DataType.Int ? false : true);
				case TokenID.OpBitwiseNot:
					return (type.DataType != DataType.Int ? false : true);
				case TokenID.OpDecrement:
					return (type.DataType == DataType.Object || type.DataType == DataType.String || type.DataType == DataType.Null ? false : true);
				case TokenID.OpDivide:
					return (type.DataType == DataType.Object || type.DataType == DataType.String || type.DataType == DataType.Null ? false : true);
				case TokenID.OpEqual:
					return true;
				case TokenID.OpGreater:
					return (type.DataType == DataType.Object || type.DataType == DataType.String || type.DataType == DataType.Null ? false : true);
				case TokenID.OpGreaterEqual:
					return (type.DataType == DataType.Object || type.DataType == DataType.String || type.DataType == DataType.Null ? false : true);
				case TokenID.OpIncrement:
					return (type.DataType == DataType.Object || type.DataType == DataType.String || type.DataType == DataType.Null ? false : true);
				case TokenID.OpLess:
					return (type.DataType == DataType.Object || type.DataType == DataType.String || type.DataType == DataType.Null ? false : true);
				case TokenID.OpLessEqual:
					return (type.DataType == DataType.Object || type.DataType == DataType.String || type.DataType == DataType.Null ? false : true);
				case TokenID.OpLogicalAnd:
					return (type.DataType != DataType.Bool ? false : true);
				case TokenID.OpLogicalOr:
					return (type.DataType != DataType.Bool ? false : true);
				case TokenID.OpLogicalNot:
					return (type.DataType != DataType.Bool ? false : true);
				case TokenID.OpModulus:
					return (type.DataType != DataType.Int ? false : true);
				case TokenID.OpMultiply:
					return (type.DataType == DataType.Object || type.DataType == DataType.String || type.DataType == DataType.Null ? false : true);
				case TokenID.OpNotEqual:
					return true;
				case TokenID.OpSub:
					return (type.DataType == DataType.Object || type.DataType == DataType.String || type.DataType == DataType.Null ? false : true);
				default:
					return false;
			}
		}

		/// <summary>
		///		Parses a single statement. A statement encompesses the highest level of parsing, things like
		///		the while loop, the if statement and the return statement ... ecetera.
		/// </summary>
		private void ParseStatement()
		{
			// Check is is an empty statement.
			if (LookAheadToken() != null && LookAheadToken().ID == TokenID.CharSemiColon)
			{
				ExpectToken(TokenID.CharSemiColon);
				return;
			}

			// Check if there is an open block next, and if so parse it.
			if (LookAheadToken().ID == TokenID.CharOpenBrace)
			{
				ParseBlock(TokenID.CharOpenBrace, TokenID.CharCloseBrace);
				return;
			}

			// Check if we have a chunk of meta data next.
			bool foundThisParse = true;
			if (LookAheadToken().ID == TokenID.CharOpenBracket)
			{
				_lastMetaDataSymbol = null;
				foundThisParse = true;
				ParseMetaData();
			}

            // Emit a enter statement opcode if in debug.
            bool isBreakpoint = (LookAheadToken().ID == TokenID.KeywordBreakpoint);
            if ((_compileFlags & CompileFlags.Debug) != 0 && _currentPass == 1 && isBreakpoint == false)
                CreateInstruction(OpCode.ENTER_STATEMENT, _currentScope, (Token)_tokenList[_tokenIndex]);

			// Parse based on starting token.
			switch (NextToken().ID)
			{
				// General statements.
				case TokenID.KeywordDo:			ParseDo();			break;
				case TokenID.KeywordEnum:		ParseEnum();		break;
				case TokenID.KeywordIf:			ParseIf();			break;
				case TokenID.KeywordLock:		ParseLock();		break;
				case TokenID.KeywordAtom:		ParseAtom();		break;
				case TokenID.KeywordReturn:		ParseReturn();		break;
				case TokenID.KeywordSwitch:		ParseSwitch();		break;
				case TokenID.KeywordState:		ParseState();		break;
				case TokenID.KeywordWhile:		ParseWhile();		break;
				case TokenID.KeywordFor:		ParseFor();			break;
				case TokenID.KeywordGoto:		ParseGoto();		break;
				case TokenID.KeywordBreak:		ParseBreak();		break;
				case TokenID.KeywordContinue:	ParseContinue();	break;
				case TokenID.KeywordGotoState:	ParseGotoState();	break;
				case TokenID.KeywordBreakpoint: ParseBreakPoint();  break;
                case TokenID.KeywordNamespace:  ParseNamespace();   break;

				case TokenID.KeywordEditor:
				case TokenID.KeywordEngine:	
					ParseState();		
					break;
				/*
				case TokenID.KeywordStruct:		ParseStruct();		break;
				case TokenID.KeywordClass:		ParseClass();		break;
				*/

				// Variable / function declarations.
				case TokenID.KeywordBool:
				case TokenID.KeywordByte:
				case TokenID.KeywordDouble:
				case TokenID.KeywordObject:
				case TokenID.KeywordFloat:
				case TokenID.KeywordInt:
				case TokenID.KeywordLong:
				case TokenID.KeywordShort:
				case TokenID.KeywordString:
				case TokenID.KeywordVoid:
				case TokenID.KeywordConst:
				case TokenID.KeywordStatic:
				case TokenID.KeywordProperty:
				case TokenID.KeywordImport:
                case TokenID.KeywordExport:
                case TokenID.KeywordThread:
				case TokenID.KeywordPublic:
				case TokenID.KeywordProtected:
				case TokenID.KeywordPrivate:
				case TokenID.KeywordEvent:
                case TokenID.KeywordConsole:

					int offset = 0;
					if (_currentToken.ID == TokenID.KeywordPublic || _currentToken.ID == TokenID.KeywordProtected || _currentToken.ID == TokenID.KeywordPrivate)
						offset = 1;

                    if (_currentToken.ID == TokenID.KeywordConsole || _currentToken.ID == TokenID.KeywordEvent || _currentToken.ID == TokenID.KeywordExport || _currentToken.ID == TokenID.KeywordImport || _currentToken.ID == TokenID.KeywordThread || LookAheadToken(2 + offset).ID == TokenID.CharOpenParenthesis)
						ParseFunction();

					// If its not parenthesis its a variable declaration.
					else
						ParseVariable();

					break;

				// Assignments and function calls.
				case TokenID.TypeIdentifier:

					// Check if its a label.
				    if (LookAheadToken().ID == TokenID.CharColon)
						ParseLabel();

                    // See if its a function or assignment.
                    else
                    {
                        int tokenIndex = _tokenIndex;
                        Token currentToken = _currentToken;
                        ResolveMemberScope();

                        // Ignore any array accessors.
                        if (LookAheadToken().ID == TokenID.CharOpenBracket)
                        {
                            NextToken();
                            int depth = 1;
                            while (depth != 0)
                            {
                                NextToken();
                                if (_currentToken.ID == TokenID.CharOpenBracket)
                                    depth++;
                                else if (_currentToken.ID == TokenID.CharCloseBracket)
                                    depth--;
                            }
                            //ExpectToken(TokenID.CharCloseBracket);
                        }

                        // Function
                        if (LookAheadToken().ID == TokenID.CharOpenParenthesis)
                        {
                            string functionName = _currentToken.Ident;
                            _tokenIndex = tokenIndex;
                            _currentToken = currentToken;

                            // Check we are in a valid scope.
                            if (_currentScope.Type != SymbolType.Function || _currentScope == _globalScope)
                                Error(ErrorCode.InvalidScope, "Function calls are only valid inside a function's or event's scope.", false, 0);
                            
                            DataTypeValue functionDataType = ParseFunctionCall();

                            // Do we have a member call after?
                            if (LookAheadToken().ID == TokenID.OpMemberResolver)
                            {
                                NextToken();

                                // If its not an object how can we access it?
                                if (functionDataType != new DataTypeValue(DataType.Object, false, false))
                                    Error(ErrorCode.IllegalResolve, "Member accessing can only be preformed on objects.", false, 1);

                                // Parse the cast.
                                ExpectToken(TokenID.CharOpenParenthesis);
                                if (NextToken().IsDataType == false)
                                    Error(ErrorCode.InvalidCast, "Attempt to cast member to unknown type.", false, 0);

                                DataTypeValue castDataType = new DataTypeValue(DataTypeFromKeywordToken(_currentToken.ID), false, false);

                                // Is it an array?
                                if (LookAheadToken().ID == TokenID.CharCloseBracket)
                                {
                                    NextToken();
                                    ExpectToken(TokenID.CharCloseBracket);
                                    castDataType.IsArray = true;
                                }
                                ExpectToken(TokenID.CharCloseParenthesis);

                                ExpectToken(TokenID.TypeIdentifier);

                                if (LookAheadToken().ID == TokenID.CharOpenParenthesis)
                                    ParseMemberFunctionCall(_currentToken.Ident, castDataType);
                                else
                                    ParseMemberAssignment(_currentToken.Ident, castDataType);
                            }

                            ExpectToken(TokenID.CharSemiColon);
                        }

                        // Member function / assignment?
                        else if (LookAheadToken().ID == TokenID.OpMemberResolver)
                        {
                            if (_currentPass == 1) CreateInstruction(OpCode.ENTER_ATOM, _currentScope, _currentToken);
                            ParseMemberAccessor();
                            ExpectToken(TokenID.CharSemiColon);
                            if (_currentPass == 1) CreateInstruction(OpCode.LEAVE_ATOM, _currentScope, _currentToken);
                        }

                        // Assignment.
                        else
                        {
                            _tokenIndex = tokenIndex;
                            _currentToken = currentToken;

                            if (_currentPass == 1) CreateInstruction(OpCode.ENTER_ATOM, _currentScope, _currentToken);
                            ParseAssignment();
                            ExpectToken(TokenID.CharSemiColon);
                            if (_currentPass == 1) CreateInstruction(OpCode.LEAVE_ATOM, _currentScope, _currentToken);
                        }
                    }

					break;

				// Its none of the above its invalid.
				default:
					Error(ErrorCode.UnexpectedToken, "Encountered unexpected token \"" + _currentToken.ToString() + "\"", false, 0);

					break;
			}

            // Emit a exit statement opcode if in debug.
            if ((_compileFlags & CompileFlags.Debug) != 0 && _currentPass == 1 && isBreakpoint == false)
                CreateInstruction(OpCode.EXIT_STATEMENT, _currentScope, _currentToken);

			// If we read in any meta data associated with this statement then destroy it now.
			if (_metaDataList.Count > 0 && foundThisParse == true)
			{
				Symbol symbolToAttachTo = null;
				if (_lastMetaDataSymbol != null)
					symbolToAttachTo = _lastMetaDataSymbol;
				else
					symbolToAttachTo = _currentScope;

				if (_currentPass == 0)
				{
					foreach (MetaDataSymbol metaDataSymbol in _metaDataList)
					{
						metaDataSymbol.Scope = symbolToAttachTo;
						symbolToAttachTo.AddSymbol(metaDataSymbol);
					}
				}
				_metaDataList.Clear();
			}
		}

		/// <summary>
		///		Parses a namespace declaration. 
		///		Syntax:
		///			"{" Block "}"
		/// </summary>
        private void ParseNamespace()
        {
            // Read in the identifier.
            ArrayList layers = new ArrayList();
            while (true)
            {
                ExpectToken(TokenID.TypeIdentifier);
                layers.Add(_currentToken.Ident);

                if (LookAheadToken().ID == TokenID.CharPeriod)
                    ExpectToken(TokenID.CharPeriod);
                else
                    break;
            }

            Symbol symbol = null;
            if (_currentPass == 0)
            {
                Symbol lastSymbol = _currentScope;
                foreach (String layer in layers)
                {
                    symbol = lastSymbol.FindSymbol(layer);
                    if (symbol == null)
                    {
                        symbol = new NamespaceSymbol(lastSymbol);
                        symbol.Identifier = layer;
                    }
                    lastSymbol = symbol;
                }
                symbol = lastSymbol;
            }
            else
            {
                Symbol lastSymbol = _currentScope;
                foreach (String layer in layers)
                    lastSymbol = lastSymbol.FindSymbol(layer);
                symbol = lastSymbol;
            }

            // Parse the namespaces block.
            Symbol scope = _currentScope;
            _currentScope = symbol;
            ParseStatement();
            _currentScope = scope;

            // And thats it! Simple huh?
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DataTypeValue ParseMemberAccessor()
        {
            // Skip back a bit.
            _tokenIndex -= 2;
            NextToken();

            // Parse the left hand factor expression. (avatarInstance)
            DataTypeValue factorDataType1 = ParseFactorExpression();

            // Parse any subsequent factor expressions.
            while (true)
            {
                // ->(object)cameraInstance
                // Check that the next token is an member resolver operator.
                Token operatorToken = LookAheadToken();
                if (operatorToken.ID != TokenID.OpMemberResolver)
                    break;
                NextToken();

                Token oldToken = _currentToken;
                int oldTokenIndex = _tokenIndex;

                // If its not an object how can we access it?
                if (factorDataType1 != new DataTypeValue(DataType.Object, false, false))
                    Error(ErrorCode.IllegalResolve, "Member accessing can only be preformed on objects.", false, 1);

                // Parse the cast.
                ExpectToken(TokenID.CharOpenParenthesis);
                if (NextToken().IsDataType == false)
                    Error(ErrorCode.InvalidCast, "Attempt to cast member to unknown type.", false, 0);

                DataTypeValue castDataType = new DataTypeValue(DataTypeFromKeywordToken(_currentToken.ID), false, false);

                // Is it an array?
                if (LookAheadToken().ID == TokenID.CharCloseBracket)
                {
                    NextToken();
                    ExpectToken(TokenID.CharCloseBracket);
                    castDataType.IsArray = true;
                }
                ExpectToken(TokenID.CharCloseParenthesis);

                // Set the factor data type as the casts value.
                factorDataType1 = castDataType;

                // Read in the indentifier of the member.
                string identifier = ExpectToken(TokenID.TypeIdentifier).Ident;

                // We'e accessing a sub-object ->
                if (LookAheadToken().ID == TokenID.OpMemberResolver)
                {
                    // Are we going to index it?
                    bool parsingArray = false;
                    DataTypeValue arrayIndexValue = new DataTypeValue(DataType.Invalid, false, false);
                    if (LookAheadToken().ID == TokenID.CharOpenBracket)
                    {
                        parsingArray = true;
                        arrayIndexValue = ParseExpression();
                        ExpectToken(TokenID.CharCloseBracket);
                    }

                    // Create the symbol.
                    VariableSymbol variableSymbol = null;
                    if (_currentPass == 0)
                    {
                        variableSymbol = _memberScope.FindVariableSymbol(identifier, castDataType) as VariableSymbol;
                        if (variableSymbol == null)
                        {
                            variableSymbol = new VariableSymbol(_memberScope);
                            variableSymbol.Identifier = identifier;
                            variableSymbol.DataType = castDataType;
                            variableSymbol.VariableType = VariableType.Member;
                        }
                    }
                    else if (_currentPass == 1)
                        variableSymbol = _memberScope.FindVariableSymbol(identifier, castDataType) as VariableSymbol;

                    if (_currentPass == 1)
                    {
                        Instruction instruction = null;

                        // Pop the index into arith 1.
                        if (parsingArray == true)
                        {
                            instruction = CreateInstruction(OpCodeByType(arrayIndexValue, OpCodeType.POP), _currentScope, _currentToken);
                            new Operand(instruction, Register.Arithmetic1);
                        }

                        // Pop the object into the member register.
                        instruction = CreateInstruction(OpCode.POP_OBJECT, _currentScope, _currentToken);
                        new Operand(instruction, Register.Arithmetic2);

                        // Call the GET_MEMBER opCode.
                        if (parsingArray)
                        {
                            instruction = CreateInstruction(OpCode.GET_MEMBER, _currentScope, _currentToken);
                            new Operand(instruction, Register.Arithmetic2);
                            new Operand(instruction, variableSymbol);
                        }
                        else
                        {
                            instruction = CreateInstruction(OpCode.GET_MEMBER_INDEXED, _currentScope, _currentToken);
                            new Operand(instruction, Register.Arithmetic2);
                            new Operand(instruction, variableSymbol);
                            new Operand(instruction, Register.Arithmetic1);
                        }

                        // Push the returned value.
                        instruction = CreateInstruction(OpCodeByType(castDataType, OpCodeType.PUSH), _currentScope, _currentToken);
                        new Operand(instruction, Register.Return);
                    }
                }

                // We're calling a function.
                else if (LookAheadToken().ID == TokenID.CharOpenParenthesis)
                    ParseMemberFunctionCall(identifier, castDataType);
                
                // We're parsing an assignment.
                else
                    ParseMemberAssignment(identifier, castDataType);                   
            }

            return factorDataType1;

        }

        /// <summary>
        /// 
        /// </summary>
        private void ParseMemberFunctionCall(string identifier, DataTypeValue returnDataType)
        {
            // Read the opening parenthesis.
            ExpectToken(TokenID.CharOpenParenthesis);

            // Pop the object out.
            if (_currentPass == 1)
            {
                Instruction instruction = CreateInstruction(OpCode.POP_OBJECT, _currentScope, _currentToken);
                new Operand(instruction, Register.Member);
            }

            // Read in each parameter's expression.
            ArrayList parameterTypeList = new ArrayList();
            string parameterMask = "";
            while (true)
            {
                // Check for parenthesis close
                if (LookAheadToken().ID != TokenID.CharCloseParenthesis)
                {
                    // Read in the parameters expression.
                    DataTypeValue expressionType = ParseExpression();
                    parameterMask += (parameterMask != "" ? "," : "") + expressionType.ToString();
                    parameterTypeList.Add(expressionType);

                    // Read in comma if we are not at the 
                    // end of the parameter list.
                    if (LookAheadToken().ID != TokenID.CharCloseParenthesis)
                        ExpectToken(TokenID.CharComma);
                }
                else
                    break;
            }

            // *looks arounds shiftily* ... Ok its hack like but it works.
            parameterTypeList.Reverse();

            // Read the closing parenthesis.
            ExpectToken(TokenID.CharCloseParenthesis);

            // Get the function symbol if in pass 2.
            FunctionSymbol functionSymbol = null;
            if (_currentPass == 1)
            {
                functionSymbol = _memberScope.FindFunctionByMask(identifier, parameterTypeList) as FunctionSymbol;
                if (functionSymbol == null)
                {
                    // Create the function symbol.
                    functionSymbol = new FunctionSymbol(identifier, _memberScope);
                    functionSymbol.Identifier = identifier;
                    functionSymbol.ReturnType = returnDataType;
                    functionSymbol.IsMember = true;
                    functionSymbol.ParameterCount = parameterTypeList.Count;
                    
                    // Create a symbol for each parameters.
                    foreach (DataTypeValue value in parameterTypeList)
                    {
                        VariableSymbol variableSymbol = new VariableSymbol(functionSymbol);
                        variableSymbol.DataType = value;
                        variableSymbol.Identifier = "";
                        variableSymbol.VariableType = VariableType.Parameter;
                        variableSymbol.IsArray = value.IsArray;
                    }
                }

                Instruction instruction = CreateInstruction(OpCode.CALL_METHOD, _currentScope, _currentToken);
                new Operand(instruction, Register.Member);
                new Operand(instruction, functionSymbol);
            }
        }

        /// <summary>
        ///     Parses a member assignment.
        /// </summary>
        private void ParseMemberAssignment(string identifier, DataTypeValue returnDataType)
        {
            // Check we are in a valid scope.
            if (_currentScope.Type != SymbolType.Function || _currentScope == _globalScope)
                Error(ErrorCode.InvalidScope, "Assignments are only valid inside a function's or event's scope.", false, 0);

            #region Variable retrieving

            // If we are in pass 2 try and retrieve the variable.
            VariableSymbol variableSymbol = null;
            DataTypeValue dataType = returnDataType;
            if (_currentPass == 0)
            {
                variableSymbol = _memberScope.FindVariableSymbol(identifier, returnDataType) as VariableSymbol;
                if (variableSymbol == null)
                {
                    variableSymbol = new VariableSymbol(_memberScope);
                    variableSymbol.Identifier = identifier;
                    variableSymbol.DataType = returnDataType;
                    variableSymbol.VariableType = VariableType.Member;
                    if (variableSymbol.VariableType == VariableType.Constant)
                        Error(ErrorCode.IllegalAssignment, "Encountered attempt to assign to a constant variable \"" + identifier + "\".");
                }
            }
            else if (_currentPass == 1)
                variableSymbol = _memberScope.FindVariableSymbol(identifier, returnDataType) as VariableSymbol;

            #endregion
            #region Array index parsing

            // Check if we are assigning to an array.
            bool isArray = false;
            DataTypeValue indexDataType = null;
            if (LookAheadToken().ID == TokenID.CharOpenBracket)
            {
                // Make sure variable is an array if we are in pass 2.
                if (_currentPass == 1 && variableSymbol.DataType.IsArray == false)
                    Error(ErrorCode.InvalidArrayIndex, "Non-array variables can not be indexed.");

                // As its an array and its index we can remove the array declaration from
                // the data type.
                dataType.IsArray = false;

                // Read in opening bracket and check expression is valid
                NextToken();
                if (LookAheadToken().ID == TokenID.CharCloseBracket)
                    Error(ErrorCode.InvalidArrayIndex, "Array's must be indexed.", false, 0);

                // Read in expression and closing bracket.
                indexDataType = ParseExpression();
                ExpectToken(TokenID.CharCloseBracket);

                isArray = true;
            }
            #endregion
            #region Operator parsing

            // Check if the operator is a valid assignment operator.
            NextToken();
            if (_currentToken.ID != TokenID.OpAssign && _currentToken.ID != TokenID.OpAssignAdd &&
                _currentToken.ID != TokenID.OpAssignBitwiseAnd && _currentToken.ID != TokenID.OpAssignBitwiseNot &&
                _currentToken.ID != TokenID.OpAssignBitwiseOr && _currentToken.ID != TokenID.OpAssignBitwiseSHL &&
                _currentToken.ID != TokenID.OpAssignBitwiseSHR && _currentToken.ID != TokenID.OpAssignBitwiseXOr &&
                _currentToken.ID != TokenID.OpAssignDivide && _currentToken.ID != TokenID.OpAssignModulus &&
                _currentToken.ID != TokenID.OpAssignMultiply && _currentToken.ID != TokenID.OpAssignSub &&
                _currentToken.ID != TokenID.OpIncrement && _currentToken.ID != TokenID.OpDecrement)
                Error(ErrorCode.IllegalAssignmentOperator, "Encountered attempt to use an illegal assignment operator.");

            Token operatorToken = _currentToken;
            Instruction instruction = null;
            DataTypeValue expressionType = new DataTypeValue(DataType.Invalid, false, false);
            #endregion

            // Check the data types are valid with this operator.
            if (_currentPass == 1 && OperatorDataTypeValid(variableSymbol.DataType, operatorToken.ID) == false)
                Error(ErrorCode.InvalidDataType, "Operator \"" + operatorToken.Ident + "\" can't be applied to data type \"" + variableSymbol.DataType.ToString() + "\"");

            #region Value byte code emission and parsing

            // Read in expression if it is not a increment (++) or decrement (--) operator.
            if (operatorToken.ID != TokenID.OpIncrement && operatorToken.ID != TokenID.OpDecrement)
            {
                // Parse the expression. 
                expressionType = ParseExpression();

                // Pop the expression into reserved register 3.
                if (_currentPass == 1)
                {
                    // Pop the value of into reserved register 3.
                    instruction = CreateInstruction(OpCodeByType(expressionType, OpCodeType.POP), _currentScope, _currentToken);
                    new Operand(instruction, Register.Reserved3);

                    // Cast the value to a valid type.
                    if (dataType != expressionType)
                    {
                        instruction = CreateInstruction(OpCodeByType(dataType, OpCodeType.CAST), _currentScope, _currentToken);
                        new Operand(instruction, Register.Reserved3);
                    }
                }

                // If we are an array check that we have been indexed, unless we
                // are assigning null to it.
                if (_currentPass == 1 && variableSymbol.DataType.IsArray == true && isArray == false && expressionType.DataType != DataType.Null && expressionType.IsArray != true)
                    Error(ErrorCode.InvalidArrayIndex, "Arrays must be indexed.");
            }
            else
            {
                expressionType = new DataTypeValue(DataType.Int, false, false);
            }
            #endregion

            // Check we can cast to the expression result to the variables data type.
            if (_currentPass == 1 && CanImplicitlyCast(dataType, expressionType) == false)
                Error(ErrorCode.InvalidCast, "Can't implicitly cast between \"" + dataType.ToString() + "\" and \"" + expressionType.ToString() + "\"");

            #region Array index byte code emission

            // If this is an array pop the index into reserved register 2.
            if (isArray == true && _currentPass == 1)
            {
                // Pop the index into reserved register 2.
                instruction = CreateInstruction(OpCodeByType(indexDataType, OpCodeType.POP), _currentScope, _currentToken);
                new Operand(instruction, Register.Reserved2);
            }

            #endregion

            #region Assignment byte code emission

            // Create assignment byte code if we are in pass 2.
            if (_currentPass == 1)
            {
                // Pop the object into the member register.
                instruction = CreateInstruction(OpCode.POP_OBJECT, _currentScope, _currentToken);
                new Operand(instruction, Register.Arithmetic1);

                // Call the GET_MEMBER opCode.
                if (returnDataType.IsArray)
                {
                    instruction = CreateInstruction(OpCode.GET_MEMBER, _currentScope, _currentToken);
                    new Operand(instruction, Register.Arithmetic1);
                    new Operand(instruction, variableSymbol);
                }
                else
                {
                    instruction = CreateInstruction(OpCode.GET_MEMBER_INDEXED, _currentScope, _currentToken);
                    new Operand(instruction, Register.Arithmetic2);
                    new Operand(instruction, variableSymbol);
                    new Operand(instruction, Register.Arithmetic2);
                }

                switch (operatorToken.ID)
                {
                    case TokenID.OpAssign: instruction = CreateInstruction(OpCodeByType(dataType, OpCodeType.MOV), _currentScope, _currentToken); break;
                    case TokenID.OpAssignAdd: instruction = CreateInstruction(OpCodeByType(dataType, OpCodeType.ADD), _currentScope, _currentToken); break;
                    case TokenID.OpAssignBitwiseAnd: instruction = CreateInstruction(OpCode.BIT_AND_INT, _currentScope, _currentToken); break;
                    case TokenID.OpAssignBitwiseNot: instruction = CreateInstruction(OpCode.BIT_NOT_INT, _currentScope, _currentToken); break;
                    case TokenID.OpAssignBitwiseOr: instruction = CreateInstruction(OpCode.BIT_OR_INT, _currentScope, _currentToken); break;
                    case TokenID.OpAssignBitwiseSHL: instruction = CreateInstruction(OpCode.BIT_SHL_INT, _currentScope, _currentToken); break;
                    case TokenID.OpAssignBitwiseSHR: instruction = CreateInstruction(OpCode.BIT_SHR_INT, _currentScope, _currentToken); break;
                    case TokenID.OpAssignBitwiseXOr: instruction = CreateInstruction(OpCode.BIT_XOR_INT, _currentScope, _currentToken); break;
                    case TokenID.OpAssignDivide: instruction = CreateInstruction(OpCodeByType(dataType, OpCodeType.DIV), _currentScope, _currentToken); break;
                    case TokenID.OpAssignModulus: instruction = CreateInstruction(OpCode.MOD_INT, _currentScope, _currentToken); break;
                    case TokenID.OpAssignMultiply: instruction = CreateInstruction(OpCodeByType(dataType, OpCodeType.MUL), _currentScope, _currentToken); break;
                    case TokenID.OpAssignSub: instruction = CreateInstruction(OpCodeByType(dataType, OpCodeType.SUB), _currentScope, _currentToken); break;
                    case TokenID.OpIncrement: instruction = CreateInstruction(OpCodeByType(dataType, OpCodeType.INC), _currentScope, _currentToken); break;
                    case TokenID.OpDecrement: instruction = CreateInstruction(OpCodeByType(dataType, OpCodeType.DEC), _currentScope, _currentToken); break;
                }

                // Generate destination operands based on if variable is array.
                new Operand(instruction, Register.Return);

                // Generate the source operand if we are not dealing with the ++ and -- operators.
                if (operatorToken.ID != TokenID.OpIncrement && operatorToken.ID != TokenID.OpDecrement)
                    new Operand(instruction, Register.Reserved3);

                // Call the SET_MEMBER opCode.
                if (returnDataType.IsArray)
                {
                    instruction = CreateInstruction(OpCode.SET_MEMBER, _currentScope, _currentToken);
                    new Operand(instruction, Register.Arithmetic1);
                    new Operand(instruction, variableSymbol);
                    new Operand(instruction, Register.Return);
                }
                else
                {
                    instruction = CreateInstruction(OpCode.GET_MEMBER_INDEXED, _currentScope, _currentToken);
                    new Operand(instruction, Register.Arithmetic2);
                    new Operand(instruction, variableSymbol);
                    new Operand(instruction, Register.Return);
                    new Operand(instruction, Register.Arithmetic2);
                }
            }

            #endregion
        }

        /// <summary>
        ///     When a function or variable comes across a member resolover this function is called
        ///     to try and resolve it.
        /// 
        ///     Basically if something like this is found in a script
        /// 
        ///         TestA.TestB.TestC = A
        ///     
        ///     This function will resolve TestA into TestB and allow the calling function to
        ///     deal with the variable TestC.
        /// </summary>
        /// <returns>Resolved scope.</returns>
        private Symbol ResolveMemberScope()
        {
            // Find the scope of the first part.
            Symbol checkResolveScope = _currentScope;
            while (checkResolveScope != null)
            {
                Symbol symbol = checkResolveScope.FindSymbol(_currentToken.Ident);
                if (symbol != null)
                {
                    checkResolveScope = symbol;
                    break;
                }
                checkResolveScope = checkResolveScope.Scope;
            }
            
            // Not declared.
            if (checkResolveScope == null && _currentPass != 0)
            {
                Error(ErrorCode.UndeclaredVariable, "Unable to resolve undeclared symbol '" + _currentToken.Ident + "'.", false, 1);
                return null;
            }

            // No member resolve operators? Just return the current scope.
            if (LookAheadToken().ID != TokenID.CharPeriod)
            {
                //if (_currentPass != 0)
                //    System.Console.WriteLine("Returned(A) "+LookAheadToken().Ident);
                return (checkResolveScope != null && checkResolveScope.Scope != null) ? checkResolveScope.Scope : null;
            }

            while (true)
            {
                // Read in member resolver.
                ExpectToken(TokenID.CharPeriod);

                // Read in identifier.
                ExpectToken(TokenID.TypeIdentifier);

                // No member resolver after this identifier? Must be 
                // the member scope then.
                if (LookAheadToken().ID != TokenID.CharPeriod)
                {
                    //if (_currentPass != 0)
                    //    System.Console.WriteLine("Returned(B) " + LookAheadToken().Ident+","+checkResolveScope.Identifier);
                    break;
                }

                if (_currentPass != 0)
                {
                    // Try and get that scope from within this new scope.
                    Symbol originalResolvedscope = checkResolveScope;
                    checkResolveScope = checkResolveScope.FindSymbol(_currentToken.Ident);

                    // Check its valid.
                    if (checkResolveScope == null || (checkResolveScope.Type != SymbolType.Namespace && checkResolveScope.Type != SymbolType.Enumeration))
                    {
                        Error(ErrorCode.IllegalResolve, "Unable to resolve scope '" + _currentToken.Ident + "'.", false, 1);
                        return null;
                    }
                }
            }

            return checkResolveScope;
        }

		/// <summary>
		///		Parses a meta data declaration. 
		///		Syntax:
		///			"[" { Identifier "=" Identifier [ "," ] } "]"
		/// </summary>
		private void ParseMetaData()
		{
			ExpectToken(TokenID.CharOpenBracket);

			while (true)
			{
				string name = "";
				string value = "";

				// Read in an identifier for the variable.
				name = ExpectToken(TokenID.TypeIdentifier).Ident;

				// Read in the equals sign.
				ExpectToken(TokenID.OpAssign);

				// Read in an identifier, string or number for the value.
				NextToken();
				if (_currentToken.ID == TokenID.TypeBoolean ||
					_currentToken.ID == TokenID.TypeByte ||
					_currentToken.ID == TokenID.TypeDouble ||
					_currentToken.ID == TokenID.TypeFloat ||
					_currentToken.ID == TokenID.TypeIdentifier ||
					_currentToken.ID == TokenID.TypeInteger ||
					_currentToken.ID == TokenID.TypeLong ||
					_currentToken.ID == TokenID.TypeShort ||
					_currentToken.ID == TokenID.TypeString)
				{
					if (_currentToken.ID == TokenID.TypeBoolean)
						value = _currentToken.Ident.ToLower() == "true" ? "1" : "0";
					else
						value = _currentToken.Ident;
				}
				else
				{
					Error(ErrorCode.InvalidDataType, "Property variables can't be assigned the value of \"" + _currentToken.Ident + "\".", false, 0);
				}

				// Create a meta data symbol to store this data in.
				MetaDataSymbol metaDataSymbol = new MetaDataSymbol(null, name, value);
				_metaDataList.Add(metaDataSymbol);

				// Read in comma or break out of loop if there isn't one.
				if (LookAheadToken().ID == TokenID.CharComma)
					NextToken();
				else
					break;
			}

			ExpectToken(TokenID.CharCloseBracket);
		}

		/// <summary>
		///		Parses a statement block. A block is usually a 
		///		block of statements surrounded with braces.
		/// </summary>
		private void ParseBlock(TokenID startTokenID, TokenID endTokenID)
		{
			ExpectToken(startTokenID);

			// Check if we are missing a closing brace.
			if (LookAheadToken() != null)
			{
				// Check if we are parsing an empty block.
				if (LookAheadToken().ID == endTokenID)
				{
					NextToken();
					return;
				}
			}
			else
				Error(ErrorCode.ExpectingToken, "Expecting \"}\" token.", false, 0);

			// Keep parsing statements until the end of the block.
			while (LookAheadToken() != null && LookAheadToken().ID != endTokenID)
			{
				if (EndOfTokenStream() == true)
				{
					CheckToken(endTokenID);
					return;
				}
				ParseStatement();
			}

			ExpectToken(endTokenID);
		}

		/// <summary>
		///		Parses a state block. A state block is used to seperate events into specific
		///		states to better emulate a state based machine.
		///		Syntax:
		///			"state" Identifier Statement
		/// </summary>
		private void ParseState()
		{
			// Make sure we are in a state's scope.
			if (_currentScope != _globalScope)
				Error(ErrorCode.InvalidScope, "States can only be declared within the global scope.", false, 0);

			// See if this is the engine or editor default state.
			bool isEngineDefault = false;
			bool isEditorDefault = false;
			int flagCount = 0;
			while (_currentToken.ID != TokenID.KeywordState && EndOfTokenStream() != true)
			{
				switch (_currentToken.ID)
				{
					case TokenID.KeywordEngine:
						if (_defaultEngineState != null) Error(ErrorCode.DuplicateDefault, "Default engine state declared multiple times.", false, 0);
						if (isEngineDefault == true) Error(ErrorCode.DuplicateFlag, "\"" + _currentToken.Ident + "\" flag declared multiple times.", false, 0);
						isEngineDefault = true;
						break;

					case TokenID.KeywordEditor:
						if (_defaultEditorState != null) Error(ErrorCode.DuplicateDefault, "Default editor state declared multiple times.", false, 0);
						if (isEditorDefault == true) Error(ErrorCode.DuplicateFlag, "\"" + _currentToken.Ident + "\" flag declared multiple times.", false, 0);
						isEditorDefault = true;
						break;
				}
				flagCount++;
				NextToken();
			}

			// Read in state keyword if there are previous flags.
			if (flagCount != 0) CheckToken(TokenID.KeywordState);

			// Validate the "state(Identifier)" section of the
			// the event declaration.
			ExpectToken(TokenID.TypeIdentifier);
			string stateIdent = _currentToken.Ident;

			// Create event's symbol.
			StateSymbol symbol = null;
			if (_currentPass == 0)
			{
				if (_currentScope.FindSymbol(stateIdent, SymbolType.State) != null)
					Error(ErrorCode.DuplicateSymbol, "State \"" + stateIdent + "\" declared multiple times.", false, 0);
				symbol = new StateSymbol(_currentScope);
				symbol.Identifier = stateIdent;
				symbol.IsEngineDefault = isEngineDefault;
				symbol.IsEditorDefault = isEditorDefault;
				if (isEngineDefault == true) 
                    _defaultEngineState = symbol;
				if (isEditorDefault == true)
                    _defaultEditorState = symbol;
			}
			else if (_currentPass == 1)
			{
				symbol = _currentScope.FindSymbol(stateIdent, SymbolType.State) as StateSymbol;
			}

			// Read in this event's contents.
			_lastMetaDataSymbol = symbol;
			Symbol scope = _currentScope;
			_currentScope = symbol;
			ParseStatement();
			_currentScope = scope;
		}

		/// <summary>
		///		Parses a variable declaration. A variable is a place in memory that is used	
		///		to store a value used by other statements.
		///		Syntax:
		///			DataType { Identifier [ "[" Expression "]" ] [ "=" Expression ] [ "," ] } ";"
		/// </summary>
		private void ParseVariable()
		{
			// Make sure we are in a valid scope.
			if (_currentScope.Type != SymbolType.Function &&
				_currentScope.Type != SymbolType.State &&
                _currentScope.Type != SymbolType.Namespace) Error(ErrorCode.InvalidScope, "Variables can't be declared in this scope.", false, 0);

			// Check if we are in a states scope.
			bool inState = (_currentScope.Type == SymbolType.State);
            bool inNamespace = (_currentScope.Type == SymbolType.Namespace);
            bool inEnumeration = (_currentScope.Type == SymbolType.Enumeration);

			// Read in each flag.
			bool isStatic = false;
			bool isConst = false;
			bool isProperty = false;
            SymbolAccessModifier modifier = SymbolAccessModifier.Private;
            bool gotModifier = false;
			while (EndOfTokenStream() != true)
			{
				switch (_currentToken.ID)
				{
					case TokenID.KeywordStatic:
						if (isStatic == true) Error(ErrorCode.DuplicateFlag, "\"" + _currentToken.Ident + "\" flag declared multiple times.", false, 0);
						isStatic = true;
						break;
					case TokenID.KeywordConst:
						if (isConst == true) Error(ErrorCode.DuplicateFlag, "\"" + _currentToken.Ident + "\" flag declared multiple times.", false, 0);
						isConst = true;
						break;
					case TokenID.KeywordProperty:
						if (isProperty == true) Error(ErrorCode.DuplicateFlag, "\"" + _currentToken.Ident + "\" flag declared multiple times.", false, 0);
						isProperty = true;
						break;
                    case TokenID.KeywordPublic:
                        if (gotModifier == true) Error(ErrorCode.DuplicateFlag, "Access modifier has already been declared.", false, 0);		
                        modifier = SymbolAccessModifier.Public;
                        gotModifier = true;
                        break;
                    case TokenID.KeywordPrivate:
                        if (gotModifier == true) Error(ErrorCode.DuplicateFlag, "Access modifier has already been declared.", false, 0);
                        modifier = SymbolAccessModifier.Private;
                        gotModifier = true;
                        break;
                    case TokenID.KeywordProtected:
                        if (gotModifier == true) Error(ErrorCode.DuplicateFlag, "Access modifier has already been declared.", false, 0);
                        modifier = SymbolAccessModifier.Protected;
                        gotModifier = true;
                        break;
                    default:
                        goto outOfLoop;
				}
				NextToken();
			}
            outOfLoop:

			// Get data type by the current tokens id.
			DataType dataType = DataTypeFromKeywordToken(_currentToken.ID);
			if (dataType == DataType.Void || dataType == DataType.Invalid)
				Error(ErrorCode.InvalidDataType, "Variable can't be declared as \"" + _currentToken.Ident + "\".", false, 0);

			// Check if its an array.
			bool isArray = false;
			if (LookAheadToken().ID == TokenID.CharOpenBracket)
			{
				ExpectToken(TokenID.CharOpenBracket);
				isArray = true;
				ExpectToken(TokenID.CharCloseBracket);
			}

			// Parse all variables seperated by a comma.
			while (true)
			{
				// Read in variable identifier and store it for later use.
				ExpectToken(TokenID.TypeIdentifier);
				string variableIdent = _currentToken.Ident;

				// Create a new variable symbol.
				VariableSymbol variableSymbol = null;
				if (_currentPass == 0)
				{
					if (_currentScope.FindSymbol(variableIdent, SymbolType.Variable) != null)
						Error(ErrorCode.DuplicateSymbol, "Variable \"" + variableIdent + "\" declared multiple times.", false, 0);
					variableSymbol = new VariableSymbol(_currentScope);
					variableSymbol.Identifier = variableIdent;
					variableSymbol.IsArray = isArray;
					variableSymbol.IsConstant = isConst;
					variableSymbol.IsProperty = isProperty;
                    variableSymbol.AccessModifier = modifier;
					variableSymbol.DataType = new DataTypeValue(dataType, isArray, false);
					variableSymbol.VariableType = isConst == true ? VariableType.Constant : (_currentScope == _globalScope || inState == true || isStatic == true || inEnumeration == true || inNamespace == true ? VariableType.Global : VariableType.Local);
                    if (variableSymbol.VariableType == VariableType.Constant || variableSymbol.VariableType == VariableType.Global)
					{
						variableSymbol.MemoryIndex = _memorySize;
						_memorySize++;
					}
					else
					{
						if (_currentScope.Type == SymbolType.Function)
						{
							variableSymbol.StackIndex = -(((FunctionSymbol)_currentScope).LocalDataSize + 2);
							((FunctionSymbol)_currentScope).LocalDataSize++;
						}
					}
				}
				else if (_currentPass == 1)
				{
					variableSymbol = _currentScope.FindSymbol(variableIdent, SymbolType.Variable) as VariableSymbol;
					if (variableSymbol == null) variableSymbol = _globalScope.FindSymbol(variableIdent, SymbolType.Variable) as VariableSymbol;
				}

				_lastMetaDataSymbol = variableSymbol;

				// Check if there is an assignment.
				if (LookAheadToken().ID == TokenID.OpAssign)
				{
					NextToken();

					// If we are a constant then see if we are being assigned a literal value.
					Token LAT = LookAheadToken();
					if (isConst == true &&
					   (LAT.ID == TokenID.TypeBoolean || LAT.ID == TokenID.TypeByte ||
						LAT.ID == TokenID.TypeDouble || LAT.ID == TokenID.TypeFloat ||
						LAT.ID == TokenID.TypeInteger || LAT.ID == TokenID.TypeLong ||
						LAT.ID == TokenID.TypeShort ||
						LAT.ID == TokenID.TypeString) && LookAheadToken(2).ID == TokenID.CharSemiColon)
					{
						NextToken();
						if (CanImplicitlyCast(variableSymbol.DataType, new DataTypeValue(DataTypeFromTypeToken(_currentToken.ID), false, false)) == true)
							variableSymbol.ConstToken = _currentToken;
						else
							Error(ErrorCode.InvalidDataType, "Unable to implicitly cast from type \"" + DataTypeFromTypeToken(_currentToken.ID).ToString() + "\" to type \"" + variableSymbol.DataType.ToString() + "\".", false, 1);
					}
					else
					{
						// This will temporarily change the state if the variable is static.
                        Symbol previousScope = null;
                        if (isStatic == true || inState == true || inNamespace == true || inEnumeration == true)
						{
                            previousScope = _overrideInstructionScope;
							_overrideInstructionScope = _globalScope;
					    }

						// Parse the assignment expression.
						DataTypeValue expressionType = ParseExpression();

						if (CanImplicitlyCast(variableSymbol.DataType, expressionType) == true)
						{
							if (_currentPass == 1)
							{
								// Pop the value out of the stack and assign it to this variable.
								Instruction instruction = CreateInstruction(OpCodeByType(expressionType, OpCodeType.POP), _currentScope, _currentToken);
								new Operand(instruction, Register.Arithmetic1);

								// Cast arith register 1 into the variables type.
								if (variableSymbol.DataType != expressionType)
								{
									instruction = CreateInstruction(OpCodeByType(variableSymbol.DataType, OpCodeType.CAST), _currentScope, _currentToken);
									new Operand(instruction, Register.Arithmetic1);
								}

								// Move the resulting value (stored in arithmatic register 3)
								// into the variable.
								instruction = CreateInstruction(OpCodeByType(variableSymbol.DataType, OpCodeType.MOV), _currentScope, _currentToken);

								Operand op1 = null;
								if (variableSymbol.VariableType == VariableType.Global || variableSymbol.VariableType == VariableType.Constant)
									op1 = Operand.DirectMemoryOperand(instruction, variableSymbol.MemoryIndex);
								else
									op1 = Operand.DirectStackOperand(instruction, variableSymbol.StackIndex);
								new Operand(instruction, Register.Arithmetic1);
							}
						}
						else
						{
							Error(ErrorCode.InvalidDataType, "Unable to implicitly cast from type \"" + expressionType.ToString() + "\" to type \"" + variableSymbol.DataType.ToString() + "\".", false, 1);
						}

						// If we have changed the scope then change back.
                        if (isStatic == true || inState == true || inNamespace == true || inEnumeration == true)
						{
                            _overrideInstructionScope = previousScope;
						}
					}
				}

                else
                {
                    if (isConst == true) Error(ErrorCode.IllegalAssignment, "Constant must be assigned a value.", false, 0);
                }

				// Read in comma or break out of loop if there isn't one.
				if (LookAheadToken().ID == TokenID.CharComma)
					NextToken();
				else
					break;
			}

			// Read in semi-colon at end of declarations.
			ExpectToken(TokenID.CharSemiColon);
		}

		/// <summary>
		///		Parses a breakpoint declaration, the breakpoint statement is used to
		///		generate a byte code instruction to stop execution and bring up the debug window.
		///		Syntax:
		///			"breakpoint" ";"
		/// </summary>
		private void ParseBreakPoint()
		{
			// Make sure we are in a valid scope.
			if (_currentScope.Type != SymbolType.Function)
				Error(ErrorCode.InvalidScope, "Lock statements can only be declared within a function or event scope.", false, 0);

			// Create a breakpoint instruction.
			if (_currentPass == 1) CreateInstruction(OpCode.BREAKPOINT, _currentScope, _currentToken);

			// Read in semi-colon at end of declarations.
			ExpectToken(TokenID.CharSemiColon);
		}

		/// <summary>
		///		Parses a thread lock declaration. A lock will stop any other
		///		threads from executing the code contained within it beore the thread
		///		that enters the lock block exits it.
		///		Syntax:
		///			"lock" Statement
		/// </summary>
		private void ParseLock()
		{
			// Make sure we are in a valid scope.
			if (_currentScope.Type != SymbolType.Function)
				Error(ErrorCode.InvalidScope, "Lock statements can only be declared within a function or event scope.", false, 0);

			// Make sure we are not already in a lock statement.
			if (_inLock == true)
				Error(ErrorCode.InvalidNesting,"Lock statements cannot be nested.");

			// If we are in the second pass emit lock byte code.
			if (_currentPass == 1) CreateInstruction(OpCode.LOCK, _currentScope, _currentToken);

			// Parse the statement block.
			_inLock = true;
			ParseStatement();
			_inLock = false;

			// If we are in the second pass emit unlock byte code.
			if (_currentPass == 1) CreateInstruction(OpCode.UNLOCK, _currentScope, _currentToken);
		}

		/// <summary>
		///		Parses a thread atom declaration. An atom will force the virtual machine
		///		to execute the area of code in side it before returning, even if the virtual
		///		machines time slice runs out.
		///		Syntax:
		///			"atom" Statement
		/// </summary>
		private void ParseAtom()
		{
			// Make sure we are in a valid scope.
			if (_currentScope.Type != SymbolType.Function)
				Error(ErrorCode.InvalidScope, "Atom statements can only be declared within a function or event scope.", false, 0);

			// Make sure we are not already in a atom statement.
			if (_inAtom == true)
				Error(ErrorCode.InvalidNesting, "Atom statements cannot be nested.");

			// If we are in the second pass emit lock byte code.
			if (_currentPass == 1) CreateInstruction(OpCode.ENTER_ATOM, _currentScope, _currentToken);

			// Parse the locked block.
			_inAtom = true;
			ParseStatement();
			_inAtom = false;

			// If we are in the second pass emit unlock byte code.
			if (_currentPass == 1) CreateInstruction(OpCode.LEAVE_ATOM, _currentScope, _currentToken);
		}

		/// <summary>
		///		Parses a enumeration declaration. An enumeration is a way of creating a list
		///		of sequential integer constants without having to create and assign them.
		///		Syntax:
		///			"enum" "{" { Identifier [ "=" Expression ] [ "," ] } "}"
		/// </summary>
		private void ParseEnum()
		{
			// Make sure we are in a valid scope.
			if (_currentScope != _globalScope &&
                _currentScope.Type != SymbolType.Namespace)
				Error(ErrorCode.InvalidScope, "Enumerations are only valid in global scope.", false, 0);

			// Read in enumeration identifier.
			ExpectToken(TokenID.TypeIdentifier);

			// Create enumeration symbol.
			EnumerationSymbol mainEnumSymbol = null; 
			if (_currentPass == 0)
			{
				if (_currentScope.FindSymbol(_currentToken.Ident) != null) Error(ErrorCode.DuplicateSymbol, "Encountered multiple declarations of the "+_currentToken.Ident+" symbol");
				mainEnumSymbol = new EnumerationSymbol(_currentScope);
				mainEnumSymbol.Identifier = _currentToken.Ident;
			}
			else
				mainEnumSymbol = _currentScope.FindSymbol(_currentToken.Ident) as EnumerationSymbol;

			// Read in opening brace.
			ExpectToken(TokenID.CharOpenBrace);

			// Read in each constant and its expression.
			int indexValue = 1;
			while (true)
			{

				// Read in constant identifier.
				ExpectToken(TokenID.TypeIdentifier);
				string enumIdentifier = _currentToken.Ident;

				// Create constant variable to store value into.
				VariableSymbol enumSymbol = null;
                if (_currentPass == 0)
                {
                    if (mainEnumSymbol.FindSymbol(_currentToken.Ident) != null) Error(ErrorCode.DuplicateSymbol, "Encountered multiple declarations of the " + _currentToken.Ident + " symbol");
                    enumSymbol = new VariableSymbol(mainEnumSymbol);
                    enumSymbol.Identifier = enumIdentifier;
                    enumSymbol.DataType = new DataTypeValue(DataType.Int, false, false);
                    enumSymbol.IsConstant = true;
                }
                else
                    enumSymbol = mainEnumSymbol.FindSymbol(enumIdentifier) as VariableSymbol;

                _lastMetaDataSymbol = enumSymbol;

				// Read in expression if its there.
				Instruction instruction;
				if (LookAheadToken().ID == TokenID.OpAssign)
				{
					NextToken();

					Token LAT = LookAheadToken();
					Token LALAT = LookAheadToken(2);

					if ((LAT.ID == TokenID.TypeBoolean || LAT.ID == TokenID.TypeByte ||
						LAT.ID == TokenID.TypeDouble || LAT.ID == TokenID.TypeFloat ||
						LAT.ID == TokenID.TypeInteger || LAT.ID == TokenID.TypeLong ||
						LAT.ID == TokenID.TypeShort ||
                        LAT.ID == TokenID.TypeString) && (LALAT.ID == TokenID.CharComma || LALAT.ID == TokenID.CharCloseBrace))
					{
						NextToken();
						if (CanImplicitlyCast(enumSymbol.DataType, new DataTypeValue(DataTypeFromTypeToken(_currentToken.ID), false, false)) == true)
							enumSymbol.ConstToken = _currentToken;
						else
							Error(ErrorCode.InvalidDataType, "Unable to implicitly cast from type \"" + DataTypeFromTypeToken(_currentToken.ID).ToString() + "\" to type \"" + enumSymbol.DataType.ToString() + "\".", false, 1);
					}
					else
					{
                        Symbol oldScope = _currentScope;
                        _currentScope = _globalScope;

						// Parse the assignment expression.
						DataTypeValue assignmentType = ParseExpression();

						if (CanImplicitlyCast(new DataTypeValue(DataType.Int, false, false), assignmentType) == true)
						{
                            // We need to allocate some memory space.
                            if (_currentPass == 0)
                            {
                                enumSymbol.MemoryIndex = _memorySize;
                                _memorySize++;
                            }

							if (_currentPass == 1)
							{
								// Pop the expression value into arith register 1.
								instruction = CreateInstruction(OpCodeByType(enumSymbol.DataType, OpCodeType.POP), _currentScope, _currentToken);
								new Operand(instruction, Register.Arithmetic1);
								
								// Move the value stored in arith register 1 into enums memory space.
								instruction = CreateInstruction(OpCodeByType(enumSymbol.DataType, OpCodeType.MOV), _currentScope, _currentToken);
								Operand.DirectMemoryOperand(instruction, enumSymbol.MemoryIndex);
								new Operand(instruction, Register.Arithmetic1);
							}
						}
						else
							Error(ErrorCode.InvalidDataType, "Enumeration const of type \"" + enumSymbol.DataType.ToString() + "\" can't store type \"" + assignmentType.ToString() + "\".", false, 0);

                        _currentScope = oldScope;
                    }

				}
				else
					enumSymbol.ConstToken = new Token(TokenID.TypeInteger, indexValue.ToString(), 0, 0, "");

				// Update index number in a *2 fashion so it can
				// be used as a bitmask.
				indexValue *= 2;

				// Read in comma if it exists.
				if (LookAheadToken().ID == TokenID.CharComma)
					NextToken();
				else
					break;

			}

			// Read in closing brace.
			ExpectToken(TokenID.CharCloseBrace);
		}

		/// <summary>
		///		Parses a function declaration. A function is a way of isolating code that you want
		///		to be able to call multiple times without rewriting it each time.
		///		Syntax:
		///			ReturnType Identifier "(" { Identifier ["=" Expression] [ "," ] } ")" Statement
		/// </summary>
		private void ParseFunction()
		{
			// Make sure we are in a state's scope.
			if (_currentToken.ID == TokenID.KeywordEvent && 
                _currentScope.Type != SymbolType.State)
				Error(ErrorCode.InvalidScope, "Events can only be declared within a state block.", false, 0);

			// Make sure we are in the global scopes as functions can't 
			// be declared anywhere else.
			if (_currentScope.Type != SymbolType.State && 
                _currentScope.Type != SymbolType.Function &&
                _currentScope.Type != SymbolType.Namespace)
				Error(ErrorCode.InvalidScope, "Functions can only be declared in a function's, state's or event's scope.", false, 0);

			// Read in each flag.
			bool isThread = false;
			bool isEvent = false;
            bool isConsole = false;
            bool isExport = false;
            bool isImport = false;
            SymbolAccessModifier modifier = SymbolAccessModifier.Private;
            bool gotModifier = false;
			while (_currentToken.IsDataType == false && EndOfTokenStream() != true)
			{
				switch (_currentToken.ID)
				{
					case TokenID.KeywordThread:
						if (isThread == true) Error(ErrorCode.DuplicateFlag, "\"" + _currentToken.Ident + "\" flag declared multiple times.", false, 0);
						isThread = true;
						break;
					case TokenID.KeywordEvent:
						if (isEvent == true) Error(ErrorCode.DuplicateFlag, "\"" + _currentToken.Ident + "\" flag declared multiple times.", false, 0);
						isEvent = true;
						break;
                    case TokenID.KeywordConsole:
                        if (isConsole == true) Error(ErrorCode.DuplicateFlag, "\"" + _currentToken.Ident + "\" flag declared multiple times.", false, 0);
                        isConsole = true;
                        break;
                    case TokenID.KeywordImport:
                        if (isImport == true) Error(ErrorCode.DuplicateFlag, "\"" + _currentToken.Ident + "\" flag declared multiple times.", false, 0);
                        isImport = true;
                        break;
                    case TokenID.KeywordExport:
                        if (isExport == true) Error(ErrorCode.DuplicateFlag, "\"" + _currentToken.Ident + "\" flag declared multiple times.", false, 0);
                        isExport = true;
                        break;
                    case TokenID.KeywordPublic:
                        if (gotModifier == true) Error(ErrorCode.DuplicateFlag, "Access modifier has already been declared.", false, 0);
                        modifier = SymbolAccessModifier.Public;
                        gotModifier = true;
                        break;
                    case TokenID.KeywordPrivate:
                        if (gotModifier == true) Error(ErrorCode.DuplicateFlag, "Access modifier has already been declared.", false, 0);
                        modifier = SymbolAccessModifier.Private;
                        gotModifier = true;
                        break;
                    case TokenID.KeywordProtected:
                        if (gotModifier == true) Error(ErrorCode.DuplicateFlag, "Access modifier has already been declared.", false, 0);
                        modifier = SymbolAccessModifier.Protected;
                        gotModifier = true;
                        break;
				}
                NextToken();
			}
            if (isEvent == true && (isThread == true || isExport == true || isImport == true || isConsole == true))
				Error(ErrorCode.InvalidFlag, "Events can't be declared as native, threaded, exported, imported or console.");

			// Store the return type for later.
			DataTypeValue returnDataType = new DataTypeValue(DataTypeFromKeywordToken(_currentToken.ID), false, false);
            if (returnDataType.DataType == DataType.Invalid)
                Error(ErrorCode.InvalidDataType, "Functions can't be declared as \"" + _currentToken.Ident + "\".", false, 0);

			// Check for an array reference
			if (LookAheadToken().ID == TokenID.CharOpenBracket)
			{
				NextToken();
				returnDataType.IsArray = true;
				ExpectToken(TokenID.CharCloseBracket);
			}

			// Read in the functions identifier and store it
			// for layer use.
			ExpectToken(TokenID.TypeIdentifier);
			string functionIdentifier = _currentToken.Ident;

			// Read in the opening parenthesis used to define the paremeter list.
			ExpectToken(TokenID.CharOpenParenthesis);

			// Read in each paremeter.
			ArrayList functionParameterMask = new ArrayList();
			int parameterCount = 0;
			ArrayList paramList = new ArrayList();
			if (LookAheadToken().ID != TokenID.CharCloseParenthesis)
			{

				// Read in each parameter
				while (true)
				{

					// Check if there is a constant keyword before this variable.
					bool paramIsConst = false;
					if (LookAheadToken().ID == TokenID.KeywordConst)
					{
						NextToken();
						paramIsConst = true;
					}

					// Read in the data type keyword and store it.
					NextToken();
					DataTypeValue paramDataType = new DataTypeValue(DataTypeFromKeywordToken(_currentToken.ID), false, false);
					if (paramDataType.DataType == DataType.Invalid) Error(ErrorCode.InvalidDataType, "Expecting data type keyword.", false, 0);

					// Read in array declaration.
					bool paramIsArray = false;
					if (LookAheadToken().ID == TokenID.CharOpenBracket)
					{
						NextToken();
						paramIsArray = true;
						paramDataType.IsArray = true;
                        ExpectToken(TokenID.CharCloseBracket);
					}

					// Increase the parameter mask
					functionParameterMask.Add(paramDataType);
					parameterCount++;

					// Read in the parameters identifier.
					ExpectToken(TokenID.TypeIdentifier);
					string paramIdentifier = _currentToken.Ident;

					// Process parameters depending on current pass.
					if (_currentPass == 0)
					{
						VariableSymbol variableSymbol = new VariableSymbol(null);
						variableSymbol.DataType = paramDataType;
						variableSymbol.Identifier = paramIdentifier;
						variableSymbol.VariableType = VariableType.Parameter;
						variableSymbol.IsArray = paramIsArray;
						variableSymbol.IsConstant = paramIsConst;
						paramList.Add(variableSymbol);
					}

					// Read in comma if it exists.
					if (LookAheadToken().ID == TokenID.CharComma)
						NextToken();
					else
						break;

				}

			}

			// *looks arounds shiftily* ... Ok its hack like but it works.
			functionParameterMask.Reverse();
			
			// Create a new function symbol.
			FunctionSymbol functionSymbol;
			if (_currentPass == 0)
			{
				if (_currentScope.FindFunctionByMask(functionIdentifier, functionParameterMask) != null)
					Error(ErrorCode.DuplicateSymbol, "Function \"" + functionIdentifier + "\" redefinition.", false, 0);

				functionSymbol = new FunctionSymbol(functionIdentifier, _currentScope);
				functionSymbol.ReturnType = returnDataType;
				functionSymbol.ParameterCount = parameterCount;
				functionSymbol.IsThreadSpawner = isThread;
				functionSymbol.IsEvent = isEvent;
                functionSymbol.IsConsole = isConsole;
                functionSymbol.IsImport = isImport;
                functionSymbol.IsExport = isExport;
                functionSymbol.AccessModifier = modifier;

                if (isConsole == true && functionSymbol.ReturnType.DataType != DataType.Void)
                    Error(ErrorCode.InvalidDataType, "Console variables cannot return a value.");

				paramList.Reverse();
				int parameterIndex = 0;
				foreach (VariableSymbol variableSymbol in paramList)
				{
					if (functionSymbol.FindSymbol(variableSymbol.Identifier, SymbolType.Variable) != null)
						Error(ErrorCode.DuplicateSymbol, "Variable redefinition \"" + variableSymbol.Identifier + "\"");

                    // If we're a console function we can only accept bool, int, string or float
                    if (isConsole == true && 
                        (variableSymbol.DataType.IsArray == true ||
                        variableSymbol.DataType.DataType == DataType.Byte ||
                        variableSymbol.DataType.DataType == DataType.Double ||
                        variableSymbol.DataType.DataType == DataType.Object ||
                        variableSymbol.DataType.DataType == DataType.Short)) 
                        Error(ErrorCode.InvalidDataType, "Console variables can only accept bool, integer, string or float parameters.");

					functionSymbol.AddSymbol(variableSymbol);
					variableSymbol.Scope = functionSymbol;
					parameterIndex++;
				}
			}
			else
			{
				functionSymbol = _currentScope.FindFunctionByMask(functionIdentifier, functionParameterMask) as FunctionSymbol;
				if (functionSymbol == null) functionSymbol = _globalScope.FindFunctionByMask(functionIdentifier, functionParameterMask) as FunctionSymbol;
				if (functionSymbol == null) Error(ErrorCode.InvalidFunction, "Attempt to call undeclared function \"" + functionIdentifier + "(" + functionParameterMask + ")\".");

				for (int i = 0; i < parameterCount; i++)
				{
					VariableSymbol variableSymbol = (VariableSymbol)functionSymbol.Symbols[i];
					variableSymbol.StackIndex = -(functionSymbol.LocalDataSize + 2 + (i + 1));
				}
			}
			_lastMetaDataSymbol = functionSymbol;

			// Read in the closing parenthesis used to define the end of the paremeter list.
			ExpectToken(TokenID.CharCloseParenthesis);

			// Read in this function's statement block.
            if (functionSymbol.IsImport == false)
			{
				Symbol scope = _currentScope;
				Instruction instruction = null;
				_currentScope = functionSymbol;
				ParseStatement();
				_currentScope = scope;

				// Append a mandatory return instruction if we are in pass 2.
				if (_currentPass == 1)
				{
					// Cast the return registered into the return type.
					if (functionSymbol.ReturnType.DataType != DataType.Void)
					{
						instruction = CreateInstruction(OpCodeByType(functionSymbol.ReturnType, OpCodeType.CAST), functionSymbol, _currentToken);
						new Operand(instruction, Register.Return);
					}

					// Return from function.
					instruction = CreateInstruction(OpCode.RETURN, functionSymbol, _currentToken);
				}
			}
			else
				ExpectToken(TokenID.CharSemiColon);
		}

		/// <summary>
		///		Parses a function call, which forces the virtual machine to push a function's 
		///		stack frame onto the stack and execute it before returning to its current executing
		///		statement.
		///		Syntax:
		///			Identifier "(" { Expression [ "," ] } ")"
		/// </summary>
		/// <returns>The data type returned by the function call.</returns>
		private DataTypeValue ParseFunctionCall()
		{
			// Store function identifier.
            Symbol resolvedScope = ResolveMemberScope();
            string functionIdentifier = _currentToken.Ident;

			// Read the opening parenthesis.
			ExpectToken(TokenID.CharOpenParenthesis);

			// Read in each parameter's expression.
			ArrayList parameterTypeList = new ArrayList();
			string parameterMask = "";
			while (true)
			{
				// Check for parenthesis close
				if (LookAheadToken().ID != TokenID.CharCloseParenthesis)
				{
					// Read in the parameters expression.
					DataTypeValue expressionType = ParseExpression();
					parameterMask += (parameterMask != "" ? "," : "") + expressionType.ToString();
					parameterTypeList.Add(expressionType);

					// Read in comma if we are not at the 
					// end of the parameter list.
					if (LookAheadToken().ID != TokenID.CharCloseParenthesis)
						ExpectToken(TokenID.CharComma);
				}
				else
					break;
			}

			// *looks arounds shiftily* ... Ok its hack like but it works.
			parameterTypeList.Reverse();

			// Read the closing parenthesis.
			ExpectToken(TokenID.CharCloseParenthesis);

			// Get the function symbol if in pass 2.
			FunctionSymbol functionSymbol = null;
			if (_currentPass == 1)
			{
                functionSymbol = resolvedScope.FindFunctionByMask(functionIdentifier, parameterTypeList) as FunctionSymbol;
				if (functionSymbol == null) Error(ErrorCode.InvalidFunction,"Attempt to call undeclared function \""+functionIdentifier+"("+parameterMask+")\".");
				if (functionSymbol.IsEvent == true) Warning(ErrorCode.InvalidFunction, "Events should only be called from native code, to do otherwise is bad coding practice.");
                functionSymbol.IsUsed = true;
            }

			// Go through each parameter and check its type.
            //bool needsConverting = false;
			for (int i = 0; i < parameterTypeList.Count; i++)
			{
				if (_currentPass == 1)
				{
					VariableSymbol paramSymbol = functionSymbol.FindSymbol(i) as VariableSymbol;
                    if (CanImplicitlyCast((DataTypeValue)parameterTypeList[i], paramSymbol.DataType) == false)
                        Error(ErrorCode.InvalidCast, "Can't implicitly cast between \"" + ((DataTypeValue)parameterTypeList[i]).ToString() + "\" and \"" + paramSymbol.DataType.ToString() + "\"");
                    //if ((DataTypeValue)parameterTypeList[i] != paramSymbol.DataType)
                    //    needsConverting = true;
                }
			}
            
            // Do we need to convert parameters?
            //if (needsConverting == true && _currentPass == 1)
            //{

            //}
            //else
            //{

            //}

			// Create calling byte code.
			if (_currentPass == 1)
			{
				Instruction instruction = CreateInstruction(OpCode.CALL, _currentScope, _currentToken);
				new Operand(instruction, functionSymbol);
			}

			if (_currentPass == 1)
				return functionSymbol.ReturnType;
			else
				return new DataTypeValue(DataType.Void, false, false);
		}

		/// <summary>
		///		Parses a for loop, which is similar to the while statement but keeps the initialization,
		///		incrementation and comparison in a more intuative format.
		///		Syntax:
		///			"For" "(" Initialization ";" Expression ";" Expression ")" Statement
		/// 
		///		Output byte code example:
        ///         [initialization]
        ///     start:
        ///         [expression]
        /// 		pop a1
        ///			cmp a1, 0
        ///			jmp_eq exit
        ///         [statement block]
        ///         [increment]
        ///         jmp start
        ///     
		/// </summary>
		private void ParseFor()
		{
            // Check we are in a valid scope.
            if (_currentScope.Type != SymbolType.Function || _currentScope == _globalScope)
                Error(ErrorCode.InvalidScope, "For statements are only valid inside a function's or event's scope.", false, 0);

            // Declare some important variables.
            JumpTargetSymbol startJumpTarget = null;
            JumpTargetSymbol finishJumpTarget = null;
            JumpTargetSymbol incrementJumpTarget = null;
            Instruction instruction = null;

            // Create jump targets if we are in pass 2.
            if (_currentPass == 1)
            {
                startJumpTarget = new JumpTargetSymbol(_currentScope);
                incrementJumpTarget = new JumpTargetSymbol(_currentScope);
                finishJumpTarget = new JumpTargetSymbol(_currentScope);
            }

            // Read in the variable declaration.
            ExpectToken(TokenID.CharOpenParenthesis);
            NextToken();
            if (_currentToken.ID == TokenID.KeywordBool || _currentToken.ID == TokenID.KeywordByte ||
                _currentToken.ID == TokenID.KeywordDouble || _currentToken.ID == TokenID.KeywordObject ||
                _currentToken.ID == TokenID.KeywordFloat || _currentToken.ID == TokenID.KeywordInt ||
                _currentToken.ID == TokenID.KeywordLong || _currentToken.ID == TokenID.KeywordShort ||
                _currentToken.ID == TokenID.KeywordString || _currentToken.ID == TokenID.KeywordVoid ||
                _currentToken.ID == TokenID.KeywordConst || _currentToken.ID == TokenID.KeywordStatic)
                ParseVariable();

            // Its not a variable declaration so read it in as a assignment.
            else
            {
                ParseAssignment();
                ExpectToken(TokenID.CharSemiColon);
            }

            // Bind the starting target.
            if (_currentPass == 1)
                startJumpTarget.Bind();

            // Parse the expression block.
            DataTypeValue expressionType = ParseExpression();
            ExpectToken(TokenID.CharSemiColon);

            // Create the expression evaluation instruction's.
            if (_currentPass == 1)
            {
                // Pop the result of the expression into arith register 1.
                instruction = CreateInstruction(OpCodeByType(expressionType, OpCodeType.POP), _currentScope, _currentToken);
                new Operand(instruction, Register.Arithmetic1);

                // Cast it to boolean if its not already boolean.
                if (expressionType != new DataTypeValue(DataType.Bool, false, false))
                {
                    instruction = CreateInstruction(OpCode.CAST_BOOL, _currentScope, _currentToken);
                    new Operand(instruction, Register.Arithmetic1);
                }

                // Compare the result of the expression to false.
                instruction = CreateInstruction(OpCode.CMP_BOOL, _currentScope, _currentToken);
                new Operand(instruction, Register.Arithmetic1);
                new Operand(instruction, false);

                // If its equal jump out of this loop.
                instruction = CreateInstruction(OpCode.JMP_EQ, _currentScope, _currentToken);
                new Operand(instruction, finishJumpTarget);
            }

            // Skip the increment and parse the block.
            int incrementTokenIndex = _tokenIndex;
            Token incrementToken = _currentToken;
            while (EndOfTokenStream() == false && LookAheadToken().ID != TokenID.CharCloseParenthesis)
                NextToken();
            NextToken(); // Read in closing brace.

            // Push a loop tracker into the tracker stack.
			if (_currentPass == 1)
				_loopTrackerStack.Push(new LoopTracker(incrementJumpTarget, finishJumpTarget));

			// Parse the loop's body.
			ParseStatement();

            // Pop the loop tracker of the tracker state.
			if (_currentPass == 1)
				_loopTrackerStack.Pop();

            // Note down the current token index / token.
            int finishTokenIndex = _tokenIndex;
            Token finishToken = _currentToken;

            // Go back to the increment block and parse it.
            _tokenIndex = incrementTokenIndex;
            _currentToken = incrementToken;
            NextToken();

            // Bind the increment jump target.
            if (_currentPass == 1)
                incrementJumpTarget.Bind();

            // Parse the increment.
            ParseAssignment();

            // Go back to the end.
            _tokenIndex = finishTokenIndex;
            _currentToken = finishToken;

			// Jump to the start block.
            if (_currentPass == 1)
            {
                instruction = CreateInstruction(OpCode.JMP, _currentScope, _currentToken);
                new Operand(instruction, startJumpTarget);
            }

            // Bind the finishing target.
            if (_currentPass == 1)
                finishJumpTarget.Bind();
		}

		/// <summary>
		///		Parses a switch statement, a switch statement is essentially a 
		///		nicer way of write a list of if/elseif blocks.
		///		Syntax:
		///			"Switch" "(" Expression ")" "{" { [ "case" Expresion ":" Statement ] [ "default" ":" Statement ] } "}" 
		///		
		///		Output byte code example:
		///			[expression]
		///			pop a1
		///			
		///			[expression]
		///			pop a2
		///			cmp a2, a1
		///			jmp_ne case1exit
		///			[block]
		///			jmp exit	
		///		case1exit:
		/// 
		///			[expression]
		///			pop a2
		///			cmp a2, a1
		///			jmp_ne case2exit
		///			[block]
		///			jmp exit
		///		case2exit:
		///		
		///		default:
		///			jmp exit
		/// 
		///		exit:
		///			
		/// </summary>
		private void ParseSwitch()
		{
			// Check we are in a valid scope.
			if (_currentScope.Type != SymbolType.Function || _currentScope == _globalScope)
				Error(ErrorCode.InvalidScope, "Switch statements are only valid inside a function's or event's scope.", false, 0);

			// Declare variables used within this function.
			JumpTargetSymbol defaultJumpTarget = null, exitJumpTarget = null;
			JumpTargetSymbol exitCaseJumpTarget = null;
			Instruction instruction = null;

			// If we are in pass 2 then create the jump targets.
			if (_currentPass == 1)
			{
				defaultJumpTarget = new JumpTargetSymbol(_currentScope);
				exitJumpTarget = new JumpTargetSymbol(_currentScope);
			}

			// Read in the main expression.
			ExpectToken(TokenID.CharOpenParenthesis);
			DataTypeValue expressionDataType = ParseExpression();
			ExpectToken(TokenID.CharCloseParenthesis);

			// Pop the expression into reserved register 3.
			if (_currentPass == 1)
			{
				instruction = CreateInstruction(OpCodeByType(expressionDataType, OpCodeType.POP), _currentScope, _currentToken);
				new Operand(instruction, Register.Reserved3);
			}

			// Read in opening brace.
			ExpectToken(TokenID.CharOpenBrace);

			// Create and push a new loop tracker into the tracker stack.
			if (_currentPass == 1)
				_loopTrackerStack.Push(new LoopTracker(null, exitJumpTarget));

			// Keep looping until we find the end of this switch statement.
			bool foundDefault = false;
			while (true)
			{
				// If the next token is a closing brace then exit loop.
				if (LookAheadToken().ID == TokenID.CharCloseBrace)
					break;

				// Read in a case block.
				if (LookAheadToken().ID == TokenID.KeywordCase)
				{
					NextToken(); // Read in the case keyword.
					
					// Create a exit-case jump target.
					if (_currentPass == 1) exitCaseJumpTarget = new JumpTargetSymbol(_currentScope);

					// Read in the case expression.
					DataTypeValue caseResultDataType = ParseExpression();

					// Cast the value into the correct type.
					if (CanImplicitlyCast(expressionDataType, caseResultDataType) == false)
						Error(ErrorCode.InvalidCast, "Can't implicitly cast between \"" + expressionDataType.ToString() + "\" and \"" + caseResultDataType.ToString() + "\"");

					if (_currentPass == 1)
					{
						// Pop the value into arith register 1.
						instruction = CreateInstruction(OpCodeByType(caseResultDataType, OpCodeType.POP), _currentScope, _currentToken);
						new Operand(instruction, Register.Arithmetic1);

						// Cast to the correct type if its not already it.
						if (expressionDataType != caseResultDataType)
						{
							instruction = CreateInstruction(OpCodeByType(expressionDataType, OpCodeType.CAST), _currentScope, _currentToken);
							new Operand(instruction, Register.Arithmetic1);
						}

						// Compare the expression and this case's expression.
						instruction = CreateInstruction(OpCodeByType(expressionDataType, OpCodeType.CMP), _currentScope, _currentToken);
						new Operand(instruction, Register.Reserved3);
						new Operand(instruction, Register.Arithmetic1);

						// If its not equal to the expression then jump to the case
						// exit jump target.
						instruction = CreateInstruction(OpCode.JMP_NE, _currentScope, _currentToken);
						new Operand(instruction, exitCaseJumpTarget);
					}

					ExpectToken(TokenID.CharColon); // Read in the colon.

					// Parse the case statement.
					ParseStatement();

					if (_currentPass == 1)
					{
						// Jump to the default block.
						instruction = CreateInstruction(OpCode.JMP, _currentScope, _currentToken);
						new Operand(instruction, exitJumpTarget);
	
						// Bind the case exit jump target.
						exitCaseJumpTarget.Bind();
					}
				}

				// Its not a case so check if its a default block.
				if (LookAheadToken().ID == TokenID.KeywordDefault)
				{
					NextToken(); // Read in the deault keyword.
					ExpectToken(TokenID.CharColon); // Read in the colon.

					// Check we haven't already found a default block.
					if (foundDefault == true) Error(ErrorCode.DuplicateDefault, "Switch statements can't contain duplicate default blocks.");
					foundDefault = true;

					if (_currentPass == 1) 
					{
						// Create a exit-case jump target.
						exitCaseJumpTarget = new JumpTargetSymbol(_currentScope);

						// Jump past the default block.
						instruction = CreateInstruction(OpCode.JMP, _currentScope, _currentToken);
						new Operand(instruction, exitCaseJumpTarget);

						// Bind the default jump target.
						defaultJumpTarget.Bind();
					}

					// Parses the default block's statement.
					ParseStatement();

					if (_currentPass == 1)  
					{
						// Jump to the exit jump target.
						instruction = CreateInstruction(OpCode.JMP, _currentScope, _currentToken);
						new Operand(instruction, exitJumpTarget);

						// Bind the case exit jump target.
						exitCaseJumpTarget.Bind();
					}
				}
			}

			// Read in closing brace.
			ExpectToken(TokenID.CharCloseBrace);

			// Pop the loop tracker off the tracker stack.
			if (_currentPass == 1)
				_loopTrackerStack.Pop();

			if (_currentPass == 1)
			{
				// Jump to the default block if its defined.
				if (foundDefault == true)
				{
					instruction = CreateInstruction(OpCode.JMP, _currentScope, _currentToken);
					new Operand(instruction, defaultJumpTarget);
				}

				// Bind the exit jump target.
				exitJumpTarget.Bind();
			}

		}

		/// <summary>
		///		Parses a gotostate statement, which is used to force the 
		///		execution context into the given state.
		///		Syntax:
		///			"GotoState" Identifier ";"
		/// </summary>
		private void ParseGotoState()
		{
			// Check we are in a valid scope.
			if (_currentScope.Type != SymbolType.Function || _currentScope == _globalScope)
				Error(ErrorCode.InvalidScope, "GotoState statements are only valid inside a function's or event's scope.", false, 0);

			// Read in the states identifier.
			ExpectToken(TokenID.TypeIdentifier);

			// If we are in pass 2 generate the code to switch the state.
			if (_currentPass == 1)
			{
				// Find the states symbol.
				StateSymbol symbol = _globalScope.FindSymbol(_currentToken.Ident, SymbolType.State) as StateSymbol;
                if (symbol == null)
                    Error(ErrorCode.UndeclaredVariable, "Unable to access undeclared state '"+_currentToken.Ident+"'.", false, 1);

				// Generate the state change instruction.
				Instruction instruction = CreateInstruction(OpCode.GOTO_STATE, _currentScope, _currentToken);
				new Operand(instruction, symbol);
			}

			// Read in the semi-colon that ends this statement.
			ExpectToken(TokenID.CharSemiColon);
		}

		/// <summary>
		///		Parses a goto statement, which willjump execution to a specific label.
		///		Syntax:
		///			Goto Identifier ";"
		/// </summary>
		private void ParseGoto()
		{
			// Check we are in a valid scope.
			if (_currentScope.Type != SymbolType.Function || _currentScope == _globalScope)
				Error(ErrorCode.InvalidScope, "Goto statments are only valid inside a function's or event's scope.", false, 0);

			// Read in the labels identifier.
			ExpectToken(TokenID.TypeIdentifier);

			// Generate the jumping byte code.
			if (_currentPass == 1)
			{
				// Find the jump target we have been told to go to.
				JumpTargetSymbol jumpTarget = _currentScope.FindSymbol(_currentToken.Ident, SymbolType.JumpTarget) as JumpTargetSymbol;

				// Generate jump instruction
				Instruction instruction = CreateInstruction(OpCode.JMP, _currentScope, _currentToken);
				new Operand(instruction, jumpTarget);
			}

			// Read in the semi-colon that ends this statement.
			ExpectToken(TokenID.CharSemiColon);

		}

		/// <summary>
		///		Parses a label, which is used in conjunction with the goto statement to jump 
		///		to specific areas in the code.
		///		Syntax:
		///			Identifier ":"
		/// </summary>
		private void ParseLabel()
		{
			// Check we are in a valid scope.
			if (_currentScope.Type != SymbolType.Function || _currentScope == _globalScope)
				Error(ErrorCode.InvalidScope, "Label statments are only valid inside a function's or event's scope.", false, 0);

			// Create a jump target for this label.
			JumpTargetSymbol jumpTarget = null;
			if (_currentPass == 0)
			{
				jumpTarget = new JumpTargetSymbol(_currentScope);
				jumpTarget.Identifier = _currentToken.Ident;
			}
			else if (_currentPass == 1)
			{
				jumpTarget = _currentScope.FindSymbol(_currentToken.Ident, SymbolType.JumpTarget) as JumpTargetSymbol;
				jumpTarget.Bind();
			}

			// Read in the colon that ends this label.
			ExpectToken(TokenID.CharColon);
		}

		/// <summary>
		///		Parses a break statement, which generates code to break out of a given amount of loops.
		///		Syntax:
		///			break [ "(" Integer ")" ] ";"
		/// </summary>
		private void ParseBreak()
		{
			// Check we are in a valid scope.
			if (_currentScope.Type != SymbolType.Function || _currentScope == _globalScope)
				Error(ErrorCode.InvalidScope, "Break statments are only valid inside a function's or event's scope.", false, 0);

			// Check for index of loop to jump out of.
			int loopIndex = 1;
			if (LookAheadToken().ID == TokenID.CharOpenParenthesis)
			{
				NextToken();
				ExpectToken(TokenID.TypeInteger);
				loopIndex = int.Parse(_currentToken.Ident);
				ExpectToken(TokenID.CharCloseParenthesis);
			}

			// If we are in pass 2 grab the loop tracker and generate
			// byte code to jump out of loop.
			if (_currentPass == 1)
			{
				Object[] trackerArray = _loopTrackerStack.ToArray();
				if (loopIndex - 1 > trackerArray.Length - 1)
					Error(ErrorCode.InvalidLoopIndex, "Loop depth is not high enough to break \"" + loopIndex + "\" loops.");

				LoopTracker loopTracker = (LoopTracker)trackerArray[loopIndex - 1];
				if (loopTracker.EndJumpTarget == null)
					Error(ErrorCode.InvalidBreak, "Break is not valid in this type of loop.");

				Instruction instruction = CreateInstruction(OpCode.JMP, _currentScope, _currentToken);
				new Operand(instruction, loopTracker.EndJumpTarget);
			}

			// Read in semi-colon at end of declaration.
			ExpectToken(TokenID.CharSemiColon);
		}

		/// <summary>
		///		Parses a continue statement, which generates byte code to jump to the start of a given loop.
		///		Syntax:
		///			continue [ "(" Integer ")" ] ";"
		/// </summary>
		private void ParseContinue()
		{
			// Check we are in a valid scope.
			if (_currentScope.Type != SymbolType.Function || _currentScope == _globalScope)
				Error(ErrorCode.InvalidScope, "Continue statments are only valid inside a function's or event's scope.", false, 0);

			// Check for index of loop to jump out of.
			int loopIndex = 1;
			if (LookAheadToken().ID == TokenID.CharOpenParenthesis)
			{
				NextToken();
				ExpectToken(TokenID.TypeInteger);
				loopIndex = int.Parse(_currentToken.Ident);
				ExpectToken(TokenID.CharCloseParenthesis);
			}

			// If we are in pass 2 grab the loop tracker and generate
			// byte code to jump out of loop.
			if (_currentPass == 1)
			{
				Object[] trackerArray = _loopTrackerStack.ToArray();
				if (loopIndex - 1 > trackerArray.Length - 1)
					Error(ErrorCode.InvalidLoopIndex, "Loop depth is not high enough to continue \""+loopIndex+"\" loops.");

				LoopTracker loopTracker = (LoopTracker)trackerArray[loopIndex - 1];
				if (loopTracker.StartJumpTarget == null)
					Error(ErrorCode.InvalidContinue, "Continue is not valid in this type of loop.");

				Instruction instruction = CreateInstruction(OpCode.JMP, _currentScope, _currentToken);
				new Operand(instruction, loopTracker.StartJumpTarget);
			}

			// Read in semi-colon at end of declaration.
			ExpectToken(TokenID.CharSemiColon);
		}

		/// <summary>
		///		Parses a flow-of-control if statement, which will preform the given action
		///		if the expression evaluates to true. Optional Else and ElseIf block's will be
		///		preformed if expression evaluates to false.
		///		Syntax:
		///			"if" "(" Expression ")" Statement { [ "elseif" Block ] } [ "else" Block ] 
		/// </summary>
		private void ParseIf()
		{
			// Check we are in a valid scope.
			if (_currentScope.Type != SymbolType.Function || _currentScope == _globalScope)
				Error(ErrorCode.InvalidScope, "If statments are only valid inside a function's or event's scope.", false, 0);

			// Generate a false jump target.
			Instruction instruction = null;
			JumpTargetSymbol falseJumpTarget = null;
			JumpTargetSymbol skipFalseJumpTarget = null;

			// Parse the expression.
			ExpectToken(TokenID.CharOpenParenthesis);
			DataTypeValue expressionDataType = ParseExpression();
			ExpectToken(TokenID.CharCloseParenthesis);

			// Make sure we can cast between the expression type and boolean.
			if (!CanImplicitlyCast(new DataTypeValue(DataType.Bool, false, false), expressionDataType))
				Error(ErrorCode.InvalidCast, "Can't implicitly cast between \"Bool\" and \"" + expressionDataType.ToString() + "\"", false, 0);

			// Generate comparison byte code.
			if (_currentPass == 1)
			{
				// Generate false jump target
				falseJumpTarget = new JumpTargetSymbol(_currentScope);

				// Pop the result into arith register 1
				instruction = CreateInstruction(OpCodeByType(expressionDataType, OpCodeType.POP), _currentScope, _currentToken);
				new Operand(instruction, Register.Arithmetic1);

				// Cast it to boolean if its not already boolean
				if (expressionDataType != new DataTypeValue(DataType.Bool, false, false))
				{
					instruction = CreateInstruction(OpCode.CAST_BOOL,_currentScope,_currentToken);
					new Operand(instruction, Register.Arithmetic1);
				}

				// Compare result with 0
				instruction = CreateInstruction(OpCode.CMP_BOOL, _currentScope, _currentToken);
				new Operand(instruction, Register.Arithmetic1);
				new Operand(instruction, false);

				// Jump to false jump target if comparison is equal.
				instruction = CreateInstruction(OpCode.JMP_EQ, _currentScope, _currentToken);
				new Operand(instruction, falseJumpTarget);
			}

			// Parse the true statement.
			ParseStatement();

			// Check for the else keyword.
			if (LookAheadToken().ID == TokenID.KeywordElse)
			{
				// Read in the else keyword.
				NextToken();

				if (_currentPass == 1)
				{
					// Append an unconditional jump past the false 
					// block to the true block.
					skipFalseJumpTarget = new JumpTargetSymbol(_currentScope);
					instruction = CreateInstruction(OpCode.JMP, _currentScope, _currentToken);
					new Operand(instruction, skipFalseJumpTarget);

					// Bind the false jump target.
					falseJumpTarget.Bind();
				}

				// Parse the else statement.
				ParseStatement();

				if (_currentPass == 1) skipFalseJumpTarget.Bind();
			}
			else
				if (_currentPass == 1) falseJumpTarget.Bind();
		}

		/// <summary>
		///		Parses a return statement, which will break out of a function call.
		///		Syntax:
		///			return [ Expression ] ";"
		/// </summary>
		private void ParseReturn()
		{
			// Check we are in a valid scope.
			if (_currentScope.Type != SymbolType.Function || _currentScope == _globalScope)
				Error(ErrorCode.InvalidScope, "Returns are only valid inside a function's or event's scope.", false, 0);

			if (_currentScope.Type == SymbolType.Function)
			{
				FunctionSymbol functionSymbol = (FunctionSymbol)_currentScope;

				// Check if there is an return expression after this.
				if (LookAheadToken().ID != TokenID.CharSemiColon)
				{
					// Check that the function is expecting a return value.
					if (functionSymbol.ReturnType.DataType == DataType.Void)
						Error(ErrorCode.ExpectingReturnValue, "Void function can't return a value.", false, 0);

					// Parse the return value's expression.
					DataTypeValue returnValueDataType = ParseExpression();

					// Check the return value can be implicitly cast to the given type.
					if (CanImplicitlyCast(functionSymbol.ReturnType, returnValueDataType) == true)
					{
						if (_currentPass == 1)
						{
							// Pop the return value into the return register.
							Instruction instruction = CreateInstruction(OpCodeByType(returnValueDataType, OpCodeType.POP), _currentScope, _currentToken);
							Operand op1 = new Operand(instruction, Register.Return);

							// Cast return value into correct type.
							if (functionSymbol.ReturnType != returnValueDataType)
							{
								instruction = CreateInstruction(OpCodeByType(functionSymbol.ReturnType, OpCodeType.CAST), _currentScope, _currentToken);
								new Operand(instruction, Register.Return);
							}
						}
					}
					else
						Error(ErrorCode.InvalidCast, "Can't implicitly cast between \"" + functionSymbol.ReturnType.ToString() + "\" and \"" + returnValueDataType.ToString() + "\"", false, 0);
				}
				else
				{
					// Check if we need a return value or not.
					if (functionSymbol.ReturnType.DataType != DataType.Void)
						Error(ErrorCode.ExpectingReturnValue, "Function expects return value.", false, 0);
					
					if (_currentPass == 1)
					{
						// Nullify the return register incase it contains a 
						// value from a previous function call.
						Instruction instruction = CreateInstruction(OpCode.CAST_NULL, functionSymbol, _currentToken);
						new Operand(instruction, Register.Return);
					}
				}

				// Create return instruction.
				if (_currentPass == 1) CreateInstruction(OpCode.RETURN, _currentScope, _currentToken);
			}

			// Read in semi-colon at end of declaration.
			ExpectToken(TokenID.CharSemiColon);

		}

		/// <summary>
		///		Parses a looping expression that is dependent on an expression
		///		evaluating to true.
		///		Syntax:
		///			"While" "{" Expression "}" Statement
		/// </summary>
		private void ParseWhile()
		{
			// Check we are in a valid scope.
			if (_currentScope.Type != SymbolType.Function || _currentScope == _globalScope)
				Error(ErrorCode.InvalidScope, "While statments are only valid inside a function's or event's scope.", false, 0);

			// Create some jump target symbols.
			JumpTargetSymbol startJumpTarget = null, endJumpTarget = null;
			Instruction instruction;

			// Bind the starting jump target to here.
			if (_currentPass == 1)
			{
				startJumpTarget = new JumpTargetSymbol(_currentScope);
				endJumpTarget = new JumpTargetSymbol(_currentScope);
			}

			// Read in the expression to evaluate
			ExpectToken(TokenID.CharOpenParenthesis);
			DataTypeValue expressionDataType = ParseExpression();
			ExpectToken(TokenID.CharCloseParenthesis);

			// Make sure we can cast between the expression type and boolean.
			if (!CanImplicitlyCast(new DataTypeValue(DataType.Bool, false, false), expressionDataType))
				Error(ErrorCode.InvalidCast, "Can't implicitly cast between \"Bool\" and \"" + expressionDataType.ToString() + "\"", false, 0);

			// Emit evaluation byte code.
			if (_currentPass == 1)
			{
				// Pop the result of the expression into arith register 1.
				instruction = CreateInstruction(OpCodeByType(expressionDataType, OpCodeType.POP), _currentScope, _currentToken);
				new Operand(instruction, Register.Arithmetic1);

				// Cast it to boolean if its not already boolean
				if (expressionDataType != new DataTypeValue(DataType.Bool, false, false))
				{
					instruction = CreateInstruction(OpCode.CAST_BOOL, _currentScope, _currentToken);
					new Operand(instruction, Register.Arithmetic1);
				}

				// Compare the expression result to 0.
				instruction = CreateInstruction(OpCode.CMP_BOOL, _currentScope, _currentToken);
				new Operand(instruction, Register.Arithmetic1);
				new Operand(instruction, false);

				// Jump to finish jump target if comparison is equal to false.
				instruction = CreateInstruction(OpCode.JMP_EQ, _currentScope, _currentToken);
				new Operand(instruction, endJumpTarget);

				// Push a new loop tracker onto the loop stack.
				_loopTrackerStack.Push(new LoopTracker(startJumpTarget, endJumpTarget));
			}

			// Parse the statements contained in this while block.
			ParseStatement();

			// Emit finishing byte code.
			if (_currentPass == 1)
			{
				// Pop the loop tracker of the loop stack.
				_loopTrackerStack.Pop();

				// Jump to the start of the loop.
				instruction = CreateInstruction(OpCode.JMP, _currentScope, _currentToken);
				new Operand(instruction, startJumpTarget);

				// Bind exit jump target.
				endJumpTarget.Bind();
			}
		}

		/// <summary>
		///		Parses a do looping statment. The do loop comes in 2 flavours do and do/while; The do 
		///		loop will loop through the statements body for x amount of times, where x is the result
		///		of the expression. The Do/While loop works just like the While loop but gets evaluated
		///		at the end rather than the start.
		///		Syntax:
		///			"Do" ["(" Expression ")"] Statement [ "While" "(" Expression ")" ]
		/// </summary>
		private void ParseDo()
		{
			// Check we are in a valid scope.
			if (_currentScope.Type != SymbolType.Function || _currentScope == _globalScope)
				Error(ErrorCode.InvalidScope, "Do statments are only valid inside a function's or event's scope.", false, 0);

			bool whileLoop = true;
			VariableSymbol doTrackerVariable = null;
			Instruction instruction = null;

			// Bind the starting jump target.
			JumpTargetSymbol startJumpTarget = null, endJumpTarget = null;
			if (_currentPass == 1)
			{
				startJumpTarget = new JumpTargetSymbol(_currentScope);
				endJumpTarget = new JumpTargetSymbol(_currentScope);   
			}

			// Check if there is an expression next, if there is its a do loop.
			if (LookAheadToken().ID == TokenID.CharOpenParenthesis)
			{
				// Create an internal variable to count number of loops.
				if (_currentPass == 0)
				{
					doTrackerVariable = new VariableSymbol(_currentScope);
					doTrackerVariable.Identifier = "$" + _internalVariableIndex++;
					doTrackerVariable.IsUsed = true;
					doTrackerVariable.DataType = new DataTypeValue(DataType.Int, false, false);
					doTrackerVariable.VariableType = VariableType.Local;
					if (_currentScope.Type == SymbolType.Function)
					{
						doTrackerVariable.StackIndex = -(((FunctionSymbol)_currentScope).LocalDataSize + 2);
						((FunctionSymbol)_currentScope).LocalDataSize++;
					}
				}
				else
				{
					doTrackerVariable = _currentScope.FindSymbol("$" + _internalVariableIndex++, SymbolType.Variable) as VariableSymbol;

					// Reset the tracker variable to 0.
					instruction = CreateInstruction(OpCode.MOV_INT, _currentScope, _currentToken);
					Operand.DirectStackOperand(instruction, doTrackerVariable.StackIndex);
					new Operand(instruction, (long)0);
				}

				// Bind the jump target to here.
				if (_currentPass == 1) startJumpTarget.Bind();

				// Read in do expression.
				whileLoop = false;
				NextToken();
				DataTypeValue expressionDataType = ParseExpression();
				ExpectToken(TokenID.CharCloseParenthesis);

				// Make sure we can cast between the expression type and boolean.
				if (!CanImplicitlyCast(new DataTypeValue(DataType.Int, false, false), expressionDataType))
					Error(ErrorCode.InvalidCast, "Can't implicitly cast between \"Int\" and \"" + expressionDataType.ToString() + "\"", false, 0);

				// If we are in pass 2 emit byte code to check value.
				if (_currentPass == 1)
				{
					// Pop the amount of times to loop into the reserved register 1.
					instruction = CreateInstruction(OpCodeByType(expressionDataType, OpCodeType.POP), _currentScope, _currentToken);
					new Operand(instruction, Register.Reserved1);

					// If the value is not a long then cast it into one.
					if (expressionDataType != new DataTypeValue(DataType.Int, false, false))
					{
						instruction = CreateInstruction(OpCode.CAST_INT, _currentScope, _currentToken);
						new Operand(instruction, Register.Reserved1);
					}

					// Compare the loop tracker to the number of times to loop.
					instruction = CreateInstruction(OpCode.CMP_INT, _currentScope, _currentToken);
					Operand.DirectStackOperand(instruction, doTrackerVariable.StackIndex);
					new Operand(instruction, Register.Reserved1);

					// If its above or equal to the number of times we want to loop for, then exit.
					instruction = CreateInstruction(OpCode.JMP_GE, _currentScope, _currentToken);
					new Operand(instruction, endJumpTarget);

					// Increment the loop tracker variable.
					instruction = CreateInstruction(OpCode.INC_INT, _currentScope, _currentToken);
					Operand.DirectStackOperand(instruction, doTrackerVariable.StackIndex);
				}
			}
			else
			{
				// Bind the jump target to here.
				if (_currentPass == 1) startJumpTarget.Bind();
			}

			// Push a loop tracker onto the loop stack.
			if (_currentPass == 1)
				_loopTrackerStack.Push(new LoopTracker(startJumpTarget, endJumpTarget));

			// Parse the body of this statement.
			bool indexerLoop = _insideIndexerLoop;
			int indexerIndex = _indexerLoopIndex;
			if (whileLoop == false)
			{
				_insideIndexerLoop = true;
				_indexerLoopIndex = _internalVariableIndex - 1;
			}
			ParseStatement();
			_insideIndexerLoop = indexerLoop;
			_indexerLoopIndex = indexerIndex;

			// Pop the loop tracker of the loop stack
			if (_currentPass == 1)
				_loopTrackerStack.Pop();

			// If its a while loop then read in the while expression and output evaluation code.
			if (whileLoop == true)
			{
				// Parse the expression.
				ExpectToken(TokenID.KeywordWhile);
				ExpectToken(TokenID.CharOpenParenthesis);
				DataTypeValue expressionDataType = ParseExpression();
				ExpectToken(TokenID.CharCloseParenthesis);

				if (_currentPass == 1)
				{
					// Pop the result of the expression into arith register 1.
					instruction = CreateInstruction(OpCodeByType(expressionDataType, OpCodeType.POP), _currentScope, _currentToken);
					new Operand(instruction, Register.Arithmetic1);

					// Cast it to boolean if its not already boolean
					if (expressionDataType != new DataTypeValue(DataType.Bool, false, false))
					{
						instruction = CreateInstruction(OpCode.CAST_BOOL, _currentScope, _currentToken);
						new Operand(instruction, Register.Arithmetic1);
					}

					// Compare the expression result to 0.
					instruction = CreateInstruction(OpCode.CMP_BOOL, _currentScope, _currentToken);
					new Operand(instruction, Register.Arithmetic1);
					new Operand(instruction, false);

					// Jump to finish jump target if comparison is equal to false.
					instruction = CreateInstruction(OpCode.JMP_EQ, _currentScope, _currentToken);
					new Operand(instruction, endJumpTarget);
				}
			}

			// Create an instruction to jump back to the start of the loop.
			if (_currentPass == 1)
			{
				instruction = CreateInstruction(OpCode.JMP, _currentScope, _currentToken);
				new Operand(instruction, startJumpTarget);

				// Bind the end jump target to here.
				endJumpTarget.Bind();
			}
		}

		/// <summary>
		///		Parses an variable assignment. A variable assignment basically just
		///		places a given value into a previously declared variables memory space.
		///		Syntax:
		///			Identifier AssignmentOperator Expression
		/// </summary>
		private void ParseAssignment()
		{
			// Check we are in a valid scope.
			if (_currentScope.Type != SymbolType.Function || _currentScope == _globalScope)
				Error(ErrorCode.InvalidScope, "Assignments are only valid inside a function's or event's scope.", false, 0);

			// Retrieve the variables identifier and save it for later on.
            Symbol resolvedScope = ResolveMemberScope();
            string variableIdentifier = _currentToken.Ident;

			#region Variable retrieving
			// If we are in pass 2 try and retrieve the variable.
			VariableSymbol variableSymbol = null;
			DataTypeValue dataType = new DataTypeValue(DataType.Invalid, false, false);
			if (_currentPass == 1)
			{
                variableSymbol = resolvedScope.FindSymbol(variableIdentifier, SymbolType.Variable) as VariableSymbol;
				if (variableSymbol == null)
					Error(ErrorCode.UndeclaredVariable, "Encountered attempt to assign a value to an undeclared variable \"" + variableIdentifier + "\".");
				if (variableSymbol.VariableType == VariableType.Constant)
					Error(ErrorCode.IllegalAssignment, "Encountered attempt to assign to a constant variable \"" + variableIdentifier + "\".");
				variableSymbol.IsUsed = true;
				dataType.DataType = variableSymbol.DataType.DataType;
				dataType.IsArray = variableSymbol.DataType.IsArray;
				dataType.IsReference = variableSymbol.DataType.IsReference;
			}
			#endregion
			#region Array index parsing
			// Check if we are assigning to an array.
			bool isArray = false;
			DataTypeValue indexDataType = null;
			if (LookAheadToken().ID == TokenID.CharOpenBracket)
			{
				// Make sure variable is an array if we are in pass 2.
				if (_currentPass == 1 && variableSymbol.IsArray == false)
					Error(ErrorCode.InvalidArrayIndex, "Non-array variables can not be indexed.");

				// As its an array and its index we can remove the array declaration from
				// the data type.
				dataType.IsArray = false;

				// Read in opening bracket and check expression is valid
				NextToken();
				if (LookAheadToken().ID == TokenID.CharCloseBracket)
					Error(ErrorCode.InvalidArrayIndex, "Array's must be indexed.", false, 0);

				// Read in expression and closing bracket.
				indexDataType = ParseExpression();
				ExpectToken(TokenID.CharCloseBracket);

				// Check the expression is of a valid data type.
				if (CanImplicitlyCast(new DataTypeValue(DataType.Int, false, false), indexDataType) == false)
					Error(ErrorCode.InvalidCast, "Can't implicitly cast between \"Int\" and \"" + indexDataType.ToString() + "\"");

				isArray = true;
			}
			#endregion
			#region Operator parsing
			// Check if the operator is a valid assignment operator.
			NextToken();
			if (_currentToken.ID != TokenID.OpAssign && _currentToken.ID != TokenID.OpAssignAdd &&
				_currentToken.ID != TokenID.OpAssignBitwiseAnd && _currentToken.ID != TokenID.OpAssignBitwiseNot &&
				_currentToken.ID != TokenID.OpAssignBitwiseOr && _currentToken.ID != TokenID.OpAssignBitwiseSHL &&
				_currentToken.ID != TokenID.OpAssignBitwiseSHR && _currentToken.ID != TokenID.OpAssignBitwiseXOr &&
				_currentToken.ID != TokenID.OpAssignDivide && _currentToken.ID != TokenID.OpAssignModulus &&
				_currentToken.ID != TokenID.OpAssignMultiply && _currentToken.ID != TokenID.OpAssignSub &&
				_currentToken.ID != TokenID.OpIncrement && _currentToken.ID != TokenID.OpDecrement)
				Error(ErrorCode.IllegalAssignmentOperator, "Encountered attempt to use an illegal assignment operator.");

			Token operatorToken = _currentToken;
			Instruction instruction = null;
			DataTypeValue expressionType = new DataTypeValue(DataType.Invalid, false, false);
			#endregion

			// Check the data types are valid with this operator.
			if (_currentPass == 1 && OperatorDataTypeValid(variableSymbol.DataType, operatorToken.ID) == false)
				Error(ErrorCode.InvalidDataType, "Operator \"" + operatorToken.Ident + "\" can't be applied to data type \"" + variableSymbol.DataType.ToString() + "\"");

			#region Value byte code emission and parsing
			// Read in expression if it is not a increment (++) or decrement (--) operator.
			if (operatorToken.ID != TokenID.OpIncrement && operatorToken.ID != TokenID.OpDecrement)
			{
				// Parse the expression. 
				expressionType = ParseExpression();

				// Pop the expression into reserved register 3.
				if (_currentPass == 1)
				{
					// If we are an array deallocate our previous memory.
					/*
					if (variableSymbol.DataType.IsArray == true)
					{
						// Move the memory index into arith 2.
						instruction = CreateInstruction(OpCode.MOV_MEMORY_INDEX, _currentScope, _currentToken);
						new Operand(instruction, Register.Arithmetic2);
						if (variableSymbol.VariableType == VariableType.Constant || variableSymbol.VariableType == VariableType.Global)
							Operand.DirectMemoryOperand(instruction, variableSymbol.MemoryIndex);
						else
							Operand.DirectStackOperand(instruction, variableSymbol.StackIndex);

						// Allocate enough space on the heap for the array.
						instruction = CreateInstruction(OpCode.DEALLOCATE_HEAP, _currentScope, _currentToken);
						new Operand(instruction, Register.Arithmetic2);
					}
					*/

					// Pop the value of into reserved register 3.
					instruction = CreateInstruction(OpCodeByType(expressionType, OpCodeType.POP), _currentScope, _currentToken);
					new Operand(instruction, Register.Reserved3);

					// Cast the value to a valid type.
					if (dataType != expressionType)
					{
						instruction = CreateInstruction(OpCodeByType(dataType, OpCodeType.CAST), _currentScope, _currentToken);
						new Operand(instruction, Register.Reserved3);
					}
				}

				// If we are an array check that we have been indexed, unless we
				// are assigning null to it.
				if (_currentPass == 1 && variableSymbol.IsArray == true && isArray == false && expressionType.DataType != DataType.Null && expressionType.IsArray != true)
					Error(ErrorCode.InvalidArrayIndex, "Arrays must be indexed.");
			}
			else
			{
				expressionType = new DataTypeValue(DataType.Int, false, false);
			}
			#endregion

			// Check we can cast to the expression result to the variables data type.
			if (_currentPass == 1 && CanImplicitlyCast(dataType, expressionType) == false)
				Error(ErrorCode.InvalidCast, "Can't implicitly cast between \"" + variableSymbol.DataType.ToString() + "\" and \"" + expressionType.ToString() + "\"");

			#region Array index byte code emission
			// If this is an array pop the index into reserved register 2.
			if (isArray == true && _currentPass == 1)
			{
				// Pop the index into reserved regu=ister 2.
				instruction = CreateInstruction(OpCodeByType(indexDataType, OpCodeType.POP), _currentScope, _currentToken);
				new Operand(instruction, Register.Reserved2);

				// Cast it to an integer index if its of another type.
				if (indexDataType != new DataTypeValue(DataType.Int, false, false))
				{
					instruction = CreateInstruction(OpCode.CAST_INT, _currentScope, _currentToken);
					new Operand(instruction, Register.Reserved2);
				}

				// Move the array memory slot into reserved register 1.
				instruction = CreateInstruction(OpCode.MOV_MEMORY_INDEX, _currentScope, _currentToken);
				new Operand(instruction, Register.Reserved1);
				if (variableSymbol.VariableType == VariableType.Constant || variableSymbol.VariableType == VariableType.Global)
					Operand.DirectMemoryOperand(instruction, variableSymbol.MemoryIndex);
				else
					Operand.DirectStackOperand(instruction, variableSymbol.StackIndex);
			}
			#endregion

			#region Assignment byte code emission
			// Create assignment byte code if we are in pass 2.
			if (_currentPass == 1)
			{
				switch (operatorToken.ID)
				{
					case TokenID.OpAssign: instruction = CreateInstruction(OpCodeByType(dataType, OpCodeType.MOV), _currentScope, _currentToken); break;
					case TokenID.OpAssignAdd: instruction = CreateInstruction(OpCodeByType(dataType, OpCodeType.ADD), _currentScope, _currentToken); break;
					case TokenID.OpAssignBitwiseAnd: instruction = CreateInstruction(OpCode.BIT_AND_INT, _currentScope, _currentToken); break;
					case TokenID.OpAssignBitwiseNot: instruction = CreateInstruction(OpCode.BIT_NOT_INT, _currentScope, _currentToken); break;
					case TokenID.OpAssignBitwiseOr: instruction = CreateInstruction(OpCode.BIT_OR_INT, _currentScope, _currentToken); break;
					case TokenID.OpAssignBitwiseSHL: instruction = CreateInstruction(OpCode.BIT_SHL_INT, _currentScope, _currentToken); break;
					case TokenID.OpAssignBitwiseSHR: instruction = CreateInstruction(OpCode.BIT_SHR_INT, _currentScope, _currentToken); break;
					case TokenID.OpAssignBitwiseXOr: instruction = CreateInstruction(OpCode.BIT_XOR_INT, _currentScope, _currentToken); break;
					case TokenID.OpAssignDivide: instruction = CreateInstruction(OpCodeByType(dataType, OpCodeType.DIV), _currentScope, _currentToken); break;
					case TokenID.OpAssignModulus: instruction = CreateInstruction(OpCode.MOD_INT, _currentScope, _currentToken); break;
					case TokenID.OpAssignMultiply: instruction = CreateInstruction(OpCodeByType(dataType, OpCodeType.MUL), _currentScope, _currentToken); break;
					case TokenID.OpAssignSub: instruction = CreateInstruction(OpCodeByType(dataType, OpCodeType.SUB), _currentScope, _currentToken); break;
					case TokenID.OpIncrement: instruction = CreateInstruction(OpCodeByType(dataType, OpCodeType.INC), _currentScope, _currentToken); break;
					case TokenID.OpDecrement: instruction = CreateInstruction(OpCodeByType(dataType, OpCodeType.DEC), _currentScope, _currentToken); break;
				}

				// Generate destination operands based on if variable is array.
				if (isArray == true)
					Operand.IndirectMemoryIndexedOperand(instruction, Register.Reserved1, Register.Reserved2);
				else
				{
					if (variableSymbol.VariableType == VariableType.Global || variableSymbol.VariableType == VariableType.Constant)
						Operand.DirectMemoryOperand(instruction, variableSymbol.MemoryIndex);
					else
						Operand.DirectStackOperand(instruction, variableSymbol.StackIndex);
				}

				// Generate the source operand if we are not dealing with the ++ and -- operators.
				if (operatorToken.ID != TokenID.OpIncrement && operatorToken.ID != TokenID.OpDecrement)
					new Operand(instruction, Register.Reserved3);

			}
			#endregion
		}

		/// <summary>
		///		Parses a top level expression. The top level is responsible for parsing 
		///		any sub expressions and logical operators.
		/// </summary>
		/// <returns>The data type this expression evaluates to.</returns>
		private DataTypeValue ParseExpression()
		{
			// Parse the left hand sub expression.
			DataTypeValue subDataType1 = ParseSubExpression();

			// Parse any subsequent sub expressions that are seperated
			// by logical operators.
			while (true)
			{
				// Check that the next token is logical or relational.
				Token operatorToken = LookAheadToken();
				if (operatorToken.IsLogical == false)
					break;
				NextToken();

				// Parse the right hand sub expression.
				DataTypeValue subDataType2 = ParseSubExpression();

				// Check the new data type can be cast to the main data type
				if (CanImplicitlyCast(subDataType1, subDataType2) == true)
				{

					// Check the data types are valid with this operator.
					if (OperatorDataTypeValid(subDataType1, operatorToken.ID) == false)
						Error(ErrorCode.InvalidDataType, "Operator \"" + operatorToken.Ident + "\" can't be applied to data type \"" + subDataType1.ToString() + "\"", false, 1);

					// We only want to emit byte code on the second pass.
					if (_currentPass == 1)
					{

						// Pop the result of the second sub expression into arithmetic
						// register 1.
						Instruction instruction = CreateInstruction(OpCodeByType(subDataType2, OpCodeType.POP), _currentScope, _currentToken);
						new Operand(instruction, Register.Arithmetic2);

						// Pop the result of the first sub expression into arithmetic
						// register 2.
						instruction = CreateInstruction(OpCodeByType(subDataType1, OpCodeType.POP), _currentScope, _currentToken);
						new Operand(instruction, Register.Arithmetic1);

						#region Logical Byte Code Emiting

						// Create operation code based on operator.
						// Logical instructions only take boolean values
						// so there is no need to cast the values here.
						switch (operatorToken.ID)
						{
							case TokenID.OpLogicalAnd:
								instruction = CreateInstruction(OpCode.LOGICAL_AND, _currentScope, _currentToken);
								new Operand(instruction, Register.Arithmetic1);
								new Operand(instruction, Register.Arithmetic2);
								break;
							case TokenID.OpLogicalOr:
								instruction = CreateInstruction(OpCode.LOGICAL_OR, _currentScope, _currentToken);
								new Operand(instruction, Register.Arithmetic1);
								new Operand(instruction, Register.Arithmetic2);
								break;
						}

						#endregion
					}

					// Data type is now boolean due to the comparison operators.
					subDataType1 = new DataTypeValue(DataType.Bool, false, false);

				}
				else
					Error(ErrorCode.InvalidCast, "Can't implicitly cast between \"" + subDataType1.ToString() + "\" and \"" + subDataType2.ToString() + "\"", false, 1);

			}

			return subDataType1;
		}

		/// <summary>
		///		Parses a lower-top level expression. The lower-top level is responsible for parsing 
		///		any leaf expression and relational operators.
		/// </summary>
		/// <returns>The data type this expression evaluates to.</returns>
		private DataTypeValue ParseSubExpression()
		{
			// Parse the left hand leaf expression.
			DataTypeValue leafDataType1 = ParseLeafExpression();

			// Parse any subsequent leaf expressions that are seperated
			// by logical operators.
			while (true)
			{
				// Check that the next token is relational.
				Token operatorToken = LookAheadToken();
				if (operatorToken.IsRelational == false)
					break;
				NextToken();

				// Parse the right hand lead expression.
				DataTypeValue leafDataType2 = ParseLeafExpression();

				// Check the new data type can be cast to the main data type
				if (CanImplicitlyCast(leafDataType1, leafDataType2) == true)
				{

					// Check the data types are valid with this operator.
					if (OperatorDataTypeValid(leafDataType1, operatorToken.ID) == false)
						Error(ErrorCode.InvalidDataType, "Operator \"" + operatorToken.Ident + "\" can't be applied to data type \"" + leafDataType1.ToString() + "\"", false, 1);

					// We only want to emit byte code on the second pass.
					if (_currentPass == 1)
					{

						// Pop the result of the second sub expression into arithmetic
						// register 1.
						Instruction instruction = CreateInstruction(OpCodeByType(leafDataType2, OpCodeType.POP), _currentScope, _currentToken);
						new Operand(instruction, Register.Arithmetic2);

						// Pop the result of the first sub expression into arithmetic
						// register 2.
						instruction = CreateInstruction(OpCodeByType(leafDataType1, OpCodeType.POP), _currentScope, _currentToken);
						new Operand(instruction, Register.Arithmetic1);

						#region Relational Byte Code Emiting

						// Cast rvalue into lvalues type.
						if (leafDataType1 != leafDataType2)
						{
							instruction = CreateInstruction(OpCodeByType(leafDataType1, OpCodeType.CAST), _currentScope, _currentToken);
							new Operand(instruction, Register.Arithmetic2);
						}

						// Compare the results.
						instruction = CreateInstruction(OpCodeByType(leafDataType1, OpCodeType.CMP), _currentScope, _currentToken);
						new Operand(instruction, Register.Arithmetic1);
						new Operand(instruction, Register.Arithmetic2);

						// Create equality byte code based on the given operator.
						switch (operatorToken.ID)
						{
							case TokenID.OpEqual: instruction = CreateInstruction(OpCode.IS_EQ, _currentScope, _currentToken); break;
							case TokenID.OpGreater: instruction = CreateInstruction(OpCode.IS_G, _currentScope, _currentToken); break;
							case TokenID.OpGreaterEqual: instruction = CreateInstruction(OpCode.IS_GE, _currentScope, _currentToken); break;
							case TokenID.OpLess: instruction = CreateInstruction(OpCode.IS_L, _currentScope, _currentToken); break;
							case TokenID.OpLessEqual: instruction = CreateInstruction(OpCode.IS_LE, _currentScope, _currentToken); break;
							case TokenID.OpNotEqual: instruction = CreateInstruction(OpCode.IS_NE, _currentScope, _currentToken); break;
						}

						#endregion
					}

					// Data type is now boolean due to the comparison operators.
					leafDataType1 = new DataTypeValue(DataType.Bool, false, false);
				}
				else
					Error(ErrorCode.InvalidCast, "Can't implicitly cast between \"" + leafDataType1.ToString() + "\" and \"" + leafDataType2.ToString() + "\"", false, 1);

			}

			return leafDataType1;
		}

		/// <summary>
		///		Parses a middle level expression. The middle level is responsible for parsing 
		///		any terms and the +- arithmetic operators.
		// </summary>
		/// <returns>The data type this expression evaluates to.</returns>
		private DataTypeValue ParseLeafExpression()
		{
			// Parse the left hand term expression.
			DataTypeValue termDataType1 = ParseTermExpression();

			// Parse any subsequent term expressions.
			while (true)
			{
				// Check that the next token is an arithmatic + or -.
				Token operatorToken = LookAheadToken();
				if (operatorToken.ID != TokenID.OpAdd && operatorToken.ID != TokenID.OpSub)
					break;
				NextToken();

				// Parse the right hand term expression.
				DataTypeValue termDataType2 = ParseTermExpression();

				// Check the new data type can be cast to the main data type
				if (CanImplicitlyCast(termDataType1, termDataType2) == true)
				{

					// Check the data types are valid with this operator.
					if (OperatorDataTypeValid(termDataType1, operatorToken.ID) == false)
						Error(ErrorCode.InvalidDataType, "Operator \"" + operatorToken.Ident + "\" can't be applied to data type \"" + termDataType1.ToString() + "\"", false, 1);

					// We only want to emit byte code on the second pass.
					if (_currentPass == 0) 
                        continue;

					// Pop the result of the second term into arithmetic
					// register 1.
					Instruction instruction = CreateInstruction(OpCodeByType(termDataType2, OpCodeType.POP), _currentScope, _currentToken);
					new Operand(instruction, Register.Arithmetic2);

					// Pop the result of the first term into arithmetic
					// register 2.
					instruction = CreateInstruction(OpCodeByType(termDataType1, OpCodeType.POP), _currentScope, _currentToken);
					Operand op1 = new Operand(instruction, Register.Arithmetic1);

                    // Cast rvalue into lvalues type.
                    if (termDataType1 != termDataType2)
                    {
                        instruction = CreateInstruction(OpCodeByType(termDataType1, OpCodeType.CAST), _currentScope, _currentToken);
                        new Operand(instruction, Register.Arithmetic2);
                    }

					// Create the arithmatic instruction based on the operator.
					switch (operatorToken.ID)
					{
						case TokenID.OpAdd: instruction = CreateInstruction(OpCodeByType(termDataType1, OpCodeType.ADD), _currentScope, _currentToken); break;
						case TokenID.OpSub: instruction = CreateInstruction(OpCodeByType(termDataType1, OpCodeType.SUB), _currentScope, _currentToken); break;
					}
					new Operand(instruction, Register.Arithmetic1);
					new Operand(instruction, Register.Arithmetic2);

					// Push the result onto the stack.
					instruction = CreateInstruction(OpCodeByType(termDataType1, OpCodeType.PUSH), _currentScope, _currentToken);
					op1 = new Operand(instruction, Register.Arithmetic1);

				}
				else
                    Error(ErrorCode.InvalidCast, "Can't implicitly cast between \"" + termDataType1.ToString() + "\" and \"" + termDataType2.ToString() + "\"", false, 1);

			}

			return termDataType1;
		}

		/// <summary>
		///		Parses a lower level expression. The lower level is responsible for parsing 
		///		any sub terms and all operators apart from +- arithmetic operators.
		/// </summary>
		/// <returns>The data type this expression evaluates to.</returns>
		private DataTypeValue ParseTermExpression()
		{
			// Parse the left hand factor expression.
			DataTypeValue factorDataType1 = ParseSubTermExpression();

			// Parse any subsequent factor expressions.
			while (true)
			{
				// Check that the next token is an arithmatic operator.
				Token operatorToken = LookAheadToken();
				if (operatorToken.ID != TokenID.OpDivide &&
					operatorToken.ID != TokenID.OpModulus && operatorToken.ID != TokenID.OpMultiply &&
					operatorToken.ID != TokenID.OpBitwiseXOr && operatorToken.ID != TokenID.OpBitwiseSHR &&
					operatorToken.ID != TokenID.OpBitwiseSHL && operatorToken.ID != TokenID.OpBitwiseOr &&
					operatorToken.ID != TokenID.OpBitwiseAnd)
					break;
				NextToken();

				// Parse the right hand term expression.
                DataTypeValue factorDataType2 = ParseSubTermExpression();

				// Check the new data type can be cast to the main data type
				if (CanImplicitlyCast(factorDataType1, factorDataType2) == true)
				{

					// Check the data types are valid with this operator.
					if (OperatorDataTypeValid(factorDataType1, operatorToken.ID) == false)
						Error(ErrorCode.InvalidDataType, "Operator \"" + operatorToken.Ident + "\" can't be applied to data type \"" + factorDataType1.ToString() + "\"", false, 1);

					// We only want to emit byte code on the second pass.
					if (_currentPass == 0) continue;

					// Pop the result of the first term into arithmetic
					// register 1.
					Instruction instruction = CreateInstruction(OpCodeByType(factorDataType2, OpCodeType.POP), _currentScope, _currentToken);
					new Operand(instruction, Register.Arithmetic2);

					// Pop the result of the second term into arithmetic
					// register 2.
					instruction = CreateInstruction(OpCodeByType(factorDataType1, OpCodeType.POP), _currentScope, _currentToken);
					Operand op1 = new Operand(instruction, Register.Arithmetic1);

					// Cast rvalue into lvalues type.
					if (factorDataType1 != factorDataType2)
					{
						instruction = CreateInstruction(OpCodeByType(factorDataType1, OpCodeType.CAST), _currentScope, _currentToken);
						new Operand(instruction, Register.Arithmetic2);
					}

					// Create the arithmatic instruction based on the operator.
					switch (operatorToken.ID)
					{
						case TokenID.OpDivide: instruction = CreateInstruction(OpCodeByType(factorDataType1, OpCodeType.DIV), _currentScope, _currentToken); break;
						case TokenID.OpModulus: instruction = CreateInstruction(OpCode.MOD_INT, _currentScope, _currentToken); break;
						case TokenID.OpMultiply: instruction = CreateInstruction(OpCodeByType(factorDataType1, OpCodeType.MUL), _currentScope, _currentToken); break;
						case TokenID.OpBitwiseXOr: instruction = CreateInstruction(OpCode.BIT_XOR_INT, _currentScope, _currentToken); break;
						case TokenID.OpBitwiseSHR: instruction = CreateInstruction(OpCode.BIT_SHL_INT, _currentScope, _currentToken); break;
						case TokenID.OpBitwiseSHL: instruction = CreateInstruction(OpCode.BIT_SHR_INT, _currentScope, _currentToken); break;
						case TokenID.OpBitwiseOr: instruction = CreateInstruction(OpCode.BIT_OR_INT, _currentScope, _currentToken); break;
						case TokenID.OpBitwiseAnd: instruction = CreateInstruction(OpCode.BIT_AND_INT, _currentScope, _currentToken); break;
					}
					new Operand(instruction, Register.Arithmetic1);
					new Operand(instruction, Register.Arithmetic2);

					// Push the result onto the stack.
					instruction = CreateInstruction(OpCodeByType(factorDataType1, OpCodeType.PUSH), _currentScope, _currentToken);
					op1 = new Operand(instruction, Register.Arithmetic1);

				}
				else
					Error(ErrorCode.InvalidCast, "Can't implicitly cast between \"" + factorDataType1.ToString() + "\" and \"" + factorDataType2.ToString() + "\"", false, 1);

			}

			return factorDataType1;
		}

        /// <summary>
        ///		Parses a lower level expression. The lower level is responsible for parsing 
        ///		any factors and the -> operator.
        /// </summary>
        /// <returns>The data type this expression evaluates to.</returns>
        private DataTypeValue ParseSubTermExpression()
        {
            // Parse the left hand factor expression.
            DataTypeValue factorDataType1 = ParseFactorExpression();

            // Parse any subsequent factor expressions.
            while (true)
            {
                // Check that the next token is an member resolver operator.
                Token operatorToken = LookAheadToken();
                if (operatorToken.ID != TokenID.OpMemberResolver)
                    break;
                NextToken();

                // If its not an object how can we access it?
                if (factorDataType1.DataType != DataType.Object)
                    Error(ErrorCode.IllegalResolve, "Member accessing can only be preformed on objects.", false, 1);

                // ARRAY ACCESSING!

                // Parse the cast.
                ExpectToken(TokenID.CharOpenParenthesis);
                if (NextToken().IsDataType == false)
                    Error(ErrorCode.InvalidCast, "Attempt to cast member to unknown type.", false, 0);

                DataTypeValue castDataType = new DataTypeValue(DataTypeFromKeywordToken(_currentToken.ID), false, false);

                // Is it an array?
                if (LookAheadToken().ID == TokenID.CharCloseBracket)
                {
                    NextToken();
                    ExpectToken(TokenID.CharCloseBracket);
                    castDataType.IsArray = true;
                }
                ExpectToken(TokenID.CharCloseParenthesis);

                // Set the factor data type as the casts value.
                factorDataType1 = castDataType;

                // Read in the indentifier of the member.
                string identifier = ExpectToken(TokenID.TypeIdentifier).Ident;

                // Parse The member that is being called.
                if (LookAheadToken().ID != TokenID.CharOpenParenthesis)
                {
                    // Are we going to index it?
                    bool parsingArray = false;
                    DataTypeValue arrayIndexValue = new DataTypeValue(DataType.Invalid, false, false);
                    if (LookAheadToken().ID == TokenID.CharOpenBracket)
                    {
                        parsingArray = true;
                        arrayIndexValue = ParseExpression();
                        ExpectToken(TokenID.CharCloseBracket);
                    }

                    // Create the symbol.
                    VariableSymbol variableSymbol = null;
                    if (_currentPass == 0)
                    {
                        variableSymbol = _memberScope.FindVariableSymbol(identifier, castDataType) as VariableSymbol;
                        if (variableSymbol == null)
                        {
                            variableSymbol = new VariableSymbol(_memberScope);
                            variableSymbol.Identifier = identifier;
                            variableSymbol.DataType = castDataType;
                            variableSymbol.VariableType = VariableType.Member;
                        }
                    }
                    else if (_currentPass == 1)
                        variableSymbol = _memberScope.FindVariableSymbol(identifier, castDataType) as VariableSymbol;

                    if (_currentPass == 1)
                    {
                        Instruction instruction = null;

                        // Pop the index into arith 1.
                        if (parsingArray == true)
                        {
                            instruction = CreateInstruction(OpCodeByType(arrayIndexValue, OpCodeType.POP), _currentScope, _currentToken);
                            new Operand(instruction, Register.Arithmetic1);
                        }

                        // Pop the object into the member register.
                        instruction = CreateInstruction(OpCode.POP_OBJECT, _currentScope, _currentToken);
                        new Operand(instruction, Register.Arithmetic2);

                        // Call the GET_MEMBER opCode.
                        if (parsingArray)
                        {
                            instruction = CreateInstruction(OpCode.GET_MEMBER, _currentScope, _currentToken);
                            new Operand(instruction, Register.Arithmetic2);
                            new Operand(instruction, variableSymbol);
                        }
                        else
                        {
                            instruction = CreateInstruction(OpCode.GET_MEMBER_INDEXED, _currentScope, _currentToken);
                            new Operand(instruction, Register.Arithmetic2);
                            new Operand(instruction, variableSymbol);
                            new Operand(instruction, Register.Arithmetic1);
                        }

                        // Push the returned value.
                        instruction = CreateInstruction(OpCodeByType(castDataType, OpCodeType.PUSH), _currentScope, _currentToken);
                        new Operand(instruction, Register.Return);
                    }
                }
                else
                {
                    #region Function calling

                    // Parse the function call and remember the return type.
                    ParseMemberFunctionCall(identifier, castDataType);

                    // Push the return value of the function onto the stack.
                    if (_currentPass == 1)
                    {
                        Instruction instruction = CreateInstruction(OpCodeByType(castDataType, OpCodeType.PUSH), _currentScope, _currentToken);
                        new Operand(instruction, Register.Return);
                    }

                    #endregion
                }
            }

            return factorDataType1;
        }

		/// <summary>
		///		Parses the lowest level of an expression. The lowest level is respolsible
		///		for parsing things such as literal values and function calls, it also
		///		deals with unary, prefix and postfix operators.
		/// </summary>
		/// <returns>The data type this expression evaluates to.</returns>
		private DataTypeValue ParseFactorExpression()
		{
			// Declare a variable to store the data type in.
			DataTypeValue factorDataType = new DataTypeValue(DataType.Invalid, false, false);
			StringSymbol stringSymbol = null;

			#region Unary op
			// Check if there is a unary operator pending.
			bool unaryOpPending = false;
			Token unaryOpToken = LookAheadToken();
			if (unaryOpToken.ID == TokenID.OpAdd || unaryOpToken.ID == TokenID.OpSub ||
				unaryOpToken.ID == TokenID.OpLogicalNot || unaryOpToken.ID == TokenID.OpBitwiseNot ||
				unaryOpToken.ID == TokenID.OpIncrement || unaryOpToken.ID == TokenID.OpDecrement)
			{
				unaryOpPending = true;
				NextToken();
			}
			#endregion

			#region Cast Pending
			// Check if there is a explicit cast pending.
			bool castPending = false;
			DataTypeValue castType = new DataTypeValue(DataType.Invalid, false, false);
			if (LookAheadToken().ID == TokenID.CharOpenParenthesis)
			{
				castType.DataType = DataTypeFromKeywordToken(LookAheadToken(2).ID);
				if (castType.DataType != DataType.Invalid)
				{
					NextToken(); // Read in open parenthesis.
					NextToken(); // Read in cast keyword.
					// Check for array.
					if (LookAheadToken().ID == TokenID.CharOpenBracket)
					{
						NextToken();
						castType.IsArray = true;
						ExpectToken(TokenID.CharCloseBracket);
					}
					ExpectToken(TokenID.CharCloseParenthesis); // Read in closing parenthesis.
					castPending = true;
				}
			}
			#endregion

			// Determine what factor type we are looking at 
			// and act accordingly.
			NextToken();
			switch (_currentToken.ID)
			{
				#region Indexer
				case TokenID.KeywordIndexer:

					// Check we are inside an indexer loop.
					if (_insideIndexerLoop == false)
						Error(ErrorCode.InvalidIndexer, "The Indexer keyword can only be used inside a valid indexable loop.");

					// Set the factor data type as an integer (an indexer is always an integer).
					factorDataType = new DataTypeValue(DataType.Int, false, false);

					// Generate byte code to push indexer onto the stack.
					if (_currentPass == 1)
					{
						VariableSymbol indexerVariable = _currentScope.FindSymbol("$" + _indexerLoopIndex, SymbolType.Variable) as VariableSymbol;
						Instruction instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.PUSH), _currentScope, _currentToken);
						Operand.DirectStackOperand(instruction, indexerVariable.StackIndex);
					}
					break;

				#endregion
				#region New
				case TokenID.KeywordNew:

					// Read in identifier specifying what we wish to create a new 
					// instance of.
					NextToken();

					// Check if we are detailing with a data type.
					DataTypeValue dataType = new DataTypeValue(DataTypeFromKeywordToken(_currentToken.ID), true, false);
					factorDataType = dataType;
					if (dataType.DataType != DataType.Invalid)
					{
						
						// Check this is an array.
						if (LookAheadToken().ID == TokenID.CharOpenBracket)
						{
							// Read in opening bracket.
							NextToken(); 
							
							// Parse the size expression.
							DataTypeValue indexDataType = ParseExpression();

							// Make sure we can convert it to an integer index.
							if (CanImplicitlyCast(new DataTypeValue(DataType.Int, false, false), indexDataType))
							{
								// If we are in pass 2 create the byte code to allocate a
								// new array on the heap and associate it with this variable.
								if (_currentPass == 1)
								{
									// Pop the index value into arith register 2.
									Instruction instruction = CreateInstruction(OpCodeByType(indexDataType, OpCodeType.POP), _currentScope, _currentToken);
									new Operand(instruction, Register.Arithmetic2);

                                    // Cast?

									// Allocate enough space on the heap for the array.
									instruction = CreateInstruction(OpCodeByType(dataType, OpCodeType.ALLOCATE_HEAP), _currentScope, _currentToken);
									new Operand(instruction, Register.Arithmetic2);
									new Operand(instruction, Register.Reserved4);

                                    // Push the memory index onto the stack.
                                    instruction = CreateInstruction(OpCodeByType(dataType, OpCodeType.PUSH), _currentScope, _currentToken);
                                    new Operand(instruction, Register.Reserved4);
								}
							}
							else
								Error(ErrorCode.InvalidCast, "Can't implicitly cast between \"Int\" and \"" + indexDataType.ToString() + "\"", false, 0);

							// Read in closing bracket.
							ExpectToken(TokenID.CharCloseBracket);

                            if (LookAheadToken().ID == TokenID.CharOpenBrace)
                            {
                                ExpectToken(TokenID.CharOpenBrace);

                                int index = 0;
                                while (true)
                                {
                                    // Parse the expression
                                    DataTypeValue valueType = ParseExpression();
                                    if (CanImplicitlyCast(new DataTypeValue(dataType.DataType, false, false), valueType) == true)
                                    {
                                        // Its valid so insert.
                                        if (_currentPass == 1)
                                        {
                                            Instruction instruction = null;

                                            // Pop the value into arith 2.
                                            instruction = CreateInstruction(OpCodeByType(valueType, OpCodeType.POP), _currentScope, _currentToken);
                                            new Operand(instruction, Register.Arithmetic2);

                                            // Pop the memory index off the stack.
                                            instruction = CreateInstruction(OpCodeByType(dataType, OpCodeType.POP), _currentScope, _currentToken);
                                            new Operand(instruction, Register.Reserved4);
                                            
                                            // Do we need to cast first?
                                            if (dataType.DataType != valueType.DataType)
                                            {
                                                // Cast value.
                                                instruction = CreateInstruction(OpCodeByType(new DataTypeValue(dataType.DataType, false, false), OpCodeType.CAST), _currentScope, _currentToken);
                                                new Operand(instruction, Register.Arithmetic2);
                                            }

                                            // Move index into arith 3.
                                            instruction = CreateInstruction(OpCode.MOV_INT, _currentScope, _currentToken);
                                            new Operand(instruction, Register.Arithmetic3);
                                            new Operand(instruction, index);

                                            // Insert!
                                            instruction = CreateInstruction(OpCodeByType(new DataTypeValue(dataType.DataType, false, false), OpCodeType.MOV), _currentScope, _currentToken);
                                            Operand.IndirectMemoryIndexedOperand(instruction, Register.Reserved4, Register.Arithmetic3);
                                            new Operand(instruction, Register.Arithmetic2);

                                            // Push the memory index back onto the stack.
                                            instruction = CreateInstruction(OpCodeByType(dataType, OpCodeType.PUSH), _currentScope, _currentToken);
                                            new Operand(instruction, Register.Reserved4);
                                        }
                                    }
                                    else
                                        Error(ErrorCode.InvalidDataType, "Unable to implicitly cast from type \"" + DataTypeFromTypeToken(_currentToken.ID).ToString() + "\" to type \"" + dataType.DataType.ToString() + "\".", false, 1);

                                    // Read in comma or break out of loop if there isn't one.
                                    if (LookAheadToken().ID == TokenID.CharComma)
                                    {
                                        NextToken();
                                        index++;
                                    }
                                    else
                                        break;
                                }

                                ExpectToken(TokenID.CharCloseBrace);
                            }
						}
						else
							Error(ErrorCode.InvalidFactor, "Primitive data types must currently be created as arrays.", false,0);
					}
					else
						Error(ErrorCode.InvalidFactor, "New keyword can currently only be used to create new primitive data arrays.", false,0);

					break;
				#endregion
				#region Boolean
				case TokenID.TypeBoolean:
					factorDataType = new DataTypeValue(DataType.Bool, false, false);
					if (_currentPass == 1)
					{
						Instruction instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.PUSH), _currentScope, _currentToken);
						new Operand(instruction, bool.Parse(_currentToken.Ident));
					}
					break;
				#endregion
				#region Byte
				case TokenID.TypeByte:
					factorDataType = new DataTypeValue(DataType.Byte, false, false);
					if (_currentPass == 1)
					{
						Instruction instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.PUSH), _currentScope, _currentToken);
						new Operand(instruction, byte.Parse(_currentToken.Ident));
					}
					break;
				#endregion
				#region Double
				case TokenID.TypeDouble:
					factorDataType = new DataTypeValue(DataType.Double, false, false);
					if (_currentPass == 1)
					{
						Instruction instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.PUSH), _currentScope, _currentToken);
						new Operand(instruction, double.Parse(_currentToken.Ident));
					}
					break;
				#endregion
				#region Float
				case TokenID.TypeFloat:
					factorDataType = new DataTypeValue(DataType.Float, false, false);
					if (_currentPass == 1)
					{
						Instruction instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.PUSH), _currentScope, _currentToken);
						new Operand(instruction, float.Parse(_currentToken.Ident));
					}
					break;
				#endregion
				#region Integer
				case TokenID.TypeInteger:
					factorDataType = new DataTypeValue(DataType.Int, false, false);
					if (_currentPass == 1)
					{
						Instruction instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.PUSH), _currentScope, _currentToken);
						if (_currentToken.Ident.Length > 2 && _currentToken.Ident.Substring(0, 2) == "0x")
							new Operand(instruction, Convert.ToInt32(_currentToken.Ident, 16));
						else
							new Operand(instruction, int.Parse(_currentToken.Ident));
					}
					break;
				#endregion
				#region Long
				case TokenID.TypeLong:
					factorDataType = new DataTypeValue(DataType.Long, false, false);
					if (_currentPass == 1)
					{
						Instruction instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.PUSH), _currentScope, _currentToken);
						new Operand(instruction, Convert.ToInt64(_currentToken.Ident));
					}
					break;
				#endregion
				#region Short
				case TokenID.TypeShort:
					factorDataType = new DataTypeValue(DataType.Short, false, false);
					if (_currentPass == 1)
					{
						Instruction instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.PUSH), _currentScope, _currentToken);
						new Operand(instruction, short.Parse(_currentToken.Ident));
					}
					break;
				#endregion
				#region Null
				case TokenID.TypeNull:
					factorDataType = new DataTypeValue(DataType.Null, false, false);
					if (_currentPass == 1)
					{
						Instruction instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.PUSH), _currentScope, _currentToken);
					}
					break;
				#endregion
				#region String
				case TokenID.TypeString:
					// Always add strings to the global scope, this stops them
					// being duplicated in multiple scopes.
					factorDataType = new DataTypeValue(DataType.String, false, false);

                    // Check for a pre-existing string with the same value.
                    foreach (Symbol subSymbol in _globalScope.Symbols)
                    {
                        if (subSymbol.Identifier == null || subSymbol.Type != SymbolType.String) continue;
                        if (subSymbol.Identifier == _currentToken.Ident)
                        {
                            stringSymbol = subSymbol as StringSymbol;
                            break;
                        }
                    }
                    
					if (stringSymbol == null) stringSymbol = new StringSymbol(_globalScope, _currentToken.Ident);
					if (_currentPass == 1)
					{
						Instruction instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.PUSH), _currentScope, _currentToken);
						new Operand(instruction, stringSymbol);
					}
					break;
				#endregion
				#region Identifier
				case TokenID.TypeIdentifier:
					VariableSymbol symbol = null;
					string symbolIdent = _currentToken.Ident;
                    int tokenIndex = _tokenIndex;
                    Token currentToken = _currentToken;
                    Symbol resolvedScope = ResolveMemberScope();
                    symbol = resolvedScope == null ? null : resolvedScope.FindSymbol(_currentToken.Ident, SymbolType.Variable) as VariableSymbol;
					if (symbol != null)
                    {
                        // Tag the variable as used so we don't end up throwing up a warning.
                        symbol.IsUsed = true;

                        // Set the factor data type.
                        factorDataType = new DataTypeValue(symbol.DataType.DataType, symbol.DataType.IsArray, symbol.DataType.IsReference);

                        if (symbol.IsConstant == true && symbol.ConstToken != null)
                        {
                            switch (symbol.DataType.DataType)
                            {
                                case DataType.Bool:
                                    factorDataType = new DataTypeValue(DataType.Bool, false, false);
                                    if (_currentPass == 1)
                                    {
                                        Instruction instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.PUSH), _currentScope, _currentToken);
                                        new Operand(instruction, bool.Parse(symbol.ConstToken.Ident));
                                    }
                                    break;
                                case DataType.Byte:
                                    factorDataType = new DataTypeValue(DataType.Byte, false, false);
                                    if (_currentPass == 1)
                                    {
                                        Instruction instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.PUSH), _currentScope, _currentToken);
                                        new Operand(instruction, byte.Parse(symbol.ConstToken.Ident));
                                    }
                                    break;
                                case DataType.Double:
                                    factorDataType = new DataTypeValue(DataType.Double, false, false);
                                    if (_currentPass == 1)
                                    {
                                        Instruction instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.PUSH), _currentScope, _currentToken);
                                        new Operand(instruction, double.Parse(symbol.ConstToken.Ident));
                                    }
                                    break;
                                case DataType.Float:
                                    factorDataType = new DataTypeValue(DataType.Float, false, false);
                                    if (_currentPass == 1)
                                    {
                                        Instruction instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.PUSH), _currentScope, _currentToken);
                                        new Operand(instruction, float.Parse(symbol.ConstToken.Ident));
                                    }
                                    break;
                                case DataType.Int:
                                    factorDataType = new DataTypeValue(DataType.Int, false, false);
                                    if (_currentPass == 1)
                                    {
                                        Instruction instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.PUSH), _currentScope, _currentToken);
                                        if (symbol.ConstToken.Ident.Length > 2 && symbol.ConstToken.Ident.Substring(0, 2) == "0x")
                                            new Operand(instruction, Convert.ToInt32(symbol.ConstToken.Ident, 16));
                                        else
                                            new Operand(instruction, int.Parse(symbol.ConstToken.Ident));
                                    }
                                    break;
                                case DataType.Long:
                                    factorDataType = new DataTypeValue(DataType.Long, false, false);
                                    if (_currentPass == 1)
                                    {
                                        Instruction instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.PUSH), _currentScope, _currentToken);
                                        new Operand(instruction, Convert.ToInt64(symbol.ConstToken.Ident));
                                    }
                                    break;
                                case DataType.Short:
                                    factorDataType = new DataTypeValue(DataType.Short, false, false);
                                    if (_currentPass == 1)
                                    {
                                        Instruction instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.PUSH), _currentScope, _currentToken);
                                        new Operand(instruction, short.Parse(symbol.ConstToken.Ident));
                                    }
                                    break;
                                case DataType.String:
                                    factorDataType = new DataTypeValue(DataType.String, false, false);
                                    stringSymbol = _globalScope.FindSymbol(symbol.ConstToken.Ident, SymbolType.String) as StringSymbol;
                                    if (stringSymbol == null) stringSymbol = new StringSymbol(_globalScope, symbol.ConstToken.Ident);
                                    if (_currentPass == 1)
                                    {
                                        Instruction instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.PUSH), _currentScope, _currentToken);
                                        new Operand(instruction, stringSymbol);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            // Check if we are parsing an array.
                            if (LookAheadToken().ID == TokenID.CharOpenBracket)
                            {
                                // As the array is indexed we can remove array from the factor data type.
                                factorDataType.IsArray = false;

                                // Read in open bracket.
                                NextToken();

                                // Make sure it really is an array.
                                if (symbol.IsArray == false) Error(ErrorCode.InvalidArrayIndex, "Non-array variables can not be indexed.", false, 0);

                                // Check that the next token is not a closing brace otherwise
                                // its an invalid index.
                                if (LookAheadToken().ID == TokenID.CharCloseBracket)
                                    Error(ErrorCode.InvalidArrayIndex, "Array's must be indexed.", false, 0);

                                // Parse the index expression.
                                DataTypeValue indexDataType = ParseExpression();
                                ExpectToken(TokenID.CharCloseBracket);

                                // Check the index data type can be cast to an integer index.
                                if (CanImplicitlyCast(new DataTypeValue(DataType.Int, false, false), indexDataType) == true)
                                {
                                    // Emit the value retrieval's byte code.
                                    if (_currentPass == 1)
                                    {
                                        // Pop the index value into register 1.
                                        Instruction instruction = CreateInstruction(OpCodeByType(indexDataType, OpCodeType.POP), _currentScope, _currentToken);
                                        new Operand(instruction, Register.Reserved1);

                                        // Cast index value to integer
                                        if (indexDataType.DataType != DataType.Int)
                                        {
                                            instruction = CreateInstruction(OpCode.CAST_INT, _currentScope, _currentToken);
                                            new Operand(instruction, Register.Reserved1);
                                        }

                                        // Move the array's memory slot into reserved2 register.
                                        instruction = CreateInstruction(OpCode.MOV_MEMORY_INDEX, _currentScope, _currentToken);
                                        new Operand(instruction, Register.Reserved2);
                                        if (symbol.VariableType == VariableType.Constant || symbol.VariableType == VariableType.Global)
                                            Operand.DirectMemoryOperand(instruction, symbol.MemoryIndex);
                                        else
                                            Operand.DirectStackOperand(instruction, symbol.StackIndex);

                                        // Push the array's data onto the stack indexed
                                        // by the register we just poped the index into.
                                        instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.PUSH), _currentScope, _currentToken);
                                        Operand.IndirectMemoryIndexedOperand(instruction, Register.Reserved2, Register.Reserved1);

                                    }

                                    #region ++ / -- Parsing
                                    // Check if there is a increment (++) or decrement (--) operator following this value..
                                    if (LookAheadToken().ID == TokenID.OpIncrement)
                                    {
                                        // Read in increment token.
                                        NextToken();

                                        // Check the data types are valid with this operator.
                                        if (OperatorDataTypeValid(symbol.DataType, _currentToken.ID) == false)
                                            Error(ErrorCode.InvalidDataType, "Operator \"" + _currentToken.Ident + "\" can't be applied to data type \"" + symbol.DataType.ToString() + "\"", false, 0);

                                        // Make sure the symbol is not constant.
                                        if (symbol.IsConstant == true)
                                            Error(ErrorCode.IllegalAssignment, "Can't modify constant value.", false, 0);

                                        // Increment the variable.
                                        if (_currentPass == 1)
                                        {
                                            // Take the assumtion that the values are still in the register from the previous code.
                                            Instruction instruction = CreateInstruction(OpCodeByType(symbol.DataType, OpCodeType.INC), _currentScope, _currentToken);
                                            Operand.IndirectMemoryIndexedOperand(instruction, Register.Reserved2, Register.Reserved1);
                                        }
                                    }
                                    else if (LookAheadToken().ID == TokenID.OpDecrement)
                                    {
                                        // Read in increment token.
                                        NextToken();

                                        // Check the data types are valid with this operator.
                                        if (OperatorDataTypeValid(symbol.DataType, _currentToken.ID) == false)
                                            Error(ErrorCode.InvalidDataType, "Operator \"" + _currentToken.Ident + "\" can't be applied to data type \"" + symbol.DataType.ToString() + "\"", false, 0);

                                        // Make sure the symbol is not constant.
                                        if (symbol.IsConstant == true)
                                            Error(ErrorCode.IllegalAssignment, "Can't modify constant value.", false, 0);

                                        // Decrement the variable.
                                        if (_currentPass == 1)
                                        {
                                            // Take the assumtion that the values are still in the register from the previous code.
                                            Instruction instruction = CreateInstruction(OpCodeByType(symbol.DataType, OpCodeType.DEC), _currentScope, _currentToken);
                                            Operand.IndirectMemoryIndexedOperand(instruction, Register.Reserved2, Register.Reserved1);
                                        }
                                    }
                                    #endregion

                                }
                                else
                                    Error(ErrorCode.InvalidCast, "Can't implicitly cast between \"Int\" and \"" + indexDataType.ToString() + "\"", false, 0);

                            }

                            // Its not an array so parse it as a reqular variable.
                            else
                            {
                                // Push the variable's value onto stack.
                                if (_currentPass == 1)
                                {
                                    Instruction instruction = CreateInstruction(OpCodeByType(symbol.DataType, OpCodeType.PUSH), _currentScope, _currentToken);
                                    if (symbol.VariableType == VariableType.Global || symbol.VariableType == VariableType.Constant)
                                        Operand.DirectMemoryOperand(instruction, symbol.MemoryIndex);
                                    else
                                        Operand.DirectStackOperand(instruction, symbol.StackIndex);
                                }

                                #region ++ / -- Parsing
                                // Check if there is a increment (++) or decrement (--) operator following this value..
                                if (LookAheadToken().ID == TokenID.OpIncrement)
                                {
                                    // Read in increment token.
                                    NextToken();

                                    // Check the data types are valid with this operator.
                                    if (OperatorDataTypeValid(symbol.DataType, _currentToken.ID) == false)
                                        Error(ErrorCode.InvalidDataType, "Operator \"" + _currentToken.Ident + "\" can't be applied to data type \"" + symbol.DataType.ToString() + "\"", false, 0);

                                    // Make sure the symbol is not constant.
                                    if (symbol.IsConstant == true)
                                        Error(ErrorCode.IllegalAssignment, "Can't modify constant value.", false, 0);

                                    // Increment the variable.
                                    if (_currentPass == 1)
                                    {
                                        Instruction instruction = CreateInstruction(OpCodeByType(symbol.DataType, OpCodeType.INC), _currentScope, _currentToken);
                                        if (symbol.VariableType == VariableType.Global || symbol.VariableType == VariableType.Constant)
                                            Operand.DirectMemoryOperand(instruction, symbol.MemoryIndex);
                                        else
                                            Operand.DirectStackOperand(instruction, symbol.StackIndex);
                                    }
                                }
                                else if (LookAheadToken().ID == TokenID.OpDecrement)
                                {
                                    // Read in increment token.
                                    NextToken();

                                    // Check the data types are valid with this operator.
                                    if (OperatorDataTypeValid(symbol.DataType, _currentToken.ID) == false)
                                        Error(ErrorCode.InvalidDataType, "Operator \"" + _currentToken.Ident + "\" can't be applied to data type \"" + symbol.DataType.ToString() + "\"", false, 0);

                                    // Make sure the symbol is not constant.
                                    if (symbol.IsConstant == true)
                                        Error(ErrorCode.IllegalAssignment, "Can't modify constant value.", false, 0);

                                    // Decrement the variable.
                                    if (_currentPass == 1)
                                    {
                                        Instruction instruction = CreateInstruction(OpCodeByType(symbol.DataType, OpCodeType.DEC), _currentScope, _currentToken);
                                        if (symbol.VariableType == VariableType.Global || symbol.VariableType == VariableType.Constant)
                                            Operand.DirectMemoryOperand(instruction, symbol.MemoryIndex);
                                        else
                                            Operand.DirectStackOperand(instruction, symbol.StackIndex);
                                    }
                                }
                                #endregion

                            }
                        }
					}

					// It look for opening parenthesis which would make it a function call.
					else if (EndOfTokenStream() == false && LookAheadToken().ID == TokenID.CharOpenParenthesis)
					{
						#region Function calling

                        _tokenIndex = tokenIndex;
                        _currentToken = currentToken;

						// Parse the function call and remember the return type.
						DataTypeValue functionReturnType = ParseFunctionCall();

						// Push the return value of the function onto the stack.
						if (_currentPass == 1)
						{
							Instruction instruction = CreateInstruction(OpCodeByType(functionReturnType, OpCodeType.PUSH), _currentScope, _currentToken);
							new Operand(instruction, Register.Return);
						}

						// Set the factors data type.
						factorDataType = functionReturnType;

						#endregion
					}

					// WTF! Is this?
					else
					{
						Error(ErrorCode.UndeclaredVariable, "Encountered attempt to access an undeclared symbol \"" + symbolIdent + "\".", false, 1);
					}
					break;
				#endregion
				#region Sub expression
				case TokenID.CharOpenParenthesis:
					factorDataType = ParseExpression();
					ExpectToken(TokenID.CharCloseParenthesis);
					break;
				#endregion
				default:
					Error(ErrorCode.InvalidFactor, "Invalid factor.", false, 0);
					break;
			}

			// Create instructions for unary operator if in second pass.
			if (unaryOpPending == true && _currentPass == 1)
			{
				// Check the data types are valid with this operator.
				if (OperatorDataTypeValid(factorDataType, unaryOpToken.ID) == false)
					Error(ErrorCode.InvalidDataType, "Operator \"" + unaryOpToken.Ident + "\" can't be apply to data type \"" + factorDataType.ToString() + "\"");

				// Pop the value of this value out of the stack
				// and preform action on it.
				Instruction instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.POP), _currentScope, _currentToken);
				new Operand(instruction, Register.Arithmetic1);

				// Create instruction based on operation code.
				switch (unaryOpToken.ID)
				{
					case TokenID.OpAdd: instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.ABS), _currentScope, _currentToken); break;
					case TokenID.OpSub: instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.NEG), _currentScope, _currentToken); break;
					case TokenID.OpLogicalNot: instruction = CreateInstruction(OpCode.LOGICAL_NOT, _currentScope, _currentToken); break;
					case TokenID.OpBitwiseNot: instruction = CreateInstruction(OpCode.BIT_NOT_INT, _currentScope, _currentToken); break;
					case TokenID.OpIncrement: instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.INC), _currentScope, _currentToken); break;
					case TokenID.OpDecrement: instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.DEC), _currentScope, _currentToken); break;
				}
				new Operand(instruction, Register.Arithmetic1);

				// Push the value back onto the stack (as long as its not the logical not, which does that itself).
                if (unaryOpToken.ID != TokenID.OpLogicalNot)
                {
                    instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.PUSH), _currentScope, _currentToken);
                    new Operand(instruction, Register.Arithmetic1);
                }
			}

			// If a cast is pending then convert the result to the given type.
			if (castPending == true && factorDataType != castType)
			{
				if (_currentPass == 1)
				{
					// Pop the result out of the stack.
					Instruction instruction = CreateInstruction(OpCodeByType(factorDataType, OpCodeType.POP), _currentScope, _currentToken);
					new Operand(instruction, Register.Arithmetic1);

					// Cast it to the given type.
					instruction = CreateInstruction(OpCodeByType(castType, OpCodeType.CAST), _currentScope, _currentToken);
					new Operand(instruction, Register.Arithmetic1);

					// Push the value back onto the stack.
					instruction = CreateInstruction(OpCodeByType(castType, OpCodeType.PUSH), _currentScope, _currentToken);
					new Operand(instruction, Register.Arithmetic1);
				}
				factorDataType = castType;
			}

			return factorDataType;
		}

		#endregion
	}

}