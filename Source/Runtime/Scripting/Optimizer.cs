/* 
 * File: Optimizer.cs
 *
 * This source file contains the declarations of any and all classes
 * used to optimize the byte code of compiled scripts.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Collections;

namespace BinaryPhoenix.Fusion.Runtime.Scripting
{

	/// <summary>
	///		The script optimizer is responsible for removing unneccessary and redundent code
	///		from a scripts byte code whilst keeping the action the same.
	/// </summary>
	public sealed class ScriptOptimizer
	{
		#region Members
		#region Variables

		private ArrayList _instructionList = null;
		private ArrayList _symbolList = null;

		private Instruction _currentInstruction;
		private int _instructionIndex;
		private int _optimizationCount = 0;
        private int _totalOptimizationCount = 0;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the list of optimized instructions this optimizer has generated.
		/// </summary>
		public ArrayList OptimizedInstructions
		{
			get { return _instructionList; }
			set { _instructionList = value; }
		}

		/// <summary>
		///		Gets or sets the list of optimized symbols this optimizer has generated.
		/// </summary>
		public ArrayList OptimizedSymbols
		{
			get { return _symbolList; }
			set { _symbolList = value; }
		}

        /// <summary>
        ///     Get or set the total number of optimizations done on this script.
        /// </summary>
        public int TotalOptimizationCount
        {
            get { return _totalOptimizationCount; }
            set { _totalOptimizationCount = value; }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Optimizes the given byte code instruction list.
		/// </summary>
		/// <param name="instructionList">Instruction list to optimize.</param>
		/// <param name="symbolList">List of symbols referenced by the instruction list.</param>
		public void Optimize(ArrayList instructionList, ArrayList symbolList)
		{
			// Store and reset variables for future use.
			_instructionList = instructionList;
			_symbolList = symbolList;
            _totalOptimizationCount = 0;

			// Go through byte code instruction list in until it has been
			// optimized to the best it can be.
			while (true)
			{
				_currentInstruction = null;
				_instructionIndex = 0;
				_optimizationCount = 0;

				while (!EndOfInstructionStream())
					OptimizeInstruction();

                _totalOptimizationCount += _optimizationCount;

				if (_optimizationCount == 0)
					break;
			}

            // Remove all NOP's.

		}

		/// <summary>
		///		Returns true if all instructions have been read from the instruction list.
		/// </summary>
		/// <returns>True if all instructions have been read from the instruction list.</returns>
		private bool EndOfInstructionStream()
		{
			if (_instructionIndex >= _instructionList.Count) return true;
			return false;
		}

		/// <summary>
		///		Returns the next instruction in the list and advances the stream.
		/// </summary>
		/// <returns>Next instruction in the list.</returns>
		private Instruction NextInstruction()
		{
			if (EndOfInstructionStream() == true) return null;
			_currentInstruction = (Instruction)_instructionList[_instructionIndex];
			_instructionIndex++;
			return _currentInstruction;
		}

		/// <summary>
		///		Returns the next instruction in the list without advancing the stream.
		/// </summary>
		/// <returns>Next instruction in the list.</returns>
		private Instruction LookAheadInstruction(int count)
		{
			if (EndOfInstructionStream() == true) return null;
			if (_instructionIndex + (count - 1) > _instructionList.Count - 1) return null;
			return (Instruction)_instructionList[_instructionIndex + (count - 1)];
		}
		private Instruction LookAheadInstruction()
		{
			return LookAheadInstruction(1);
		}

		/// <summary>
		///		Optimizes the next instructions in the stream.
		/// </summary>
		private void OptimizeInstruction()
		{
			switch (NextInstruction().OpCode)
            {
                // Remove unneccessary MOV operations.
                // mov a, b; mov x, a
                // mov a, 1; mov x, a
                #region MOV Optimization
                    /*
                case OpCode.MOV_BOOL:
                case OpCode.MOV_BYTE:
                case OpCode.MOV_DOUBLE:
                case OpCode.MOV_FLOAT:
                case OpCode.MOV_INT:
                case OpCode.MOV_LONG:
                case OpCode.MOV_MEMORY_INDEX:
                case OpCode.MOV_OBJECT:
                case OpCode.MOV_SHORT:
                case OpCode.MOV_STRING:
                    {
                        Instruction LAI = LookAheadInstruction();

                        System.Console.WriteLine("Found mov..." + _currentInstruction.Decompile()+";   "+ LAI.Decompile());
                        if (_currentInstruction[0] == LAI[1] &&
                            ((LAI.OpCode == OpCode.MOV_BOOL && _currentInstruction.OpCode == OpCode.MOV_BOOL) ||
                             (LAI.OpCode == OpCode.MOV_BYTE && _currentInstruction.OpCode == OpCode.MOV_BYTE) ||
                             (LAI.OpCode == OpCode.MOV_DOUBLE && _currentInstruction.OpCode == OpCode.MOV_DOUBLE) ||
                             (LAI.OpCode == OpCode.MOV_FLOAT && _currentInstruction.OpCode == OpCode.MOV_FLOAT) ||
                             (LAI.OpCode == OpCode.MOV_INT && _currentInstruction.OpCode == OpCode.MOV_INT) ||
                             (LAI.OpCode == OpCode.MOV_LONG && _currentInstruction.OpCode == OpCode.MOV_LONG) ||
                             (LAI.OpCode == OpCode.MOV_MEMORY_INDEX && _currentInstruction.OpCode == OpCode.MOV_MEMORY_INDEX) ||
                             (LAI.OpCode == OpCode.MOV_OBJECT && _currentInstruction.OpCode == OpCode.MOV_OBJECT) ||
                             (LAI.OpCode == OpCode.MOV_SHORT && _currentInstruction.OpCode == OpCode.MOV_SHORT) ||
                             (LAI.OpCode == OpCode.MOV_STRING && _currentInstruction.OpCode == OpCode.MOV_STRING)
                             ))
                        {
                            System.Console.WriteLine(_currentInstruction.OpCode+","+LAI.OpCode);
                            _currentInstruction[0] = LAI[0];
                            LAI.OpCode = OpCode.NOP;

                            NextInstruction();
                            _optimizationCount++;
                        }
                        break;
                    }
                    */
                
                #endregion

                // Remove unneccessary PUSH/POP operations.
                // push a; pop b;
                // push a; pop_destroy;
                // push_null; pop_null b;
                #region PUSH, POP Optimization
                    
                case OpCode.PUSH_BOOL:
                case OpCode.PUSH_BYTE:
                case OpCode.PUSH_DOUBLE:
                case OpCode.PUSH_FLOAT:
                case OpCode.PUSH_INT:
                case OpCode.PUSH_LONG:
                case OpCode.PUSH_MEMORY_INDEX:
                case OpCode.PUSH_NULL:
                case OpCode.PUSH_OBJECT:
                case OpCode.PUSH_SHORT:
                case OpCode.PUSH_STRING:
                    {
                        Instruction LAI = LookAheadInstruction();
                        if ((LAI.OpCode == OpCode.POP_BOOL && _currentInstruction.OpCode == OpCode.PUSH_BOOL) ||
                            (LAI.OpCode == OpCode.POP_BYTE && _currentInstruction.OpCode == OpCode.PUSH_BYTE) ||
                            (LAI.OpCode == OpCode.POP_DOUBLE && _currentInstruction.OpCode == OpCode.PUSH_DOUBLE) ||
                            (LAI.OpCode == OpCode.POP_FLOAT && _currentInstruction.OpCode == OpCode.PUSH_FLOAT) ||
                            (LAI.OpCode == OpCode.POP_INT && _currentInstruction.OpCode == OpCode.PUSH_INT) ||
                            (LAI.OpCode == OpCode.POP_LONG && _currentInstruction.OpCode == OpCode.PUSH_LONG) ||
                            (LAI.OpCode == OpCode.POP_MEMORY_INDEX && _currentInstruction.OpCode == OpCode.PUSH_MEMORY_INDEX) ||
                            (LAI.OpCode == OpCode.POP_NULL && _currentInstruction.OpCode == OpCode.PUSH_NULL) ||
                            (LAI.OpCode == OpCode.POP_OBJECT && _currentInstruction.OpCode == OpCode.PUSH_OBJECT) ||
                            (LAI.OpCode == OpCode.POP_SHORT && _currentInstruction.OpCode == OpCode.PUSH_SHORT) ||
                            (LAI.OpCode == OpCode.POP_STRING && _currentInstruction.OpCode == OpCode.PUSH_STRING) ||
                            LAI.OpCode == OpCode.POP_DESTROY)
                        {
                            if (LAI.OpCode == OpCode.POP_DESTROY)
                            {
                                // WTF IS THE POINT OF THIS?
                                _currentInstruction.OpCode = OpCode.NOP;
                                LAI.OpCode = OpCode.NOP;
                            }
                            else if (_currentInstruction.OpCode == OpCode.PUSH_NULL)
                            {
                                _currentInstruction.OpCode = OpCode.MOV_NULL;
                                _currentInstruction[0] = LAI[0];
                                LAI.OpCode = OpCode.NOP;
                            }
                            else
                            {
                                switch (_currentInstruction.OpCode)
                                {
                                    case OpCode.PUSH_BOOL: _currentInstruction.OpCode = OpCode.MOV_BOOL; break;
                                    case OpCode.PUSH_BYTE: _currentInstruction.OpCode = OpCode.MOV_BYTE; break;
                                    case OpCode.PUSH_DOUBLE: _currentInstruction.OpCode = OpCode.MOV_DOUBLE; break;
                                    case OpCode.PUSH_FLOAT: _currentInstruction.OpCode = OpCode.MOV_FLOAT; break;
                                    case OpCode.PUSH_INT: _currentInstruction.OpCode = OpCode.MOV_INT; break;
                                    case OpCode.PUSH_LONG: _currentInstruction.OpCode = OpCode.MOV_LONG; break;
                                    case OpCode.PUSH_MEMORY_INDEX: _currentInstruction.OpCode = OpCode.MOV_MEMORY_INDEX; break;
                                    case OpCode.PUSH_OBJECT: _currentInstruction.OpCode = OpCode.MOV_OBJECT; break;
                                    case OpCode.PUSH_SHORT: _currentInstruction.OpCode = OpCode.MOV_SHORT; break;
                                    case OpCode.PUSH_STRING: _currentInstruction.OpCode = OpCode.MOV_STRING; break;
                                }
                                _currentInstruction[1] = _currentInstruction[0];
                                _currentInstruction[0] = LAI[0];
                                LAI.OpCode = OpCode.NOP;
                            }

                            NextInstruction();
                            _optimizationCount++;
                        }
                    }
                    break;
                    
				#endregion
			}
		}

		/// <summary>
		///		Removes the given amount of instructions starting at the given index.
		/// </summary>
		/// <param name="index">Index of instruction.</param>
		/// <param name="count">Amount of instructions removed.</param>
		private void RemoveInstructions(int index, int count)
		{
			// Update the instruction index's of the instruction index operands.
			foreach (Instruction instruction in _instructionList)
				for (int i = 0; i < instruction.OperandCount; i++)
				{
					Operand operand = instruction[i];
                    if (operand.OpType == OperandType.InstrIndex && operand.InstrIndex > index)
                        operand.InstrIndex -= count;
				}

			// Update the entry points of the functions.
            foreach (Symbol symbol in _symbolList)
                if (symbol.Type == SymbolType.Function)
                {
                    FunctionSymbol functionSymbol = (FunctionSymbol)symbol;
                    if (functionSymbol.EntryPoint > index && functionSymbol.IsImport == false)
                        functionSymbol.EntryPoint -= count;
                }
                else if (symbol.Type == SymbolType.JumpTarget)
                {
                    JumpTargetSymbol jumpSymbol = (JumpTargetSymbol)symbol;
                    if (jumpSymbol.InstrIndex > index)
                        jumpSymbol.InstrIndex -= count;
                }

            // Remove instructions from instruction list.
            for (int i = 0; i < count; i++)
                _instructionList.RemoveAt(index);

            _instructionIndex -= (count - 1);
		}

		/// <summary>
		///		Inserts the given instruction at the given index.
		/// </summary>
		/// <param name="index">Index to insert instruction at.</param>
		/// <param name="insertInstruction">Instruction to insert.</param>
		private void InsertInstruction(int index, Instruction insertInstruction)
		{
			// Update the instruction index's of the instruction index operands.
			foreach (Instruction instruction in _instructionList)
				for (int i = 0; i < instruction.OperandCount; i++)
				{
					Operand operand = instruction[i];
					if (operand.OpType == OperandType.InstrIndex && operand.InstrIndex > index)
						operand.InstrIndex++;
				}

			// Update the entry points of the functions.
			foreach (Symbol symbol in _symbolList)
				if (symbol.Type == SymbolType.Function)
				{
					FunctionSymbol functionSymbol = (FunctionSymbol)symbol;
					if (functionSymbol.EntryPoint > index && functionSymbol.IsImport == false)
						functionSymbol.EntryPoint++;
				}

            _instructionList.Insert(index, insertInstruction);
            _instructionIndex += 1;
		}

		#endregion
	}

}