/*
 * File: Structures.cs
 *
 * Contains the main declaration of several commonly used structures.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Collections;

namespace BinaryPhoenix.Fusion.Runtime
{
    #region Classes

    /// <summary>
	///		This struct is used as a way of easily applying relative and compound
	///		transformations together.
	/// </summary>
	public struct Transformation
	{
		#region Members
		#region Variables

		private float _x, _y, _z;
		private float _scaleX, _scaleY, _scaleZ;
		private float _angleX, _angleY, _angleZ;

		#endregion
		#region Properties

		/// <summary>
		///		Gets the X position of this transformation.
		/// </summary>
		public float X
		{
			get { return _x; }
			set { _x = value; }
		}

		/// <summary>
		///		Gets the Y position of this transformation.
		/// </summary>
		public float Y
		{
			get { return _y; }
			set { _y = value; }
		}

		/// <summary>
		///		Gets the X position of this transformation.
		/// </summary>
		public float Z
		{
			get { return _z; }
			set { _z = value; }
		}

		/// <summary>
		///		Gets the angle of rotation on the x-axis of this transformation.
		/// </summary>
		public float AngleX
		{
			get { return _angleX; }
			set { _angleX = value % 360; }
		}

        /// <summary>
        ///		Gets the angle of rotation on the y-axis of this transformation.
        /// </summary>
        public float AngleY
        {
            get { return _angleY; }
            set { _angleY = value % 360; }
        }

        /// <summary>
        ///		Gets the angle of rotation on the z-axis of this transformation.
        /// </summary>
        public float AngleZ
        {
            get { return _angleZ; }
            set { _angleZ = value % 360; }
        }

		/// <summary>
		///		Gets the scale on the x-axis of this transformation.
		/// </summary>
		public float ScaleX
		{
			get { return _scaleX; }
			set { _scaleX = value; }
		}

		/// <summary>
		///		Gets the scale on the y-axis of this transformation.
		/// </summary>
		public float ScaleY
		{
			get { return _scaleY; }
			set { _scaleY = value; }
		}

        /// <summary>
        ///		Gets the scale on the z-axis of this transformation.
        /// </summary>
        public float ScaleZ
        {
            get { return _scaleZ; }
            set { _scaleZ = value; }
        }

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Converts this class to a textural form.
		/// </summary>
		/// <returns>A textural form of this class.</returns>
		public override string ToString()
		{
			return _x + "," + _y + "," + _z + " - " + _angleX +","+_angleY+","+_angleZ + " - " + _scaleX + "," + _scaleY + "," + _scaleZ;
		}

        /// <summary>
        ///     Overloads the equality operator to allow transformations to be compared together.
        /// </summary>
        /// <param name="t1">First factor.</param>
        /// <param name="t2">Second factor.</param>
        /// <returns>True if both are the same, else false.</returns>
        public static bool operator ==(Transformation t1, Transformation t2)
        {
            return t1.Equals(t2);
        }


        /// <summary>
        ///     Overloads the inequality operator to allow transformations to be compared together.
        /// </summary>
        /// <param name="t1">First factor.</param>
        /// <param name="t2">Second factor.</param>
        /// <returns>True if both are not the same, else false.</returns>
        public static bool operator !=(Transformation t1, Transformation t2)
        {
            return !t1.Equals(t2);
        }

        /// <summary>
        ///     Overloads the equality function.
        /// </summary>
        /// <param name="obj">Object to check against.</param>
        /// <returns>True if this object and the given object are equal.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Transformation))
                return base.Equals(obj);

            Transformation t2 = (Transformation)obj;
            return (_x == t2._x && _y == t2._y && _z == t2._z && _scaleX == t2._scaleX && _scaleY == t2._scaleY && _scaleZ == t2._scaleZ && _angleX == t2._angleX && _angleY == t2._angleY && _angleZ == t2._angleZ);
        }

		/// <summary>
		///		Initializes a new instance of this class.
		/// </summary>
		/// <param name="x">The x position this transformation should store.</param>
		/// <param name="y">The y position this transformation should store.</param>
		/// <param name="z">The z position this transformation should store.</param>
        /// <param name="ax">The angle of rotation on the x-axis this transformation should store.</param>
        /// <param name="ay">The angle of rotation on the y-axis this transformation should store.</param>
        /// <param name="az">The angle of rotation on the z-axis this transformation should store.</param>
		/// <param name="sx">The scale on the x-axis this transformation should store.</param>
		/// <param name="sy">The scale on the y-axis this transformation should store.</param>
        /// <param name="sz">The scale on the z-axis this transformation should store.</param>
		public Transformation(float x, float y, float z, float ax, float ay, float az, float sx, float sy, float sz)
		{
			_x = x;
			_y = y;
			_z = z;
			_angleX = ax;
            _angleY = ay;
            _angleZ = az;
			_scaleX = sx;
			_scaleY = sy;
            _scaleZ = sz;
		}

       // public Transformation() { }

		#endregion
    }

    /// <summary>
    ///     Used to define a point in space.
    /// </summary>
    [TypeConverterAttribute(typeof(VectorConverter))] 
    public struct Vector
    {
        #region Members
        #region Variables

        private float _x, _y, _z;

        #endregion
        #region Properties

        /// <summary>
        ///     Position along the x axis that this vector points to.
        /// </summary>
        [RefreshProperties(RefreshProperties.All), NotifyParentProperty(true)]
        public float X
        {
            get { return _x; }
            set { _x = value; }
        }

        /// <summary>
        ///     Position along the y axis that this vector points to.
        /// </summary>
        [RefreshProperties(RefreshProperties.All), NotifyParentProperty(true)]
        public float Y
        {
            get { return _y; }
            set { _y = value; }
        }

        /// <summary>
        ///     Position along the z axis that this vector points to.
        /// </summary>
        [RefreshProperties(RefreshProperties.All), NotifyParentProperty(true)]
        public float Z
        {
            get { return _z; }
            set { _z = value; }
        }

        /// <summary>
        ///     Gets the length of this vector.
        /// </summary>
        [Browsable(false)]
        public float Length
        {
            get { return (float)Math.Sqrt((_x * _x) + (_y * _y) + (_z * _z)); }
        }

        /// <summary>
        ///     Gets the normalized version of this vector.
        /// </summary>
        [Browsable(false)]
        public Vector Normalized
        {
            get
            {
                float length = Length;
                return new Vector(_x / length, _y / length, _z / length);
            }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Overloads the equality operator to allow vectors to be compared together.
        /// </summary>
        /// <param name="t1">First factor.</param>
        /// <param name="t2">Second factor.</param>
        /// <returns>True if both are the same, else false.</returns>
        public static bool operator ==(Vector t1, Vector t2)
        {
            return t1.Equals(t2);
        }

        /// <summary>
        ///     Overloads the inequality operator to allow vectors to be compared together.
        /// </summary>
        /// <param name="t1">First factor.</param>
        /// <param name="t2">Second factor.</param>
        /// <returns>True if both are not the same, else false.</returns>
        public static bool operator !=(Vector t1, Vector t2)
        {
            return !t1.Equals(t2);
        }

        /// <summary>
        ///     Overloads the equality function.
        /// </summary>
        /// <param name="obj">Object to check against.</param>
        /// <returns>True if this object and the given object are equal.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Vector))
                return base.Equals(obj);

            Vector t2 = (Vector)obj;
            return (_x == t2._x && _y == t2._y && _z == t2._z);
        }

        /// <summary>
        ///		Converts this class to a textural form.
        /// </summary>
        /// <returns>A textural form of this class.</returns>
        public override string ToString()
        {
            return _x + ", " + _y + ", " + _z;
        }

        /// <summary>
        ///     Returns the dot product of this vector and the given vector.
        /// </summary>
        /// <param name="v2">Second vector to get dot product from.</param>
        /// <returns>The dot product of this vector and the given vector.</returns>
        public float DotProduct(Vector v2)
        {
            return (_x * v2._x) + (_y * v2._y) + (_z * v2._z);
        }

        /// <summary>
        ///     Returns the cross product of this vector and the given vector.
        /// </summary>
        /// <param name="v2">Second vector to get cross product from.</param>
        /// <returns>The cross product of this vector and the given vector.</returns>
        public Vector CrossProduct(Vector v2)
        {
            return new Vector((_y * v2._z) - (_z * v2._y), (_z * v2._x) - (_x * v2._z), (_x * v2._y) - (_y * v2._x));
        }

        /// <summary>
        ///     Returns the projection of this vector onto a given vector.
        /// </summary>
        /// <param name="v2">Second vector to project onto.</param>
        /// <returns>Projection of this vector to the given vector.</returns>
        public Vector Project(Vector v2)
        {
            float dp = DotProduct(v2);
            return new Vector((dp / ((v2._x * v2._x) + (v2._y * v2._y) + (v2._z * v2._z))) * v2._x,
                              (dp / ((v2._x * v2._x) + (v2._y * v2._y) + (v2._z * v2._z))) * v2._y,
                              (dp / ((v2._x * v2._x) + (v2._y * v2._y) + (v2._z * v2._z))) * v2._z);
        }

        /// <summary>
        ///     Overloads the addition operator to allow vectors to be added together.
        /// </summary>
        /// <param name="v1">First factor.</param>
        /// <param name="v2">Second factor.</param>
        /// <returns>The result of adding v1 to v2.</returns>
        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1._x + v2._x, v1._y + v2._y, v1._z + v2._z);
        }

        /// <summary>
        ///     Overloads the subtraction operator to allow vectors to be subtracted from each other.
        /// </summary>
        /// <param name="v1">First factor.</param>
        /// <param name="v2">Second factor.</param>
        /// <returns>The result of subtracting v1 from v2.</returns>
        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1._x - v2._x, v1._y - v2._y, v1._z - v2._z);
        }

        /// <summary>
        ///     Overloads the multiply operator to allow vectors to be multiplied together.
        /// </summary>
        /// <param name="v1">First factor.</param>
        /// <param name="v2">Second factor.</param>
        /// <returns>The result of multiplying v1 by v2.</returns>
        public static Vector operator *(Vector v1, Vector v2)
        {
            return new Vector(v1._x * v2._x, v1._y * v2._y, v1._z * v2._z);
        }

        /// <summary>
        ///     Overloads the division operator to allow vectors to be divided by each other.
        /// </summary>
        /// <param name="v1">First factor.</param>
        /// <param name="v2">Second factor.</param>
        /// <returns>The result of dividing v1 by v2.</returns>
        public static Vector operator /(Vector v1, Vector v2)
        {
            return new Vector(v1._x / v2._x, v1._y / v2._y, v1._z / v2._z);
        }

        /// <summary>
        ///     Creates a new instance of this class with the given values.
        /// </summary>
        /// <param name="x">Position on the x axis.</param>
        /// <param name="y">Position on the y axis.</param>
        /// <param name="z">Position on the z axis.</param>
        /// <param name="tx">Position on the x axis of the texture.</param>
        /// <param name="ty">Position on the y axis of the texture.</param>
        public Vector(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

#endregion
    }
    
    /// <summary>
    ///     Allows a vector to be converted to and from textural values.
    /// </summary>
    public class VectorConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value as string == null)
                return base.ConvertFrom(context, culture, value); 
            
            string[] values = value.ToString().Split(',');
            if (values.Length != 3) 
                return base.ConvertFrom(context, culture, value);

            float tryResult;
            for (int i = 0; i < 3; i++)
                if (float.TryParse(values[i].Trim(), out tryResult) == false)
                    return base.ConvertFrom(context, culture, value);

            return new Vector(float.Parse(values[0].Trim()), float.Parse(values[1].Trim()), float.Parse(values[2].Trim()));
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value is Vector == false)
                return base.ConvertTo(context, culture, value, destinationType);

            return value.ToString();
        }
    }
    
    /// <summary>
    ///     Simple structure that holds minimum and maximum values as integers.
    /// </summary>
    public class MinMax
    {
        #region Members
        #region Variables

        private int _min, _max;

        #endregion
        #region Properties

        /// <summary>
        ///     Minimum value.
        /// </summary>
        public int Minimum
        {
            get { return _min; }
            set { _min = value; }
        }

        /// <summary>
        ///     Maximum valud.
        /// </summary>
        public int Maximum
        {
            get { return _max; }
            set { _max = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Converts this class to a textural format.
        /// </summary>
        /// <returns>Textural format of this class</returns>
        public override string ToString()
        {
            return _min + ", " + _max;
        }

        /// <summary>
        ///     Creates a new instance of this class with the given values.
        /// </summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public MinMax(int min, int max)
        {
            _min = min;
            _max = max;
        }

        #endregion
    }

    /// <summary>
    ///     Simple structure that holds minimum and maximum values as floats.
    /// </summary>
    public class MinMaxF
    {
        #region Members
        #region Variables

        private float _min, _max;

        #endregion
        #region Properties

        /// <summary>
        ///     Minimum value.
        /// </summary>
        public float Minimum
        {
            get { return _min; }
            set { _min = value; }
        }

        /// <summary>
        ///     Maximum valud.
        /// </summary>
        public float Maximum
        {
            get { return _max; }
            set { _max = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Converts this class to a textural format.
        /// </summary>
        /// <returns>Textural format of this class</returns>
        public override string ToString()
        {
            return _min + ", " + _max;
        }

        /// <summary>
        ///     Creates a new instance of this class with the given values.
        /// </summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public MinMaxF(float min, float max)
        {
            _min = min;
            _max = max;
        }

        #endregion
    }

    /// <summary>
    ///     Simple structure that holds start and finish values as integers.
    /// </summary>
    public class StartFinish
    {
        #region Members
        #region Variables

        private int _start, _finish;

        #endregion
        #region Properties

        /// <summary>
        ///     Start value.
        /// </summary>
        public int Start
        {
            get { return _start; }
            set { _start = value; }
        }

        /// <summary>
        ///     Finish valud.
        /// </summary>
        public int Finish
        {
            get { return _finish; }
            set { _finish = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Converts this class to a textural format.
        /// </summary>
        /// <returns>Textural format of this class</returns>
        public override string ToString()
        {
            return _start + ", " + _finish;
        }

        /// <summary>
        ///     Creates a new instance of this class with the given values.
        /// </summary>
        /// <param name="start">Start value.</param>
        /// <param name="finish">Finish value.</param>
        public StartFinish(int start, int finish)
        {
            _start = start;
            _finish = finish;
        }

        #endregion
    }

    /// <summary>
    ///     Simple structure that holds start and finish values as integers.
    /// </summary>
    public class StartFinishF
    {
        #region Members
        #region Variables

        private float _start, _finish;

        #endregion
        #region Properties

        /// <summary>
        ///     Start value.
        /// </summary>
        public float Start
        {
            get { return _start; }
            set { _start = value; }
        }

        /// <summary>
        ///     Finish valud.
        /// </summary>
        public float Finish
        {
            get { return _finish; }
            set { _finish = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Converts this class to a textural format.
        /// </summary>
        /// <returns>Textural format of this class</returns>
        public override string ToString()
        {
            return _start+", "+_finish;
        }

        /// <summary>
        ///     Creates a new instance of this class with the given values.
        /// </summary>
        /// <param name="start">Start value.</param>
        /// <param name="finish">Finish value.</param>
        public StartFinishF(float start, float finish)
        {
            _start = start;
            _finish = finish;
        }

        #endregion
    }

    /// <summary>
    ///     Simple structure that holds start and finish values as colors.
    /// </summary>
    public class StartFinishColor
    {
        #region Members
        #region Variables

        private Color _start, _finish;
        private byte _startAlpha, _finishAlpha;

        #endregion
        #region Properties

        /// <summary>
        ///     Start value.
        /// </summary>
        public Color Start
        {
            get { return _start; }
            set { _start = value; }
        }

        /// <summary>
        ///     Finish value.
        /// </summary>
        public Color Finish
        {
            get { return _finish; }
            set { _finish = value; }
        }

        /// <summary>
        ///     Start alpha value.
        /// </summary>
        [Category("Color"), DisplayName("Start Alpha")] 
        public byte StartAlpha
        {
            get { return _startAlpha; }
            set { _startAlpha = value; }
        }

        /// <summary>
        ///     Finish alpha value.
        /// </summary>
        [Category("Color"), DisplayName("Finish Alpha")] 
        public byte FinishAlpha
        {
            get { return _finishAlpha; }
            set { _finishAlpha = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Converts this class to a textural format.
        /// </summary>
        /// <returns>Textural format of this class</returns>
        public override string ToString()
        {
            return ColorMethods.CombineColor(ColorFormats.A8R8G8B8, _start.R, _start.G, _start.B, _startAlpha) + ", " + ColorMethods.CombineColor(ColorFormats.A8R8G8B8, _finish.R, _finish.G, _finish.B, _finishAlpha);
        }

        /// <summary>
        ///     Creates a new instance of this class with the given values.
        /// </summary>
        /// <param name="start">Start value.</param>
        /// <param name="finish">Finish value.</param>
        /// <param name="startAlpha">Start alpha value.</param>
        /// <param name="finishAlpha">Finish alpha value.</param>
        public StartFinishColor(Color start, Color finish, byte startAlpha, byte finishAlpha)
        {
            _start = start;
            _finish = finish;
            _startAlpha = startAlpha;
            _finishAlpha = finishAlpha;
        }

        #endregion
    }

    #endregion
}