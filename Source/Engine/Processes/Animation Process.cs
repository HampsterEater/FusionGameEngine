/* 
 * File: Animation Process.cs
 *
 * This source file contains the declaration of the AnimationProcess class which 
 * can be attached to game entitys to automate the animation of their image frames.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Collections;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Engine.Entitys;
using BinaryPhoenix.Fusion.Runtime.Processes;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Engine.Processes
{

	/// <summary>
	///		Describes how a AnimationProcess instance will animate its entity.
	/// </summary>
	public enum AnimationMode
	{
		Loop,
		Random,
		PingPong
	}

	/// <summary>
	///		Used to automate the animation of an game entitys image frames.
	/// </summary>
	public sealed class AnimationProcess : Process
	{
		#region Members
		#region Variables

		private EntityNode _entity = null;

		private int[] _frames = null;
		private int _frameIndex = 0;
		private int _frameDelay = 0;
        private int _lastFrameTime = 0;

		private AnimationMode _mode = AnimationMode.Loop;
		private int _loopLimit = 0;

		private int _pingPongDirection = 1;
		private int _pingPongTracker = 0;

		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the entity that this process is attached to.
		/// </summary>
		public EntityNode Entity
		{
			get { return _entity; }
			set { _entity = value; }
		}

		/// <summary>
		///		Gets or sets the mode describing how this process should animation
		///		the entity it is associated with.
		/// </summary>
		public AnimationMode Mode
		{
			get { return _mode; }
			set { _mode = value; }
		}

		/// <summary>
		///		Gets the delay in milliseconds between each frame of this animation.
		/// </summary>
		public int FrameDelay
		{
			get { return _frameDelay; }
			set { _frameDelay = value; }
		}

		/// <summary>
		///		Gets or sets the frames in the current animation.
		/// </summary>
		public int[] Frames
		{
			get { return _frames; }
			set { _frames = value; }
		}

		/// <summary>
		///		Gets or sets the amount of loops this process is allowed to complete before 
		///		having to finish.
		/// </summary>
		public int LoopLimit
		{
			get { return _loopLimit; }
			set { _loopLimit = value; }
		}

        /// <summary>
        ///     Gets if this process should be disposed of when the map changes.
        /// </summary>
        protected override bool DisposeOnMapChange
        {
            get { return (_entity == null || _entity.IsPersistent == false); }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Updates the animation of the entity associated with this process.
		/// </summary>
		/// <param name="deltaTime">Elapsed time since last frame.</param>
		public override void Run(float deltaTime)
		{
			if (_entity.IsEnabled == false || _frames.Length <= 0) return;
            if (Engine.GlobalInstance.FrameStartTime - _lastFrameTime >= _frameDelay)
			{
				switch (_mode)
				{
					case AnimationMode.Loop:
						_entity.Frame = _frames[(_frameIndex++) % _frames.Length];
						if (_frameIndex / _frames.Length >= _loopLimit && _loopLimit != 0) Finish(ProcessResult.Success);
						break;

					case AnimationMode.PingPong:
						_frameIndex += _pingPongDirection;
						if (_frameIndex >= _frames.Length - 1)
						{
							_pingPongTracker++;
							_pingPongDirection = -1;
						}
						else if (_frameIndex <= 0)
						{
							_pingPongTracker++;
							_pingPongDirection = 1;
						}
						_entity.Frame = _frames[_frameIndex];
						if (_pingPongTracker / 2 >= _loopLimit && _loopLimit != 0) Finish(ProcessResult.Success);
						break;

					case AnimationMode.Random:
						Random random = new Random(Environment.TickCount);
						_entity.Frame = _frames[random.Next() % _frames.Length];
						if (_loopLimit-- <= 0 && _loopLimit != 0) Finish(ProcessResult.Success);
						break;
				}

                _lastFrameTime = Engine.GlobalInstance.FrameStartTime;
			}
		}

        /// <summary>
        ///     Invoked when we are attached to the manager. If we are not waiting for another process
        ///     then we will setup the first frame.
        /// </summary>
        public override void Attached()
        {
            if (_waitForProcess == null)
            {
                Run(1.0f);

                /*   bool isAny = false;
                   for (int i = 0; i < _frames.Length; i++)
                   {
                       if (_entity.Frame == _frames[i])
                       {
                           _frameIndex = i;
                           isAny = true;
                           break;
                       }
                   }
                   if (isAny == false)
                        _entity.Frame = _frames[0];
               */
            }
        }

		/// <summary>
		///		Initializes a new instance of this class with the specified parameters.
		/// </summary>
		/// <param name="entity">Entity to associate this animation with.</param>
		/// <param name="mode">Mode of animation to animate entity with.</param>
		/// <param name="frameDelay">Delay between each animation frame.</param>
		/// <param name="startFrame">Starting frame index of animation.</param>
		/// <param name="endFrame">End frame index of animation.</param>
		/// <param name="loopLimit">Maximum amount of loops before this process has to finish. 0 is equal to infinite.</param>
		public AnimationProcess(EntityNode entity, AnimationMode mode, int frameDelay, int startFrame, int endFrame, int loopLimit)
		{
			_entity = entity;
			_mode = mode;
			_frameDelay = frameDelay;
			_loopLimit = loopLimit;

			_frames = new int[(endFrame - startFrame) + 1];
            for (int i = startFrame; i <= endFrame; i++)
                _frames[i - startFrame] = i;
        }
		public AnimationProcess(EntityNode entity, AnimationMode mode, int frameDelay, int startFrame, int endFrame)
		{
			_entity = entity;
			_mode = mode;
			_frameDelay = frameDelay;

            _frames = new int[(endFrame - startFrame) + 1];
            for (int i = startFrame; i <= endFrame; i++)
                _frames[i - startFrame] = i;
		}

		/// <summary>
		///		Initializes a new instance of this class with the specified parameters.
		/// </summary>
		/// <param name="entity">Entity to associate this animation with.</param>
		/// <param name="mode">Mode of animation to animate entity with.</param>
		/// <param name="frameDelay">Delay between each animation frame.</param>
		/// <param name="frames">Array of frame indexs to animate entity with.</param>
		/// <param name="loopLimit">Maximum amount of loops before this process has to finish. 0 is equal to infinite.</param>
		public AnimationProcess(EntityNode entity, AnimationMode mode, int frameDelay, int[] frames, int loopLimit)
		{
			_entity = entity;
			_mode = mode;
			_frameDelay = frameDelay;
			_loopLimit = loopLimit;
			_frames = frames;
		}
		public AnimationProcess(EntityNode entity, AnimationMode mode, int frameDelay, int[] frames)
		{
			_entity = entity;
			_mode = mode;
			_frameDelay = frameDelay;
			_frames = frames;
		}

		#endregion
	}

}
