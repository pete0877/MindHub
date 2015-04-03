<CFINCLUDE TEMPLATE="_header.cfm">

<CFLOOP LIST="Email,Username,POPserver,SMTPserver" INDEX="CurrItem">
	<CFIF IsDefined("Cookie.#CurrItem#")>
		<CFSET "#CurrItem#" = evaluate("Cookie." & CurrItem)>
	<CFELSE>
		<CFSET "#CurrItem#" = "">
	</CFIF>
</CFLOOP>

<CFIF IsDefined("Cookie.SaveInfo")>
	<CFSET Checked = "CHECKED">
<CFELSE>
	<CFSET Checked = "">
</CFIF>

<table border="0" cellspacing="0" cellpadding="0" align="CENTER">
<tr>
	<td colspan="3"></td>
</tr>
<!--- <tr><td height="15">&nbsp;</td></tr> --->
<tr>
	<td valign="top" width="165" nowrap>

	<p align="right">
	<font face="arial">
	To log into the Crazy Cab Mail client, please enter your POP mail account information in the appropriate fields on your right.

	</p>
	</td>
	<td width="25" nowrap>&nbsp;</td>
	<td valign="TOP" width="200" nowrap>

	<FORM ACTION="auth.cfm" METHOD="POST">
	
	<CFOUTPUT>
	
	<P><INPUT TYPE="TEXT" NAME="Email" VALUE="#Email#"><BR>
	<FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">E-mail Address</FONT></P>
	
	<P><INPUT TYPE="TEXT" NAME="Username" VALUE="#Username#"><BR>
	<FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">POP Account Username</FONT></P>
	
	<P><INPUT TYPE="PASSWORD" NAME="Password"><BR>
	<FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">POP Password</FONT></P>
	
	<P><INPUT TYPE="TEXT" NAME="POPserver" VALUE="#POPserver#"><BR>
	<FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">POP Server</FONT></P>
	
	<P><INPUT TYPE="TEXT" NAME="SMTPserver" VALUE="#SMTPserver#"><BR>
	<FONT FACE="MS Sans Serif, Helvetica" SIZE="-2">SMTP Server</FONT></P>
	
	<P><INPUT TYPE="CHECKBOX" NAME="SaveInfo" #Checked#><FONT
	FACE="MS Sans Serif, Helvetica" SIZE="-2">Save login information</FONT></P>
	
	</CFOUTPUT>
	
	<P><INPUT TYPE="SUBMIT" VALUE="Log In"></P>
	
	</FORM>
	</td>
</tr>
</table>

<CFINCLUDE TEMPLATE="_footer.cfm">