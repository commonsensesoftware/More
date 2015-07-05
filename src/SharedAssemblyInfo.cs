using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

#if DEBUG
[assembly: AssemblyConfiguration( "Debug" )]
#elif RELEASE
[assembly: AssemblyConfiguration( "Release" )]
#endif
[assembly: AssemblyCompany( "Commonsense Software" )]
[assembly: AssemblyCopyright( "Copyright © Commonsense Software 2014.  All rights reserved." )]
[assembly: AssemblyTrademark( "" )]
[assembly: AssemblyCulture( "" )]
[assembly: ComVisible( false )]
[assembly: CLSCompliant( true )]
[assembly: NeutralResourcesLanguage( "en" )]