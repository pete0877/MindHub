<CFSET Session.AddContent.ReqContentTypes = "">
<CFSET Session.AddContent.ReqContentTypeNames = "">
<CFSET Session.AddContent.ReqContentTypeFile = "">
<CFSET Session.AddContent.OptContentTypes = "">
<CFSET Session.AddContent.OptContentTypeNames = "">
<CFSET Session.AddContent.OptContentTypeFile = "">
<CFSET Session.AddContent.ObjectID = ObjectID>

<CFQUERY datasource="cfx" NAME="GetObject">
SELECT * FROM tblPbObj
WHERE strObjID = '#ObjectID#'
</CFQUERY>

<CFSET Session.AddContent.ClassID = GetObject.strClsIDFK>

<CFQUERY datasource="cfx" NAME="GetOptContentTypes">
SELECT * FROM tblPbClD, tblPbCTp
WHERE strTypIDF2 = strTypID
	AND strClsIDF2 = '#GetObject.strClsIDFK#'
</CFQUERY>

<CFQUERY datasource="cfx" NAME="GetCurrContentTypes">
SELECT strTypIDFK FROM tblPbCnt
WHERE strObjIDFK = '#ObjectID#'
</CFQUERY>

<CFSET CurrContentList = ValueList(GetCurrContentTypes.strTypIDFK)>

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

<CFLOOP QUERY="GetOptContentTypes">
	<CFIF ListFind(CurrContentList, strTypID) IS 0>
		<CFOUTPUT>

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
</CFLOOP>

</TABLE>

<P><INPUT TYPE="SUBMIT" VALUE="Continue"></P>

</FORM>

</BODY>
</HTML>
