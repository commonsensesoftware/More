using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

#if DEBUG
[assembly: AssemblyConfiguration( "Debug" )]
#elif RELEASE
[assembly: AssemblyConfiguration( "Release" )]
#endif
[assembly: AssemblyTitle( "More.VisualStudio.Editors" )]
[assembly: AssemblyProduct( "More.VisualStudio.Editors" )]
[assembly: AssemblyDescription( "The editors and code generators for the \"More\" framework used by Microsoft Visual Studio." )]
[assembly: AssemblyCompany( "Commonsense Software" )]
[assembly: AssemblyCopyright( "Copyright © Commonsense Software 2014.  All rights reserved." )]
[assembly: AssemblyTrademark( "" )]
[assembly: AssemblyCulture( "" )]
[assembly: ComVisible( false )]
[assembly: Guid( "42b4c0b5-0112-4ba5-9e30-22801dd5cf9a" )]
[assembly: CLSCompliant( false )]
[assembly: AssemblyVersion( "1.0.0.0" )]
[assembly: NeutralResourcesLanguage( "en" )]