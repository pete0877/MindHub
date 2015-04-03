
<!--- This is the query whose data will be stored in the Verity index --->
<CFQUERY DATASOURCE=#Application.datasource# NAME="GetItems">
		SELECT StoreItems.*, StoreManufacturers.Name FROM StoreItems, StoreManufacturers 
		WHERE StoreManufacturers.ManufacturerID = StoreItems.ManufacturerID
</CFQUERY>

<!--- This CFINDEX tag will actually perform the indexing of the query
	data.  For each record, all the text in StoreManufacturers.Name,
	StoreItems.ItemName, and StoreItems.ItemDesc will be indexed; when
	a search is performed on this index, the ItemID and ItemName will
	be returned as the Key and Title. --->
<CFINDEX ACTION="UPDATE"
	COLLECTION=#Application.datasource#
	TYPE="CUSTOM"
	BODY="Name,ItemName,ItemDesc,Teaser,PartNum"
	CUSTOM1="Name"
	CUSTOM2="ItemPrice"
	KEY="ItemID"
	TITLE="ItemName"
	QUERY="GetItems">

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2 Final//EN">

<HTML>
<HEAD>
	<TITLE>Online Store - Search</TITLE>
</HEAD>


<BODY BGCOLOR="#FFCC00" LINK="Maroon" BACKGROUND="images/storebg.gif" LEFTMARGIN=5 TOPMARGIN=0>


<TABLE BORDER="0" CELLSPACING="0" CELLPADDING="0">
<TR>
	<TD WIDTH=130 HEIGHT=15 NOWRAP ALIGN="RIGHT">
		<CFINCLUDE TEMPLATE="_showcart.cfm">
	</TD>
	<TD WIDTH="70" NOWRAP>&nbsp;</TD>
	<TD VALIGN="top" WIDTH=350>

	<BR><BR>
	
	<P><FONT FACE="Helvetica" SIZE="-1">Indexing complete.</FONT></P>
	
	<P><FONT FACE="Helvetica" SIZE="-1"><A HREF="search.cfm">Search</A></FONT></P>

	<CF_ShowDoc ID=1000>
	
	</TD>
</TR>
</TABLE>

</BODY>
</HTML>
