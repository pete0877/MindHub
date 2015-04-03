<CFHEADER NAME="pragma" VALUE="no-cache">

<CFIF NOT IsDefined("ObjectID")>
	<CFLOCATION URL="getobject.cfm" ADDTOKEN="NO">
</CFIF>

<CFQUERY datasource="cfx" NAME="GetObjectData">
SELECT * FROM tblPbObj, tblPbDCl
WHERE strClsIDFK = strClsID
	AND strObjID = '#ObjectID#'
</CFQUERY>

<CFQUERY datasource="cfx" NAME="GetHeadline">
SELECT * FROM tblPbCnt, tblPbCTp
WHERE strTypIDFK = strTypID
	AND strObjIDFK = '#ObjectID#'
	AND strTypIDFK = '#Application.HeadLineTypeID#'
</CFQUERY>

<CFQUERY datasource="cfx" NAME="GetTeaser">
SELECT * FROM tblPbCnt, tblPbCTp
WHERE strTypIDFK = strTypID
	AND strObjIDFK = '#ObjectID#'
	AND strTypIDFK = '#Application.TeaserTypeID#'
</CFQUERY>

<CFQUERY datasource="cfx" NAME="GetContent">
SELECT * FROM tblPbCnt, tblPbCTp
WHERE strTypIDFK = strTypID
	AND strObjIDFK = '#ObjectID#'
	AND NOT (strTypIDFK = '#Application.HeadLineTypeID#' OR strTypIDFK = '#Application.TeaserTypeID#')
ORDER BY strCntID
</CFQUERY>

<CFQUERY datasource="cfx" NAME="GetAllowedContentTypes">
SELECT * FROM tblPbClD
WHERE strClsIDF2 = '#GetObjectData.strClsID#'
</CFQUERY>

<CFQUERY datasource="cfx" NAME="GetInstances">
SELECT * FROM tblPInt, tblPubPg
WHERE strPgIDFK = strPgID
	AND strObjIDFK = '#ObjectID#'
ORDER BY strPgIDFK
</CFQUERY>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>Object Properties</TITLE>
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
		<A HREF="getobject.cfm">Choose New Object</A><BR>
		<CFOUTPUT><A HREF="deleteobject.cfm?ObjectID=#ObjectID#" onClick="return confirm('This will permanently delete this object.\n\nPress OK to continue.')">Delete Object</A></CFOUTPUT><BR>
	</FONT>
	</TD>
</TR>
</TABLE>

<P><IMG SRC="images/objectproperties.gif" WIDTH=146 HEIGHT=24 BORDER=0 ALT="Object Properties"></P>

<CFIF GetObjectData.RecordCount LT 1>

	<FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">Object <CFOUTPUT>#ObjectID#</CFOUTPUT> does not exist.</FONT>

