<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<!--- find customer record --->
<CFQUERY datasource=#Application.datasource# Name="GetCustomerID" MAXROWS="1">
	SELECT strCustID FROM tblStCst
	WHERE strUname = '#Form.Username#'
	AND strPasswd = '#Form.Password#'
</cfquery>

<html>
<head>
	<title>Online Store - Find Customer</title>
</head>

<body>

<!--- Store customer ID as form field --->
<CFOUTPUT Query="GetCustomerID">
	<CFLOCATION url="payment.cfm?CustomerID=#GetCustomerID.strCustID#">
</cfoutput>

<!--- If customer record properly validated and saved then go to checkout --->
<CFLOCATION URL="checkout.cfm?badlogin=1">

</body>
</html>
