/*
 * File: Group.cs
 *
 * Contains the main declaration of the group class
 * which is a derivitive of the SceneNode class and is
 * used to group large amounts of objects together.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.Drawing;
using System.Collections;
using BinaryPhoenix.Fusion.Graphics;
using BinaryPhoenix.Fusion.Runtime;

namespace BinaryPhoenix.Fusion.Engine.Entitys
{

	/// <summary>
	///		The group class is a derivitive of the SceneNode class and is
	///		used to group large amounts of objects together.
	/// </summary>
	public class GroupNode : EntityNode
	{
		#region Members
		#region Variables

		#endregion
		#region Properties

        /// <summary>
        ///		Returns a string which can be used to identify what kind of entity this is.
        /// </summary>
        public override string Type
        {
            get { return _type != "" ? _type : "group"; }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Checks if this node intersects with the given rectangle.
		/// </summary>
		/// <param name="transformation">Transformation of this nodes parent.</param>
		/// <param name="collisionList">List to add intersecting nodes to.</param>
		/// <param name="rectangle">Rectangle to check intersecting against.</param>
		public override bool RectangleBoundingBoxIntersect(Rectangle rectangle)
		{
			return false;
		}

		/// <summary>
		///		Initializes a new instance and gives it the specified  
		///		name.
		/// </summary>
		/// <param name="name">Name to name as node to.</param>
		public GroupNode(string name)
		{
			_name = name;
			DeinitializeCollision(); // We don't want our group nodes colliding with everything do we?
		}
		public GroupNode()
		{
			_name = "Group";
			DeinitializeCollision(); // We don't want our group nodes colliding with everything do we?
		}

		#endregion
	}

}