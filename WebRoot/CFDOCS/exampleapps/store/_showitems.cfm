<!--- This template is used by other templates via CFINCLUDE; for
      example, after additem.cfm is used to add an item to the shopping
	  cart, _showcart.cfm is CFINCLUDE-ed to show the result contents
	  of the shopping cart. --->

<!--- Don't let the browser cache this page. Otherwise we might
      be looking at a cached shopping cart which does not reflect
	  what is truly in the cart. --->
<CFHEADER Name="Expires" Value="#Now()#">
<CFHEADER NAME="pragma" VALUE="no-cache">
<CFIF IsDefined("Cookie.CartID")>
	<!--- If there are 0 of any item, remove that item from the cart --->
	<CFQUERY datasource="#Application.datasource#" name="PurgeEmptyItems">
		DELETE FROM tblCtItm
		WHERE strCrtIDPK = '#Cookie.CartID#'
		AND intQty = 0
	</cfquery>

	<!--- Get List of Cart Items --->
	<CFQUERY datasource="#Application.datasource#" NAME="GetCartItems">
		SELECT strItmIDPK, strPartNum, strItmName, dblItmCost, intQty 
		FROM tblStItm, tblCtItm
		WHERE strItmID = strItmIDPK
		AND strCrtIDPK = '#Cookie.CartID#'
	</cfquery>
</cfif>



<FONT FACE=" arial black,Helvetica bold,geneva,helvetica" SIZE="6" COLOR="##000000"><I>Bag It!</i></FONT><br>
<br>

<TABLE BORDER="0" CELLPADDING="5" CELLSPACING="1" bgcolor="000000">

<!--- These are just the column headings --->
<TR>
	<TD BGCOLOR="#660000"><FONT FACE="Helvetica" SIZE="-1" COLOR="#FFFF00"><B>Item #</B></TD>
	<TD BGCOLOR="#660000"><FONT FACE="Helvetica" SIZE="-1" COLOR="#FFFF00"><B>Description</B></TD>
	<TD BGCOLOR="#660000"><FONT FACE="Helvetica" SIZE="-1" COLOR="#FFFF00"><B>Unit Price</B></TD>
	<TD BGCOLOR="#660000"><FONT FACE="Helvetica" SIZE="-1" COLOR="#FFFF00"><B>Quantity</B></TD>
	<TD BGCOLOR="#660000"><FONT FACE="Helvetica" SIZE="-1" COLOR="#FFFF00"><B>Subtotal</B></TD>
</TR>


<!--- If the shopping cart is empty, just say that. --->
<CFIF IsDefined("Cookie.CartID")>
	<CFIF GetCartItems.RecordCount LT 1>
	
		<TR>
			<TD BGCOLOR="#FFcc66" COLSPAN="5" ALIGN="CENTER">
			<FONT FACE="Helvetica" SIZE="-1"><I>(There are currently no items in your
			shopping cart.)</I></FONT>
			</TD>
		</TR>
	
	<!--- On the other hand, if the cart is not empty... --->
	<CFELSE>
	
		<!--- This form is called if quantities of the items are changed --->
		<FORM ACTION="changequants.cfm" METHOD="POST">
		
		<!--- This variable will be used to accumulate the total cost --->
		<CFSET TotalCost = 0>
	
		<!--- Now display the data for the current item --->
		<CFOUTPUT QUERY="GetCartItems">
			<TR>
				<TD BGCOLOR="##FFff99"><FONT FACE="Helvetica" SIZE="-1">#strPartNum#</TD>
				<TD BGCOLOR="##FFff99"><FONT FACE="Helvetica" SIZE="-1">#strItmName#</TD>
				<td ALIGN="RIGHT" bgcolor="##FFFF99"><FONT FACE="Helvetica" SIZE="-1">#DollarFormat(dblItmCost)#</TD>
				<td bgcolor="##FFFF99"><FONT FACE="Helvetica">
					<input type="Text" name="Quantity#Replace(strItmIDPK,"-","_","ALL")#" value="#intQty#" align="RIGHT" size="3">
				</TD>
				<TD BGCOLOR="##FFff99" ALIGN="RIGHT"><FONT FACE="Helvetica" SIZE="-1">
					#DollarFormat(dblItmCost * intQty)#
				</TD>
			</TR>
			<!--- Add cost of current item(s) to total cost --->
			<CFSET TotalCost = TotalCost + (GetCartItems.dblItmCost * GetCartItems.intQty)>
		</CFOUTPUT>
	
		<TR>
			<TD COLSPAN="4" BGCOLOR="#FFff99" ALIGN="RIGHT">
				<FONT FACE="Helvetica" SIZE="-1"><B>Total</B></FONT>
			</TD>
			<TD BGCOLOR="#FFff99" ALIGN="RIGHT">
				<!--- Display the total cost --->
	            <CFOUTPUT>
					<FONT FACE="Helvetica" SIZE="-1">#DollarFormat(TotalCost)#</FONT>
				</CFOUTPUT>
			</TD>
		</TR>
	</TABLE>
	<TABLE BORDER=0 CELLPADDING=0 CELLSPACING=0 WIDTH=382>
		<TR>
			<TD><BR><INPUT TYPE="SUBMIT" VALUE="Change Quantities"></TD>
			<TD ALIGN="RIGHT"><BR><INPUT TYPE="BUTTON" VALUE="Checkout" onClick="location.href = 'checkout.cfm'"></TD>
		</TR>
		
		</FORM>
	
	</CFIF> <!--- is Cart empty --->
<CFELSE>
	<TR>
		<TD BGCOLOR="#FFcc66" COLSPAN="5" ALIGN="CENTER">
		<FONT FACE="Helvetica" SIZE="-1"><I>(There are currently no items in your
		shopping cart.)</I></FONT>
		</TD>
	</TR>
</CFIF> <!--- IS Cart defined --->

</TABLE>

<BR>

<FONT FACE="Helvetica" SIZE="-1">
<CFOUTPUT>
	<A HREF="showcategories.cfm">Return to Category Listing</A><BR>
	<CFIF IsDefined("Cookie.LastCategory")><A HREF="showitems.cfm?CategoryID=#Cookie.LastCategory#">Return to Last Category</A></CFIF>
</CFOUTPUT>
</FONT>