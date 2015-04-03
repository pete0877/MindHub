<CFTRY>
	<CFPOP ACTION="GETHEADERONLY"
	   	NAME="Messages"
	   	SERVER="#Session.POPserver#"
	   	TIMEOUT="120"
	   	USERNAME="#Session.Username#"
	   	PASSWORD="#Session.Password#">
	<CFCATCH TYPE="ANY">
		<CFINCLUDE TEMPLATE="_header.cfm">
	
		<P><FONT FACE="Helvetica" SIZE="+1"><B>Oops...!</B></FONT></P>
		
		<FONT FACE="Helvetica" SIZE="-1"><P>
	
		<CFIF CFCatch.Detail CONTAINS "password">
			The username and/or password you supplied was invalid!
		<CFELSEIF CFCatch.Detail CONTAINS "timeout">
			CrazyCab is tired of waiting for <CFOUTPUT>#Session.POPserver#</CFOUTPUT>
			to respond.
		<CFELSE>
			CrazyCab has encountered the following error:</P>
			
			<P><TABLE BORDER="1" CELLPADDING="5" CELLSPACING="0" BGCOLOR="#FFFFCC">
			<TR>
				<TD><TT><CFOUTPUT><B>#CFCatch.Message#</B><BR><BR>#CFCatch.Detail#</CFOUTPUT></TT></TD>
			</TR>
			</TABLE></P>
		</CFIF>

		<P>Please <A HREF="login.cfm">log in</A> again.
	
		</FONT></P>
	
		<CFINCLUDE TEMPLATE="_footer.cfm">
		<CFABORT>
	</CFCATCH>
</CFTRY>

<CFSET Session.Messages = Messages>

<CFLOCATION URL="messagelist.cfm" ADDTOKEN="NO">