<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<!--- Create Order Header --->
<CFTRANSACTION>

	<!--- Create Order Header --->
	<CFSET Expires = CreateODBCDate(CreateDate(Form.Exp2,Form.Exp1,1))>
	<CFSET NewID = CreateUUID()>
	<CFQUERY datasource="#Application.datasource#" Name="CreateOrderHeader">
		INSERT INTO tblOrder 
			(strCustIDF, 
			dtOrdDate,
			dblSubTtl,
			dblTax,
			dblCostTtl,
			strCardTp,
			dtExpires,
			strCardNum,
			strFname,
			strLname,
			strCpyName,
			strAdd1,
			strAdd2,
			strCity,
			strState,
			strZipCode,
			strPhone,
			strFax,
			strEmail,
			strOrdID
			)
		SELECT strCustID,
			#Now()#,
			#Val(Form.SubTotal)#,
			#Val(Form.Tax)#,
			#Val(Form.TotalCost)#,
			'#Form.CardType#',
			#Expires#,
			'#Form.CardNum#',
			strFname,
			strLname,
			strCpyName,
			strAdd1,
			strAdd2,
			strCity,
			strState,
			intZipCode,
			strPhone,
			strFax,
			strEmail,
			'#NewID#'
		FROM tblStCst
		WHERE strCustID = '#Form.CustomerID#'
	</cfquery>
	
	
</cftransaction>

<!--- set order number variable --->
<CFSET OrderID = NewID>

<!--- Create Order Details --->
<CFQUERY datasource=#Application.datasource# Name="CreateOrderHeader">
	INSERT INTO tblOrdDt (strOrdIDPK, strItmIDPK, intQty)
	SELECT '#OrderID#', strItmIDPK, intQty
	FROM tblCtItm,tblStItm
	WHERE strItmIDPK = strItmID
	AND strCrtIDPK='#Cookie.CartID#'
</cfquery>
	
<!--- Purge Cart --->
<CFINCLUDE TEMPLATE="killcart.cfm">

<HTML>
<HEAD>
	<TITLE>Online Store - Order Confirmation</TITLE>
</HEAD>

<BODY BGCOLOR="#FFCC00" LINK="Maroon" BACKGROUND="images/storebg.gif" LEFTMARGIN=5 TOPMARGIN=0>

<SPAN STYLE="position: absolute; left: 205px; top: 10px; width: 350px">
	<FONT FACE=" arial black,Helvetica bold,geneva,helvetica" SIZE="5" COLOR="##000000">Order Confirmed.</FONT>

<TABLE BORDER="0" CELLSPACING="0" CELLPADDING="0">
<TR>
	<TD>
	<BR>
	<FONT FACE="Arial, Helvetica" COLOR="##000000" SIZE="4"><B>Thanks for your order!<BR>
	Your order is #<CFOUTPUT>#OrderID#</cfoutput>.</FONT>
	
	<BR><BR><BR>
	
	<a href="showcategories.cfm"><font face="Verdana">Continue shopping?</font></a>

	</td>
</tr>
</table>

<!--- Since this is not a real online store, processing ends
	here. If this were a real store, there would credit
    card processing.
    
    Credit card transactions can be handled via CyberCash using
    the custom tag CFX_ONCR_CYBERCASH, available for purchase
    from Online Creations' website (http://www.oncr.com). --->

</SPAN>

	<!--- Delete Cookie --->
	<!--- <CFCOOKIE NAME="CartID" EXPIRES="NOW"> --->

	<!--- Show Cart --->
	<!--- <CFINCLUDE TEMPLATE="_showcart.cfm"> --->

</BODY>
</HTML>
