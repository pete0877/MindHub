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
: * ? &lt; &gt; | &quot; /\(space)">

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

<!--- Check if the file already exists in the database --->
<CFQUERY datasource="cfx" NAME="FindDupe">
SELECT * FROM tblPubPg
WHERE strTmpPth = '#Path#'
</CFQUERY>
<CFIF FindDupe.RecordCount GT 0>
	<CFSET Error = 1>
	<CFSET ErrorMessage = "That file is already registered.">
</CFIF>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>Register Page</TITLE>
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

<P><IMG SRC="images/registerpage.gif" WIDTH=120 HEIGHT=24 BORDER=0></P>

<CFIF Error IS 1>

	<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><B>Error:</B> <CFOUTPUT>#ErrorMessage#</CFOUTPUT></FONT></P>

	<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1">Please try again...</FONT></P>

	<FORM ACTION="regpage2.cfm" METHOD="POST">

	<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><B>Template Name</B> (relative to <CFOUTPUT>#ExpandPath("../")#</CFOUTPUT>)</FONT><BR>
	<INPUT TYPE="TEXT" NAME="Path"></P>

	<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><B>No. of locations</B></FONT><BR>
	<INPUT TYPE="TEXT" NAME="Locations" SIZE="3" MAXLENGTH="3" VALUE="1"></P>

	<P><INPUT TYPE="SUBMIT" VALUE="Continue"></P>

	</FORM>

<CFELSE>

	<CFTRANSACTION>

		<CFSET NewPageID = CreateUUID()>

		<CFQUERY datasource="cfx">
		INSERT INTO tblPubPg (strPgID, strTmpPth, intMaxLoc)
		VALUES ('#NewPageID#', '#Path#', #Locations#)
		</CFQUERY>

	</CFTRANSACTION>

   	<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1">Page created successfully! You can now schedule objects to appear on <CFOUTPUT>#Path#</CFOUTPUT>.</FONT></P>

	<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><A HREF="index.cfm">Go back</A></FONT></P>

</CFIF>

</BODY>
</HTML>