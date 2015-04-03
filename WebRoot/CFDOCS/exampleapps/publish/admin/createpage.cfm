<!--- Separator is based on OS --->
<CFLOCK SCOPE="Server" TIMEOUT="30" TYPE="ReadOnly">
	<CFIF Server.OS.Name CONTAINS "Windows">
		<CFSET Sep = "\">
	<CFELSE>
		<CFSET Sep = "/">	
	</CFIF>
</CFLOCK>

<!--- This publishing systems' "templates" (not Cold Fusion templates)
	are just like regular Cold Fusion templates except that they have
	a block of "metadata" (for lack of a better word) in the first few
	lines of the file. The template files are all named <Template name>.template
	so that the file default.template would be a template named "default".
	
	An example of a template is this:
	
	---BEGIN PUBLISHING DATA---
	LOCATIONS=1
	DESCRIPTION=This is a completely bare template which should be modified before use.
	---END PUBLISHING DATA---
	
	<CFINCLUDE TEMPLATE="_header.cfm">
	
	<CF_ShowContent>
	
	<CFINCLUDE TEMPLATE="_footer.cfm">
--->

<!--- These are just aliases; ValidAttributes lists the
	template attributes that are allowed --->
<CFSET ValidAttributes = "Name,Locations,Description">
<CFSET NewLine = Chr(10)>

<!--- Get the template files' info and put it in the query Templates --->
<CFDIRECTORY ACTION="LIST"
			 DIRECTORY="#ExpandPath('templates')#"
			 NAME="Templates"
			 FILTER="*.template"
			 SORT="DateLastModified">

<!--- Initialize the query TemplateInfo... this will hold the
	final data; each template will have a record, each column
	will be an attribute --->
<CFSET TemplateInfo = QueryNew(ValidAttributes)>

<!--- Now go into each file and retrieve the attributes --->
<CFLOOP QUERY="Templates">

	<CFFILE ACTION="READ" FILE="#ExpandPath('templates#Sep#' & Name)#" VARIABLE="FileData">

	<!--- Adding a row to the TemplateInfo query, and putting in the name --->
	<CFSET Temp = QueryAddRow(TemplateInfo)>
	<CFSET Temp = QuerySetCell(TemplateInfo, "Name", Left(Name, Len(Name) - Len(".template")))>

	<!--- Grab the whole chunk of attribute-value pairs, call it MetaData --->
	<CFSET MetaDataStart = FindNoCase("---BEGIN PUBLISHING DATA---", FileData) + Len("---BEGIN PUBLISHING DATA---")>
	<CFSET MetaDataEnd = FindNoCase("---END PUBLISHING DATA---", FileData)>
	<CFSET MetaData = Trim(Mid(FileData, MetaDataStart, MetaDataEnd - MetaDataStart))>

	<!--- Now loop over each line of MetaData --->
	<CFLOOP CONDITION="Trim(MetaData) NEQ ''">

		<!--- Grab just one line of info from MetaData.  If this
			happens to be the last line in MetaData, just grab
			all that's left. --->
		<CFSET EOL = Find(Newline, MetaData)>
		<CFIF EOL NEQ 0>
			<CFSET CurrentLine = Trim(Left(MetaData, EOL))>
		<CFELSE>
			<CFSET CurrentLine = Trim(MetaData)>
		</CFIF>
		
		<!--- If there's a "=" character in the current line, continue processing --->
		<CFIF Find("=", CurrentLine) NEQ 0 AND Left(CurrentLine, 1) NEQ ":">

			<!--- Split the current line at the "="; everything to the left
				is the attribute name, everything to the right is the value --->
			<CFSET MidPoint = Find("=", CurrentLine)>
			<CFSET Attribute = Left(CurrentLine, MidPoint - 1)>
			<CFSET Value = Right(CurrentLine, Len(CurrentLine) - MidPoint)>
			
			<!--- If the parameter is valid, save the value in the TemplateInfo query --->
			<CFIF ListFindNoCase(ValidAttributes, Attribute) NEQ 0>
				<CFSET Temp = QuerySetCell(TemplateInfo, Attribute, Value)>
			</CFIF>

		</CFIF>

		<CFIF Len(Trim(MetaData)) NEQ Len(CurrentLine)>
			<CFSET MetaData = Trim(Right(MetaData, Len(MetaData) - Len(CurrentLine)))>
		<CFELSE>
			<CFSET MetaData = "">
		</CFIF>

	</CFLOOP>

</CFLOOP>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>Create Page</TITLE>
<STYLE TYPE="text/css">
<!-- A {text-decoration: none} -->
</STYLE>
</HEAD>

<BODY BGCOLOR="#660000" TEXT="#FFFFFF" LINK="#FFFF00" VLINK="#FFFF00">

<TABLE ALIGN="RIGHT" CELLPADDING="0" CELLSPACING="0">
<TR>
	<TD ALIGN="RIGHT">
	<FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
		<A HREF="index.cfm">Administrator Home</A>
	</FONT>
	</TD>
</TR>
</TABLE>

<P><IMG SRC="images/createpage.gif" WIDTH=96 HEIGHT=23 BORDER=0 ALT="Create Page"></P>

<FORM ACTION="createpage2.cfm" METHOD="POST">

<TABLE BORDER="1" CELLPADDING="5" CELLSPACING="2">
<TR>
	<TD></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Name</B></FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Locations</B></FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Description</B></FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Sample Image</B></FONT></TD>
</TR>
<CFOUTPUT QUERY="TemplateInfo">
<TR>
	<TD><INPUT TYPE="RADIO" NAME="Template" VALUE="#Name#"></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">#Name#</FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">#Locations#</FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">#Description#</FONT></TD>
	<CFIF FileExists(ExpandPath("templates/" & Name & ".gif"))>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><A HREF="templates/#name#.gif" TARGET="_blank">Sample Image</A></FONT></TD>
	</CFIF>
</TR>
</CFOUTPUT>
</TABLE>

<P><INPUT TYPE="SUBMIT" VALUE="Continue"></P>

</FORM>

</BODY>
</HTML>