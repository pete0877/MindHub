<!--- This template gets user input for REQUIRED content types.
	The Session variable Session.AddContent.ReqContentTypes
	holds a list of content types to be added, and the variable
	URL.CurrPos indicates which item on the list we are dealing
	with. --->

<!--- Separator is based on OS --->
<CFLOCK SCOPE="Server" TIMEOUT="30" TYPE="ReadOnly">
	<CFIF Server.OS.Name CONTAINS "Windows">
		<CFSET Sep = "\">
	<CFELSE>
		<CFSET Sep = "/">	
	</CFIF>
</CFLOCK>

<CFSET CurrID = ListGetAt(Session.AddContent.OptContentTypes, URL.CurrPos)>
<CFSET CurrName = ListGetAt(Session.AddContent.OptContentTypeNames, URL.CurrPos)>

<!--- Find out whether this content type is a file --->
<CFQUERY datasource="cfx" NAME="GetContentType">
SELECT * FROM tblPbCTp
WHERE strTypID = '#CurrID#'
</CFQUERY>

<CFIF GetContentType.RecordCount NEQ 0>
	<CFIF "Headline,Teaser,Body" CONTAINS GetContentType.strTypName>
		<CFSET SampleText = "Enter your text...">
	<CFELSEIF GetContentType.strTypName IS "HREF">
		<CFSET SampleText = "http://your.server.com/">
	</CFIF>
</CFIF>
<CFPARAM NAME="SampleText" DEFAULT="">

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

<FORM ACTION="addcontent2.opt.cfm" METHOD="POST">

<CFOUTPUT>
	<INPUT TYPE="HIDDEN" NAME="CurrPos" VALUE="#URL.CurrPos#">
	<INPUT TYPE="HIDDEN" NAME="CurrItem" VALUE="#CurrID#">
</CFOUTPUT>

<TABLE BORDER="0" CELLPADDING="0" CELLSPACING="0">
<TR>

	<TD VALIGN="TOP" WIDTH="100">

	<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1">
	<CFLOOP LIST="#Session.AddContent.ReqContentTypeNames#" INDEX="TypeName">
		<FONT COLOR="#999999"><CFOUTPUT>#TypeName#</CFOUTPUT></FONT><BR>
	</CFLOOP>
	</FONT></P>

	<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1">
	<CFLOOP LIST="#Session.AddContent.OptContentTypeNames#" INDEX="TypeName">
		<CFIF TypeName IS CurrName>
			<B><CFOUTPUT>#TypeName#</CFOUTPUT></B><BR>
		<CFELSE>
			<FONT COLOR="#999999"><CFOUTPUT>#TypeName#</CFOUTPUT></FONT><BR>
		</CFIF>
	</CFLOOP>
	</FONT></P>

	</TD>

	<TD>&nbsp;&nbsp;&nbsp;</TD>

	<TD VALIGN="TOP">

	<CFIF GetContentType.binTypFile IS TRUE>

		<CFDIRECTORY ACTION="LIST" DIRECTORY="#ExpandPath("..#Sep#binarydata")#" NAME="BinDir">

		<CFOUTPUT><SELECT NAME="Type#Replace(CurrID,"-","_","ALL")#" SIZE="#Min(10,BinDir.RecordCount)#" onChange="if (this.selectedIndex == 1) this.selectedIndex = 0"></CFOUTPUT>
			<OPTION VALUE="|0">Upload new file...
			<OPTION VALUE="|1">------------------------
		<CFOUTPUT QUERY="BinDir">
			<CFIF NOT (Name IS "." OR Name IS "..")><OPTION>#Name#</CFIF>
		</CFOUTPUT>
		</SELECT>

	<CFELSE>
	
        <CFIF IsDefined("Session.AddContent.Content.Type" & Replace(CurrID,"-","_","ALL"))>
        	<CFSET PreviousText = evaluate("Session.AddContent.Content.Type" & Replace(CurrID,"-","_","ALL"))>
        <CFELSE>
        	<CFSET PreviousText = "">
        </CFIF>
	
		<CFOUTPUT>
			<CFIF PreviousText IS "">
				<TEXTAREA COLS="40" ROWS="12" NAME="Type#Replace(CurrID,"-","_","ALL")#">#SampleText#</TEXTAREA>
			<CFELSE>
				<TEXTAREA COLS="40" ROWS="12" NAME="Type#Replace(CurrID,"-","_","ALL")#">#evaluate("Session.AddContent.Content.Type" & Replace(CurrID,"-","_","ALL"))#</TEXTAREA>
			</CFIF>
		</CFOUTPUT>

	</CFIF>

	<P><INPUT TYPE="SUBMIT" VALUE="Next &gt;"></P>		

	</TD>
	
</TR>
</TABLE>

</FORM>

</BODY>
</HTML>
