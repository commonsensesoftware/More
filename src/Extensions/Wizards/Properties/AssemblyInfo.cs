using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;

#if DEBUG
[assembly: AssemblyConfiguration( "Debug" )]
#elif RELEASE
[assembly: AssemblyConfiguration( "Release" )]
#endif
[assembly: AssemblyTitle( "More.VisualStudio" )]
[assembly: AssemblyProduct( "More.VisualStudio" )]
[assembly: AssemblyDescription( "The standard template wizards for the \"More\" framework used by Microsoft Visual Studio." )]
[assembly: AssemblyCompany( "Commonsense Software" )]
[assembly: AssemblyCopyright( "Copyright © Commonsense Software 2014.  All rights reserved." )]
[assembly: AssemblyTrademark( "" )]
[assembly: AssemblyCulture( "" )]
[assembly: ComVisible( false )]
[assembly: CLSCompliant( false )]
[assembly: AssemblyVersion( "1.0.0.0" )]
[assembly: NeutralResourcesLanguage( "en" )]
[assembly: ThemeInfo( ResourceDictionaryLocation.SourceAssembly, ResourceDictionaryLocation.None )]
