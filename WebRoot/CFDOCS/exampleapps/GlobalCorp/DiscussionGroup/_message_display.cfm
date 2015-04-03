<!--- ColdFusion(r) Express Global Corp. Example Application --->


<!--- 	This cfm acts as a fake custom tag. we cfinclude this on a page where we have already declared the values to be displayed. By using this file, we can abstract how the messages are displayed throughout the application. --->
<CFOUTPUT>
<TABLE WIDTH="450">
<CFIF IsDefined("strThrd")>
	<TR>
	<TD><B>Thread:</B></TD>
	<TD><A HREF="view_thread.cfm?id=#strThdIDFK#">#strThrd#</A></TD>
	</TR>
</CFIF>
<TR>
	<TD><B>Subject:</B></TD>
	<TD>#strSubject#</TD>
</TR>
<TR>
	<TD><B>From:</B></TD>
	<TD><A HREF="mailto:#strEmail#?subject=#URLEncodedFormat(strSubject)#">#strFname# #strLname#</A.</FONT></TD>
</TR>
<TR>
	<TD><B>Posted:</B></TD>
	<TD>#DateFormat(dtPosted, "mmmm d, yyyy")#</TD>
</TR>
<TR>
	<TD COLSPAN="2">#ParagraphFormat(glbMessage)#</TD>
</TR>
<TR>
	<TD COLSPAN="2"><FONT SIZE="-1">[ <A HREF="##Top">Top</A> ]</FONT></TD>
</TR>
</TABLE>
<P>
<HR NOSHADE SIZE="1">
</CFOUTPUT>
