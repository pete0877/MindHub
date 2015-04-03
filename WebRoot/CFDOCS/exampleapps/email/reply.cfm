<CFINCLUDE TEMPLATE="_header.cfm">

<CFPOP ACTION="GETALL"
       NAME="Message"
       MESSAGENUMBER="#URL.Msg#"
       SERVER="#Session.POPserver#"
       USERNAME="#Session.Username#"
       PASSWORD="#Session.Password#">

<CFSET NewBody = Message.Body>
<CF_Wrap VARIABLE="NewBody" WIDTH="80">
<CFSET NewBody = "> " & Replace(NewBody, Chr(10), Chr(10) & "> ", "ALL")>

<CFIF FindNoCase("Re:", Left(Trim(Message.Subject), 3)) IS 0>
	<CFSET NewSubject = "Re: " & Trim(Message.Subject)>
<CFELSE>
	<CFSET NewSubject = Trim(Message.Subject)>
</CFIF>

<FORM ACTION="send.cfm" METHOD="POST" ENCTYPE="multipart/form-data">

<CFOUTPUT QUERY="Message">

<TABLE BORDER="0" CELLPADDING="3" CELLSPACING="0">
<TR>
	<TD ALIGN="RIGHT"><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><B>To</B></FONT></TD>
	<TD><INPUT TYPE="TEXT" NAME="To" VALUE="#HTMLEditFormat(Trim(Message.From))#" SIZE="30"></TD>
</TR>
<TR>
	<TD ALIGN="RIGHT"><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><B>cc</B></FONT></TD>
	<TD><INPUT TYPE="TEXT" NAME="cc" SIZE="30"></TD>
</TR>
<TR>
	<TD ALIGN="RIGHT"><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><B>Subject</B></FONT></TD>
	<TD><INPUT TYPE="TEXT" NAME="Subject" VALUE="#HTMLEditFormat(NewSubject)#" SIZE="30"></TD>
</TR>
</TABLE>

<P><TEXTAREA NAME="MessageBody" COLS="54" ROWS="20" WRAP="VIRTUAL">#NewBody#</TEXTAREA></P>

</CFOUTPUT>

<P><FONT FACE="MS Sans Serif, Helvetica" SIZE="-1"><B>Attachment</B></FONT><BR>
<INPUT TYPE="FILE" NAME="AttachFile"></P>

<INPUT TYPE="SUBMIT" VALUE="Send Message">

</FORM>


<CFINCLUDE TEMPLATE="_footer.cfm">