<!--- Get Cart Item Count --->
<CFQUERY Datasource="#Application.datasource#" name="CartItems">
	SELECT strItmIDPK, intQty
	FROM tblCtItm
	WHERE strCrtIDPK = '#Cookie.CartID#'
</cfquery>

<!--- Return the cart to the state it was at when the
      template that called this template was generated --->
<!--- <CFSET Session.StoreItems = Form.StoreItems>
<CFSET Session.StoreQuantities = Form.StoreQuantities> --->

<CFLOOP query="CartItems">

	<!--- If the cached quantity changed... --->
	<CFIF CartItems.intQty NEQ Evaluate("Form.Quantity#Replace(strItmIDPK,"-","_","ALL")#")
	 AND IsNumeric(Evaluate("Form.Quantity#Replace(strItmIDPK,"-","_","ALL")#"))>
	
		<!--- Store new Quantity in a temporary Variable --->
		<CFSET NewQuantity = Evaluate("Form.Quantity#Replace(strItmIDPK,"-","_","ALL")#")>
		
		<!--- Update Quantity in Cart --->
		<CFQUERY datasource=#Application.datasource# name="ChangeQuantity">
			UPDATE tblCtItm
			SET intQty = #Val(Variables.NewQuantity)#
			WHERE strCrtIDPK = '#Cookie.CartID#'
			AND strItmIDPK = '#CartItems.strItmIDPK#'
		</cfquery>
	
	</cfif>
	<!--- <CFSET NewQuantity = Evaluate("Form.Item" & ListGetAt(Session.StoreItems, CurrVal))>
	<CFSET Session.StoreQuantities = ListSetAt(Session.StoreQuantities, CurrVal, Trim(NewQuantity))> --->
</CFLOOP>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>Online Store - Shopping Cart</TITLE>
</HEAD>

<BODY BGCOLOR="#FFCC00" LINK="Maroon" background="images/storebg.gif" leftmargin=5 topmargin=0>

<SPAN STYLE="position: absolute; left: 205px; top: 15px; width: 350px;">
	<CFINCLUDE TEMPLATE="_showitems.cfm">
	<BR><BR>
	<CF_ShowDoc ID=4>
</SPAN>

<CFINCLUDE TEMPLATE="_showcart.cfm">

</BODY>
</HTML>
