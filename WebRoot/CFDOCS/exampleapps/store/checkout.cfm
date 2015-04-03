<!--- This is an ordinary HTML page with a form to enter
      contact/credit card information --->

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>Online Store - Checkout</TITLE>
</HEAD>

<BODY BGCOLOR="#FFCC00" LINK="Maroon" background="images/storebg.gif" leftmargin=5 topmargin=0>

<SPAN STYLE="position: absolute; left: 205px; top: 10px; width: 350px">
	<FONT FACE=" arial black,Helvetica bold,geneva,helvetica" SIZE="7" COLOR="##000000"><I>Check Out</I></FONT><BR>
	
	<FONT FACE=" arial black,Helvetica bold,geneva,helvetica" SIZE="3" COLOR="##000000">Are you...</FONT>
	<TABLE BORDER="2" CELLPADDING="3">
		<tr align="CENTER" valign="MIDDLE">
			<td align="CENTER">
				<!--- New Customers --->
				<CFFORM ACTION="customerentry.cfm" METHOD="POST">
					<input type="Submit" value="New Customer" align="RIGHT">
				</CFFORM>
			</td>

	<TD>
	<!--- Existing Customers --->
	<CFFORM ACTION="findcustomer.cfm" METHOD="POST">
		<TABLE BORDER="0" CELLPADDING="3">
			<CFIF IsDefined("URL.badlogin")>
				<TR><TD COLSPAN=2><FONT FACE="MS Sans Serif" SIZE="-2">Username and password not found.</FONT></TD></TR>
			</CFIF>
			<TR>
				<TD><FONT FACE="MS Sans Serif" SIZE="-2">Username</FONT></TD>
				<TD><cfinput type="Text" name="Username" message="Please enter your username!" required="Yes" size="15"></TD>
			</TR>
			<TR>
				<TD><FONT FACE="MS Sans Serif" SIZE="-2">Password</FONT></TD>
				<TD><cfinput type="Password" name="Password" size="15"></TD>
			</TR>
			<tr>
				<td colspan="2" align="CENTER">
					<INPUT TYPE="SUBMIT" VALUE="Existing Customer">
				</td>
			</TR>
		</TABLE>
	</CFFORM>
	</td>
	</tr>
	</TABLE>

	<BR>
	
	<CF_ShowDoc ID=8>

</SPAN>

<CFINCLUDE TEMPLATE="_showcart.cfm">

</BODY>
</HTML>
