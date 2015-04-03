<!--- This CFQUERY retrieves the item that will be viewed --->
<CFQUERY DATASOURCE=#Application.datasource# NAME="GetItem" MAXROWS="1">
	SELECT strItmID,
	strItmName,
	strPartNum,
	dblItmCost,
	strTeaser,
	strItmImg,
	glbItmDesc,
	strCatIDFK,
	strName
	FROM tblStItm, tblMfg
	WHERE strMfgIDFK = strMfgID
	AND strItmID = '#ItemID#'
</CFQUERY>

<HTML>
<HEAD>
<TITLE>Online Store - Browse Items</TITLE>
</HEAD>

<BODY BGCOLOR="#FFCC00" LINK="Maroon" background="images/storebg.gif" leftmargin=5 topmargin=0>


<TABLE BORDER="0" CELLSPACING="0" CELLPADDING="0">
<TR>
	<TD WIDTH=130 HEIGHT=15 NOWRAP ALIGN="RIGHT">
	<CFINCLUDE TEMPLATE="_showcart.cfm">
	</TD>
	<TD WIDTH="70" NOWRAP>&nbsp;</TD>
	<TD VALIGN="top" WIDTH=350>
<BR>
<BR>


<TABLE BORDER="0" CELLPADDING="5" CELLSPACING="0">

	<!--- This CFOUTPUT block loops over the GetCategories query
	      and creates the following HTML for each one. --->
	<CFOUTPUT QUERY="GetItem">

	<H2><FONT FACE="Helvetica">#strItmName#</FONT></H2>

		<FORM ACTION="additem.cfm" METHOD="POST">

			<INPUT TYPE="HIDDEN" NAME="ItemID" VALUE="#strItmID#">

			<TR>
				<td colspan="2" bgcolor="Teal"><FONT FACE="Helvetica" SIZE="-1" COLOR="##FFFFFF"><img src="images/products/#strItmImg#" width=50 height=64 border=2 alt="" align="left"><B>#glbItmDesc#</b></FONT></TD>
			</TR>

			<TR>
				<TD BGCOLOR="##FFFFCC" ALIGN="RIGHT"><B>Product:</B></TD>
				<TD BGCOLOR="##FFFFCC">#strItmName#</TD>
			</TR>
				
			<TR>
				<TD BGCOLOR="##FFFFCC" ALIGN="RIGHT"><B>Price:</B></TD>
				<TD BGCOLOR="##FFFFCC">#DollarFormat(dblItmCost)#</TD>
			</TR>
				
			<TR>
				<TD BGCOLOR="##FFFFCC" ALIGN="RIGHT"><B>Manufacturer:</B></TD>
				<TD BGCOLOR="##FFFFCC">#strName#</TD>
			</TR>
				
			<TR>
				<TD BGCOLOR="##FFFFCC" ALIGN="RIGHT"><B>Part Number:</B></TD>
				<TD BGCOLOR="##FFFFCC">#strPartNum#</TD>
			</TR>

			<TR>
				<TD COLSPAN="2" BGCOLOR="##FFFFCC" ALIGN="RIGHT"><FONT FACE="Helvetica" SIZE="-1">
				<INPUT TYPE="SUBMIT" VALUE="Add to Cart"></FONT></TD>
			</TR>

			<TR>
				<TD COLSPAN="2" HEIGHT="6"></TD>
			</TR>

		</FORM>
	
	</CFOUTPUT>
</TABLE>

<FONT FACE="Helvetica" SIZE="-1">
	<CFOUTPUT QUERY="GetItem">
		<A HREF="showitems.cfm?CategoryID=#strCatIDFK#">Return to Current Category.</A><BR>
	</cfoutput>
		<A HREF="showcategories.cfm">Return to Category List.</A>
</FONT>

<BR><BR><CF_ShowDoc ID=8>
	</TD>
</TR>
</TABLE>

</BODY>
</HTML>
