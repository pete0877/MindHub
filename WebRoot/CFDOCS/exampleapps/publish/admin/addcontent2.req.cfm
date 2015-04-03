<CFIF evaluate("Type" & Replace(CurrItem,"-","_","ALL")) IS "|0">

	<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">
	
	<HTML>
	<HEAD>
		<TITLE>Upload File</TITLE>
	</HEAD>
	
	<BODY BGCOLOR="#660000" TEXT="#FFFFFF" LINK="#FFFF00" VLINK="#FFFF00">
	
	<P><IMG SRC="images/addcontent.gif" WIDTH=109 HEIGHT=20 BORDER=0 ALT="Add Content"></P>
	
	<FORM ACTION="addcontent2a.req.cfm" METHOD="POST" ENCTYPE="multipart/form-data">
	
		<CFOUTPUT>
			<INPUT TYPE="HIDDEN" NAME="CurrItem" VALUE="#CurrItem#">
			<INPUT TYPE="HIDDEN" NAME="CurrPos" VALUE="#CurrPos#">
		</CFOUTPUT>
	
		<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-2"><B>Select a file...</B></FONT><BR>
		<INPUT TYPE="FILE" NAME="UploadedFile"></P>
	
		<P><INPUT TYPE="SUBMIT" VALUE="Continue"></P>
	
	</FORM>
	
	</BODY>
	</HTML>

<CFELSE>

	<CFSET "Session.AddContent.Content.Type#Replace(CurrItem,"-","_","ALL")#" = #evaluate("Type" & Replace(CurrItem,"-","_","ALL"))#>

	<CFIF CurrPos IS ListLen(Session.AddContent.ReqContentTypes)>
		<CFIF Session.AddContent.OptContentTypes IS "">
			<CFLOCATION URL="addcontent3.cfm" ADDTOKEN="NO">
		<CFELSE>
			<CFLOCATION URL="addcontent.opt.cfm?CurrPos=1" ADDTOKEN="NO">
		</CFIF>
	<CFELSE>
		<CFSET NextPos = CurrPos + 1>
		<CFLOCATION URL="addcontent.req.cfm?CurrPos=#NextPos#" ADDTOKEN="NO">
	</CFIF>

</CFIF>