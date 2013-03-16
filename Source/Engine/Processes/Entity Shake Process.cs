/* 
 * File: Entity Shake Process.cs
 *
 * This source file contains the declaration of the EntityShakeProcess class which 
 * can be attached to an entity (specifically a camera) to make them shake.
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
	///		Used to automate the shaking of an entity.
	/// </summary>
	public sealed class EntityShakeProcess : Process
	{
		#region Members
		#region Variables

		private EntityNode _entity = null;
		private float _shakeIntensity = 0.0f;

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
		///		Gets or sets the intensity to shake the cameras viewport.
		/// </summary>
		public float ShakeIntensity
		{
			get { return _shakeIntensity; }
			set { _shakeIntensity = value; }
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
		///		Updates the camera of the entity associated with this process.
		/// </summary>
		/// <param name="deltaTime">Elapsed time since last frame.</param>
		public override void Run(float deltaTime)
		{
			//if (_entity.IsEnabled == false) return;
            //if (_shakeIntensity <= 0)
           // {
            //    _shakeIntensity = 0;
            //    Finish(ProcessResult.Success);
            //}
            //else
            //    _shakeIntensity -= deltaTime;
            _entity.TransformationOffset = new Transformation(MathMethods.Random(-_shakeIntensity, _shakeIntensity),
                                                              MathMethods.Random(-_shakeIntensity, _shakeIntensity),
                                                              _entity.TransformationOffset.Z,
                                                              _entity.TransformationOffset.AngleX,
                                                              _entity.TransformationOffset.AngleY,
                                                              _entity.TransformationOffset.AngleZ,
                                                              _entity.TransformationOffset.ScaleX,
                                                              _entity.TransformationOffset.ScaleY,
                                                              _entity.TransformationOffset.ScaleZ);
        }

        /// <summary>
        ///     Overrides the dettached event. Just resets the transformatioh offset.
        /// </summary>
        public override void Dettached()
        {
            _entity.TransformationOffset = new Transformation();
        }

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        /// <param name="entity">Entity to shake.</param>
        /// <param name="intensity">Intensity to shake entity by.</param>
		public EntityShakeProcess(EntityNode entity, float intensity)
		{
            _entity = entity;
            _shakeIntensity = intensity;
		}

		#endregion
	}

}
