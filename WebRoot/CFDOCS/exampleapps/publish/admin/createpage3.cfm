<!--- Separator is based on OS --->
<CFLOCK SCOPE="Server" TIMEOUT="30" TYPE="ReadOnly">
	<CFIF Server.OS.Name CONTAINS "Windows">
		<CFSET Sep = "\">
	<CFELSE>
		<CFSET Sep = "/">	
	</CFIF>
</CFLOCK>

<CFSET Error = 0>

<!--- Check if the string contains any invalid characters --->
<CFIF FindOneOf(':*?<>|"\/ ', Path) NEQ 0>
	<CFSET Error = 1>
	<CFSET ErrorMessage = "The filename/path cannot contain any of the following characters:<BR>
: * ? &lt; &gt; | &quot; / \(space)">

<!--- Check if the file has a .cfm extension --->
<CFELSEIF Right(Form.Path, 4) NEQ ".cfm">
	<CFSET Error = 1>
	<CFSET ErrorMessage = "The file must have a .cfm extension!">
</CFIF>

<!--- Separate directory from path --->
<CFIF Path CONTAINS "#Sep#">
	<CFSET Temp = Find("#Sep#", Reverse(Path))>
	<CFSET Filename = Right(Path, Temp - 1)>
<CFELSE>
	<CFSET Filename = Path>
</CFIF>

<!--- Check if the filename is long enough --->
<CFIF Len(Filename) LTE 4 OR Left(Path, 2) IS "..">
	<CFSET Error = 1>
	<CFSET ErrorMessage = "The filename was invalid!">
</CFIF>

<!--- Check if the file already exists --->
<CFQUERY datasource="cfx" NAME="FindDupe">
SELECT * FROM tblPubPg
WHERE strTmpPth = '#Path#'
</CFQUERY>
<CFIF FindDupe.RecordCount GT 0>
	<CFSET Error = 1>
	<CFSET ErrorMessage = "The file already exists!">
</CFIF>

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

<CFIF Error IS 1>
	<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><B>Error:</B> <CFOUTPUT>#ErrorMessage#</CFOUTPUT></FONT></P>

	<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1">Please try again...</FONT></P>

	<FORM ACTION="createpage3.cfm" METHOD="POST">

	<CFOUTPUT><INPUT TYPE="HIDDEN" NAME="Template" VALUE="#Form.Template#"></CFOUTPUT>

	<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">Filename:</FONT><BR>
	<CFOUTPUT><INPUT TYPE="TEXT" NAME="Path" VALUE="#Form.Path#"></CFOUTPUT></P>

	<P><INPUT TYPE="SUBMIT" VALUE="Continue"></P>

	</FORM>

<CFELSE>

	<CFQUERY datasource="cfx" NAME="query1">
	SELECT * FROM tblPubPg
	</CFQUERY>
	
	<CFFILE ACTION="READ" FILE="#ExpandPath('templates#Sep#' & Template & ".template")#" VARIABLE="FileData">

	<!--- Grab the whole chunk of attribute-value pairs, call it MetaData --->
	<CFSET MetaDataStart = FindNoCase("---BEGIN PUBLISHING DATA---", FileData) + Len("---BEGIN PUBLISHING DATA---")>
	<CFSET MetaDataEnd = FindNoCase("---END PUBLISHING DATA---", FileData)>
	<CFSET MetaData = Trim(Mid(FileData, MetaDataStart, MetaDataEnd - MetaDataStart))>

	<!--- Now loop over each line of MetaData --->
	<CFLOOP CONDITION="Trim(MetaData) NEQ ''">

		<!--- Grab just one line of info from MetaData.  If this
			happens to be the last line in MetaData, just grab
			all that's left. --->
		<CFSET EOL = Find(Chr(10), MetaData)>
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
			<CFSET "#Attribute#" = Value>

	   	</CFIF>

		<CFIF Len(Trim(MetaData)) NEQ Len(CurrentLine)>
			<CFSET MetaData = Trim(Right(MetaData, Len(MetaData) - Len(CurrentLine)))>
		<CFELSE>
			<CFSET MetaData = "">
		</CFIF>

	</CFLOOP>
	
	<CFTRANSACTION>

		<CFSET NewPageID = CreateUUID()>

		<CFQUERY datasource="cfx">
		INSERT INTO tblPubPg (strPgID, strTmpPth, intMaxLoc)
		VALUES ('#NewPageID#', '#Path#', #Locations#)
		</CFQUERY>

	</CFTRANSACTION>

	<CFSET Path = Replace(Path, "/", "#Sep#", "ALL")>
	<CFSET Path = Replace(Path, "\", "#Sep#", "ALL")>
	<CFSET DestPath = ExpandPath("..#Sep#" & Path)>
	<CFFILE ACTION="WRITE" FILE="#DestPath#" OUTPUT="#Trim(Right(FileData, Len(FileData) - MetaDataEnd - 25))#">

	<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1">Page created successfully! You can now schedule objects to appear on <CFOUTPUT>#Path#</CFOUTPUT>.</FONT></P>

	<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><A HREF="index.cfm">Go back</A></FONT></P>

</CFIF>

</BODY>
</HTML>