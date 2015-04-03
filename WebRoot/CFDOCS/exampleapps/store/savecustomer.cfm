<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">

<!--- Password re-entry should be checked here, with other validation --->

<!--- insert new customer record --->
<CFTRANSACTION>
	<CFSET NewID = CreateUUID()>
	<CFQUERY datasource="#Application.datasource#" Name="SaveCustomer">
		INSERT INTO tblStCst
			(strCustID,
			strFname,
			strLname,
			strCpyName,
			strAdd1,
		<CFIF IsDefined("Form.Address2")>
			strAdd2,
		</cfif>
			strCity,
			strState,
			intZipCode,
			strPhone,
			strFax,
			strEmail,
			strUname,
			strPasswd)
	
		VALUES
	
			('#NewID#',
			'#Form.FirstName#',
			'#Form.LastName#',
			'#Form.CompanyName#',
			'#Form.Address1#',
	<CFIF IsDefined("Form.Address2")>
			'#Form.Address2#',
	</cfif>
			'#Form.City#',
			'#Form.Province#',
			'#Form.PostCode#',
			'#Form.Phone#',
			'#Form.Fax#',
			'#Form.Email#',
			'#Form.Username#',
			'#Form.Password#')
	</cfquery>
		
</cftransaction>

<html>
<head>
	<title>Online Store - Save Customer</title>
</head>

<body>

<!--- Store customer ID as form field --->
<CFLOCATION url="payment.cfm?CustomerID=#NewID#">

<!--- If customer record properly validated and saved then go to checkout --->
<!--- <CFLOCATION url="payment.cfm"> --->

</body>
</html>

