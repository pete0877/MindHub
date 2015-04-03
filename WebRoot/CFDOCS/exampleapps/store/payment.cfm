<!--- This is an ordinary HTML page with a form to enter
      contact/credit card information --->

<!--- Pull customer information out of database --->
<CFQUERY datasource="#Application.datasource#" name="GetCustomerInfo" maxrows="1">
	SELECT *
	FROM tblStCst
	WHERE strCustID = '#URL.CustomerID#'
</cfquery>

<CFIF IsDefined("Cookie.CartID")>	  
<!--- Get List of Cart Items --->
<CFQUERY datasource="#Application.datasource#" NAME="GetCartItems">
	SELECT strItmIDPK, strPartNum, strItmName, dblItmCost, intQty 
	FROM tblStItm, tblCtItm
	WHERE strItmID = strItmIDPK
	AND strCrtIDPK = '#Cookie.CartID#'
</cfquery>
</cfif>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>Online Store - Payment</TITLE>
</HEAD>

<BODY BGCOLOR="#FFCC00" LINK="Maroon" background="images/storebg.gif" leftmargin=5 topmargin=0>

<SPAN STYLE="position: absolute; left: 205px; top: 10px; width: 350px">
	<FONT FACE=" arial black,Helvetica bold,geneva,helvetica" SIZE="7" COLOR="##000000"><I>Check Out</I></FONT>

	<CFOUTPUT query="GetCustomerInfo">
		<table>
		<tr>
			<td colspan="2"><B>Shipping Address:</b></td>
		</tr>
		<tr>
			<td colspan="2">
				#GetCustomerInfo.strFname# #GetCustomerInfo.strLName#<BR>
				#GetCustomerInfo.strCpyName#<BR>
				#GetCustomerInfo.strAdd1#<BR>
				#GetCustomerInfo.strAdd2#<BR>
				#GetCustomerInfo.strCity#, #GetCustomerInfo.strState#, #GetCustomerInfo.intZipCode#<BR>
			</td>
		</tr>
		</table>
	</cfoutput>
	
	<BR>

	<!--- Show Order Details --->
<TABLE BORDER="0" CELLPADDING="5" CELLSPACING="1" bgcolor="000000">

<!--- These are just the column headings --->
<TR>
	<TD BGCOLOR="#660000"><FONT FACE="Helvetica" SIZE="-1" COLOR="#FFFF00"><B>Item #</B></TD>
	<TD BGCOLOR="#660000"><FONT FACE="Helvetica" SIZE="-1" COLOR="#FFFF00"><B>Description</B></TD>
	<TD BGCOLOR="#660000"><FONT FACE="Helvetica" SIZE="-1" COLOR="#FFFF00"><B>Unit Price</B></TD>
	<TD BGCOLOR="#660000"><FONT FACE="Helvetica" SIZE="-1" COLOR="#FFFF00"><B>Quantity</B></TD>
	<TD BGCOLOR="#660000"><FONT FACE="Helvetica" SIZE="-1" COLOR="#FFFF00"><B>Subtotal</B></TD>
</TR>

<CFSET SubTotal=0>
<CFSET TotalCost=0>
<CFSET Tax=0>

<!--- If the shopping cart is empty, just say that. --->
	<CFIF NOT IsDefined("Cookie.CartID")>
		<CFSET ITEMS=0>
	 <CFELSE>
	 	<CFSET ITEMS=#GetCartItems.RecordCount#>
	</cfif>
	<CFIF ITEMS LT 1>
		<TR>
			<TD BGCOLOR="#FFcc66" COLSPAN="5" ALIGN="CENTER">
			<FONT FACE="Helvetica" SIZE="-1"><I>(There are currently no items in your
			shopping cart.)</I></FONT>
			</TD>
		</TR>
	
	<!--- On the other hand, if the cart is not empty... --->
	<CFELSE>
	
		<!--- This variable will be used to accumulate the total cost --->
		<CFSET SubTotal = 0>
	
		<!--- Now display the data for the current item --->
		<CFOUTPUT QUERY="GetCartItems">
			<TR>
				<TD BGCOLOR="##FFff99"><FONT FACE="Helvetica" SIZE="-1">#strPartNum#</TD>
				<TD BGCOLOR="##FFff99"><FONT FACE="Helvetica" SIZE="-1">#strItmName#</TD>
				<td ALIGN="RIGHT" bgcolor="##FFFF99"><FONT FACE="Helvetica" SIZE="-1">#DollarFormat(dblItmCost)#</TD>
				<TD BGCOLOR="##FFff99" ALIGN="RIGHT"><FONT FACE="Helvetica" SIZE="-1">#intQty#</TD>
					<!--- Save the Original Quantity in form variable for Quantity change detection --->
					<!--- <INPUT TYPE="HIDDEN" NAME="OrgQuantity#ItemID#" VALUE="#Quantity#"> --->

				<TD BGCOLOR="##FFff99" ALIGN="RIGHT"><FONT FACE="Helvetica" SIZE="-1">
					#DollarFormat(dblItmCost * intQty)#
				</TD>
			</TR>
			<!--- Add cost of current item(s) to total cost --->
			<CFSET SubTotal = SubTotal + (GetCartItems.dblItmCost * GetCartItems.intQty)>
		</CFOUTPUT>
	
		<TR>
			<TD COLSPAN="4" BGCOLOR="#FFff99" ALIGN="RIGHT">
				<FONT FACE="Helvetica" SIZE="-1"><B>SubTotal</B></FONT>
			</TD>
			<TD BGCOLOR="#FFff99" ALIGN="RIGHT">
				<!--- Display the total cost --->
	            <CFOUTPUT>
					<FONT FACE="Helvetica" SIZE="-1">#DollarFormat(SubTotal)#</FONT>
				</CFOUTPUT>
			</TD>
		</TR>
		<TR>
			<TD COLSPAN="4" BGCOLOR="#FFff99" ALIGN="RIGHT">
				<FONT FACE="Helvetica" SIZE="-1"><B>Tax</B></FONT>
			</TD>
			<TD BGCOLOR="#FFff99" ALIGN="RIGHT">
				<!--- Display the total cost --->
				<CFSET Tax = (SubTotal * Application.TaxRate)>
	            <CFOUTPUT>
					<FONT FACE="Helvetica" SIZE="-1">#DollarFormat(Tax)#</FONT>
				</CFOUTPUT>
			</TD>
		</TR>
		<TR>
			<TD COLSPAN="4" BGCOLOR="#FFff99" ALIGN="RIGHT">
				<FONT FACE="Helvetica" SIZE="-1"><B>Total</B></FONT>
			</TD>
			<TD BGCOLOR="#FFff99" ALIGN="RIGHT">
				<!--- Display the total cost --->
				<CFSET TotalCost = SubTotal + Tax>
	            <CFOUTPUT>
					<FONT FACE="Helvetica" SIZE="-1">#DollarFormat(TotalCost)#</FONT>
				</CFOUTPUT>
			</TD>
		</TR>
		
