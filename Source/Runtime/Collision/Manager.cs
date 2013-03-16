/* 
 * File: Manager.cs
 *
 * This source file contains the declaration of the CollisionManager class which 
 * is responsible for dealing with collision between multiple objects.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

//#define USE_OCTTREE

using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Events;
using BinaryPhoenix.Fusion.Runtime.Debug;

namespace BinaryPhoenix.Fusion.Runtime.Collision
{

	/// <summary>
	///     This class is contains all the code responsible for recognizing 
    ///     and responding to collisions
	/// </summary>
	public static class CollisionManager
	{
		#region Members
		#region Variables

		private static ArrayList _polygonList = new ArrayList();

        private static bool _enabled = true;

        private static ArrayList[,] _octTree = null;
        private static CollisionRectangle[,] _octTreeCollisionRectangles = null;
        private static ArrayList _octTreeAdditions = new ArrayList();
        private static ArrayList _octTreeRemovals = new ArrayList();
        private static int _octTreeXCells, _octTreeYCells;

        private static ArrayList[] _touchPolygons = new ArrayList[2] { new ArrayList(), new ArrayList() };
        private static ArrayList[] _enterPolygons = new ArrayList[2] { new ArrayList(), new ArrayList() };
        private static ArrayList[] _leavePolygons = new ArrayList[2] { new ArrayList(), new ArrayList() };

		#endregion
		#region Properties

        /// <summary>
        ///		Gets a list of all polygons attached to this manager.
        /// </summary>
        public static ArrayList Polygons
        {
            get { return _polygonList; }
        }

        /// <summary>
        ///     Gets or sets if collision processing is enabled or not.
        /// </summary>
        public static bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Attachs a given collision polygon to this manager.
		/// </summary>
		/// <param name="polygon">Collision polygon to attach.</param>
		public static void AttachPolygon(CollisionPolygon polygon)
		{
            if (_polygonList.Contains(polygon)) return;
			_polygonList.Add(polygon);
#if USE_OCTTREE
            AddToOctTree(polygon);
#endif
        }

		/// <summary>
		///		Detachs a given collision polygon to this manager.
		/// </summary>
		/// <param name="polygon">Collision polygon to detach.</param>
		public static void DetachPolygon(CollisionPolygon polygon)
		{
			_polygonList.Remove(polygon);
#if USE_OCTTREE
            RemoveFromOctTree(polygon);
#endif
        }

        /// <summary>
        ///     Detachs all attached polygons.
        /// </summary>
        public static void Clear()
        {
#if USE_OCTTREE
            for (int x = 0; x < _octTreeXCells; x++)
                for (int y = 0; y < _octTreeYCells; y++)
                    _octTree[x, y].Clear();
#endif
            _polygonList.Clear();
        }

#if USE_OCTTREE
        /// <summary>
        ///     Adds the polygon to the oct-tree.
        /// </summary>
        /// <param name="polygon">Collision polygon to attach.</param>
        private static void AddToOctTree(CollisionPolygon polygon)
        {
            _octTreeAdditions.Add(polygon);
        }

        /// <summary>
        ///     Adds the polygon to the oct-tree.
        /// </summary>
        /// <param name="polygon">Collision polygon to detach.</param>
        private static void RemoveFromOctTree(CollisionPolygon polygon)
        {
            _octTreeRemovals.Add(polygon);
        }

        /// <summary>
        ///     Generates an OctTree used to improve collision processing.
        /// </summary>
        /// <returns>Newly generated OctTree.</returns>
        private static ArrayList[,] GenerateOctTree()
        {
            // Work out a full-size bounding box that encloses all polygons.
            float minX = 0, minY = 0, maxX = 0, maxY = 0;
            foreach (CollisionPolygon prePolygon in _polygonList)
            {
                if (prePolygon.Transformation.X < minX) minX = prePolygon.Transformation.X;
                if (prePolygon.Transformation.Y < minY) minY = prePolygon.Transformation.Y;
                if (prePolygon.Transformation.X + prePolygon.BoundingWidth > maxX) maxX = prePolygon.Transformation.X + prePolygon.BoundingWidth;
                if (prePolygon.Transformation.Y + prePolygon.BoundingHeight > maxY) maxY = prePolygon.Transformation.Y + prePolygon.BoundingHeight;
            }

            int octTreeCellWidth = 128;
            int octTreeCellHeight = 128;
            int octTreeXCells = (int)Math.Ceiling((maxX - minX) / (float)octTreeCellWidth);
            int octTreeYCells = (int)Math.Ceiling((maxY - minY) / (float)octTreeCellHeight);

            if (_octTree == null || octTreeXCells != _octTreeXCells || octTreeYCells != _octTreeYCells)
            {
                // Generate an Oct-Tree.
                _octTreeXCells = octTreeXCells;
                _octTreeYCells = octTreeYCells;
                _octTree = new ArrayList[octTreeXCells, octTreeYCells];
                for (int x = 0; x < octTreeXCells; x++)
                    for (int y = 0; y < octTreeYCells; y++)
                        _octTree[x, y] = new ArrayList();

                // Generate collision rectangles.
                _octTreeCollisionRectangles = new CollisionRectangle[octTreeXCells, octTreeYCells];
                for (int x = 0; x < octTreeXCells; x++)
                    for (int y = 0; y < octTreeYCells; y++)
                        _octTreeCollisionRectangles[x, y] = new CollisionRectangle(new Transformation(minX + (x * octTreeCellWidth), minY + (y * octTreeCellHeight), 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f), octTreeCellWidth, octTreeCellHeight);

                // Place entities into Oct-Tree cells.
                for (int x = 0; x < octTreeXCells; x++)
                    for (int y = 0; y < octTreeYCells; y++)
                    {
                        foreach (CollisionPolygon prePolygon in _polygonList)
                        {
                            if (_octTreeCollisionRectangles[x, y].HitTest(prePolygon) == true)
                                _octTree[x, y].Add(prePolygon);
                        }
                    }
            }
            else
            {
                // Have any polygons moved? If so we need to update the cells they are in.
                foreach (CollisionPolygon polygon in _polygonList)
                {
                    if ((!((polygon.PreviousTransformationValid == false ||
                        ((polygon.Transformation.X - polygon.PreviousTransformation.X == 0) &&
                        (polygon.Transformation.Y - polygon.PreviousTransformation.Y == 0))) && polygon.FrameCount > 0)))// || polygon.BoundingChanged == true)
                    {
                        _octTreeRemovals.Add(polygon);
                        _octTreeAdditions.Add(polygon);
                    }
                }
            }

            // Remove superflous ones.
            if (_octTreeRemovals.Count != 0)
            {
                foreach (CollisionPolygon polygon in _octTreeRemovals)
                {
                    for (int x = 0; x < octTreeXCells; x++)
                        for (int y = 0; y < octTreeYCells; y++)
                            _octTree[x, y].Remove(polygon);
                }
                _octTreeRemovals.Clear();
            }

            // Add aditional ones.
            if (_octTreeAdditions.Count != 0)
            {
                foreach (CollisionPolygon polygon in _octTreeAdditions)
                {
                    for (int x = 0; x < octTreeXCells; x++)
                        for (int y = 0; y < octTreeYCells; y++)
                        {
                            if (_octTreeCollisionRectangles[x, y].HitTest(polygon) == true)
                            {
                                _octTree[x, y].Add(polygon);
                            }
                        }
                }
                _octTreeAdditions.Clear();
            }

            return _octTree;
        }

        /// <summary>
        ///     Processes all the collisions that can occur within a specific oct-tree cell.
        /// </summary>
        /// <param name="list">List of polygons in the oct tree cell.</param>
        private static int ProcessOctTreeCellCollisions(ArrayList list)
        {
            int collisionsProcessed = 0;
            foreach (CollisionPolygon aPolygon in list)
            {
                //if (aPolygon.BoundingChanged == false || aPolygon.FrameCount == 0)
                //{
                    if ((aPolygon.PreviousTransformationValid == false ||
                        ((aPolygon.Transformation.X - aPolygon.PreviousTransformation.X == 0) &&
                         (aPolygon.Transformation.Y - aPolygon.PreviousTransformation.Y == 0))) && aPolygon.FrameCount > 0) continue;
                 if ((aPolygon.Solid == false && aPolygon.OnLeaveDelegate == null && aPolygon.OnLeaveDelegate == null && aPolygon.OnCollidingDelegate == null)) continue;
                       
                //}

                foreach (CollisionPolygon bPolygon in _polygonList)
                {
                    //if (bPolygon.BoundingChanged == false || bPolygon.FrameCount == 0)
                    //{
                    if ((aPolygon == bPolygon || bPolygon.PreviousTransformationValid == false) && bPolygon.FrameCount > 0 && aPolygon.FrameCount > 0) continue;
                    //}

                    // Check we are on the same collision layer.
                    for (int i = 0; i < aPolygon.Layers.Length; i++)
                        for (int j = 0; j < bPolygon.Layers.Length; j++)
                            if (aPolygon.Layers[i] == bPolygon.Layers[j])
                                goto outOfLoopC;

                    continue;
                outOfLoopC:


                    if (aPolygon.HitTest(bPolygon) == true)
                    {
                        // Are we processing this one?
                        if (aPolygon.Solid == true && bPolygon.Solid == true)
                            collisionsProcessed++;

                        // Work out the acceleration of each polygon.
                        float aVectorX = aPolygon.Transformation.X - aPolygon.PreviousTransformation.X;
                        float aVectorY = aPolygon.Transformation.Y - aPolygon.PreviousTransformation.Y;
                        float aAcceleration = aPolygon.PreviousTransformationValid == false ? 0 : (float)Math.Sqrt(aVectorX * aVectorX + aVectorY * aVectorY);
                        float bVectorX = bPolygon.Transformation.X - bPolygon.PreviousTransformation.X;
                        float bVectorY = bPolygon.Transformation.Y - bPolygon.PreviousTransformation.Y;
                        float bAcceleration = bPolygon.PreviousTransformationValid == false ? 0 : (float)Math.Sqrt(bVectorX * bVectorX + bVectorY * bVectorY);

                        // If both are static then we should leave them alone.
                        //if (aAcceleration == 0 && bAcceleration == 0)
                        //    continue;

                        // If accelerations are the same then use the one that has
                        // had the most acceleration in the past.
                        if (aAcceleration == bAcceleration && (aAcceleration != 0 && bAcceleration != 0))
                        {
                            if (Math.Abs(aPolygon.TopAcceleration) > Math.Abs(bPolygon.TopAcceleration))
                                aAcceleration++;
                            else
                                bAcceleration++;
                        }
                        else
                        {
                            // Check if this is the highest acceleration we have ever
                            // reached.
                            if (aAcceleration > aPolygon.TopAcceleration)
                                aPolygon.TopAcceleration = aAcceleration;
                            if (bAcceleration > bPolygon.TopAcceleration)
                                bPolygon.TopAcceleration = bAcceleration;
                        }

                        // If the second polygon is moving faster then let it deal with it not us.
                        if (Math.Abs(bAcceleration) > Math.Abs(aAcceleration) || (aAcceleration == bAcceleration && (aAcceleration != 0 && bAcceleration != 0))) continue;

                        // Its solid so check for touch events and possible collisions responses.
                        if (aPolygon.Solid == true && bPolygon.Solid == true)
                        {
                            // Respond!
                            aPolygon.RespondToCollision(bPolygon);

                            _touchPolygons[0].Add(aPolygon);
                            _touchPolygons[1].Add(bPolygon);
                        }

                        // Its a volume so check for enter/leave events.
                        else
                        {
                            if (aPolygon.EnteredPolygons.Contains(bPolygon) == false)
                            {
                                _enterPolygons[0].Add(aPolygon);
                                _enterPolygons[1].Add(bPolygon);

                                // Make a note that we have hit this polygon.
                                aPolygon.EnteredPolygons.Add(bPolygon);
                            }
                        }
                    }

                    // Check if we were inside this polygon last frame.
                    else if (aPolygon.EnteredPolygons.Contains(bPolygon))
                    {
                        // Remove the polygon from the entered list.
                        aPolygon.EnteredPolygons.Remove(bPolygon);

                        // Call the leaving delegate.
                        _leavePolygons[0].Add(aPolygon);
                        _leavePolygons[1].Add(bPolygon);
                    }
                }
            }
            return collisionsProcessed;
        }
#endif

        /// <summary>
		///     Looks through all the polygons attached to this manager and 
        ///     attempts to resolve any collisions that have occured.
		/// </summary>
		public static void ProcessCollisions()
		{
#if USE_OCTTREE
            bool runAgain = false;

            // Are collisions enabled?
            if (_enabled == false)
                return;

            // Notify the entity using the polygon that we are about to test 
            // for collision.
            foreach (CollisionPolygon prePolygon in _polygonList)
                if (prePolygon.PreProcessingDelegate != null) prePolygon.PreProcessingDelegate();
           
            // Generate our new oct-tree.
            ArrayList[,] octTree = GenerateOctTree();

            // Clear out collisions arrays.
            _touchPolygons[0].Clear(); _touchPolygons[1].Clear();
            _leavePolygons[0].Clear(); _leavePolygons[1].Clear();
            _enterPolygons[0].Clear(); _enterPolygons[1].Clear();

            // Go through each oct-tree cell and process any collisions within them.
            int collisionsProcessed = 0;
            int maximumLoopCount = 1;
            try
            {
                do
                {
                    collisionsProcessed = 0;
                    for (int x = 0; x < _octTreeXCells; x++)
                        for (int y = 0; y < _octTreeYCells; y++)
                        {
                            int cellCols = ProcessOctTreeCellCollisions(_octTree[x, y]);
                            collisionsProcessed += cellCols;
                        }
                    maximumLoopCount--;
                } while (collisionsProcessed > 0 && maximumLoopCount > 0);
            }
            catch (InvalidOperationException) // The entities may try and remove / add polygons while they are being updated. Bad move :S.
            {
                runAgain = true;
            }

            // Notify the entity using the polygon that we have finished 
            // testing for collision.
            foreach (CollisionPolygon postPolygon in _polygonList)
                if (postPolygon.PostProcessingDelegate != null) postPolygon.PostProcessingDelegate();

            // Invoke all enter / leave / touch events. This is done outside the loop
            // to make sure correct prioritation is given to each one.
            for (int i = 0; i < _leavePolygons[0].Count; i++)
            {
                CollisionPolygon aPolygon = _leavePolygons[0][i] as CollisionPolygon;
                CollisionPolygon bPolygon = _leavePolygons[1][i] as CollisionPolygon;
                if (aPolygon.OnLeaveDelegate != null) aPolygon.OnLeaveDelegate(aPolygon, bPolygon);
                if (bPolygon.OnLeaveDelegate != null) bPolygon.OnLeaveDelegate(aPolygon, bPolygon);
            }
            for (int i = 0; i < _enterPolygons[0].Count; i++)
            {
                CollisionPolygon aPolygon = _enterPolygons[0][i] as CollisionPolygon;
                CollisionPolygon bPolygon = _enterPolygons[1][i] as CollisionPolygon;
                if (aPolygon.OnEnterDelegate != null) aPolygon.OnEnterDelegate(aPolygon, bPolygon);
                if (bPolygon.OnEnterDelegate != null) bPolygon.OnEnterDelegate(aPolygon, bPolygon);
            }
            for (int i = 0; i < _touchPolygons[0].Count; i++)
            {
                CollisionPolygon aPolygon = _touchPolygons[0][i] as CollisionPolygon;
                CollisionPolygon bPolygon = _touchPolygons[1][i] as CollisionPolygon;
                if (aPolygon.OnTouchDelegate != null) aPolygon.OnTouchDelegate(aPolygon, bPolygon);
                if (bPolygon.OnTouchDelegate != null) bPolygon.OnTouchDelegate(aPolygon, bPolygon);
            }

            // Reset bounding flags. (Probably superflous)
            foreach (CollisionPolygon prePolygon in _polygonList)
            {
                prePolygon.BoundingChanged = false;
                prePolygon.FrameCount++;
            }

            // Dam it, these is a really unclean way of doing it.
            if (runAgain == true)
                ProcessCollisions();
#else

            if (_enabled == false)
                return;

            bool runAgain = false;

            // Notify the entity using the polygon that we are about to test 
            // for collision.
			foreach (CollisionPolygon prePolygon in _polygonList)
				if (prePolygon.PreProcessingDelegate != null) prePolygon.PreProcessingDelegate();

            // Test each polygon for collision against other objects.
            ArrayList collisionA = new ArrayList();
            ArrayList collisionB = new ArrayList();

            ArrayList enterPolygonA = new ArrayList();
            ArrayList enterPolygonB = new ArrayList();
            ArrayList leavePolygonA = new ArrayList();
            ArrayList leavePolygonB = new ArrayList();
            ArrayList touchPolygonA = new ArrayList();
            ArrayList touchPolygonB = new ArrayList();
            try
            {

                int collisionsProcessed = 0;
                int loopIndex = 0;
                do
                {
                    collisionsProcessed = 0;
                    foreach (CollisionPolygon aPolygon in _polygonList)
                    {
                        if ((aPolygon.PreviousTransformationValid == false ||
                            ((aPolygon.Transformation.X - aPolygon.PreviousTransformation.X == 0) &&
                             (aPolygon.Transformation.Y - aPolygon.PreviousTransformation.Y == 0))) && aPolygon.FrameCount > 0) continue;
                        //if ((aPolygon.Solid == false && aPolygon.OnEnterDelegate == null && aPolygon.OnLeaveDelegate == null && aPolygon.OnCollidingDelegate == null)) continue;
                        //HighPreformanceTimer timer = new HighPreformanceTimer();

                        foreach (CollisionPolygon bPolygon in _polygonList)
                        {
                            if ((aPolygon == bPolygon || bPolygon.PreviousTransformationValid == false) && bPolygon.FrameCount > 0) continue;

                            // Check we are on the same collision layer.
                            for (int i = 0; i < aPolygon.Layers.Length; i++)
                                for (int j = 0; j < bPolygon.Layers.Length; j++)
                                    if (aPolygon.Layers[i] == bPolygon.Layers[j])
                                        goto outOfLoopC;

                            continue;
                        outOfLoopC:

                            if (aPolygon.HitTest(bPolygon) == true)
                            {
                                // Are we processing this one?
                                if (aPolygon.Solid == true && bPolygon.Solid == true)
                                    collisionsProcessed++;

                                // Work out the acceleration of each polygon.
                                float aVectorX = aPolygon.Transformation.X - aPolygon.PreviousTransformation.X;
                                float aVectorY = aPolygon.Transformation.Y - aPolygon.PreviousTransformation.Y;
                                float aAcceleration = aPolygon.PreviousTransformationValid == false ? 0 : (float)Math.Sqrt(aVectorX * aVectorX + aVectorY * aVectorY);
                                float bVectorX = bPolygon.Transformation.X - bPolygon.PreviousTransformation.X;
                                float bVectorY = bPolygon.Transformation.Y - bPolygon.PreviousTransformation.Y;
                                float bAcceleration = bPolygon.PreviousTransformationValid == false ? 0 : (float)Math.Sqrt(bVectorX * bVectorX + bVectorY * bVectorY);

                                // If both are static then we should leave them alone.
                                //if (aAcceleration == 0 && bAcceleration == 0)
                                //    continue;

                                // If accelerations are the same then use the one that has
                                // had the most acceleration in the past.
                                if (aAcceleration == bAcceleration && (aAcceleration != 0 && bAcceleration != 0))
                                {
                                    if (Math.Abs(aPolygon.TopAcceleration) > Math.Abs(bPolygon.TopAcceleration))
                                        aAcceleration++;
                                    else
                                        bAcceleration++;
                                }
                                else
                                {
                                    // Check if this is the highest acceleration we have ever
                                    // reached.
                                    if (aAcceleration > aPolygon.TopAcceleration)
                                        aPolygon.TopAcceleration = aAcceleration;
                                    if (bAcceleration > bPolygon.TopAcceleration)
                                        bPolygon.TopAcceleration = bAcceleration;
                                }

                                // If the second polygon is moving faster then let it deal with it not us.
                                if ((bPolygon.OnEnterDelegate != null || bPolygon.OnLeaveDelegate != null || bPolygon.OnCollidingDelegate != null) && (Math.Abs(bAcceleration) > Math.Abs(aAcceleration) || (aAcceleration == bAcceleration && (aAcceleration != 0 && bAcceleration != 0)))) continue;
                                
                                // Its solid so check for touch events and possible collisions responses.
                                if (aPolygon.Solid == true && bPolygon.Solid == true)
                                {
                                    // Respond!
                                    aPolygon.RespondToCollision(bPolygon);
                                 
                                    touchPolygonA.Add(aPolygon);
                                    touchPolygonB.Add(bPolygon);
                                }

                                // Its a volume so check for enter/leave events.
                                else
                                {
                                    if (aPolygon.EnteredPolygons.Contains(bPolygon) == false)
                                    {
                                        enterPolygonA.Add(aPolygon);
                                        enterPolygonB.Add(bPolygon);

                                        // Make a note that we have hit this polygon.
                                        aPolygon.EnteredPolygons.Add(bPolygon);
                                    }
                                }
                            }

                            // Check if we were inside this polygon last frame.
                            else if (aPolygon.EnteredPolygons.Contains(bPolygon))
                            {
                                // Remove the polygon from the entered list.
                                aPolygon.EnteredPolygons.Remove(bPolygon);

                                // Call the leaving delegate.
                                leavePolygonA.Add(aPolygon);
                                leavePolygonB.Add(bPolygon);
                            }
                        }

                        //if (timer.DurationMillisecond > 1)
                        //{
                        //    System.Console.WriteLine("Took " + timer.DurationMillisecond + " to check collisions of " + aPolygon.MetaData.ToString());
                        //}
                    }


                    loopIndex++;
                } while (collisionsProcessed != 0 && loopIndex < 10);
            }
            catch (InvalidOperationException) { runAgain = true; } // The scripts occassionally try to be cheeky me modifying the list.

            // Notify the entity using the polygon that we have finished 
            // testing for collision.
			foreach (CollisionPolygon postPolygon in _polygonList)
				if (postPolygon.PostProcessingDelegate != null) postPolygon.PostProcessingDelegate();

            // Reset bounding flags. (Probably superflous)
            foreach (CollisionPolygon prePolygon in _polygonList)
            {
                prePolygon.BoundingChanged = false;
                prePolygon.FrameCount++;
            }

            // If we have to do collision again because of some unknown error. Then do it now.
            if (runAgain == true)
                ProcessCollisions();

            // ----------------------------------------------------------
            // The events are called outside the loop so as to make sure
            // that leave events are always called before enter events!
            // ----------------------------------------------------------

            // Throw out leave events!
            for (int i = 0; i < leavePolygonA.Count; i++)
            {
                CollisionPolygon aPolygon = leavePolygonA[i] as CollisionPolygon;
                CollisionPolygon bPolygon = leavePolygonB[i] as CollisionPolygon;

                // Call the enter delegate.
                if (aPolygon.OnLeaveDelegate != null) aPolygon.OnLeaveDelegate(aPolygon, bPolygon);
                if (bPolygon.OnLeaveDelegate != null) bPolygon.OnLeaveDelegate(aPolygon, bPolygon);
            }

            // Throw out enter events!
            for (int i = 0; i < enterPolygonA.Count; i++)
            {
                CollisionPolygon aPolygon = enterPolygonA[i] as CollisionPolygon;
                CollisionPolygon bPolygon = enterPolygonB[i] as CollisionPolygon;

                // Call the enter delegate.
                if (aPolygon.OnEnterDelegate != null) aPolygon.OnEnterDelegate(aPolygon, bPolygon);
                if (bPolygon.OnEnterDelegate != null) bPolygon.OnEnterDelegate(aPolygon, bPolygon);
            }

            // Throw out touch events!
            for (int i = 0; i < touchPolygonA.Count; i++)
            {
                CollisionPolygon aPolygon = touchPolygonA[i] as CollisionPolygon;
                CollisionPolygon bPolygon = touchPolygonB[i] as CollisionPolygon;

                // Call the enter delegate.
                if (aPolygon.OnTouchDelegate != null) aPolygon.OnTouchDelegate(aPolygon, bPolygon);
                if (bPolygon.OnTouchDelegate != null) bPolygon.OnTouchDelegate(aPolygon, bPolygon);
            }
#endif
        }

        /// <summary>
        ///     Checks for collision against a given polygon and all the other polygons
        ///     attached to this collision manager.
        /// </summary>
        /// <param name="aPolygon">Polygon to check collision against.</param>
        /// <returns>True if a collision exists, else false.</returns>
        public static bool HitTest(CollisionPolygon aPolygon)
        {
            if (aPolygon.Solid == false) return false;

            foreach (CollisionPolygon polygon in _polygonList)
                if (aPolygon != polygon && polygon.HitTest(aPolygon) == true && polygon.Solid == true)
                    return true;

            return false;
        }

        /// <summary>
        ///     Checks for and returns the polygon at the given point.
        /// </summary>
        /// <param name="x">Position on the x-axis.</param>
        /// <param name="y">Position on the y-axis.</param>
        /// <returns>Null if no polygon at given point, else true.</returns>
        public static CollisionPolygon PolygonAtPoint(int x, int y)
        {
            CollisionPolygon aPolygon = new CollisionRectangle(new Transformation(x, y, 0, 0, 0, 0, 1, 1, 1), 1, 1);

            foreach (CollisionPolygon polygon in _polygonList)
                if (aPolygon != polygon && polygon.HitTest(aPolygon) == true && polygon.Solid == true)
                    return polygon;

            return null;
        }

		#endregion
	}

	/// <summary>
	///     Used to notify an event handler function that a collision has occured 
    ///     between the given polygons.
	/// </summary>
	/// <param name="aPolygon">Polygon that collided.</param>
	/// <param name="bPolygon">Polygon that the first polygon collided with.</param>
	public delegate void CollisionNotificationDelegate(CollisionPolygon aPolygon, CollisionPolygon bPolygon);

	/// <summary>
	///     Used to notify an event handler function that collision 
    ///     processing is about to begin or finish.
	/// </summary>
	public delegate void CollisionProcessingNotificationDelegate();

	/// <summary>
	///     Used as an abstracted base for collision shapes. Contains all the code
    ///     required to check for collisions against this shape.
	/// </summary>
	public abstract class CollisionPolygon
	{
		#region Members
		#region Variables

		protected CollisionProcessingNotificationDelegate _preProcessingDelegate = null;
		protected CollisionProcessingNotificationDelegate _postProcessingDelegate = null;

		protected CollisionNotificationDelegate _onEnterDelegate = null;
		protected CollisionNotificationDelegate _onLeaveDelegate = null;
		protected CollisionNotificationDelegate _onTouchDelegate = null;
        protected CollisionNotificationDelegate _onCollidingDelegate = null;
		protected Transformation _previousTransformation = new Transformation();
		protected Transformation _transformation = new Transformation();
        protected bool _previousTransformationValid = false, _transformationValid = false;
        protected int[] _layers = new int[] { 1 };
		protected bool _solid = true;
        protected ArrayList _enteredList = new ArrayList();
        //protected ArrayList _touchedList = new ArrayList();

        protected object _metaData = null;

        protected float _topAcceleration = 0.0f;

        protected int _frameCount = 0;

		#endregion
		#region Properties

		/// <summary>
		///     Gets or sets the delegate invoked before collision processing takes place.
		/// </summary>
		public CollisionProcessingNotificationDelegate PreProcessingDelegate
		{
			get { return _preProcessingDelegate; }
			set { _preProcessingDelegate = value; }
		}

		/// <summary>
        ///     Gets or sets the delegate invoked after collision processing takes place.
		/// </summary>
		public CollisionProcessingNotificationDelegate PostProcessingDelegate
		{
			get { return _postProcessingDelegate; }
			set { _postProcessingDelegate = value; }
		}

		/// <summary>
        ///     Gets or sets the delegate invoked when this polygon enters another one.
		/// </summary>
		public CollisionNotificationDelegate OnEnterDelegate
		{
			get { return _onEnterDelegate; }
			set { _onEnterDelegate = value; }
		}

		/// <summary>
        ///     Gets or sets the delegate invoked when this polygon leaves a previously
        ///     entered polygon.
		/// </summary>
		public CollisionNotificationDelegate OnLeaveDelegate
		{
			get { return _onLeaveDelegate; }
			set { _onLeaveDelegate = value; }
		}

		/// <summary>
        ///     Gets or sets the delegate invoked when this polygon touches another one.
		/// </summary>
		public CollisionNotificationDelegate OnTouchDelegate
		{
			get { return _onTouchDelegate; }
			set { _onTouchDelegate = value; }
		}

        /// <summary>
        ///     Gets or sets the delegate invoked when this polygon is touching another.
        /// </summary>
        public CollisionNotificationDelegate OnCollidingDelegate
        {
            get { return _onCollidingDelegate; }
            set { _onCollidingDelegate = value; }
        }

        /// <summary>
        ///     Gets or sets the meta data that this polygon holds.
        /// </summary>  
        public object MetaData
        {
            get { return _metaData; }
            set { _metaData = value; }
        }

        /// <summary>
        ///     Gets or sets the highest acceleration this polygon has ever had.
        /// </summary>  
        public float TopAcceleration
        {
            get { return _topAcceleration; }
            set { _topAcceleration = value; }
        }

		/// <summary>
		///     Gets or sets the collision layers that this polygon acts apon.
		/// </summary>  
		public int[] Layers
		{
			get { return _layers; }
			set { _layers = value; }
		}

		/// <summary>
		///     Gets or sets if this polygon is solid or not.
		/// </summary>
		public bool Solid
		{
			get { return _solid; }
			set { _solid = value; }
		}

		/// <summary>
		///     Gets or sets the list of polygons that this polygon is currently inside.
		/// </summary>
        public ArrayList EnteredPolygons
		{
            get { return _enteredList; }
            set { _enteredList = value; }
		}

        /// <summary>
        ///     Gets or sets the list of polygons that this polygon touched the previous frame.
        /// </summary>
        //public ArrayList TouchedPolygons
        //{
        //    get { return _touchedList; }
        //    set { _touchedList = value; }
        //}

		/// <summary>
		///     Gets or sets if the previous transformation of this polygon is valid.
		/// </summary>
		public bool PreviousTransformationValid
		{ 
			get { return _previousTransformationValid; }
			set { _previousTransformationValid = value; }
		}

		/// <summary>
		///     Gets or ses the transformation of this polygon on the previous frame.
		/// </summary>
		public Transformation PreviousTransformation
		{
			get { return _previousTransformation; }
			set { _previousTransformation = value; }
		}

        /// <summary>
        ///     Gets or sets the amount of frames since this polygon was created.
        /// </summary>
        public int FrameCount
        {
            get { return _frameCount; }
            set { _frameCount = value; }
        }

		/// <summary>
		///     Gets or sets the transformation of this polygon.
		/// </summary>  
		public Transformation Transformation
		{
			get { return _transformation; }
			set 
            {
                if (_previousTransformationValid == false && (_transformationValid == true || _frameCount == 0))
                    _previousTransformationValid = true;
                _previousTransformation = _transformation; 
                _transformation = value;
                _transformationValid = true;
            }
		}

        /// <summary>
        ///     Gets or sets if the bounding has been changed for this polygon.
        /// </summary>
        public virtual bool BoundingChanged
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the bounding width of this polygon.
        /// </summary>
        public virtual float BoundingWidth
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the bounding height of this polygon.
        /// </summary>
        public virtual float BoundingHeight
        {
            get;
            set;
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///     Checks if this polygons is is colliding with a given polygon.
		/// </summary>
		/// <param name="polygon">Polygon to check collision against.</param>
		/// <returns>True if polygons are colliding, else false.</returns>
		public abstract bool HitTest(CollisionPolygon polygon);

		/// <summary>
		///     Attempts to respond to a collision by moving this polygon out
        ///     of the given polygon.
		/// </summary>
		/// <param name="polygon">Polygon that this polygon is currently penetrating.</param>
        public abstract void RespondToCollision(CollisionPolygon polygon);

		/// <summary>
		///		When this method is called it will create an exact copy of this collision polygon.
		/// </summary>
		/// <returns>Exact copy of this collision polygon.</returns>
		public CollisionPolygon Clone()
		{
			CollisionPolygon polygon = (CollisionPolygon)Activator.CreateInstance(this.GetType());
			this.CopyTo(polygon);
			return polygon;
		}

		/// <summary>
		///		Copys all the data contained in this collision polygon to another collision polygon.
		/// </summary>
		/// <param name="polygon">Collision polygon to copy data into.</param>
		public virtual void CopyTo(CollisionPolygon polygon)
		{
			polygon._onEnterDelegate = _onEnterDelegate;
			polygon._onLeaveDelegate = _onLeaveDelegate;
			polygon._onTouchDelegate = _onTouchDelegate;
            polygon._onCollidingDelegate = _onCollidingDelegate;
			foreach (CollisionPolygon subPolygon in _enteredList)
				polygon._enteredList.Add(subPolygon);
           // foreach (CollisionPolygon subPolygon in _touchedList)
           //     polygon._touchedList.Add(subPolygon);
			polygon._solid = _solid;
			polygon._transformation = _transformation;
            polygon._layers = new int[_layers.Length];
            for (int i = 0; i < _layers.Length; i++)
                polygon._layers[i] = _layers[i];
		}

        #endregion
    }

    /// <summary>
    ///     Contains the code required to check for and respond to rectangle collisions.
    /// </summary>
    public class CollisionRectangle : CollisionPolygon
	{
		#region Members
		#region Variables

		private float _width = 0.0f, _height = 0.0f;
        private float _previousWidth = 0.0f, _previousHeight = 0.0f;
        private bool _boundingChanged = false;

		#endregion
		#region Properties

		/// <summary>
		///     Gets or sets the width of this rectangle.
		/// </summary>
		public override float BoundingWidth
		{
			get { return _width; }
            set 
            {
                if (value != _previousWidth && _frameCount > 0) _boundingChanged = true;
                _previousWidth = _width;
                _width = value; 
            }
		}

		/// <summary>
		///     Gets or sets the height of this rectangle.
		/// </summary>
        public override float BoundingHeight
		{
			get { return _height; }
            set
            {
                if (value != _previousHeight && _frameCount > 0) _boundingChanged = true;
                _previousHeight = _height;
                _height = value;
            }
		}

        /// <summary>
        ///     Gets or sets if the bounding has been changed for this polygon.
        /// </summary>
        public override bool BoundingChanged
        {
            get { return _boundingChanged; }
            set { _boundingChanged = value; }
        }

		#endregion
		#endregion
		#region Methods

        /// <summary>
        ///     Checks if this polygons is is colliding with a given polygon.
        /// </summary>
        /// <param name="polygon">Polygon to check collision against.</param>
        /// <returns>True if polygons are colliding, else false.</returns>
		public override bool HitTest(CollisionPolygon polygon)
		{
			if (polygon is CollisionRectangle)
			{
				CollisionRectangle collisionRectangle = polygon as CollisionRectangle;
				if (collisionRectangle._width == 0 || collisionRectangle._height == 0 || _width == 0 || _height == 0) return false;

				int right1 = (int)(collisionRectangle.Transformation.X) + (int)(collisionRectangle.BoundingWidth * Math.Abs(collisionRectangle.Transformation.ScaleX));
                int bottom1 = (int)(collisionRectangle.Transformation.Y) + (int)(collisionRectangle.BoundingHeight * Math.Abs(collisionRectangle.Transformation.ScaleY));
                int right2 = (int)(_transformation.X) + (int)(_width * Math.Abs(_transformation.ScaleX));
                int bottom2 = (int)(_transformation.Y) + (int)(_height * Math.Abs(_transformation.ScaleY));

                return (!((bottom1 <= (int)(_transformation.Y)) || ((int)(collisionRectangle.Transformation.Y) >= bottom2) || (right1 <= (int)(_transformation.X)) || ((int)(collisionRectangle.Transformation.X) >= right2)));
			}
			return false;
		}

        /// <summary>
        ///     Attempts to respond to a collision by moving this rectangle out
        ///     of the given polygon.
        /// </summary>
        /// <param name="polygon">Polygon that this rectangle is currently penetrating.</param>
        public override void RespondToCollision(CollisionPolygon polygon)
        {
            if (polygon is CollisionRectangle)
                RespondToRectangleCollision(polygon as CollisionRectangle);
            else
                return;
        }

        /// <summary>
        ///     Attempts to respond to a collision by moving this rectangle out
        ///     of the given rectangle.
        /// </summary>
        /// <param name="rectangle">Rectangle that this rectangle is currently penetrating.</param>
        public void RespondToRectangleCollision(CollisionRectangle rectangle)
        {
            // Truncate the positions of the entitys as decimal places tend
            // to fuck up collision response.

            _transformation.X = (float)Math.Truncate(_transformation.X);
            _transformation.Y = (float)Math.Truncate(_transformation.Y);
            rectangle._transformation.X = (float)Math.Truncate(rectangle._transformation.X);
            rectangle._transformation.Y = (float)Math.Truncate(rectangle._transformation.Y);

            // Really crude form of SAT collision response.
            // Probably a good idea to clean this up a bit, possibly implement
            // vector projection as well, to remove the if blocks.

            float rectangleWidth = rectangle._width * Math.Abs(rectangle._transformation.ScaleX), rectangleHeight = rectangle._height * Math.Abs(rectangle._transformation.ScaleY);
            float rectangleCenterX = rectangle._transformation.X + (rectangleWidth / 2.0f), rectangleCenterY = rectangle._transformation.Y + (rectangleHeight / 2.0f);
            float thisWidth = _width * Math.Abs(_transformation.ScaleX), thisHeight = _height * Math.Abs(_transformation.ScaleY);
            float thisCenterX = _transformation.X + (thisWidth / 2.0f), thisCenterY = _transformation.Y + (thisHeight / 2.0f);
            float rectangleVertexX = 0, rectangleVertexY = 0;
            float thisVertexX = 0, thisVertexY = 0;

            if (thisCenterX > rectangleCenterX)
                if (thisCenterY > rectangleCenterY)
                {
                    // Were in the bottom-right corner of the rectangle.
                    rectangleVertexX = rectangle._transformation.X + rectangleWidth;
                    rectangleVertexY = rectangle._transformation.Y + rectangleHeight;
                    thisVertexX = _transformation.X;
                    thisVertexY = _transformation.Y;
                }
                else
                {
                    // Were in the top-right corner of the rectangle.
                    rectangleVertexX = rectangle._transformation.X + rectangleWidth;
                    rectangleVertexY = rectangle._transformation.Y;
                    thisVertexX = _transformation.X;
                    thisVertexY = _transformation.Y + thisHeight;
                }
            else
                if (thisCenterY > rectangleCenterY)
                {
                    // Were in the bottom-left corner of the rectangle.
                    rectangleVertexX = rectangle._transformation.X;
                    rectangleVertexY = rectangle._transformation.Y + rectangleHeight;
                    thisVertexX = _transformation.X + thisWidth;
                    thisVertexY = _transformation.Y;
                }
                else
                {
                    // Were in the top-left corner of the rectangle.
                    rectangleVertexX = rectangle._transformation.X;
                    rectangleVertexY = rectangle._transformation.Y;
                    thisVertexX = _transformation.X + thisWidth;
                    thisVertexY = _transformation.Y + thisHeight;
                }

            float xDifference = thisVertexX - rectangleVertexX;
            float yDifference = thisVertexY - rectangleVertexY;
            if (Math.Abs(xDifference) > Math.Abs(yDifference))
                _transformation.Y -= yDifference;
            else
                _transformation.X -= xDifference;
        }

		/// <summary>
		///		Copys all the data contained in this collision polygon to another collision polygon.
		/// </summary>
		/// <param name="polygon">Collision polygon to copy data into.</param>
		public override void CopyTo(CollisionPolygon polygon)
		{
			CollisionRectangle rectanglePolygon = polygon as CollisionRectangle;
			if (rectanglePolygon == null) return;
			base.CopyTo(polygon);

			rectanglePolygon._width = _width;
            rectanglePolygon._height = _height;
		}

		/// <summary>
		///     Ininitalizes a new instance of this class with the given data.
		/// </summary>
		/// <param name="transformation">Transformation of this rectangle.</param>
		/// <param name="width">Width of this rectangle.</param>
		/// <param name="height">Height of this rectangle.</param>
		public CollisionRectangle(Transformation transformation, float width, float height)
		{
			_transformation = transformation;
			_width = width;
			_height = height;
		}
		public CollisionRectangle()
		{
		}

		#endregion
	}

}