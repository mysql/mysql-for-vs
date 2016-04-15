MySQL for Visual Studio 1.2
=========
MySQL for Visual Studio provides access to MySQL objects and data without forcing your developers to leave Visual Studio. Designed and developed as a Visual Studio package, MySQL for Visual Studio integrates directly into Server Explorer providing a seamless experience for setting up new connections and working with database objects.
Copyright (c) 2012, 2016, Oracle and/or its affiliates. All rights reserved.

## Installation

* Prerequisites:
	* Visual Studio 2010 or greater.
	* .NET Framework 4.0 (Client or Full Profile).
	* Visual Studio SDK for the corresponding Visual Studio version.
	* WiX Toolset, Sandcastle, Sandcastle Help File Builder and MSBuild Tools 2013 Or 2015 for building the installer MSI.
	
  The following MySQL for Visual Studio features require MySQL Connector/Net:
	* Entity Framework Designer, when using the Data Base First or the Model First approach.
	* Web Configuration Tool
	
* Open MySqlForVisualStudio.sln in Visual Studio.

## Features

MySQL for Visual Studio is a product that includes all of the Visual Studio integration functionality to create and manage MySQL databases when developing .NET applications. Functionality concepts includes:

* SQL Development
	* By integrating directly into Visual Studio, database objects (tables, views, stored routines, triggers, indexes, etc) can be created, altered, or dropped directly inside Server Explorer.
	Visual object editors include helpful information to guide you through the editing process. Standard data views are also available to help you view your data.
* Query Designer
	* Visual Studio’s query design tool is also directly supported. With this tool, you can query and view data from tables or views while also combining filters, group conditions, and parameters. Stored routines (both with and without parameters) can also be queried.
* Stored Routine Debugging
	* Use the full debugging support for stored routines. Using the standard Visual Studio environment and controls, you can set breakpoints, add watches, and step into, out of, and over routines and calls. Local variables can be added to the watch window and call stack navigation is also supported.
* Entity Framework
	* The Entity Framework is spoorted, allowing for templated based code generation as well as full support of the model designers and wizards.
* MySQL Projects Templates wizards
	* A MySQL Windows Forms Project wizard was added, and is available from the Visual Studio New Project dialog. This wizard automates the generation of a simple .NET Windows Forms Application with a form that is connected to a MySQL data source.
	* A MySQL MVC 3 Template wizard was added, and is available from the Visual Studio New Project dialog. This wizard automates the generation of an ASP.NET application that includes a MySQL .
* MySQL Project Items
	* MySQL Project Items are available for creating data views in Windows Forms and ASP.NET MVC web applications, automating the generation of a Windows Form or ASPP.NET MVC view, representing a view for MySQL data available through an existing Entity Framework’s model entity containing a MySQL table or view.
* MySQL Simple Membership provider for web projects
	* A MySQL Simple Membership provider option is available when working with web projects, using the MySQL website configuration tool.
	

## Documentation

For further information about MySQL or additional documentation, see:
* http://www.mysql.com
* http://dev.mysql.com/doc/visual-studio/en/

## License

License information can be found in the Installer/COPYING file.

__MySQL FOSS License Exception__
We want free and open source software applications under certain licenses to be able to use specified GPL-licensed MySQL client libraries despite the fact that not all such FOSS licenses are compatible with version 2 of the GNU General Public License.
Therefore there are special exceptions to the terms and conditions of the GPLv2 as applied to these client libraries, which are identified and described in more detail in the FOSS License Exception at http://www.mysql.com/about/legal/licensing/foss-exception.html.

This distribution may include materials developed by third parties.
For license and attribution notices for these materials, please refer to the documentation that accompanies this distribution (see the "Licenses for Third-Party Components" appendix) or view the online documentation at http://dev.mysql.com/doc/.

__GPLv2 Disclaimer__
For the avoidance of doubt, except that if any license choice other than GPL or LGPL is available it will apply instead, Oracle elects to use only the General Public License version 2 (GPLv2) at this time for any software where a choice of GPL  license versions is made available with the language indicating that GPLv2 or any later version may be used, or where a choice of which version of the GPL is applied is otherwise unspecified.