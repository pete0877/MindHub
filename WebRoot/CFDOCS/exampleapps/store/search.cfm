
<CFTRY>
<!--- Check whether the index is properly built ---->
<CFSEARCH 
		NAME="CheckSearch" 
		COLLECTION="CFexamples" 
		CRITERIA = ""
		MAXROWS = "1">

		<CFCATCH type="any">
<!--- Please ignore this code... it checks whether the online
	store's Verity collection exists, and if not, creates it.

	You wouldn't have to do this, instead you would use the
	Cold Fusion administrator to create your collection. --->

<CFREGISTRY ACTION=GETALL Branch="HKEY_LOCAL_MACHINE\SOFTWARE\Allaire\ColdFusion\CurrentVersion\Collections" 
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
	<CFREGISTRY ACTION=GET Branch="HKEY_LOCAL_MACHINE\SOFTWARE\Allaire\ColdFusion\CurrentVersion\Collections"
				Entry="RootDirectory"
				Type="STRING"
				Variable="CFRootDir">
	<CFIF Server.OS.Name IS NOT "UNIX">
		<CFSET CollectionPath="#CFRootDir#\Verity\Collections\">
	<CFELSE>
		<CFSET CollectionPath="#CFRootDir#/verity/collections/">
	</CFIF>

	<CFCOLLECTION Action="CREATE" 	Collection=#Application.datasource# 
									Path="#CollectionPath#"
									Language="english">
</CFIF> 


<!--- Check whether the index is properly built ---->
<CFSEARCH 
		NAME="CheckSearch" 
		COLLECTION=#Application.datasource# 
		CRITERIA = ""
		MAXROWS = "1">

<!--- OK, start paying attention again... :) --->
		</CFCATCH>
</CFTRY>


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

	<!--- If the index is empty, we need to index it; we're
		assuming that the store has at least one item --->
	<CFIF CheckSearch.RecordCount IS 0 >

		<H2><FONT FACE="Helvetica">Quick Search</FONT></H2>
		
		<P><FONT FACE="Helvetica" SIZE="-1">This store needs to be
		indexed before it can be searched. Please be patient; indexing
		may	take several moments, but only needs to be performed
		once.</FONT></P>

		<FORM ACTION="verityindex.cfm" METHOD="GET">
			<INPUT TYPE="SUBMIT" VALUE="Index">
		</FORM>

	<!--- Otherwise just make an HTML form --->
	<CFELSE>

		<H2><FONT FACE="Helvetica">Quick Search</FONT></H2>

		<FORM ACTION="results.cfm" METHOD="POST">
			<INPUT TYPE="TEXT" NAME="Keyword"><BR>
			<INPUT TYPE="SUBMIT" VALUE="Search">
		</FORM>

	</CFIF>

	<CF_ShowDoc ID=1000>

	</TD>
</TR>
</TABLE>


</BODY>
</HTML>
