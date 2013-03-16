/* 
 * File: Assembly.cs
 *
 * This source file contains all the assembly infomation such as version info that
 * gets included into the compiled executable code.
 *
 * Copyright (c) Binary Phoenix. All rights reserved.
 */

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if DEBUG
[assembly: AssemblyTitle("Runtime System - Debug Build")]
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyTitle("Runtime System ")]
[assembly: AssemblyConfiguration("Retail")]
#endif

[assembly: AssemblyProduct("Fusion: Runtime System")]
[assembly: AssemblyDescription("Fusion: Runtime System")]

[assembly: AssemblyCompany("Binary Phoenix")]
[assembly: AssemblyCopyright("Copyright © Binary Phoenix 2006")]
[assembly: AssemblyTrademark("All Rights Reserved")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: Guid("958c6c5f-6b36-4748-9986-dc71315652e1")]

[assembly: AssemblyVersion("1.0.*")]
//[assembly: AssemblyFileVersion("1.0.0.0")]