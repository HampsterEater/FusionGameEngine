/* 
 * File: Entity Boundry Process.cs
 *
 * This source file contains the declaration of the EntityBoundryProcess class which 
 * can be attached to entities to stop them going outside a specific boundry rectangle.
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
    ///		Used to stop an entity going outside a specific boundry rectangle.
    /// </summary>
    public sealed class EntityBoundryProcess : Process
    {
        #region Members
        #region Variables

        private EntityNode _entity = null;
        private RectangleF _boundry = new RectangleF(0,0,0,0);
        private EntityNode _entityBoundry = null;

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
        ///		Gets or sets the entity used as the boundry.
        /// </summary>
        public EntityNode BoundryEntity
        {
            get { return _entityBoundry; }
            set { _entityBoundry = value; }
        }

        /// <summary>
        ///		Gets or sets the boundry this entity should be restricted to.
        /// </summary>
        public RectangleF Boundry
        {
            get { return _boundry; }
            set { _boundry = value; }
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

            Transformation entityTransform = _entity.CalculateTransformation();
            if (_entity is CameraNode)
            {
                entityTransform.X = -entityTransform.X;
                entityTransform.Y = -entityTransform.Y;
            }
            entityTransform.X = (int)entityTransform.X;
            entityTransform.Y = (int)entityTransform.Y;

            // Are we using a rectangle or entity as the boundry?
            Rectangle boundry = new Rectangle((int)_boundry.X, (int)_boundry.Y, (int)_boundry.Width, (int)_boundry.Height);
            if (_entityBoundry != null)
            {
                Transformation boundryTransform = _entityBoundry.CalculateTransformation();
                if (_entityBoundry is CameraNode)
                {
                    boundryTransform.X = -boundryTransform.X;
                    boundryTransform.Y = -boundryTransform.Y;
                }
                boundryTransform.X = (int)boundryTransform.X;
                boundryTransform.Y = (int)boundryTransform.Y;
                boundry = new Rectangle((int)boundryTransform.X, (int)boundryTransform.Y, (int)(_entityBoundry.Width * boundryTransform.ScaleX), (int)(_entityBoundry.Height * boundryTransform.ScaleY));
            }

            float x = entityTransform.X; // Cast to int to fix jittering bugs.
            float y = entityTransform.Y; // Cast to int to fix jittering bugs.

            if (x < boundry.Left) x = boundry.Left;
            if (y < boundry.Top) y = boundry.Top;
            if (x > boundry.Right - (_entity.BoundingRectangle.Width * entityTransform.ScaleX)) x = (int)(boundry.Right - (_entity.BoundingRectangle.Width * entityTransform.ScaleX));
            if (y > boundry.Bottom - (_entity.BoundingRectangle.Height * entityTransform.ScaleY)) y = (int)(boundry.Bottom - (_entity.BoundingRectangle.Height * entityTransform.ScaleY));

            if (!float.IsNaN(x) && !float.IsNaN(y))
                _entity.Position(x, y, _entity.Transformation.Z);
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
            }
        }

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        /// <param name="entity">Entity that will be following.</param>
        /// <param name="boundry">Boundry that entity should be limited to.</param>
        public EntityBoundryProcess(EntityNode entity, RectangleF boundry)
        {
            _entity = entity;
            _boundry = boundry;
            _priority = -1; // Try and do us last.
        }

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        /// <param name="entity">Entity that will be following.</param>
        /// <param name="boundry">Entity to be used as boundry.</param>
        public EntityBoundryProcess(EntityNode entity, EntityNode boundry)
        {
            _entity = entity;
            _entityBoundry = boundry;
            _priority = -1; // Try and do us last.
        }

        #endregion
    }

}
