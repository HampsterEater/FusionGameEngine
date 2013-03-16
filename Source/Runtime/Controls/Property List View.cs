/*
 * File: Property List View.cs
 *
 * Contains all the functional partial code declaration for the PropertyListView user control.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace BinaryPhoenix.Fusion.Runtime.Controls
{
	/// <summary>
	///		This class contains the code used to display and operate  
	///		the property list view control.
	/// </summary>
	public partial class PropertyListView : UserControl
	{
		#region Members
		#region Variables

        private PropertyListViewDescriptor _descriptor = null;

		private ArrayList _categories = new ArrayList();

        private string _defaultItem = null;

        private PropertyListViewGetValueDelegate _getValue = null;
        private PropertyListViewSetValueDelegate _setValue = null;

		#endregion
		#region Properties

        /// <summary>
        ///		Gets or sets the type descriptor of this property list.
        /// </summary>
        [Browsable(false), ReadOnly(true)]
        public PropertyListViewDescriptor Descriptor
        {
            get { return _descriptor; }
            set { _descriptor = value; }
        }

		/// <summary>
		///		Gets or sets the list of categories this property list view displays.
		/// </summary>
        [Browsable(false), ReadOnly(true)]
		public ArrayList Categories
		{
			get { return _categories; }
			set { _categories = value; }
		}

        /// <summary>
        ///		Gets or sets the default item of this property list view.
        /// </summary>
        [Browsable(false), ReadOnly(true)]
        public string DefaultItem
        {
            get { return _defaultItem; }
            set { _defaultItem = value; }
        }

        /// <summary>
        ///     Sets or gets the delegate invoked when the value of an item needs to be returned.
        /// </summary>
        [Browsable(false), ReadOnly(true)]
        public PropertyListViewGetValueDelegate GetValueDelegate
        {
            get { return _getValue; }
            set { _getValue = value; }
        }

        /// <summary>
        ///     Sets or gets the delegate invoked when the value of an item needs to be set.
        /// </summary>
        [Browsable(false), ReadOnly(true)]
        public PropertyListViewSetValueDelegate SetValueDelegate
        {
            get { return _setValue; }
            set { _setValue = value; }
        }

		#endregion
		#region Events

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Initializes a new instance of this class.
		/// </summary>
		public PropertyListView()
		{
			InitializeComponent();
            _descriptor = new PropertyListViewDescriptor(this);
        }

		/// <summary>
		///		Refreshs the view so that any changes made will be applied.
		/// </summary>
		public override void Refresh()
		{
            if (this.Parent.Visible == true && propertyGrid != null)
            propertyGrid.SelectedObject = null;
            propertyGrid.SelectedObject = _descriptor;
            propertyGrid.Refresh();
		}

		/// <summary>	
		///		Adds a new category to this control.
		/// </summary>
		/// <param name="category">Category to add.</param>
		public void AddCategory(PropertyListViewCategory category)
		{
			_categories.Add(category);
			category.Parent = this;
		}

		/// <summary>
		///		Removes a category from this control.
		/// </summary>
		/// <param name="category">Category to remove.</param>
		public void RemoveCategory(PropertyListViewCategory category)
		{
			_categories.Remove(category);
			category.Parent = null;;
		}

		/// <summary>
		///		Removes all categories from this control.
		/// </summary>
		public void Clear()
		{
			foreach (PropertyListViewCategory category in _categories)
            {
				foreach (PropertyListViewItem property in category.Properties)
					property.Parent = null;
				category.Parent = null;
			}
			_categories.Clear();
		}

		/// <summary>
		///		Enumerates all properties being shown in this control into a single array list.
		/// </summary>
		/// <returns>Array list containing all properties shown in this control.</returns>
		public ArrayList EnumerateProperties()
		{
			ArrayList properties = new ArrayList();
			foreach (PropertyListViewCategory category in _categories)
				foreach (PropertyListViewItem property in category.Properties)
					properties.Add(property);
			return properties;
		}

		#endregion
	}

    /// <summary>
    ///     Used to return the correct attributes to the reflector.
    /// </summary>
    public class PropertyListViewDescriptor : ICustomTypeDescriptor
    {
		#region Members
		#region Variables

        private PropertyListView _propertyListView = null;

		#endregion
		#region Properties

        /// <summary>
        ///     Gets or sets the property list view this descriptor is refering to.
        /// </summary>
        public PropertyListView PropertyListView
        {
            get { return _propertyListView; }
            set { _propertyListView = value; }
        }

		#endregion
		#region Events

		#endregion
		#endregion
		#region Methods

        #region Default return methods

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(_propertyListView, true);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(_propertyListView, true);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(_propertyListView, true);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(_propertyListView, true);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(_propertyListView, true);
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(_propertyListView, editorBaseType, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(_propertyListView, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(_propertyListView, attributes, true);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
        }

        #endregion

        /// <summary>
        ///     Gets the default property of this list view.
        /// </summary>
        /// <returns>Default property of this list view.</returns>
        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            PropertyListViewItem item = null;
            if (_propertyListView.DefaultItem != null)
                foreach (PropertyListViewCategory category in _propertyListView.Categories)
                {
                    foreach (PropertyListViewItem property in category.Properties)
                        if (property.Name == _propertyListView.DefaultItem)
                        {
                            item = property;
                            continue;
                        }
                    if (item != null) continue;
                }

            if (item != null)
                return new PropertyListViewItemDesciptor(item.Name, _propertyListView, item, null);
            else
                return null;
        }

        /// <summary>
        ///     Gets the properties of this list view.
        /// </summary>
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            ArrayList list = new ArrayList();
            foreach (PropertyListViewCategory category in _propertyListView.Categories)
            {
                foreach (PropertyListViewItem property in category.Properties)
                {
                    if (property.Name == "") continue;
                    ArrayList propertyAttributes = new ArrayList();

                    if (property.Category != null && property.Category.Name != null)
                        propertyAttributes.Add(new CategoryAttribute(property.Category.Name));

                    if (property.Description != null)
                        propertyAttributes.Add(new DescriptionAttribute(property.Description));

                    if (property.Editor != null)
                        propertyAttributes.Add(new EditorAttribute(property.Editor, typeof(UITypeEditor)));

                    if (property.TypeConverter != null)
                        propertyAttributes.Add(new TypeConverterAttribute(property.TypeConverter));

                    if (property.Attributes != null)
                        propertyAttributes.AddRange(property.Attributes);

                    list.Add(new PropertyListViewItemDesciptor(property.Name, _propertyListView, property, (Attribute[])propertyAttributes.ToArray(typeof(Attribute))));
                }
            }

            return new PropertyDescriptorCollection((PropertyDescriptor[])list.ToArray(typeof(PropertyDescriptor)));
        }

        /// <summary>
        ///     Initalizes a new instance of this class.
        /// </summary>
        /// <param name="propertyGrid">Property grid associated with this descriptor.</param>
        public PropertyListViewDescriptor(PropertyListView propertyGrid)
        {
            _propertyListView = propertyGrid;
        }

		#endregion
    }

	/// <summary>
	///		Describes a category used by the PropertyListView to arrange properties.
	/// </summary>
	public class PropertyListViewCategory
	{
		#region Members
		#region Variables

		private PropertyListView _parent = null;

		private string _name = "";

		private ArrayList _properties = new ArrayList();


		#endregion
		#region Properties

		/// <summary>
		///		Gets or sets the names displayed on this category.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		///		Gets or sets the list of properties this category owns.
		/// </summary>
		public ArrayList Properties
		{
			get { return _properties; }
			set { _properties = value; }
		}

		/// <summary>
		///		Gets or sets the list view that this category is owned by.
		/// </summary>
		public PropertyListView Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}

		#endregion
		#endregion
		#region Methods

		/// <summary>	
		///		Adds a new property to this category.
		/// </summary>
		/// <param name="node">Property to add.</param>
		public void AddProperty(PropertyListViewItem node)
		{
			_properties.Add(node);
			node.Parent = this;
		}

		/// <summary>	
		///		Removes a property from this category.
		/// </summary>
		/// <param name="node">Property to remove.</param>
        public void RemoveProperty(PropertyListViewItem node)
		{
			_properties.Remove(node);
			node.Parent = null;
		}

		/// <summary>
		///		Removes all the properties owned by this category.
		/// </summary>
		public void Clear()
		{
			foreach (PropertyListViewItem property in _properties)
				property.Parent = null;
			_properties.Clear();
		}

		/// <summary>
		///		Initializes a new instance of this class with the given name.
		/// </summary>	
		/// <param name="name">Name to give this category.</param>
		public PropertyListViewCategory(string name)
		{
			_name = name;
		}

		#endregion
	}

	/// <summary>
	///		Used by the PropertyListViewCategory class to store data about
	///		 a single property.
	/// </summary>
	public class PropertyListViewItem 
	{
		#region Members
		#region Variables

		private PropertyListViewCategory _parent = null;

		private string  _name = "";
		private object _value;
		private object _defaultValue;

        private string _description;

        private string _editor;
        private string _type;
        private string _typeConverter;

        private Attribute[] _attributes;

        private ArrayList _enumerationValues = null;
        private int _enumerationValue = 0;

		#endregion
		#region Properties

		/// <summary>
		///		Gets the category this property is attached to.
		/// </summary>
		public PropertyListViewCategory Category
		{
			get { return _parent; }
		}

		/// <summary>
		///		Gets or sets the name of this property.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>	
		///		Gets or sets the value associated with this property.
		/// </summary>
		public object Value
		{
			get { return _value; }
			set { _value = value; }
		}

		/// <summary>	
		///		Gets or sets the default value associated with this property.
		/// </summary>
		public object DefaultValue
		{
			get { return _defaultValue; }
			set { _defaultValue = value; }
		}

        /// <summary>	
        ///		Gets or sets the editor associated with this property.
        /// </summary>
        public string Editor
        {
            get { return _editor; }
            set { _editor = value; }
        }

        /// <summary>	
        ///		Gets or sets the description associated with this property.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>	
        ///		Gets or sets the type associated with this property.
        /// </summary>
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>	
        ///		Gets or sets the type converter associated with this property.
        /// </summary>
        public string TypeConverter
        {
            get { return _typeConverter; }
            set { _typeConverter = value; }
        }

        /// <summary>	
        ///		Gets or sets the attributes associated with this property.
        /// </summary>
        public Attribute[] Attributes
        {
            get { return _attributes; }
            set { _attributes = value; }
        }

		/// <summary>
		///		Gets or sets the category this property is owned by.
		/// </summary>
		public PropertyListViewCategory Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}

        /// <summary>
        ///     Gets or sets the list of enumeration values to use if this item is an enumeration.
        /// </summary>
        public ArrayList EnumerationValues
        {
            get { return _enumerationValues; }
            set { _enumerationValues = value; }
        }

        /// <summary>
        ///     Gets or sets the current enumeration value that is selected.
        /// </summary>
        public int EnumerationValue
        {
            get { return _enumerationValue; }
            set { _enumerationValue = value; }
        }

		#endregion
		#region Events

		#endregion
		#endregion
		#region Methods

		/// <summary>
		///		Initializes a new instance of this class with the given values.
		/// </summary>
		/// <param name="name">Name to give this property.</param>
		/// <param name="value">Value this property should store.</param>
        /// <param name="defaultValue">Default value of this property.</param>
        /// <param name="description">Description of this property.</param>
        /// <param name="type">Type of this properties value.</param>
        /// <param name="editor">Editor of this properties value.</param>
        public PropertyListViewItem(string name, object value, object defaultValue, string description, Type type, Type editor)
		{
			_name = name;
			_value = value;
            _defaultValue = defaultValue;
            _description = description;
            _type = type.AssemblyQualifiedName;
            _editor = editor.AssemblyQualifiedName;
		}
        public PropertyListViewItem(string name, object value, object defaultValue, string description, Type type)
        {
            _name = name;
            _value = value;
            _defaultValue = defaultValue;
            _description = description;
            _type = type.AssemblyQualifiedName;
        }

		#endregion
	}

    /// <summary>
    ///     Descripes a list view item to the reflector.
    /// </summary>
    public class PropertyListViewItemDesciptor : PropertyDescriptor
    {  
        #region Members
        #region Variables

        private PropertyListView _listView = null;
        private PropertyListViewItem _item = null;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the property list view this descriptor is associated with.
        /// </summary>
        public PropertyListView ListView
        {
            get { return _listView; }
            set { _listView = value; }
        }

        /// <summary>
        ///     Gets or sets the property item that this descriptor is describing.
        /// </summary>
        public PropertyListViewItem Item
        {
            get { return _item; }
            set { _item = value; }
        }

        /// <summary>
        ///     Gets the component type.
        /// </summary>
        public override Type ComponentType
		{
			get { return _item.GetType(); }
		}

        /// <summary>   
        ///     Gets if this item is read only or not.
        /// </summary>
		public override bool IsReadOnly
		{
            get { return (Attributes.Matches(ReadOnlyAttribute.Yes)); }
		}

        /// <summary>
        ///     Gets the property type.
        /// </summary>
		public override Type PropertyType
		{
			get { return Type.GetType(_item.Type); }
		}

		#endregion
		#region Events

		#endregion
		#endregion
		#region Methods

        #region Descriptor Method

        public override bool CanResetValue(object component)
		{
			if(_item.DefaultValue == null)
				return false;
			else
				return !this.GetValue(component).Equals(_item.DefaultValue);
		}

		public override object GetValue(object component)
		{
            if (_listView.GetValueDelegate != null) _listView.GetValueDelegate(_item);
            return _item.Value;
		}

		public override void ResetValue(object component)
		{
            _item.Value = _item.DefaultValue;
		}

		public override void SetValue(object component, object value)
		{
            _item.Value = value;
            if (_item.EnumerationValues != null)
            {
                int enumerationValue = 0;
                foreach (PropertyEnumerationEntry entry in _item.EnumerationValues)
                    if (entry.Name.ToLower() == value.ToString().ToLower())
                        enumerationValue = entry.Value;

                _item.EnumerationValue = enumerationValue;
            }
            if (_listView.SetValueDelegate != null) _listView.SetValueDelegate(_item, value);
		}

		public override bool ShouldSerializeValue(object component)
		{
			object val = this.GetValue(component);
			if(_item.DefaultValue == null && val == null)
				return false;
			else
				return !val.Equals(_item.DefaultValue);
        }

        #endregion

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        /// <param name="name">Name of this descriptor.</param>
        /// <param name="listView">List view associated with this descriptor.</param>
        /// <param name="item">Item to describe.</param>
        /// <param name="attributes">Attributes to use to describe item.</param>
        public PropertyListViewItemDesciptor(string name, PropertyListView listView, PropertyListViewItem item, Attribute[] attributes) : base(name, attributes)
        {
            _listView = listView;
            _item = item;
        }

        #endregion
    }

    /// <summary>
    ///     Used to gather a list of all current items within an enumeration entry.
    /// </summary>
    public class PropertyEnumerationConverter : StringConverter
    {
        #region Methods 

        /// <summary>
        ///     Returns the method of which the combo box list should be shown.
        /// </summary>
        /// <param name="context">Context which this method was called from.</param>
        /// <returns>Method in which combo box list should be shown.</returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            //true means show a combobox
            return true;
        }

        /// <summary>
        ///     Returns the method of which the combo box list should be shown.
        /// </summary>
        /// <param name="context">Context which this method was called from.</param>
        /// <returns>Method in which combo box list should be shown.</returns>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            //true will limit to list. false will show the list, 
            //but allow free-form entry
            return true;
        }

        /// <summary>
        ///     Gathers a list of all values in the enumeration.
        /// </summary>
        /// <param name="context">Context which this method was called from.</param>
        /// <returns>List of all value in enumeration.</returns>
        public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            ArrayList list = new ArrayList();
            foreach (PropertyEnumerationEntry entry in ((PropertyListViewItemDesciptor)context.PropertyDescriptor).Item.EnumerationValues)
                list.Add(entry.Name);
            return new StandardValuesCollection(list);
        }

        #endregion
    }

    /// <summary>
    ///     Used to describe an enumeration property entry.
    /// </summary>
    public class PropertyEnumerationEntry
    {
        #region Members
        #region Variables

        private string _name = "";
        private int _value = 0;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the name representing this enumeration entry.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        ///     Gets or sets the integer value of this enumeration entry.
        /// </summary>
        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Initializes a new instance of this class.
        /// </summary>
        /// <param name="name">Name representing this enumeration entry.</param>
        /// <param name="value">Value of this enumeration entry.</param>
        public PropertyEnumerationEntry(string name, int value)
        {
            _name = name;
            _value = value;
        }

        #endregion
    }


    /// <summary>
    ///     Used to notify an object that a property grid view wishs to set the value of an item.
    /// </summary>
    /// <param name="item">Item to set value of.</param>
    /// <param name="value">Value to set item to.</param>
    public delegate void PropertyListViewSetValueDelegate(PropertyListViewItem item, object value);
    
    /// <summary>
    ///     Used to notify an object that a property grid view wishs to get the value of an item.
    /// </summary>
    /// <param name="item">Item to get value of.</param>
    /// <returns>Value of item.</returns>
    public delegate object PropertyListViewGetValueDelegate(PropertyListViewItem item);
}
