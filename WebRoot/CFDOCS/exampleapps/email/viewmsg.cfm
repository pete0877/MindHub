<CFINCLUDE TEMPLATE="_header.cfm">

<!--- Separator is based on OS --->
<CFLOCK SCOPE="Server" TIMEOUT="30" TYPE="ReadOnly">
	<CFIF Server.OS.Name CONTAINS "Windows">
		<CFSET Sep = "\">
	<CFELSE>
		<CFSET Sep = "/">	
	</CFIF>
</CFLOCK>

<!--- Create main temp directory if it doesn't already exist --->
<CFSET TempDir = GetTempDirectory() & "CrazyCab#Sep#">
<CFTRY>
	<CFDIRECTORY ACTION="CREATE" DIRECTORY="#TempDir#">
	<CFCATCH TYPE="ANY">
	</CFCATCH>
</CFTRY>

<!--- Create inbound temp directory if it doesn't already exist --->
<CFSET AttachDir = TempDir & "inbound#Sep#">
<CFTRY>
	<CFDIRECTORY ACTION="CREATE" DIRECTORY="#AttachDir#">
	<CFCATCH TYPE="ANY">
	</CFCATCH>
</CFTRY>

<CFPOP ACTION="GETALL"
       NAME="Message"
       MESSAGENUMBER="#URL.Msg#"
       SERVER="#Session.POPserver#"
       USERNAME="#Session.Username#"
       PASSWORD="#Session.Password#"
	   ATTACHMENTPATH="#AttachDir#">

<CFSET Body = Message.Body>
<CF_Wrap VARIABLE="Body" WIDTH=50>

<table border="0" cellspacing="0" cellpadding="0">
<CFOUTPUT>
<tr>
	<td bgcolor="cc9900" valign="top" rowspan=2 width="55"><FONT FACE="Helvetica" SIZE="-1" color="FFffcc"><B><a href="messagelist.cfm"><img src="images/back.gif" width=46 height=36 border=0 alt=""></a></td>
	<td bgcolor="cc9900" height=5 colspan=3><img src="images/curve3.gif" width=1 height=1 border=0 alt=""></td>
	<td valign="top" rowspan=2><img src="images/curve3.gif" width=36 height=36 border=0 alt=""></td>
</tr>
<TR>
	<TD valign=top bgcolor="cc9900"><FONT FACE="Helvetica" SIZE="-1" color="FFffcc"><B><a href="compose.cfm"><img src="images/compose.gif" width=29 height=25 border=0 alt="" align="left">Compose</a>&nbsp;&nbsp;</TD>
	<TD valign=top bgcolor="cc9900"><FONT FACE="Helvetica" SIZE="-1" color="FFffcc"><B><a href="reply.cfm?Msg=#URL.Msg#"><img src="images/reply.gif" width=36 height=26 border=0 alt="" align="left">Reply</a>&nbsp;&nbsp;</TD>
	<TD valign=top bgcolor="cc9900"><FONT FACE="Helvetica" SIZE="-1" color="FFffcc"><B><a href="forward.cfm?Msg=#URL.Msg#"><img src="images/forward.gif" width=35 height=25 border=0 alt="" align="left">Forward</a></TD>
</TR>
</CFOUTPUT>
</TABLE>
<table border="0" cellspacing="2" cellpadding="25" align="CENTER" bgcolor="#660000">
<TR>
	<TD width=400 nowrap bgcolor="FFffcc">

	<FONT COLOR="#000000">

	<CFIF IsDefined("URL.FullHeaders")>

		<CFOUTPUT>#HTMLCodeFormat(Message.Header)#</CFOUTPUT>

	<CFELSE>

		<CFOUTPUT QUERY="Message">

		<FONT FACE="Helvetica" SIZE="-1" COLOR="##000000">
		<P><B>Date:</B> #Date#<BR>
		<B>From:</B> #HTMLEditFormat(From)#<BR>
		<B>To:</B> #HTMLEditFormat(To)#<BR>
		<B>cc:</B> #HTMLEditFormat(cc)#<BR>
		<B>Subject:</B> #HTMLEditFormat(Subject)#<BR>
		<FONT SIZE="1"><A HREF="viewmsg.cfm?Msg=#URL.Msg#&FullHeaders=On">Click here for full headers</A></FONT></P>
		</FONT>

		</CFOUTPUT>

	</CFIF>

	<CFOUTPUT>
	<PRE>--------</PRE>
	#HTMLCodeFormat(Body)#
	</CFOUTPUT>
	<CFIF Message.Attachments NEQ "">
	<FONT FACE="Helvetica" SIZE="-1"><B>Attached:</B>
	<CFLOOP FROM="1" TO="#ListLen(Message.Attachments,'	')#" INDEX="CurrItem">
		<CFSET FileName = "#AttachDir##URLEncodedFormat(GetFileFromPath(ListGetAt(Message.AttachmentFiles, CurrItem, '	')))#">
		<CFSET FileName = Replace(FileName, "%2E", ".", "ALL")>
		<CFOUTPUT>
			<A HREF="getfile.cfm/#GetFileFromPath(Filename)#?Filename=#URLEncodedFormat(Filename)#">#ListGetAt(Message.Attachments, CurrItem, "	")#</A>
		</CFOUTPUT>
	</CFLOOP>
	</FONT>
	</CFIF>
	</FONT>

	</TD>
</TR>
</TABLE>

<CFINCLUDE TEMPLATE="_footer.cfm">