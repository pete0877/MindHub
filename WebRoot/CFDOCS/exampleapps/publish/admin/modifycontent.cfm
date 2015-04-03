<CFIF ListLen(ContentIDList) IS 1>
	<CFIF evaluate("C" & Replace(ContentIDList,"-","_","ALL")) IS "|0">
	<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">
	
	<HTML>
	<HEAD>
		<TITLE>Upload File</TITLE>
	</HEAD>
	
	<BODY BGCOLOR="#660000" TEXT="#FFFFFF" LINK="#FFFF00" VLINK="#FFFF00">
	
	<P><IMG SRC="images/modifycontent.gif" WIDTH=132 HEIGHT=25 BORDER=0 ALT="Modify Content"></P>
	
	<FORM ACTION="modifycontent2.cfm" METHOD="POST" ENCTYPE="multipart/form-data">
	
		<CFOUTPUT>
			<INPUT TYPE="HIDDEN" NAME="ContentIDList" VALUE="#Replace(ContentIDList,"-","_","ALL")#">
		</CFOUTPUT>
	
		<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Select a file...</B></FONT><BR>
		<INPUT TYPE="FILE" NAME="UploadedFile"></P>
	
		<P><INPUT TYPE="SUBMIT" VALUE="Continue"></P>
	
	</FORM>
	
	</BODY>
	</HTML>
	<CFABORT>
	</CFIF>
</CFIF>

<CFPARAM NAME="ContentIDList" DEFAULT="">

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>Modify Content</TITLE>
<STYLE TYPE="text/css">
<!-- A {text-decoration: none} -->
</STYLE>
</HEAD>

<BODY BGCOLOR="#660000" TEXT="#FFFFFF" LINK="#FFFF00" VLINK="#FFFF00">

<P><IMG SRC="images/modifycontent.gif" WIDTH=132 HEIGHT=25 BORDER=0 ALT="Modify Content"></P>

<CFIF ContentIDList IS "">

	<FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Error:</B><BR>
	Nothing to do...</FONT>

<CFELSE>

	<CFLOOP LIST="#ContentIDList#" INDEX="CurrID">
	
		<CFSET Data = evaluate("C" & Replace(CurrID,"-","_","ALL"))>
		<CFQUERY datasource="cfx" NAME="UpdateContent">
		UPDATE tblPbCnt
		SET glbData = '#Data#'
		WHERE strCntID = '#CurrID#'
		</CFQUERY>
	
	</CFLOOP>
	
	<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">Your changes have been made.</FONT></P>

	<CFQUERY datasource="cfx" NAME="GetObjectID">
	SELECT strObjIDFK FROM tblPbCnt
	WHERE strCntID = '#ListGetAt(ContentIDList,1)#'
	</CFQUERY>
	
	<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
	<CFOUTPUT><A HREF="properties.cfm?ObjectID=#GetObjectID.strObjIDFK#">Back to Object #GetObjectID.strObjIDFK#</A></CFOUTPUT>
	</FONT></P>

</CFIF>

</BODY>
</HTML>