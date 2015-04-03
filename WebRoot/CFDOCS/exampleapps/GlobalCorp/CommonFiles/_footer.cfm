<!--- ColdFusion(r) Express Global Corp. Example Application --->

<!--- End the table that has started in _header.cfm --->

<CFOUTPUT>

</TD>

</TR>

</TABLE>

</TD>

</TR>

<TR>

<!--- THis is a link to log out.  it passes a url parameter called "logout" by  appending it to the url with a "?".  notice it uses a dynamic link: #getfilefrompath(getbasetemplatepath())# this makes it a relative link to whatever page you are on, this allows a user to log out, but maintain the page they are on for later.  this is similar to  what happens in the login form (logon_form.cfm). --->

<TD COLSPAN="9" BGCOLOR="#application.lightorange#" ALIGN="RIGHT" HEIGHT="16">

<!--- #getfilefrompath(getbasetemplatepath())# --->

<A HREF="#getfilefrompath(getbasetemplatepath())#?logout=yes"><IMG SRC="../#application.image_path#footer_button_logout.jpg" WIDTH="583" HEIGHT="16" BORDER="0"></A>

</TD></TR></TABLE>

<P>

<FONT SIZE="1">

Global Corp. is a fictitious company created to<BR>

demonstrate ColdFusion Express functionality.

</FONT>



</CENTER>







</BODY>

</HTML>

</CFOUTPUT>



