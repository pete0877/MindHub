<!--- There should be a ClassID... if this isn't defined, we're in the
	wrong place --->
<CFIF NOT IsDefined("ClassID")>
	<CFLOCATION URL="createobject.cfm" ADDTOKEN="NO">
	<CFABORT>
</CFIF>

<CFHEADER Name="Expires" Value="#Now()#">
<CFHEADER NAME="pragma" VALUE="no-cache">

<!--- Initialize the Session variables we're going to need --->
<CFSET Session.AddContent.ReqContentTypes = "">
<CFSET Session.AddContent.ReqContentTypeNames = "">
<CFSET Session.AddContent.ReqContentTypeFile = "">
<CFSET Session.AddContent.OptContentTypes = "">
<CFSET Session.AddContent.OptContentTypeNames = "">
<CFSET Session.AddContent.OptContentTypeFile = "">

<!--- Initialize session variables so we always know what class
	we're dealing with, and what object (if any) this content
	should be added (or modified) to --->
<CFSET Session.AddContent.ClassID = ClassID>
<CFIF IsDefined("ObjectID")>
	<CFSET Session.AddContent.ObjectID = ObjectID>
</CFIF>

<!--- Find all the content types that are required for this class --->
<CFQUERY datasource="cfx" NAME="GetRequiredContentTypes">
SELECT * FROM tblPbClD, tblPbCTp 
WHERE strTypIDF2 =strTypID
	AND strClsIDF2 = '#ClassID#'
	AND binIsReqd = 1
ORDER BY strClsItmI
</CFQUERY>

<!--- Set the list of required content types into a session variable --->
<CFSET Session.AddContent.ReqContentTypes = ValueList(GetRequiredContentTypes.strTypID)>
<CFSET Session.AddContent.ReqContentTypeNames = ValueList(GetRequiredContentTypes.strTypName)>
<CFSET Session.AddContent.ReqContentTypeFile = ValueList(GetRequiredContentTypes.binTypFile)>

<!--- Retrieve the optional content types so the user can choose --->
<CFQUERY datasource="cfx" NAME="GetOptionalContentTypes">
SELECT * FROM tblPbClD, tblPbCTp 
WHERE strTypIDF2 = strTypID
	AND strClsIDF2 = '#ClassID#'
	AND binIsReqd = 0
ORDER BY strClsItmI
</CFQUERY>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>Add Content</TITLE>
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

<P><IMG SRC="images/addcontent.gif" WIDTH=109 HEIGHT=20 BORDER=0 ALT="Add Content"></P>

<FORM ACTION="addcontent2.cfm" METHOD="POST">

<TABLE BORDER="1" CELLPADDING="5" CELLSPACING="2">

<TR>
	<TD COLSPAN="4"><FONT FACE="Helvetica"><B>Required Content</B></FONT></TD>
</TR>

<CFIF GetRequiredContentTypes.RecordCount IS 0>
<TR>
	<TD COLSPAN="4"><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">(no required content)</FONT></TD>
</TR>
<CFELSE>
	<CFOUTPUT QUERY="GetRequiredContentTypes">
		<TR>
			<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
				<INPUT TYPE="checkbox" onClick="this.checked = true" CHECKED>
			</FONT></TD>
			<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>#strTypName#</B></FONT></TD>
			<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">#strTypDesc#<BR></FONT></TD>
			<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
				<CFIF binTypFile IS 1>Binary<CFELSE>Text/HTML</CFIF>
			</FONT></TD>
		</TR>
	</CFOUTPUT>
</CFIF>

<TR>
	<TD COLSPAN="4"><FONT FACE="Helvetica"><B>Optional Content</B></FONT></TD>
</TR>
<CFIF GetOptionalContentTypes.RecordCount IS 0>
<TR>
	<TD COLSPAN="4"><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">(no optional content)</FONT></TD>
</TR>
<CFELSE>
	<CFOUTPUT QUERY="GetOptionalContentTypes">
		<TR>
			<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
				<INPUT TYPE="checkbox" NAME="OptContentTypes" VALUE="#strTypID#">
			</FONT></TD>
			<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>#strTypName#</B></FONT></TD>
			<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">#strTypDesc#<BR></FONT></TD>
			<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
				<CFIF binTypFile IS 1>Binary File<CFELSE>Text/HTML</CFIF>
			</FONT></TD>
		</TR>
	</CFOUTPUT>
</CFIF>

</TABLE>

<P><INPUT TYPE="SUBMIT" VALUE="Continue"></P>

</FORM>

</BODY>
</HTML>