<CFQUERY datasource="cfx" NAME="GetObjects">
SELECT * FROM tblPbObj, tblPbCnt, tblPbDCl
WHERE strObjID = strObjIDFK
	AND strClsIDFK = strClsID
	AND strTypIDFK = '#Application.HeadLineTypeID#'
<CFIF IsDefined("SortOrder")>
	ORDER BY #SortOrder#
<CFELSE>
	ORDER BY dtCreated DESC
</CFIF>
</CFQUERY>

<CFQUERY datasource="cfx" NAME="GetTypeName">
SELECT * FROM tblPbCTp
WHERE strTypID = '#Application.HeadLineTypeID#'
</CFQUERY>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>Select Object</TITLE>
<STYLE TYPE="text/css">
<!-- A {text-decoration: none} -->
</STYLE>
</HEAD>

<BODY BGCOLOR="#660000" TEXT="#FFFFFF" LINK="#FFFF00" VLINK="#FFFF00" onLoad="document.Sort.SortOrder.selectedIndex = 0">

<TABLE ALIGN="RIGHT" CELLPADDING="0" CELLSPACING="0">
<TR>
	<TD ALIGN="RIGHT">
	<FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
		<A HREF="index.cfm">Administrator Home</A>
	</FONT>
	</TD>
</TR>
</TABLE>

<P><IMG SRC="images/selectobject.gif" WIDTH=112 HEIGHT=24 BORDER=0 ALT="Select Object"></P>

<FORM ACTION="getobject.cfm" METHOD="GET" NAME="Sort">
	<SELECT NAME="SortOrder" onChange="submit()">
		<OPTION>Sort By:
		<OPTION VALUE="strObjID">Object ID
		<OPTION VALUE="dtCreated DESC">Date Created
		<OPTION VALUE="dtUpdated DESC">Date Updated
	</SELECT>
</FORM>

<FORM ACTION="properties.cfm" METHOD="GET">

<TABLE BORDER="1" CELLPADDING="5" CELLSPACING="2">
<TR>
	<TD></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><B>ID</B></FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Class</B></FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><B>Created</B></FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><B>Last Modified</B></FONT></TD>
	<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><B><CFOUTPUT>#GetTypeName.strTypName#</CFOUTPUT></B></FONT></TD>
</TR>

<CFOUTPUT QUERY="GetObjects">
<TR>

	<TD>
	<INPUT TYPE="RADIO" NAME="ObjectID" VALUE="#strObjID#" onClick="submit()">
	</TD>

	<TD>
	<FONT FACE="MS Sans Serif, Helvetica" SIZE="-1">
	#strObjID#
	</FONT>
	</TD>

	<TD>
	<FONT FACE="MS Sans Serif, Helvetica" SIZE="-1">
	#strClsName#
	</FONT>
	</TD>

	<TD>
	<FONT FACE="MS Sans Serif, Helvetica" SIZE="-1">
	#DateFormat(dtCreated, "m/d/yy")#, #TimeFormat(dtCreated, "h:mm:ss tt")#</A>
	</FONT>
	</TD>

	<TD>
	<FONT FACE="MS Sans Serif, Helvetica" SIZE="-1">
	#DateFormat(dtUpdated, "m/d/yy")#, #TimeFormat(dtUpdated, "h:mm:ss tt")#</A>
	</FONT>
	</TD>

	<TD>
	<FONT FACE="MS Sans Serif, Helvetica" SIZE="-1">
	#glbData#&nbsp;
	</FONT>
	</TD>

</TR>
</CFOUTPUT>
</TABLE>

</FORM>

</BODY>
</HTML>
