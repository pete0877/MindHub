<!---
	
	Template:			DirectoryList.cfm
	Author:				Simeon Simeonov
	
	Source Control:		$Header: /ColdFusion/Docs/directorylist.cfm 4     8/19/98 3:15p Ereiber $
	
	DirectoryList:
	
	Lists the entries in a directory tree in a format compatible with CFDirectory
	To handle recursion, a new query column 'Path' is added. Path contains the 
	full path to a file/directory.
	
	Limitations:
	
	Directory names are built using DOS/Win backslashes.
	
	Usage:
	
	<CF_DirectoryList Directory=... Filter=... Recurse=... ResultQuery=... AppendToResultQuery=...>
	
	Attributes:
	
	Directory (required, string)
	The root of the directory tree, for example, "c:\wwwroot"

	Filter (optional, string, default: *)
	Filter (search specification) to use.
	
	Recurse (optional, boolean, default: false)
	Determines whether a recursive directory search is performed.
	
	ResultQuery (required, variable name)
	The name of the query that the tag creates in the caller scope.
	
	AppendToResultQuery (optional, boolean, default: false)
	Determines whether a new result query is created in the caller scope
	or whether rows get appended to an existing query by this name
	
--->

<!---
	Validate attribute set
--->

<cfparam name="attributes.directory">
<cfparam name="attributes.filter" default="*">
<cfparam name="attributes.recurse" default=false>
<cfparam name="attributes.resultQuery">
<cfparam name="attributes.appendToResultQuery" default=false>

<!---
	Make sure you are using OS-friendly paths
--->

<cfif server.os.name contains "Windows">
	<cfset dirSep = '\'>
<cfelse>
	<cfset dirSep = '/'>	
</cfif>

<!---
	Create or bind to a result query
--->

<cfif attributes.appendToResultquery>
	<cfparam name="Caller.#attributes.resultQuery#">
	<cfset resultQuery = Evaluate("Caller.#attributes.resultQuery#")>
	<cfif not isQuery(resultQuery)>
		<cfabort showerror="CF_DirectoryList: variable 'Caller.#attributes.resultQuery#' is not a query">
	</cfif>
<cfelse>
	<cfset resultQuery = QueryNew("Name,Path,Size,Type,DateLastModified,Attributes,Mode")>
</cfif>	

<!---
	Run the initial directory search
--->

<cfdirectory
	action=list
	directory=#attributes.directory#
	filter=#attributes.filter#
	name=dirList>
	
<!---
	Iterate over the directory listing of matching specification
--->

<cfloop query=dirList>	
	<!--- Add the current entry to the result query --->
	<cfset temp = queryAddRow(resultQuery)>
	<cfset temp = querySetCell(resultQuery, "Path", "#attributes.directory##dirSep##name#")>
	<cfset temp = querySetCell(resultQuery, "Name", name)>
	<cfset temp = querySetCell(resultQuery, "Size", size)>
	<cfset temp = querySetCell(resultQuery, "DateLastModified", dateLastModified)>
	<cfset temp = querySetCell(resultQuery, "Type", type)>
	<cfset temp = querySetCell(resultQuery, "Attributes", attributes)>
	<cfset temp = querySetCell(resultQuery, "Mode", mode)>
</cfloop>

<!---
	Check whether recursing is required
--->

<cfif attributes.recurse>

	<!---
		Take a full listing, if a special search was used
	--->
	
	<cfif attributes.filter is not "*" AND attributes.filter is not "*.*">
		<cfdirectory
			action=list
			directory=#attributes.directory#
			name=dirList>
	</cfif>
		
	<!---
		Iterate over the full directory listing to look for directories to recurse into
	--->
	
	<cfloop query=dirList>
		<!--- Make a special check for directories that are not . and ..--->
		<cfif type is "Dir" and name is not "." and name is not "..">
			<!-- Recurse into the directory-->
			<CF_DirectoryList
				directory="#attributes.directory##dirSep##name#"
				filter=#attributes.filter#
				recurse=true
				resultQuery=resultQuery
				appendToResultQuery=true>
		</cfif>		
	</cfloop>
	
</cfif>
	
<!---
	Set result in caller scope
--->

<cfset "Caller.#attributes.resultQuery#" = resultQuery>


