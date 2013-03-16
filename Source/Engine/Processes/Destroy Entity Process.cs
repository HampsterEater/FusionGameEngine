/* 
 * File: Entity Destroy Process.cs
 *
 * This source file contains the declaration of the EntityDestroyProcess class which 
 * can be attached to an entity to destroy them.
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
    ///		Used to automate the destruction of an entity.
    /// </summary>
    public sealed class EntityDestroyProcess : Process
    {
        #region Members
        #region Variables

        private SceneNode _entity = null;

        #endregion
        #region Properties

        /// <summary>
        ///		Gets or sets the entity that this process is attached to.
        /// </summary>
        public SceneNode Entity
        {
            get { return _entity; }
            set { _entity = value; }
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
            _entity.Dispose();
            Finish(ProcessResult.Success);
        }

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        /// <param name="entity">Entity to destroy.</param>
        public EntityDestroyProcess(SceneNode entity)
        {
            _entity = entity;
        }

        #endregion
    }

}
