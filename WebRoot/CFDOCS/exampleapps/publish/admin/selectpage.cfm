<CFQUERY datasource="cfx" NAME="GetPages">
SELECT * FROM tblPubPg
ORDER BY strPgID
</CFQUERY>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>Select Page</TITLE>
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

<P><IMG SRC="images/selectpage.gif" WIDTH=97 HEIGHT=24 BORDER=0 ALT="Select Page"></P>

<FORM ACTION="viewpage.cfm" METHOD="GET">

<TABLE BORDER="1" CELLPADDING="5" CELLSPACING="2">
<TR>
	<TD></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><B>ID</B></FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><B>Page Name</B></FONT></TD>
</TR>

<CFOUTPUT QUERY="GetPages">

<TR>
	<TD><INPUT TYPE="RADIO" NAME="PageID" VALUE="#strPgID#" onClick="submit()"></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1">#strPgID#</FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1">#strTmpPth#</FONT></TD>
</TR>

</CFOUTPUT>

</TABLE>

</FORM>

</BODY>
</HTML>
