/* 
 * File: Entity Move To Process.cs
 *
 * This source file contains the declaration of the EntityMoveToProcess class which 
 * can be attached to entities to make them move.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Engine.Entitys;
using BinaryPhoenix.Fusion.Runtime.Processes;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Engine.Processes
{

	/// <summary>
	///		Used to make an entity move..
	/// </summary>
	public sealed class EntityMoveToProcess : Process
	{
		#region Members
		#region Variables

		private EntityNode _entity = null;

        private float _originalDistance;
        private float _x, _y;

		private StartFinishF _followSpeed = new StartFinishF(1, 1);

        private Transformation _previousTransformation;
        private HighPreformanceTimer _previousTransformationTimer = new HighPreformanceTimer();
        private bool _gotPreviousTransformation = false;

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
		///		Gets or sets the speed at which the entity follows the other.
		/// </summary>
		public StartFinishF FollowSpeed
		{
			get { return _followSpeed; }
			set { _followSpeed = value; }
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
		///		Updates the following entity associated with this process.
		/// </summary>
		/// <param name="deltaTime">Elapsed time since last frame.</param>
		public override void Run(float deltaTime)
		{
			//if (_entity.IsEnabled == false) return;

            // Work out the central points.
            Transformation entityTransform = _entity.CalculateTransformation();

            // If its a camera then we need to invert the coordinates as it uses
            // slightly different ones from normal entities.
            if (_entity is CameraNode)
            {
                entityTransform.X = -entityTransform.X;
                entityTransform.Y = -entityTransform.Y;
            }

            // Are we at the correct place? If so why bother with the below code :P.
            if (entityTransform.X == _x && entityTransform.Y == _y)
            {
                Finish(ProcessResult.Success);
                return;
            }

            // If we can't move then finish.
            if (_gotPreviousTransformation == true)
            {
                if (_previousTransformation.X == entityTransform.X && _previousTransformation.Y == entityTransform.Y && _previousTransformation.Z == entityTransform.Z)
                {
                    if (_previousTransformationTimer.DurationMillisecond > 500) // Half a second and no movement O_o, oh shiz we must have something in out way.
                    {
                        Finish(ProcessResult.Failed);
                        return;
                    }
                }
                else
                {
                    _previousTransformationTimer.Restart();
                    _previousTransformation = entityTransform;
                }
            }
            else
            {
                _previousTransformation = entityTransform;
                _previousTransformationTimer.Restart();
                _gotPreviousTransformation = true;
            }

            float entityTransformCenterX = entityTransform.X + ((_entity.BoundingRectangle.Width / 2) * entityTransform.ScaleX);
            float entityTransformCenterY = entityTransform.Y + ((_entity.BoundingRectangle.Height / 2) * entityTransform.ScaleY);
            float vectorX = entityTransformCenterX - _x;
            float vectorY = entityTransformCenterY - _y;
            float distance = (float)Math.Sqrt(vectorX * vectorX + vectorY * vectorY);
            vectorX /= distance;
            vectorY /= distance;

            float followSpeed = _followSpeed.Start + (((_followSpeed.Finish - _followSpeed.Start) / _originalDistance) * (_originalDistance - distance));

            // Work out vector towards entity.
            float movementX = (vectorX * followSpeed) * deltaTime;
            float movementY = (vectorY * followSpeed) * deltaTime;
            _entity.Move(-movementX,-movementY, 0.0f);

            // Are we done yet?
            if (Math.Abs(Math.Round(distance)) <= Math.Max(1,_followSpeed.Finish) * 2)// || followSpeed == _followSpeed.Finish)
            {
                Finish(ProcessResult.Success);
                return;
            }
        }

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        /// <param name="entity">Entity that will be moving.</param>
        /// <param name="x">X position to move to.</param>
        /// <param name="y">Y position to move to.</param>
        /// <param name="speed">Speed at which entity moves.</param>
        public EntityMoveToProcess(EntityNode entity, float x, float y, StartFinishF speed)
		{
            _entity = entity;
            _followSpeed = speed;
            _x = x;
            _y = y;

            Transformation entityTransform = _entity.CalculateTransformation();
            float vectorX = entityTransform.X - _x;
            float vectorY = entityTransform.Y - _y;
            _originalDistance = (float)Math.Sqrt(vectorX * vectorX + vectorY * vectorY);
		}

		#endregion
	}

}
