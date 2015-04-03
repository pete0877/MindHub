<!--- If no category is currently selected, go to the category
      selection page --->
<CFIF NOT IsDefined("CategoryID")>
	<CFLOCATION URL="showcategories.cfm">
</CFIF>

<!--- This CFQUERY retrieves the name of the current category --->
<CFQUERY DATASOURCE="#Application.datasource#" NAME="GetCurrentCategory">
	SELECT strCatname FROM tblStCat
	WHERE strCatID = '#CategoryID#'
</CFQUERY>

<!--- This CFQUERY retrieves the items that belong
      to the current category --->
<CFQUERY DATASOURCE="#Application.datasource#" NAME="GetItems">
	SELECT 
	strItmID,
	strItmName,
	strPartNum,
	dblItmCost,
	strTeaser,
	strName
	FROM tblStItm, tblMfg
	WHERE strMfgIDFK = strMfgID
		AND strCatIDFK = '#CategoryID#'
	ORDER BY strItmID
</CFQUERY>

<!--- Set this for later --->
<CFCOOKIE NAME="LastCategory" VALUE=#CategoryID#>

<HTML>
<HEAD>
<TITLE>Online Store - Browse Items</TITLE>
</HEAD>

<BODY BGCOLOR="#FFCC00" LINK="Maroon" background="images/storebg.gif" leftmargin=5 topmargin=0>


<SPAN STYLE="position: absolute; left: 205px; top: 10px; width: 350px">
<CFOUTPUT><FONT FACE="Arial Black, Helvetica Bold, Geneva, Helvetica" SIZE="7" COLOR="##000000"><I>#GetCurrentCategory.strCatName#</I></FONT></CFOUTPUT>
<BR>
<TABLE BORDER="0" CELLPADDING="5" CELLSPACING="0" WIDTH="350">

	<!--- This CFOUTPUT block loops over the GetCategories query
	      and creates the following HTML for each one. --->
	<CFOUTPUT QUERY="GetItems">

		<FORM ACTION="additem.cfm" METHOD="POST">

			<INPUT TYPE="HIDDEN" NAME="ItemID" VALUE="#strItmID#">

			<TR>
				<TD><B><a href="showoneitem.cfm?ItemID=#strItmID#"> <!--target="DetailWindow"-->#strItmName#</a></B></TD>
				<TD><B>#strName#</b><BR></td>
				<TD ALIGN="RIGHT">
					<FONT FACE="Helvetica" SIZE="-1" COLOR="##000000"><B>#DollarFormat(dblItmCost)#</b></FONT>
				</TD>
				<TD ALIGN="RIGHT"><INPUT TYPE="SUBMIT" VALUE="Buy"></FONT></TD>
			</TR>

			<TR>
				<TD COLSPAN="4"><font size="-1" color="Blue">&nbsp;&nbsp;&nbsp;&nbsp;#strTeaser#</font></TD>
			</TR>

		</FORM>
	
	</CFOUTPUT>
</TABLE>

<FONT FACE="Helvetica" SIZE="-1">
	<A HREF="showcategories.cfm">Return to Categories</A>
</FONT>
<CF_ShowDoc ID=2>
<BR><BR>
	</TD>
</TR>
</TABLE>

</SPAN>

<CFINCLUDE TEMPLATE="_showcart.cfm">

</BODY>
</HTML>
