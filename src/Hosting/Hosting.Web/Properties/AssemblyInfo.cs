using More.Composition;
using System;
using System.Composition;
using System.Reflection;
using System.Web;

[assembly: AssemblyTitle( "More.Web.Hosting" )]
[assembly: AssemblyProduct( "More.Web.Hosting" )]
[assembly: AssemblyDescription( "The web hosting library for the \"More\" framework." )]
[assembly: PreApplicationStartMethod( typeof( RequestCompositionScopeModule ), "Start" )]