<CFELSE>

	<CFOUTPUT QUERY="GetObjectData">
	
		<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
		<B>ObjectID:</B> #strObjID#<BR><BR>
		<B>Class:</B> #strClsName#<BR>
		<B>Created:</B> #DateFormat(dtCreated, "m/d/yy")#, #TimeFormat(dtCreated, "h:mm:ss tt")#<BR>
		<B>Modified:</B> #DateFormat(dtUpdated, "m/d/yy")#, #TimeFormat(dtUpdated, "h:mm:ss tt")#
		</FONT></P>

	</CFOUTPUT>

	<FORM ACTION="modifycontent.cfm" METHOD="POST">

	<CFOUTPUT QUERY="GetHeadline">
	
		<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>#strTypName#</B></FONT><BR>
		<FONT FACE="MS Sans Serif, Arial"><INPUT TYPE="TEXT" NAME="C#Replace(strCntID,"-","_","ALL")#" VALUE="#HTMLEditFormat(glbData)#" SIZE="40"></FONT></P>

		<INPUT TYPE="HIDDEN" NAME="ContentIDList" VALUE="#strCntID#">

	</CFOUTPUT>
	
	<CFOUTPUT QUERY="GetTeaser">
	
		<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>#strTypName#</B></FONT><BR>
		<TEXTAREA NAME="C#Replace(strCntID,"-","_","ALL")#" COLS="40" ROWS="6" WRAP="VIRTUAL">#HTMLEditFormat(glbData)#</TEXTAREA></P>

		<INPUT TYPE="HIDDEN" NAME="ContentIDList" VALUE="#strCntID#">

	</CFOUTPUT>

		<INPUT TYPE="SUBMIT" VALUE="Modify"> <INPUT TYPE="RESET" VALUE="Revert">
	
	</FORM>

	<BR><BR>

	<TABLE BORDER="1" CELLPADDING="5" CELLSPACING="2">
	<TR>
		<TD COLSPAN="4" BGCOLOR="#CCCCCC"><FONT COLOR="#000000" FACE="Verdana, Helvetica" SIZE="4"><B>Content</B></FONT></TD>
	</TR>
	<TR>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Content Type</B></FONT></TD>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Value</B></FONT></TD>
		<TD>&nbsp;</TD>
		<TD>&nbsp;</TD>
	</TR>
	<CFOUTPUT QUERY="GetContent">
	<TR>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">#strTypName#</FONT></TD>
		<CFIF binTypFile IS TRUE>
			<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><A HREF="../binarydata/#URLEncodedFormat(glbData)#">#glbData#</A></FONT></TD>
		<CFELSE>
			<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><CFIF Len(HTMLEditFormat(glbData)) GT 255>#Left(HTMLEditFormat(glbData),255)#&nbsp;...<CFELSE>#HTMLEditFormat(glbData)#</CFIF></FONT></TD>
		</CFIF>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><A HREF="modifycontentform.cfm?ContentID=#strCntID#">Modify</A></FONT></TD>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><A HREF="deletecontent.cfm?ContentID=#strCntID#" onClick="return confirm('This will permanently delete this information.\n\nPress OK to continue.')">Delete</A></FONT></TD>
	</TR>
	</CFOUTPUT>
	<CFIF GetContent.RecordCount + 2 LT GetAllowedContentTypes.RecordCount>
		<TR>
			<TD COLSPAN="4" ALIGN="RIGHT"><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><CFOUTPUT><A HREF="addmorecontent.cfm?ObjectID=#ObjectID#"></CFOUTPUT>Add Content</A></FONT></TD>
		</TR>
	</CFIF>
	</TABLE>

	<BR><BR>
	
	<TABLE BORDER="1" CELLPADDING="5" CELLSPACING="2">
	<TR>
		<TD COLSPAN="7" BGCOLOR="#CCCCCC"><FONT COLOR="#000000" FACE="Verdana, Helvetica" SIZE="4"><B>Instances</B></FONT></TD>
	</TR>
	<TR>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>PageID</B></FONT></TD>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Template</B></FONT></TD>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Location</B></FONT></TD>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Start</B></FONT></TD>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>End</B></FONT></TD>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Priority</B></FONT></TD>
		<TD>&nbsp;</TD>
	</TR>
	<CFOUTPUT QUERY="GetInstances">
	<TR>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><A HREF="viewpage.cfm?PageID=#strPgID#">#strPgID#</A></FONT></TD>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">#strTmpPth#</FONT></TD>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">#intLoc#</FONT></TD>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
			<CFIF dtStrtTm IS "">Beginning of time
			<CFELSE>#dtStrtTm#</CFIF>
			</FONT></TD>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
			<CFIF dtEndTm IS "">End of time
			<CFELSE>#dtEndTm#</CFIF>
			</FONT></TD>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">#intPrior#</FONT></TD>
		<TD><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><A HREF="deleteinstance.cfm?InstanceID=#strInsID#" onClick="return confirm('This will permanently delete this instance.\n\nPress OK to continue.')">Delete</A></FONT></TD>
	</TR>
	</CFOUTPUT>
	<TR>
		<TD COLSPAN="7" ALIGN="RIGHT"><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><CFOUTPUT><A HREF="createinstance.cfm?ObjectID=#ObjectID#"></CFOUTPUT>Create Instance</A></FONT></TD>
	</TR>
	</TABLE>
	
</CFIF>

</FONT>

</BODY>
</HTML>
