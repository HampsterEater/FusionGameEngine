/* 
 * File: Entity Follow Process.cs
 *
 * This source file contains the declaration of the EntityFollowProcess class which 
 * can be attached to entities (specifically cameras) to make them follow another entity.
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
	///		Used to automate the following of an entity.
	/// </summary>
	public sealed class EntityFollowProcess : Process
	{
		#region Members
		#region Variables

		private EntityNode _entity = null;
        private EntityNode _targetEntity = null;

		private float _followSpeed = 0.0f;
        private float _bufferZoneRadius = 0.0f;

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
		///		Gets or sets the entity that this process should follow.
		/// </summary>
		public EntityNode TargetEntity
		{
			get { return _targetEntity; }
			set { _targetEntity = value; }
		}

		/// <summary>
		///		Gets or sets the speed at which the entity follows the other.
		/// </summary>
		public float FollowSpeed
		{
			get { return _followSpeed; }
			set { _followSpeed = value; }
		}

		/// <summary>
		///		Gets or sets the buffer zone around the entity in which the following entity
        ///     will stay stationary.
		/// </summary>
		public float BufferZoneRadius
		{
			get { return _bufferZoneRadius; }
			set { _bufferZoneRadius = value; }
		}

        /// <summary>
        ///     Gets if this process should be disposed of when the map changes.
        /// </summary>
        protected override bool DisposeOnMapChange
        {
            get { return (_entity == null || _entity.IsPersistent == false) || (_targetEntity == null || _targetEntity.IsPersistent == false); }
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
  
            CameraNode camera = _entity as CameraNode;

            // Work out the central points.
            Transformation entityTransform = _entity.CalculateTransformation();
            Transformation targetTransform = _targetEntity.CalculateTransformation();

            // If its a camera then we need to invert the coordinates as it uses
            // slightly different ones from normal entities.
            if (camera != null)
            {
                entityTransform.X = -entityTransform.X;
                entityTransform.Y = -entityTransform.Y;
            }

            float entityTransformCenterX = entityTransform.X + ((_entity.BoundingRectangle.Width / 2) * entityTransform.ScaleX);
            float entityTransformCenterY = entityTransform.Y + ((_entity.BoundingRectangle.Height / 2) * entityTransform.ScaleY);
            float targetEntityTransformCenterX = targetTransform.X + ((_targetEntity.BoundingRectangle.Width / 2) * targetTransform.ScaleX);
            float targetEntityTransformCenterY = targetTransform.Y + ((_targetEntity.BoundingRectangle.Height / 2) * targetTransform.ScaleY);
            float vectorX = entityTransformCenterX - targetEntityTransformCenterX;
            float vectorY = entityTransformCenterY - targetEntityTransformCenterY;
            float distance = (float)Math.Sqrt(vectorX * vectorX + vectorY * vectorY);
            vectorX /= distance;
            vectorY /= distance;

            // Are we set to instantly snap to the target?
            if (_bufferZoneRadius == 0.0f || _followSpeed == 0.0f)
            {
                _entity.Position((float)Math.Floor(targetEntityTransformCenterX - ((_entity.BoundingRectangle.Width / 2) * entityTransform.ScaleX)), (float)Math.Floor(targetEntityTransformCenterY - ((_entity.BoundingRectangle.Width / 2) * entityTransform.ScaleX)), targetTransform.Z);
                return;
            }

            // Are we at the correct place? If so why bother with the below code :P.
            if (entityTransformCenterX == targetEntityTransformCenterX && entityTransformCenterY == targetEntityTransformCenterY)
                return;

            // Are we in the stationary buffer zone?
            if (distance <= _bufferZoneRadius)
                return; // Yep.

            distance = (int)distance;

            // Work out vector towards entity.
            float scaleAmount = (distance / _bufferZoneRadius);
            //if (distance > _bufferZoneRadius * 2) scaleAmount *= 2;

            float movementX = (float)Math.Round(vectorX * (_followSpeed * scaleAmount), 1);// *deltaTime;
            float movementY = (float)Math.Round(vectorY * (_followSpeed * scaleAmount), 1);// *deltaTime;
            movementX = (int)movementX;
            movementY = (int)movementY;
            
            if (!float.IsNaN(movementX) && !float.IsNaN(movementY))     
                _entity.Move(-movementX,-movementY, 0.0f);
		}

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        /// <param name="entity">Entity that will be following.</param>
        /// <param name="targetEntity">Entity that be followed.</param>
        /// <param name="bufferRadius">Buffer radius around target in which no movement is produced.</param>
        /// <param name="speed">Speed at which entity moves.</param>
        public EntityFollowProcess(EntityNode entity, EntityNode targetEntity, float bufferRadius, float speed)
		{
            _priority = 2; // Right at the start, before things start to move :P.
            _entity = entity;
            _targetEntity = targetEntity;
            _bufferZoneRadius = bufferRadius;
            _followSpeed = speed;
		}

		#endregion
	}

}
