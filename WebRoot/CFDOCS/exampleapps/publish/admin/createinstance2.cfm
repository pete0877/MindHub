<CFIF NOT IsDefined("PageID")>
	<CFIF IsDefined("ObjectID")>
    	<CFLOCATION URL="createinstance.cfm?ObjectID=#ObjectID#">
    <CFELSE>
    	<CFLOCATION URL="index.cfm">
    </CFIF>
</CFIF>

<CFQUERY datasource="cfx" NAME="GetPage">
SELECT * FROM tblPubPg
WHERE strPgID = '#PageID#'
</CFQUERY>

<!--- Just in case we have to skip to createinstance3.cfm (this will
	occur if the page that was selected only has one location
	available), since createinstance3.cfm refers to TemplatePath
	and not GetPage.TemplatePath --->
<CFSET TemplatePath = GetPage.strTmpPth>

<CFIF GetPage.intMaxLoc IS 1>

	<CFINCLUDE TEMPLATE="createinstance3.cfm">

<CFELSE>

	<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">
	
	<HTML>
	<HEAD>
		<TITLE>Add Instance</TITLE>
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
			<CFOUTPUT><A HREF="properties.cfm?ObjectID=#ObjectID#">Return to Object #ObjectID#</A></CFOUTPUT>
		</FONT>
		</TD>
	</TR>
	</TABLE>

	<P><IMG SRC="images/createinstance.gif" WIDTH=131 HEIGHT=20 BORDER=0 ALT="Create Instance"></P>
	
	<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><CFOUTPUT>Creating an instance for <B>Object #ObjectID#</B> on <B>#GetPage.TemplatePath#</B>...</CFOUTPUT></P>

	<BR>

	<FORM ACTION="createinstance3.cfm" METHOD="POST">
	
	<CFOUTPUT>
		<INPUT TYPE="HIDDEN" NAME="ObjectID" VALUE="#ObjectID#">
		<INPUT TYPE="HIDDEN" NAME="PageID" VALUE="#PageID#">
		<INPUT TYPE="HIDDEN" NAME="TemplatePath" VALUE="#GetPage.strTmpPth#">
	</CFOUTPUT>
	
	<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Select a location:</B></FONT></P>
	
	<P><SELECT NAME="Location" SIZE="3">
	<CFLOOP FROM="1" TO="#GetPage.intMaxLoc#" INDEX="CurrLocation">
		<OPTION><CFOUTPUT>#CurrLocation#</CFOUTPUT>
	</CFLOOP>
	</SELECT></P>
	
	<P><INPUT TYPE="SUBMIT" VALUE="Continue"></P>
	
	</FORM>
	
	</BODY>
	</HTML>

</CFIF>