/* 
 * File: Entity Fade Process.cs
 *
 * This source file contains the declaration of the EntityFadeProcess class which 
 * can be attached to game entities to force them to fade from a color to another
 * color.
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
	///		Used to force an entity to fade from a color to another color.
	/// </summary>
	public sealed class EntityFadeProcess : Process
	{
		#region Members
		#region Variables

        private EntityNode _entity = null;
        private HighPreformanceTimer _timer = new HighPreformanceTimer();
        private int _duration;
        private int _fromColorRed = 255, _fromColorGreen = 255, _fromColorBlue = 255, _fromColorAlpha = 255;
        private int _toColorRed = 255, _toColorGreen = 255, _toColorBlue = 255, _toColorAlpha = 0;
        
		#endregion
		#region Properties

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
           // if (_entity.IsEnabled == false)
           //     return;

            // Couple of useful variables.
            float expired = (int)_timer.DurationMillisecond;
            float duration = (float)_duration;
            if (expired > _duration)
            {
                _entity.Color = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, _toColorRed, _toColorGreen, _toColorBlue, _toColorAlpha);
                Finish(ProcessResult.Success);
                return;
            }

            // Set the color
            int r = (int)(_fromColorRed + (((_toColorRed - _fromColorRed) / duration) * expired));
            int g = (int)(_fromColorGreen + (((_toColorGreen - _fromColorGreen) / duration) * expired));
            int b = (int)(_fromColorBlue + (((_toColorBlue - _fromColorBlue) / duration) * expired));
            int a = (int)(_fromColorAlpha + (((_toColorAlpha - _fromColorAlpha) / duration) * expired));
           _entity.Color = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, r, g, b, a);
		}

        /// <summary>
        ///		Called when the process this process is waiting for finishes.
        /// </summary>
        public override void WaitFinished()
        {
            _timer.Restart();
        }

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        public EntityFadeProcess(EntityNode node, int fromColor, int toColor, int duration)
        {
            _entity = node;
            ColorMethods.SplitColor(ColorFormats.A8R8G8B8, fromColor, out _fromColorRed, out _fromColorGreen, out _fromColorBlue, out _fromColorAlpha);
            ColorMethods.SplitColor(ColorFormats.A8R8G8B8, toColor, out _toColorRed, out _toColorGreen, out _toColorBlue, out _toColorAlpha);
            _duration = duration;
            _timer.Restart();
        }

		#endregion
	}

}
