<!--- Test for localhost, and abort to message window if not --->

<CFParam Name = "CGI.Host" default="">

<CFIF CGI.Host is '' OR NOT (CGI.Host IS 'localhost' OR CGI.Host IS '127.0.0.1')>


	<CFINCLUDE template="/cfdocs/sorry.htm">

	<CFABORT>

</CFIF>


<CFAPPLICATION NAME="Tack2Admin" SESSIONMANAGEMENT="Yes" SESSIONTIMEOUT="#CreateTimeSpan(0,2,0,0)#">

<!--- Set to use for qs --->
<CFSET Application.HeadLineTypeID = "EA35ADB1-7CBA-11D3-A926005004218998">
<CFSET Application.TeaserTypeID = "EA35ADB2-7CBA-11D3-A926005004218998">

<!--- Only let this directory be used if the user is on the server --->
<CFIF CGI.REMOTE_ADDR NEQ "127.0.0.1" AND 0>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<HTML>
<HEAD>
	<TITLE>Access denied.</TITLE>
</HEAD>

<BODY BGCOLOR="#000066" TEXT="#FFFFFF" LINK="#FFFF00" VLINK="#FFFF00">

<P><FONT FACE="Helvetica" SIZE="+3"><B>Sorry...</B></FONT></P>

<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1">
For security reasons, access to this portion of the<BR>
publishing example application has been restricted<BR>
to the IP address 127.0.0.1. That means you must be<BR>
sitting at the server and using 127.0.0.1 as the server<BR>
name to access this directory.</FONT></P>

<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1">
If you are on the server, try <A HREF="http://127.0.0.1<CFOUTPUT>#CGI.PATH_INFO#?#CGI.QUERY_STRING#</CFOUTPUT>">this link</A>.
</FONT></P>

</BODY>
</HTML>
<CFABORT>
</CFIF>
