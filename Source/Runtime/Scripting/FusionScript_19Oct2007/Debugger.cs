/* 
 * File: Debugger.cs
 *
 * This source file contains the declarations of any and all classes
 * used to debug scripts.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Runtime.Controls;

namespace BinaryPhoenix.Fusion.Runtime.Scripting
{

    /// <summary>
    ///     This class is used to show a window in which scripts can be debugged.
    /// </summary>
    public partial class Debugger : Form
    {
        #region Members
        #region Variables

        private ScriptThread _scriptThread = null;
        private bool _runScript = false;
        private bool _notifyOfEntry = false;
        private bool _notifyOfExit = false;
        private bool _notifyOfBreakPoint = false;
        private bool _allowInvokedFunctions = false;

        private string _sourceURL = "";

        private PropertyListViewCategory _globalCategory = null;
        private PropertyListViewCategory _localCategory = null;

        private FunctionSymbol _lastScope = null;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the script thread this debugger is debugging.
        /// </summary>
        public ScriptThread ScriptThread
        {
            get { return _scriptThread; }
            set { _scriptThread = value; }
        }

        /// <summary>
        ///     Gets if the script should run.
        /// </summary>
        public bool RunScript
        {
            get { return _runScript; }
        }

        /// <summary>
        ///     Gets if the script should notify this debugger of break points.
        /// </summary>
        public bool NotifyOnBreakPoint
        {
            get { return _notifyOfBreakPoint; }
        }

        /// <summary>
        ///     Gets if the script should notify this debugger of statement entry.
        /// </summary>
        public bool NotifyOnEnter
        {
            get { return _notifyOfEntry; }
        }

        /// <summary>
        ///     Gets if the script should notify this debugger of statement exit.
        /// </summary>
        public bool NotifyOnExit
        {
            get { return _notifyOfExit; }
        }

        /// <summary>
        ///     Gets if externally invoked functions are allowed.
        /// </summary>
        public bool AllowInvokedFunctions
        {
            get { return _allowInvokedFunctions; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Syncronizes all the data shown on the window with that of the script. 
        /// </summary>
        private void SyncronizeWindow()
        {
            // Clear everything.
            Text = "Script Debugger - " + _scriptThread.Process.Url;
            variablesPropertyListView.Clear();
            callStackListView.Items.Clear();
            stackTreeView.Nodes.Clear();
            heapTreeView.Nodes.Clear();
            objectsTreeView.Nodes.Clear();
            sourceCodeScriptTextBox.Enabled = true;
            disassemblyScriptTextBox.Enabled = true;
            variablesPropertyListView.Enabled = true;
            callStackListView.Enabled = true;
            startToolStripButton.Enabled = true;
            stopToolStripButton.Enabled = false;
            stepToolStripButton.Enabled = true;
            stackTreeView.Enabled = true;
            stackPropertyGrid.Enabled = true;
            heapTreeView.Enabled = true;
            heapPropertyGrid.Enabled = true;
            objectsTreeView.Enabled = true;
            objectsPropertyGrid.Enabled = true;

            // If we are running then show nothing.
            if (_runScript == true)
            {
                startToolStripButton.Enabled = false;
                stopToolStripButton.Enabled = true;
                stepToolStripButton.Enabled = false;
                sourceCodeScriptTextBox.Enabled = false;
                disassemblyScriptTextBox.Enabled = false;
                variablesPropertyListView.Enabled = false;
                callStackListView.Enabled = false;
                stackTreeView.Enabled = false;
                stackPropertyGrid.Enabled = false;
                heapTreeView.Enabled = false;
                heapPropertyGrid.Enabled = false;
                objectsTreeView.Enabled = false;
                objectsPropertyGrid.Enabled = false;

                return;
            }

            // Grab the current instruction.
            RuntimeInstruction currentInstruction = _scriptThread.Process.Instructions[_scriptThread.InstructionPointer];

            // See if we can find the source code for this instruction.
            if (currentInstruction.File != "")
            {
                if (File.Exists(currentInstruction.File))
                {
                    // Load in source code.
                    if (_sourceURL == "" || _sourceURL != currentInstruction.File)
                    {
                        string text = File.ReadAllText(currentInstruction.File);
                        sourceCodeScriptTextBox.Text = text;
                        _sourceURL = currentInstruction.File;

                        // Highlight.
                        sourceCodeScriptTextBox.Highlight(true);
                    }

                    // Remove any background colors.
                    sourceCodeScriptTextBox.RichTextBox.SelectionStart = 0;
                    sourceCodeScriptTextBox.RichTextBox.SelectionLength = sourceCodeScriptTextBox.RichTextBox.Text.Length;
                    sourceCodeScriptTextBox.RichTextBox.SelectionBackColor = Color.White;

                    // Scroll to the correct line.
                    sourceCodeScriptTextBox.Focus();
                    sourceCodeScriptTextBox.RichTextBox.SelectionStart = sourceCodeScriptTextBox.RichTextBox.GetFirstCharIndexFromLine(currentInstruction.Line - 1);// +currentInstruction.Offset;
                    int nextLineIndex = sourceCodeScriptTextBox.RichTextBox.Text.IndexOf('\n', sourceCodeScriptTextBox.RichTextBox.SelectionStart);
                    sourceCodeScriptTextBox.RichTextBox.SelectionLength = nextLineIndex == -1 ? sourceCodeScriptTextBox.Text.Length - sourceCodeScriptTextBox.RichTextBox.SelectionStart : nextLineIndex - sourceCodeScriptTextBox.RichTextBox.SelectionStart;
                    sourceCodeScriptTextBox.RichTextBox.SelectionBackColor = Color.Red;
                    sourceCodeScriptTextBox.RichTextBox.ScrollToCaret();
                    sourceCodeScriptTextBox.RichTextBox.SelectionLength = 0;
                }
                else
                    sourceCodeScriptTextBox.Text = "";
            }

            // Build the disassembly text.
            if (disassemblyScriptTextBox.Text == "")
            {
                StringBuilder builder = new StringBuilder();
                foreach (RuntimeInstruction instruction in _scriptThread.Process.Instructions)
                    builder.Append(instruction.Decompile() + "\n");
                disassemblyScriptTextBox.Text = builder.ToString();
            }

            // Remove any background colors.
            disassemblyScriptTextBox.RichTextBox.SelectionStart = 0;
            disassemblyScriptTextBox.RichTextBox.SelectionLength = disassemblyScriptTextBox.RichTextBox.Text.Length;
            disassemblyScriptTextBox.RichTextBox.SelectionBackColor = Color.Transparent;

            // Scroll to the correct line.
            disassemblyScriptTextBox.Focus();
            disassemblyScriptTextBox.RichTextBox.SelectionStart = disassemblyScriptTextBox.RichTextBox.GetFirstCharIndexFromLine(_scriptThread.InstructionPointer);
            int disassemblyNextLineIndex = disassemblyScriptTextBox.RichTextBox.Text.IndexOf('\n', disassemblyScriptTextBox.RichTextBox.SelectionStart);
            disassemblyScriptTextBox.RichTextBox.SelectionLength = disassemblyNextLineIndex == -1 ? disassemblyScriptTextBox.Text.Length - disassemblyScriptTextBox.RichTextBox.SelectionStart : disassemblyNextLineIndex - disassemblyScriptTextBox.RichTextBox.SelectionStart;
            disassemblyScriptTextBox.RichTextBox.SelectionBackColor = Color.Red;
            disassemblyScriptTextBox.RichTextBox.ScrollToCaret();
            disassemblyScriptTextBox.RichTextBox.SelectionLength = 0;

            // Find the last last scope we were in thats not native :P.
            _lastScope = null;
            object[] callStack = _scriptThread.CallStack.ToArray();
            for (int i = 0; i < callStack.Length; i++)
            {
                callStackListView.Items.Add(new ListViewItem(new string[] { ((FunctionSymbol)callStack[i]).Identifier})); // FIX ENTRY POINT
                if (_lastScope == null && ((FunctionSymbol)callStack[i]).IsImport == false)
                {
                    _lastScope = ((FunctionSymbol)callStack[i]);
                    callStackListView.SelectedIndices.Add(callStackListView.Items.Count - 1);
                    callStackListView.Items[callStackListView.Items.Count - 1].BackColor = Color.Red;
                }
            }

            // If we have a valid scope then starting filling the locals list :P.
            #region Variables
            _localCategory = new PropertyListViewCategory("Locals");
            variablesPropertyListView.AddCategory(_localCategory);
            if (_lastScope != null)
            {
                foreach (Symbol symbol in _lastScope.Symbols)
                {
                    if (symbol.Type == SymbolType.Variable)
                    {
                        VariableSymbol variable = symbol as VariableSymbol;
                        if (variable.IsArray == true)
                        {
                            int arrayIndex = _scriptThread.GetArrayLocal(variable.Identifier); // Can be optimized.
                            int arrayLength = _scriptThread.GetArrayLength(arrayIndex);
                            object value = "";
                            Type valueType = null;

                            switch (variable.DataType.DataType)
                            {
                                case DataType.Bool:
                                    value = new bool[arrayLength];
                                    for (int i = 0; i < arrayLength; i++)
                                        ((bool[])value)[i] = _scriptThread.GetRuntimeValueArrayElement(arrayIndex, i).BooleanLiteral;
                                    valueType = typeof(bool[]);
                                    break;
                                case DataType.Byte:
                                    value = new byte[arrayLength];
                                    for (int i = 0; i < arrayLength; i++)
                                        ((byte[])value)[i] = _scriptThread.GetRuntimeValueArrayElement(arrayIndex, i).ByteLiteral;
                                    valueType = typeof(byte[]);
                                    break;
                                case DataType.Double:
                                    value = new double[arrayLength];
                                    for (int i = 0; i < arrayLength; i++)
                                        ((double[])value)[i] = _scriptThread.GetRuntimeValueArrayElement(arrayIndex, i).DoubleLiteral;
                                    valueType = typeof(double[]);
                                    break;
                                case DataType.Float:
                                    value = new float[arrayLength];
                                    for (int i = 0; i < arrayLength; i++)
                                        ((float[])value)[i] = _scriptThread.GetRuntimeValueArrayElement(arrayIndex, i).FloatLiteral;
                                    valueType = typeof(float[]);
                                    break;
                                case DataType.Int:
                                    value = new int[arrayLength];
                                    for (int i = 0; i < arrayLength; i++)
                                        ((int[])value)[i] = _scriptThread.GetRuntimeValueArrayElement(arrayIndex, i).IntegerLiteral;
                                    valueType = typeof(int[]);
                                    break;
                                case DataType.Long:
                                    value = new long[arrayLength];
                                    for (int i = 0; i < arrayLength; i++)
                                        ((long[])value)[i] = _scriptThread.GetRuntimeValueArrayElement(arrayIndex, i).LongLiteral;
                                    valueType = typeof(long[]);
                                    break;
                                case DataType.Object:
                                    value = new int[arrayLength];
                                    for (int i = 0; i < arrayLength; i++)
                                        ((int[])value)[i] = _scriptThread.GetRuntimeValueArrayElement(arrayIndex, i).ObjectIndex;
                                    valueType = typeof(int[]);
                                    break;
                                case DataType.Short:
                                    value = new short[arrayLength];
                                    for (int i = 0; i < arrayLength; i++)
                                        ((short[])value)[i] = _scriptThread.GetRuntimeValueArrayElement(arrayIndex, i).ShortLiteral;
                                    valueType = typeof(short[]);
                                    break;
                                case DataType.String:
                                    value = new string[arrayLength];
                                    for (int i = 0; i < arrayLength; i++)
                                        ((string[])value)[i] = _scriptThread.GetRuntimeValueArrayElement(arrayIndex, i).StringLiteral;
                                    valueType = typeof(string[]);
                                    break;
                            }

                            _localCategory.AddProperty(new PropertyListViewItem(variable.Identifier, value, value, "", valueType));
                        }
                        else
                        {
                            RuntimeValue variableValue = _scriptThread.GetRuntimeValueLocal(variable.Identifier); // Can be optimized.
                            object value = "";
                            Type valueType = null;
                            switch (variable.DataType.DataType)
                            {
                                case DataType.Bool: value = variableValue.BooleanLiteral; valueType = typeof(bool); break;
                                case DataType.Byte: value = variableValue.ByteLiteral; valueType = typeof(byte); break;
                                case DataType.Double: value = variableValue.DoubleLiteral; valueType = typeof(double); break;
                                case DataType.Float: value = variableValue.FloatLiteral; valueType = typeof(float); break;
                                case DataType.Int: value = variableValue.IntegerLiteral; valueType = typeof(int); break;
                                case DataType.Long: value = variableValue.LongLiteral; valueType = typeof(long); break;
                                case DataType.Object: value = variableValue.ObjectIndex; valueType = typeof(int); break;
                                case DataType.Short: value = variableValue.ShortLiteral; valueType = typeof(short); break;
                                case DataType.String: value = variableValue.StringLiteral; valueType = typeof(string); break;
                            }
                            _localCategory.AddProperty(new PropertyListViewItem(variable.Identifier, value,value, "", valueType));
                        }
                    }
                }
            }

            // Global list FTW!
           _globalCategory = new PropertyListViewCategory("Globals");
            variablesPropertyListView.AddCategory(_globalCategory);
            foreach (Symbol symbol in _scriptThread.Process.GlobalScope.Symbols)
            {
                if (symbol.Type == SymbolType.Variable)
                {
                    VariableSymbol variable = symbol as VariableSymbol;
                    if (variable.IsArray == true)
                    {
                        int arrayIndex = _scriptThread.GetArrayGlobal(variable.Identifier); // Can be optimized.
                        if (arrayIndex == -1)
                        {
                            _globalCategory.AddProperty(new PropertyListViewItem(variable.Identifier, null, null, "", typeof(string)));
                            continue;
                        }

                        int arrayLength = _scriptThread.GetArrayLength(arrayIndex);
                        object value = "";
                        Type valueType = null;

                        switch (variable.DataType.DataType)
                        {
                            case DataType.Bool:
                                value = new bool[arrayLength];
                                for (int i = 0; i < arrayLength; i++)
                                    ((bool[])value)[i] = _scriptThread.GetRuntimeValueArrayElement(arrayIndex, i).BooleanLiteral;
                                valueType = typeof(bool[]); 
                                break;
                            case DataType.Byte:
                                value = new byte[arrayLength];
                                for (int i = 0; i < arrayLength; i++)
                                    ((byte[])value)[i] = _scriptThread.GetRuntimeValueArrayElement(arrayIndex, i).ByteLiteral;
                                valueType = typeof(byte[]); 
                                break;
                            case DataType.Double:
                                value = new double[arrayLength];
                                for (int i = 0; i < arrayLength; i++)
                                    ((double[])value)[i] = _scriptThread.GetRuntimeValueArrayElement(arrayIndex, i).DoubleLiteral;
                                valueType = typeof(double[]); 
                                break;
                            case DataType.Float:
                                value = new float[arrayLength];
                                for (int i = 0; i < arrayLength; i++)
                                    ((float[])value)[i] = _scriptThread.GetRuntimeValueArrayElement(arrayIndex, i).FloatLiteral;
                                valueType = typeof(float[]); 
                                break;
                            case DataType.Int:
                                value = new int[arrayLength];
                                for (int i = 0; i < arrayLength; i++)
                                    ((int[])value)[i] = _scriptThread.GetRuntimeValueArrayElement(arrayIndex, i).IntegerLiteral;
                                valueType = typeof(int[]); 
                                break;
                            case DataType.Long:
                                value = new long[arrayLength];
                                for (int i = 0; i < arrayLength; i++)
                                    ((long[])value)[i] = _scriptThread.GetRuntimeValueArrayElement(arrayIndex, i).LongLiteral;
                                valueType = typeof(long[]); 
                                break;
                            case DataType.Object:
                                value = new int[arrayLength];
                                for (int i = 0; i < arrayLength; i++)
                                    ((int[])value)[i] = _scriptThread.GetRuntimeValueArrayElement(arrayIndex, i).ObjectIndex;
                                valueType = typeof(int[]); 
                                break;
                            case DataType.Short:
                                value = new short[arrayLength];
                                for (int i = 0; i < arrayLength; i++)
                                    ((short[])value)[i] = _scriptThread.GetRuntimeValueArrayElement(arrayIndex, i).ShortLiteral;
                                valueType = typeof(short[]); 
                                break;
                            case DataType.String:
                                value = new string[arrayLength];
                                for (int i = 0; i < arrayLength; i++)
                                    ((string[])value)[i] = _scriptThread.GetRuntimeValueArrayElement(arrayIndex, i).StringLiteral;
                                valueType = typeof(string[]); 
                                break;
                        }

                        _globalCategory.AddProperty(new PropertyListViewItem(variable.Identifier, value, value, "", valueType));
                    }
                    else
                    {
                        RuntimeValue variableValue = _scriptThread.GetRuntimeValueGlobal(variable.Identifier); // Can be optimized.
                        object value = "";
                        Type valueType = null;
                        switch (variable.DataType.DataType)
                        {
                            case DataType.Bool: value = variableValue.BooleanLiteral; valueType = typeof(bool); break;
                            case DataType.Byte: value = variableValue.ByteLiteral; valueType = typeof(byte); break;
                            case DataType.Double: value = variableValue.DoubleLiteral; valueType = typeof(double); break;
                            case DataType.Float: value = variableValue.FloatLiteral; valueType = typeof(float); break;
                            case DataType.Int: value = variableValue.IntegerLiteral; valueType = typeof(int); break;
                            case DataType.Long: value = variableValue.LongLiteral; valueType = typeof(long); break;
                            case DataType.Object: value = variableValue.ObjectIndex; valueType = typeof(int); break;
                            case DataType.Short: value = variableValue.ShortLiteral; valueType = typeof(short); break;
                            case DataType.String: value = variableValue.StringLiteral; valueType = typeof(string); break;
                        }
                        _globalCategory.AddProperty(new PropertyListViewItem(variable.Identifier, value, value, "", valueType));
                    }
                }
            }
            #endregion

            // Fill up stack.
            topIndexLabel.Text = "Top Index: " + _scriptThread.Stack.TopIndex;
            frameIndexLabel.Text = "Frame Index: " + _scriptThread.Stack.FrameIndex;
            int top = _scriptThread.Stack.TopIndex > _scriptThread.Stack.FrameIndex ? _scriptThread.Stack.TopIndex : _scriptThread.Stack.FrameIndex;
            for (int i = 0; i < top; i++)
            {
                RuntimeValue runtimeValue = _scriptThread.Stack.RawStack[i];
                string name = runtimeValue.ToString();
                stackTreeView.Nodes.Add(i + ": " + name, i + ": " + name, 0);
            }

            // Fill up the object stack.
            int index = 0;
            foreach (RuntimeObject obj in _scriptThread.Process.ObjectHeap)
            {
                if (obj != null)
                {
                    string name = obj.ToString();
                    if (name.LastIndexOf('.') > -1)
                        name = name.Substring(name.LastIndexOf('.') + 1);
                    objectsTreeView.Nodes.Add(index + ": " + name, index + ": " + name, 0);
                }
                index++;
            }

            // Fill up the heap.
            index = 0;
            foreach (RuntimeValue obj in _scriptThread.Process.MemoryHeap)
            {
                if (obj != null)
                {
                    string name = obj.ToString();
                    heapTreeView.Nodes.Add(index + ": " + name, index + ": " + name, 0);
                }
                index++;
            }

            // Refresh the variables list!
            variablesPropertyListView.Refresh();
        }

        /// <summary>
        ///     Called when the value of a variable is changed in the property list view.
        /// </summary>
        /// <param name="item">Property item that was changed</param>
        /// <param name="value">Newly changed value.</param>
        public void SetVariable(PropertyListViewItem item, object value)
        {
            // Is it a global variable?
            if (item.Category == _globalCategory)
            {
                foreach (Symbol symbol in _scriptThread.Process.GlobalScope.Symbols)
                    if (symbol.Type == SymbolType.Variable && symbol.Identifier == item.Name)
                    {
                        VariableSymbol variable = symbol as VariableSymbol;
                        if (variable.IsArray == true)
                        {
                            int index = _scriptThread.AllocateArray(variable.DataType.DataType, ((Array)value).Length);
                            switch (variable.DataType.DataType)
                            {
                                case DataType.Bool:
                                    for (int i = 0; i < ((bool[])value).Length; i++)
                                        _scriptThread.SetArrayElement(index, i, ((bool[])value)[i]);
                                    break;
                                case DataType.Byte:
                                    for (int i = 0; i < ((byte[])value).Length; i++)
                                        _scriptThread.SetArrayElement(index, i, ((byte[])value)[i]);
                                    break;
                                case DataType.Double:
                                    for (int i = 0; i < ((double[])value).Length; i++)
                                        _scriptThread.SetArrayElement(index, i, ((double[])value)[i]);
                                    break;
                                case DataType.Float:
                                    for (int i = 0; i < ((float[])value).Length; i++)
                                        _scriptThread.SetArrayElement(index, i, ((float[])value)[i]);
                                    break;
                                case DataType.Int:
                                    for (int i = 0; i < ((int[])value).Length; i++)
                                        _scriptThread.SetArrayElement(index, i, ((int[])value)[i]);
                                    break;
                                case DataType.Long:
                                    for (int i = 0; i < ((bool[])value).Length; i++)
                                        _scriptThread.SetArrayElement(index, i, ((long[])value)[i]);
                                    break;
                                case DataType.Object:
                                    for (int i = 0; i < ((int[])value).Length; i++)
                                        _scriptThread.SetArrayElement(index, i, ((int[])value)[i] >= _scriptThread.Process.ObjectHeap.Length ? null : _scriptThread.Process.ObjectHeap[((int[])value)[i]]);
                                    break;
                                case DataType.Short:
                                    for (int i = 0; i < ((short[])value).Length; i++)
                                        _scriptThread.SetArrayElement(index, i, ((short[])value)[i]);
                                    break;
                                case DataType.String:
                                    for (int i = 0; i < ((string[])value).Length; i++)
                                        _scriptThread.SetArrayElement(index, i, ((string[])value)[i]);
                                    break;
                            }
                            _scriptThread.SetGlobalArray(variable.Identifier, index);
                        }
                        else
                        {
                            switch (variable.DataType.DataType)
                            {
                                case DataType.Bool: _scriptThread.SetGlobalVariable(variable.Identifier, (bool)value); break;
                                case DataType.Byte: _scriptThread.SetGlobalVariable(variable.Identifier, (byte)value); break;
                                case DataType.Double: _scriptThread.SetGlobalVariable(variable.Identifier, (double)value); break;
                                case DataType.Float: _scriptThread.SetGlobalVariable(variable.Identifier, (float)value); break;
                                case DataType.Int: _scriptThread.SetGlobalVariable(variable.Identifier, (int)value); break;
                                case DataType.Long: _scriptThread.SetGlobalVariable(variable.Identifier, (long)value); break;
                                case DataType.Object:
                                    RuntimeValue variableValue = _scriptThread.GetRuntimeValueGlobal(variable.Identifier); // Can be optimized.
                                    _scriptThread.SetGlobalVariable(variable.Identifier, (int)value >= _scriptThread.Process.ObjectHeap.Length ? null : _scriptThread.Process.ObjectHeap[variableValue.ObjectIndex]);
                                    break;
                                case DataType.Short: _scriptThread.SetGlobalVariable(variable.Identifier, (short)value); break;
                                case DataType.String: _scriptThread.SetGlobalVariable(variable.Identifier, value as string); break;
                            }
                        }
                    }
            }

            // No? Is it a local then?
            else if (item.Category == _localCategory)
            {
                foreach (Symbol symbol in _lastScope.Symbols)
                    if (symbol.Type == SymbolType.Variable && symbol.Identifier == item.Name)
                    {
                        VariableSymbol variable = symbol as VariableSymbol;
                        if (variable.IsArray == true)
                        {
                            int index = _scriptThread.AllocateArray(variable.DataType.DataType, ((Array)value).Length);
                            switch (variable.DataType.DataType)
                            {
                                case DataType.Bool:
                                    for (int i = 0; i < ((bool[])value).Length; i++)
                                        _scriptThread.SetArrayElement(index, i, ((bool[])value)[i]);
                                    break;
                                case DataType.Byte:
                                    for (int i = 0; i < ((byte[])value).Length; i++)
                                        _scriptThread.SetArrayElement(index, i, ((byte[])value)[i]);
                                    break;
                                case DataType.Double:
                                    for (int i = 0; i < ((double[])value).Length; i++)
                                        _scriptThread.SetArrayElement(index, i, ((double[])value)[i]);
                                    break;
                                case DataType.Float:
                                    for (int i = 0; i < ((float[])value).Length; i++)
                                        _scriptThread.SetArrayElement(index, i, ((float[])value)[i]);
                                    break;
                                case DataType.Int:
                                    for (int i = 0; i < ((int[])value).Length; i++)
                                        _scriptThread.SetArrayElement(index, i, ((int[])value)[i]);
                                    break;
                                case DataType.Long:
                                    for (int i = 0; i < ((bool[])value).Length; i++)
                                        _scriptThread.SetArrayElement(index, i, ((long[])value)[i]);
                                    break;
                                case DataType.Object:
                                    for (int i = 0; i < ((int[])value).Length; i++)
                                        _scriptThread.SetArrayElement(index, i, ((int[])value)[i] >= _scriptThread.Process.ObjectHeap.Length ? null : _scriptThread.Process.ObjectHeap[((int[])value)[i]]);
                                    break;
                                case DataType.Short:
                                    for (int i = 0; i < ((short[])value).Length; i++)
                                        _scriptThread.SetArrayElement(index, i, ((short[])value)[i]);
                                    break;
                                case DataType.String:
                                    for (int i = 0; i < ((string[])value).Length; i++)
                                        _scriptThread.SetArrayElement(index, i, ((string[])value)[i]);
                                    break;
                            }
                            _scriptThread.SetLocalArray(variable.Identifier, index);
                        }
                        else
                        {
                            switch (variable.DataType.DataType)
                            {
                                case DataType.Bool: _scriptThread.SetLocalVariable(variable.Identifier, (bool)value); break;
                                case DataType.Byte: _scriptThread.SetLocalVariable(variable.Identifier, (byte)value); break;
                                case DataType.Double: _scriptThread.SetLocalVariable(variable.Identifier, (double)value); break;
                                case DataType.Float: _scriptThread.SetLocalVariable(variable.Identifier, (float)value); break;
                                case DataType.Int: _scriptThread.SetLocalVariable(variable.Identifier, (int)value); break;
                                case DataType.Long: _scriptThread.SetLocalVariable(variable.Identifier, (long)value); break;
                                case DataType.Object:
                                    RuntimeValue variableValue = _scriptThread.GetRuntimeValueLocal(variable.Identifier); // Can be optimized.
                                    _scriptThread.SetLocalVariable(variable.Identifier, (int)value >= _scriptThread.Process.ObjectHeap.Length ? null : _scriptThread.Process.ObjectHeap[(int)value]);
                                    break;
                                case DataType.Short: _scriptThread.SetLocalVariable(variable.Identifier, (short)value); break;
                                case DataType.String: _scriptThread.SetLocalVariable(variable.Identifier, value as string); break;
                            }
                        }
                    }
            }
        }

        /// <summary>
        ///     Invoked when the script comes across a breakpoint.
        /// </summary>
        public void NotifyOfBreakPoint()
        {
            _runScript = false;
            _notifyOfEntry = false;
            _notifyOfExit = false;
            SyncronizeWindow();
        }

        /// <summary>
        ///     Invoked when the statement wants to inform the debugger it has exited a statement.
        /// </summary>
        public void NotifyOfEntry()
        {
            _runScript = false;
            _notifyOfEntry = false;
            _notifyOfExit = false;
            SyncronizeWindow();
        }

        /// <summary>
        ///     Invoked when the script wants to inform the debugger it has exited a statement.
        /// </summary>
        public void NotifyOfExit()
        {

        }

        /// <summary>
        ///     Overrides the normal show method.
        /// </summary>
        public new void Show()
        {
           SyncronizeWindow();
            Visible = true;
            base.Show();
        }

        /// <summary>
        ///     Called when the debugger is closed. Mainly used to set the thread running again.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void Debugger_FormClosed(object sender, FormClosedEventArgs e)
        {
            _scriptThread.Debugger = null;
        }

        /// <summary>
        ///     Called when the start tool strip button is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void startToolStripButton_Click(object sender, EventArgs e)
        {
            _runScript = true;
            _notifyOfEntry = false;
            _notifyOfExit = false;
            _notifyOfBreakPoint = true;
            _allowInvokedFunctions = true;
            SyncronizeWindow();
        }

        /// <summary>
        ///     Called when the stop tool strip button is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void stopToolStripButton_Click(object sender, EventArgs e)
        {
            _runScript = false;
            _notifyOfEntry = false;
            _notifyOfExit = false;
            _notifyOfBreakPoint = false;
            _allowInvokedFunctions = false;
            SyncronizeWindow();
        }

        /// <summary>
        ///     Called when the step tool strip button is clicked.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void stepToolStripButton_Click(object sender, EventArgs e)
        {
            _runScript = true;
            _notifyOfEntry = true;
            _notifyOfExit = false;
            _notifyOfBreakPoint = true;
            _allowInvokedFunctions = true;
            SyncronizeWindow();
        }

        /// <summary>
        ///     Invoked when the user selects an object in the objects tree view.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void objectsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            int index = 0;
            foreach (RuntimeObject obj in _scriptThread.Process.ObjectHeap)
            {
                if (objectsTreeView.SelectedNode.Index == index)
                    objectsPropertyGrid.SelectedObject = obj;
                index++;
            }
        }

        /// <summary>
        ///     Invoked when the user selects an object in the heap tree view.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void heapTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            int index = 0;
            foreach (RuntimeValue obj in _scriptThread.Process.MemoryHeap)
            {
                if (heapTreeView.SelectedNode.Index == index)
                    heapPropertyGrid.SelectedObject = obj;
                index++;
            }
        }

        /// <summary>
        ///     Invoked when the user selects an object in the stack tree view.
        /// </summary>
        /// <param name="sender">Object that caused this event to be triggered.</param>
        /// <param name="e">Arguments explaining why this event occured.</param>
        private void stackTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            int top = _scriptThread.Stack.TopIndex > _scriptThread.Stack.FrameIndex ? _scriptThread.Stack.TopIndex : _scriptThread.Stack.FrameIndex;
            for (int i = 0; i < top; i++)
            {
                RuntimeValue runtimeValue = _scriptThread.Stack.RawStack[i];
                if (i == stackTreeView.SelectedNode.Index)
                    stackPropertyGrid.SelectedObject = runtimeValue;
            }
        }

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        public Debugger()
        {
            InitializeComponent();
            variablesPropertyListView.SetValueDelegate += new PropertyListViewSetValueDelegate(SetVariable);
        }

        #endregion
    }

}