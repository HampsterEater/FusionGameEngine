/* 
 * File: Process.cs
 *
 * This source file contains the declaration of the ProcessFunctionSet class which 
 * contains numerous native functions that are commonly used in scripts.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using BinaryPhoenix.Fusion.Engine.Processes;
using BinaryPhoenix.Fusion.Engine.Entitys;
using BinaryPhoenix.Fusion.Audio;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Debug;
using BinaryPhoenix.Fusion.Runtime.Scripting;
using BinaryPhoenix.Fusion.Runtime.Processes;

namespace BinaryPhoenix.Fusion.Engine.ScriptingFunctions
{

    /// <summary>
    ///     Wraps a process object used in a script.
    /// </summary>
    public class ProcessScriptObject : NativeObject
    {
        public ProcessScriptObject(Runtime.Processes.Process process)
        {
            _nativeObject = process;
        }

        public ProcessScriptObject() { }
    }

	/// <summary>
	///		Contains numerous native functions that are commonly used in scripts.
	///		None are commented as they are all fairly self-explanatory.
	/// </summary>
	public class ProcessFunctionSet : NativeFunctionSet
	{
		[NativeFunctionInfo("CreateAnimationProcess", "object", "object,int,int,int,int,int")]
		public void CreateAnimationProcessA(ScriptThread thread)
		{
			EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
			if (entity == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called CreateAnimationProcess with an invalid object.", LogAlertLevel.Error);
				return;
			}
			thread.SetReturnValue(new ProcessScriptObject(new AnimationProcess(entity, (AnimationMode)thread.GetIntegerParameter(1), thread.GetIntegerParameter(2), thread.GetIntegerParameter(3), thread.GetIntegerParameter(4), thread.GetIntegerParameter(5))));
		}

		[NativeFunctionInfo("CreateAnimationProcess", "object", "object,int,int,int,int")]
		public void CreateAnimationProcessB(ScriptThread thread)
		{
			EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
			if (entity == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called CreateAnimationProcess with an invalid object.", LogAlertLevel.Error);
				return;
			}
            thread.SetReturnValue(new ProcessScriptObject(new AnimationProcess(entity, (AnimationMode)thread.GetIntegerParameter(1), thread.GetIntegerParameter(2), thread.GetIntegerParameter(3), thread.GetIntegerParameter(4))));		
		}

		[NativeFunctionInfo("CreateAnimationProcess", "object", "object,int,int,int[],int")]
		public void CreateAnimationProcessC(ScriptThread thread)
		{
			EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
			int memoryIndex = thread.GetArrayParameter(3);
			if (entity == null || memoryIndex == 0)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called CreateAnimationProcess with an invalid object.", LogAlertLevel.Error);
				return;
			}
			int arrayLength = thread.GetArrayLength(memoryIndex);
			int[] frames = new int[arrayLength];
			for (int i = 0; i < arrayLength; i++)
				frames[i] = thread.GetIntArrayElement(memoryIndex, i);
            thread.SetReturnValue(new ProcessScriptObject(new AnimationProcess(entity, (AnimationMode)thread.GetIntegerParameter(1), thread.GetIntegerParameter(2), frames, thread.GetIntegerParameter(4))));
		}

		[NativeFunctionInfo("CreateAnimationProcess", "object", "object,int,int,int[]")]
		public void CreateAnimationProcessD(ScriptThread thread)
		{
			EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
			int memoryIndex = thread.GetArrayParameter(3);
			if (entity == null || memoryIndex == 0)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called CreateAnimationProcess with an invalid object.", LogAlertLevel.Error);
				return;
			}
			int arrayLength = thread.GetArrayLength(memoryIndex);
			int[] frames = new int[arrayLength];
			for (int i = 0; i < arrayLength; i++)
				frames[i] = thread.GetIntArrayElement(memoryIndex, i);
            thread.SetReturnValue(new ProcessScriptObject(new AnimationProcess(entity, (AnimationMode)thread.GetIntegerParameter(1), thread.GetIntegerParameter(2), frames)));	
		}

        [NativeFunctionInfo("CreateFollowingProcess", "object", "object,object,float,float")]
        public void CreateFollowingProcess(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            EntityNode targetEntity = ((NativeObject)thread.GetObjectParameter(1)).Object as EntityNode;
            if (entity == null || targetEntity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called CreateFollowingProcess with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(new ProcessScriptObject(new EntityFollowProcess(entity, targetEntity, thread.GetFloatParameter(2), thread.GetFloatParameter(3))));
        }

        [NativeFunctionInfo("CreateMoveToProcess", "object", "object,float,float,float,float")]
        public void CreateMoveToProcessA(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called CreateMoveToProcess with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(new ProcessScriptObject(new EntityMoveToProcess(entity, thread.GetFloatParameter(3), thread.GetFloatParameter(4), new StartFinishF(thread.GetFloatParameter(1), thread.GetFloatParameter(2)))));
        }

        [NativeFunctionInfo("CreateMoveToProcess", "object", "object,float,float,object")]
        public void CreateMoveToProcessB(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            EntityNode moveEntity = ((NativeObject)thread.GetObjectParameter(3)).Object as EntityNode;
            if (entity == null || moveEntity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called CreateMoveToProcess with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(new ProcessScriptObject(new EntityMoveToProcess(entity, moveEntity.Transformation.X + ((moveEntity.BoundingRectangle.Width / 2.0f) * moveEntity.Transformation.ScaleX), moveEntity.Transformation.Y + ((moveEntity.BoundingRectangle.Height / 2.0f) * moveEntity.Transformation.ScaleY), new StartFinishF(thread.GetFloatParameter(1), thread.GetFloatParameter(2)))));
        }

        [NativeFunctionInfo("CreateShakingProcess", "object", "object,float")]
        public void CreateShakingProcess(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called CreateShakingProcess with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(new ProcessScriptObject(new EntityShakeProcess(entity, thread.GetFloatParameter(1))));
        }

        [NativeFunctionInfo("CreateDestroyProcess", "object", "object")]
        public void CreateDestroyProcess(ScriptThread thread)
        {
            SceneNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as SceneNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called CreateDestroyProcess with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(new ProcessScriptObject(new EntityDestroyProcess(entity)));
        }

        [NativeFunctionInfo("CreateBoundryProcess", "object", "object,float,float,float,float")]
        public void CreateBoundryProcessA(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called CreateBoundryProcess with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(new ProcessScriptObject(new EntityBoundryProcess(entity, new RectangleF(thread.GetFloatParameter(1), thread.GetFloatParameter(2), thread.GetFloatParameter(3), thread.GetFloatParameter(4)))));
        }

        [NativeFunctionInfo("CreateBoundryProcess", "object", "object,object")]
        public void CreateBoundryProcessB(ScriptThread thread)
        {
            EntityNode entity1 = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            EntityNode entity2 = ((NativeObject)thread.GetObjectParameter(1)).Object as EntityNode;
            if (entity1 == null || entity2 == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called CreateBoundryProcess with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(new ProcessScriptObject(new EntityBoundryProcess(entity1, entity2)));
        }

		[NativeFunctionInfo("ActivateProcess", "void", "object")]
		public void ActivateProcess(ScriptThread thread)
		{
			Runtime.Processes.Process process = ((NativeObject)thread.GetObjectParameter(0)).Object as Runtime.Processes.Process;
			if (process == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called ActivateProcess with an invalid object.", LogAlertLevel.Error);
				return;
			}
			ProcessManager.AttachProcess(process);
		}

		[NativeFunctionInfo("DeactivateProcess", "void", "object")]
		public void DeactiveProcess(ScriptThread thread)
		{
			Runtime.Processes.Process process = ((NativeObject)thread.GetObjectParameter(0)).Object as Runtime.Processes.Process;
			if (process == null)
			{
				DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called DeactivateProcess with an invalid object.", LogAlertLevel.Error);
				return;
			}
			ProcessManager.DettachProcess(process);
		}

        [NativeFunctionInfo("DestroyProcess", "void", "object")]
        public void DestroyProcess(ScriptThread thread)
        {
            Runtime.Processes.Process process = ((NativeObject)thread.GetObjectParameter(0)).Object as Runtime.Processes.Process;
            if (process == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called DestroyProcess with an invalid object.", LogAlertLevel.Error);
                return;
            }
            process.Finish(ProcessResult.Failed);
        }

        [NativeFunctionInfo("CreateFadeProcess", "object", "object,int,int,int")]
        public void CreateFadeProcess(ScriptThread thread)
        {
            EntityNode entity = ((NativeObject)thread.GetObjectParameter(0)).Object as EntityNode;
            if (entity == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called CreateFadeProcess with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(new ProcessScriptObject(new EntityFadeProcess(entity, thread.GetIntegerParameter(1), thread.GetIntegerParameter(2), thread.GetIntegerParameter(3))));
        }

        [NativeFunctionInfo("CreateDelayProcess", "object", "int")]
        public void CreateDelayProcess(ScriptThread thread)
        {
            thread.SetReturnValue(new ProcessScriptObject(new DelayProcess(thread.GetIntegerParameter(0))));
        }

        [NativeFunctionInfo("MakeProcessWait", "void", "object,object,int")]
        public void MakeProcessWait(ScriptThread thread)
        {
            Runtime.Processes.Process process = ((NativeObject)thread.GetObjectParameter(0)).Object as Runtime.Processes.Process;
            Runtime.Processes.Process waitForProcess = ((NativeObject)thread.GetObjectParameter(1)).Object as Runtime.Processes.Process;
            if (process == null || waitForProcess == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called MakeProcessWait with an invalid object.", LogAlertLevel.Error);
                return;
            }
            process.WaitForProcess(waitForProcess, (ProcessResult)thread.GetIntegerParameter(2));
        }

        [NativeFunctionInfo("WaitForProcessToFinish", "void", "object")]
        public void WaitForProcessToFinish(ScriptThread thread)
        {
            Runtime.Processes.Process process = ((NativeObject)thread.GetObjectParameter(0)).Object as Runtime.Processes.Process;
            if (process == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called WaitForProcessToFinish with an invalid object.", LogAlertLevel.Error);
                return;
            }

            thread.IsWaiting = true; // Stop the thread.
       
            // Create a script that will start this thread again when the process
            // has finished.
            StartScriptProcess waitProcess = new StartScriptProcess(thread);
            waitProcess.WaitForProcess(process, ProcessResult.Success);
            ProcessManager.AttachProcess(waitProcess);
        }

        [NativeFunctionInfo("IsProcessFinished", "bool", "object")]
        public void IsProcessFinished(ScriptThread thread)
        {
            Runtime.Processes.Process process = ((NativeObject)thread.GetObjectParameter(0)).Object as Runtime.Processes.Process;
            if (process == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called IsProcessFinished with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(process.IsFinished);
        }

        [NativeFunctionInfo("CreateChannelFadeProcess", "object", "object,float,float,int")]
        public void CreateChannelFadeProcess(ScriptThread thread)
        {
            Audio.ISampleBuffer sound = ((NativeObject)thread.GetObjectParameter(0)).Object as ISampleBuffer;
            if (sound == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called CreateChannelFadeProcess with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(new ProcessScriptObject(new ChannelFadeProcess(sound, thread.GetFloatParameter(1), thread.GetFloatParameter(2), thread.GetIntegerParameter(3))));
        }

        [NativeFunctionInfo("CreatePathFollowProcess", "object","object,object")]
        public void CreatePathFollowProcess(ScriptThread thread)
        {
            PathMarkerNode node1 = ((NativeObject)thread.GetObjectParameter(0)).Object as PathMarkerNode;
            EntityNode node2 = ((NativeObject)thread.GetObjectParameter(1)).Object as EntityNode;
            if (node1 == null || node2 == null)
            {
                DebugLogger.WriteLog((thread.Process.Url != null && thread.Process.Url != "" ? thread.Process.Url : "A script") + " called CreatePathFollowProcess with an invalid object.", LogAlertLevel.Error);
                return;
            }
            thread.SetReturnValue(new ProcessScriptObject(new PathFollowProcess(node1, node2)));
        }
    }

}