/*
 * File: Emitter.cs
 *
 * Contains the main declaration of the emitter class
 * which is a derivitive of the SceneNode class and is
 * used to create complex particle effects.
 * 
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Drawing.Design;
using BinaryPhoenix.Fusion.Graphics;
using BinaryPhoenix.Fusion.Runtime;
using BinaryPhoenix.Fusion.Runtime.Resources;
using BinaryPhoenix.Fusion.Engine.Windows;
using BinaryPhoenix.Fusion.Runtime.Processes;
using BinaryPhoenix.Fusion.Engine.Processes;
using System.ComponentModel;

namespace BinaryPhoenix.Fusion.Engine.Entitys
{
	/// <summary>
	///		Used by the EmitterNode to determine what shape to emit particles in.
	/// </summary>
	public enum EmitShape
	{
		Rectangle,
		Circle
	}

    /// <summary>
    ///     Contains the details on a single particle type that can be emitted from an 
    ///     emitter node.
    /// </summary>
    public class ParticleType
    {
        #region Members
        #region Variables

        private EmitterNode _emitter = null;

        private MinMax _emitRate = new MinMax(100, 100);
        private int _cycleEmitRate = 1;
        private MinMax _emitCount = new MinMax(3, 5);

        private MinMax _particleLifeTimeRandom = new MinMax(0, 0);

        private EmitShape _emitShape = EmitShape.Circle;
        private HighPreformanceTimer _emitTimer = new HighPreformanceTimer();

        private ArrayList _modifiers = new ArrayList();
        private ArrayList _particles = new ArrayList();
        private ArrayList _deadParticles = new ArrayList();

        private HighPreformanceTimer _deadCleanUpTimer = new HighPreformanceTimer();

        private int _maximumCycles = 0, _cycleCount;

        #endregion
        #region Properties

        /// <summary>
        ///		Gets or sets the emitter this particle type is associated with.
        /// </summary>
        [Browsable(false), ReadOnly(true)]
        public EmitterNode Emitter
        {
            get { return _emitter; }
            set { _emitter = value; }
        }

        /// <summary>
        ///		Gets or sets the delay between each particle emission.
        /// </summary>
        [Category("Emission"), DisplayName("Emit Rate"), Description("Indicates the minimum and maximum period of time between each emission cycle."), TypeConverter(typeof(ExpandableObjectConverter))]
        public MinMax EmitRate
        {
            get { return _emitRate; }
            set { _emitRate = value; _cycleEmitRate = MathMethods.Random(value.Minimum, value.Maximum); }
        }

        /// <summary>
        ///		Gets or sets the amount of particle to emit on each each particle emission.
        /// </summary>
        [Category("Emission"), DisplayName("Emit Count"), Description("Indicates the minimum and maximum amount of particles that are released on each cycle."), TypeConverter(typeof(ExpandableObjectConverter))]
        public MinMax EmitCount
        {
            get { return _emitCount; }
            set { _emitCount = value; }
        }

        /// <summary>
        ///		Gets or sets the shape that particles should be emitted in.
        /// </summary>
        [Category("Emission"), DisplayName("Emit Shape"), Description("Indicates the shape in which the particles will be emitted.")]
        public EmitShape EmitShape
        {
            get { return _emitShape; }
            set { _emitShape = value; }
        }

        /// <summary>
        ///     Gets or sets the maximum cycles this type can process.
        /// </summary>
        [Category("Emission"), DisplayName("Maximum Cycles"), Description("Indicates the maximum amount of cycles this particle type is allowed to process..")]
        public int MaximumCycles
        {
            get { return _maximumCycles; }
            set { _maximumCycles = value; Restart(); }
        }

        /// <summary>
        ///     Gets or sets the maximum and minimum randomization of the life time of  
        ///     all particles created by this type.
        /// </summary>
        [Category("Particles"), DisplayName("Particle Life Time"), Description("Indicates the minimum and maximum life times of particles spawned by this particle type."), TypeConverter(typeof(ExpandableObjectConverter))]
        public MinMax ParticleLifeTimeRandom
        {
            get { return _particleLifeTimeRandom; }
            set { _particleLifeTimeRandom = value; }
        }

        /// <summary>
        ///     Gets or sets the list of modifiers applied to all particles controlled
        ///     by this particle type.
        /// </summary>
        [Browsable(false), ReadOnly(true)]
        public ArrayList Modifiers
        {
            get { return _modifiers; }
            set { _modifiers = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Disposes of an instance of this class.
        /// </summary>
        public void Dispose()
        {
            _deadParticles.Clear();
            ArrayList removeList = new ArrayList();
            foreach (ParticleNode particle in _particles)
            {
                _emitter.Particles.Remove(particle);
                removeList.Add(particle);
            }
            foreach (ParticleNode particle in removeList)
                _particles.Remove(particle);

            foreach (ParticleModifier modifier in _modifiers)
                modifier.ParticleType = null;
            _modifiers.Clear();

            _emitter = null;
        }

        /// <summary>
        ///		When this method is called it will create an exact copy of this parcticle type.
        /// </summary>
        /// <returns>Exact copy of this particle type.</returns>
        public ParticleType Clone()
        {
            ParticleType type = new ParticleType();
            this.CopyTo(type);
            return type;
        }

        /// <summary>
        ///		Copys all the data contained in this particle type to another particle type.
        /// </summary>
        /// <param name="type">Particle type to copy data into.</param>
        public void CopyTo(ParticleType type)
        {
            type._emitter = _emitter;
            type._emitRate = new MinMax(_emitRate.Minimum, _emitRate.Maximum);
            type._cycleEmitRate = _cycleEmitRate;
            type._emitCount = new MinMax(_emitCount.Minimum, _emitCount.Maximum);
            type._particleLifeTimeRandom = new MinMax(_particleLifeTimeRandom.Minimum, _particleLifeTimeRandom.Maximum);
            type._emitShape = _emitShape;
            type._maximumCycles = _maximumCycles;
            type._cycleCount = _cycleCount;

            foreach (ParticleModifier modifier in _modifiers)
            {
                ParticleModifier mod = modifier.Clone();
                mod.ParticleType = type;
                type._modifiers.Add(mod);
            }
        }

        /// <summary>
        ///     Registers a particle modifier with this emitter.
        /// </summary>
        /// <param name="mod">Particle modfifier to register.</param>
        public void RegisterParticleModifier(ParticleModifier mod)
        {
            _modifiers.Add(mod);
            mod.ParticleType = this;
        }

        /// <summary>
        ///     Unregisters a particle modifier with this emitter.
        /// </summary>
        /// <param name="mod">Particle modfifier to unregister.</param>
        public void UnregisterParticleModifier(ParticleModifier mod)
        {
            _modifiers.Remove(mod);
            mod.ParticleType = null;
        }

        /// <summary>
        ///     Resets this particle type.
        /// </summary>
        public void Restart()
        {
            _cycleCount = 0;
            _cycleEmitRate = MathMethods.Random(_emitRate.Minimum, _emitRate.Maximum);
            _emitTimer.Restart();
        }

        /// <summary>
        ///     Causes this particle type to update itself.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last frame.</param>
        public void Think(float deltaTime)
        {
            // Find any new dead particles.
            ArrayList removeList = new ArrayList();
            foreach (ParticleNode particle in _particles)
            {
                if (particle.IsDead == true)
                {
                    particle.IsVisible = false;
                    _deadParticles.Add(particle);
                    _emitter.Particles.Remove(particle);
                    removeList.Add(particle);
                }
            }
            foreach (ParticleNode particle in removeList)
                _particles.Remove(particle);

            // Remove dead particles from particle list.
            if (_deadCleanUpTimer.DurationMillisecond > 500)
            {
                _deadCleanUpTimer.Restart();
                _deadParticles.Clear();
            }

            // Create any new particles that need to be created.
            if (_emitTimer.DurationMillisecond > _cycleEmitRate && (_cycleCount < _maximumCycles || _maximumCycles == 0))
            {
                _cycleEmitRate = MathMethods.Random(_emitRate.Minimum, _emitRate.Maximum);
                _emitTimer.Restart();

                int emitCount = MathMethods.Random(_emitCount.Minimum, _emitCount.Maximum);
                for (int particleNumber = 0; particleNumber < emitCount; particleNumber++)
                {
                    ParticleNode particle = null;

                    // Recycle dead particle if we can.
                    if (_deadParticles.Count > 0)
                    {
                        particle = (ParticleNode)_deadParticles[0];
                        _deadParticles.Remove(particle);
                        particle.Reset();
                    }

                    // Create a new particle as we can't recycle.
                    if (particle == null)
                        particle = new ParticleNode();

                    // Position it according to emit shape.
                    float x = _emitter.Transformation.X;
                    float y = _emitter.Transformation.Y;
                    if (_emitShape == EmitShape.Circle)
                    {
                        float angle = MathMethods.Random(0, 360);
                        x += ((float)Math.Sin(angle) * MathMethods.Random(0, _emitter.BoundingRectangle.Width / 2)) + (_emitter.BoundingRectangle.Width / 2);
                        y += ((float)Math.Cos(angle) * MathMethods.Random(0, _emitter.BoundingRectangle.Height / 2)) + (_emitter.BoundingRectangle.Height / 2);
                    }
                    else if (_emitShape == EmitShape.Rectangle)
                    {
                        x += MathMethods.Random(0, _emitter.BoundingRectangle.Width);
                        y += MathMethods.Random(0, _emitter.BoundingRectangle.Height);
                    }

                    // Setup some basic settings.
                    particle.IsVisible = _emitter.IsVisible;
                    particle.LifeTime = MathMethods.Random(_particleLifeTimeRandom.Minimum, _particleLifeTimeRandom.Maximum);
                    particle.Position(x, y, _emitter.Transformation.Z);

                    // Add particle to lists.
                    _particles.Add(particle);
                    _emitter.Particles.Add(particle);
                }

                _cycleCount++;
            }

            // Apply modifiers to all alive particles.
            foreach (ParticleModifier modifier in _modifiers)
                foreach (ParticleNode particle in _particles)
                    modifier.Apply(particle, deltaTime);
        }

        /// <summary>
        ///		Saves this type into a given binary writer.
        /// </summary>
        /// <param name="writer">Binary writer to save this type into.</param>
        public void Save(BinaryWriter writer)
        {
            // General crap that needs saving.
            writer.Write(_maximumCycles);
            writer.Write((int)_emitShape);
            writer.Write(_particleLifeTimeRandom.Minimum);
            writer.Write(_particleLifeTimeRandom.Maximum);
            writer.Write(_emitCount.Minimum);
            writer.Write(_emitCount.Maximum);
            writer.Write(_emitRate.Minimum);
            writer.Write(_emitRate.Maximum);
            writer.Write(_modifiers.Count);

            // Save each modifier.
            foreach (ParticleModifier modifier in _modifiers)
                modifier.Save(writer);
        }

        /// <summary>
        ///		Loads this stype from a given binary reader.
        /// </summary>
        /// <param name="reader">Binary reader to load this type from.</param>
        public void Load(BinaryReader reader)
        {
            _maximumCycles = reader.ReadInt32();
            _emitShape = (EmitShape)reader.ReadInt32();
            _particleLifeTimeRandom = new MinMax(reader.ReadInt32(), reader.ReadInt32());
            _emitCount = new MinMax(reader.ReadInt32(), reader.ReadInt32());
            _emitRate = new MinMax(reader.ReadInt32(), reader.ReadInt32());
            
            int modifierCount = reader.ReadInt32();
            
            // Load each modifier.
            for (int i = 0; i < modifierCount; i++)
            {
                string modName = reader.ReadString();
                ParticleModifier modifier = (ParticleModifier)ReflectionMethods.CreateObject(modName);
                modifier.ParticleType = this;
                modifier.Load(reader);
                _modifiers.Add(modifier);
            }
        }

        /// <summary>
        ///     Invoked when a new instance of this class is created.
        /// </summary>
        /// <param name="emitRate">Minimum and maximum time period between emission cycles.</param>
        /// <param name="emitCount">Minimum and maximum amount of particles that are emitted each cycle.</param>
        /// <param name="emitShape">Shape in which particles are emitted.</param>
        /// <param name="maximumCycles">Maximum number of cycles this type can process.</param>
        /// <param name="particleLifeTimeRandom">Minimum and maximum randomization of life time.</param>
        public ParticleType(MinMax emitRate, MinMax emitCount, EmitShape emitShape, int maximumCycles, MinMax particleLifeTimeRandom)
        {
            _emitRate = emitRate;
            _emitCount = emitCount;
            _emitShape = emitShape;
            _maximumCycles = maximumCycles;
            _particleLifeTimeRandom = particleLifeTimeRandom;
            Restart();
        }
        public ParticleType() { }

        #endregion
    }

    /// <summary>
    ///     Used as the base for effect classes that apply specific effects to particles.
    /// </summary>
    public abstract class ParticleModifier
    {
        #region Members
        #region Variables

        protected ParticleType _particleType = null;

        protected StartFinish _effectDuration = new StartFinish(0, 100);

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the particle type this modifier is associated with.
        /// </summary>
        [Browsable(false), ReadOnly(true)]
        public ParticleType ParticleType
        {
            get { return _particleType; }
            set { _particleType = value; }
        }

        /// <summary>
        ///     Gets the time (as a percentage of life) at which this effect starts being applied to a particle.
        /// </summary>
        [Category("Modifier"), DisplayName("Effect Duration"), Description("Indicates the start and end times betwen which this modifier effects particles. Time is in a percentage of life span."), TypeConverter(typeof(ExpandableObjectConverter))]
        public StartFinish EffectDuration
        {
            get { return _effectDuration; }
            set { _effectDuration = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///		When this method is called it will create an exact copy of this particle modifier.
        /// </summary>
        /// <returns>Exact copy of this particle modifier.</returns>
        public abstract ParticleModifier Clone();

        /// <summary>
        ///		Copys all the data contained in this particle modifier to another particle modifier.
        /// </summary>
        /// <param name="mod">Particle modifier to copy data into.</param>
        public virtual void CopyTo(ParticleModifier mod)
        {
            mod.EffectDuration = new StartFinish(_effectDuration.Start, _effectDuration.Finish);
        }

        /// <summary>
        ///     Gets or sets if this modifier is active on the give node.
        /// </summary>
        /// <param name="node">Node to check activity against.</param>
        protected bool Active(ParticleNode node)
        {
            float percent = (100.0f / (float)node.LifeTime) * (float)node.ExpiredLife; 
            return (percent >= _effectDuration.Start && percent <= _effectDuration.Finish);
        }

        /// <summary>
        ///     Applies this modifiers effect to the given particle node.
        /// </summary>
        /// <param name="node">Node to apply effect to.</param>
        /// <param name="deltaTime">Time elapsed since last frame.</param>
        public abstract void Apply(ParticleNode node, float deltaTime);

        /// <summary>
        ///		Saves this modifier into a given binary writer.
        /// </summary>
        /// <param name="writer">Binary writer to save this modifier into.</param>
        public virtual void Save(BinaryWriter writer)
        {
            // Make sure to save the name :D, otherwise we won't be
            // able to recreate it when we load.
            writer.Write(GetType().FullName);

            writer.Write(_effectDuration.Start);
            writer.Write(_effectDuration.Finish);
        }

        /// <summary>
        ///		Loads this modifier from a given binary reader.
        /// </summary>
        /// <param name="reader">Binary reader to load this modifier from.</param>
        public virtual void Load(BinaryReader reader)
        {
            _effectDuration = new StartFinish(reader.ReadInt32(), reader.ReadInt32());
        }

        #endregion
    }

    /// <summary>
    ///     Used to apply an acceleration effect to a particle.
    /// </summary>
    public class ParticleAccelerationModifier : ParticleModifier
    {
        #region Members
        #region Variables

        private StartFinishF _xAcceleration = new StartFinishF(0.0f, 0.0f);
        private StartFinishF _yAcceleration = new StartFinishF(0.0f, 0.0f);
        private MinMaxF _xAccelerationRandom = new MinMaxF(0.0f, 0.0f);
        private MinMaxF _yAccelerationRandom = new MinMaxF(0.0f, 0.0f);

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the start and end acceleration randomizations (on the x axis) in the form of an array.
        /// </summary>
        [Category("Acceleration"), DisplayName("X-Axis Randomization"), Description("Indicates the minimum and maximum randomization of acceleration of particles on the x-axis."), TypeConverter(typeof(ExpandableObjectConverter))]
        public MinMaxF XAccelerationRandom
        {
            get { return _xAccelerationRandom; }
            set { _xAccelerationRandom = value; }
        }

        /// <summary>
        ///     Gets or sets the start and end acceleration randomizations (on the y axis) in the form of an array.
        /// </summary>
        [Category("Acceleration"), DisplayName("Y-Axis Randomization"), Description("Indicates the minimum and maximum randomization of acceleration of particles on the y-axis."), TypeConverter(typeof(ExpandableObjectConverter))]
        public MinMaxF YAccelerationRandom
        {
            get { return _yAccelerationRandom; }
            set { _yAccelerationRandom = value; }
        }

        /// <summary>
        ///     Gets or sets the start and end accelerations (on the x axis) in the form of an array.
        /// </summary>
        [Category("Acceleration"), DisplayName("X-Axis"), Description("Indicates the starting and ending acceleration of particles on the x-axis."), TypeConverter(typeof(ExpandableObjectConverter))]
        public StartFinishF XAcceleration
        {
            get { return _xAcceleration; }
            set { _xAcceleration = value; }
        }

        /// <summary>
        ///     Gets or sets the start and end accelerations (on the y axis) in the form of an array.
        /// </summary>
        [Category("Acceleration"), DisplayName("Y-Axis"), Description("Indicates the starting and ending acceleration of particles on the y-axis."), TypeConverter(typeof(ExpandableObjectConverter))]
        public StartFinishF YAcceleration
        {
            get { return _yAcceleration; }
            set { _yAcceleration = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Converts this object into a textural format.
        /// </summary>
        /// <returns>Textural version of this object.</returns>
        public override string ToString()
        {
            return "Acceleration Modifier";
        }

        /// <summary>
        ///		When this method is called it will create an exact copy of this particle modifier.
        /// </summary>
        /// <returns>Exact copy of this particle modifier.</returns>
        public override ParticleModifier Clone()
        {
            ParticleModifier mod = new ParticleAccelerationModifier();
            CopyTo(mod);
            return mod;
        }

        /// <summary>
        ///		Copys all the data contained in this particle modifier to another particle modifier.
        /// </summary>
        /// <param name="mod">Particle modifier to copy data into.</param>
        public override void CopyTo(ParticleModifier mod)
        {
            base.CopyTo(mod);

            ParticleAccelerationModifier accMod = (ParticleAccelerationModifier)mod;
            accMod._xAcceleration = new StartFinishF(_xAcceleration.Start, _xAcceleration.Finish);
            accMod._xAccelerationRandom = new MinMaxF(_xAccelerationRandom.Minimum, _xAccelerationRandom.Maximum);
            accMod._yAcceleration = new StartFinishF(_yAcceleration.Start, _yAcceleration.Finish);
            accMod._yAccelerationRandom = new MinMaxF(_yAccelerationRandom.Minimum, _yAccelerationRandom.Maximum);
        }

        /// <summary>
        ///     Applies this modifiers effect to the given particle node.
        /// </summary>
        /// <param name="node">Node to apply effect to.</param>
        /// <param name="deltaTime">Time elapsed since last frame.</param>
        public override void Apply(ParticleNode node, float deltaTime)
        {
            // Check that we are active on this particle.
            if (Active(node) == false) return;
            
            // Couple of useful variables.
            float onePercent = (100.0f / (float)node.LifeTime);
            float duration = (_effectDuration.Finish - _effectDuration.Start);
            float expired = (onePercent * (float)node.ExpiredLife) - _effectDuration.Start;
            float xRand = _xAccelerationRandom.Minimum + ((_xAccelerationRandom.Maximum - _xAccelerationRandom.Minimum) * node.RandomNumbers[0]);
            float yRand = _yAccelerationRandom.Minimum + ((_yAccelerationRandom.Maximum - _yAccelerationRandom.Minimum) * node.RandomNumbers[1]);

            // Move the particle.
            float accX = (float)(_xAcceleration.Start + (((_xAcceleration.Finish - _xAcceleration.Start) / duration) * expired));
            float accY = (float)(_yAcceleration.Start + (((_yAcceleration.Finish - _yAcceleration.Start) / duration) * expired));
            node.Move((accX + xRand) * deltaTime, (accY + yRand) * deltaTime, 0);
        }

        /// <summary>
        ///		Saves this modifier into a given binary writer.
        /// </summary>
        /// <param name="writer">Binary writer to save this modifier into.</param>
        public override void Save(BinaryWriter writer)
        {
            // All your bases are belong to us!
            base.Save(writer);
        
            // Save properties.
            writer.Write(_xAcceleration.Start);
            writer.Write(_xAcceleration.Finish);
            writer.Write(_xAccelerationRandom.Minimum);
            writer.Write(_xAccelerationRandom.Maximum);
            writer.Write(_yAcceleration.Start);
            writer.Write(_yAcceleration.Finish);
            writer.Write(_yAccelerationRandom.Minimum);
            writer.Write(_yAccelerationRandom.Maximum);   
        }

        /// <summary>
        ///		Loads this modifier from a given binary reader.
        /// </summary>
        /// <param name="reader">Binary reader to load this modifier from.</param>
        public override void Load(BinaryReader reader)
        {
            // Load ze base!
            base.Load(reader);

            // Load properties.
            _xAcceleration = new StartFinishF(reader.ReadSingle(), reader.ReadSingle());
            _xAccelerationRandom = new MinMaxF(reader.ReadSingle(), reader.ReadSingle());
            _yAcceleration = new StartFinishF(reader.ReadSingle(), reader.ReadSingle());
            _yAccelerationRandom = new MinMaxF(reader.ReadSingle(), reader.ReadSingle());
        }

        /// <summary>
        ///     Invoked when a new instance of this class is created.
        /// </summary>
        /// <param name="effectDuration">The start and finish time which the type effects particles in between.</param>
        /// <param name="xAcceleration">Acceleration on the x-axis.</param>
        /// <param name="yAcceleration">Acceleration on the y-axis.</param>
        /// <param name="xAccelerationRandom">Minimum and maximum randomization of acceleration on the x axis in the form of an array.</param>
        /// <param name="yAccelerationRandom">Minimum and maximum randomization of acceleration on the y axis in the form of an array.</param>
        public ParticleAccelerationModifier(StartFinish effectDuration, StartFinishF xAcceleration, StartFinishF yAcceleration, MinMaxF xAccelerationRandom, MinMaxF yAccelerationRandom)
        {
            _effectDuration = effectDuration;
            _xAcceleration = xAcceleration;
            _yAcceleration = yAcceleration;
            _xAccelerationRandom = xAccelerationRandom;
            _yAccelerationRandom = yAccelerationRandom;
        }
        public ParticleAccelerationModifier() { }

        #endregion
    }

    /// <summary>
    ///     Used to apply an scale effect to a particle.
    /// </summary>
    public class ParticleScaleModifier : ParticleModifier
    {
        #region Members
        #region Variables

        private StartFinishF _xScale = new StartFinishF(1.0f, 1.0f);
        private StartFinishF _yScale = new StartFinishF(1.0f, 1.0f);
        private StartFinishF _zScale = new StartFinishF(1.0f, 1.0f);
        private MinMaxF _xScaleRandom = new MinMaxF(0.0f, 0.0f);
        private MinMaxF _yScaleRandom = new MinMaxF(0.0f, 0.0f);
        private MinMaxF _zScaleRandom = new MinMaxF(0.0f, 0.0f);

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the start and end scale randomizations (on the x axis) in the form of an array.
        /// </summary>
        [Category("Scale"), DisplayName("X-Axis Randomization"), Description("Indicates the minimum and maximum randomization of the scale of particles on the x-axis."), TypeConverter(typeof(ExpandableObjectConverter))]
        public MinMaxF XScaleRandom
        {
            get { return _xScaleRandom; }
            set { _xScaleRandom = value; }
        }

        /// <summary>
        ///     Gets or sets the start and end scale randomizations (on the y axis) in the form of an array.
        /// </summary>
        [Category("Scale"), DisplayName("Y-Axis Randomization"), Description("Indicates the minimum and maximum randomization of the scale of particles on the y-axis."), TypeConverter(typeof(ExpandableObjectConverter))]
        public MinMaxF YScaleRandom
        {
            get { return _yScaleRandom; }
            set { _yScaleRandom = value; }
        }

        /// <summary>
        ///     Gets or sets the start and end scale randomizations (on the z axis) in the form of an array.
        /// </summary>
        [Category("Scale"), DisplayName("Z-Axis Randomization"), Description("Indicates the minimum and maximum randomization of the scale of particles on the z-axis."), TypeConverter(typeof(ExpandableObjectConverter))]
        public MinMaxF ZScaleRandom
        {
            get { return _zScaleRandom; }
            set { _zScaleRandom = value; }
        }

        /// <summary>
        ///     Gets or sets the start and end Scale (on the x axis) in the form of an array.
        /// </summary>
        [Category("Scale"), DisplayName("X-Axis"), Description("Indicates the starting and ending scale of particles on the x-axis."), TypeConverter(typeof(ExpandableObjectConverter))]
        public StartFinishF XScale
        {
            get { return _xScale; }
            set { _xScale = value; }
        }

        /// <summary>
        ///     Gets or sets the start and end Scale (on the y axis) in the form of an array.
        /// </summary>
        [Category("Scale"), DisplayName("Y-Axis"), Description("Indicates the starting and ending scale of particles on the y-axis."), TypeConverter(typeof(ExpandableObjectConverter))]
        public StartFinishF YScale
        {
            get { return _yScale; }
            set { _yScale = value; }
        }

        /// <summary>
        ///     Gets or sets the start and end Scale (on the y axis) in the form of an array.
        /// </summary>
        [Category("Scale"), DisplayName("Z-Axis"), Description("Indicates the starting and ending scale of particles on the z-axis."), TypeConverter(typeof(ExpandableObjectConverter))]
        public StartFinishF ZScale
        {
            get { return _zScale; }
            set { _zScale = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Converts this object into a textural format.
        /// </summary>
        /// <returns>Textural version of this object.</returns>
        public override string ToString()
        {
            return "Scale Modifier";
        }

        /// <summary>
        ///		When this method is called it will create an exact copy of this particle modifier.
        /// </summary>
        /// <returns>Exact copy of this particle modifier.</returns>
        public override ParticleModifier Clone()
        {
            ParticleModifier mod = new ParticleScaleModifier();
            CopyTo(mod);
            return mod;
        }

        /// <summary>
        ///		Copys all the data contained in this particle modifier to another particle modifier.
        /// </summary>
        /// <param name="mod">Particle modifier to copy data into.</param>
        public override void CopyTo(ParticleModifier mod)
        {
            base.CopyTo(mod);

            ParticleScaleModifier scaleMod = (ParticleScaleModifier)mod;
            scaleMod._xScale = new StartFinishF(_xScale.Start, _xScale.Finish);
            scaleMod._xScaleRandom = new MinMaxF(_xScaleRandom.Minimum, _xScaleRandom.Maximum);
            scaleMod._yScale = new StartFinishF(_yScale.Start, _yScale.Finish);
            scaleMod._yScaleRandom = new MinMaxF(_yScaleRandom.Minimum, _yScaleRandom.Maximum);
            scaleMod._zScale = new StartFinishF(_zScale.Start, _zScale.Finish);
            scaleMod._zScaleRandom = new MinMaxF(_zScaleRandom.Minimum, _zScaleRandom.Maximum);
        }

        /// <summary>
        ///     Applies this modifiers effect to the given particle node.
        /// </summary>
        /// <param name="node">Node to apply effect to.</param>
        /// <param name="deltaTime">Time elapsed since last frame.</param>
        public override void Apply(ParticleNode node, float deltaTime)
        {
            // Check that we are active on this particle.
            if (Active(node) == false) return;

            // Couple of useful variables.
            float onePercent = (100.0f / (float)node.LifeTime);
            float duration = (_effectDuration.Finish - _effectDuration.Start);
            float expired = (onePercent * (float)node.ExpiredLife) - _effectDuration.Start;
            float xRand = _xScaleRandom.Minimum + ((_xScaleRandom.Maximum - _xScaleRandom.Minimum) * node.RandomNumbers[6]);
            float yRand = _yScaleRandom.Minimum + ((_yScaleRandom.Maximum - _yScaleRandom.Minimum) * node.RandomNumbers[7]);
            float zRand = _zScaleRandom.Minimum + ((_zScaleRandom.Maximum - _zScaleRandom.Minimum) * node.RandomNumbers[7]);

            // Move the particle.
            float accX = (float)(_xScale.Start + (((_xScale.Finish - _xScale.Start) / duration) * expired));
            float accY = (float)(_yScale.Start + (((_yScale.Finish - _yScale.Start) / duration) * expired));
            float accZ = (float)(_zScale.Start + (((_zScale.Finish - _zScale.Start) / duration) * expired));
            node.Scale(accX + xRand, accY + yRand, accZ + zRand);
        }

        /// <summary>
        ///		Saves this modifier into a given binary writer.
        /// </summary>
        /// <param name="writer">Binary writer to save this modifier into.</param>
        public override void Save(BinaryWriter writer)
        {
            // All your bases are belong to us!
            base.Save(writer);

            // Save properties.
            writer.Write(_xScale.Start);
            writer.Write(_xScale.Finish);
            writer.Write(_xScaleRandom.Minimum);
            writer.Write(_xScaleRandom.Maximum);
            writer.Write(_yScale.Start);
            writer.Write(_yScale.Finish);
            writer.Write(_yScaleRandom.Minimum);
            writer.Write(_yScaleRandom.Maximum);
            writer.Write(_zScale.Start);
            writer.Write(_zScale.Finish);
            writer.Write(_zScaleRandom.Minimum);
            writer.Write(_zScaleRandom.Maximum);
        }

        /// <summary>
        ///		Loads this modifier from a given binary reader.
        /// </summary>
        /// <param name="reader">Binary reader to load this modifier from.</param>
        public override void Load(BinaryReader reader)
        {
            // Load ze base!
            base.Load(reader);

            // Load properties.
            _xScale = new StartFinishF(reader.ReadSingle(), reader.ReadSingle());
            _xScaleRandom = new MinMaxF(reader.ReadSingle(), reader.ReadSingle());
            _yScale = new StartFinishF(reader.ReadSingle(), reader.ReadSingle());
            _yScaleRandom = new MinMaxF(reader.ReadSingle(), reader.ReadSingle());
            _zScale = new StartFinishF(reader.ReadSingle(), reader.ReadSingle());
            _zScaleRandom = new MinMaxF(reader.ReadSingle(), reader.ReadSingle());
        }

        /// <summary>
        ///     Invoked when a new instance of this class is created.
        /// </summary>
        /// <param name="effectDuration">The start and finish time which the type effects particles in between.</param>
        /// <param name="xScale">Scale on the x-axis.</param>
        /// <param name="yScale">Scale on the y-axis.</param>
        /// <param name="zScale">Scale on the z-axis.</param>
        /// <param name="xScaleRandom">Minimum and maximum randomization of scale on the x axis in the form of an array.</param>
        /// <param name="yScaleRandom">Minimum and maximum randomization of scale on the y axis in the form of an array.</param>
        /// <param name="zScaleRandom">Minimum and maximum randomization of scale on the z axis in the form of an array.</param>
        public ParticleScaleModifier(StartFinish effectDuration, StartFinishF xScale, StartFinishF yScale, StartFinishF zScale, MinMaxF xScaleRandom, MinMaxF yScaleRandom, MinMaxF zScaleRandom)
        {
            _effectDuration = effectDuration;
            _xScale = xScale;
            _yScale = yScale;
            _zScale = zScale;
            _xScaleRandom = xScaleRandom;
            _yScaleRandom = yScaleRandom;
            _zScaleRandom = zScaleRandom;
        }
        public ParticleScaleModifier() { }

        #endregion
    }

    /// <summary>
    ///     Used to apply an rotation effect to a particle.
    /// </summary>
    public class ParticleRotationModifier : ParticleModifier
    {
        #region Members
        #region Variables

        private StartFinishF _xRotation = new StartFinishF(0.0f, 0.0f);
        private StartFinishF _yRotation = new StartFinishF(0.0f, 0.0f);
        private StartFinishF _zRotation = new StartFinishF(0.0f, 0.0f);
        private MinMaxF _xRotationRandom = new MinMaxF(0.0f, 0.0f);
        private MinMaxF _yRotationRandom = new MinMaxF(0.0f, 0.0f);
        private MinMaxF _zRotationRandom = new MinMaxF(0.0f, 0.0f);

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the start and end Rotation randomizations (on the x axis) in the form of an array.
        /// </summary>
        [Category("Rotation"), DisplayName("X-Axis Randomization"), Description("Indicates the minimum and maximum randomization of the scale of particles on the x-axis."), TypeConverter(typeof(ExpandableObjectConverter))]
        public MinMaxF XRotationRandom
        {
            get { return _xRotationRandom; }
            set { _xRotationRandom = value; }
        }

        /// <summary>
        ///     Gets or sets the start and end Rotation randomizations (on the y axis) in the form of an array.
        /// </summary>
        [Category("Rotation"), DisplayName("Y-Axis Randomization"), Description("Indicates the minimum and maximum randomization of the scale of particles on the y-axis."), TypeConverter(typeof(ExpandableObjectConverter))]
        public MinMaxF YRotationRandom
        {
            get { return _yRotationRandom; }
            set { _yRotationRandom = value; }
        }

        /// <summary>
        ///     Gets or sets the start and end Rotation randomizations (on the z axis) in the form of an array.
        /// </summary>
        [Category("Rotation"), DisplayName("Z-Axis Randomization"), Description("Indicates the minimum and maximum randomization of the scale of particles on the z-axis."), TypeConverter(typeof(ExpandableObjectConverter))]
        public MinMaxF ZRotationRandom
        {
            get { return _zRotationRandom; }
            set { _zRotationRandom = value; }
        }

        /// <summary>
        ///     Gets or sets the start and end Rotation (on the x axis) in the form of an array.
        /// </summary>
        [Category("Rotation"), DisplayName("X-Axis"), Description("Indicates the starting and ending scale of particles on the x-axis."), TypeConverter(typeof(ExpandableObjectConverter))]
        public StartFinishF XRotation
        {
            get { return _xRotation; }
            set { _xRotation = value; }
        }

        /// <summary>
        ///     Gets or sets the start and end Rotation (on the y axis) in the form of an array.
        /// </summary>
        [Category("Rotation"), DisplayName("Y-Rotation"), Description("Indicates the starting and ending scale of particles on the y-axis."), TypeConverter(typeof(ExpandableObjectConverter))]
        public StartFinishF YRotation
        {
            get { return _yRotation; }
            set { _yRotation = value; }
        }

        /// <summary>
        ///     Gets or sets the start and end Rotation (on the y axis) in the form of an array.
        /// </summary>
        [Category("Rotation"), DisplayName("Z-Axis"), Description("Indicates the starting and ending scale of particles on the z-axis."), TypeConverter(typeof(ExpandableObjectConverter))]
        public StartFinishF ZRotation
        {
            get { return _zRotation; }
            set { _zRotation = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Converts this object into a textural format.
        /// </summary>
        /// <returns>Textural version of this object.</returns>
        public override string ToString()
        {
            return "Rotation Modifier";
        }

        /// <summary>
        ///		When this method is called it will create an exact copy of this particle modifier.
        /// </summary>
        /// <returns>Exact copy of this particle modifier.</returns>
        public override ParticleModifier Clone()
        {
            ParticleModifier mod = new ParticleRotationModifier();
            CopyTo(mod);
            return mod;
        }

        /// <summary>
        ///		Copys all the data contained in this particle modifier to another particle modifier.
        /// </summary>
        /// <param name="mod">Particle modifier to copy data into.</param>
        public override void CopyTo(ParticleModifier mod)
        {
            base.CopyTo(mod);

            ParticleRotationModifier scaleMod = (ParticleRotationModifier)mod;
            scaleMod._xRotation = new StartFinishF(_xRotation.Start, _xRotation.Finish);
            scaleMod._xRotationRandom = new MinMaxF(_xRotationRandom.Minimum, _xRotationRandom.Maximum);
            scaleMod._yRotation = new StartFinishF(_yRotation.Start, _yRotation.Finish);
            scaleMod._yRotationRandom = new MinMaxF(_yRotationRandom.Minimum, _yRotationRandom.Maximum);
            scaleMod._zRotation = new StartFinishF(_zRotation.Start, _zRotation.Finish);
            scaleMod._zRotationRandom = new MinMaxF(_zRotationRandom.Minimum, _zRotationRandom.Maximum);
        }

        /// <summary>
        ///     Applies this modifiers effect to the given particle node.
        /// </summary>
        /// <param name="node">Node to apply effect to.</param>
        /// <param name="deltaTime">Time elapsed since last frame.</param>
        public override void Apply(ParticleNode node, float deltaTime)
        {
            // Check that we are active on this particle.
            if (Active(node) == false) return;

            // Couple of useful variables.
            float onePercent = (100.0f / (float)node.LifeTime);
            float duration = (_effectDuration.Finish - _effectDuration.Start);
            float expired = (onePercent * (float)node.ExpiredLife) - _effectDuration.Start;
            float xRand = _xRotationRandom.Minimum + ((_xRotationRandom.Maximum - _xRotationRandom.Minimum) * node.RandomNumbers[6]);
            float yRand = _yRotationRandom.Minimum + ((_yRotationRandom.Maximum - _yRotationRandom.Minimum) * node.RandomNumbers[7]);
            float zRand = _zRotationRandom.Minimum + ((_zRotationRandom.Maximum - _zRotationRandom.Minimum) * node.RandomNumbers[7]);

            // Move the particle.
            float accX = (float)(_xRotation.Start + (((_xRotation.Finish - _xRotation.Start) / duration) * expired));
            float accY = (float)(_yRotation.Start + (((_yRotation.Finish - _yRotation.Start) / duration) * expired));
            float accZ = (float)(_zRotation.Start + (((_zRotation.Finish - _zRotation.Start) / duration) * expired));
            node.Rotate(accX + xRand, accY + yRand, accZ + zRand);
        }

        /// <summary>
        ///		Saves this modifier into a given binary writer.
        /// </summary>
        /// <param name="writer">Binary writer to save this modifier into.</param>
        public override void Save(BinaryWriter writer)
        {
            // All your bases are belong to us!
            base.Save(writer);

            // Save properties.
            writer.Write(_xRotation.Start);
            writer.Write(_xRotation.Finish);
            writer.Write(_xRotationRandom.Minimum);
            writer.Write(_xRotationRandom.Maximum);
            writer.Write(_yRotation.Start);
            writer.Write(_yRotation.Finish);
            writer.Write(_yRotationRandom.Minimum);
            writer.Write(_yRotationRandom.Maximum);
            writer.Write(_zRotation.Start);
            writer.Write(_zRotation.Finish);
            writer.Write(_zRotationRandom.Minimum);
            writer.Write(_zRotationRandom.Maximum);
        }

        /// <summary>
        ///		Loads this modifier from a given binary reader.
        /// </summary>
        /// <param name="reader">Binary reader to load this modifier from.</param>
        public override void Load(BinaryReader reader)
        {
            // Load ze base!
            base.Load(reader);

            // Load properties.
            _xRotation = new StartFinishF(reader.ReadSingle(), reader.ReadSingle());
            _xRotationRandom = new MinMaxF(reader.ReadSingle(), reader.ReadSingle());
            _yRotation = new StartFinishF(reader.ReadSingle(), reader.ReadSingle());
            _yRotationRandom = new MinMaxF(reader.ReadSingle(), reader.ReadSingle());
            _zRotation = new StartFinishF(reader.ReadSingle(), reader.ReadSingle());
            _zRotationRandom = new MinMaxF(reader.ReadSingle(), reader.ReadSingle());
        }

        /// <summary>
        ///     Invoked when a new instance of this class is created.
        /// </summary>
        /// <param name="effectDuration">The start and finish time which the type effects particles in between.</param>
        /// <param name="xRotation">Rotation on the x-axis.</param>
        /// <param name="yRotation">Rotation on the y-axis.</param>
        /// <param name="zRotation">Rotation on the z-axis.</param>
        /// <param name="xScaleRotation">Minimum and maximum randomization of Rotation on the x axis in the form of an array.</param>
        /// <param name="yScaleRotation">Minimum and maximum randomization of Rotation on the y axis in the form of an array.</param>
        /// <param name="zScaleRotation">Minimum and maximum randomization of Rotation on the z axis in the form of an array.</param>
        public ParticleRotationModifier(StartFinish effectDuration, StartFinishF xRotation, StartFinishF yRotation, StartFinishF zRotation, MinMaxF xRotationRandom, MinMaxF yRotationRandom, MinMaxF zRotationRandom)
        {
            _effectDuration = effectDuration;
            _xRotation = xRotation;
            _yRotation = yRotation;
            _zRotation = zRotation;
            _xRotationRandom = xRotationRandom;
            _yRotationRandom = yRotationRandom;
            _zRotationRandom = zRotationRandom;
        }
        public ParticleRotationModifier() { }

        #endregion
    }

    /// <summary>
    ///     Used to apply a color tinting effect to a particle.
    /// </summary>
    public class ParticleColorModifier : ParticleModifier
    {
        #region Members
        #region Variables

        private StartFinishColor _colorTint = new StartFinishColor(Color.White, Color.Black, 255, 0);
        private MinMax _colorTintRedRandom = new MinMax(0, 0);
        private MinMax _colorTintGreenRandom = new MinMax(0, 0);
        private MinMax _colorTintBlueRandom = new MinMax(0, 0);
        private MinMax _colorTintAlphaRandom = new MinMax(0, 0);

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the start and end color tint in the form of an array.
        /// </summary>
        [Category("Color"), DisplayName("Color Tint"), Description("Indicates the starting and ending color tint of particles."), TypeConverter(typeof(ExpandableObjectConverter))] 
        public StartFinishColor ColorTint
        {
            get { return _colorTint; }
            set { _colorTint = value; }
        }

        /// <summary>
        ///     Gets or sets the minimum and maximum randomization of red factor in the color tint.
        /// </summary>
        [Category("Color"), DisplayName("Red Randomization"), Description("Indicates the minimum and maximum randomization of the red factor in the color tint."), TypeConverter(typeof(ExpandableObjectConverter))]
        public MinMax ColorTintRedRandom
        {
            get { return _colorTintRedRandom; }
            set { _colorTintRedRandom = value; }
        }

        /// <summary>
        ///     Gets or sets the minimum and maximum randomization of green factor in the color tint.
        /// </summary>
        [Category("Color"), DisplayName("Green Randomization"), Description("Indicates the minimum and maximum randomization of the green factor in the color tint."), TypeConverter(typeof(ExpandableObjectConverter))]
        public MinMax ColorTintGreenRandom
        {
            get { return _colorTintGreenRandom; }
            set { _colorTintGreenRandom = value; }
        }

        /// <summary>
        ///     Gets or sets the minimum and maximum randomization of blue factor in the color tint.
        /// </summary>
        [Category("Color"), DisplayName("Blue Randomization"), Description("Indicates the minimum and maximum randomization of the blue factor in the color tint."), TypeConverter(typeof(ExpandableObjectConverter))]
        public MinMax ColorTintBlueRandom
        {
            get { return _colorTintBlueRandom; }
            set { _colorTintBlueRandom = value; }
        }

        /// <summary>
        ///     Gets or sets the minimum and maximum randomization of alpha factor in the color tint.
        /// </summary>
        [Category("Color"), DisplayName("Alpha Randomization"), Description("Indicates the minimum and maximum randomization of the alpha factor in the color tint."), TypeConverter(typeof(ExpandableObjectConverter))]
        public MinMax ColorTintAlphaRandom
        {
            get { return _colorTintAlphaRandom; }
            set { _colorTintAlphaRandom = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Converts this object into a textural format.
        /// </summary>
        /// <returns>Textural version of this object.</returns>
        public override string ToString()
        {
            return "Color Modifier";
        }

        /// <summary>
        ///		When this method is called it will create an exact copy of this particle modifier.
        /// </summary>
        /// <returns>Exact copy of this particle modifier.</returns>
        public override ParticleModifier Clone()
        {
            ParticleModifier mod = new ParticleColorModifier();
            CopyTo(mod);
            return mod;
        }

        /// <summary>
        ///		Copys all the data contained in this particle modifier to another particle modifier.
        /// </summary>
        /// <param name="mod">Particle modifier to copy data into.</param>
        public override void CopyTo(ParticleModifier mod)
        {
            base.CopyTo(mod);

            ParticleColorModifier colMod = (ParticleColorModifier)mod;
            colMod._colorTint = new StartFinishColor(_colorTint.Start, _colorTint.Finish, _colorTint.StartAlpha, _colorTint.FinishAlpha);
            colMod._colorTintAlphaRandom = new MinMax(_colorTintAlphaRandom.Minimum, _colorTintAlphaRandom.Maximum);
            colMod._colorTintBlueRandom = new MinMax(_colorTintBlueRandom.Minimum, _colorTintBlueRandom.Maximum);
            colMod._colorTintGreenRandom = new MinMax(_colorTintGreenRandom.Minimum, _colorTintGreenRandom.Maximum);
            colMod._colorTintRedRandom = new MinMax(_colorTintRedRandom.Minimum, _colorTintRedRandom.Maximum);
        }

        /// <summary>
        ///     Applies this modifiers effect to the given particle node.
        /// </summary>
        /// <param name="node">Node to apply effect to.</param>
        /// <param name="deltaTime">Time elapsed since last frame.</param>
        public override void Apply(ParticleNode node, float deltaTime)
        {
            // Check that we are active on this particle.
            if (Active(node) == false) return;

            // Couple of useful variables.
            float onePercent = (100.0f / (float)node.LifeTime);
            float duration = (_effectDuration.Finish - _effectDuration.Start);
            float expired = (onePercent * (float)node.ExpiredLife) - _effectDuration.Start;

            // Color the particle
            int rRand = _colorTintRedRandom.Minimum + (int)((_colorTintRedRandom.Maximum - _colorTintRedRandom.Minimum) * node.RandomNumbers[2]);
            int gRand = _colorTintGreenRandom.Minimum + (int)((_colorTintGreenRandom.Maximum - _colorTintGreenRandom.Minimum) * node.RandomNumbers[3]);
            int bRand = _colorTintBlueRandom.Minimum + (int)((_colorTintBlueRandom.Maximum - _colorTintBlueRandom.Minimum) * node.RandomNumbers[4]);
            int aRand = _colorTintAlphaRandom.Minimum + (int)((_colorTintAlphaRandom.Maximum - _colorTintAlphaRandom.Minimum) * node.RandomNumbers[5]);
            int r = (int)(_colorTint.Start.R + (((_colorTint.Finish.R - _colorTint.Start.R) / duration) * expired));
            int g = (int)(_colorTint.Start.G + (((_colorTint.Finish.G - _colorTint.Start.G) / duration) * expired));
            int b = (int)(_colorTint.Start.B + (((_colorTint.Finish.B - _colorTint.Start.B) / duration) * expired));
            int a = (int)(_colorTint.StartAlpha + (((_colorTint.FinishAlpha - _colorTint.StartAlpha) / duration) * expired));
            node.Color = ColorMethods.CombineColor(ColorFormats.A8R8G8B8, MathMethods.Restrict(r + rRand, 0, 255), MathMethods.Restrict(g + gRand, 0, 255), MathMethods.Restrict(b + bRand, 0, 255), MathMethods.Restrict(a + aRand, 0, 255));
        }

        /// <summary>
        ///		Saves this modifier into a given binary writer.
        /// </summary>
        /// <param name="writer">Binary writer to save this modifier into.</param>
        public override void Save(BinaryWriter writer)
        {
            // All your bases are belong to us!
            base.Save(writer);

            // Save properties.
            writer.Write(ColorMethods.CombineColor(ColorFormats.A8R8G8B8, _colorTint.Start.R, _colorTint.Start.G, _colorTint.Start.B, _colorTint.StartAlpha));
            writer.Write(ColorMethods.CombineColor(ColorFormats.A8R8G8B8, _colorTint.Finish.R, _colorTint.Finish.G, _colorTint.Finish.B, _colorTint.FinishAlpha));
            writer.Write(_colorTintRedRandom.Minimum);
            writer.Write(_colorTintRedRandom.Maximum);
            writer.Write(_colorTintGreenRandom.Minimum);
            writer.Write(_colorTintGreenRandom.Maximum);
            writer.Write(_colorTintBlueRandom.Minimum);
            writer.Write(_colorTintBlueRandom.Maximum);
            writer.Write(_colorTintAlphaRandom.Minimum);
            writer.Write(_colorTintAlphaRandom.Maximum);
        }

        /// <summary>
        ///		Loads this modifier from a given binary reader.
        /// </summary>
        /// <param name="reader">Binary reader to load this modifier from.</param>
        public override void Load(BinaryReader reader)
        {
            // Load ze base!
            base.Load(reader);

            // Load properties.
            int sr, sg, sb, sa;
            ColorMethods.SplitColor(ColorFormats.A8R8G8B8, reader.ReadInt32(), out sr, out sg, out sb, out sa);
            int fr, fg, fb, fa;
            ColorMethods.SplitColor(ColorFormats.A8R8G8B8, reader.ReadInt32(), out fr, out fg, out fb, out fa);
            
            _colorTint.Start = Color.FromArgb(sr, sg, sb);
            _colorTint.StartAlpha = (byte)sa;

            _colorTint.Finish = Color.FromArgb(fr, fg, fb);
            _colorTint.FinishAlpha = (byte)fa;

            _colorTintRedRandom = new MinMax(reader.ReadInt32(), reader.ReadInt32());
            _colorTintGreenRandom = new MinMax(reader.ReadInt32(), reader.ReadInt32());
            _colorTintBlueRandom = new MinMax(reader.ReadInt32(), reader.ReadInt32());
            _colorTintAlphaRandom = new MinMax(reader.ReadInt32(), reader.ReadInt32());
        }

        /// <summary>
        ///     Invoked when a new instance of this class is created.
        /// </summary>
        /// <param name="effectDuration">The start and finish time which the type effects particles in between.</param>
        /// <param name="colorTint">Starting and ending color tint in the form of an array.</param>
        public ParticleColorModifier(StartFinish effectDuration, StartFinishColor colorTint)
        {
            _effectDuration = effectDuration;
            _colorTint = colorTint;
        }
        public ParticleColorModifier() { }

        #endregion
    }

    /// <summary>
    ///     Used to apply a render state effect to a particle.
    /// </summary>
    public class ParticleRenderStateModifier : ParticleModifier
    {
        #region Members
        #region Variables

        private Graphics.Image _image = null;
        private BitmapFont _font = null;
        private EntityRenderMode _renderMode = EntityRenderMode.Rectangle;
        private BlendMode _blendMode = BlendMode.Alpha;
        private string _text = "";
        private bool _isVisible = true, _isEnabled = true;

        private FileEditorValue _shaderFileEditor = new FileEditorValue("", "Fusion Shader Files|*.xml", Environment.CurrentDirectory + "\\" + Engine.GlobalInstance.ShaderPath);
        private Shader _shader = null;

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the image that all particles are modified to use.
        /// </summary>
        [Category("Render State"), DisplayName("Image"), Description("Indicates the image that particles should use when rendering in an image related render mode."), Editor(typeof(ImageEditor), typeof(UITypeEditor))]
        public Graphics.Image Image
        {
            get { return _image; }
            set 
            { 
                _image = value; 
                if (_image != null) 
                {
                    _image.Origin = new Point(_image.Width / 2, _image.Height / 2);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the  font that all particles are modified to use.
        /// </summary>
        [Category("Render State"), DisplayName("Font"), Description("Indicates the font that particles should render text in when their render mode is set to Text."), Editor(typeof(BitmapFontEditor), typeof(UITypeEditor))]
        public Graphics.BitmapFont Font
        {
            get { return _font; }
            set { _font = value; }
        }

        /// <summary>
        ///     Gets or sets the shader that all particles are modified to use.
        /// </summary>
        [Category("Render State"), DisplayName("Shader"), Description("Indicates the shader that particles should apply to themselfs."), Editor(typeof(FileEditor), typeof(UITypeEditor))]
        public FileEditorValue Shader
        {
            get { return _shaderFileEditor; }
            set 
            {
                if (value.FileUrl == "")
                    _shader = null;
                else
                    _shader = new Shader(Engine.GlobalInstance.ShaderPath + "\\" + value.FileUrl);
                _shaderFileEditor = value;
            }
        }

        /// <summary>
        ///     Gets or sets the render mode that all particles are modified to use.
        /// </summary>
        [Category("Render State"), DisplayName("Render Mode"), Description("Indicates the rendering mode particles should use.")]
        public EntityRenderMode RenderMode
        {
            get { return _renderMode; }
            set { _renderMode = value; }
        }

        /// <summary>
        ///     Gets or sets the blend mode that all particles are modified to use.
        /// </summary>
        [Category("Render State"), DisplayName("Blend Mode"), Description("Indicates the blend mode particles should use when rendering.")]
        public BlendMode BlendMode
        {
            get { return _blendMode; }
            set { _blendMode = value; }
        }

        /// <summary>
        ///     Gets or sets the text that all particles are modified to use.
        /// </summary>
        [Category("Render State"), DisplayName("Text"), Description("Indicates the text that particles should render if their render mode is set to Text.")]
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        /// <summary>
        ///     Gets or sets if modified particles are visible.
        /// </summary>
        [Category("Render State"), DisplayName("Visible"), Description("Indicates the visibility state that particles should be set to.")]
        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }

        /// <summary>
        ///     Gets or sets if modified particles are enabled.
        /// </summary>
        [Category("Render State"), DisplayName("Enabled"), Description("Indicates the enabled state that particles should be set to.")]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Converts this object into a textural format.
        /// </summary>
        /// <returns>Textural version of this object.</returns>
        public override string ToString()
        {
            return "Render State Modifier";
        }

        /// <summary>
        ///     Applies this modifiers effect to the given particle node.
        /// </summary>
        /// <param name="node">Node to apply effect to.</param>
        /// <param name="deltaTime">Time elapsed since last frame.</param>
        public override void Apply(ParticleNode node, float deltaTime)
        {
            // Check that we are active on this particle.
            if (Active(node) == false) return;

            // Properties, properties :D.
            node.Image = _image;
            node.RenderMode = _renderMode;
            node.BlendMode = _blendMode;
            node.Text = _text;
            node.Font = _font;
            node.IsSolid = _isVisible;
            node.IsVisible = _isEnabled;
            node.Shader = _shader;
        }

        /// <summary>
        ///		When this method is called it will create an exact copy of this particle modifier.
        /// </summary>
        /// <returns>Exact copy of this particle modifier.</returns>
        public override ParticleModifier Clone()
        {
            ParticleModifier mod = new ParticleRenderStateModifier();
            CopyTo(mod);
            return mod;
        }

        /// <summary>
        ///		Copys all the data contained in this particle modifier to another particle modifier.
        /// </summary>
        /// <param name="mod">Particle modifier to copy data into.</param>
        public override void CopyTo(ParticleModifier mod)
        {
            base.CopyTo(mod);

            ParticleRenderStateModifier rsMod = (ParticleRenderStateModifier)mod;
            rsMod._blendMode = _blendMode;
            rsMod._font = _font;
            rsMod._image = _image;
            rsMod._isEnabled = _isEnabled;
            rsMod._isVisible = _isVisible;
            rsMod._renderMode = _renderMode;
            rsMod._text = _text;
        }

        /// <summary>
        ///		Saves this modifier into a given binary writer.
        /// </summary>
        /// <param name="writer">Binary writer to save this modifier into.</param>
        public override void Save(BinaryWriter writer)
        {
            // All your bases are belong to us!
            base.Save(writer);

            // Save properties.
            writer.Write((int)_renderMode);
            writer.Write((int)_blendMode);
            writer.Write(_text);
            writer.Write(_isVisible);
            writer.Write(_isEnabled);

            if (_image != null && _image.URL != null)
            {
                writer.Write(true);
                writer.Write(_image.URL);
                writer.Write(_image.Width);
                writer.Write(_image.Height);
                writer.Write((short)_image.VerticalSpacing);
                writer.Write((short)_image.HorizontalSpacing);
            }
            else
                writer.Write(false);

            if (_font != null && _font.URL != null)
            {
                writer.Write(true);
                writer.Write(_font.URL);
            }
            else
                writer.Write(false);

            if (_shader != null && _shader.URL != null)
            {
                writer.Write(true);
                writer.Write(_shader.URL);
            }
            else
                writer.Write(false);
        }

        /// <summary>
        ///		Loads this modifier from a given binary reader.
        /// </summary>
        /// <param name="reader">Binary reader to load this modifier from.</param>
        public override void Load(BinaryReader reader)
        {
            // Load ze base!
            base.Load(reader);

            // Load properties.
            _renderMode = (EntityRenderMode)reader.ReadInt32();
            _blendMode = (BlendMode)reader.ReadInt32();
            _text = reader.ReadString();
            _isVisible = reader.ReadBoolean();
            _isEnabled = reader.ReadBoolean();

            if (reader.ReadBoolean() == true)
            {
                string imageUrl = reader.ReadString();
                int cellWidth = reader.ReadInt32();
                int cellHeight = reader.ReadInt32();
                int hSpacing = reader.ReadInt16();
                int vSpacing = reader.ReadInt16();
                if (ResourceManager.ResourceExists(imageUrl) == true)
                {
                    _image = GraphicsManager.LoadImage(imageUrl, cellWidth, cellHeight, hSpacing, vSpacing, 0);
                    _image.Origin = new Point(cellWidth / 2, cellHeight / 2);
                }
            }

            if (reader.ReadBoolean() == true)
            {
                string fontUrl = reader.ReadString();
                if (ResourceManager.ResourceExists(fontUrl) == true)
                    _font = GraphicsManager.LoadFont(fontUrl);
			
            }

            if (reader.ReadBoolean() == true)
            {
                string shaderUrl = reader.ReadString();
                if (ResourceManager.ResourceExists(shaderUrl) == true)
                {
                    _shader = GraphicsManager.LoadShader(shaderUrl);
                    _shaderFileEditor.FileUrl = shaderUrl;
                }

            }
        }

        #endregion
    }

    /// <summary>
    ///     Used to describe the animation mode of a particle animation modifier.
    /// </summary>
    public enum AnimationModifierMode
    {
        OneShot,
        Loop,
    }

    /// <summary>
    ///     Used to apply an animation effect to a particle.
    /// </summary>
    public class ParticleAnimationModifier : ParticleModifier
    {
        #region Members
        #region Variables

        private AnimationModifierMode _animationMode = AnimationModifierMode.OneShot;
        private int _frameDelay = 100;
        private StartFinish _frames = new StartFinish(0,0);

        #endregion
        #region Properties

        /// <summary>
        ///     Gets or sets the method used to animate particles.
        /// </summary>
        [Category("Animation"), DisplayName("Animation Mode"), Description("Indicates the method used to animate particles."), TypeConverter(typeof(ExpandableObjectConverter))]
        public AnimationModifierMode Mode
        {
            get { return _animationMode; }
            set { _animationMode = value; }
        }

        /// <summary>
        ///     Gets or sets the frame delay between particles.
        /// </summary>
        [Category("Animation"), DisplayName("Frame Delay"), Description("Indicates the delay (in milliseconds) between frames. This only takes effect if the animation mode is something other than OneShot."), TypeConverter(typeof(ExpandableObjectConverter))]
        public int FrameDelay
        {
            get { return _frameDelay; }
            set { _frameDelay = value; }
        }

        /// <summary>
        ///     Gets or sets the frame delay between particles.
        /// </summary>
        [Category("Animation"), DisplayName("Frames"), Description("Specifies the starting and finishing frames of the animation loop."), TypeConverter(typeof(ExpandableObjectConverter))]
        public StartFinish Frames
        {
            get { return _frames; }
            set { _frames = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Converts this object into a textural format.
        /// </summary>
        /// <returns>Textural version of this object.</returns>
        public override string ToString()
        {
            return "Animation Modifier";
        }

        /// <summary>
        ///		When this method is called it will create an exact copy of this particle modifier.
        /// </summary>
        /// <returns>Exact copy of this particle modifier.</returns>
        public override ParticleModifier Clone()
        {
            ParticleModifier mod = new ParticleAnimationModifier();
            CopyTo(mod);
            return mod;
        }

        /// <summary>
        ///		Copys all the data contained in this particle modifier to another particle modifier.
        /// </summary>
        /// <param name="mod">Particle modifier to copy data into.</param>
        public override void CopyTo(ParticleModifier mod)
        {
            base.CopyTo(mod);

            ParticleAnimationModifier aniMod = (ParticleAnimationModifier)mod;
            aniMod._frameDelay = _frameDelay;
            aniMod._animationMode = _animationMode;
            aniMod._frames.Start = _frames.Start;
            aniMod._frames.Finish = _frames.Finish;
        }

        /// <summary>
        ///     Applies this modifiers effect to the given particle node.
        /// </summary>
        /// <param name="node">Node to apply effect to.</param>
        /// <param name="deltaTime">Time elapsed since last frame.</param>
        public override void Apply(ParticleNode node, float deltaTime)
        {
            // Check that we are active on this particle.
            if (Active(node) == false) return;

            // Couple of useful variables.
            float onePercent = (100.0f / (float)node.LifeTime);
            float duration = (_effectDuration.Finish - _effectDuration.Start);
            float expired = (onePercent * (float)node.ExpiredLife) - _effectDuration.Start;

            if (_animationMode == AnimationModifierMode.Loop)
                node.Frame = _frames.Start + ((((int)expired) / _frameDelay) % (_frames.Finish - _frames.Start));
            else if (_animationMode == AnimationModifierMode.OneShot)
                node.Frame = _frames.Start + (int)(((_frames.Finish - _frames.Start) / duration) * expired);
        }

        /// <summary>
        ///		Saves this modifier into a given binary writer.
        /// </summary>
        /// <param name="writer">Binary writer to save this modifier into.</param>
        public override void Save(BinaryWriter writer)
        {
            // All your bases are belong to us!
            base.Save(writer);

            // Save properties.
            writer.Write((byte)_animationMode);
            writer.Write((short)_frameDelay);
            writer.Write(_frames.Start);
            writer.Write(_frames.Finish);
        }

        /// <summary>
        ///		Loads this modifier from a given binary reader.
        /// </summary>
        /// <param name="reader">Binary reader to load this modifier from.</param>
        public override void Load(BinaryReader reader)
        {
            // Load ze base!
            base.Load(reader);

            // Load properties.
            _animationMode = (AnimationModifierMode)reader.ReadByte();
            _frameDelay = reader.ReadInt16();
            _frames.Start = reader.ReadInt32();
            _frames.Finish = reader.ReadInt32();
        }

        /// <summary>
        ///     Invoked when a new instance of this class is created.
        /// </summary>
        /// <param name="effectDuration">The start and finish time which the type effects particles in between.</param>
        /// <param name="startFrame">Starting frame of animation.</param>
        /// <param name="endFrame">Ending frame of animation.</param>
        public ParticleAnimationModifier(StartFinish effectDuration, StartFinish frames)
        {
            _effectDuration = effectDuration;
            _frames = frames;
        }
        public ParticleAnimationModifier() { }

        #endregion
    }

	/// <summary>
	///		The emitter class is a derivitive of the SceneNode class and is
	///		used to create complex particle effects.
	/// </summary>
	public class EmitterNode : EntityNode
	{
		#region Members
		#region Variables

		private CallbackProcess _callbackProcess = null;

		private int _maximumParticles = 500;

		private ArrayList _particleList = new ArrayList();
        private ArrayList _particleTypes = new ArrayList();

		#endregion
		#region Properties

        /// <summary>
        ///		Returns a string which can be used to identify what kind of entity this is.
        /// </summary>
        public override string Type
        {
            get { return _type != "" ? _type : "emitter"; }
        }

		/// <summary>
		///		Gets or sets the maximum amount of particles attached to this emitter
		///		that can be in existant at any time.
		/// </summary>
		public int MaximumParticles
		{
			get { return _maximumParticles; }
			set { _maximumParticles = value; }
		}

        /// <summary>
        ///     Gets or sets the list of particles this emitter controls.
        /// </summary>
        public ArrayList Particles
        {
            get { return _particleList; }
            set { _particleList = value; }
        }

        /// <summary>
        ///     Gets or sets the list of particle types this emitter emits.
        /// </summary>
        public ArrayList ParticleTypes
        {
            get { return _particleTypes; }
            set { _particleTypes = value; }
        }

		#endregion
		#endregion
		#region Methods

        /// <summary>
        ///     Disposes of an instance of this class.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (_callbackProcess != null)
               ProcessManager.DettachProcess(_callbackProcess);

            foreach (ParticleType type in _particleTypes)
                type.Dispose();
            _particleTypes.Clear();
        }

        /// <summary>
        ///     Registers a particle type with this emitter.
        /// </summary>
        /// <param name="type">Particle type to register.</param>
        public void RegisterParticleType(ParticleType type)
        {
            _particleTypes.Add(type);
            type.Emitter = this;
        }

        /// <summary>
        ///     Unregisters a particle type with this emitter.
        /// </summary>
        /// <param name="type">Particle type to unregister.</param>
        public void UnregisterParticleType(ParticleType type)
        {
            _particleTypes.Remove(type);
            type.Emitter = null;
        }
        
        /// <summary>
        ///     Restarts this emitter.
        /// </summary>
        public void Restart()
        {
            foreach (ParticleType type in _particleTypes)
                type.Restart();
        }

		/// <summary>
		///		Copys all the data contained in this scene node to another scene node.
		/// </summary>
		/// <param name="node">Scene node to copy data into.</param>
		public override void CopyTo(SceneNode node)
		{
			EmitterNode emitterNode = node as EmitterNode;
			if (emitterNode == null) return;

			base.CopyTo(node);

			// Copy emitter stuff
			emitterNode._maximumParticles = _maximumParticles;
            foreach (ParticleType type in _particleTypes)
            {
                ParticleType typeClone = type.Clone();
                typeClone.Emitter = emitterNode;
                emitterNode._particleTypes.Add(typeClone);
            }
        }

		/// <summary>
		///		Saves this entity into a given binary writer.
		/// </summary>
		/// <param name="writer">Binary writer to save this entity into.</param>
		public override void Save(BinaryWriter writer)
		{
			// Save all the basic entity details.
			base.Save(writer);

			// Save all the emitter specific details.
			writer.Write(_maximumParticles);
            writer.Write(_particleTypes.Count);

            // Save each particle type.
            foreach (ParticleType type in _particleTypes)
                type.Save(writer);
		}

		/// <summary>
		///		Loads this scene node from a given binary reader.
		/// </summary>
		/// <param name="reader">Binary reader to load this scene node from.</param>
		public override void Load(BinaryReader reader)
		{
			// Load all the basic entity details.
			base.Load(reader);

			// Load all the emitter specific details.
			_maximumParticles = reader.ReadInt32();
            int typeCount = reader.ReadInt32();

            // Load each particle type.
            for (int i = 0; i < typeCount; i++)
            {
                ParticleType type = new ParticleType();
                type.Emitter = this;
                type.Load(reader);
                _particleTypes.Add(type);
            }
		}

        /// <summary>
        ///		Processes this entitys logic.
        /// </summary>
        public void Think(float deltaTime)
		{
			if (_enabled == false) return;

			// Kill off some old particles if we have reached the maximum
            while (_particleList.Count > _maximumParticles)
                _particleList.RemoveAt(0);

            // Go through each particle type and allow it to think.
            foreach (ParticleType type in _particleTypes)
                type.Think(deltaTime);
		}

		/// <summary>
		///		Sets the rendering state so that all children of this 
		///		camera are drawn correctly.
		/// </summary>
		/// <param name="position">Position where this entity's parent node was rendered.</param>
        public override void Render(Transformation transformation, CameraNode camera, int layer)
		{
			Transformation relativeTransformation = CalculateRelativeTransformation(transformation);
            SetupRenderingState(relativeTransformation);

            // Are we currnetly rendering the layer this entity is on?
            if (layer != _depthLayer)
            {
                if (_visible == true) RenderChildren(relativeTransformation, camera, layer);
                return;
            }

            //Statistics.StoreInt("Nodes Rendered", Statistics.ReadInt("Nodes Rendered") + 1);

			// Render all the particles attached to this emitter.
			if (_visible == true || _forceVisibility == true)
			{
				GraphicsManager.PushRenderState();
                foreach (ParticleNode particleNode in _particleList)
					particleNode.Render(transformation, camera, layer);
				GraphicsManager.PopRenderState();
			}

			// Render the bounding box and sizing points if we have been asked to do so.
			if (_renderBoundingBox == true || _forceBoundingBoxVisibility == true) RenderBoundingBox(relativeTransformation, camera);
			if (_renderSizingPoints == true) RenderSizingPoints(relativeTransformation, camera);
			if (_renderEventLines == true) RenderEventLines(relativeTransformation, camera);
            if (_forceGlobalDebugVisibility == true) RenderDebug(relativeTransformation, camera);

			// Render all the children of this entity.
			RenderChildren(relativeTransformation, camera, layer);
		}

		/// <summary>
		///		Initializes a new instance and gives it the specified  
		///		name.
		/// </summary>
		/// <param name="name">Name to name as node to.</param>
		/// <param name="emitRate">Delay (in milliseconds) between particle emissions.</param>
		/// <param name="emitCount">Amount of particles to emit each time.</param>
		/// <param name="emitShape">Shape to emit particles in.</param>
		public EmitterNode(string name, int maximumParticles)
		{
			_name = name;
            _maximumParticles = maximumParticles;
			_callbackProcess = new CallbackProcess(new CallbackProcessEventHandler(Think));
			ProcessManager.AttachProcess(_callbackProcess);
		}
		public EmitterNode()
		{
			_name = "Emitter";
			_callbackProcess = new CallbackProcess(new CallbackProcessEventHandler(Think));
			ProcessManager.AttachProcess(_callbackProcess);
		}

		#endregion
	}

    /// <summary>
    ///		The particle class is a derivitive of the SceneNode class and is
    ///		used by the EmitterNode class to create complex particle effects.
    /// </summary>
    public class ParticleNode : EntityNode
    {
        #region Members
        #region Variables

        private int _lifeTime = 0;
        private HighPreformanceTimer _lifeTimer = new HighPreformanceTimer();
        private float[] _randomNumbers = new float[]
            {
                MathMethods.Random(),
                MathMethods.Random(),
                MathMethods.Random(),
                MathMethods.Random(),
                MathMethods.Random(),
                MathMethods.Random(),
                MathMethods.Random(),
                MathMethods.Random(),
                MathMethods.Random(),
                MathMethods.Random(),
            };

        #endregion
        #region Properties

        /// <summary>
        ///		Returns a string which can be used to identify what kind of entity this is.
        /// </summary>
        public override string Type
        {
            get { return _type != "" ? _type : "particle"; }
        }

        /// <summary>
        ///		Gets or sets the life time of this particle in milliseconds.
        /// </summary>
        public int LifeTime
        {
            get { return _lifeTime; }
            set { _lifeTime = value; }
        }

        /// <summary>
        ///		Gets the time this particle has been alive for.
        /// </summary>
        public int ExpiredLife
        {
            get { return (int)_lifeTimer.DurationMillisecond; }
        }

        /// <summary>
        ///     Returns true if this particle is dead.
        /// </summary>
        public bool IsDead
        {
            get { return _lifeTimer.DurationMillisecond > _lifeTime; }
        }

        /// <summary>
        ///     Gets or sets a list of random numbers used to apply randomized
        ///     modifier effects to this particle.
        /// </summary>
        public float[] RandomNumbers
        {
            get { return _randomNumbers; }
            set { _randomNumbers = value; }
        }

        #endregion
        #endregion
        #region Methods

        /// <summary>
        ///     Resets this entity to the state it was in when it was created.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            _lifeTimer.Restart();
            for (int i = 0; i < _randomNumbers.Length; i++)
                _randomNumbers[i] = MathMethods.Random();
        }

        /// <summary>
        ///		Initializes a new instance of this class.
        /// </summary>
        public ParticleNode()
        {
            _name = "Particle";
            DeinitializeCollision();
        }

        #endregion
    }
}