<!--- This template is used by other templates via CFINCLUDE; it
      is used to display the contents of the shopping cart in a
	  small table (a less detailed view than _showitems.cfm). --->

<!--- Don't let the browser cache this page. Otherwise we might
      be looking at a cached shopping cart which does not reflect
	  what is truly in the cart. --->
<CFHEADER Name="Expires" Value="#Now()#">
<CFHEADER NAME="pragma" VALUE="no-cache">

<CFQUERY DATASOURCE="#Application.datasource#" NAME="GetCategories">
	SELECT * FROM tblStCat
</CFQUERY>

<!--- Get List of Cart Items if a cart exists --->
<CFIF IsDefined("Cookie.CartID")>
	<CFQUERY datasource="#Application.datasource#" NAME="GetCartItems">
		SELECT strItmID, dblItmCost, intQty, strItmName FROM tblStItm, tblCtItm
		WHERE strItmID = strItmIDPK
		AND strCrtIDPK = '#Cookie.CartID#'
	</cfquery>
</CFIF>

<STYLE TYPE="text/css">
#floater {
	position: absolute;
	left: 15;
	top: 150;
	width: 125;
	visibility: visible;
	z-index: 10;
}
</STYLE>

<DIV ID="floater">

<TABLE BORDER="0" CELLPADDING="0" CELLSPACING="0">
<TR>
	<TD><IMG SRC="images/widget_top.gif" WIDTH=122 HEIGHT=26 BORDER=0></TD>
</TR>

<FORM ACTION="results.cfm" METHOD="POST">
<TR>
	<TD BACKGROUND="images/widget_bg.gif" BGCOLOR="#CC9933"><NOBR>&nbsp;&nbsp;<FONT SIZE="-1"><INPUT TYPE="TEXT" NAME="Keyword" SIZE="10"></FONT> <INPUT TYPE="IMAGE" SRC="images/widget_go.gif" ALIGN="MIDDLE" BORDER=0></NOBR></TD>
</TR>
</FORM>

<TR>
	<TD><IMG SRC="images/widget_divider.gif" WIDTH=122 HEIGHT=10 BORDER=0></TD>
</TR>

<!--- loop over categories --->
<TR>
	<TD BACKGROUND="images/widget_bg.gif"><NOBR>&nbsp;&nbsp;<FONT FACE="MS Sans Serif, Helvetica" SIZE="-1" COLOR="#FFFFCC"><A HREF="showcategories.cfm"><IMG SRC="images/widget_click.gif" WIDTH=13 HEIGHT=16 BORDER=0 ALIGN="TOP"></A> Store Front</FONT></NOBR>
	<CFOUTPUT QUERY="GetCategories"><BR><NOBR>&nbsp;&nbsp;<FONT FACE="MS Sans Serif, Helvetica" SIZE="-1" COLOR="##FFFFCC"><A HREF="showitems.cfm?CategoryID=#strCatID#"><IMG SRC="images/widget_click.gif" WIDTH=13 HEIGHT=16 BORDER=0 ALIGN="TOP"></A> #strCatName#</FONT></NOBR></CFOUTPUT></TD>
</TR>

<TR>
	<TD><IMG SRC="images/widget_divider.gif" WIDTH=122 HEIGHT=10 BORDER=0></TD>
</TR>
<TR>
	<TD><IMG SRC="images/widget_inthebag.gif" WIDTH=122 HEIGHT=21 BORDER=0></TD>
</TR>
<TR>
	<TD BACKGROUND="images/widget_bg2.gif" ALIGN="RIGHT">
	<FONT FACE="MS Sans Serif, Helvetica" SIZE="1" COLOR="#00FF00">
	<CFIF NOT IsDefined("Cookie.CartID")>
	 	<NOBR>(empty cart)&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</NOBR>
	<CFELSE>
		<!--- Initialize total cost variable --->
		<CFSET TotalCost = 0>
		
		<!--- Display quantity and item name --->
		<CFOUTPUT QUERY="GetCartItems">
			<NOBR>#GetCartItems.intQty# x #GetCartItems.strItmName#&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</NOBR><BR>
			<!--- Add total value of current item to total cost variable --->
			<CFSET TotalCost = TotalCost + GetCartItems.intQty * GetCartItems.dblItmCost>
		</CFOUTPUT>
		<CFIF TotalCost EQ 0>
		 	<NOBR>(empty cart)&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</NOBR>
		<CFELSE>
			<NOBR>Total: <CFOUTPUT>#DollarFormat(TotalCost)#</CFOUTPUT>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</NOBR>
		</cfif>
	</CFIF>
	</FONT>
	</TD>
</TR>
<TR>
	<TD><IMG SRC="images/widget_midbottom.gif" WIDTH=122 HEIGHT=7 BORDER=0></TD>
</TR>
<TR>
	<TD BACKGROUND="images/widget_bg.gif"><NOBR>&nbsp;&nbsp;<FONT FACE="MS Sans Serif, Helvetica" SIZE="-1" COLOR="#FFFFCC"><A HREF="viewcart.cfm"><IMG SRC="images/widget_click.gif" WIDTH=13 HEIGHT=16 BORDER=0 ALIGN="TOP"></A> Details</FONT></NOBR><BR>
	<NOBR>&nbsp;&nbsp;<FONT FACE="MS Sans Serif, Helvetica" SIZE="-1" COLOR="#FFFFCC"><A HREF="checkout.cfm"><IMG SRC="images/widget_click.gif" WIDTH=13 HEIGHT=16 BORDER=0 ALIGN="TOP"></A> Checkout</FONT></NOBR></TD>
</TR>
<TR>
	<TD><IMG SRC="images/widget_bottom.gif" WIDTH=122 HEIGHT=8 BORDER=0></TD>
</TR>
</TABLE>


</DIV>

<CF_STALKER STYLE="floater">
