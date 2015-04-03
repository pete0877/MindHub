<!--- This is an ordinary HTML page with a form to enter
      contact/credit card information --->

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>Online Store - Checkout</TITLE>
</HEAD>

<BODY BGCOLOR="#FFCC00" LINK="Maroon" background="images/storebg.gif" leftmargin=5 topmargin=0>

<SPAN STYLE="position: absolute; left: 205px; top: 10px; width: 350px">
	<FONT FACE=" arial black,Helvetica bold,geneva,helvetica" SIZE="7" COLOR="##000000"><I>Info Entry</I></FONT>
	
	<CFFORM ACTION="savecustomer.cfm" METHOD="POST">
	
	<TABLE BORDER="0" CELLPADDING="3">
	<TR>
		<TD><CFINPUT TYPE="Text" NAME="FirstName" MESSAGE="Please enter your name!" REQUIRED="Yes" SIZE="20"><BR>
		<FONT FACE="MS Sans Serif" SIZE="-2">First Name</FONT></TD>
		<TD COLSPAN="2"><CFINPUT TYPE="Text" NAME="LastName" MESSAGE="Please enter your name!" REQUIRED="Yes" SIZE="20"><BR>
		<FONT FACE="MS Sans Serif" SIZE="-2">Last Name</FONT></TD>
	</TR>
	<TR>
		<TD COLSPAN="3"><CFINPUT TYPE="Text" NAME="CompanyName" MESSAGE="Please enter your company name!" REQUIRED="No" SIZE="45"><BR>
		<FONT FACE="MS Sans Serif" SIZE="-2">Company Name</FONT></TD>
	</TR>
	<TR>
		<TD COLSPAN="3"><CFINPUT TYPE="Text" NAME="Address1" MESSAGE="Please enter your address!" REQUIRED="Yes" SIZE="45"><BR>
		<FONT FACE="MS Sans Serif" SIZE="-2">Address Line 1</FONT></TD>
	</TR>
	<TR>
		<TD COLSPAN="3"><CFINPUT TYPE="Text" NAME="Address2" MESSAGE="Please enter your address!" REQUIRED="No" SIZE="45"><BR>
		<FONT FACE="MS Sans Serif" SIZE="-2">Address Line 2</FONT></TD>
	</TR>
	<TR>
		<TD><CFINPUT TYPE="Text" NAME="City" MESSAGE="Please enter your city!" REQUIRED="Yes" SIZE="15"><BR>
		<FONT FACE="MS Sans Serif" SIZE="-2">City</FONT></TD>
		<TD><CFINPUT TYPE="Text" NAME="Province" MESSAGE="Please enter your province or state!" REQUIRED="Yes" SIZE="2"><BR>
		<FONT FACE="MS Sans Serif" SIZE="-2">State</FONT></TD>
		<TD><CFINPUT TYPE="Text" NAME="PostCode" MESSAGE="Please enter your postal code!" VALIDATE="zipcode" REQUIRED="Yes" SIZE="10"><BR>
		<FONT FACE="MS Sans Serif" SIZE="-2">Postal Code</FONT></TD>
	</TR>
	</TABLE>
	
	<BR>
	
	<TABLE BORDER="0" CELLPADDING="3">
	</TR>
		<TD><CFINPUT TYPE="Text" NAME="Phone" MESSAGE="Please enter a valid phone number!" VALIDATE="telephone" REQUIRED="Yes" SIZE="15"><BR>
		<FONT FACE="MS Sans Serif" SIZE="-2">Phone</FONT></TD>
	</TR>
	</TR>
		<TD><CFINPUT TYPE="Text" NAME="Fax" MESSAGE="Please enter a valid fax number (or leave it blank)!" VALIDATE="telephone" REQUIRED="No" SIZE="15"><BR>
		<FONT FACE="MS Sans Serif" SIZE="-2">Fax</FONT></TD>
	</TR>
	<TR>
		<TD><CFINPUT TYPE="Text" NAME="Email" MESSAGE="Please enter your e-mail address!" REQUIRED="Yes" SIZE="40"><BR>
		<FONT FACE="MS Sans Serif" SIZE="-2">E-mail</FONT></TD>
	<TR>
	</TABLE>
	
	<BR>
	
	<TABLE BORDER="0" CELLPADDING="3">
	</TR>
		<TD><cfinput type="Text" name="Username" message="Please enter your username!" required="Yes" size="15"><BR>
		<FONT FACE="MS Sans Serif" SIZE="-2">Username</FONT></TD>
	</TR>
	</TR>
		<TD><cfinput type="Password" name="Password" size="15"><BR>
		<FONT FACE="MS Sans Serif" SIZE="-2">Password</FONT></TD>
	</TR>
	</TR>
		<TD><cfinput type="Password" name="PswdConfirm" size="15"><BR>
		<FONT FACE="MS Sans Serif" SIZE="-2">ConfirmPassword</FONT></TD>
	</TR>
	</TABLE>
	
	<BR>

	<!--- Identify this form as new customer to next form --->
	<INPUT TYPE="HIDDEN" NAME="NewCustomer" VALUE="xxxx">

	<INPUT TYPE="SUBMIT" VALUE="Continue"> <INPUT TYPE="Reset">
	</CFFORM>

	<CF_ShowDoc ID=8>

</SPAN>

<CFINCLUDE TEMPLATE="_showcart.cfm">

</BODY>
</HTML>
