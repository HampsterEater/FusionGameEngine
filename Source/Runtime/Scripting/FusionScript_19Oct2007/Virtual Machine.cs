/* 
 * File: Virtual Machine.cs
 *
 * This source file contains the declarations of any and all classes
 * used to run scripts previously compiled to byte code.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Reflection;
using System.IO;
using System.Collections;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Runtime.Console;

namespace BinaryPhoenix.Fusion.Runtime.Scripting
{

	/// <summary>
	///		Used to describe how much of a virtual machines timeslice a given
	///		process or thread should be allow to use.
	/// </summary>
    public enum Priority : sbyte
	{
		RealTime		= -1,
		AboveNormal		= 3,
		Normal			= 2,
		BelowNormal		= 1,
		Low				= 0,
	}

	/// <summary>
	///		Used to describe the data stored within a runtime value.
	/// </summary>
	public enum RuntimeValueType : short
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
		SymbolIndexTracker,

		Object,
		ReturnAddress,
		StackFrameIndex,
		StackBaseMarker,
		MemoryBoundry
	}

	/// <summary>
	///		Encapsulates a single byte code instruction, is almost the same as the
	///		Instruction class but with a few added runtime specified things.
	/// </summary>
	public sealed class RuntimeInstruction
	{
		#region Members
		#region Variables

		private OpCode _opCode;
		private RuntimeValue[] _operands = new RuntimeValue[5]; // Maximum of 5 ops per instruction.
		private short _line, _offset;
		private string _file;
		private byte _operandCount;

		private bool _locked;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the array of operands used by this instruction.
		/// </summary>
		public RuntimeValue[] Operands
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
		public short Offset
		{
			get { return _offset; }
			set { _offset = value; }
		}

		/// <summary>
		///		Gets or sets the line this instruction was generated on.
		/// </summary>
		public short Line
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
		///		Gets or sets if this instruction is locked at the moment.
		/// </summary>
		public bool Locked
		{
			get { return _locked;  }
			set { _locked = value; }
		}

		/// <summary>
		///		Gets or sets the operand at a given index of this instruction operand list.
		/// </summary>
		/// <param name="index">Operand index.</param>
		/// <returns>Operand at the given index.</returns>
		public RuntimeValue this[int index]
		{
			get
			{
				if (index + 1 > _operandCount)
				{
					_operandCount = (byte)(index + 1);
					Array.Resize<RuntimeValue>(ref _operands, _operandCount);
				}
				return _operands[index];
			}
			set
			{
				if (index + 1 > _operandCount)
				{
                    _operandCount = (byte)(index + 1);
					Array.Resize<RuntimeValue>(ref _operands, _operandCount);
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
			set { _operandCount = (byte)value; }
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
				RuntimeValue op = _operands[i];
				switch (op.ValueType)
				{
					case RuntimeValueType.BooleanLiteral:
						asmString += op.BooleanLiteral;
						break;
					case RuntimeValueType.ByteLiteral:
						asmString += op.ByteLiteral;
						break;
					case RuntimeValueType.DirectMemory:
						asmString += "memory[" + op.MemoryIndex + "]";
						break;
					case RuntimeValueType.DirectMemoryIndexed:
						asmString += "memory[" + op.MemoryIndex + " + " + op.OffsetRegister.ToString() + "]";
						break;
					case RuntimeValueType.IndirectMemory:
						asmString += "memory[" + op.Register.ToString() + "]";
						break;
					case RuntimeValueType.IndirectMemoryIndexed:
						asmString += "memory[" + op.Register.ToString() + " + " + op.OffsetRegister.ToString() + "]";
						break;
					case RuntimeValueType.DirectStack:
						asmString += "stack[" + op.StackIndex + "]";
						break;
					case RuntimeValueType.DirectStackIndexed:
						asmString += "stack[" + op.StackIndex + " + " + op.OffsetRegister.ToString() + "]";
						break;
					case RuntimeValueType.IndirectStack:
						asmString += "stack[" + op.Register.ToString() + "]";
						break;
					case RuntimeValueType.IndirectStackIndexed:
						asmString += "stack[" + op.Register.ToString() + " + " + op.OffsetRegister.ToString() + "]";
						break;
					case RuntimeValueType.FloatLiteral:
						asmString += op.FloatLiteral;
						break;
					case RuntimeValueType.DoubleLiteral:
						asmString += op.DoubleLiteral;
						break;
					case RuntimeValueType.InstrIndex:
						asmString += "instruction[" + op.InstrIndex + "]";
						break;
					case RuntimeValueType.IntegerLiteral:
						asmString += op.IntegerLiteral;
						break;
					case RuntimeValueType.LongLiteral:
						asmString += op.LongLiteral;
						break;
					case RuntimeValueType.Register:
						asmString += op.Register.ToString();
						break;
					case RuntimeValueType.ShortLiteral:
						asmString += op.ShortLiteral;
						break;
					case RuntimeValueType.StringLiteral:
						asmString += "\"" + op.StringLiteral + "\"";
						break;
					case RuntimeValueType.SymbolIndex:
						asmString += op.Symbol.Identifier;
						break;
				}
				if (i < _operandCount - 1) asmString += ", ";
			}
			return asmString;
		}

		/// <summary>
		///		Initializes a new instruction.
		/// </summary>
		/// <param name="opCode">Operation code for this instruction.</param>
		public RuntimeInstruction(OpCode opCode)
		{
			_opCode = opCode;
		}

		#endregion
	}

    /// <summary>
    ///     Holds details about a console command embedded in a script.
    /// </summary>
    public class ScriptConsoleCommand
    {
        #region Members
        #region Variables

        private ScriptThread _thread = null;
        private FunctionSymbol _functionSymbol = null;

        private ConsoleCommand _command = null;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the script thread that this command is embedded in.
        /// </summary>
        public ScriptThread Thread
        {
            get { return _thread; }
            set { _thread = value; }
        }

        /// <summary>
        ///     Gets or sets the function symbol this command refers to.
        /// </summary>
        public FunctionSymbol Symbol
        {
            get { return _functionSymbol; }
            set { _functionSymbol = value; }
        }

        /// <summary>
        ///     Gets or sets the console command that is used for this command.
        /// </summary>
        public ConsoleCommand Command
        {
            get { return _command; }
            set { _command = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Invokes the command.
        /// </summary>
        /// <param name="args">Arguments used to call the command.</param>
        private void InvokeCommand(object[] args)
        {
            ConsoleValueType[] parameters = new ConsoleValueType[_functionSymbol.ParameterCount];
            for (int i = 0; i < parameters.Length; i++)
            {
                switch (((VariableSymbol)_functionSymbol.Symbols[i]).DataType.DataType)
                {
                    case DataType.Bool: _thread.PassParameter((bool)args[i]); break;
                    case DataType.Float: _thread.PassParameter((float)args[i]); break;
                    case DataType.Int: _thread.PassParameter((int)args[i]); break;
                    case DataType.String: _thread.PassParameter((string)args[i]); break;
                }
            }
            _thread.InvokeFunction(_functionSymbol);
        }

        /// <summary>
        ///     Initializes a new instance of thsi class.
        /// </summary>
        /// <param name="symbol">Function symbol describing the function to call.</param>
        /// <param name="thread">Script thread that function is embedded in.</param>
        public ScriptConsoleCommand(FunctionSymbol symbol, ScriptThread thread)
        {
            _thread = thread;
            _functionSymbol = symbol;

            ConsoleValueType[] parameters = new ConsoleValueType[symbol.ParameterCount];
            for (int i = 0; i < parameters.Length; i++)
            {
                switch (((VariableSymbol)symbol.Symbols[i]).DataType.DataType)
                {
                    case DataType.Bool: parameters[i] = ConsoleValueType.Bool; break;
                    case DataType.Float: parameters[i] = ConsoleValueType.Float; break;
                    case DataType.Int: parameters[i] = ConsoleValueType.Int; break;
                    case DataType.String: parameters[i] = ConsoleValueType.String; break;
                }
            }

            _command = new ConsoleCommand(symbol.Identifier, new CommandDelegate(InvokeCommand), parameters);
            Console.Console.RegisterCommand(_command);
        }

        #endregion
    }

    /// <summary>
    ///     Holds details on a function exported from a script.
    /// </summary>
    public class ScriptExportFunction
    {
        #region Members
        #region Variables

        private static ArrayList _functionList = new ArrayList();

        private ScriptThread _thread = null;
        private FunctionSymbol _functionSymbol = null;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the list of exported functions.
        /// </summary>
        public static ArrayList FunctionList
        {
            get { return _functionList; }
            set { _functionList = value; }
        }

        /// <summary>
        ///     Gets or sets the script thread that this function is embedded in.
        /// </summary>
        public ScriptThread Thread
        {
            get { return _thread; }
            set { _thread = value; }
        }

        /// <summary>
        ///     Gets or sets the function symbol that describes the function we need to call.
        /// </summary>
        public FunctionSymbol Symbol
        {
            get { return _functionSymbol; }
            set { _functionSymbol = value; }
        }

        #endregion
        #endregion
        #region Methods

        ///     Initializes a new instance of thsi class.
        /// </summary>
        /// <param name="symbol">Function symbol describing the function to call.</param>
        /// <param name="thread">Script thread that function is embedded in.</param>
        public ScriptExportFunction(FunctionSymbol symbol, ScriptThread thread)
        {
            _thread = thread;
            _functionSymbol = symbol;
            _functionList.Add(this);
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class RuntimeObject
    {
        #region Members
        #region Variables

        protected int _referenceCount = 0;
        protected bool _collectable = true;

        public ArrayList References = new ArrayList();

        #endregion
        #region Properties
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual RuntimeValue this[string propertyName] { get { return null; } set { } }

        /// <summary>
        /// 
        /// </summary>
        public int ReferenceCount
        {
            get { return _referenceCount; }
            set { _referenceCount = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Collectable
        {
            get { return _collectable; }
            set { _collectable = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public virtual void Deallocate()
        {

        }

        /// <summary>
        ///     Invoked when a script wants to call a method of this object.
        /// </summary>
        /// <param name="thread">Script thread that invoked this function.</param>
        /// <param name="method">Symbol describing the method that it wants called.</param>
        /// <returns>True if successfull or false if not.</returns>
        public virtual bool InvokeMethod(ScriptThread thread, FunctionSymbol method)
        {
            return false;
        }

        /// <summary>
        ///     Invoked when a script wants to set the value of a member of this object.
        /// </summary>
        /// <param name="thread">Script thread that invoked this function.</param>
        /// <param name="variable">Symbol describing the member that it wants set.</param>
        /// <param name="value">Value it wants the member to be set to.</param>
        /// <returns>True if successfull or false if not.</returns>
        public virtual bool SetMember(ScriptThread thread, VariableSymbol variable, RuntimeValue value)
        {
            return false;
        }

        /// <summary>
        ///     Invoked when a script wants to set the value of an indexed member of this object.
        /// </summary>
        /// <param name="thread">Script thread that invoked this function.</param>
        /// <param name="variable">Symbol describing the member that it wants set.</param>
        /// <param name="value">Value it wants the member to be set to.</param>
        /// <param name="index">Index of member to it wants set.</param>
        /// <returns>True if successfull or false if not.</returns>
        public virtual bool SetMember(ScriptThread thread, VariableSymbol variable, RuntimeValue value, RuntimeValue indexer)
        {
            return false;
        }

        /// <summary>
        ///     Invoked when a script wants to get the value of a member of this object.
        /// </summary>
        /// <param name="thread">Script thread that invoked this function.</param>
        /// <param name="variable">Symbol describing the member that it wants get.</param>
        /// <returns>True if successfull or false if not.</returns>
        public virtual bool GetMember(ScriptThread thread, VariableSymbol variable)
        {
            return false;
        }

        /// <summary>
        ///     Invoked when a script wants to get the value of an indexed member of this object.
        /// </summary>
        /// <param name="thread">Script thread that invoked this function.</param>
        /// <param name="variable">Symbol describing the member that it wants get.</param>
        /// <param name="index">Index of member it want to get.</param>
        /// <returns>True if successfull or false if not.</returns>
        public virtual bool GetMember(ScriptThread thread, VariableSymbol variable, RuntimeValue indexer)
        {
            return false;
        }

        #endregion
    }

    /// <summary>
    ///     
    /// </summary>
    public abstract class NativeObject : RuntimeObject
    {
        #region Members
        #region Variables

        protected object _nativeObject = null;

        #endregion
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public object Object
        {
            get { return _nativeObject; }
            set { _nativeObject = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public override void Deallocate()
        {
            _nativeObject = null;
        }

        #endregion
    }

	/// <summary>
	///		Used by the a process to store details on a stack entry or 
	///		instruction operand.
	/// </summary>
	public class RuntimeValue
	{
		#region Members
		#region Variables

		private RuntimeValueType _valueType;
		private DataTypeValue _dataType;

		private string _stringLiteral = "";
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
		private int _memoryIndex = -1;

		private Symbol _symbol;

		private int _objectIndex = -1;

        private int _referenceCount = 0;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the value type used by this runtime value.
		/// </summary>
		public RuntimeValueType ValueType
		{
			get { return _valueType; }
			set { _valueType = value; }
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

		/// <summary>
		///		Gets or sets the symbol associated with this operand.
		/// </summary>
		public Symbol Symbol
		{
			get { return _symbol; }
			set { _symbol = value; }
		}

		/// <summary>
		///		Gets or sets the index of Object object associated with this value.
		/// </summary>
		public int ObjectIndex
		{
			get { return _objectIndex; }
			set 
            { 
                _objectIndex = value; 
            }
		}

		/// <summary>
		///		Gets or sets the data type tracker, mainly used when minipulating arrays.
		/// </summary>
		public DataTypeValue DataType
		{
			get { return _dataType; }
			set { _dataType = value; }
		}

        /// <summary>
        ///     Gets or sets the reference count if this value is used as a memory boundry.
        /// </summary>
        public int ReferenceCount
        {
            get { return _referenceCount; }
            set { _referenceCount = value; }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Converts the value of this instance into a textural format.
		/// </summary>
		/// <returns>Textural format of this value.</returns>
		public override string ToString()
		{
			string text = _valueType.ToString() + ": ";
			switch (_valueType)
			{
				case RuntimeValueType.BooleanLiteral: text += _booleanLiteral; break;
				case RuntimeValueType.ByteLiteral: text += _byteLiteral; break;
				case RuntimeValueType.DirectMemory: text += _memoryIndex; break;
				case RuntimeValueType.DirectMemoryIndexed: text += _memoryIndex + " + r"+((int)_register)+"]"; break;
				case RuntimeValueType.DirectStack: text += _stackIndex; break;
				case RuntimeValueType.DirectStackIndexed: text += _stackIndex + " + r"+((int)_register)+"]"; break;
				case RuntimeValueType.DoubleLiteral: text += _doubleLiteral; break;
				case RuntimeValueType.FloatLiteral: text += _floatLiteral; break;
				case RuntimeValueType.IndirectMemory: text += "r" + ((int)_register) + "[" + _memoryIndex + "]"; break;
				case RuntimeValueType.IndirectMemoryIndexed: text += "r" + ((int)_register) + "[" + _memoryIndex + " + r" + ((int)_offsetRegister) + "]"; break;
				case RuntimeValueType.IndirectStack: text += "r" + ((int)_register) + "[" + _memoryIndex + "]"; break;
				case RuntimeValueType.IndirectStackIndexed: text += "r" + ((int)_register) + "[" + _memoryIndex + " + r" + ((int)_offsetRegister) + "]"; break;
				case RuntimeValueType.InstrIndex: text += _instrIndex; break;
				case RuntimeValueType.IntegerLiteral: text += _integerLiteral; break;
				case RuntimeValueType.LongLiteral: text += _longLiteral; break;
				case RuntimeValueType.Object: text += _objectIndex; break;
				case RuntimeValueType.Register: text += "r"+((int)_register); break;
				case RuntimeValueType.ReturnAddress: text += _instrIndex; break;
				case RuntimeValueType.ShortLiteral: text += _shortLiteral; break;
				case RuntimeValueType.StringLiteral: text += _stringLiteral == null ? "" : _stringLiteral; break;
				case RuntimeValueType.SymbolIndex: text += _symbolIndex; break;
			}
			return text;
		}

		/// <summary>
		///		Nullifys all values in this instance.
		/// </summary>
		public void Clear()
		{
			_booleanLiteral = false;
			_byteLiteral = 0;
			_doubleLiteral = 0;
			_floatLiteral = 0;
			_integerLiteral = 0;
			_longLiteral = 0;
			_memoryIndex = -1;
			_shortLiteral = 0;
			_stringLiteral = "";
			_objectIndex = -1;
            _referenceCount = 0;
            _valueType = RuntimeValueType.Invalid;
            _dataType = null;
		}

        /// <summary>
        ///     Creates an exact clone of this value.
        /// </summary>
        /// <returns>Exact clone of this value.</returns>
        public RuntimeValue Clone()
        {
            RuntimeValue value = new RuntimeValue(RuntimeValueType.Invalid);
            CopyTo(value);
            return value;
        }

        /// <summary>
        ///		Copys this value to another.
        /// </summary>
        public void CopyTo(RuntimeValue value)
        {
            value._booleanLiteral = _booleanLiteral;
            value._byteLiteral = _byteLiteral;
            value._doubleLiteral = _doubleLiteral;
            value._floatLiteral = _floatLiteral;
            value._integerLiteral = _integerLiteral;
            value._longLiteral = _longLiteral;
            value._memoryIndex = _memoryIndex;
            value._shortLiteral = _shortLiteral;
            value._stringLiteral = _stringLiteral;
            value._objectIndex = _objectIndex;
            value._instrIndex = _instrIndex;
            value._symbolIndex = _symbolIndex; 
            value._symbol = _symbol;
            value._stackIndex = _stackIndex;
            value._register = _register;
            value._offsetRegister = _offsetRegister;
            value._valueType = _valueType;
            value._dataType = _dataType;
            value._referenceCount = _referenceCount;
        }

		/// <summary>
		///		Returns true if this value is currently null (anything
		///		equal to 0 or "" is).
		/// </summary>
		/// <returns>Tru if this value is null.</returns>
		public bool IsNull()
		{
			switch (_valueType)
			{
				case RuntimeValueType.Object:			if (_objectIndex == -1)			return true;	break;
				case RuntimeValueType.BooleanLiteral:	if (_booleanLiteral == false)	return true;	break;
				case RuntimeValueType.ByteLiteral:		if (_byteLiteral == 0)			return true;	break;
				case RuntimeValueType.DoubleLiteral:	if (_doubleLiteral == 0)		return true;	break;
				case RuntimeValueType.FloatLiteral:		if (_floatLiteral == 0)			return true;	break;
				case RuntimeValueType.IntegerLiteral:	if (_integerLiteral == 0)		return true;	break;
				case RuntimeValueType.LongLiteral:		if (_longLiteral == 0)			return true;	break;
				case RuntimeValueType.ShortLiteral:		if (_shortLiteral == 0)			return true;	break;
				case RuntimeValueType.StringLiteral:	if (_stringLiteral == "")		return true;	break;
			}
			return false;
		}

		/// <summary>
		///		Initializes an instance of this class.
		/// </summary>
		/// <param name="type">Type of value this runtime value should store.</param>
		public RuntimeValue(RuntimeValueType type)
		{
			_valueType = type;
		}

		#endregion
	}

	/// <summary>
	///		Used by a script thread to store local values and keep track
	///		of function frames.
	/// </summary>
	public class RuntimeStack
	{
		#region Members
		#region Variables

		private RuntimeValue[] _stack;
		private int _topIndex, _frameIndex;

		#endregion
		#region Properties

		/// <summary>
		///		Get or sets the frame index of this stack.
		/// </summary>
		public int FrameIndex
		{
			get { return _frameIndex; }
			set { _frameIndex = value; }
		}

		/// <summary>
		///		Get or sets the top index of this stack.
		/// </summary>
		public int TopIndex
		{
			get { return _topIndex;  }
			set { _topIndex = value; }
		}

		/// <summary>
		///		Gets or sets an element in the stack.
		/// </summary>
		/// <param name="index">Index of element to get or set.</param>
		/// <returns>The runtime value at index.</returns>
		public RuntimeValue this[int index]
		{
			get { return _stack[ResolveIndex(index)]; }
			set { _stack[ResolveIndex(index)] = value; }
		}

		/// <summary>
		///		Gets an array containing the raw stack data.
		/// </summary>
		public RuntimeValue[] RawStack
		{
			get { return _stack;  }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Resolves a stack index to an array index.
		/// </summary>
		/// <param name="index">Index to resolve.</param>
		/// <returns>Resolved stack index.</returns>
		public int ResolveIndex(int index)
		{
			return index < 0 ? _frameIndex + index : index;
		}

		/// <summary>
		///		Pop a value of the top of the stack and returns it.
		/// </summary>
		/// <returns>Value popped of the top of the stack.</returns>
		public RuntimeValue Pop()
		{
			return _stack[--_topIndex];
		}

		/// <summary>
		///		Returns the value at the top of the stack without popping it off.
		/// </summary>
		/// <returns>Value at the top of the stack.</returns>
		public RuntimeValue Peek()
		{
			return _stack[_topIndex - 1];
		}

		/// <summary>
		///		Pushs a new value onto the top of the stack.
		/// </summary>
		/// <param name="value">Value to push onto stack.</param>
		public RuntimeValue Push(RuntimeValue value)
		{
			if (_topIndex >= _stack.Length - 1)
				Resize(_stack.Length + 1);
			_stack[_topIndex++] = value;
			return value;
		}

		/// <summary>
		///		Pushs an empty element onto the stack.
		/// </summary>
		/// <param name="type">Type of empty value to push onto stack.</param>
		public RuntimeValue PushEmpty(RuntimeValueType type)
		{
			if (_topIndex >= _stack.Length - 1) 
				Resize(_stack.Length + 1);
			_stack[_topIndex] = new RuntimeValue(type);
			return _stack[_topIndex++];
		}

		/// <summary>
		///		Pushs a new stack frame onto the stack. A stackframe is used to 
		///		decide how stack index's should be resolved.
		/// </summary>
		/// <param name="size">Size of stack frame to push onto stack.</param>
		public void PushFrame(int size)
		{
			_topIndex += size;
			_frameIndex = _topIndex;
            if (_topIndex - 1 >= _stack.Length - 1)
                Resize(_topIndex);
        }

		/// <summary>
		///		Pops a stack frame of the given size of the stack. A stackframe is used to 
		///		decide how stack index's should be resolved.
		/// </summary>
		/// <param name="size">Size of stack frame to pop of stack.</param>
		public void PopFrame(int size)
		{
			_topIndex -= size;
		}

		/// <summary>
		///		Clears all elements out of this stack.
		/// </summary>
		public void Clear()
		{
			for (int i = 0; i < _stack.Length; i++)
					_stack[i] = new RuntimeValue(0);
            _topIndex = 0;
            _frameIndex = 0;
		}

		/// <summary>
		///		Resizes the stack so it can store a given capacity 
		///		of runtime values.
		/// </summary>
		/// <param name="capacity">New capacity of stack.</param>
		public void Resize(int capacity)
		{
			if (_stack == null)
				_stack = new RuntimeValue[capacity];
			else
				Array.Resize<RuntimeValue>(ref _stack, capacity);

            for (int i = 0; i < _stack.Length; i++)
                if (_stack[i] == null) _stack[i] = new RuntimeValue(0);
		}

		/// <summary>
		///		Initializes a new instance of this class and sets its initial capacity
		///		to the given capacity.
		/// </summary>
		/// <param name="capacity">Initial capacity of this stack.</param>
		public RuntimeStack(int capacity)
		{
			Resize(capacity);
		}

		#endregion
	}

	/// <summary>
	///		Used to describe a callable native function delegate.
	/// </summary>
	/// <param name="thread">Thread that called this function.</param>
	public delegate void FunctionDelegate(ScriptThread thread);

	/// <summary>
	///		Wraps all the details of a callable native function.
	/// </summary>
	public sealed class NativeFunction
	{
		#region Members
		#region Variables

		private string			 _identifier;
		private FunctionDelegate _delegate;
		private DataTypeValue	 _returnType;
		private DataTypeValue[]	 _parameterTypes;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the identifier used to call this function.
		/// </summary> 
		public string Identifier
		{
			get { return _identifier;  }
			set { _identifier = value; }
		}

		/// <summary>
		///		Gets or sets the delegate used to call this native function
		/// </summary> 
		public FunctionDelegate Delegate
		{
			get { return _delegate;  }
			set { _delegate = value; }
		}

		/// <summary>
		///		Gets or sets the data type this function returns.
		/// </summary>
		public DataTypeValue ReturnType
		{
			get { return _returnType;  }
			set { _returnType = value; }
		}

		/// <summary>
		///		Get or sets the array of parameter data types taken by this function.
		/// </summary>
		public DataTypeValue[] ParameterTypes
		{
			get { return _parameterTypes;  }
			set { _parameterTypes = value; }
		}

		#endregion
		#endregion
		#region Methods

        /// <summary>
        ///     Converts this class to a textural format.
        /// </summary>
        /// <returns>Textural format of this class</returns>
        public override string ToString()
        {
            string name = "native " + _returnType.ToString() + " " + _identifier + "(";
            for (int i = 0; i < _parameterTypes.Length; i++)
                name += ((DataTypeValue)_parameterTypes[i]).ToString() + ((i < _parameterTypes.Length - 1) ? ", " : "");
            return name + ")";
        }

		/// <summary>
		///		Checks if a given parameter data type can be applied to this parameter.
		/// </summary>
		/// <param name="index">Index of parameter to check.</param>
		/// <returns>Boolean describing if the data type is valid.</returns>
		public bool CheckParameterTypeValid(int index, DataTypeValue value)
		{
			return (_parameterTypes[index] == value || value.DataType == DataType.Null);
		}

		/// <summary>
		///		Initializes a new instance of this class.
		/// </summary>
		/// <param name="identifier">Identifier used to call this function from a script.</param>
		/// <param name="nativeFunctionDelegate">Delegate of function you wish to be called when function is called from the script.</param>
		/// <param name="returnType">Data type that this function returns.</param>
		/// <param name="parameterTypes">Array of parameter data types this function uses.</param>
		public NativeFunction(string identifier, FunctionDelegate nativeFunctionDelegate, DataTypeValue returnType, DataTypeValue[] parameterTypes)
		{
			_identifier = identifier;
			_delegate = nativeFunctionDelegate;
			_returnType = returnType;
			_parameterTypes = parameterTypes;
		}

		#endregion
	}

	/// <summary>
	///		Used inside NativeFunctionSet classes to describe how a native function
	///		should be called from inside a script.
	/// </summary>
	[AttributeUsage(AttributeTargets.All)] 
	public class NativeFunctionInfo : Attribute
	{
		#region Members
		#region Variables

		private string _name;
		private DataTypeValue _returnType;
		private DataTypeValue[] _parameterTypes;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the identifier used inside a script to call this function.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		///		Gets or sets the data type that this function returns to the script.
		/// </summary>
		public DataTypeValue ReturnType
		{
			get { return _returnType; }
			set { _returnType = value; }
		}

		/// <summary>
		///		Gets or sets the parameters that this function accepts.
		/// </summary>
		public DataTypeValue[] ParameterTypes
		{
			get { return _parameterTypes; }
			set { _parameterTypes = value; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Creates a new instance of this class with the given data.
		/// </summary>
		/// <param name="name">Identifier used inside a script to call this function</param>
		/// <param name="returnType">Data type that this function returns to the script</param>
		/// <param name="parameterTypes">Parameters that this function accepts</param>
		public NativeFunctionInfo(string name, string returnType, string parameterTypes)
		{
			_name = name;
			_returnType = DataTypeValue.FromMnemonic(returnType);

			if (parameterTypes != "")
			{
				string[] parameterTypesSplit = parameterTypes.Split(new Char[] { ',' });
				_parameterTypes = new DataTypeValue[parameterTypesSplit.Length];

                for (int i = 0; i < parameterTypesSplit.Length; i++)
                {
                    if (parameterTypesSplit[i].EndsWith("[]"))
                    {
                        _parameterTypes[i] = DataTypeValue.FromMnemonic(parameterTypesSplit[i].Substring(0, parameterTypesSplit[i].Length - 2));
                        _parameterTypes[i].IsArray = true;
                    }
                    else
                        _parameterTypes[i] = DataTypeValue.FromMnemonic(parameterTypesSplit[i]);
                }
            }
			else
				_parameterTypes = new DataTypeValue[0];
		}

		#endregion
	}

	/// <summary>
	///		All function sets are derived from this, to make it easier
	///		to preform mass-operations.	
	/// </summary>
	public class NativeFunctionSet
	{
		#region Members
		#region Variables

		private static ArrayList _globalFunctionSets = new ArrayList();

		private ArrayList _nativeFunctions = new ArrayList();
		private bool _prepared = false;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the global pool of function sets being used.
		/// </summary>
		public static ArrayList GlobalFunctionSets
		{
			get { return _globalFunctionSets; }
			set { _globalFunctionSets = value; }
		}

		/// <summary>
		///		Gets or sets the list of native functions this set contains.
		/// </summary>
		public ArrayList NativeFunctions
		{
			get { return _nativeFunctions; }
			set { _nativeFunctions = value; }
		}

		#endregion
		#endregion
		#region Register Functions

		/// <summary>
		///		Creates a new instance of this class and calls the PrepareSet method.
		/// </summary>
		public NativeFunctionSet()
		{
			PrepareSet();
		}

		/// <summary>
		///		Parses through this class to find any methods that can be registered 
		///		to a Virtual Machine.
		/// </summary>
		public void PrepareSet()
		{
			if (_prepared == true) return;
			_prepared = true;
			
			_globalFunctionSets.Add(this);

			Type type = this.GetType();
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
		
			foreach (MethodInfo method in methods)
			{
				// Go through attributes until we find a NativeFunctionInfo attribute
				// if we don't find one then ignore this function.
				NativeFunctionInfo infoAttribute = method.GetCustomAttributes(typeof(NativeFunctionInfo), true)[0] as NativeFunctionInfo;
				if (infoAttribute == null) continue;

				// Create a native function delegate for this method.
				NativeFunction function = new NativeFunction(infoAttribute.Name, (FunctionDelegate)FunctionDelegate.CreateDelegate(typeof(FunctionDelegate), this, method), infoAttribute.ReturnType, infoAttribute.ParameterTypes);
				_nativeFunctions.Add(function);
			}
		}

		/// <summary>
		///		Registers all the native functions in this set to a VM.
		/// </summary>
		/// <param name="virtualMachine">VM to register native functions to.</param>
		public void RegisterToVirtualMachine(VirtualMachine virtualMachine)
		{
			foreach (NativeFunction function in _nativeFunctions)
				virtualMachine.RegisterNativeFunction(function);
		}

		///		Registers all the native functions in all the global sets to a VM.
		/// </summary>
		/// <param name="virtualMachine">VM to register native functions to.</param>
		public static void RegisterCommandSetsToVirtualMachine(VirtualMachine virtualMachine)
		{
			foreach (NativeFunctionSet set in _globalFunctionSets)
				set.RegisterToVirtualMachine(virtualMachine);
		}

		#endregion
	}

	/// <summary>
	///		This is the heart of this scripting language, it is
	///		responsible for running each thread assigned to a script
	///		process.
	/// </summary>
	public sealed class VirtualMachine
	{
		#region Members
		#region Variables

		private static ArrayList _globalNativeFunctions = new ArrayList();

		private ArrayList _processes = new ArrayList();

        private Hashtable _nativeFunctions = new Hashtable();

		#endregion
		#region Properties

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Runs every process's thread's attached to this virtual machine.
		/// </summary>
		/// <param name="timeslice">If equal to -1 the processes will be given infinite time.</param>
		public void RunProcesses(int timeslice)
		{
			// Ignore if we have no processes attached to this VM.
			if (_processes.Count == 0) return;

			// Work out how much time we have to run our processes.
			int processTimeslice = timeslice;

			// Add up the prioritys and work out how much each prioriy
			// segment is allowed.
			int priorityCount = 0;
			foreach (ScriptProcess process in _processes)
			{
				if (process.Priority == Priority.RealTime) continue;
				priorityCount += (int)process.Priority;
			}
			float timePerPrioritySegment = 0;
			if (priorityCount > 0) timePerPrioritySegment = (float)processTimeslice / (float)priorityCount;

			// Go through each process and tell it to run.
			foreach (ScriptProcess process in _processes)
			{
				int originalCount = _processes.Count;
				if (process.Priority != Priority.RealTime && processTimeslice != -1)
					process.Run((int)Math.Ceiling(timePerPrioritySegment * (float)(int)process.Priority));
				else
					process.Run(-1);
				if (_processes.Count != originalCount) break;
			}				
		}
		public void RunProcesses()
		{
			RunProcesses(-1);
		}

		/// <summary>
		///		Loads a new script and creates a process out of it, then associates it
		///		with this virtual machine.
		/// </summary>
		/// <param name="url">Url of file this script is in.</param>
        /// <param name="cache">If set to true this scripts byte code will be cached for quick loading.</param>
		public ScriptProcess LoadScript(object url, bool cache)
		{
            DebugLogger.WriteLog("Loading script into virtual machine from " + url);

            HighPreformanceTimer timer = new HighPreformanceTimer();

            if (Resources.ResourceManager.ResourceIsCached(url.ToString()))
            {
                ScriptProcess oldProcess = (ScriptProcess)Resources.ResourceManager.RetrieveResource(url.ToString());

                // Create process and hand loading over to it.
                ScriptProcess process = new ScriptProcess(this, (ScriptProcess)oldProcess);
                process.Url = url as string;
                _processes.Add(process);

                DebugLogger.WriteLog("Loaded script from cache in "+timer.DurationMillisecond+".");

                return process;
            }

			// Open a stream so we can read in the script.
			Stream stream = StreamFactory.RequestStream(url, StreamMode.Open);
			if (stream == null) return null;
			BinaryReader reader = new BinaryReader(stream);

			// Check if this script is already compiled or not.
			if (reader.ReadByte() == 'C' && reader.ReadByte() == 'R' && reader.ReadByte() == 'X')
			{
				// Clean up our file handle.
				reader.Close();
				stream.Close();

				// Create process and hand loading over to it.
				ScriptProcess process = new ScriptProcess(this, url);
				process.Url = url as string;
				_processes.Add(process);

                if (cache == true) Resources.ResourceManager.CacheResource(url.ToString(), process);

                DebugLogger.WriteLog("Script loaded in " + timer.DurationMillisecond + ".");
				return process;
			}
			else
			{
				// Clean up our file handle.
				reader.Close();

				// Compiler script to byte code.
				ScriptCompiler compiler = new ScriptCompiler();
                if (compiler.Compile(url) > 0)
                {
                    bool serious = false;
                    foreach (CompileError error in compiler.ErrorList)
                        if (error.AlertLevel == ErrorAlertLevel.Error || error.AlertLevel == ErrorAlertLevel.FatalError)
                            serious = true;

                    if (serious == true)
                    {
                        DebugLogger.WriteLog(compiler.ErrorList.Count + " error(s) occured while compiling script from " + url.ToString() + ".");
                        foreach (CompileError error in compiler.ErrorList)
                            DebugLogger.WriteLog("\t" + error.ToString());
                        return null;
                    }
                }

				// Create a memory stream and dump the byte code into it.
				Stream memoryStream = new MemoryStream();
				if (memoryStream == null) return null;
				BinaryWriter memoryWriter = new BinaryWriter(memoryStream);
				BinaryReader memoryReader = new BinaryReader(memoryStream);

				compiler.DumpExecutableFile(memoryWriter);

				// Create process and hand loading over to it.
                memoryStream.Position = 0;
				ScriptProcess process = new ScriptProcess(this, memoryReader);
				process.Url = url as string;
				_processes.Add(process);

				memoryReader.Close();
				memoryWriter.Close();
				memoryStream.Close();
				stream.Close();

                if (cache == true) Resources.ResourceManager.CacheResource(url.ToString(), process);
                DebugLogger.WriteLog("Script loaded in " + timer.DurationMillisecond + ".");
				return process;
			}
		}
        public ScriptProcess LoadScript(object url)
        {
            return LoadScript(url, true);
        }

		/// <summary>
		///		Removes a previously attached process from this VM.
		/// </summary>
		/// <param name="process">Process to remove.</param>
		public void DetachProcess(ScriptProcess process)
		{
			_processes.Remove(process);
		}

		/// <summary>
		///		Attachs a process to this VM.
		/// </summary>
		/// <param name="process">Process to attach.</param>
		public void AttachProcess(ScriptProcess process)
		{
			_processes.Add(process);
		}

		/// <summary>
		///		Removes all previously attached process from this VM.
		/// </summary>
		public void ClearProcesses()
		{
			_processes.Clear();
		}

		/// <summary>
		///		Registers a native function that can be imported and used by script processes.
		/// </summary>
		public void RegisterNativeFunction(string identifier, FunctionDelegate functionDelegate, DataTypeValue returnType, params DataTypeValue[] parameterTypeList)
		{
            string fullName = identifier + "(";
            for (int i = 0; i < parameterTypeList.Length; i++)
                fullName += parameterTypeList[i] + (i < parameterTypeList.Length - 1 ? "," : "");
            fullName += ")";

            _nativeFunctions.Add(fullName, new NativeFunction(identifier, functionDelegate, returnType, parameterTypeList));
		}
		public void RegisterNativeFunction(NativeFunction function)
		{
            string fullName = function.Identifier + "(";
            for (int i = 0; i < function.ParameterTypes.Length; i++)
                fullName += function.ParameterTypes[i] + (i < function.ParameterTypes.Length - 1 ? "," : "");
            fullName += ")";

            _nativeFunctions.Add(fullName, function);
		}

		/// <summary>
		///		Unregisters any native function's that have the same identifier, return type 
		///		and parameter types as those passed.
		/// </summary>
		/// <param name="identifier">Identifier of function to unregister.</param>
		/// <param name="parameterTypes">Array of parameter types of function to unregister.</param>
		public void UnregisterNativeFunction(string identifier, params DataTypeValue[] parameterTypes)
		{
            string fullName = identifier + "(";
            for (int i = 0; i < parameterTypes.Length; i++)
                fullName += parameterTypes[i] + (i < parameterTypes.Length - 1 ? "," : "");
            fullName += ")";

            _nativeFunctions.Remove(fullName);
        }
		public void UnregisterNativeFunction(NativeFunction function)
		{
            string fullName = function.Identifier + "(";
            for (int i = 0; i < function.ParameterTypes.Length; i++)
                fullName += function.ParameterTypes[i] + (i < function.ParameterTypes.Length - 1 ? "," : "");
            fullName += ")";

            _nativeFunctions.Remove(fullName);
		}

		/// <summary>
		///		Finds a native function that has the same identifier, return type and
		///		parameter types as those passed.
		/// </summary>
		/// <param name="identifier">Identifier of function to find.</param>
		/// <param name="returnType">Return type of function to find.</param>
		/// <param name="parameterTypes">Array of parameter types of function to find.</param>
		/// <returns>Null if no function is found, if one is then it is returned.</returns>
		public NativeFunction FindNativeFunction(string identifier, DataTypeValue[] parameterMask)
		{
            string fullName = identifier + "(";
            for (int i = 0; i < parameterMask.Length; i++)
                fullName += parameterMask[i] + (i < parameterMask.Length - 1 ? "," : "");
            fullName += ")";

            /*
			foreach (NativeFunction function in _nativeFunctions[(int)identifier[0]])
			{
				if (function.Identifier.ToLower() != identifier.ToLower()) continue;
				if (function.ParameterTypes.Length != parameterMask.Length) continue;

				bool paramsValid = true;
				for (int i = 0; i < parameterMask.Length; i++)
					if (function.CheckParameterTypeValid(i, parameterMask[i]) == false)
						paramsValid = false;
				if (paramsValid == false) continue;
				return function;
			}
            */

           // System.Console.WriteLine("LOoking for " + fullName + " found = " + (_nativeFunctions[fullName] != null));

            return _nativeFunctions[fullName] as NativeFunction;
		}

		/// <summary>
		///		Changes the current state of every process attached to this VM.
		/// </summary>
		/// <param name="state">Identifier of state to switch to.</param>
		public void ChangeState(string state)
		{
			foreach (ScriptProcess process in _processes)
				process.ChangeState(state);
		}

		/// <summary>
		///		Invokes a given function in all processes attached to this VM.
		/// </summary>
		/// <param name="identifier">Identifier of function to invoke.</param>
		/// <param name="ignoreThread">If set all thread spawning flags associated with this function will be ignored.</param>
		/// <param name="waitForReturn">If set to true this function will not return until the function has executed fully.</param>
		public void InvokeFunction(string identifier, bool waitForReturn, bool ignoreThread)
		{
			foreach (ScriptProcess process in _processes)
				process.InvokeFunction(identifier, waitForReturn, ignoreThread);
		}

		/// <summary>
		///		Invokes a garbage collection on all processes attached to this VM.
		/// </summary>
		public void CollectGarbage()
		{
			foreach (ScriptProcess process in _processes)
				process.CollectGarbage();
		}

		/// <summary>
		///		Creates a new instance of this class.
		/// </summary>
		/// <param name="registerSets">If set to true all instances of the NativeFunctionSet class will be registered to this instance.</param>
		public VirtualMachine(bool registerSets)
		{
			if (registerSets == true) NativeFunctionSet.RegisterCommandSetsToVirtualMachine(this);
		}

		#endregion
	}

	/// <summary>
	///		Used to describe a callable function that can be called when the state
    ///     of a process is changed..
	/// </summary>
	/// <param name="process">Process that had its state changed.</param>
    /// <param name="sate">New state.</param>
	public delegate void StateChangeDelegate(ScriptProcess process, StateSymbol state);

	/// <summary>
	///		Encapsulates all details about a loaded (and running) script process,
	///		it also manages all the threads attached to this process.
	/// </summary>
	public sealed class ScriptProcess
	{
		#region Members
		#region Variables

		private VirtualMachine _virtualMachine;

		private ArrayList _threads = new ArrayList();
		private ArrayList _defineList = new ArrayList();
		private Priority  _priority = Priority.Normal;

		private CompileFlags _compileFlags;
		private int _internalVariableIndex;
		private int _memorySize;
		private FunctionSymbol _globalScope, _memberScope;

		private Symbol[] _symbolList;
		private RuntimeInstruction[] _instructionList;

		private RuntimeValue[] _memoryHeap = new RuntimeValue[512];
        private RuntimeObject[] _objectHeap = new RuntimeObject[512];

		private StateSymbol _currentState;
		private int _defaultEngineStateIndex, _defaultEditorStateIndex;

		private HighPreformanceTimer _garbageCollectionTimer = new HighPreformanceTimer();
        private int _lastCollectionMemoryCount = 0, _lastCollectionObjectCount = 0;

		private string _url = "";

		#endregion
		#region Properties

		/// <summary>
		///		Gets the default state used by the engine.
		/// </summary>
		public StateSymbol DefaultEngineState
		{
			get 
			{
				if (_defaultEngineStateIndex == -1) return null;
				return _symbolList[_defaultEngineStateIndex] as StateSymbol; 
			}
		}

		/// <summary>
		///		Gets the default state used by the editor.
		/// </summary>
		public StateSymbol DefaultEditorState
		{
			get 
			{
				if (_defaultEditorStateIndex == -1) return null;
				return _symbolList[_defaultEditorStateIndex] as StateSymbol; 
			}
		}
		
		/// <summary>
		///		Gets or sets the url this process's script was loaded from.
		/// </summary>
		public string Url
		{
			get { return _url; }
			set { _url = value; }
		}

		/// <summary>
		///		Gets or sets the symbol representing the global scope.
		/// </summary>
		public FunctionSymbol GlobalScope
		{
			get { return _globalScope;  }
			set { _globalScope = value; }
		}

		/// <summary>
		///		Gets or sets the virtual machine associated with this process.
		/// </summary>
		public VirtualMachine VirtualMachine
		{
			get { return _virtualMachine;  }
			set { _virtualMachine = value; }
		}

		/// <summary>
		///		Gets or sets the thread at the given index.
		/// </summary>
		/// <param name="index">Index of thread to get or set.</param>
		/// <returns>Thread at given index.</returns>
		public ScriptThread this[int index]
		{
			get { return (ScriptThread)_threads[index]; }
			set { _threads[index] = value; }
		}

		/// <summary>
		///		Gets or sets the list of threads attached to this process.
		/// </summary>
		public ArrayList Threads
		{
			get { return _threads;  }
			set { _threads = value; }
		}

		/// <summary>
		///		Gets or sets the list of defines declared in this script.
		/// </summary>
		public ArrayList Defines
		{
			get { return _defineList; }
			set { _defineList = value; }
		}

		/// <summary>
		///		Retrieves the symbol list.
		/// </summary>
		public Symbol[] Symbols
		{
			get { return _symbolList; }
		}

		/// <summary>
		///		Retrieves the instruction list.
		/// </summary>
		public RuntimeInstruction[] Instructions
		{
			get { return _instructionList; }
		}

		/// <summary>
		///		Retrieves the memory heap used by this process to store globals
		///		and dynamically allocated data.
		/// </summary>
		public RuntimeValue[] MemoryHeap
		{
			get { return _memoryHeap; }
		}

		/// <summary>
		///		Retrieves the Object heap used by this process to store ingame
		///		Objects.
		/// </summary>
		public RuntimeObject[] ObjectHeap
		{
			get { return _objectHeap; }
		}

		/// <summary>
		///		Gets or sets the priority this process should be given when
		///		running multiple processes.
		/// </summary>
		public Priority Priority
		{
			get { return _priority; }
			set { _priority = value; }
		}

		/// <summary>
		///		Returns a state symbol representing the state this function is currently in.
		/// </summary>
		public StateSymbol State
		{
			get { return _currentState;  }
			set { ChangeState(value); }
		}

		#endregion
        #region Events

        /// <summary>
        ///     Invoked when the state of this process is changed.
        /// </summary>
        public event StateChangeDelegate OnStateChange;

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Copys this processes data to another process.
        /// </summary>
        /// <param name="process">Process to copy to.</param>
        public void CopyTo(ScriptProcess process)
        {
            process._defineList = _defineList;
            process._internalVariableIndex = _internalVariableIndex;
            process._memorySize = _memorySize;
            process._globalScope = _globalScope;
            process._memberScope = _memberScope;
            process._symbolList = _symbolList;
            process._instructionList = _instructionList;
            //process._memoryHeap = new RuntimeObject[_memoryHeap.Length];
            //process._objectHeap = new RuntimeObject[_objectHeap.Length];
            //process._currentState = _currentState;
            process._defaultEditorStateIndex = _defaultEditorStateIndex;
            process._defaultEngineStateIndex = _defaultEngineStateIndex;
            process._url = _url;


            // Create a new memory heap of the correct size
            for (int i = 0; i < _memorySize; i++)
                process._memoryHeap[i] = new RuntimeValue(RuntimeValueType.Invalid);

            // Set the 'special' globals to their appropriate values.
            //process._memoryHeap[0].ValueType = RuntimeValueType.Object;
            //process._memoryHeap[0].ObjectIndex = -1;
        }

        /// <summary>
        ///     Starts all threads attached to this process.
        /// </summary>
        public void Start()
        {
            foreach (ScriptThread thread in _threads)
                thread.Start();
        }

        /// <summary>
        ///     Stops all threads attached to this process.
        /// </summary>
        public void Stop()
        {
            foreach (ScriptThread thread in _threads)
                thread.Stop();
        }

        /// <summary>
        ///     Pauses all threads attached to this process.
        /// </summary>
        public void Pause(int length)
        {
            foreach (ScriptThread thread in _threads)
                thread.Pause(length);
        }

        /// <summary>
        ///     Resumes all threads attached to this process.
        /// </summary>
        public void Resume()
        {
            foreach (ScriptThread thread in _threads)
                thread.Resume();
        }

        /// <summary>
        ///     Attempts to find and retrieve a symbol with the given identifier and type.
        /// </summary>
        /// <param name="ident">Identifier of symbol.</param>
        /// <param name="type">Type of symbol, if set to 0 all types are valid.</param>
        /// <returns>Symbol with same identifier as one passed if it exists; else null.</returns>
        public Symbol GetSymbol(string ident, SymbolType type)
        {
            foreach (Symbol symbol in _symbolList)
                if (symbol != null && (type == 0 || (symbol.Type & type) != 0) && symbol.Identifier == ident)
                    return symbol;
            return null;
        }
        public Symbol GetSymbol(string ident)
        {
            return GetSymbol(ident, 0);
        }

        /// <summary>
        ///     Checks if a symbol exists with the given identifier and type.
        /// </summary>
        /// <param name="ident">Identifier of symbol.</param>
        /// <param name="type">Type of symbol, if set to 0 all types are valid.</param>
        /// <returns>True if symbol exists; else false.</returns>
        public bool SymbolExists(string ident, SymbolType type)
        {
            return (GetSymbol(ident, type) != null);
        }
        public bool SymbolExists(string ident)
        {
            return SymbolExists(ident, 0);
        }

		/// <summary>
		///		Runs all threads associated with this process.
		/// </summary>
		/// <param name="timeslice">Timeslice in milliseconds this process is allow to use while running.</param>
		public void Run(int timeslice)
		{
			// Ignore if we have no threads attached to this process.
			if (_threads.Count == 0) return;

			// Check if we need to clean up any garbage first.
			if (_garbageCollectionTimer.DurationMillisecond > 1000)
			{
				_garbageCollectionTimer.Restart();
				CollectGarbage();
			}

			// Work out how much time we have to run our thread.
			int threadTimeslice = timeslice;

			// Add up the prioritys and work out how much each prioriy
			// segment is allowed.
			int priorityCount = 0;
			foreach (ScriptThread thread in _threads)
			{
				if (thread.Priority == Priority.RealTime) continue;
				priorityCount += (int)thread.Priority;
			}
			float timePerPrioritySegment = 0;
			if (priorityCount > 0) timePerPrioritySegment = (float)threadTimeslice / (float)priorityCount;

			// Go through each thread and tell it to run.
			foreach (ScriptThread thread in _threads)
			{
				int originalCount = _threads.Count;
				if (thread.Priority != Priority.RealTime && threadTimeslice != -1)
					thread.Run((int)Math.Ceiling(timePerPrioritySegment * (float)(int)thread.Priority));
				else
					thread.Run(-1);
				if (_threads.Count != originalCount) break;
			}
		}

		/// <summary>
		///		Loads the byte code from a given file url.
		/// </summary>
		/// <param name="url">Url of file to load byte code from.</param>
		public void LoadByteCode(object url)
		{
			if (url is BinaryReader)
			{
				LoadByteCode(url as BinaryReader);
				return;
			}
			else if (url is string)
				_url = (string)url;

			// Open a stream so we can write in the script.
			Stream stream = StreamFactory.RequestStream(url, StreamMode.Open);
			if (stream == null) return;
			BinaryReader reader = new BinaryReader(stream);

			// Dump it into the stream.
			LoadByteCode(reader);

			// Close the stream.
			stream.Close();
			reader.Close();
		}

		/// <summary>
		///		Loads the byte code from a given binary reader.
		/// </summary>
		/// <param name="reader">BinaryReader to load byte code from.</param>
		private void LoadByteCode(BinaryReader reader)
		{
			// Check the header is correct
			if (reader.ReadByte() == 'C' && reader.ReadByte() == 'R' && reader.ReadByte() == 'X')
			{
				// Read in header variables.
				_compileFlags			= (CompileFlags)reader.ReadInt32();
				_internalVariableIndex	= reader.ReadInt32();
				_memorySize				= reader.ReadInt32();
				int globalScopeIndex	= reader.ReadInt32();
                int memberScopeIndex = reader.ReadInt32();
                _defaultEngineStateIndex = reader.ReadInt32();
				_defaultEditorStateIndex = reader.ReadInt32();
				
				// Create a new memory heap of the correct size
				for (int i = 0; i < _memorySize; i++)
					_memoryHeap[i] = new RuntimeValue(RuntimeValueType.Invalid);

				// Set the 'special' globals to their appropriate values.
				_memoryHeap[0].ValueType = RuntimeValueType.Object;
				_memoryHeap[0].ObjectIndex = 0;

				int defineCount = reader.ReadInt32();
				int symbolCount			= reader.ReadInt32();
				int instructionCount	= reader.ReadInt32();
				int debugFileCount		= 0;

				if ((_compileFlags & CompileFlags.Debug) != 0)
					debugFileCount = reader.ReadInt32();

				// Read in debug file list.
				string[] debugFiles = new string[debugFileCount];
				if ((_compileFlags & CompileFlags.Debug) != 0)
				{
					for (int i = 0; i < debugFileCount; i++)
						debugFiles[i] = reader.ReadString();		
				}

				// Read in the define list.
				for (int i = 0; i < defineCount; i++)
				{
					string ident = reader.ReadString();
					TokenID valueID = (TokenID)reader.ReadInt32();
					string value = reader.ReadString();
					Define define = new Define(ident, value, valueID);
					_defineList.Add(define);
				}

				// Read in symbol list.
				_symbolList = new Symbol[symbolCount];
				for (int i = 0; i < symbolCount; i++)
				{
					// Read in general details about symbol.
					SymbolType type	  = (SymbolType)reader.ReadByte();
					string identifier = reader.ReadString();
					int scopeIndex	  = reader.ReadInt32();
					Symbol scope	  = null;
					Symbol symbol	  = null;

					if (scopeIndex != -1)
						scope = (Symbol)_symbolList[scopeIndex];

					// Read in specialized details about symbol.
					switch (type)
					{
						case SymbolType.JumpTarget:
							continue; // Ignore jump targets.

                        case SymbolType.Namespace:
                            NamespaceSymbol namespaceSymbol = new NamespaceSymbol(scope);
                            symbol = namespaceSymbol;
                            break;

						case SymbolType.Enumeration:
							EnumerationSymbol enumSymbol = new EnumerationSymbol(scope);
							symbol = enumSymbol;
							break;

						case SymbolType.String:
							StringSymbol stringSymbol = new StringSymbol(scope, identifier);
							symbol = stringSymbol;
							break;

						case SymbolType.Function:
							FunctionSymbol functionSymbol = new FunctionSymbol(identifier, scope);
							symbol = functionSymbol;
							functionSymbol.EntryPoint = reader.ReadInt32();
							functionSymbol.LocalDataSize = reader.ReadInt16();
							functionSymbol.IsEvent = reader.ReadBoolean();
                            functionSymbol.IsConsole = reader.ReadBoolean();
                            functionSymbol.IsExport = reader.ReadBoolean();
                            functionSymbol.IsImport = reader.ReadBoolean();
							functionSymbol.IsThreadSpawner = reader.ReadBoolean();
                            functionSymbol.IsMember = reader.ReadBoolean();
							functionSymbol.ParameterCount = reader.ReadByte();
							bool isArray = reader.ReadBoolean();
							bool isReference = reader.ReadBoolean();
							functionSymbol.ReturnType = new DataTypeValue((DataType)reader.ReadByte(), isArray, isReference);
                            functionSymbol.AccessModifier = (SymbolAccessModifier)reader.ReadByte();
							break;

						case SymbolType.State:
							StateSymbol stateSymbol = new StateSymbol(scope);
							symbol = stateSymbol;
							stateSymbol.IsEngineDefault = reader.ReadBoolean();
							stateSymbol.IsEditorDefault = reader.ReadBoolean();
							break;

						case SymbolType.Variable:
							VariableSymbol variableSymbol = new VariableSymbol(scope);
							symbol = variableSymbol;
							variableSymbol.DataType		= new DataTypeValue((DataType)reader.ReadByte(), false, false);
							variableSymbol.DataType.IsReference = reader.ReadBoolean();
							variableSymbol.IsArray		= reader.ReadBoolean();
							variableSymbol.DataType.IsArray = variableSymbol.IsArray;
							variableSymbol.IsConstant	= reader.ReadBoolean();
							variableSymbol.MemoryIndex	= reader.ReadInt32();
							variableSymbol.StackIndex	= reader.ReadInt32();
							variableSymbol.VariableType = (VariableType)reader.ReadByte();
							variableSymbol.IsProperty = reader.ReadBoolean();
                            variableSymbol.AccessModifier = (SymbolAccessModifier)reader.ReadByte();
                            variableSymbol.ConstToken = new Token(TokenID.TypeIdentifier, reader.ReadString(), 0, 0, "");
							break;

						case SymbolType.MetaData:
							MetaDataSymbol metaDataSymbol = new MetaDataSymbol(scope, identifier, "");
							symbol = metaDataSymbol;
							metaDataSymbol.Value = reader.ReadString();
							break;
					}

					symbol.Identifier = identifier;
					symbol.Index = i;
					_symbolList[i] = symbol;
				}

				// Retrieve global scope.
				_globalScope = _symbolList[globalScopeIndex] as FunctionSymbol;
                _memberScope = _symbolList[memberScopeIndex] as FunctionSymbol;
                //_currentState = _symbolList[_defaultEngineStateIndex] as StateSymbol; // Force this to be declared in the engine / editor.

				// Read in instruction list.
				_instructionList = new RuntimeInstruction[instructionCount];
				for (int i = 0; i < instructionCount; i++)
				{
					// Read in instruction details and create a new instruction.
					OpCode opCode = (OpCode)reader.ReadByte();
					int operandCount = reader.ReadByte();
					RuntimeInstruction instruction = new RuntimeInstruction(opCode);
					_instructionList[i] = instruction;

					if ((_compileFlags & CompileFlags.Debug) != 0)
					{
						int fileIndex = reader.ReadSByte();
						if (fileIndex != -1) instruction.File = debugFiles[fileIndex];
						instruction.Offset = reader.ReadInt16();
						instruction.Line   = reader.ReadInt16();
					}

					// Read in each operand attached to this instruction
					for (int k = 0; k < operandCount; k++)
					{
						// Read in general details about this operand and create
						// a new runtime value instance.
						RuntimeValueType opType = (RuntimeValueType)reader.ReadInt32();
						RuntimeValue operand = new RuntimeValue(opType);
						instruction.Operands[instruction.OperandCount] = operand;
						instruction.OperandCount++;

						// Read in specialized info about this operand.
						switch(opType)
						{
							case RuntimeValueType.BooleanLiteral:
								operand.BooleanLiteral = reader.ReadBoolean();
								break;
							case RuntimeValueType.ByteLiteral:
								operand.ByteLiteral = reader.ReadByte();
								break;
							case RuntimeValueType.DirectMemory:
								operand.MemoryIndex = reader.ReadInt32();
								break;
							case RuntimeValueType.DirectMemoryIndexed:
                                operand.MemoryIndex = reader.ReadInt32();
								operand.OffsetRegister = (Register)reader.ReadByte();
								break;
							case RuntimeValueType.DirectStack:
                                operand.StackIndex = reader.ReadInt32();
								break;
							case RuntimeValueType.DirectStackIndexed:
                                operand.StackIndex = reader.ReadInt32();
								operand.OffsetRegister = (Register)reader.ReadByte();
								break;
							case RuntimeValueType.DoubleLiteral:
								operand.DoubleLiteral = reader.ReadDouble();
								break;
							case RuntimeValueType.FloatLiteral:
								operand.FloatLiteral = reader.ReadSingle();
								break;
							case RuntimeValueType.IndirectMemory:
								operand.Register = (Register)reader.ReadByte();
								break;
							case RuntimeValueType.IndirectMemoryIndexed:
								operand.Register = (Register)reader.ReadByte();
								operand.OffsetRegister = (Register)reader.ReadByte();
								break;
							case RuntimeValueType.IndirectStack:
								operand.Register = (Register)reader.ReadByte();
								break;
							case RuntimeValueType.IndirectStackIndexed:
								operand.Register = (Register)reader.ReadByte();
								operand.OffsetRegister = (Register)reader.ReadByte();
								break;
							case RuntimeValueType.InstrIndex:
								operand.InstrIndex = reader.ReadInt32();
								break;
							case RuntimeValueType.IntegerLiteral:
								operand.IntegerLiteral = reader.ReadInt32();
								break;
							case RuntimeValueType.LongLiteral:
								operand.LongLiteral = reader.ReadInt64();
								break;
							case RuntimeValueType.Register:
								operand.Register = (Register)reader.ReadByte();
								break;
							case RuntimeValueType.ShortLiteral:
								operand.ShortLiteral = reader.ReadInt16();
								break;
							case RuntimeValueType.SymbolIndex:
                                operand.SymbolIndex = reader.ReadInt32();
								operand.Symbol = (Symbol)_symbolList[operand.SymbolIndex];
								if (operand.Symbol is StringSymbol) 
								{
									operand.StringLiteral = operand.Symbol.Identifier;
									operand.ValueType = RuntimeValueType.StringLiteral;
								}
								break;
						}
					}
				}
			}
			else
				throw new Exception("Unable to load script byte code, header is invalid.");
		}

		/// <summary>
		///		Creates a new thread and attaches it to this process.
		/// </summary>
		/// <param name="entryPoint">Entry point where the thread should start.</param>
		/// <returns>New instance of ScriptThread.</returns>
		public ScriptThread CreateThread(string functionIdent)
		{
			ScriptThread thread = new ScriptThread(this);
			thread.InvokeFunction(functionIdent);
			return thread;
		}

		/// <summary>
		///		Removes a given thread from this process.
		/// </summary>
		/// <param name="thread">Thread to remove.</param>
		public void RemoveThread(ScriptThread thread)
		{
			_threads.Remove(thread);
		}

		/// <summary>
		///		Attachs a given thread to this process.
		/// </summary>
		/// <param name="thread">Thread to attach to this process.</param>
		public void AttachThread(ScriptThread thread)
		{
			_threads.Add(thread);
		}

		/// <summary>
		///		Allocates a chunk of data on the heap.
		/// </summary>
		/// <param name="size">Size of data to allocate.</param>
		/// <param name="type">Data type the allocated memory should store.</param>
		/// <param name="value">Data type this heap is stored as (differs from the type paramter 'type' as it keeps track of arrays and references to)</param>
		/// <returns>Starting index of data on the heap.</returns>
		public int AllocateHeap(int size, RuntimeValueType type, DataTypeValue value)
		{
			// Check if there is any space on the stack. 
			// Ignore the constant and global memory index as they are persistant.
			int allocatableIndex = 0, allocatableSize = 0;

            // First element is reserved to capture null-to-zero-cast-to-memory-index (XD) errors.
			for (int i = 1; i < _memoryHeap.Length; i++)
			{
				if (_memoryHeap[i] != null)
				{
					allocatableSize = 0;
					allocatableIndex = i + 1;
                    if (_memoryHeap[i].ValueType == RuntimeValueType.MemoryBoundry)
                    {
                        i += _memoryHeap[i].IntegerLiteral;
                        allocatableIndex = i + 1;
                        continue;
                    }
                    i++;
                    continue;
				}
				else
					allocatableSize++;

				if (allocatableSize >= (size + 1))
					break;
			}
            if (allocatableSize < (size + 1))// || (allocatableIndex + allocatableSize) >= _memoryHeap.Length)
            {
                CollectGarbage(); // Desperatly try and free some space :S.
                if (allocatableSize + _lastCollectionMemoryCount < (size + 1))
                {
                    // If all else fails, resize. Its better than crashing.
                    Array.Resize(ref _memoryHeap, _memoryHeap.Length * 2 < _memoryHeap.Length + size ? _memoryHeap.Length + size : _memoryHeap.Length * 2); // Double the size.
                    DebugLogger.WriteLog("Memory heap of script '"+_url.ToString()+"' was resized due to overflow.");
                    return AllocateHeap(size, type, value);
                }
                else
                    return AllocateHeap(size, type, value);
            } 

			// Actually allocate the data.
			for (int i = allocatableIndex; i <= (allocatableIndex + allocatableSize); i++)
				_memoryHeap[i] = new RuntimeValue(type);

			// Set the first data block as a memory boundry so the GC can look after it.
			_memoryHeap[allocatableIndex].ValueType = RuntimeValueType.MemoryBoundry;
			_memoryHeap[allocatableIndex].IntegerLiteral = size;
            _memoryHeap[allocatableIndex].ReferenceCount = 0;
			_memoryHeap[allocatableIndex].DataType = value; 

			return allocatableIndex + 1;
		}

		/// <summary>
		///		Deallocates a chuck of data that has been 
		///		previously allocated on the heap.
		/// </summary>
		/// <param name="start">Starting index of data on the heap.</param>
		/// <param name="size">Size of data to deallocate.</param>
		public void DeallocateHeap(int start, int size)
		{
            for (int i = start - 1; i <= start + size; i++)
				_memoryHeap[i] = null;
		}
		public void DeallocateHeap(int start)
		{
			DeallocateHeap(start, _memoryHeap[start - 1].IntegerLiteral);
		}

		/// <summary>
		///		Allocates an Object index in the Object heap and returns its index.
		/// </summary>
		/// <param name="Object">Object to allocate index for.</param>
		/// <returns>Allocated index.</returns>
		public int AllocateObjectIndex(RuntimeObject objectToIndex)
		{
            // First element is reserved to capture null-to-zero-cast-to-object (XD) errors.
            for (int i = 1; i < _objectHeap.Length; i++)
                if (_objectHeap[i] == null || _objectHeap[i] == objectToIndex)
                {
                    _objectHeap[i] = objectToIndex;
                    return i;
                }
                else if (objectToIndex is NativeObject && _objectHeap[i] is NativeObject && ((NativeObject)objectToIndex).Object == ((NativeObject)_objectHeap[i]).Object)
                    return i;

            // See if we can clean up a bit :S.
            CollectGarbage(); // Desperatly try and free some space :S.
            if (_lastCollectionObjectCount == 0)
            {
                Array.Resize(ref _objectHeap, _objectHeap.Length * 2); // Double the size.
                return AllocateObjectIndex(objectToIndex);
            }
            else
                return AllocateObjectIndex(objectToIndex);
        }

		/// <summary>
		///		Deallocates an Object index.
		/// </summary>
		/// <param name="index">Index to deallocate.</param>
		public void DeallocateObjectIndex(int index)
		{
            _objectHeap[index].Deallocate();
			_objectHeap[index] = null;
		}

        /// <summary>
        ///     Deallocates all instances of a given native object.
        /// </summary>
        /// <param name="obj">Objec to deallocate.</param>
        public void DeallocateNativeObject(object obj)
        {
            // First element is reserved to capture null-to-zero-cast-to-object (XD) errors.
            for (int i = 1; i < _objectHeap.Length; i++)
            {
                if ((_objectHeap[i] as NativeObject) != null && ((NativeObject)_objectHeap[i]).Object == obj)
                {
                    _objectHeap[i].Deallocate();
                    _objectHeap[i] = null;                   
                }
            }
        }

		/// <summary>
		///		Preforms a garbage collection cycle on the heap. 
		/// </summary>
		public void CollectGarbage()
		{
            // -------------------------------------------------------------------------------------
			// Deallocate object references.
            // -------------------------------------------------------------------------------------
            int collectedObjects = 0;

            // First element is reserved to capture null-to-zero-cast-to-object (XD) errors.
            for (int i = 1; i < _objectHeap.Length; i++)
            {
                if (_objectHeap[i] == null) continue;

                if (_objectHeap[i].ReferenceCount <= 0 && _objectHeap[i].Collectable == true)
                {
                    collectedObjects++;
                    _objectHeap[i].Deallocate();
                    _objectHeap[i] = null;
                }
            }

            // -------------------------------------------------------------------------------------
            // Deallocate memory references.
            // -------------------------------------------------------------------------------------
            int collectedMemSize = 0;

            // First element is reserved to capture null-to-zero-cast-to-memory-index (XD) errors.
            for (int i = 1; i < _memoryHeap.Length; i++)
            {
                if (_memoryHeap[i] == null) continue;

                if (_memoryHeap[i].ValueType == RuntimeValueType.MemoryBoundry && _memoryHeap[i].ReferenceCount <= 0)
                {
                    //collectedMemSize++;
                    //DeallocateHeap(i + 1); // FIX
                }
            }

            _lastCollectionObjectCount = collectedObjects;
            _lastCollectionMemoryCount = collectedMemSize;
		}

		/// <summary>
		///		Changes the current state the script is in.
		/// </summary>
		/// <param name="state">Identifier of state to switch to.</param>
		public void ChangeState(string state)
		{
			foreach(Symbol symbol in _globalScope.Symbols)
				if (symbol.Identifier.ToLower() == state.ToLower() && symbol.Type == SymbolType.State)
				{
					ChangeState(symbol as StateSymbol);
					return;
				}
            throw new Exception("Unable to change to state \"" + state + "\", state does not exist.");
		}
		public void ChangeState(StateSymbol state)
		{
            // Invoke the state finish function.
			if (_currentState != null) InvokeFunction("OnStateFinish", true, false);
			_currentState = state;
			
            // Change state for each thread.
            foreach (ScriptThread thread in _threads)
				thread.ChangeState(state);

            // Call the state change event.
            if (OnStateChange != null)
                OnStateChange(this, state);

            // Invoke the state begin function.
			if (_currentState != null) 
				InvokeFunction("OnStateBegin", true, false);
		}

		/// <summary>
		///		Invokes a given function in all threads attached to this process.
		/// </summary>
		/// <param name="identifier">Identifier of function to invoke.</param>
		/// <param name="ignoreThread">If set all thread spawning flags associated with this function will be ignored.</param>
		/// <param name="waitForReturn">If set to true this function will not return until the function has executed fully.</param>
		public void InvokeFunction(string identifier, bool waitForReturn, bool ignoreThread)
		{
			foreach (ScriptThread threads in _threads)
				threads.InvokeFunction(identifier, waitForReturn, ignoreThread);
		}

		/// <summary>
		///		Initializes a new instance of this class and loads its script
		///		from a given file url.
		/// </summary>
		/// <param name="vm">Virtual machine this oricess us associated with.</param>
		/// <param name="url">Url of file to load this script process from.</param>
		public ScriptProcess(VirtualMachine vm, object url)
		{
			// Store virtual machine for layer use.
			_virtualMachine = vm;

			// Load the given script into this process.
			LoadByteCode(url);

			// Create a default thread that should run this process.
			ScriptThread newThread = new ScriptThread(this);

            // Attach console commands / exported command to this thread.
            // TODO: What if they already exist?
            foreach (Symbol symbol in _symbolList)
                if (symbol != null && symbol.Type == SymbolType.Function)
                {
                    if (((FunctionSymbol)symbol).IsConsole == true)
                        new ScriptConsoleCommand((FunctionSymbol)symbol, newThread);
                    else if (((FunctionSymbol)symbol).IsExport == true)
                        new ScriptExportFunction((FunctionSymbol)symbol, newThread);
                    else if (((FunctionSymbol)symbol).IsImport == true)
                    {
                        // Find the functions parameter types.
                        DataTypeValue[] parameterTypes = new DataTypeValue[((FunctionSymbol)symbol).ParameterCount];
                        for (int i = 0; i < ((FunctionSymbol)symbol).ParameterCount; i++)
                            parameterTypes[(((FunctionSymbol)symbol).ParameterCount - 1) - i] = ((VariableSymbol)symbol.Symbols[i]).DataType;

                        ((FunctionSymbol)symbol).NativeFunction = _virtualMachine.FindNativeFunction(symbol.Identifier, parameterTypes);
                    }
                }
		}

        /// <summary>
        ///     Copys the given processes data to this process.
        /// </summary>
        /// <param name="vm">Virtual machine this oricess us associated with.</param>
        /// <param name="process">Process to recieve data from.</param>
        public ScriptProcess(VirtualMachine vm, ScriptProcess process)
        {
            HighPreformanceTimer timer = new HighPreformanceTimer();

            // Store virtual machine for layer use.
            _virtualMachine = vm;

            // Load the given script into this process.
            process.CopyTo(this);

            // Create a default thread that should run this process.
            ScriptThread newThread = new ScriptThread(this);

            // Attach console commands / exported command to this thread.
            // TODO: What if they already exist?
            foreach (Symbol symbol in _symbolList)
                if (symbol != null && symbol.Type == SymbolType.Function)
                {
                    if (((FunctionSymbol)symbol).IsConsole == true)
                        new ScriptConsoleCommand((FunctionSymbol)symbol, newThread);
                    else if (((FunctionSymbol)symbol).IsExport == true)
                        new ScriptExportFunction((FunctionSymbol)symbol, newThread);
                    else if (((FunctionSymbol)symbol).IsImport == true)
                    {
                        // Find the functions parameter types.
                        DataTypeValue[] parameterTypes = new DataTypeValue[((FunctionSymbol)symbol).ParameterCount];
                        for (int i = 0; i < ((FunctionSymbol)symbol).ParameterCount; i++)
                            parameterTypes[(((FunctionSymbol)symbol).ParameterCount - 1) - i] = ((VariableSymbol)symbol.Symbols[i]).DataType;

                        ((FunctionSymbol)symbol).NativeFunction = _virtualMachine.FindNativeFunction(symbol.Identifier, parameterTypes);
                    }
                }
        }

		#endregion
	}

	/// <summary>
	///		Encapsulates all details about a single script thread attached
	///		to a script process.
	/// </summary>
	public sealed class ScriptThread
	{
		#region Members
		#region Variables

        private static int _instructionsExecuted = 0;

		private ScriptProcess _process;

		private Priority _priority = Priority.Normal;

		private int _instructionPointer;
		private RuntimeValue[] _registers = new RuntimeValue[10]
			{
				new RuntimeValue(RuntimeValueType.Invalid), new RuntimeValue(RuntimeValueType.Invalid),
				new RuntimeValue(RuntimeValueType.Invalid), new RuntimeValue(RuntimeValueType.Invalid),
				new RuntimeValue(RuntimeValueType.Invalid), new RuntimeValue(RuntimeValueType.Invalid),
				new RuntimeValue(RuntimeValueType.Invalid), new RuntimeValue(RuntimeValueType.Invalid),
                new RuntimeValue(RuntimeValueType.Invalid), new RuntimeValue(RuntimeValueType.Invalid)
			};
		private RuntimeStack _runtimeStack = new RuntimeStack(512); // When the hell are you going to use more than 512?

		private bool _isRunning   = false;
		private bool _isPaused    = false;
        private bool _isWaiting    = false;
		private int  _pauseLength = 0;
		private HighPreformanceTimer _pauseTimer = new HighPreformanceTimer();

		private bool _inAtom = false;
		private bool _previousInAtom = false;
		private RuntimeInstruction _lockInstruction = null;

		private FunctionSymbol _callingFunction = null;

		private int _passedParameterCount = 0;
		private ArrayList _passedParameterMask = new ArrayList();

		private Stack _callStack = new Stack();

        private Debugger _debugger = null;

		#endregion
		#region Properties

        /// <summary>
        ///     Gets or sets the amount of instructions executed since this application began.
        /// </summary>
        public static int InstructionsExecuted
        {
            get { return _instructionsExecuted; }
            set { _instructionsExecuted = value; }
        }

        /// <summary>
        ///     Gets or sets the current callstack.
        /// </summary>
        public Stack CallStack
        {
            get { return _callStack; }
            set { _callStack = value; }
        }

		/// <summary>
		///		Gets or sets the process this thread is associated with.
		/// </summary>
		public ScriptProcess Process
		{
			get { return _process;  }
			set { _process = value; }
		}

		/// <summary>
		///		Gets or sets if this process is running.
		/// </summary>
		public bool IsRunning
		{
			get { return _isRunning; }
			set { _isRunning = value; }
		}

		/// <summary>
		///		Gets or sets if this process is paused.
		/// </summary>
		public bool IsPaused
		{
			get { return _isPaused; }
			set { _isPaused = value; }
		}

        /// <summary>
        ///		Gets or sets if this process is waiting for something else in the engine to happen.
        /// </summary>
        public bool IsWaiting
        {
            get { return _isWaiting; }
            set { _isWaiting = value; }
        }

		/// <summary>
		///		Gets or sets the priority this thread should be given when
		///		running multiple threads.
		/// </summary>
		public Priority Priority
		{
			get { return _priority; }
			set { _priority = value; }
		}

		/// <summary>
		///		Gets or sets the position of the instruction pointer in this
		///		thread.
		/// </summary>
		public int InstructionPointer
		{
			get { return _instructionPointer;  }
			set { _instructionPointer = value; }
		}

		/// <summary>
		///		Retrieves an array containing all the registers
		///		used by this thread.
		/// </summary>
		public RuntimeValue[] Registers
		{
			get { return _registers; }
		}

		/// <summary>
		///		Retrieves the runtime stack used by this thread to store local 
		///		data and to keep track of function frames.
		/// </summary>
		public RuntimeStack Stack
		{
			get { return _runtimeStack; }
		}

        /// <summary>
        ///     Gets or sets the debugger attached to this thread.
        /// </summary>
        public Debugger Debugger
        {
            get { return _debugger; }
            set { _debugger = value; }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Gets an operand at a given index from a given instruction.
		/// </summary>
		/// <param name="instruction">Instruction to get operand from.</param>
		/// <param name="index">Index of operand to get.</param>
		/// <returns>Operand at index of instruction's operand list.</returns>
		private RuntimeValue GetOperand(RuntimeInstruction instruction, int index)
		{
			return instruction.Operands[index];
		}

		/// <summary>
		///		Resolves an operands value, if its indirectly points to another value.
		/// </summary>
		/// <param name="instruction">Instruction to get operand from.</param>
		/// <param name="index">Index of operand to get.</param>
		/// <returns>Resolved operand.</returns>
		private RuntimeValue ResolveOperand(RuntimeInstruction instruction, int index)
		{
			return ResolveValue(GetOperand(instruction, index));
		}

		/// <summary>
		///		If the given value directly or indirectly points to another
		///		value it will be resolved and returned.
		/// </summary>
		/// <param name="value">Value to resolve.</param>
		/// <returns>Resolved value.</returns>
		private RuntimeValue ResolveValue(RuntimeValue value)
		{
			switch (value.ValueType)
			{
				case RuntimeValueType.Register:
					return _registers[(int)value.Register];

				case RuntimeValueType.DirectStack:
					return _runtimeStack[value.StackIndex];
				case RuntimeValueType.DirectStackIndexed:
					return _runtimeStack[value.StackIndex + _registers[(int)value.OffsetRegister].IntegerLiteral];
				case RuntimeValueType.IndirectStack:
					return _runtimeStack[_registers[(int)value.Register].StackIndex];
				case RuntimeValueType.IndirectStackIndexed:
					return _runtimeStack[_registers[(int)value.Register].StackIndex + _registers[(int)value.OffsetRegister].IntegerLiteral];

				case RuntimeValueType.DirectMemory:
					return _process.MemoryHeap[value.MemoryIndex];
				case RuntimeValueType.DirectMemoryIndexed:
					return _process.MemoryHeap[value.MemoryIndex + _registers[(int)value.OffsetRegister].IntegerLiteral];
				case RuntimeValueType.IndirectMemory:
					return _process.MemoryHeap[_registers[(int)value.Register].MemoryIndex];
				case RuntimeValueType.IndirectMemoryIndexed:
					return _process.MemoryHeap[_registers[(int)value.Register].MemoryIndex + _registers[(int)value.OffsetRegister].IntegerLiteral];

				default:
					return value;
			}
		}

        /// <summary>
        ///     Sets the object value of a runtime value. This is used to keep reference counts up to date.
        /// </summary>
        /// <param name="value">Runtime value to set.</param>
        /// <param name="objectIndex">New object index.</param>
        private void SetObjectValue(RuntimeValue value, int objectIndex)
        {
            for (int i = 0; i < _registers.Length; i++)
                if (value == _registers[i])
                {
                    value.ObjectIndex = objectIndex;
                    return;     // Don't bother about registers, they will loose there value soon.
                }

            if (value.ObjectIndex != -1 && _process.ObjectHeap[value.ObjectIndex] != null)
            {
                _process.ObjectHeap[value.ObjectIndex].ReferenceCount--;
                //System.Console.WriteLine("Reduced reference of (" + value.ObjectIndex + ", " + _process.ObjectHeap[value.ObjectIndex].ToString() + ") to " + _process.ObjectHeap[value.ObjectIndex].ReferenceCount);
            }

            value.ObjectIndex = objectIndex;

            if (value.ObjectIndex != -1 && _process.ObjectHeap[value.ObjectIndex] != null)
            {
                _process.ObjectHeap[value.ObjectIndex].ReferenceCount++;
                //System.Console.WriteLine("Increased reference of (" + value.ObjectIndex + ", " + _process.ObjectHeap[value.ObjectIndex].ToString() + ") to " + _process.ObjectHeap[value.ObjectIndex].ReferenceCount);
            }
        }

        /// <summary>
        ///     Sets the memory inde value of a runtime value. This is used to keep reference counts up to date.
        /// </summary>
        /// <param name="value">Runtime value to set.</param>
        /// <param name="memoryIndex">New memory index.</param>
        private void SetMemoryIndexValue(RuntimeValue value, int memoryIndex)
        {
            for (int i = 0; i < _registers.Length; i++)
                if (value == _registers[i])
                {
                    value.MemoryIndex = memoryIndex;
                    return;     // Don't bother about registers, they will loose there value soon.
                }

            if (value.MemoryIndex != -1 && _process.MemoryHeap[value.MemoryIndex - 1] != null)
                _process.MemoryHeap[value.MemoryIndex - 1].ReferenceCount--;

            value.MemoryIndex = memoryIndex;

            if (value.MemoryIndex != -1 && _process.MemoryHeap[value.MemoryIndex - 1] != null)
                _process.MemoryHeap[value.MemoryIndex - 1].ReferenceCount++;
        }

		/// <summary>
		///		Runs this threads byte code.
		/// </summary>
		/// <param name="timeslice">Amount of time this thread is allowed to run for.</param>
		/// <returns>1 if the script has been forced to stop by a latency instruction, 2 if it has been forced to stop by a stack base marker or 3 if by an exit instruction.</returns>
		public int Run(int timeslice)
		{
			// Check that this thread is actually running.
			if (_isRunning == false) return 5;

			// Check the pause timer of this current thread.
			if (_isPaused == true)
			{
				if (_pauseTimer.DurationMillisecond <= _pauseLength) return 0;
				_isPaused = false;
			}

			// Check we are not at the end of this script, if we are then 
			if (_instructionPointer >= _process.Instructions.Length) return 0;

            // If the debugger is running then we can't continue :S.
            if (_debugger != null && _debugger.RunScript == false)
                return 6;

			// Keep running until our timeslice runs out.
            int ticks = Environment.TickCount; // Don't use a high speed timer - To slow.
            //int secondsTimer = Environment.TickCount;
            //int instrCount = 0;
            //HighPreformanceTimer instructionTimer = new HighPreformanceTimer();
            //float[] opCodeTimes = new float[Enum.GetNames(typeof(OpCode)).Length];
            while (true)
            {
#if !DEBUG
                try
                {
#endif

                    //instructionTimer.Restart();
                    // Checks if this script is still in its timeslice.
                    if (timeslice != -1 && (Environment.TickCount - ticks) > timeslice &&
                        _priority != Priority.RealTime && _inAtom == false)
                        return 0;

                    // General exit stuff.
                    else if (_callStack.Count == 0 || _instructionPointer >= _process.Instructions.Length || _isPaused == true || _isRunning == false || _isWaiting == true)
                        return 5;

                    // Debugger
                    else if (_debugger != null && _debugger.RunScript == false)
                        return 6;

                    // Retrieve's and execute's the next instruction in the list.
                    int originalPointer = _instructionPointer;

                    // Execute instruction and return result if neccessary.
                    RuntimeValue op1, op2, stack1;
                    RuntimeInstruction instruction = _process.Instructions[_instructionPointer];

                    //if (Environment.TickCount - secondsTimer > 10)
                    //{
                    //    secondsTimer = Environment.TickCount;
                    //if (_process.Url == "media\\scripts\\LTTP 2.0.fs")
                    //    System.Console.WriteLine(_process.Url+":"+(instrCount * 100) + " instrs/ps");
                    //    instrCount = 0;
                    //}
                    //else
                    //    instrCount++;

                    // This was originally a seperate function but to speed it up a bit it has been inlined.
                    #region Instruction Execution
                    switch (instruction.OpCode)
                    {
                        #region POP
                        case OpCode.POP_OBJECT:
                            op1 = ResolveOperand(instruction, 0);
                            op1.ValueType = RuntimeValueType.Object;
                            stack1 = _runtimeStack.Pop();
                            SetObjectValue(op1, stack1.ObjectIndex);
                            if (stack1.ObjectIndex != -1) SetObjectValue(stack1, -1);
                            break;
                        case OpCode.POP_BOOL:
                            op1 = ResolveOperand(instruction, 0);
                            op1.ValueType = RuntimeValueType.BooleanLiteral;
                            op1.BooleanLiteral = _runtimeStack.Pop().BooleanLiteral;
                            break;
                        case OpCode.POP_BYTE:
                            op1 = ResolveOperand(instruction, 0);
                            op1.ValueType = RuntimeValueType.ByteLiteral;
                            op1.ByteLiteral = _runtimeStack.Pop().ByteLiteral;
                            break;
                        case OpCode.POP_INT:
                            op1 = ResolveOperand(instruction, 0);
                            op1.ValueType = RuntimeValueType.IntegerLiteral;
                            op1.IntegerLiteral = _runtimeStack.Pop().IntegerLiteral;
                            break;
                        case OpCode.POP_SHORT:
                            op1 = ResolveOperand(instruction, 0);
                            op1.ValueType = RuntimeValueType.ShortLiteral;
                            op1.ShortLiteral = _runtimeStack.Pop().ShortLiteral;
                            break;
                        case OpCode.POP_FLOAT:
                            op1 = ResolveOperand(instruction, 0);
                            op1.ValueType = RuntimeValueType.FloatLiteral;
                            op1.FloatLiteral = _runtimeStack.Pop().FloatLiteral;
                            break;
                        case OpCode.POP_STRING:
                            op1 = ResolveOperand(instruction, 0);
                            op1.ValueType = RuntimeValueType.StringLiteral;
                            op1.StringLiteral = _runtimeStack.Pop().StringLiteral;
                            break;
                        case OpCode.POP_LONG:
                            op1 = ResolveOperand(instruction, 0);
                            op1.ValueType = RuntimeValueType.LongLiteral;
                            op1.LongLiteral = _runtimeStack.Pop().LongLiteral;
                            break;
                        case OpCode.POP_DOUBLE:
                            op1 = ResolveOperand(instruction, 0);
                            op1.ValueType = RuntimeValueType.DoubleLiteral;
                            op1.DoubleLiteral = _runtimeStack.Pop().DoubleLiteral;
                            break;
                        case OpCode.POP_MEMORY_INDEX:
                            op1 = ResolveOperand(instruction, 0);
                            op2 = _runtimeStack.Pop();
                            op1.ValueType = RuntimeValueType.Invalid;
                            // op1.MemoryIndex = op2.MemoryIndex;
                            op1.StackIndex = op2.StackIndex;

                            SetMemoryIndexValue(op1, op2.MemoryIndex);
                            if (op2.MemoryIndex != -1) SetMemoryIndexValue(op2, -1);
                            break;
                        case OpCode.POP_NULL:
                            _runtimeStack.Pop(); // Pop value of stack, we are not actually
                            // going to be using it so ignore it.
                            if (ResolveOperand(instruction, 0).ObjectIndex != -1) SetObjectValue(ResolveOperand(instruction, 0), -1);
                            else if (ResolveOperand(instruction, 0).MemoryIndex != -1) SetMemoryIndexValue(ResolveOperand(instruction, 0), -1);
                            ResolveOperand(instruction, 0).Clear(); // Nullify value.
                            break;
                        case OpCode.POP_DESTROY:
                            stack1 = _runtimeStack.Pop();
                            if (stack1.ObjectIndex != -1) SetObjectValue(stack1, -1);
                            else if (stack1.MemoryIndex != -1) SetMemoryIndexValue(stack1, -1);
                            _runtimeStack.Pop();
                            break;
                        #endregion
                        #region PUSH
                        case OpCode.PUSH_OBJECT:
                            SetObjectValue(_runtimeStack.PushEmpty(RuntimeValueType.Object), ResolveOperand(instruction, 0).ObjectIndex);
                            break;
                        case OpCode.PUSH_BOOL:
                            _runtimeStack.PushEmpty(RuntimeValueType.BooleanLiteral).BooleanLiteral = ResolveOperand(instruction, 0).BooleanLiteral;
                            break;
                        case OpCode.PUSH_BYTE:
                            _runtimeStack.PushEmpty(RuntimeValueType.ByteLiteral).ByteLiteral = ResolveOperand(instruction, 0).ByteLiteral;
                            break;
                        case OpCode.PUSH_INT:
                            _runtimeStack.PushEmpty(RuntimeValueType.IntegerLiteral).IntegerLiteral = ResolveOperand(instruction, 0).IntegerLiteral;
                            break;
                        case OpCode.PUSH_SHORT:
                            _runtimeStack.PushEmpty(RuntimeValueType.ShortLiteral).ShortLiteral = ResolveOperand(instruction, 0).ShortLiteral;
                            break;
                        case OpCode.PUSH_FLOAT:
                            _runtimeStack.PushEmpty(RuntimeValueType.FloatLiteral).FloatLiteral = ResolveOperand(instruction, 0).FloatLiteral;
                            break;
                        case OpCode.PUSH_STRING:
                            _runtimeStack.PushEmpty(RuntimeValueType.StringLiteral).StringLiteral = ResolveOperand(instruction, 0).StringLiteral;
                            break;
                        case OpCode.PUSH_LONG:
                            _runtimeStack.PushEmpty(RuntimeValueType.LongLiteral).LongLiteral = ResolveOperand(instruction, 0).LongLiteral;
                            break;
                        case OpCode.PUSH_DOUBLE:
                            _runtimeStack.PushEmpty(RuntimeValueType.DoubleLiteral).DoubleLiteral = ResolveOperand(instruction, 0).DoubleLiteral;
                            break;
                        case OpCode.PUSH_MEMORY_INDEX:
                            op1 = ResolveOperand(instruction, 0);
                            op2 = _runtimeStack.PushEmpty(RuntimeValueType.Invalid);
                            op2.MemoryIndex = op1.MemoryIndex;
                            op2.StackIndex = op1.StackIndex;
                            SetMemoryIndexValue(op2, op1.MemoryIndex);
                            break;
                        case OpCode.PUSH_NULL:
                            _runtimeStack.PushEmpty(RuntimeValueType.Invalid).Clear();
                            break;
                        #endregion
                        #region CAST
                        // Value casting op codes.
                        case OpCode.CAST_NULL:
                            if (ResolveOperand(instruction, 0).ObjectIndex != -1) SetObjectValue(ResolveOperand(instruction, 0), -1);
                            else if (ResolveOperand(instruction, 0).MemoryIndex != -1) SetMemoryIndexValue(ResolveOperand(instruction, 0), -1);
                            ResolveOperand(instruction, 0).Clear();
                            break;
                        case OpCode.CAST_BOOL:
                            op1 = ResolveOperand(instruction, 0);
                            switch (op1.ValueType)
                            {
                                case RuntimeValueType.Object: op1.BooleanLiteral = op1.ObjectIndex == 0 ? false : true; SetObjectValue(op1, -1); break;
                                case RuntimeValueType.ByteLiteral: op1.BooleanLiteral = op1.ByteLiteral == 0 ? false : true; break;
                                case RuntimeValueType.DoubleLiteral: op1.BooleanLiteral = op1.DoubleLiteral == 0 ? false : true; break;
                                case RuntimeValueType.FloatLiteral: op1.BooleanLiteral = op1.FloatLiteral == 0 ? false : true; break;
                                case RuntimeValueType.IntegerLiteral: op1.BooleanLiteral = op1.IntegerLiteral == 0 ? false : true; break;
                                case RuntimeValueType.LongLiteral: op1.BooleanLiteral = op1.LongLiteral == 0 ? false : true; break;
                                case RuntimeValueType.ShortLiteral: op1.BooleanLiteral = op1.ShortLiteral == 0 ? false : true; break;
                                case RuntimeValueType.StringLiteral: op1.BooleanLiteral = op1.StringLiteral == "0" ? false : true; break;
                            }
                            op1.ValueType = RuntimeValueType.BooleanLiteral;
                            break;
                        case OpCode.CAST_BYTE:
                            op1 = ResolveOperand(instruction, 0);
                            switch (op1.ValueType)
                            {
                                case RuntimeValueType.Object: op1.ByteLiteral = (byte)op1.ObjectIndex; SetObjectValue(op1, -1); break;
                                case RuntimeValueType.BooleanLiteral: op1.ByteLiteral = byte.Parse(op1.BooleanLiteral.ToString()); break;
                                case RuntimeValueType.DoubleLiteral: op1.ByteLiteral = (byte)op1.DoubleLiteral; break;
                                case RuntimeValueType.FloatLiteral: op1.ByteLiteral = (byte)op1.FloatLiteral; break;
                                case RuntimeValueType.IntegerLiteral: op1.ByteLiteral = (byte)op1.IntegerLiteral; break;
                                case RuntimeValueType.LongLiteral: op1.ByteLiteral = (byte)op1.LongLiteral; break;
                                case RuntimeValueType.ShortLiteral: op1.ByteLiteral = (byte)op1.ShortLiteral; break;
                                case RuntimeValueType.StringLiteral: op1.ByteLiteral = op1.StringLiteral == "" ? (byte)0 : (op1.StringLiteral.IndexOf('.') >= 0 ? (byte)float.Parse(op1.StringLiteral) : byte.Parse(op1.StringLiteral)); break;
                            }
                            op1.ValueType = RuntimeValueType.ByteLiteral;
                            break;
                        case OpCode.CAST_INT:
                            op1 = ResolveOperand(instruction, 0);
                            switch (op1.ValueType)
                            {
                                case RuntimeValueType.Object: op1.IntegerLiteral = (int)op1.ObjectIndex; SetObjectValue(op1, -1); break;
                                case RuntimeValueType.BooleanLiteral: op1.IntegerLiteral = (op1.BooleanLiteral == true ? 1 : 0); break;
                                case RuntimeValueType.ByteLiteral: op1.IntegerLiteral = (int)op1.ByteLiteral; break;
                                case RuntimeValueType.DoubleLiteral: op1.IntegerLiteral = (int)op1.DoubleLiteral; break;
                                case RuntimeValueType.FloatLiteral: op1.IntegerLiteral = (int)op1.FloatLiteral; break;
                                case RuntimeValueType.LongLiteral: op1.IntegerLiteral = (int)op1.LongLiteral; break;
                                case RuntimeValueType.ShortLiteral: op1.IntegerLiteral = (int)op1.ShortLiteral; break;
                                case RuntimeValueType.StringLiteral: op1.IntegerLiteral = op1.StringLiteral == "" ? 0 : (op1.StringLiteral.IndexOf('.') >= 0 ? (int)float.Parse(op1.StringLiteral) : int.Parse(op1.StringLiteral)); break;
                            }
                            op1.ValueType = RuntimeValueType.IntegerLiteral;
                            break;
                        case OpCode.CAST_SHORT:
                            op1 = ResolveOperand(instruction, 0);
                            switch (op1.ValueType)
                            {
                                case RuntimeValueType.Object: op1.ShortLiteral = (short)op1.ObjectIndex; SetObjectValue(op1, -1); break;
                                case RuntimeValueType.BooleanLiteral: op1.ShortLiteral = (short)(op1.BooleanLiteral == true ? 1 : 0); break;
                                case RuntimeValueType.ByteLiteral: op1.ShortLiteral = (short)op1.ByteLiteral; break;
                                case RuntimeValueType.DoubleLiteral: op1.ShortLiteral = (short)op1.DoubleLiteral; break;
                                case RuntimeValueType.FloatLiteral: op1.ShortLiteral = (short)op1.FloatLiteral; break;
                                case RuntimeValueType.IntegerLiteral: op1.ShortLiteral = (short)op1.IntegerLiteral; break;
                                case RuntimeValueType.LongLiteral: op1.ShortLiteral = (short)op1.LongLiteral; break;
                                case RuntimeValueType.StringLiteral: op1.ShortLiteral = op1.StringLiteral == "" ? (short)0 : (op1.StringLiteral.IndexOf('.') >= 0 ? (short)float.Parse(op1.StringLiteral) : short.Parse(op1.StringLiteral)); break;
                            }
                            op1.ValueType = RuntimeValueType.ShortLiteral;
                            break;
                        case OpCode.CAST_FLOAT:
                            op1 = ResolveOperand(instruction, 0);
                            switch (op1.ValueType)
                            {
                                case RuntimeValueType.FloatLiteral: op1.FloatLiteral = (float)op1.ObjectIndex; SetObjectValue(op1, -1); break;
                                case RuntimeValueType.BooleanLiteral: op1.FloatLiteral = (op1.BooleanLiteral == true ? 1 : 0); break;
                                case RuntimeValueType.ByteLiteral: op1.FloatLiteral = (float)op1.ByteLiteral; break;
                                case RuntimeValueType.DoubleLiteral: op1.FloatLiteral = (float)op1.DoubleLiteral; break;
                                case RuntimeValueType.IntegerLiteral: op1.FloatLiteral = (float)op1.IntegerLiteral; break;
                                case RuntimeValueType.LongLiteral: op1.FloatLiteral = (float)op1.LongLiteral; break;
                                case RuntimeValueType.ShortLiteral: op1.FloatLiteral = (float)op1.ShortLiteral; break;
                                case RuntimeValueType.StringLiteral: op1.FloatLiteral = op1.StringLiteral == "" ? 0 : float.Parse(op1.StringLiteral); break;
                            }
                            op1.ValueType = RuntimeValueType.FloatLiteral;
                            break;
                        case OpCode.CAST_DOUBLE:
                            op1 = ResolveOperand(instruction, 0);
                            switch (op1.ValueType)
                            {
                                case RuntimeValueType.Object: op1.DoubleLiteral = (double)op1.ObjectIndex; SetObjectValue(op1, -1); break;
                                case RuntimeValueType.BooleanLiteral: op1.DoubleLiteral = (op1.BooleanLiteral == true ? 1 : 0); break;
                                case RuntimeValueType.ByteLiteral: op1.DoubleLiteral = (double)op1.ByteLiteral; break;
                                case RuntimeValueType.FloatLiteral: op1.DoubleLiteral = (double)op1.FloatLiteral; break;
                                case RuntimeValueType.IntegerLiteral: op1.DoubleLiteral = (double)op1.IntegerLiteral; break;
                                case RuntimeValueType.LongLiteral: op1.DoubleLiteral = (double)op1.LongLiteral; break;
                                case RuntimeValueType.ShortLiteral: op1.DoubleLiteral = (double)op1.ShortLiteral; break;
                                case RuntimeValueType.StringLiteral: op1.DoubleLiteral = op1.StringLiteral == "" ? 0 : double.Parse(op1.StringLiteral); break;
                            }
                            op1.ValueType = RuntimeValueType.DoubleLiteral;
                            break;
                        case OpCode.CAST_LONG:
                            op1 = ResolveOperand(instruction, 0);
                            switch (op1.ValueType)
                            {
                                case RuntimeValueType.Object: op1.LongLiteral = (long)op1.ObjectIndex; SetObjectValue(op1, -1); break;
                                case RuntimeValueType.BooleanLiteral: op1.LongLiteral = (op1.BooleanLiteral == true ? 1 : 0); break;
                                case RuntimeValueType.ByteLiteral: op1.LongLiteral = (long)op1.ByteLiteral; break;
                                case RuntimeValueType.DoubleLiteral: op1.LongLiteral = (long)op1.DoubleLiteral; break;
                                case RuntimeValueType.FloatLiteral: op1.LongLiteral = (long)op1.FloatLiteral; break;
                                case RuntimeValueType.IntegerLiteral: op1.LongLiteral = (long)op1.IntegerLiteral; break;
                                case RuntimeValueType.ShortLiteral: op1.LongLiteral = (long)op1.ShortLiteral; break;
                                case RuntimeValueType.StringLiteral: op1.LongLiteral = op1.StringLiteral == "" ? 0 : (op1.StringLiteral.IndexOf('.') >= 0 ? (long)float.Parse(op1.StringLiteral) : long.Parse(op1.StringLiteral)); break;
                            }
                            op1.ValueType = RuntimeValueType.LongLiteral;
                            break;
                        case OpCode.CAST_Object:
                            op1 = ResolveOperand(instruction, 0);
                            switch (op1.ValueType)
                            {
                                case RuntimeValueType.BooleanLiteral: SetObjectValue(op1, (op1.BooleanLiteral == true ? 1 : 0)); break;
                                case RuntimeValueType.ByteLiteral: SetObjectValue(op1, op1.ByteLiteral); break;
                                case RuntimeValueType.DoubleLiteral: SetObjectValue(op1, (int)op1.DoubleLiteral); break;
                                case RuntimeValueType.FloatLiteral: SetObjectValue(op1, (int)op1.FloatLiteral); break;
                                case RuntimeValueType.IntegerLiteral: SetObjectValue(op1, op1.IntegerLiteral); break;
                                case RuntimeValueType.LongLiteral: SetObjectValue(op1, (int)op1.LongLiteral); break;
                                case RuntimeValueType.ShortLiteral: SetObjectValue(op1, op1.ShortLiteral); break;
                                case RuntimeValueType.StringLiteral: SetObjectValue(op1, (op1.StringLiteral != "" ? (op1.StringLiteral.IndexOf('.') >= 0 ? (int)float.Parse(op1.StringLiteral) : int.Parse(op1.StringLiteral)) : 0)); break;
                            }
                            op1.ValueType = RuntimeValueType.Object;
                            break;
                        case OpCode.CAST_STRING:
                            op1 = ResolveOperand(instruction, 0);
                            switch (op1.ValueType)
                            {
                                case RuntimeValueType.Object: op1.StringLiteral = op1.ObjectIndex.ToString(); SetObjectValue(op1, -1); break;
                                case RuntimeValueType.BooleanLiteral: op1.StringLiteral = (op1.BooleanLiteral == true ? "1" : "0"); break;
                                case RuntimeValueType.ByteLiteral: op1.StringLiteral = op1.ByteLiteral.ToString(); break;
                                case RuntimeValueType.DoubleLiteral: op1.StringLiteral = op1.DoubleLiteral.ToString(); break;
                                case RuntimeValueType.FloatLiteral: op1.StringLiteral = op1.FloatLiteral.ToString(); break;
                                case RuntimeValueType.IntegerLiteral: op1.StringLiteral = op1.IntegerLiteral.ToString(); break;
                                case RuntimeValueType.LongLiteral: op1.StringLiteral = op1.LongLiteral.ToString(); break;
                                case RuntimeValueType.ShortLiteral: op1.StringLiteral = op1.ShortLiteral.ToString(); break;
                            }
                            op1.ValueType = RuntimeValueType.StringLiteral;
                            break;
                        #endregion
                        #region MOV
                        // Arithmetic op codes.
                        case OpCode.MOV_BOOL:
                            op1 = ResolveOperand(instruction, 0);
                            op1.ValueType = RuntimeValueType.BooleanLiteral;
                            op1.BooleanLiteral = ResolveOperand(instruction, 1).BooleanLiteral;
                            break;
                        case OpCode.MOV_BYTE:
                            op1 = ResolveOperand(instruction, 0);
                            op1.ValueType = RuntimeValueType.ByteLiteral;
                            op1.ByteLiteral = ResolveOperand(instruction, 1).ByteLiteral;
                            break;
                        case OpCode.MOV_INT:
                            op1 = ResolveOperand(instruction, 0);
                            op1.ValueType = RuntimeValueType.IntegerLiteral;
                            op1.IntegerLiteral = ResolveOperand(instruction, 1).IntegerLiteral;
                            break;
                        case OpCode.MOV_SHORT:
                            op1 = ResolveOperand(instruction, 0);
                            op1.ValueType = RuntimeValueType.ShortLiteral;
                            op1.ShortLiteral = ResolveOperand(instruction, 1).ShortLiteral;
                            break;
                        case OpCode.MOV_FLOAT:
                            op1 = ResolveOperand(instruction, 0);
                            op1.ValueType = RuntimeValueType.FloatLiteral;
                            op1.FloatLiteral = ResolveOperand(instruction, 1).FloatLiteral;
                            break;
                        case OpCode.MOV_DOUBLE:
                            op1 = ResolveOperand(instruction, 0);
                            op1.ValueType = RuntimeValueType.DoubleLiteral;
                            op1.DoubleLiteral = ResolveOperand(instruction, 1).DoubleLiteral;
                            break;
                        case OpCode.MOV_LONG:
                            op1 = ResolveOperand(instruction, 0);
                            op1.ValueType = RuntimeValueType.LongLiteral;
                            op1.LongLiteral = ResolveOperand(instruction, 1).LongLiteral;
                            break;
                        case OpCode.MOV_OBJECT:
                            op1 = ResolveOperand(instruction, 0);
                            op1.ValueType = RuntimeValueType.Object;
                            SetObjectValue(op1, ResolveOperand(instruction, 1).ObjectIndex);
                            break;
                        case OpCode.MOV_NULL:
                            if (ResolveOperand(instruction, 0).ObjectIndex != -1) SetObjectValue(ResolveOperand(instruction, 0), -1);
                            else if (ResolveOperand(instruction, 0).MemoryIndex != -1) SetMemoryIndexValue(ResolveOperand(instruction, 0), -1);
                            ResolveOperand(instruction, 0).Clear();
                            break;
                        case OpCode.MOV_STRING:
                            op1 = ResolveOperand(instruction, 0);
                            op1.ValueType = RuntimeValueType.StringLiteral;
                            op1.StringLiteral = ResolveOperand(instruction, 1).StringLiteral;
                            break;
                        case OpCode.MOV_MEMORY_INDEX:
                            op1 = ResolveOperand(instruction, 0);
                            op2 = ResolveOperand(instruction, 1);
                            //op1.MemoryIndex = op2.MemoryIndex;
                            op1.StackIndex = op2.StackIndex;
                            SetMemoryIndexValue(op1, op2.MemoryIndex);
                            break;
                        #endregion
                        #region MUL
                        case OpCode.MUL_BYTE:
                            ResolveOperand(instruction, 0).ByteLiteral *= ResolveOperand(instruction, 1).ByteLiteral;
                            break;
                        case OpCode.MUL_INT:
                            ResolveOperand(instruction, 0).IntegerLiteral *= ResolveOperand(instruction, 1).IntegerLiteral;
                            break;
                        case OpCode.MUL_SHORT:
                            ResolveOperand(instruction, 0).ShortLiteral *= ResolveOperand(instruction, 1).ShortLiteral;
                            break;
                        case OpCode.MUL_FLOAT:
                            ResolveOperand(instruction, 0).FloatLiteral *= ResolveOperand(instruction, 1).FloatLiteral;
                            break;
                        case OpCode.MUL_DOUBLE:
                            ResolveOperand(instruction, 0).DoubleLiteral *= ResolveOperand(instruction, 1).DoubleLiteral;
                            break;
                        case OpCode.MUL_LONG:
                            ResolveOperand(instruction, 0).LongLiteral *= ResolveOperand(instruction, 1).LongLiteral;
                            break;
                        #endregion
                        #region DIV
                        case OpCode.DIV_BYTE:
                            ResolveOperand(instruction, 0).ByteLiteral /= ResolveOperand(instruction, 1).ByteLiteral;
                            break;
                        case OpCode.DIV_INT:
                            ResolveOperand(instruction, 0).IntegerLiteral /= ResolveOperand(instruction, 1).IntegerLiteral;
                            break;
                        case OpCode.DIV_SHORT:
                            ResolveOperand(instruction, 0).ShortLiteral /= ResolveOperand(instruction, 1).ShortLiteral;
                            break;
                        case OpCode.DIV_FLOAT:
                            ResolveOperand(instruction, 0).FloatLiteral /= ResolveOperand(instruction, 1).FloatLiteral;
                            break;
                        case OpCode.DIV_DOUBLE:
                            ResolveOperand(instruction, 0).DoubleLiteral /= ResolveOperand(instruction, 1).DoubleLiteral;
                            break;
                        case OpCode.DIV_LONG:
                            ResolveOperand(instruction, 0).LongLiteral /= ResolveOperand(instruction, 1).LongLiteral;
                            break;
                        #endregion
                        #region ADD
                        case OpCode.ADD_STRING:
                            ResolveOperand(instruction, 0).StringLiteral += ResolveOperand(instruction, 1).StringLiteral;
                            break;
                        case OpCode.ADD_BYTE:
                            ResolveOperand(instruction, 0).ByteLiteral += ResolveOperand(instruction, 1).ByteLiteral;
                            break;
                        case OpCode.ADD_INT:
                            ResolveOperand(instruction, 0).IntegerLiteral += ResolveOperand(instruction, 1).IntegerLiteral;
                            break;
                        case OpCode.ADD_SHORT:
                            ResolveOperand(instruction, 0).ShortLiteral += ResolveOperand(instruction, 1).ShortLiteral;
                            break;
                        case OpCode.ADD_FLOAT:
                            ResolveOperand(instruction, 0).FloatLiteral += ResolveOperand(instruction, 1).FloatLiteral;
                            break;
                        case OpCode.ADD_DOUBLE:
                            ResolveOperand(instruction, 0).DoubleLiteral += ResolveOperand(instruction, 1).DoubleLiteral;
                            break;
                        case OpCode.ADD_LONG:
                            ResolveOperand(instruction, 0).LongLiteral += ResolveOperand(instruction, 1).LongLiteral;
                            break;
                        #endregion
                        #region SUB
                        case OpCode.SUB_BYTE:
                            ResolveOperand(instruction, 0).ByteLiteral -= ResolveOperand(instruction, 1).ByteLiteral;
                            break;
                        case OpCode.SUB_INT:
                            ResolveOperand(instruction, 0).IntegerLiteral -= ResolveOperand(instruction, 1).IntegerLiteral;
                            break;
                        case OpCode.SUB_SHORT:
                            ResolveOperand(instruction, 0).ShortLiteral -= ResolveOperand(instruction, 1).ShortLiteral;
                            break;
                        case OpCode.SUB_FLOAT:
                            ResolveOperand(instruction, 0).FloatLiteral -= ResolveOperand(instruction, 1).FloatLiteral;
                            break;
                        case OpCode.SUB_DOUBLE:
                            ResolveOperand(instruction, 0).DoubleLiteral -= ResolveOperand(instruction, 1).DoubleLiteral;
                            break;
                        case OpCode.SUB_LONG:
                            ResolveOperand(instruction, 0).LongLiteral -= ResolveOperand(instruction, 1).LongLiteral;
                            break;
                        #endregion
                        #region INC
                        case OpCode.INC_BYTE:
                            ResolveOperand(instruction, 0).ByteLiteral++;
                            break;
                        case OpCode.INC_INT:
                            ResolveOperand(instruction, 0).IntegerLiteral++;
                            break;
                        case OpCode.INC_SHORT:
                            ResolveOperand(instruction, 0).ShortLiteral++;
                            break;
                        case OpCode.INC_FLOAT:
                            ResolveOperand(instruction, 0).FloatLiteral++;
                            break;
                        case OpCode.INC_DOUBLE:
                            ResolveOperand(instruction, 0).DoubleLiteral++;
                            break;
                        case OpCode.INC_LONG:
                            ResolveOperand(instruction, 0).LongLiteral++;
                            break;
                        #endregion
                        #region DEC
                        case OpCode.DEC_BYTE:
                            ResolveOperand(instruction, 0).ByteLiteral--;
                            break;
                        case OpCode.DEC_INT:
                            ResolveOperand(instruction, 0).IntegerLiteral--;
                            break;
                        case OpCode.DEC_SHORT:
                            ResolveOperand(instruction, 0).ShortLiteral--;
                            break;
                        case OpCode.DEC_FLOAT:
                            ResolveOperand(instruction, 0).FloatLiteral--;
                            break;
                        case OpCode.DEC_DOUBLE:
                            ResolveOperand(instruction, 0).DoubleLiteral--;
                            break;
                        case OpCode.DEC_LONG:
                            ResolveOperand(instruction, 0).LongLiteral--;
                            break;
                        #endregion
                        #region NEG
                        case OpCode.NEG_BYTE:
                            ResolveOperand(instruction, 0).ByteLiteral = (byte)(-(int)ResolveOperand(instruction, 0).ByteLiteral);
                            break;
                        case OpCode.NEG_INT:
                            ResolveOperand(instruction, 0).IntegerLiteral = -ResolveOperand(instruction, 0).IntegerLiteral;
                            break;
                        case OpCode.NEG_SHORT:
                            ResolveOperand(instruction, 0).ShortLiteral = (short)(-(int)ResolveOperand(instruction, 0).ShortLiteral);
                            break;
                        case OpCode.NEG_FLOAT:
                            ResolveOperand(instruction, 0).FloatLiteral = -ResolveOperand(instruction, 0).FloatLiteral;
                            break;
                        case OpCode.NEG_DOUBLE:
                            ResolveOperand(instruction, 0).DoubleLiteral = -ResolveOperand(instruction, 0).DoubleLiteral;
                            break;
                        case OpCode.NEG_LONG:
                            ResolveOperand(instruction, 0).LongLiteral = -ResolveOperand(instruction, 0).LongLiteral;
                            break;
                        #endregion
                        #region ABS
                        case OpCode.ABS_BYTE:
                            ResolveOperand(instruction, 0).ByteLiteral = (byte)(+(int)ResolveOperand(instruction, 0).ByteLiteral);
                            break;
                        case OpCode.ABS_INT:
                            ResolveOperand(instruction, 0).IntegerLiteral = +ResolveOperand(instruction, 0).IntegerLiteral;
                            break;
                        case OpCode.ABS_SHORT:
                            ResolveOperand(instruction, 0).ShortLiteral = (short)(-(int)ResolveOperand(instruction, 0).ShortLiteral);
                            break;
                        case OpCode.ABS_FLOAT:
                            ResolveOperand(instruction, 0).FloatLiteral = +ResolveOperand(instruction, 0).FloatLiteral;
                            break;
                        case OpCode.ABS_DOUBLE:
                            ResolveOperand(instruction, 0).DoubleLiteral = +ResolveOperand(instruction, 0).DoubleLiteral;
                            break;
                        case OpCode.ABS_LONG:
                            ResolveOperand(instruction, 0).LongLiteral = +ResolveOperand(instruction, 0).LongLiteral;
                            break;
                        #endregion
                        #region Bitwise Operations
                        case OpCode.BIT_OR_INT:
                            ResolveOperand(instruction, 0).IntegerLiteral |= ResolveOperand(instruction, 0).IntegerLiteral;
                            break;
                        case OpCode.BIT_AND_INT:
                            ResolveOperand(instruction, 0).IntegerLiteral &= ResolveOperand(instruction, 0).IntegerLiteral;
                            break;
                        case OpCode.BIT_XOR_INT:
                            ResolveOperand(instruction, 0).IntegerLiteral ^= ResolveOperand(instruction, 0).IntegerLiteral;
                            break;
                        case OpCode.BIT_NOT_INT:
                            ResolveOperand(instruction, 0).IntegerLiteral = ~ResolveOperand(instruction, 0).IntegerLiteral;
                            break;
                        case OpCode.BIT_SHL_INT:
                            ResolveOperand(instruction, 0).IntegerLiteral <<= ResolveOperand(instruction, 0).IntegerLiteral;
                            break;
                        case OpCode.BIT_SHR_INT:
                            ResolveOperand(instruction, 0).IntegerLiteral >>= ResolveOperand(instruction, 0).IntegerLiteral;
                            break;
                        #endregion
                        #region Misc Operations
                        case OpCode.EXP_INT:
                            ResolveOperand(instruction, 0).IntegerLiteral = (int)Math.Pow(ResolveOperand(instruction, 0).IntegerLiteral, ResolveOperand(instruction, 1).IntegerLiteral);
                            break;
                        case OpCode.MOD_INT:
                            if (ResolveOperand(instruction, 0).IntegerLiteral != 0 && ResolveOperand(instruction, 1).IntegerLiteral != 0)
                                ResolveOperand(instruction, 0).IntegerLiteral %= ResolveOperand(instruction, 1).IntegerLiteral;
                            else
                                ResolveOperand(instruction, 0).IntegerLiteral = 0;
                            break;
                        #endregion
                        #region CMP
                        // Flow of control op codes.
                        case OpCode.CMP_NULL:
                            _registers[(int)Register.Compare].IntegerLiteral = ResolveOperand(instruction, 0).IsNull() == true ? 1 : 0;
                            break;
                        case OpCode.CMP_Object:
                            RuntimeObject obj1 = ResolveOperand(instruction, 0).ObjectIndex == -1 ? null : _process.ObjectHeap[ResolveOperand(instruction, 0).ObjectIndex];
                            RuntimeObject obj2 = ResolveOperand(instruction, 1).ObjectIndex == -1 ? null : _process.ObjectHeap[ResolveOperand(instruction, 1).ObjectIndex];
                            if (obj1 as NativeObject != null && obj2 as NativeObject != null)
                                _registers[(int)Register.Compare].IntegerLiteral = (((NativeObject)obj1).Object == ((NativeObject)obj2).Object) ? 0 : 1;
                            else
                                _registers[(int)Register.Compare].IntegerLiteral = Math.Sign(ResolveOperand(instruction, 0).ObjectIndex - ResolveOperand(instruction, 1).ObjectIndex);
                            break;
                        case OpCode.CMP_BOOL:
                            _registers[(int)Register.Compare].IntegerLiteral = Math.Sign((ResolveOperand(instruction, 0).BooleanLiteral == true ? 1 : 0) - (ResolveOperand(instruction, 1).BooleanLiteral == true ? 1 : 0));
                            break;
                        case OpCode.CMP_BYTE:
                            _registers[(int)Register.Compare].IntegerLiteral = Math.Sign(ResolveOperand(instruction, 0).ByteLiteral - ResolveOperand(instruction, 1).ByteLiteral);
                            break;
                        case OpCode.CMP_INT:
                            _registers[(int)Register.Compare].IntegerLiteral = Math.Sign(ResolveOperand(instruction, 0).IntegerLiteral - ResolveOperand(instruction, 1).IntegerLiteral);
                            break;
                        case OpCode.CMP_SHORT:
                            _registers[(int)Register.Compare].IntegerLiteral = Math.Sign(ResolveOperand(instruction, 0).ShortLiteral - ResolveOperand(instruction, 1).ShortLiteral);
                            break;
                        case OpCode.CMP_FLOAT:
                            _registers[(int)Register.Compare].IntegerLiteral = Math.Sign(ResolveOperand(instruction, 0).FloatLiteral - ResolveOperand(instruction, 1).FloatLiteral);
                            break;
                        case OpCode.CMP_STRING:
                            _registers[(int)Register.Compare].IntegerLiteral = Math.Sign(ResolveOperand(instruction, 0).StringLiteral.CompareTo(ResolveOperand(instruction, 1).StringLiteral));
                            break;
                        case OpCode.CMP_LONG:
                            _registers[(int)Register.Compare].IntegerLiteral = Math.Sign(ResolveOperand(instruction, 0).LongLiteral - ResolveOperand(instruction, 1).LongLiteral);
                            break;
                        case OpCode.CMP_DOUBLE:
                            _registers[(int)Register.Compare].IntegerLiteral = Math.Sign(ResolveOperand(instruction, 0).DoubleLiteral - ResolveOperand(instruction, 1).DoubleLiteral);
                            break;
                        case OpCode.CMP_MEMORY_INDEX:
                            _registers[(int)Register.Compare].IntegerLiteral = Math.Sign(ResolveOperand(instruction, 0).MemoryIndex - ResolveOperand(instruction, 1).MemoryIndex);
                            break;
                        #endregion
                        #region JMP

                        case OpCode.JMP_EQ:
                            if (_registers[(int)Register.Compare].IntegerLiteral == 0)
                                _instructionPointer = ResolveOperand(instruction, 0).InstrIndex;
                            break;
                        case OpCode.JMP_L:
                            if (_registers[(int)Register.Compare].IntegerLiteral == -1)
                                _instructionPointer = ResolveOperand(instruction, 0).InstrIndex;
                            break;
                        case OpCode.JMP_G:
                            if (_registers[(int)Register.Compare].IntegerLiteral == 1)
                                _instructionPointer = ResolveOperand(instruction, 0).InstrIndex;
                            break;
                        case OpCode.JMP_LE:
                            if (_registers[(int)Register.Compare].IntegerLiteral == -1 || _registers[(int)Register.Compare].IntegerLiteral == 0)
                                _instructionPointer = ResolveOperand(instruction, 0).InstrIndex;
                            break;
                        case OpCode.JMP_GE:
                            if (_registers[(int)Register.Compare].IntegerLiteral == 1 || _registers[(int)Register.Compare].IntegerLiteral == 0)
                                _instructionPointer = ResolveOperand(instruction, 0).InstrIndex;
                            break;
                        case OpCode.JMP_NE:
                            if (_registers[(int)Register.Compare].IntegerLiteral != 0)
                                _instructionPointer = ResolveOperand(instruction, 0).InstrIndex;
                            break;
                        case OpCode.JMP:
                            _instructionPointer = ResolveOperand(instruction, 0).InstrIndex;
                            break;
                        #endregion
                        #region IS
                        case OpCode.IS_EQ:
                            stack1 = _runtimeStack.PushEmpty(RuntimeValueType.BooleanLiteral);
                            stack1.BooleanLiteral = (_registers[(int)Register.Compare].IntegerLiteral == 0);
                            break;
                        case OpCode.IS_L:
                            stack1 = _runtimeStack.PushEmpty(RuntimeValueType.BooleanLiteral);
                            stack1.BooleanLiteral = (_registers[(int)Register.Compare].IntegerLiteral == -1);
                            break;
                        case OpCode.IS_G:
                            stack1 = _runtimeStack.PushEmpty(RuntimeValueType.BooleanLiteral);
                            stack1.BooleanLiteral = (_registers[(int)Register.Compare].IntegerLiteral == 1);
                            break;
                        case OpCode.IS_LE:
                            stack1 = _runtimeStack.PushEmpty(RuntimeValueType.BooleanLiteral);
                            stack1.BooleanLiteral = (_registers[(int)Register.Compare].IntegerLiteral == -1 || _registers[(int)Register.Compare].IntegerLiteral == 0);
                            break;
                        case OpCode.IS_GE:
                            stack1 = _runtimeStack.PushEmpty(RuntimeValueType.BooleanLiteral);
                            stack1.BooleanLiteral = (_registers[(int)Register.Compare].IntegerLiteral == 1 || _registers[(int)Register.Compare].IntegerLiteral == 0);
                            break;
                        case OpCode.IS_NE:
                            stack1 = _runtimeStack.PushEmpty(RuntimeValueType.BooleanLiteral);
                            stack1.BooleanLiteral = (_registers[(int)Register.Compare].IntegerLiteral != 0);
                            break;
                        #endregion
                        #region Logical Operations
                        case OpCode.LOGICAL_AND:
                            stack1 = _runtimeStack.PushEmpty(RuntimeValueType.BooleanLiteral);
                            stack1.BooleanLiteral = (ResolveOperand(instruction, 0).BooleanLiteral && ResolveOperand(instruction, 1).BooleanLiteral);
                            break;
                        case OpCode.LOGICAL_OR:
                            stack1 = _runtimeStack.PushEmpty(RuntimeValueType.BooleanLiteral);
                            stack1.BooleanLiteral = (ResolveOperand(instruction, 0).BooleanLiteral || ResolveOperand(instruction, 1).BooleanLiteral);
                            break;
                        case OpCode.LOGICAL_NOT:
                            stack1 = _runtimeStack.PushEmpty(RuntimeValueType.BooleanLiteral);
                            stack1.BooleanLiteral = !ResolveOperand(instruction, 0).BooleanLiteral;
                            break;
                        #endregion
                        #region Stack Frame Manipulation
                        case OpCode.CALL:

                            // Increase instruction following this call so we don't 
                            // end up going round is circles when this call returns.
                            _instructionPointer++;
                            CallFunction(ResolveOperand(instruction, 0).Symbol as FunctionSymbol, false);
                            break;

                        case OpCode.RETURN:

                            // Find the function index and throw corruption error if its not there.
                            RuntimeValue functionValue = _runtimeStack.Pop();
                            if (functionValue.ValueType != RuntimeValueType.StackFrameIndex && functionValue.ValueType != RuntimeValueType.StackBaseMarker)
                            {
                                // Ahh hell, lets try and recover.
#if DEBUG
                                Error("Stack has been corrupted.");
#else
                                while (functionValue.ValueType != RuntimeValueType.StackFrameIndex && functionValue.ValueType != RuntimeValueType.StackBaseMarker)
                                {
                                    if (_runtimeStack.TopIndex == 0)
                                    {
                                        Error("Stack has been corrupted.");
                                        break;
                                    }
                                    functionValue = _runtimeStack.Pop();
                                }

#endif
                            }

                            // Find the parameter count and local data size.
                            int parameterCount = 0, localDataSize = 0;
                            if (functionValue.Symbol.Type == SymbolType.Function)
                            {
                                parameterCount = ((FunctionSymbol)functionValue.Symbol).ParameterCount;
                                localDataSize = ((FunctionSymbol)functionValue.Symbol).LocalDataSize;
                            }

                            // Find the return value stack entry and throw corruption error if its not there.
                            RuntimeValue returnAddressValue = _runtimeStack[_runtimeStack.TopIndex - (localDataSize + 1)];
                            if (returnAddressValue.ValueType != RuntimeValueType.ReturnAddress)
                                Error("Stack has been corrupted.");

                            // Reduce the reference count of any local varaibles.
                            DataTypeValue[] parameterTypes = new DataTypeValue[((FunctionSymbol)functionValue.Symbol).ParameterCount];
                            for (int i = 0; i < ((FunctionSymbol)functionValue.Symbol).ParameterCount; i++)
                                parameterTypes[(((FunctionSymbol)functionValue.Symbol).ParameterCount - 1) - i] = ((VariableSymbol)((FunctionSymbol)functionValue.Symbol).Symbols[i]).DataType;

                            // Remove reference counts for parameters.
                            //for (int i = 0; i < parameterCount; i++)
                            //{
                            //    RuntimeValue parameter = _runtimeStack[(_runtimeStack.TopIndex - (parameterCount + 1)) + i];
                            //   if (parameterTypes[i].DataType == DataType.Object && parameter.ObjectIndex != -1 && _process.ObjectHeap[parameter.ObjectIndex] != null)
                            //    {
                            //        _process.ObjectHeap[parameter.ObjectIndex].ReferenceCount--; // DIE!
                            //    }
                            //}

                            // Remove reference counts for local variables.
                            //foreach (Symbol symbol in functionValue.Symbol.Symbols)
                            //{
                            //    if (!(symbol is VariableSymbol) || ((VariableSymbol)symbol).VariableType != VariableType.Local) continue;
                            //    RuntimeValue local = GetRuntimeValueLocal(symbol.Identifier);
                            //    if (((VariableSymbol)symbol).DataType.DataType == DataType.Object && local.ObjectIndex != -1 && _process.ObjectHeap[local.ObjectIndex] != null)
                            //        _process.ObjectHeap[local.ObjectIndex].ReferenceCount--; // DIE!
                            //}

                            // Pop the functions stack frame.
                            //_runtimeStack.PopFrame(parameterCount + localDataSize + 1);
                            for (int i = 0; i < (parameterCount + localDataSize + 1); i++)
                            {
                                RuntimeValue stack = _runtimeStack.Pop();

                                if (stack.ObjectIndex != -1) SetObjectValue(stack, -1);
                                else if (stack.MemoryIndex != -1) SetMemoryIndexValue(stack, -1);
                            }

                            // Restore the previous stack.
                            _runtimeStack.FrameIndex = functionValue.InstrIndex;

                            // Jump to the return address.
                            _instructionPointer = returnAddressValue.InstrIndex;

                            //if (_process.Url == "media\\scripts\\LTTP 2.0.fs" && _callStack.Peek().ToString().ToLower() == "onrender")
                            //{
                            //    System.Console.WriteLine("\n\n--- OnRender Statistics ---");
                            //    for (int i = 0; i < opCodeTimes.Length; i++)
                            //        System.Console.WriteLine("Time spent on " + ((OpCode)i) + " = " + opCodeTimes[i] + " ms");
                            //}

                            // Pop this function off the call stack.
                            _callStack.Pop();

                            // If its a stack base marker make sure we exit the loop at the end.
                            if (functionValue.ValueType == RuntimeValueType.StackBaseMarker)
                                return 2;

                            // Out of things to run?
                            if (_callStack.Count == 0)
                                return 5;

                            break;
                        #endregion
                        #region Execution

                        // Execution related op codes.
                        case OpCode.EXIT:
                            _isRunning = false;
                            _instructionPointer++;
                            return 3;

                        case OpCode.PAUSE:
                            _isPaused = true;
                            _pauseLength = ResolveOperand(instruction, 0).IntegerLiteral;
                            _pauseTimer.Restart();
                            _instructionPointer++;
                            return 0;

                        case OpCode.GOTO_STATE:
                            _process.ChangeState(ResolveOperand(instruction, 0).Symbol as StateSymbol);
                            break;
                        //return 4;

                        #endregion
                        #region Memory Allocation

                        case OpCode.ALLOCATE_HEAP_NULL:
                            ResolveOperand(instruction, 1).MemoryIndex = _process.AllocateHeap(ResolveOperand(instruction, 0).IntegerLiteral, RuntimeValueType.Invalid, null);
                            break;
                        case OpCode.ALLOCATE_HEAP_BOOL:
                            ResolveOperand(instruction, 1).MemoryIndex = _process.AllocateHeap(ResolveOperand(instruction, 0).IntegerLiteral, RuntimeValueType.BooleanLiteral, null);
                            break;
                        case OpCode.ALLOCATE_HEAP_BYTE:
                            ResolveOperand(instruction, 1).MemoryIndex = _process.AllocateHeap(ResolveOperand(instruction, 0).IntegerLiteral, RuntimeValueType.ByteLiteral, null);
                            break;
                        case OpCode.ALLOCATE_HEAP_INT:
                            ResolveOperand(instruction, 1).MemoryIndex = _process.AllocateHeap(ResolveOperand(instruction, 0).IntegerLiteral, RuntimeValueType.IntegerLiteral, null);
                            break;
                        case OpCode.ALLOCATE_HEAP_SHORT:
                            ResolveOperand(instruction, 1).MemoryIndex = _process.AllocateHeap(ResolveOperand(instruction, 0).IntegerLiteral, RuntimeValueType.ShortLiteral, null);
                            break;
                        case OpCode.ALLOCATE_HEAP_FLOAT:
                            ResolveOperand(instruction, 1).MemoryIndex = _process.AllocateHeap(ResolveOperand(instruction, 0).IntegerLiteral, RuntimeValueType.FloatLiteral, null);
                            break;
                        case OpCode.ALLOCATE_HEAP_DOUBLE:
                            ResolveOperand(instruction, 1).MemoryIndex = _process.AllocateHeap(ResolveOperand(instruction, 0).IntegerLiteral, RuntimeValueType.DoubleLiteral, null);
                            break;
                        case OpCode.ALLOCATE_HEAP_LONG:
                            ResolveOperand(instruction, 1).MemoryIndex = _process.AllocateHeap(ResolveOperand(instruction, 0).IntegerLiteral, RuntimeValueType.LongLiteral, null);
                            break;
                        case OpCode.ALLOCATE_HEAP_Object:
                            ResolveOperand(instruction, 1).MemoryIndex = _process.AllocateHeap(ResolveOperand(instruction, 0).IntegerLiteral, RuntimeValueType.Object, null);
                            break;
                        case OpCode.ALLOCATE_HEAP_STRING:
                            ResolveOperand(instruction, 1).MemoryIndex = _process.AllocateHeap(ResolveOperand(instruction, 0).IntegerLiteral, RuntimeValueType.StringLiteral, null);
                            break;
                        case OpCode.DEALLOCATE_HEAP:
                            op1 = ResolveOperand(instruction, 0);
                            if (op1.MemoryIndex <= 0) break;
                            _process.DeallocateHeap(op1.MemoryIndex);
                            break;

                        #endregion
                        #region Debug

                        // Debug op codes.
                        case OpCode.BREAKPOINT:
                            if (_debugger == null)
                                ShowDebugger();
                            else if (_debugger.NotifyOnBreakPoint == true)
                                _debugger.NotifyOfBreakPoint();
                            break;

                        // These are for the debugger. We can ignore them :P.
                        case OpCode.ENTER_STATEMENT:
                            if (_debugger != null && _debugger.NotifyOnEnter == true)
                                _debugger.NotifyOfEntry();
                            break;

                        case OpCode.EXIT_STATEMENT:
                            if (_debugger != null && _debugger.NotifyOnExit == true)
                                _debugger.NotifyOfExit();
                            break;

                        #endregion
                        #region Thread syncronization
                        case OpCode.LOCK:
                            if (instruction.Locked == true)
                                return 1; // Return and don't increment instruction pointer.
                            else
                            {
                                instruction.Locked = true;
                                _lockInstruction = instruction;
                            }
                            break;
                        case OpCode.UNLOCK:
                            _lockInstruction.Locked = false;
                            break;
                        case OpCode.ENTER_ATOM:
                            _previousInAtom = _inAtom;
                            _inAtom = true;
                            break;
                        case OpCode.LEAVE_ATOM:
                            _inAtom = _previousInAtom;
                            break;
                        #endregion
                        #region Member Modification
                        case OpCode.CALL_METHOD: // Object, Symbol
                            {
                                // Grab the object we are invoked a method off.
                                RuntimeValue objValue = ResolveOperand(instruction, 0);
                                if (objValue.ObjectIndex == -1) Error("Attempt to call method of null object.");
                                RuntimeObject obj = _process.ObjectHeap[objValue.ObjectIndex];

                                // Grab the function.
                                FunctionSymbol symbol = ResolveOperand(instruction, 1).Symbol as FunctionSymbol;

                                // Clean out the return register and push this function onto the callstack.
                                _callStack.Push(symbol);
                                _registers[(int)Register.Return].Clear();

                                // Invoke the method.
                                if (obj.InvokeMethod(this, symbol) == false)
                                    Error("An error occured while attempting to call a objects method. The method may not exist or may not be public.");

                                // Pop of this functions parameters.
                                for (int i = 0; i < symbol.ParameterCount; i++)
                                    _runtimeStack.Pop();

                                // Pop this value off the call stack.
                                _callStack.Pop();

                                break;
                            }
                        case OpCode.SET_MEMBER: // Object, symbol, value
                            {
                                // Grab the object we are invoked a method off.
                                RuntimeValue objValue = ResolveOperand(instruction, 0);
                                if (objValue.ObjectIndex == -1) Error("Attempt to set member of null object.");
                                RuntimeObject obj = _process.ObjectHeap[objValue.ObjectIndex];

                                // Grab the variable.
                                VariableSymbol symbol = ResolveOperand(instruction, 1).Symbol as VariableSymbol;

                                // Invoke the method.
                                if (obj.SetMember(this, symbol, ResolveOperand(instruction, 2)) == false)
                                    Error("An error occured while attempting to set an objects member. The member may not exist or may not be public.");

                                break;
                            }
                        case OpCode.SET_MEMBER_INDEXED: // Object, symbol, value, index
                            {
                                // Grab the object we are invoked a method off.
                                RuntimeValue objValue = ResolveOperand(instruction, 0);
                                if (objValue.ObjectIndex == -1) Error("Attempt to set member of null object.");
                                RuntimeObject obj = _process.ObjectHeap[objValue.ObjectIndex];

                                // Grab the variable.
                                VariableSymbol symbol = ResolveOperand(instruction, 1).Symbol as VariableSymbol;

                                // Invoke the method.
                                if (obj.SetMember(this, symbol, ResolveOperand(instruction, 2), ResolveOperand(instruction, 3)) == false)
                                    Error("An error occured while attempting to set an objects member. The member may not exist or may not be public.");

                                break;
                            }
                        case OpCode.GET_MEMBER: // Object, symbol
                            {
                                // Grab the object we are invoked a method off.
                                RuntimeValue objValue = ResolveOperand(instruction, 0);
                                if (objValue.ObjectIndex == -1) Error("Attempt to set member of null object.");
                                RuntimeObject obj = _process.ObjectHeap[objValue.ObjectIndex];

                                // Grab the variable.
                                VariableSymbol symbol = ResolveOperand(instruction, 1).Symbol as VariableSymbol;

                                // Clean out the return register.
                                _registers[(int)Register.Return].Clear();

                                // Invoke the method.
                                if (obj.SetMember(this, symbol, ResolveOperand(instruction, 2)) == false)
                                    Error("An error occured while attempting to get an objects member. The member may not exist or may not be public.");

                                break;
                            }
                        case OpCode.GET_MEMBER_INDEXED: // Object, symbol, index
                            {
                                // Grab the object we are invoked a method off.
                                RuntimeValue objValue = ResolveOperand(instruction, 0);
                                if (objValue.ObjectIndex == -1) Error("Attempt to set member of null object.");
                                RuntimeObject obj = _process.ObjectHeap[objValue.ObjectIndex];

                                // Grab the variable.
                                VariableSymbol symbol = ResolveOperand(instruction, 1).Symbol as VariableSymbol;

                                // Clean out the return register.
                                _registers[(int)Register.Return].Clear();

                                // Invoke the method.
                                if (obj.SetMember(this, symbol, ResolveOperand(instruction, 2), ResolveOperand(instruction, 3)) == false)
                                    Error("An error occured while attempting to get an objects member. The member may not exist or may not be public.");

                                break;
                            }
                        #endregion

                        // 'ello, 'ello. What goin' on 'ere then.
                        default:
                            Error("Virtual Machine encountered an invalid operation code ("+instruction.OpCode.ToString()+").");
                            break;
                    }
                    #endregion

                    // Move onto next instruction if this instruction has not progressed the instruction pointer.
                    if (_instructionPointer == originalPointer) _instructionPointer++;

                    //opCodeTimes[(int)instruction.OpCode] += (float)instructionTimer.DurationMillisecond;
                    //if (_process.Url == "media\\scripts\\LTTP 2.0.fs")
                    //    System.Console.WriteLine(instruction.OpCode.ToString() + " executed in " + instructionTimer.DurationMillisecond + "ms");

                    _instructionsExecuted++;
#if !DEBUG
                }
                catch (Exception e)
                {
                    Error(e.Message);
                }
#endif
            }
        }

        /// <summary>
        ///     Called when all hell breaks loose.
        /// </summary>
        /// <param name="message">Message describing the error that occured.</param>
        public void Error(string message)
        {
            Debug.DebugLogger.WriteLog("Virtual Machine (" + _process.Url + ") reported the following error;\n\t" + message, LogAlertLevel.Error); 
            ShowDebugger();
        }

        /// <summary>
        ///     Shows a debugger for this script thread.
        /// </summary>
        public void ShowDebugger()
        {
            _debugger = new Debugger();
            _debugger.ScriptThread = this;
            _debugger.Show();
        }

		/// <summary>
		///		Executes a script or native function.
		/// </summary>
		/// <param name="symbol">Symbol of function to call.</param>
		/// <param name="ignoreThread">If set and this function is a thread spawner a new thread will not be created.</param>
		private void CallFunction(FunctionSymbol symbol, bool ignoreThread)
		{
            // Can't call if debugging :S.
            if (_debugger != null && _debugger.RunScript == false)
                return;
            
			// Push this function onto the call stack.
			_callStack.Push(symbol);
			_registers[(int)Register.Return].Clear();
			_callingFunction = symbol;

			if (symbol.IsImport == true)
			{
				// Find and call native function.
				if (_process.VirtualMachine != null)
                {
                    DataTypeValue[] parameterTypes = null;
					NativeFunction function = symbol.NativeFunction;
                    if (function == null)
                    {
                        // Find the functions parameter types.
                        parameterTypes = new DataTypeValue[symbol.ParameterCount];
                        for (int i = 0; i < symbol.ParameterCount; i++)
                            parameterTypes[(symbol.ParameterCount - 1) - i] = ((VariableSymbol)symbol.Symbols[i]).DataType;

                        // Find native function
                        function = _process.VirtualMachine.FindNativeFunction(symbol.Identifier, parameterTypes);
                        symbol.NativeFunction = function;
                    }

                    if (function != null)
                    {
                        function.Delegate(this);
                    }

                    // Ok so its not a native one, lets check the script exports.
                    else
                    {
                        Array.Reverse(parameterTypes);

                        ScriptExportFunction callFunction = null;
                        foreach (ScriptExportFunction exportFunction in ScriptExportFunction.FunctionList)
                        {
                            if (exportFunction.Symbol.Identifier.ToLower() != symbol.Identifier.ToLower()) continue;
                            if (exportFunction.Symbol.ParameterCount != parameterTypes.Length) continue;

                            bool paramsValid = true;
                            for (int i = 0; i < parameterTypes.Length; i++)
                                if (exportFunction.Symbol.CheckParameterTypeValid(i, parameterTypes[i]) == false)
                                    paramsValid = false;

                            if (paramsValid == false) continue;

                            callFunction = exportFunction;
                            break;
                        }

                        if (callFunction != null)
                        {
                            InvokeExportFunction(callFunction.Symbol, callFunction.Thread, this);
                        }
                        else
                            Error("Attempt to call unknown function '" + symbol.Identifier + "'");
                    }
				}

				// Pop of this functions parameters.
                for (int i = 0; i < symbol.ParameterCount; i++)
                {
                    RuntimeValue stack = _runtimeStack.Pop();

                    if (stack.ObjectIndex != -1) SetObjectValue(stack, -1);
                    else if (stack.MemoryIndex != -1) SetMemoryIndexValue(stack, -1);
                }

				// Pop this value off the call stack.
				_callStack.Pop();
			}
			else
			{
				if (symbol.IsThreadSpawner == true && ignoreThread == false)
					SpawnThread(symbol);
				else
				{
					int oldFrameIndex = _runtimeStack.FrameIndex;

					// Push the return address which is the current instruction.
					RuntimeValue returnAddress = _runtimeStack.PushEmpty(RuntimeValueType.ReturnAddress);
					returnAddress.InstrIndex = _instructionPointer;

					// Pushs this function stack frame onto stack.
					_runtimeStack.PushFrame(symbol.LocalDataSize + 1);

					// Place the function and old stack index into value
					// and push it onto stack.
					RuntimeValue stackValue = _runtimeStack.Peek();
					stackValue.ValueType = RuntimeValueType.StackFrameIndex;
					stackValue.Symbol = symbol;
					stackValue.InstrIndex = oldFrameIndex;

					// Jump to entry point of this function.
					_instructionPointer = symbol.EntryPoint;

					// Force this script into 'running' mode if its been stopped.
					_isRunning = true;
				}
			}
		}
		private void CallFunction(string identifier, bool ignoreThread)
		{
			foreach(Symbol symbol in _process.GlobalScope.Symbols)
			{
				if (symbol.Identifier.ToLower() != identifier.ToLower() || symbol.Type != SymbolType.Function) continue;
				CallFunction(symbol as FunctionSymbol, ignoreThread);
				return;
			}
			foreach (Symbol symbol in _process.State.Symbols)
			{
				if (symbol.Identifier.ToLower() != identifier.ToLower() || symbol.Type != SymbolType.Function) continue;
				CallFunction(symbol as FunctionSymbol, ignoreThread);
				return;
			}
			Error("Unable to call function \""+identifier+"\", function does not exist.");
		}

        /// <summary>
        ///     Invokes a export function from a given script.
        /// </summary>
        /// <param name="symbol">Symbol of function to call.</param>
        /// <param name="toThread">Thread to call function.</param>
        /// <param name="fromThread">Thread to call function from.</param>
        /// <returns>Memory index of new array.</returns>
        public void InvokeExportFunction(FunctionSymbol symbol, ScriptThread toThread, ScriptThread fromThread)
        {
            // Can't invoke functions if debugging :S.
            if (_debugger != null && _debugger.AllowInvokedFunctions == false && toThread == this)
                return;

            // Find the functions parameter types.
            DataTypeValue[] parameterTypes = new DataTypeValue[symbol.ParameterCount];
            for (int i = 0; i < symbol.ParameterCount; i++)
                parameterTypes[i] = ((VariableSymbol)symbol.Symbols[i]).DataType;

            // Push any parameters we have been given.
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                RuntimeValue parameter = fromThread._runtimeStack[(fromThread._runtimeStack.TopIndex - parameterTypes.Length) + i];
                parameter.DataType = parameterTypes[i];

                if (parameterTypes[i].IsArray == true)
                    toThread.PassParameterArray(CopyValueArrayFromThread(parameter, toThread, fromThread));
                else
                    toThread.PassParameter(CopyValueFromThread(parameter, toThread, fromThread));
            }

            // Call the function.
            toThread.InvokeFunction(symbol.Identifier, true, true);

            // Copy the return register.
            fromThread._registers[(int)Register.Return].DataType = symbol.ReturnType;
            if (symbol.ReturnType.IsArray == true)
                fromThread._registers[(int)Register.Return].MemoryIndex = CopyValueArrayFromThread(toThread._registers[(int)Register.Return], fromThread, toThread);
            else
                fromThread._registers[(int)Register.Return] = CopyValueFromThread(toThread._registers[(int)Register.Return], fromThread, toThread);          
        }

        /// <summary>
        ///     Copies a runtime array value from one thread to another whilst keeping track of objects and memory allocations.
        /// </summary>
        /// <param name="value">Value to copy.</param>
        /// <param name="toThread">Thread to copy from.</param>
        /// <param name="fromThread">Thread to copy to.</param>
        /// <returns>Memory index of new array.</returns>
        public int CopyValueArrayFromThread(RuntimeValue value, ScriptThread toThread, ScriptThread fromThread)
        {
            // Create the array.
            int arraySize = GetArrayLength(value.MemoryIndex);
            int arrayIndex = toThread.AllocateArray(value.DataType.DataType, arraySize);

            // Array of objects, how fun.
            if (value.DataType.DataType == DataType.Object)
            {
                for (int j = 0; j < arraySize; j++)
                {
                    RuntimeValue elementValue = fromThread._process.MemoryHeap[arrayIndex + j];
                    if (elementValue.ObjectIndex == -1)
                        toThread.SetArrayElement(arrayIndex, j, (RuntimeObject)null);
                    else
                        toThread.SetArrayElement(arrayIndex, j, fromThread._process.ObjectHeap[elementValue.ObjectIndex]);
                }
            }

            // Yay, an array of normal stuff.
            else
            {
                for (int j = 0; j < arraySize; j++)
                    toThread.SetArrayElement(arrayIndex, j, fromThread._process.MemoryHeap[arrayIndex + j]);
            }

            return arrayIndex;
        }

        /// <summary>
        ///     Copies a runtime value from one thread to another whilst keeping track of objects and memory allocations.
        /// </summary>
        /// <param name="value">Value to copy.</param>
        /// <param name="toThread">Thread to copy from.</param>
        /// <param name="fromThread">Thread to copy to.</param>
        /// <returns>New value.</returns>
        public RuntimeValue CopyValueFromThread(RuntimeValue value, ScriptThread toThread, ScriptThread fromThread)
        {
            if (value.ValueType == RuntimeValueType.Object)
            {
                int index = (value.ObjectIndex != -1) ? toThread._process.AllocateObjectIndex(fromThread._process.ObjectHeap[value.ObjectIndex]) : -1;
                RuntimeValue stackValue = new RuntimeValue(RuntimeValueType.Object);
                stackValue.DataType = value.DataType;
                SetObjectValue(stackValue, index); 

                return stackValue;
            }
            else
            {
                return value;
            }
        }

		/// <summary>
		///		Invokes a function to start running.
		/// </summary>
		/// <param name="identifier">Identifier of function to invoke.</param>
		/// <param name="waitForReturn">If true this function will not return until the invoked function has finished executing.</param>
		/// <param name="ignoreThread">If set and this function is a thread spawner a new thread will not be created.</param>
        /// <param name="stackTop">If set this function will not be called if an instance of thsi function is already at the top of the call stack.</param>
		public void InvokeFunction(string identifier, bool waitForReturn, bool ignoreThread, bool stackTop)
		{
            // Can't invoke functions if debugging :S.
            if (_debugger != null && _debugger.AllowInvokedFunctions == false)
                return;

			// Find function we are meant to be invoking.
			FunctionSymbol functionSymbol = null;

			// Check if its the global scope.
			if (identifier.ToLower() == "$global") 
				functionSymbol = _process.GlobalScope;

			// Look in states symbol list.
			if (_process.State != null && functionSymbol == null)
			{
				foreach (Symbol symbol in _process.State.Symbols)
				{
					if (symbol.Identifier.ToLower() != identifier.ToLower() || symbol.Type != SymbolType.Function) continue;
					functionSymbol = symbol as FunctionSymbol;
					break;
				}
			}

			if (functionSymbol == null)
			{
				foreach (Symbol symbol in _process.GlobalScope.Symbols)
				{
					if (symbol.Identifier.ToLower() != identifier.ToLower() || symbol.Type != SymbolType.Function) continue;
					functionSymbol = symbol as FunctionSymbol;
					break;
				}
			}

            InvokeFunction(functionSymbol, waitForReturn, ignoreThread, stackTop);
		}
        public void InvokeFunction(FunctionSymbol functionSymbol, bool waitForReturn, bool ignoreThread, bool stackTop)
        {
            // Can't invoke functions if debugging :S.
            if (_debugger != null && _debugger.AllowInvokedFunctions == false)
                return;
            
            // Check we haven't already been called.
            if (stackTop == true && _callStack.Count != 0 && _callStack.Contains(functionSymbol))
                return;

            // Check the function exists.
            if (functionSymbol == null || _isWaiting == true)
            {
                // Clear out the return register.
                _registers[(int)Register.Return].Clear();

                // Pop of passed parameters.
                for (int i = 0; i < _passedParameterCount; i++)
                {
                    RuntimeValue value = _runtimeStack.Pop(); 

                    if (value.ObjectIndex != -1) SetObjectValue(value, -1);
                    else if (value.MemoryIndex != -1) SetMemoryIndexValue(value, -1);
                }

                _passedParameterMask.Clear();
                _passedParameterCount = 0;
                return;
            }

            // Look through functions parameters and check the mask is correct.
            int parameterIndex = 0;
            foreach (DataTypeValue value in _passedParameterMask)
                if (functionSymbol.CheckParameterTypeValid(parameterIndex++, value) == false)
                {
                    for (int i = 0; i < _passedParameterCount; i++)
                    {
                        RuntimeValue stack = _runtimeStack.Pop(); 
                        if (stack.ObjectIndex != -1) SetObjectValue(stack, -1);
                        else if (stack.MemoryIndex != -1) SetMemoryIndexValue(stack, -1);
                    }
                    _passedParameterMask.Clear();
                    _passedParameterCount = 0;
                    return;
                }
            _passedParameterMask.Clear();
            _passedParameterCount = 0;

            // Call the function.
            _isRunning = true;
            CallFunction(functionSymbol, ignoreThread);

            // Push the stack base marker onto frame.
            _runtimeStack.Peek().ValueType = RuntimeValueType.StackBaseMarker;

            // Wait until function returns if thats what has been asked for.
            // Don't exit if thread is just paused.
            if (waitForReturn == true)
            {
                while (true)
                {
                    int retVal = Run(-1);
                    if (retVal == 2 || retVal == 3 || retVal == 4 || retVal == 5 || retVal == 6) break;
                }
            }
        }
        public void InvokeFunction(string identifier, bool waitForReturn, bool ignoreThread)
        {
            InvokeFunction(identifier, waitForReturn, ignoreThread, false);
        }
        public void InvokeFunction(FunctionSymbol symbol, bool waitForReturn, bool ignoreThread)
        {
            InvokeFunction(symbol, waitForReturn, ignoreThread, false);
        }
        public void InvokeFunction(string identifier)
		{
			InvokeFunction(identifier, false, false, false);
		}
        public void InvokeFunction(FunctionSymbol symbol)
        {
            InvokeFunction(symbol, false, false, false);
        }

		/// <summary>
		///		Creates a new thread attached to the given function.
		/// </summary>
		/// <param name="symbol">The symbol of the function to execute from.</param>
		private void SpawnThread(FunctionSymbol symbol)
		{
			ScriptThread thread = new ScriptThread(_process);
			thread.InvokeFunction(symbol.Identifier, false, true);
		}
		private void SpawnThread(string identifier)
		{
			foreach (Symbol symbol in _process.GlobalScope.Symbols)
			{
				if (symbol.Identifier.ToLower() != identifier.ToLower()) continue;
				SpawnThread(symbol as FunctionSymbol);
				break;
			}
		}

		/// <summary>
		///		Resolves a parameter on the current stack frame's runtime value.
		/// </summary>
		/// <param name="index">Index of parameter to resolve.</param>
		/// <returns>Resolved parameter's runtime value.</returns>
		private RuntimeValue ResolveParameter(int index)
		{
			int stackIndex = _runtimeStack.TopIndex - (_callingFunction.ParameterCount - (index));
			return _runtimeStack[stackIndex];
		}

        /// <summary>
        ///		Resolves a local variable to a runtime value.
        /// </summary>
        /// <param name="ident">Identifier of local variable.</param>
        /// <returns>Resolved local's runtime value.</returns>
        private RuntimeValue ResolveLocal(string ident)
        {
            FunctionSymbol currentScope = null;
            foreach (FunctionSymbol function in _callStack)
                if (function.IsImport == false)
                {
                    currentScope = function;
                    break;
                }

            if (currentScope == null)
                return null;

            return ResolveLocal(currentScope.FindSymbol(ident, SymbolType.Variable) as VariableSymbol);
        }

        /// <summary>
        ///		Resolves a local variable to a runtime value.
        /// </summary>
        /// <param name="variableSymbol">Symbol of variable to resolve.</param>
        /// <returns>Resolved local's runtime value.</returns>
        private RuntimeValue ResolveLocal(VariableSymbol variableSymbol)
        {
            RuntimeValue variableValue = null;

            if (variableSymbol.VariableType == VariableType.Global)
                variableValue = _process.MemoryHeap[variableSymbol.MemoryIndex];
            else if (variableSymbol.VariableType == VariableType.Local)
                variableValue = _runtimeStack[variableSymbol.StackIndex];
            else if (variableSymbol.VariableType == VariableType.Parameter)
                variableValue = _runtimeStack[variableSymbol.StackIndex];// ResolveParameter(_callingFunction.Symbols.Count - (_callingFunction.Symbols.IndexOf(variableSymbol) - 1));

            return variableValue;
        }

		/// <summary>
		///		Gets a parameter at the given index as a boolean.
		/// </summary>
		/// <param name="index">Index of parameter to retrieve.</param>
		/// <returns>Parameter's value.</returns>
		public bool GetBooleanParameter(int index)
		{
			RuntimeValue parameter = ResolveParameter(index);
			return parameter.BooleanLiteral;
		}

		/// <summary>
		///		Gets a parameter at the given index as a byte.
		/// </summary>
		/// <param name="index">Index of parameter to retrieve.</param>
		/// <returns>Parameter's value.</returns>
		public byte GetByteParameter(int index)
		{
			RuntimeValue parameter = ResolveParameter(index);
			return parameter.ByteLiteral;
		}

		/// <summary>
		///		Gets a parameter at the given index as a short.
		/// </summary>
		/// <param name="index">Index of parameter to retrieve.</param>
		/// <returns>Parameter's value.</returns>
		public short GetShortParameter(int index)
		{
			RuntimeValue parameter = ResolveParameter(index);
			return parameter.ShortLiteral;
		}

		/// <summary>
		///		Gets a parameter at the given index as a integer.
		/// </summary>
		/// <param name="index">Index of parameter to retrieve.</param>
		/// <returns>Parameter's value.</returns>
		public int GetIntegerParameter(int index)
		{
			RuntimeValue parameter = ResolveParameter(index);
			return parameter.IntegerLiteral;
		}

		/// <summary>
		///		Gets a parameter at the given index as a float.
		/// </summary>
		/// <param name="index">Index of parameter to retrieve.</param>
		/// <returns>Parameter's value.</returns>
		public float GetFloatParameter(int index)
		{
			RuntimeValue parameter = ResolveParameter(index);
			return parameter.FloatLiteral;
		}

		/// <summary>
		///		Gets a parameter at the given index as a double.
		/// </summary>
		/// <param name="index">Index of parameter to retrieve.</param>
		/// <returns>Parameter's value.</returns>
		public double GetDoubleParameter(int index)
		{
			RuntimeValue parameter = ResolveParameter(index);
			return parameter.DoubleLiteral;
		}

		/// <summary>
		///		Gets a parameter at the given index as a long.
		/// </summary>
		/// <param name="index">Index of parameter to retrieve.</param>
		/// <returns>Parameter's value.</returns>
		public long GetLongParameter(int index)
		{
			RuntimeValue parameter = ResolveParameter(index);
			return parameter.LongLiteral;
		}

		/// <summary>
		///		Gets a parameter at the given index as a string.
		/// </summary>
		/// <param name="index">Index of parameter to retrieve.</param>
		/// <returns>Parameter's value.</returns>
		public string GetStringParameter(int index)
		{
			RuntimeValue parameter = ResolveParameter(index);
			return parameter.StringLiteral;
		}

        /// <summary>
        ///		Gets a parameter at the given index as a runtime value.
        /// </summary>
        /// <param name="index">Index of parameter to retrieve.</param>
        /// <returns>Parameter's value.</returns>
        public RuntimeValue GetRuntimeValueParameter(int index)
        {
            return ResolveParameter(index).Clone();
        }

		/// <summary>
		///		Gets a parameter at the given index as a Object.
		/// </summary>
		/// <param name="index">Index of parameter to retrieve.</param>
		/// <returns>Parameter's value.</returns>
		public RuntimeObject GetObjectParameter(int index)
		{
			RuntimeValue parameter = ResolveParameter(index);
			return parameter.ObjectIndex == -1 ? null : _process.ObjectHeap[parameter.ObjectIndex];
		}

		/// <summary>
		///		Gets a parameter at the given index as an arrays memory index. 
		/// </summary>
		/// <param name="index">Index of parameter to retrieve.</param>
		/// <returns>Parameter's value.</returns>
		public int GetArrayParameter(int index)
		{
			RuntimeValue parameter = ResolveParameter(index);
			return parameter.MemoryIndex;
		}

		/// <summary>
		///		Pushs a given value onto the runtime stack to be used
		///		as a parameter when InvokeFunction is called.
		/// </summary>
		/// <param name="value">Value to push onto stack</param>
		public void PassParameter(bool value)
		{
			RuntimeValue stackValue = new RuntimeValue(RuntimeValueType.BooleanLiteral);
			stackValue.BooleanLiteral = value;
			_runtimeStack.Push(stackValue);
			_passedParameterCount++;
			_passedParameterMask.Add(new DataTypeValue(DataType.Bool, false, false));
		}
		public void PassParameter(byte value)
		{
			RuntimeValue stackValue = new RuntimeValue(RuntimeValueType.ByteLiteral);
			stackValue.ByteLiteral = value;
			_runtimeStack.Push(stackValue);
			_passedParameterCount++;
			_passedParameterMask.Add(new DataTypeValue(DataType.Byte, false, false));
		}
		public void PassParameter(short value)
		{
			RuntimeValue stackValue = new RuntimeValue(RuntimeValueType.ShortLiteral);
			stackValue.ShortLiteral = value;
			_runtimeStack.Push(stackValue);
			_passedParameterCount++;
			_passedParameterMask.Add(new DataTypeValue(DataType.Short, false, false));
		}
		public void PassParameter(int value)
		{
			RuntimeValue stackValue = new RuntimeValue(RuntimeValueType.IntegerLiteral);
			stackValue.IntegerLiteral = value;
			_runtimeStack.Push(stackValue);
			_passedParameterCount++;
			_passedParameterMask.Add(new DataTypeValue(DataType.Int, false, false));
		}
		public void PassParameter(float value)
		{
			RuntimeValue stackValue = new RuntimeValue(RuntimeValueType.FloatLiteral);
			stackValue.FloatLiteral = value;
			_runtimeStack.Push(stackValue);
			_passedParameterCount++;
			_passedParameterMask.Add(new DataTypeValue(DataType.Float, false, false));
		}
		public void PassParameter(double value)
		{
			RuntimeValue stackValue = new RuntimeValue(RuntimeValueType.DoubleLiteral);
			stackValue.DoubleLiteral = value;
			_runtimeStack.Push(stackValue);
			_passedParameterCount++;
			_passedParameterMask.Add(new DataTypeValue(DataType.Double, false, false));
		}
		public void PassParameter(long value)
		{
			RuntimeValue stackValue = new RuntimeValue(RuntimeValueType.LongLiteral);
			stackValue.LongLiteral = value;
			_runtimeStack.Push(stackValue);
			_passedParameterCount++;
			_passedParameterMask.Add(new DataTypeValue(DataType.Long, false, false));
		}
		public void PassParameter(string value)
		{
			RuntimeValue stackValue = new RuntimeValue(RuntimeValueType.StringLiteral);
			stackValue.StringLiteral = value;
			_runtimeStack.Push(stackValue);
			_passedParameterCount++;
			_passedParameterMask.Add(new DataTypeValue(DataType.String, false, false));
		}
		public void PassParameter(RuntimeObject value)
		{
			int index = _process.AllocateObjectIndex(value);
			RuntimeValue stackValue = new RuntimeValue(RuntimeValueType.Object);
            SetObjectValue(stackValue, index);

            _runtimeStack.Push(stackValue);
			_passedParameterCount++;
			_passedParameterMask.Add(new DataTypeValue(DataType.Object, false, false));
		}
		public void PassParameterArray(int memoryIndex)
		{
			RuntimeValue stackValue = new RuntimeValue(RuntimeValueType.DirectMemory);
            SetMemoryIndexValue(stackValue, memoryIndex);

			_runtimeStack.Push(stackValue); 
			_passedParameterCount++;
			_passedParameterMask.Add(_process.MemoryHeap[memoryIndex].DataType);
		}
        public void PassParameter(RuntimeValue value)
        {
            _runtimeStack.Push(value);
            _passedParameterCount++;
            _passedParameterMask.Add(value.DataType);
        }

        /// <summary>
        ///		Gets a local at the given index as a boolean.
        /// </summary>
        /// <param name="identifier">Identifier of local.</param>
        /// <returns>Variables's value.</returns>
        public bool GetBooleanLocal(string identifier)
        {
            RuntimeValue local = ResolveLocal(identifier);
            return local.BooleanLiteral;
        }

        /// <summary>
        ///		Gets a local at the given index as a byte.
        /// </summary>
        /// <param name="identifier">Identifier of local.</param>
        /// <returns>Variables's value.</returns>
        public byte GetByteLocal(string identifier)
        {
            RuntimeValue local = ResolveLocal(identifier);
            return local.ByteLiteral;
        }

        /// <summary>
        ///		Gets a local at the given index as a short.
        /// </summary>
        /// <param name="identifier">Identifier of local.</param>
        /// <returns>Variables's value.</returns>
        public short GetShortLocal(string identifier)
        {
            RuntimeValue local = ResolveLocal(identifier);
            return local.ShortLiteral;
        }

        /// <summary>
        ///		Gets a local at the given index as a integer.
        /// </summary>
        /// <param name="identifier">Identifier of local.</param>
        /// <returns>Variables's value.</returns>
        public int GetIntegerLocal(string identifier)
        {
            RuntimeValue local = ResolveLocal(identifier);
            return local.IntegerLiteral;
        }

        /// <summary>
        ///		Gets a local at the given index as a float.
        /// </summary>
        /// <param name="identifier">Identifier of local.</param>
        /// <returns>Variables's value.</returns>
        public float GetFloatLocal(string identifier)
        {
            RuntimeValue local = ResolveLocal(identifier);
            return local.FloatLiteral;
        }

        /// <summary>
        ///		Gets a local at the given index as a double.
        /// </summary>
        /// <param name="identifier">Identifier of local.</param>
        /// <returns>Variables's value.</returns>
        public double GetDoubleLocal(string identifier)
        {
            RuntimeValue local = ResolveLocal(identifier);
            return local.DoubleLiteral;
        }

        /// <summary>
        ///		Gets a local at the given index as a long.
        /// </summary>
        /// <param name="identifier">Identifier of local.</param>
        /// <returns>Variables's value.</returns>
        public long GetLongLocal(string identifier)
        {
            RuntimeValue local = ResolveLocal(identifier);
            return local.LongLiteral;
        }

        /// <summary>
        ///		Gets a local at the given index as a string.
        /// </summary>
        /// <param name="identifier">Identifier of local.</param>
        /// <returns>Variables's value.</returns>
        public string GetStringLocal(string identifier)
        {
            RuntimeValue local = ResolveLocal(identifier);
            return local.StringLiteral;
        }

        /// <summary>
        ///		Gets a local at the given index as a runtime value.
        /// </summary>
        /// <param name="identifier">Identifier of local.</param>
        /// <returns>Variables's value.</returns>
        public RuntimeValue GetRuntimeValueLocal(string identifier)
        {
            return ResolveLocal(identifier);
        }

        /// <summary>
        ///		Gets a local at the given index as a Object.
        /// </summary>
        /// <param name="identifier">Identifier of local.</param>
        /// <returns>Local's value.</returns>
        public RuntimeObject GetObjectLocal(string identifier)
        {
            RuntimeValue local = ResolveLocal(identifier);
            return local.ObjectIndex == -1 ? null : _process.ObjectHeap[local.ObjectIndex];
        }

        /// <summary>
        ///		Gets a local at the given index as an array index.
        /// </summary>
        /// <param name="identifier">Identifier of local.</param>
        /// <returns>Variables's value.</returns>
        public int GetArrayLocal(string identifier)
        {
            RuntimeValue local = ResolveLocal(identifier);
            return local.MemoryIndex;
        }

        /// <summary>
        ///		Sets the value contained within a specified local variable.
        ///     WARNING: This is generally considered bad practice, its also unsafe.
        /// </summary>
        /// <param name="identifier">Identifier of the global variable to set.</param>
        /// <param name="value">Value to place into global variable.</param>
        public void SetLocalVariable(string identifier, bool value)
        {
            RuntimeValue localValue = ResolveLocal(identifier);
            localValue.BooleanLiteral = value;
        }
        public void SetLocalVariable(string identifier, byte value)
        {
            RuntimeValue localValue = ResolveLocal(identifier);
            localValue.ByteLiteral = value;
        }
        public void SetLocalVariable(string identifier, short value)
        {
            RuntimeValue localValue = ResolveLocal(identifier);
            localValue.ShortLiteral = value;
        }
        public void SetLocalVariable(string identifier, int value)
        {
            RuntimeValue localValue = ResolveLocal(identifier);
            localValue.IntegerLiteral = value;
        }
        public void SetLocalVariable(string identifier, float value)
        {
            RuntimeValue localValue = ResolveLocal(identifier);
            localValue.FloatLiteral = value;
        }
        public void SetLocalVariable(string identifier, double value)
        {
            RuntimeValue localValue = ResolveLocal(identifier);
            localValue.DoubleLiteral = value;
        }
        public void SetLocalVariable(string identifier, long value)
        {
            RuntimeValue localValue = ResolveLocal(identifier);
            localValue.LongLiteral = value;
        }
        public void SetLocalVariable(string identifier, RuntimeObject value)
        {
            RuntimeValue localValue = ResolveLocal(identifier);
            SetObjectValue(localValue, _process.AllocateObjectIndex(value));
        }
        public void SetLocalVariable(string identifier, string value)
        {
            RuntimeValue localValue = ResolveLocal(identifier);
            localValue.StringLiteral = value;
        }
        public void SetLocalVariable(string identifier, RuntimeValue value)
        {
            RuntimeValue localValue = ResolveLocal(identifier);
            value.CopyTo(localValue);
        }
        public void SetLocalArray(string identifier, int value)
        {
            RuntimeValue localValue = ResolveLocal(identifier);
            SetMemoryIndexValue(localValue, value);
        }

		/// <summary>
		///		Allocates space on the heap for an array of the given
		///		type and size.
		/// </summary>
		/// <param name="type">Data type array should store.</param>
		/// <param name="size">Size of array to allocate.</param>
		/// <returns>Memory index of heap where array was allocated.</returns>
		public int AllocateArray(DataType type, int size)
		{
			switch(type)
			{
				case DataType.Bool:		return _process.AllocateHeap(size, RuntimeValueType.BooleanLiteral, new DataTypeValue(type, true, false));
				case DataType.Byte: return _process.AllocateHeap(size, RuntimeValueType.ByteLiteral, new DataTypeValue(type, true, false));
				case DataType.Short: return _process.AllocateHeap(size, RuntimeValueType.ShortLiteral, new DataTypeValue(type, true, false));
				case DataType.Int: return _process.AllocateHeap(size, RuntimeValueType.IntegerLiteral, new DataTypeValue(type, true, false));
				case DataType.Float: return _process.AllocateHeap(size, RuntimeValueType.FloatLiteral, new DataTypeValue(type, true, false));
				case DataType.Double: return _process.AllocateHeap(size, RuntimeValueType.DoubleLiteral, new DataTypeValue(type, true, false));
				case DataType.Long: return _process.AllocateHeap(size, RuntimeValueType.LongLiteral, new DataTypeValue(type, true, false));
				case DataType.String: return _process.AllocateHeap(size, RuntimeValueType.StringLiteral, new DataTypeValue(type, true, false));
				case DataType.Object: return _process.AllocateHeap(size, RuntimeValueType.Object, new DataTypeValue(type, true, false));
			}
			return 0;
		}

		/// <summary>
		///		Sets the value contained in the array element.
		/// </summary>
		/// <param name="memoryIndex">Memory index of the array in the heap.</param>
		/// <param name="offset">Offset to the element in the array.</param>
		/// <param name="value">Value to place into array element.</param>
		public void SetArrayElement(int memoryIndex, int offset, bool value)
		{
			_process.MemoryHeap[memoryIndex + offset].BooleanLiteral = value;
		}
		public void SetArrayElement(int memoryIndex, int offset, byte value)
		{
			_process.MemoryHeap[memoryIndex + offset].ByteLiteral = value;
		}
		public void SetArrayElement(int memoryIndex, int offset, short value)
		{
			_process.MemoryHeap[memoryIndex + offset].ShortLiteral = value;
		}
		public void SetArrayElement(int memoryIndex, int offset, int value)
		{
			_process.MemoryHeap[memoryIndex + offset].IntegerLiteral = value;
		}
		public void SetArrayElement(int memoryIndex, int offset, float value)
		{
			_process.MemoryHeap[memoryIndex + offset].FloatLiteral = value;
		}
		public void SetArrayElement(int memoryIndex, int offset, double value)
		{
			_process.MemoryHeap[memoryIndex + offset].DoubleLiteral = value;
		}
		public void SetArrayElement(int memoryIndex, int offset, long value)
		{
			_process.MemoryHeap[memoryIndex + offset].LongLiteral = value;
		}
		public void SetArrayElement(int memoryIndex, int offset, RuntimeObject value)
		{
            SetObjectValue(_process.MemoryHeap[memoryIndex + offset], _process.AllocateObjectIndex(value));
		}
		public void SetArrayElement(int memoryIndex, int offset, string value)
		{
			_process.MemoryHeap[memoryIndex + offset].StringLiteral = value;
		}
        public void SetArrayElement(int memoryIndex, int offset, RuntimeValue value)
        {
            _process.MemoryHeap[memoryIndex + offset] = value;
        }

		/// <summary>
		///		Gets the boolean value contained in an array element.
		/// </summary>
		/// <param name="memoryIndex">Memory index of the array in the heap.</param>
		/// <param name="offset">Offset to the element in the array.</param>
		/// <returns>Value contained in the array element.</returns>
		public bool GetBooleanArrayElement(int memoryIndex, int offset)
		{
			return _process.MemoryHeap[memoryIndex + offset].BooleanLiteral;
		}

		/// <summary>
		///		Gets the byte value contained in an array element.
		/// </summary>
		/// <param name="memoryIndex">Memory index of the array in the heap.</param>
		/// <param name="offset">Offset to the element in the array.</param>
		/// <returns>Value contained in the array element.</returns>
		public byte GetByteArrayElement(int memoryIndex, int offset)
		{
			return _process.MemoryHeap[memoryIndex + offset].ByteLiteral;
		}

		/// <summary>
		///		Gets the short value contained in an array element.
		/// </summary>
		/// <param name="memoryIndex">Memory index of the array in the heap.</param>
		/// <param name="offset">Offset to the element in the array.</param>
		/// <returns>Value contained in the array element.</returns>
		public short GetShortArrayElement(int memoryIndex, int offset)
		{
			return _process.MemoryHeap[memoryIndex + offset].ShortLiteral;
		}

		/// <summary>
		///		Gets the integer value contained in an array element.
		/// </summary>
		/// <param name="memoryIndex">Memory index of the array in the heap.</param>
		/// <param name="offset">Offset to the element in the array.</param>
		/// <returns>Value contained in the array element.</returns>
		public int GetIntArrayElement(int memoryIndex, int offset)
		{
			return _process.MemoryHeap[memoryIndex + offset].IntegerLiteral;
		}

		/// <summary>
		///		Gets the float value contained in an array element.
		/// </summary>
		/// <param name="memoryIndex">Memory index of the array in the heap.</param>
		/// <param name="offset">Offset to the element in the array.</param>
		/// <returns>Value contained in the array element.</returns>
		public float GetFloatArrayElement(int memoryIndex, int offset)
		{
			return _process.MemoryHeap[memoryIndex + offset].FloatLiteral;
		}

		/// <summary>
		///		Gets the double value contained in an array element.
		/// </summary>
		/// <param name="memoryIndex">Memory index of the array in the heap.</param>
		/// <param name="offset">Offset to the element in the array.</param>
		/// <returns>Value contained in the array element.</returns>
		public double GetDoubleArrayElement(int memoryIndex, int offset)
		{
			return _process.MemoryHeap[memoryIndex + offset].DoubleLiteral;
		}

		/// <summary>
		///		Gets the string value contained in an array element.
		/// </summary>
		/// <param name="memoryIndex">Memory index of the array in the heap.</param>
		/// <param name="offset">Offset to the element in the array.</param>
		/// <returns>Value contained in the array element.</returns>
		public string GetStringArrayElement(int memoryIndex, int offset)
		{
			return _process.MemoryHeap[memoryIndex + offset].StringLiteral;
		}

        /// <summary>
        ///		Gets the runtime value contained in an array element.
        /// </summary>
        /// <param name="memoryIndex">Memory index of the array in the heap.</param>
        /// <param name="offset">Offset to the element in the array.</param>
        /// <returns>Value contained in the array element.</returns>
        public RuntimeValue GetRuntimeValueArrayElement(int memoryIndex, int offset)
        {
            return _process.MemoryHeap[memoryIndex + offset].Clone();
        }

		/// <summary>
		///		Gets the long value contained in an array element.
		/// </summary>
		/// <param name="memoryIndex">Memory index of the array in the heap.</param>
		/// <param name="offset">Offset to the element in the array.</param>
		/// <returns>Value contained in the array element.</returns>
		public long GetLongArrayElement(int memoryIndex, int offset)
		{
			return _process.MemoryHeap[memoryIndex + offset].LongLiteral;
		}

		/// <summary>
		///		Gets the Object value contained in an array element.
		/// </summary>
		/// <param name="memoryIndex">Memory index of the array in the heap.</param>
		/// <param name="offset">Offset to the element in the array.</param>
		/// <returns>Value contained in the array element.</returns>
		public RuntimeObject GetObjectArrayElement(int memoryIndex, int offset)
		{
			return _process.ObjectHeap[_process.MemoryHeap[memoryIndex + offset].ObjectIndex];
		}

		/// <summary>
		///		Sets the value contained within a specified global variable.
		/// </summary>
		/// <param name="identifier">Identifier of the global variable to set.</param>
		/// <param name="value">Value to place into global variable.</param>
		public void SetGlobalVariable(string identifier, bool value)
		{
			RuntimeValue globalValue = _process.MemoryHeap[((VariableSymbol)_process.GlobalScope.FindSymbol(identifier, SymbolType.Variable)).MemoryIndex];
			globalValue.BooleanLiteral = value;
		}
		public void SetGlobalVariable(string identifier, byte value)
		{
			RuntimeValue globalValue = _process.MemoryHeap[((VariableSymbol)_process.GlobalScope.FindSymbol(identifier, SymbolType.Variable)).MemoryIndex];
			globalValue.ByteLiteral = value;
		}
		public void SetGlobalVariable(string identifier, short value)
		{
			RuntimeValue globalValue = _process.MemoryHeap[((VariableSymbol)_process.GlobalScope.FindSymbol(identifier, SymbolType.Variable)).MemoryIndex];
			globalValue.ShortLiteral = value;
		}
		public void SetGlobalVariable(string identifier, int value)
		{
			RuntimeValue globalValue = _process.MemoryHeap[((VariableSymbol)_process.GlobalScope.FindSymbol(identifier, SymbolType.Variable)).MemoryIndex];
			globalValue.IntegerLiteral = value;
		}
		public void SetGlobalVariable(string identifier, float value)
		{
			RuntimeValue globalValue = _process.MemoryHeap[((VariableSymbol)_process.GlobalScope.FindSymbol(identifier, SymbolType.Variable)).MemoryIndex];
			globalValue.FloatLiteral = value;
		}
		public void SetGlobalVariable(string identifier, double value)
		{
			RuntimeValue globalValue = _process.MemoryHeap[((VariableSymbol)_process.GlobalScope.FindSymbol(identifier, SymbolType.Variable)).MemoryIndex];
			globalValue.DoubleLiteral = value;
		}
		public void SetGlobalVariable(string identifier, long value)
		{
			RuntimeValue globalValue = _process.MemoryHeap[((VariableSymbol)_process.GlobalScope.FindSymbol(identifier, SymbolType.Variable)).MemoryIndex];
			globalValue.LongLiteral = value;
		}
		public void SetGlobalVariable(string identifier, RuntimeObject value)
		{
			RuntimeValue globalValue = _process.MemoryHeap[((VariableSymbol)_process.GlobalScope.FindSymbol(identifier, SymbolType.Variable)).MemoryIndex];
            SetObjectValue(globalValue, _process.AllocateObjectIndex(value));
		}
		public void SetGlobalVariable(string identifier, string value)
		{
			RuntimeValue globalValue = _process.MemoryHeap[((VariableSymbol)_process.GlobalScope.FindSymbol(identifier, SymbolType.Variable)).MemoryIndex];
			globalValue.StringLiteral = value;
		}
        public void SetGlobalVariable(string identifier, RuntimeValue value)
        {
            _process.MemoryHeap[((VariableSymbol)_process.GlobalScope.FindSymbol(identifier, SymbolType.Variable)).MemoryIndex] = value.Clone();
        }
        public void SetGlobalArray(string identifier, int value)
		{
			RuntimeValue globalValue = _process.MemoryHeap[((VariableSymbol)_process.GlobalScope.FindSymbol(identifier, SymbolType.Variable)).MemoryIndex];
            SetMemoryIndexValue(globalValue, value);
		}

		/// <summary>
		///		Gets the value of a specified global variable as a boolean.
		/// </summary>
		/// <param name="identifier">Identifier of global variable to get value of.</param>
		public bool GetBooleanGlobal(string identifier)
		{
			RuntimeValue globalValue = _process.MemoryHeap[((VariableSymbol)_process.GlobalScope.FindSymbol(identifier, SymbolType.Variable)).MemoryIndex];
			if (globalValue == null) Error("Unable to get global variable \"" + identifier + "\", variable doesn't exist.");
			return globalValue.BooleanLiteral;
		}

		/// <summary>
		///		Gets the value of a specified global variable as a byte.
		/// </summary>
		/// <param name="identifier">Identifier of global variable to get value of.</param>
		public byte GetByteGlobal(string identifier)
		{
			RuntimeValue globalValue = _process.MemoryHeap[((VariableSymbol)_process.GlobalScope.FindSymbol(identifier, SymbolType.Variable)).MemoryIndex];
			if (globalValue == null) Error("Unable to get global variable \"" + identifier + "\", variable doesn't exist.");
			return globalValue.ByteLiteral;
		}

		/// <summary>
		///		Gets the value of a specified global variable as a short.
		/// </summary>
		/// <param name="identifier">Identifier of global variable to get value of.</param>
		public short GetShortGlobal(string identifier)
		{
			RuntimeValue globalValue = _process.MemoryHeap[((VariableSymbol)_process.GlobalScope.FindSymbol(identifier, SymbolType.Variable)).MemoryIndex];
			if (globalValue == null) Error("Unable to get global variable \"" + identifier + "\", variable doesn't exist.");
			return globalValue.ShortLiteral;
		}

		/// <summary>
		///		Gets the value of a specified global variable as a integer.
		/// </summary>
		/// <param name="identifier">Identifier of global variable to get value of.</param>
		public int GetIntegerGlobal(string identifier)
		{
			RuntimeValue globalValue = _process.MemoryHeap[((VariableSymbol)_process.GlobalScope.FindSymbol(identifier, SymbolType.Variable)).MemoryIndex];
			if (globalValue == null) Error("Unable to get global variable \"" + identifier + "\", variable doesn't exist.");
			return globalValue.IntegerLiteral;
		}

		/// <summary>
		///		Gets the value of a specified global variable as a float.
		/// </summary>
		/// <param name="identifier">Identifier of global variable to get value of.</param>
		public float GetFloatGlobal(string identifier)
		{
			RuntimeValue globalValue = _process.MemoryHeap[((VariableSymbol)_process.GlobalScope.FindSymbol(identifier, SymbolType.Variable)).MemoryIndex];
			if (globalValue == null) Error("Unable to get global variable \"" + identifier + "\", variable doesn't exist.");
			return globalValue.FloatLiteral;
		}

		/// <summary>
		///		Gets the value of a specified global variable as a double.
		/// </summary>
		/// <param name="identifier">Identifier of global variable to get value of.</param>
		public double GetDoubleGlobal(string identifier)
		{
			RuntimeValue globalValue = _process.MemoryHeap[((VariableSymbol)_process.GlobalScope.FindSymbol(identifier, SymbolType.Variable)).MemoryIndex];
			if (globalValue == null) Error("Unable to get global variable \"" + identifier + "\", variable doesn't exist.");
			return globalValue.DoubleLiteral;
		}

		/// <summary>
		///		Gets the value of a specified global variable as a long.
		/// </summary>
		/// <param name="identifier">Identifier of global variable to get value of.</param>
		public long GetLongGlobal(string identifier)
		{
			RuntimeValue globalValue = _process.MemoryHeap[((VariableSymbol)_process.GlobalScope.FindSymbol(identifier, SymbolType.Variable)).MemoryIndex];
			if (globalValue == null) Error("Unable to get global variable \"" + identifier + "\", variable doesn't exist.");
			return globalValue.LongLiteral;
		}

		/// <summary>
		///		Gets the value of a specified global variable as a boolean.
		/// </summary>
		/// <param name="identifier">Identifier of global variable to get value of.</param>
		public string GetStringGlobal(string identifier)
		{
			RuntimeValue globalValue = _process.MemoryHeap[((VariableSymbol)_process.GlobalScope.FindSymbol(identifier, SymbolType.Variable)).MemoryIndex];
			if (globalValue == null) Error("Unable to get global variable \"" + identifier + "\", variable doesn't exist.");
			return globalValue.StringLiteral;
		}

        /// <summary>
        ///		Gets the value of a specified global variable as a runtime value.
        /// </summary>
        /// <param name="identifier">Identifier of global variable to get value of.</param>
        public RuntimeValue GetRuntimeValueGlobal(string identifier)
        {
            RuntimeValue globalValue = _process.MemoryHeap[((VariableSymbol)_process.GlobalScope.FindSymbol(identifier, SymbolType.Variable)).MemoryIndex];
            if (globalValue == null) Error("Unable to get global variable \"" + identifier + "\", variable doesn't exist.");
            return globalValue.Clone();
        }

		/// <summary>
		///		Gets the value of a specified global variable as an Object.
		/// </summary>
		/// <param name="identifier">Identifier of global variable to get value of.</param>
		public RuntimeObject GetObjectGlobal(string identifier)
		{
			RuntimeValue globalValue = _process.MemoryHeap[((VariableSymbol)_process.GlobalScope.FindSymbol(identifier, SymbolType.Variable)).MemoryIndex];
			if (globalValue == null) Error("Unable to get global variable \"" + identifier + "\", variable doesn't exist.");
            if (globalValue.ObjectIndex == -1) return null;
            return _process.ObjectHeap[globalValue.ObjectIndex];
		}

		/// <summary>
		///		Gets the value of a specified global variable as an array index.
		/// </summary>
		/// <param name="identifier">Identifier of global variable to get value of.</param>
		public int GetArrayGlobal(string identifier)
		{
			RuntimeValue globalValue = _process.MemoryHeap[((VariableSymbol)_process.GlobalScope.FindSymbol(identifier, SymbolType.Variable)).MemoryIndex];
			if (globalValue == null) Error("Unable to get global variable \"" + identifier + "\", variable doesn't exist.");
			return globalValue.MemoryIndex;
		}

		/// <summary>
		///		Gets the return value as a boolean.
		/// </summary>
		public bool GetBooleanReturnValue()
		{
			return _registers[(int)Register.Return].BooleanLiteral;
		}

		/// <summary>
		///		Gets the return value as a byte.
		/// </summary>
		public byte GetByteReturnValue()
		{
			return _registers[(int)Register.Return].ByteLiteral;
		}

		/// <summary>
		///		Gets the return value as a short.
		/// </summary>
		public short GetShortReturnValue()
		{
			return _registers[(int)Register.Return].ShortLiteral;
		}

		/// <summary>
		///		Gets the return value as an integer.
		/// </summary>
		public int GetIntegerReturnValue()
		{
			return _registers[(int)Register.Return].IntegerLiteral;
		}

		/// <summary>
		///		Gets the return value as a float.
		/// </summary>
		public float GetFloatReturnValue()
		{
			return _registers[(int)Register.Return].FloatLiteral;
		}

		/// <summary>
		///		Gets the return value as a double.
		/// </summary>
		public double GetDoubleReturnValue()
		{
			return _registers[(int)Register.Return].DoubleLiteral;
		}

		/// <summary>
		///		Gets the return value as a long.
		/// </summary>
		public long GetLongReturnValue()
		{
			return _registers[(int)Register.Return].LongLiteral;
		}

		/// <summary>
		///		Gets the return value as a string.
		/// </summary>
		public string GetStringReturnValue()
		{
			return _registers[(int)Register.Return].StringLiteral;
		}

        /// <summary>
        ///		Gets the return value as a runtime value.
        /// </summary>
        public RuntimeValue GetRuntimeValueReturnValue()
        {
            return _registers[(int)Register.Return].Clone();
        }

		/// <summary>
		///		Gets the return value as a Object.
		/// </summary>
		public RuntimeObject GetObjectReturnValue()
		{
			return _process.ObjectHeap[_registers[(int)Register.Return].ObjectIndex];
		}

		/// <summary>
		///		Gets the return value as an array index.
		/// </summary>
		public int GetArrayReturnValue()
		{
			return _registers[(int)Register.Return].MemoryIndex;
		}

		/// <summary>
		///		Sets the return value as a given value.
		/// </summary>
		/// <param name="value">Value to move into return value.</param>
		public void SetReturnValue(bool value)
		{
			_registers[(int)Register.Return].BooleanLiteral = value;
		}

		/// <summary>
		///		Sets the return value as a given value.
		/// </summary>
		/// <param name="value">Value to move into return value.</param>
		public void SetReturnValue(byte value)
		{
			_registers[(int)Register.Return].ByteLiteral = value;
		}

		/// <summary>
		///		Sets the return value as a given value.
		/// </summary>
		/// <param name="value">Value to move into return value.</param>
		public void SetReturnValue(short value)
		{
			_registers[(int)Register.Return].ShortLiteral = value;
		}

		/// <summary>
		///		Sets the return value as a given value.
		/// </summary>
		/// <param name="value">Value to move into return value.</param>
		public void SetReturnValue(int value)
		{
			_registers[(int)Register.Return].IntegerLiteral = value;
		}

		/// <summary>
		///		Sets the return value as a given value.
		/// </summary>
		/// <param name="value">Value to move into return value.</param>
		public void SetReturnValue(float value)
		{
			_registers[(int)Register.Return].FloatLiteral = value;
		}

		/// <summary>
		///		Sets the return value as a given value.
		/// </summary>
		/// <param name="value">Value to move into return value.</param>
		public void SetReturnValue(double value)
		{
			_registers[(int)Register.Return].DoubleLiteral = value;
		}

		/// <summary>
		///		Sets the return value as a given value.
		/// </summary>
		/// <param name="value">Value to move into return value.</param>
		public void SetReturnValue(long value)
		{
			_registers[(int)Register.Return].LongLiteral = value;
		}

		/// <summary>
		///		Sets the return value as a given value.
		/// </summary>
		/// <param name="value">Value to move into return value.</param>
		public void SetReturnValue(string value)
		{
			_registers[(int)Register.Return].StringLiteral = value;
		}

		/// <summary>
		///		Sets the return value as a given value.
		/// </summary>
		/// <param name="value">Value to move into return value.</param>
		public void SetReturnValue(RuntimeObject value)
		{
            SetObjectValue(_registers[(int)Register.Return], _process.AllocateObjectIndex(value));
		}

        /// <summary>
        ///		Sets the return value as a given value.
        /// </summary>
        /// <param name="value">Value to move into return value.</param>
        public void SetReturnValue(RuntimeValue value)
        {
            _registers[(int)Register.Return] = value.Clone();
        }

		/// <summary>
		///		Sets the return value as a given value.
		/// </summary>
		/// <param name="value">Value to move into return value.</param>
		public void SetReturnValueArray(int value)
		{
            SetMemoryIndexValue(_registers[(int)Register.Return], value);
		}

		/// <summary>
		///		Gets the size of the given array.
		/// </summary>
		/// <param name="value">Memory index of array.</param>
		public int GetArrayLength(int value)
		{
			return _process.MemoryHeap[value - 1].IntegerLiteral;
		}
	
		/// <summary>
		///		Paused the running of this script for the given amount of 
		///		milliseconds.
		/// </summary>
		/// <param name="length">Length of time in milliseconds to pause script for.</param>
		public void Pause(int length)
		{
			_isPaused = true;
			_pauseLength = length;
			_pauseTimer.Restart();
		}

		/// <summary>
		///		Resumes this thread if it is paused.
		/// </summary>
		public void Resume()
		{
			_isPaused = false;
		}

		/// <summary>
		///		Stops this script from running.
		/// </summary>
		public void Stop()
		{
			_isRunning = false;
		}

		/// <summary>
		///		Restarts this script if it has previously been stopped.
		/// </summary>
		public void Start()
		{
			_isRunning = true;
		}

		/// <summary>
		///		Changes this scripts state to the given state.
		/// </summary>
		public void ChangeState(StateSymbol symbol)
		{
			_callStack.Clear();
			_runtimeStack.Clear();
			_inAtom = false;
			_lockInstruction = null;
			_previousInAtom = false;
		}

		/// <summary>
		///		Initializes a new instance of this class: and associates it with
		///		the given script process.
		/// </summary>
		/// <param name="process">Script process that this thread is associated with.</param>
		public ScriptThread(ScriptProcess process)
		{
			_process = process;
			_process.AttachThread(this);
            InvokeFunction("$global", true, true, true);
        }

		#endregion
	}

}