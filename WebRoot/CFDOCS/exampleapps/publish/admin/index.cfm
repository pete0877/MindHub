<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>Content Administrator</TITLE>
<STYLE TYPE="text/css">
<!-- A {text-decoration: none} -->
</STYLE>
</HEAD>

<BODY BGCOLOR="#660000" TEXT="#FFFFFF" LINK="#FFFF00" VLINK="#FFFF00">

<BR>

<P ALIGN="CENTER"><IMG SRC="images/contentadministrator.gif" WIDTH=307 HEIGHT=29 BORDER=0 ALT="Content Administrator"></P>

<P>&nbsp;</P>

<TABLE BORDER="0" CELLPADDING="2" CELLSPACING="3" ALIGN="CENTER">
<TR>
	<TD ALIGN="CENTER"><FONT FACE="Helvetica"><B><A HREF="getobject.cfm">View Objects</A></B></FONT></TD>
</TR>
<TR>
	<TD ALIGN="CENTER"><FONT FACE="Helvetica"><B><A HREF="createobject.cfm">Create New Object</A></B></FONT></TD>
</TR>
</TABLE>

<P>

<TABLE BORDER="0" CELLPADDING="2" CELLSPACING="3" ALIGN="CENTER">
<TR>
	<TD ALIGN="CENTER"><FONT FACE="Helvetica"><B><A HREF="selectpage.cfm">Select Page</A></B></FONT></TD>
</TR>
<TR>
	<TD ALIGN="CENTER"><FONT FACE="Helvetica"><B><A HREF="createpage.cfm">Create New Page</A></B></FONT></TD>
</TR>
<TR>
	<TD ALIGN="CENTER"><FONT FACE="Helvetica"><B><A HREF="regpage.cfm">Register Existing Page</A></B></FONT></TD>
</TR>
</TABLE>

<P>&nbsp;</P>

<TABLE BORDER="2" CELLPADDING="7" CELLSPACING="0" ALIGN="CENTER">
<TR><TD>
<FONT FACE="MS Sans Serif, Helvetica" SIZE="-1">

<P>If Administrator Mode is turned on, you <BR>
can edit/delete objects directly from<BR>
your published pages.</P>

<FORM ACTION="index.cfm">

<DIV ALIGN="CENTER"><P><B>Browse in Administrator Mode</B></P>

<P><INPUT TYPE="RADIO" NAME="URL" VALUE="adminmodeon.cfm" onClick="location.href = this.value"
<CFIF IsDefined("Cookie.PubAdminMode")>CHECKED</CFIF>
>On
<INPUT TYPE="RADIO" NAME="URL" VALUE="adminmodeoff.cfm" onClick="location.href = this.value"
<CFIF NOT IsDefined("Cookie.PubAdminMode")>CHECKED</CFIF>
>Off</P>

</FORM>
</FONT>
</TD></TR>
</TABLE>

</BODY>
</HTML>