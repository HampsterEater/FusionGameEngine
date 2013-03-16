/* 
 * File: Path Follow Process.cs
 *
 * This source file contains the declaration of the PathFollowProcess class which 
 * can be attached to entities (specifically cameras) to make them follow a path.
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
    ///		Used to automate the following of an path.
    /// </summary>
    public sealed class PathFollowProcess : Process
    {
        #region Members
        #region Variables

        private EntityNode _entity = null;
        private PathMarkerNode _startNode = null;
        private PathMarkerNode _currentNode = null;
        private PathMarkerNode _previousNode = null;

        private EntityMoveToProcess _moveToProcess = null;
        private DelayProcess _delayProcess = null;

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
        ///		Gets or sets the starting node of the path.
        /// </summary>
        public PathMarkerNode StartPathNode
        {
            get { return _startNode; }
            set { _startNode = value; }
        }

        /// <summary>
        ///		Gets or sets the current node of the path.
        /// </summary>
        public PathMarkerNode CurrentPathNode
        {
            get { return _currentNode; }
            set { _currentNode = value; }
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
            if (_entity.IsEnabled == false) return;
            AdvanceOneNode();
        }

        /// <summary>
        ///		Finishes execution of this process permenently, and returns the given result
        ///		to the process manager.
        /// </summary>
        /// <param name="result">Result of process execution.</param>
        public override void Finish(ProcessResult result)
        {
            base.Finish(result);

            // Make sure other processes are finished.
            if (_delayProcess != null) _delayProcess.Finish(ProcessResult.Failed);
            if (_moveToProcess != null) _moveToProcess.Finish(ProcessResult.Failed);
        }

        /// <summary>
        ///     Advances the entity by one node of the path.
        /// </summary>
        public void AdvanceOneNode()
        {
            // Is it a delay node?
            if (_previousNode != null && _previousNode.Delay != 0)
            {
                if (_delayProcess == null)
                {
                    _delayProcess = new DelayProcess(_previousNode.Delay);
                    ProcessManager.AttachProcess(_delayProcess);
                    WaitForProcess(_delayProcess, ProcessResult.Success);
                    return;
                }
                else
                    _delayProcess = null;
            }

            // Trigger the events of the previous node.
            if (_previousNode != null) _previousNode.TriggerEvents();

            // Are we finished?
            if (_currentNode == null)
            {
                Finish(ProcessResult.Success);
                return;
            }

            // Move to the node!
            Transformation nodeTransform = _currentNode.CalculateTransformation();
            _moveToProcess = new EntityMoveToProcess(_entity, nodeTransform.X + (_currentNode.Width / 2), nodeTransform.Y + (_currentNode.Height / 2), _currentNode.Speed);
            ProcessManager.AttachProcess(_moveToProcess);
            WaitForProcess(_moveToProcess, ProcessResult.Success);

            // Get the next node.
            _previousNode = _currentNode;
            _currentNode = _currentNode.NextNode;
        }

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        /// <param name="startNode">Starting node of the path to follow.</param>
        /// <param name="entity">Entity to follow the path.</param>
        public PathFollowProcess(PathMarkerNode startNode, EntityNode entity)
        {
            _entity = entity;
            _startNode = startNode;
            _currentNode = startNode;

            AdvanceOneNode();
        }

        #endregion
    }

}
