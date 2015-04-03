<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<HTML>
<HEAD>
	<TITLE></TITLE>
</HEAD>

<BODY BGCOLOR="#FFCC33">

<SPAN STYLE="position: absolute; left: 50px; top: 100px">

    <FONT FACE="Verdana, Helvetica" SIZE="2">
    
    <P><B>Welcome to the TACK2 online store...</B></P>
    
    <P>You'll want to use Microsoft Internet Explorer 4.0<BR>
    or Netscape 4.0 to view the store, as it uses style<BR>
    sheets and some fun DHTML.</P>

	<!--- If browser is Netscape or MSIE and version 4.x or greater... --->
	<CFIF CGI.HTTP_USER_AGENT CONTAINS "Mozilla" AND NOT (CGI.HTTP_USER_AGENT CONTAINS "1." OR CGI.HTTP_USER_AGENT CONTAINS "2." OR CGI.HTTP_USER_AGENT CONTAINS "3.")>
    
	    <P>&nbsp;</P>

	    <P>Make sure to take a look at the "Behind the Scenes"<BR>
	    and "View Source" links on each page.</P>
    
		<P><A HREF="showcategories.cfm">Click here</A> to enter the store.</P>

	</CFIF>

	</FONT>

</SPAN>
    
</BODY>
</HTML>
