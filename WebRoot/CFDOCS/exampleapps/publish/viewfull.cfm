<CF_getobject ObjectID="#URL.ObjectID#">

<!--- Set default colors --->
<CFPARAM NAME="Object.BackgroundImage" DEFAULT="bg1.gif">
<CFPARAM NAME="Object.BGColor" DEFAULT="##000000">
<CFPARAM NAME="Object.TextColor" DEFAULT="##FFFFFF">
<CFPARAM NAME="Object.LinkColor" DEFAULT="##ffcc00">
<CFPARAM NAME="Object.VLinkColor" DEFAULT="gray">
<CFPARAM NAME="Object.ALinkColor" DEFAULT="##ffcc00">
<CFPARAM NAME="Object.Image" DEFAULT="full.jpg">

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>TACK2</TITLE>
	<LINK REL="STYLESHEET" TYPE="text/css" HREF="stylesheet.css">
</HEAD>

<CFOUTPUT>
<BODY
	BGCOLOR="#Object.BGColor#"
	text="#Object.TextColor#"
	link="#Object.LinkColor#"
	vlink="#Object.VLinkColor#"
	alink="#Object.ALinkColor#"
	<CFIF Object.BackgroundImage NEQ "">
		BACKGROUND="binarydata/#Object.BackgroundImage#"
	</CFIF>
	LEFTMARGIN=5
	TOPMARGIN=0>
</CFOUTPUT>

<!--- Boxes --->

<SPAN STYLE="position: absolute; left: 40px; top: 120px;">
<IMG SRC="images/full_boxHoriz.gif">
</SPAN>

<SPAN STYLE="position: absolute; left: 425px; top: 100px;">
<IMG SRC="images/full_boxVert.gif">
</SPAN>


<!--- Previous --->
<SPAN STYLE="position: absolute; left: 42px; top: 122px;">
<A href='javascript:history.back()'><img src="images/prev.gif" width=20 height=20 alt="" border="0" valign="middle"></a>
</SPAN>

<SPAN STYLE="position: absolute; left: 63px; top: 121px;">
<font face="arial" size="-1">previous</font>
</SPAN>

<!--- Image --->
<CFOUTPUT>
	<CFIF IsDefined("Object.Image")>
		<SPAN STYLE="position: absolute; left: 120px; top: 130px;">
		<IMG SRC="binarydata/#Object.Image#">
		</SPAN>
	</CFIF>
</cfoutput>	

<!--- TACK2 logo --->
<SPAN STYLE="position: absolute; left: 40px; top: 75px;">
<img src="images/tack2_sm.gif" width=173 height=39 alt="" border="0">
</SPAN>


<!--- Content --->
<SPAN STYLE="position: absolute; left: 165px; top: 310px; width: 300px;">
	<CFOUTPUT>

	<DIV CLASS="HeadlineFull">#Object.Headline#</DIV>

	&nbsp;<BR>

	<CFIF IsDefined("Object.Body")><DIV CLASS="BodyFull">
		<CFIF IsDefined("Object.InlineImage")>
			<IMG SRC="binarydata/#Object.InlineImage#">
			
		</CFIF>
		#Object.Body#</DIV>
	</CFIF>

	&nbsp;<BR>

	<CFIF IsDefined("Object.HREF")>
		<DIV CLASS="LinkFull"><B>Go to:</B> <A HREF="#Object.HREF#">#Object.HREF#</A><BR></DIV>
	</CFIF>
	<CFIF IsDefined("Object.File")>
		<DIV CLASS="LinkFull"><B>Download:</B> <A HREF="binarydata/#Replace(Replace(URLEncodedFormat(Object.File),"%2E",".","ALL"),"+","%20","ALL")#">#Object.File#</A><BR></DIV>
	</CFIF>

	<!--- If browser is in Admin mode, display editing icons --->
	<CFIF IsDefined("Cookie.PubAdminMode")>
		<BR><A HREF="admin/properties.cfm?ObjectID=#ObjectID#"><IMG SRC="open.gif" WIDTH=16 HEIGHT=14 BORDER=0 ALT="Open" ALIGN="TOP"></A> <A HREF="admin/deleteobject.cfm?ObjectID=#ObjectID#" onClick="return confirm('This will PERMANENTLY delete this object!\n\nSure you want to continue?')"><IMG SRC="delete.gif" WIDTH=15 HEIGHT=16 BORDER=0 ALT="Delete" ALIGN="TOP"></A>
	</CFIF>

	</CFOUTPUT>

	<br><Br>

</div>
</SPAN>

</BODY>
</HTML>