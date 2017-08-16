#r "System.Xml.Linq.dll"

using System;
using System.Linq;
using System.Xml.Linq;

var nuspec = Args[0];
var src = Args[1];
var target = Args[2];
var doc = XDocument.Load( nuspec );
var xmlns = (XNamespace) "http://schemas.microsoft.com/packaging/2013/01/nuspec.xsd";
var files = doc.Root.Element( xmlns + "files" );

if ( files == null )
{
    doc.Root.Add( files = new XElement( xmlns + "files" ) );
}

files.Add(
    new XElement(
        xmlns + "file",
        new XAttribute( "src", src ),
        new XAttribute( "target", target ) ) );

doc.Save( nuspec );