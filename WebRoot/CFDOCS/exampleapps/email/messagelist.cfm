<CFINCLUDE TEMPLATE="_header.cfm">

<!--- Make sure messages are loaded --->
<CFIF NOT IsDefined("Session.Messages")>
	<CFLOCATION URL="refresh.cfm" ADDTOKEN="NO">
</CFIF>
<table border="0" cellspacing="2" cellpadding="25" align="CENTER" bgcolor="#660000">
<tr>
	<td colspan="3" bgcolor="FFffcc">
	<!--- Display login information --->
	<P><FONT FACE="Helvetica" SIZE="-1">
		<CFOUTPUT>
			<b>Hello #Session.Username#,</B><br>
			&nbsp;&nbsp;&nbsp;&nbsp;Welcome to your CrazyCab inbox. You have <B>#Session.Messages.RecordCount#</B> new message(s) on
			<B>#Session.POPserver#</B> waiting to be read.
		</CFOUTPUT>
	</FONT></P>
	</td>
</tr>
</table><br>
&nbsp;


<!-- Navigational buttons. The fact that they are form submit
	buttons is cosmetic; the form will not actually be submitted,
	since any form submission will be intercepted by JavaScript. -->
<table border="0" cellspacing="0" cellpadding="0">
<tr>
	<td colspan="5">

<!-- Column headers -->
		<table border="0" cellspacing="2" cellpadding="3" align="CENTER" bgcolor="#660000">

		<TR>
			<TD bgcolor="660000"><FONT FACE="Helvetica" SIZE="-1" color="FFffcc"><B>From</B></FONT></TD>
			<TD bgcolor="660000"><FONT FACE="Helvetica" SIZE="-1" color="FFffcc"><B>Subject</B></FONT></TD>
			<TD bgcolor="660000"><FONT FACE="Helvetica" SIZE="-1" color="FFffcc"><B>Date</B></FONT></TD>
		</TR>
		<!-- Output messages -->
		<CFOUTPUT QUERY="Session.Messages">
		<TR>
			<TD bgcolor="FFffcc"><FONT FACE="Helvetica" SIZE="-1">
				<A HREF="viewmsg.cfm?Msg=#MessageNumber#">#HTMLEditFormat(From)#</A>
			</FONT></TD>
			<TD bgcolor="FFffcc"><FONT FACE="Helvetica" SIZE="-1">
				<A HREF="viewmsg.cfm?Msg=#MessageNumber#"><CFIF Trim(Subject) IS "">(no subject)<CFELSE>#Subject#</CFIF></A>
			</FONT></TD>
			<TD bgcolor="FFffcc"><FONT FACE="Helvetica" SIZE="-1">
				<A HREF="viewmsg.cfm?Msg=#MessageNumber#">#Date#</A>
			</FONT></TD>
		</TR>
		</CFOUTPUT>
		</TABLE>
	</td>
</tr>
<tr>
	<td width=150>&nbsp;</td>
	<td align="RIGHT"><img src="images/curve.gif" width=36 height=36 border=0 alt=""></td>
	<td bgcolor=cc9900 valign="middle">
		<a href="compose.cfm"><img src="images/compose.gif" width=29 height=25 border=0 alt="" align="left"></a>
		<font face="helvetica" size="-1" color="ffffcc">&nbsp;&nbsp;<b><a href="compose.cfm">Compose</a></b>&nbsp;&nbsp;&nbsp;</font>
	</td>
	<td bgcolor=cc9900 align="RIGHT"><img src="images/curve2.gif" width=36 height=36 border=0 alt=""></td>
	<td bgcolor=660000>
		<a href="refresh.cfm"><img src="images/refresh.gif" width=25 height=24 border=0 alt="" align="left"></a>
		<font face="helvetica" size="-1" color="ffffcc">&nbsp;&nbsp;<b><a href="refresh.cfm">Refresh</a></b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</font>
	</td>
</tr>
</table>


<CFINCLUDE TEMPLATE="_footer.cfm">