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

<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1">Creating page of type <B><CFOUTPUT>#Form.Template#</CFOUTPUT></B>...</FONT></P>

<CFOUTPUT><P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1">Please provide a filename for the Cold Fusion template <BR>
that will be created (the extension must be .cfm).  The file<BR>
will be created in <B>http://#CGI.HTTP_HOST##Left(CGI.SCRIPT_NAME, Len(CGI.SCRIPT_NAME) - Len("admin/createpage2.cfm"))#</B>.</FONT></P></CFOUTPUT>

<FORM ACTION="createpage3.cfm" METHOD="POST">

<CFOUTPUT><INPUT TYPE="HIDDEN" NAME="Template" VALUE="#Form.Template#"></CFOUTPUT>

<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">Filename:</FONT><BR>
<INPUT TYPE="TEXT" NAME="Path" VALUE=".cfm"></P>

<P><INPUT TYPE="SUBMIT" VALUE="Continue"></P>

</FORM>

</BODY>
</HTML>