<CFINCLUDE TEMPLATE="_header.cfm">

<FORM ACTION="send.cfm" METHOD="POST" ENCTYPE="multipart/form-data">

<CFOUTPUT>

<TABLE BORDER="0" CELLPADDING="3" CELLSPACING="0">
<TR>
	<TD ALIGN="RIGHT"><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><B>To</B></FONT></TD>
	<TD><INPUT TYPE="TEXT" NAME="To" SIZE="30"></TD>
</TR>
<TR>
	<TD ALIGN="RIGHT"><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><B>cc</B></FONT></TD>
	<TD><INPUT TYPE="TEXT" NAME="cc" SIZE="30"></TD>
</TR>
<TR>
	<TD ALIGN="RIGHT"><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><B>Subject</B></FONT></TD>
	<TD><INPUT TYPE="TEXT" NAME="Subject" SIZE="30"></TD>
</TR>
</TABLE>

<P><TEXTAREA NAME="MessageBody" COLS="50" ROWS="20" WRAP="VIRTUAL"></TEXTAREA></P>

</CFOUTPUT>

<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><B>Attachment</B></FONT><BR>
<INPUT TYPE="FILE" NAME="AttachFile"></P>

<P><INPUT TYPE="SUBMIT" VALUE="Send Message"></P>

</FORM>


<CFINCLUDE TEMPLATE="_footer.cfm">