<CFHEADER NAME="pragma" VALUE="no-cache">

<CFQUERY datasource="cfx" NAME="GetPage">
SELECT * FROM tblPubPg WHERE strPgID = '#PageID#'
</CFQUERY>

<CFQUERY datasource="cfx" NAME="GetInstances">
SELECT * FROM tblPInt pint, tblPbCnt cnt
WHERE pint.strObjIDFK = cnt.strObjIDFK
	AND cnt.strTypIDFK = '#Application.HeadLineTypeID#'
	AND strPgIDFK = '#PageID#'
ORDER BY intPrior DESC
</CFQUERY>

<CFQUERY datasource="cfx" NAME="GetTypeName">
SELECT * FROM tblPbCTp
WHERE strTypID = '#Application.HeadLineTypeID#'
</CFQUERY>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>Page Details</TITLE>
<STYLE TYPE="text/css">
<!-- A {text-decoration: none} -->
</STYLE>
</HEAD>

<BODY BGCOLOR="#660000" TEXT="#FFFFFF" LINK="#FFFF00" VLINK="#FFFF00">

<TABLE ALIGN="RIGHT" CELLPADDING="0" CELLSPACING="0">
<TR>
	<TD ALIGN="RIGHT">
	<FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
		<A HREF="index.cfm">Administrator Home</A><BR>
		<A HREF="selectpage.cfm">Select New Page</A><BR>
	</FONT>
	</TD>
</TR>
</TABLE>

<P><IMG SRC="images/pagedetails.gif" WIDTH=102 HEIGHT=24 BORDER=0 ALT="Page Details"></P>

<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Click on an ObjectID to bring up that object's Property page.</B></FONT></P>

<TABLE BORDER="1" CELLPADDING="5" CELLSPACING="2">
<TR>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Object ID</B></FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B><CFOUTPUT>#GetTypeName.strTypName#</CFOUTPUT></B></FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Priority</B></FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Start</B></FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>End</B></FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Priority</B></FONT></TD>
	<TD></TD>
</TR>
<CFOUTPUT QUERY="GetInstances">
<TR>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><A HREF="properties.cfm?ObjectID=#strObjIDFK#">#strObjIDFK#</A></FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">#glbData#</FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">#intPrior#</FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
		<CFIF dtStrtTm IS "">Beginning of time
		<CFELSE>#dtStrtTm#</CFIF>
		</FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
		<CFIF dtEndTm IS "">End of time
		<CFELSE>#dtEndTm#</CFIF>
		</FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">#intPrior#</FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><A HREF="deleteinstance.cfm?InstanceID=#strInsID#" onClick="return confirm('This will delete the instance (the object will remain intact).\n\nPress OK to continue.')">Delete</A></FONT></TD>
</TR>
</CFOUTPUT>
</TABLE>

<P>&nbsp;</P>

<P>&nbsp;</P>

<!--- <TABLE BORDER="2" CELLPADDING="7" CELLSPACING="0">
<TR>
	<TD>
	<CFOUTPUT>
	<FORM ACTION="deletepage.cfm" METHOD="POST">
	<INPUT TYPE="HIDDEN" NAME="PageID" VALUE="#PageID#">

	<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><B>Delete Page</B></FONT></P>
	
	<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
	<INPUT TYPE="RADIO" NAME="FileDelete" VALUE="0" CHECKED>Remove from database only<BR>
	<INPUT TYPE="RADIO" NAME="FileDelete" VALUE="1">Remove from database AND delete <B>#GetPage.TemplatePath#</B> from hard drive
	</FONT></P>
	
	<P><INPUT TYPE="SUBMIT" VALUE="Delete"></P>
	
	</FORM>
	</CFOUTPUT>
	</TD>
</TR>
</TABLE> --->

</BODY>
</HTML>
