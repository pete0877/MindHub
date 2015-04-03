<!--- This template adds a specified quantity of a certain item to the
      shopping cart.  The item is indicated by Form.ItemID and the quantity
	  to be added is indicated by Form.Quant --->

<!--- If the CartID cookie is not set,set a value --->
<CFPARAM NAME="Cookie.CartID" DEFAULT="#CreateUUID()#">
		  
<!--- Begin code that actually adds the item --->

	<!--- look in basket for item of that nature --->
	<CFQUERY datasource="#Application.datasource#" name="CheckCart">
		SELECT strItmIDPK,intQty FROM tblCtItm
		WHERE strCrtIDPK = '#Cookie.CartID#'
		AND strItmIDPK = '#Form.ItemID#'
	</cfquery>

	<!--- If the item is already in the basket... --->
	<CFIF CheckCart.RecordCount GT 0>
	
		<!--- Increment Quantity --->
		<CFQUERY datasource="#Application.datasource#" name="IncrementQuantity">
			UPDATE tblCtItm
			SET intQty = (intQty + 1)
			WHERE strCrtIDPK = '#Cookie.CartID#'
			AND strItmIDPK = '#Form.ItemID#'
		</cfquery>
	
	<!--- Otherwise, just add it to the basket --->
	<CFELSE>
	
		<!--- Increment Quantity --->
		<CFQUERY datasource="#Application.datasource#" name="AddCartItem">
			INSERT INTO tblCtItm (strCrtIDPK,strItmIDPK,intQty)
			VALUES ('#Cookie.CartID#', '#Form.ItemID#', 1)
		</cfquery>

	</CFIF>

<!--- End code that adds the item --->

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>Online Store - Shopping Cart</TITLE>
</HEAD>

<BODY BGCOLOR="#FFCC00" LINK="Maroon" background="images/storebg.gif" leftmargin=5 topmargin=0>

<SPAN STYLE="position: absolute; left: 205px; top: 15px; width: 350px;">
	<CFINCLUDE TEMPLATE="_showitems.cfm">
	<BR><BR>
	<CF_ShowDoc ID=3>
</SPAN>

<CFINCLUDE TEMPLATE="_showcart.cfm">

</BODY>
</HTML>
