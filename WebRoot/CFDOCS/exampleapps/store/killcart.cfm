<!--- If there are 0 of any item, remove that item from the cart --->
<CFIF IsDefined("Cookie.CartID")>

	<!--- Delete cart data --->
	<CFQUERY datasource="#Application.datasource#" name="PurgeEmptyItems">
		DELETE FROM tblCtItm
		WHERE strCrtIDPK = '#Cookie.CartID#'
	</cfquery>

	<!--- Delete Cookie --->
	<CFCOOKIE NAME="CartID" EXPIRES="NOW">

</CFIF>