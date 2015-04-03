<CFPARAM NAME="URL.ContentID" DEFAULT="">
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
	<TITLE>Modify Content</TITLE>
<STYLE TYPE="text/css">
<!-- A {text-decoration: none} -->
</STYLE>
</HEAD>

<BODY BGCOLOR="#660000" TEXT="#FFFFFF" LINK="#FFFF00" VLINK="#FFFF00">

<P><IMG SRC="images/modifycontent.gif" WIDTH=132 HEIGHT=25 BORDER=0 ALT="Modify Content"></P>

<CFIF URL.ContentID IS "">

	<FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Error:</B><BR>
	Nothing to do...</FONT>

<CFELSE>

	<!--- Find out whether or not this content is binary data --->

	<FORM ACTION="modifycontent.cfm" METHOD="POST">

	<CFQUERY NAME="GetContent" datasource="cfx">
	SELECT * FROM tblPbCnt, tblPbCTp 
	WHERE strTypID = strTypIDFK
		AND strCntID = '#URL.ContentID#'
	</CFQUERY>

	<FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">
	
	<CFOUTPUT><P><B>Content Type:</B> #GetContent.strTypName#
	<INPUT TYPE="HIDDEN" NAME="ContentIDList" VALUE="#GetContent.strCntID#"></CFOUTPUT>

	<BR><BR>

	<B>Data</B><BR>

	<CFIF GetContent.binTypFile IS FALSE>

		<CFOUTPUT>
			<TEXTAREA NAME="C#Replace(URL.ContentID,"-","_","ALL")#" COLS="50" ROWS="6" WRAP="VIRTUAL">#HTMLEditFormat(GetContent.glbData)#</TEXTAREA>
		</CFOUTPUT>

	<CFELSE>

		<CFDIRECTORY ACTION="LIST" DIRECTORY="#GetDirectoryFromPath(GetTemplatePath())#..#Sep#binarydata" NAME="BinDir">

		<CFOUTPUT>
			<SELECT NAME="C#Replace(URL.ContentID,"-","_","ALL")#" SIZE="#Min(10,BinDir.RecordCount)#">
		</CFOUTPUT>
			<OPTION VALUE="|0">Upload new file...
			<OPTION VALUE="|1">------------------------
		<CFOUTPUT QUERY="BinDir">
			<CFIF NOT (Name IS "." OR Name IS "..")>
				<CFIF Name IS GetContent.glbData>
					<OPTION SELECTED>#Name#
				<CFELSE>
					<OPTION>#Name#
				</CFIF>
			</CFIF>
		</CFOUTPUT>
		</SELECT>

	</CFIF>

	</FONT>

	<P><INPUT TYPE="SUBMIT" VALUE="Modify"> <INPUT TYPE="RESET" VALUE="Revert"></P>

	</FORM>

</CFIF>

</BODY>
</HTML>
