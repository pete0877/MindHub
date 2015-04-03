<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<HTML>
<HEAD>
<TITLE>Tack2 - Welcome Screen</TITLE>
</HEAD>

<BODY BGCOLOR="FFcc33" BACKGROUND="images/storebg.gif" LEFTMARGIN=5 TOPMARGIN=0>

<SPAN STYLE="position: absolute; left: 250px; top: 45px">
	<A HREF="showcategories.cfm"><IMG SRC="images/airboard.gif" WIDTH=273 HEIGHT=275 BORDER=0 ALT="" HSPACE=0 VSPACE=0></A>
</SPAN>

<SPAN STYLE="position: absolute; left: 100px; top: 30px">
	<A HREF="showcategories.cfm"><IMG SRC="images/tack2.gif" WIDTH=249 HEIGHT=58 BORDER=0 ALT="TACK2"></A>
</SPAN>

<SPAN STYLE="position: absolute; left: 255px; top: 255px">
	<A HREF="showcategories.cfm"><IMG SRC="images/allthefunk.gif" WIDTH=291 HEIGHT=38 BORDER=0 ALT="All the Funk You NEED" VSPACE=0 HSPACE=0></A>
</SPAN>

<SPAN STYLE="position: absolute; left: 200px; width: 350px; top: 380px">

	<FONT FACE="Arial Black, Helvetica Bold, Geneva, Helvetica" SIZE="2" COLOR="##000000">
    
    <P><B>Welcome to the TACK2 online store...</B></P>
    </FONT>
 	<FONT FACE="Arial Black, Helvetica Bold, Geneva, Helvetica" SIZE="1" COLOR="##000000">
	<P>You'll want to use Microsoft IE 4.0
    or Netscape 4.0 to view the store, as it uses style
    sheets and some fun DHTML.</P>
	</FONT>
	<!--- If browser is Netscape or MSIE and version 4.x or greater... --->
	<CFIF CGI.HTTP_USER_AGENT CONTAINS "Mozilla" AND NOT (CGI.HTTP_USER_AGENT CONTAINS "1." OR CGI.HTTP_USER_AGENT CONTAINS "2." OR CGI.HTTP_USER_AGENT CONTAINS "3.")>  
	 	<FONT FACE="Arial Black, Helvetica Bold, Geneva, Helvetica" SIZE="2" COLOR="##000000">
			<P><A HREF="showcategories.cfm">Click here</A> or the graphic above to enter the store.</P>
	</CFIF>

	</FONT>
		
	</SPAN>

</body>
</html>
