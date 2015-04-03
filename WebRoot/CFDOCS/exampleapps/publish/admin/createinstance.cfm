<CFQUERY datasource="cfx" NAME="GetPages">
SELECT * FROM tblPubPg
ORDER BY strTmpPth
</CFQUERY>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>Add Instance</TITLE>
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
		<CFOUTPUT><A HREF="properties.cfm?ObjectID=#ObjectID#">Return to Object #ObjectID#</A></CFOUTPUT>
	</FONT>
	</TD>
</TR>
</TABLE>

<P><IMG SRC="images/createinstance.gif" WIDTH=131 HEIGHT=20 BORDER=0 ALT="Create Instance"></P>

<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><CFOUTPUT>Creating instance for <B>Object #ObjectID#</B>...</CFOUTPUT></FONT></P>

<BR>

<FORM ACTION="createinstance2.cfm" METHOD="POST">

<CFOUTPUT><INPUT TYPE="HIDDEN" NAME="ObjectID" VALUE="#ObjectID#"></CFOUTPUT>

<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Please select a page for this instance:</B></FONT>

<P><CFOUTPUT><SELECT NAME="PageID" SIZE="#Max(Min(GetPages.RecordCount,20),2)#"></CFOUTPUT>
<CFOUTPUT QUERY="GetPages">
	<OPTION VALUE="#strPgID#">#strTmpPth#
</CFOUTPUT>
</SELECT></P>

<P><INPUT TYPE="SUBMIT" VALUE="Continue"></P>

</FORM>

</BODY>
</HTML>
