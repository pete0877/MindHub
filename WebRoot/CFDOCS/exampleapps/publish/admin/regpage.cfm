<!--- Separator is based on OS --->
<CFLOCK SCOPE="Server" TIMEOUT="30" TYPE="ReadOnly">
	<CFIF Server.OS.Name CONTAINS "Windows">
		<CFSET Sep = "\">
	<CFELSE>
		<CFSET Sep = "/">	
	</CFIF>
</CFLOCK>

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

<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1">
If you have created a Cold Fusion page that can handle scheduleable<BR>
content, you can register that template with the content publishing<BR>
system using this page.</FONT></P>

<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1">
Note that this is only necessary if you want the page to display objects<BR>
using instances (the &lt;CF_ShowContent&gt; tag). If you just want to display<BR>
an object you can use the &lt;CF_ShowObject&gt; tag (see the documentation<BR>
for more information).</FONT></P>

<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1">
The file must exist in the directory <CFOUTPUT><B>#ExpandPath("..#Sep#")#</B></CFOUTPUT>.</FONT></P>

<P>&nbsp;</P>

<FORM ACTION="regpage2.cfm" METHOD="POST">

<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">Template Name</FONT><BR>
<INPUT TYPE="TEXT" NAME="Path"></P>

<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">No. of locations</FONT><BR>
<INPUT TYPE="TEXT" NAME="Locations" SIZE="3" MAXLENGTH="3" VALUE="1"></P>

<P><INPUT TYPE="SUBMIT" VALUE="Continue"></P>

</FORM>

</BODY>
</HTML>