<!--- Initialize the Session variables we're going to need --->
<CFSET Session.AddContent.ClassID = "">
<CFSET Session.AddContent.ObjectID = "">

<!--- Fetch data classes --->
<CFQUERY datasource="cfx" NAME="GetDataClasses">
SELECT * FROM tblPbDCl
</CFQUERY>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>Create Object</TITLE>
<STYLE TYPE="text/css">
<!-- A {text-decoration: none} -->
</STYLE>
</HEAD>

<BODY BGCOLOR="#660000" TEXT="#FFFFFF" LINK="#FFFF00" VLINK="#FFFF00">

<TABLE ALIGN="RIGHT" CELLPADDING="0" CELLSPACING="0">
<TR>
	<TD>
	<FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
		<A HREF="index.cfm">Administrator Home</A>
	</FONT>
	</TD>
</TR>
</TABLE>

<P><IMG SRC="images/createobject.gif" WIDTH=144 HEIGHT=28 BORDER=0 ALT="Create Object"></P>

<FORM ACTION="addcontent.cfm" METHOD="POST">

<P><FONT FACE="Helvetica"><B>1. Choose an object class:</B></FONT></P>

<FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">

<P><CFOUTPUT QUERY="GetDataClasses">
	<INPUT TYPE="RADIO" NAME="ClassID" VALUE="#strClsID#">#strClsName#<BR>
</CFOUTPUT></P>

</FONT>

<P><INPUT TYPE="SUBMIT" VALUE="Continue"></P>

</FORM>

</BODY>
</HTML>
