<!--- Search the Verity collection for our keyword(s) --->

<CFTRY>
	
	<CFSEARCH COLLECTION="#Application.datasource#"
		CRITERIA="#Form.Keyword#"
		NAME="SearchResults"
		TYPE="SIMPLE">

	<CFCATCH TYPE="any">

		<CFREGISTRY ACTION=GETALL
			Branch="HKEY_LOCAL_MACHINE\SOFTWARE\Allaire\ColdFusion\CurrentVersion\Collections" 
			Type="KEY"
			Name="Collections">

		<CFSET CollectionFound = 'No'>

		<CFLOOP QUERY="Collections">
	
			<CFIF Collections.Entry IS Application.datasource>
				<CFSET CollectionFound = 'Yes'>
			</CFIF>

		</CFLOOP>

		<CFIF CollectionFound IS 'No'>

		<!--- Create the collection --->

			<CFREGISTRY ACTION=GET
				Branch="HKEY_LOCAL_MACHINE\SOFTWARE\Allaire\ColdFusion\CurrentVersion\Collections"
				Entry="RootDirectory"
				Type="STRING"
				Variable="CFRootDir">

			<CFIF Server.OS.Name IS NOT "UNIX">
				<CFSET CollectionPath="#CFRootDir#\Verity\Collections\">
			<CFELSE>
				<CFSET CollectionPath="#CFRootDir#/verity/collections/">
			</CFIF>

				<CFCOLLECTION Action="CREATE" 	
					Collection=#Application.datasource# 
					Path="#CollectionPath#"
					Language="english">
		</CFIF> 


	<!--- This is the query whose data will be stored in the Verity index --->
	<CFQUERY DATASOURCE="#Application.datasource#" NAME="GetItems">
		SELECT *, strName FROM tblStItm, tblMfg 
		WHERE strMfgID = strMfgIDFK
	</CFQUERY>

	<!--- This CFINDEX tag will actually perform the indexing of the query
	data.  For each record, all the text in StoreManufacturers.Name,
	StoreItems.ItemName, and StoreItems.ItemDesc will be indexed; when
	a search is performed on this index, the ItemID and ItemName will
	be returned as the Key and Title. --->

	<CFINDEX ACTION="UPDATE"
		COLLECTION="#Application.datasource#"
		TYPE="CUSTOM"
		BODY="strName,strItmName,glbItmDesc,strTeaser,strPartNum"
		KEY="strItmID"
		CUSTOM1="strName"
		CUSTOM2="dblItmCost"
		TITLE="strItmName"
		QUERY="GetItems">

	<CFSEARCH COLLECTION=#Application.datasource#
		CRITERIA="#Form.Keyword#"
		NAME="SearchResults"
		TYPE="SIMPLE">
		
	</CFCATCH>
	
</CFTRY>

<!--- OK, start paying attention again... :^) --->

<!--- Check whether the index is properly built ---->

	
<!--- Now retrieve all the items from the store that
	are part of the recordset CFSEARCH returned to us --->

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>Online Store - Search Results</TITLE>
</HEAD>

<BODY BGCOLOR="#FFCC00" LINK="Maroon" BACKGROUND="images/storebg.gif" LEFTMARGIN=5 TOPMARGIN=0>

<CFINCLUDE TEMPLATE="_showcart.cfm">

<SPAN STYLE="position: absolute; left: 205px; top: 10px; width: 300px; z-index: 1">

	<!--- Show how many items we found --->
	<P><FONT FACE="Helvetica"><B><CFOUTPUT>#SearchResults.RecordCount#</CFOUTPUT></B> item(s) found.</FONT></P>

	<BR><BR>

	<TABLE BORDER="0" CELLSPACING="0" CELLPADDING="0" WIDTH="350">

	<!--- Now we'll just show the items that match the search,
		if any; this code is taken straight from showitems.cfm. --->
	<CFOUTPUT QUERY="SearchResults">
	
		<FORM ACTION="additem.cfm" METHOD="POST">

			<INPUT TYPE="HIDDEN" NAME="ItemID" VALUE=#Key#>

			<TR>
				<TD><B><a href="showoneitem.cfm?ItemID=#Key#">#Title#</a></B></TD>
				<TD><B>#Custom1#</b><BR></td>
				<TD ALIGN="RIGHT">
					<FONT FACE="Helvetica" SIZE="-1" COLOR="##000000"><B>#DollarFormat(Custom2)#</b></FONT>
				</TD>
				<TD ALIGN="RIGHT"><INPUT TYPE="SUBMIT" VALUE="Buy"></FONT></TD>
			</TR>

			<TR>
				<TD COLSPAN="4"><font size="-1" color="Blue">&nbsp;&nbsp;&nbsp;&nbsp;#Summary#</font></TD>
			</TR>

		</FORM>
	
	</CFOUTPUT>

	</TABLE>
</SPAN>

</BODY>
</HTML>
