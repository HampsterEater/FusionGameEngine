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

			// Go through byte code instruction list in until it has been
			// optimized to the best it can be.
			while (true)
			{
				_currentInstruction = null;
				_instructionIndex = 0;
				_optimizationCount = 0;

				while (!EndOfInstructionStream())
					OptimizeInstruction();

				if (_optimizationCount == 0)
					break;
			}

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
                // Add MOV optimizing
                // eg.
                //    MOV a, 1
                //    MOV b, a       
                //
                // CAN BE OPTIMIZED FURTHER BY IGNORING IRRELIVENT OPCODES
                // BETWEEN EACH OPTIMIZED INSTRUCTION.
                /*
				#region MOV Optimization
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
                        if ((LAI.OpCode == OpCode.MOV_BOOL ||
                            LAI.OpCode == OpCode.MOV_BYTE ||
                            LAI.OpCode == OpCode.MOV_DOUBLE ||
                            LAI.OpCode == OpCode.MOV_FLOAT ||
                            LAI.OpCode == OpCode.MOV_INT ||
                            LAI.OpCode == OpCode.MOV_LONG ||
                            LAI.OpCode == OpCode.MOV_MEMORY_INDEX ||
                            LAI.OpCode == OpCode.MOV_OBJECT ||
                            LAI.OpCode == OpCode.MOV_SHORT ||
                            LAI.OpCode == OpCode.MOV_STRING || 
                            LAI.OpCode == OpCode.MUL_INT)
                            && (_currentInstruction[0].Equals(LAI[1])))
                        {
                            // Grab the first mov instruction.
                            Instruction mov1Instruction = _currentInstruction;
                            int mov1Index = _instructionIndex - 1;

                            // Don't try and optimize direct stack access instructions.
                            // These just screw everything up. Not sure why though.
                            //if (pushInstruction.OpCode != OpCode.PUSH_NULL && pushInstruction[0].OpType == OperandType.DirectStack)
                            //    return;

                            // Grab the second mov instruction.
                            NextInstruction();
                            Instruction mov2Instruction = _currentInstruction;
                            int mov2Index = _instructionIndex - 1;

                            // Sort out the instructions operands.
                            mov2Instruction[1] = mov1Instruction.Operands[1];

                            // Remove the second instruction.
                            RemoveInstructions(mov1Index, 1); // Remove first instruction.
                            _optimizationCount++;
                        }
                    }
                    break;
                #endregion
                */

                // Remove unneeded PUSH, POP concurrent instructions.
                // eg.
                //    PUSH a
                //    POP a
                //    
                //    PUSH 1
                //    POP a
                //
                //    PUSH a
                //    POP_DESTROY
                //
                //    etc.
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
                        // Log the starting index.
                        Instruction pushInstruction = _currentInstruction;
                        int pushIndex = _instructionIndex - 1;

                        // Look ahead for our pop.
                        int stackDepth = 1;
                        while (stackDepth != 0)
                        {
                            NextInstruction();

                            // Check no jump targets point to this instruction, otherwise its to unsafe to optimize.
                            foreach (Symbol symbol in _symbolList)
                                if (symbol.Type == SymbolType.JumpTarget && ((JumpTargetSymbol)symbol).InstrIndex == _instructionIndex - 1)
                                    return;

                            // Jump? DAM, to unsafe to optimize this.
                            if (_currentInstruction.OpCode == OpCode.JMP ||
                                _currentInstruction.OpCode == OpCode.JMP_EQ ||
                                _currentInstruction.OpCode == OpCode.JMP_G ||
                                _currentInstruction.OpCode == OpCode.JMP_GE ||
                                _currentInstruction.OpCode == OpCode.JMP_L ||
                                _currentInstruction.OpCode == OpCode.JMP_LE ||
                                _currentInstruction.OpCode == OpCode.JMP_NE)
                            {
                                return;
                            }

                            // If its a push instruction then increase our stack depth.
                            else if (_currentInstruction.OpCode == OpCode.PUSH_BOOL ||
                                _currentInstruction.OpCode == OpCode.PUSH_BYTE ||
                                _currentInstruction.OpCode == OpCode.PUSH_DOUBLE ||
                                _currentInstruction.OpCode == OpCode.PUSH_FLOAT ||
                                _currentInstruction.OpCode == OpCode.PUSH_INT ||
                                _currentInstruction.OpCode == OpCode.PUSH_LONG ||
                                _currentInstruction.OpCode == OpCode.PUSH_MEMORY_INDEX ||
                                _currentInstruction.OpCode == OpCode.PUSH_NULL ||
                                _currentInstruction.OpCode == OpCode.PUSH_OBJECT ||
                                _currentInstruction.OpCode == OpCode.PUSH_SHORT ||
                                _currentInstruction.OpCode == OpCode.PUSH_STRING)
                            {
                                stackDepth++;
                            }

                            // If its a pop instruction then decrease our stack depth.
                            else if (_currentInstruction.OpCode == OpCode.POP_BOOL ||
                                 _currentInstruction.OpCode == OpCode.POP_BYTE ||
                                 _currentInstruction.OpCode == OpCode.POP_DOUBLE ||
                                 _currentInstruction.OpCode == OpCode.POP_FLOAT ||
                                 _currentInstruction.OpCode == OpCode.POP_INT ||
                                 _currentInstruction.OpCode == OpCode.POP_LONG ||
                                 _currentInstruction.OpCode == OpCode.POP_MEMORY_INDEX ||
                                 _currentInstruction.OpCode == OpCode.POP_NULL ||
                                 _currentInstruction.OpCode == OpCode.POP_OBJECT ||
                                 _currentInstruction.OpCode == OpCode.POP_SHORT ||
                                 _currentInstruction.OpCode == OpCode.POP_STRING ||
                                 _currentInstruction.OpCode == OpCode.POP_DESTROY)
                            {
                                stackDepth--;
                            }

                            // If its a logical instruction then it adds 1 to the stack.
                            else if (_currentInstruction.OpCode == OpCode.LOGICAL_AND ||
                                _currentInstruction.OpCode == OpCode.LOGICAL_NOT ||
                                _currentInstruction.OpCode == OpCode.LOGICAL_OR)
                            {
                                stackDepth++;
                            }

                            // If its an IS_? instruction then it adds 1 to the stack.
                            else if (_currentInstruction.OpCode == OpCode.IS_EQ ||
                                _currentInstruction.OpCode == OpCode.IS_G ||
                                _currentInstruction.OpCode == OpCode.IS_GE ||
                                _currentInstruction.OpCode == OpCode.IS_L ||
                                _currentInstruction.OpCode == OpCode.IS_LE ||
                                _currentInstruction.OpCode == OpCode.IS_NE)
                            {
                                stackDepth++;
                            }

                            // If its a calling instruction we can pop off its parameters :P.
                            else if (_currentInstruction.OpCode == OpCode.CALL)
                            {
                                stackDepth -= ((FunctionSymbol)_currentInstruction[0].SymbolIndexTracker).ParameterCount;
                            }
                            else if (_currentInstruction.OpCode == OpCode.CALL_METHOD)
                            {
                                stackDepth -= ((FunctionSymbol)_currentInstruction[1].SymbolIndexTracker).ParameterCount;
                            }
                        }

                        // Great we've found the ending pop!
                        Instruction popInstruction = _currentInstruction;
                        int popIndex = _instructionIndex - 1;

                        // Make sure its a pop.
                        if (!(_currentInstruction.OpCode == OpCode.POP_BOOL ||
                            _currentInstruction.OpCode == OpCode.POP_BYTE ||
                            _currentInstruction.OpCode == OpCode.POP_DOUBLE ||
                            _currentInstruction.OpCode == OpCode.POP_FLOAT ||
                            _currentInstruction.OpCode == OpCode.POP_INT ||
                            _currentInstruction.OpCode == OpCode.POP_LONG ||
                            _currentInstruction.OpCode == OpCode.POP_MEMORY_INDEX ||
                            _currentInstruction.OpCode == OpCode.POP_NULL ||
                            _currentInstruction.OpCode == OpCode.POP_OBJECT ||
                            _currentInstruction.OpCode == OpCode.POP_SHORT ||
                            _currentInstruction.OpCode == OpCode.POP_STRING ||
                            _currentInstruction.OpCode == OpCode.POP_DESTROY))
                        {
                            return;
                        }

                        // Its the same thing? What gives?
                        if (pushInstruction.OpCode != OpCode.PUSH_NULL && pushInstruction[0].Equals(popInstruction[0]))
                        {
                            RemoveInstructions(pushIndex, 1);
                            RemoveInstructions(popIndex - 1, 1); // Push has been removed so the pop instruction will now be -1. :S.
                            _optimizationCount++;
                            return;
                        }

                        // Check our destination is not being used between the push and the pop, otherwise
                        // its a no go!
                        bool destUsed = false;
                        for (int i = pushIndex + 1; i < popIndex; i++)
                        {
                            Instruction instruction = _instructionList[i] as Instruction;
                            for (int o = 0; o < instruction.OperandCount; o++)
                            {
                                if (instruction[o].Equals(popInstruction[0]))
                                {
                                    destUsed = true;
                                    goto exitLoop;
                                }
                            }
                        }
                    exitLoop:

                        // Can we optimize?
                        if (destUsed == false)
                        {
                            // Strip out the pop.
                            RemoveInstructions(popIndex, 1);

                            // Convert the push to a move.
                            switch (pushInstruction.OpCode)
                            {
                                case OpCode.PUSH_BOOL: pushInstruction.OpCode = OpCode.MOV_BOOL; break;
                                case OpCode.PUSH_BYTE: pushInstruction.OpCode = OpCode.MOV_BYTE; break;
                                case OpCode.PUSH_DOUBLE: pushInstruction.OpCode = OpCode.MOV_DOUBLE; break;
                                case OpCode.PUSH_FLOAT: pushInstruction.OpCode = OpCode.MOV_FLOAT; break;
                                case OpCode.PUSH_INT: pushInstruction.OpCode = OpCode.MOV_INT; break;
                                case OpCode.PUSH_LONG: pushInstruction.OpCode = OpCode.MOV_LONG; break;
                                case OpCode.PUSH_MEMORY_INDEX: pushInstruction.OpCode = OpCode.MOV_MEMORY_INDEX; break;
                                case OpCode.PUSH_NULL: pushInstruction.OpCode = OpCode.MOV_NULL; break;
                                case OpCode.PUSH_OBJECT: pushInstruction.OpCode = OpCode.MOV_OBJECT; break;
                                case OpCode.PUSH_SHORT: pushInstruction.OpCode = OpCode.MOV_SHORT; break;
                                case OpCode.PUSH_STRING: pushInstruction.OpCode = OpCode.MOV_STRING; break;
                            }

                            Operand valueOperand = pushInstruction.OpCode == OpCode.MOV_NULL ? null : pushInstruction[0];
                            pushInstruction[0] = popInstruction[0];

                            if (pushInstruction.OpCode != OpCode.MOV_NULL)
                                pushInstruction[1] = valueOperand;
                            else
                                pushInstruction.OperandCount = 1;

                            _optimizationCount++;
                        }
                        
                        // Jump back to the next instruction after the push :P
                        _instructionIndex = pushIndex;
                        NextInstruction();
                        
                        break;
                    }
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