</CFIF><!--- empty cart --->
	</TABLE>
<CFIF ITEMS LT 1>	
<p>
<B>Your bag is empty. Please continue shopping and place items in your bag before checking out.</b>
<p>
<B><a href="showcategories.cfm">Click here</a> to continue shopping.</b>
<CFELSE>
<!--- This is a new customer --->	
	<CFFORM ACTION="orderconfirm.cfm" METHOD="POST">

	<TABLE BORDER="0" CELLPADDING="3">
	<TR>
		<TD><B>Payment Details:</b></td>
	</tr>
	<TR>
		<TD COLSPAN="2"><SELECT NAME="CardType" SIZE="1">
				<OPTION VALUE="0">
				<OPTION VALUE="A">American Express
				<OPTION VALUE="D">Discover/Novus
				<OPTION VALUE="M">MasterCard
				<OPTION VALUE="V">Visa
			</SELECT><BR>
		<FONT FACE="MS Sans Serif" SIZE="-2">Card Type</FONT></TD>
		<TD><SELECT NAME="Exp1" SIZE="1">
				<OPTION VALUE="01">1
				<OPTION VALUE="02">2
				<OPTION VALUE="03">3
				<OPTION VALUE="04">4
				<OPTION VALUE="05">5
				<OPTION VALUE="06">6
				<OPTION VALUE="07">7
				<OPTION VALUE="08">8
				<OPTION VALUE="09">9
				<OPTION VALUE="10">10
				<OPTION VALUE="11">11
				<OPTION VALUE="12">12
			</SELECT>
			<SELECT NAME="Exp2" SIZE="1">
				<OPTION VALUE="99">1999
				<OPTION VALUE="00">2000
				<OPTION VALUE="01">2001
				<OPTION VALUE="02">2002
				<OPTION VALUE="03">2003
				<OPTION VALUE="04">2004
				<OPTION VALUE="05">2005
				<OPTION VALUE="06">2006
				<OPTION VALUE="07">2007
				<OPTION VALUE="08">2008
			</SELECT><BR>
		<FONT FACE="MS Sans Serif" SIZE="-2">Expiration</FONT></TD>
	</TR>
	<TR>
		<TD COLSPAN="3"><CFINPUT NAME="CardNum" MESSAGE="Please enter a valid credit card number!" SIZE="25" VALIDATE="creditcard" REQUIRED="Yes"><BR>
		<FONT FACE="MS Sans Serif" SIZE="-2">Card Number</FONT></TD>
	</TR>
	</TABLE>
	
	<BR>

	<CFOUTPUT>
	<!--- Save Customer Number on Form as hidden field --->
	<INPUT TYPE="HIDDEN" NAME="CustomerID" VALUE="#URL.CustomerID#">
	<!--- Save SubTotal on Form as hidden field --->
	<INPUT TYPE="HIDDEN" NAME="SubTotal" VALUE="#SubTotal#">
		<!--- Save Tax on Form as hidden field --->
	<INPUT TYPE="HIDDEN" NAME="Tax" VALUE="#Tax#">
		<!--- Save TotalCost on Form as hidden field --->
	<INPUT TYPE="HIDDEN" NAME="TotalCost" VALUE="#TotalCost#">	
	</cfoutput>
	
	<INPUT TYPE="SUBMIT" VALUE="Confirm Payment">
	</CFFORM>
	</CFIF> <!--- is Cart empty --->
	<CF_ShowDoc ID=8>

</SPAN>

<!--- <CFINCLUDE TEMPLATE="_showcart.cfm"> --->

</BODY>
</HTML>
