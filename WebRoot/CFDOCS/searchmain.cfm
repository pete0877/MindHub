<HTML>
<HEAD>
	<TITLE>Search</TITLE>
</HEAD>
<BODY bgcolor="999999" link="blue" vlink="#669966" leftmargin=1 topmargin=0 background="bg30.gif" text="Black">
<table border="0" cellspacing="0" cellpadding="0" bgcolor="White">
<tr>
	<td width=10>&nbsp;</td>
	<td valign="top" height="175" width=100%>


<!--- Check whether the index collection 'cfdocumentation' exists ---->
	<!--- Get all collection names. --->




	<CFREGISTRY ACTION=GETALL Branch="HKEY_LOCAL_MACHINE\SOFTWARE\Allaire\ColdFusion\CurrentVersion\Collections" 
		Type="KEY"
		Name="Collections">

	<CFSET CollectionFound = 'No'>
	<CFLOOP QUERY="Collections">
		<CFIF Collections.Entry IS 'cfdocumentation'>
			<CFSET CollectionFound = 'Yes'>
		</CFIF>
	</CFLOOP>

	<CFIF CollectionFound IS 'No'>

		<!--- Create the collection --->
		
		<CFREGISTRY ACTION=GET Branch="HKEY_LOCAL_MACHINE\SOFTWARE\Allaire\ColdFusion\CurrentVersion\Collections"
			ENTRY="RootDirectory"
			TYPE="STRING"
			VARIABLE="CFRootDir">
		<CFIF Server.OS.Name IS NOT "UNIX">
			<CFSET CollectionPath="#CFRootDir#\Verity\Collections\">
		<CFELSE>
			<CFSET CollectionPath="#CFRootDir#/verity/collections/">
		</CFIF>

		<CFCOLLECTION Action="CREATE"
			COLLECTION="cfdocumentation" 
			PATH="#CollectionPath#"
			LANGUAGE="english">
	</CFIF> 


<!--- Check whether the index is properly built ---->
<CFSEARCH 
		NAME="CheckSearch" 
		COLLECTION="cfdocumentation" 
		CRITERIA = ""
		MAXROWS = "1">

<CFIF CheckSearch.RecordCount IS 0 >

<!--- Empty Index. Indexing needed first. --->

	<FONT FACE="Arial, Helvetica" SIZE="-1"><b>Index Allaire Documentation</b></FONT><BR>
	You need to index the Allaire documentation before using it for the first time.
	This process may take a minute or two.<br>

	<form action="reindexalldocuments.cfm?RequestTimeout=300" method="POST" target="main">
		<CFIF ParameterExists(SearchString)>
			<INPUT TYPE="hidden" NAME="SearchString" VALUE="<CFOUTPUT>#SearchString#</CFOUTPUT>">
		</CFIF>
		<INPUT TYPE="submit" VALUE="Index">
	</FORM>
	
<CFELSE>

	
<!--- Run the search if any --->
<CFIF ParameterExists(SearchString)> 
	
	<CFSEARCH 
		NAME="Search1" 
		COLLECTION="cfdocumentation" 
		CRITERIA = "#ReplaceList(SearchString,'<,>,=','\<,\>,\=')#">

		<TABLE CELLSPACING=0 CELLPADDING=0 WIDTH="100%">
		<TR><TD HEIGHT="10"></TD></TR>
		<TR>
			<TD><FONT SIZE="-2" FACE="Arial, Helvetica"><B>Title</B> (<CFOUTPUT>#Search1.RecordCount#</CFOUTPUT> Documents) </FONT></TD>
			<TD><FONT SIZE="-2" FACE="Arial, Helvetica"><B>Book</B></FONT></TD>
		</TR>
		<TR>
			<TD COLSPAN=3><HR SIZE="1"></TD>
		</TR>
		<CFIF Search1.RecordCount IS 0>
		<TR>
			<TD COLSPAN=3><FONT SIZE="-2" FACE="Arial, Helvetica">No documents found. Modify your criteria and try again.</FONT></TD>
		</TR>
		</CFIF>
	
<CFOUTPUT QUERY="Search1">

			<!--- Show the search results.--->

			<CFSET DisplayDocument = 'Yes'>
			<CFIF ParameterExists(SearchAll) >
			</CFIF>

	
	<CFIF DisplayDocument IS 'Yes'>
		<TR> 
			<TD NOWRAP><FONT SIZE="-2" FACE="Arial, Helvetica"><A TARGET="opener" HREF="#Search1.URL#">
				<CFIF Search1.TITLE IS NOT "">
					#HTMLEditFormat(Search1.TITLE)#
				<CFELSE>
					Unknown Title (#HTMLEditFormat(Search1.URL)#)
				</CFIF></A></FONT></TD>
				
			<TD NOWRAP><FONT SIZE="-2" FACE="Arial, Helvetica">
			
			<!--- #Search1.URL# --->
				
  				<CFIF Left(Search1.URL, 27) IS "allaire_support/newfeatures">New Features in CF 4.0
					<CFELSEIF Left(Search1.URL, 28) IS "allaire_support/releasenotes">Release Notes
					<CFELSEIF Left(Search1.URL, 23) IS "allaire_support/support">Allaire Technical Support
					<CFELSEIF Left(Search1.URL, 15) IS "Getting_Started">Getting Started
					<CFELSEIF Left(Search1.URL, 13) IS "CFML_Language">CFML Reference
					<CFELSEIF Left(Search1.URL, 10) IS "Developing">Developing Web Applications with CF
					<CFELSEIF Left(Search1.URL, 13) IS "Administering">Administering the CF Application Server
					<CFELSEIF Left(Search1.URL, 8) IS "Advanced">Advanced CF Development
					<CFELSE>Unknown Book
				</CFIF>
				</FONT></TD>
					</TR>
	</CFIF>
	
</CFOUTPUT>
		<TR><TD COLSPAN=3><HR SIZE="1"></TD></TR>
		</TABLE>
	</CFIF>
<!--	<FONT FACE="Arial, geneva, Helvetica" SIZE=-1><br>

		Other information can go here. Maybe&nbsp;Allaire&nbsp;contact&nbsp;information:<br>
		Links to Website, 
		Links to training 
		Phone numbers for sales and training 
		Links to front pages of book titles<br>
		<br>
		<br>
		<br>
		<br>
	</font> -->
<!--- 	
	<FONT FACE="Arial, Helvetica" SIZE=-1><b>Search Allaire Documentation</b></font>
	<form action="searchmain.cfm?RequestTimeout=300" method="POST" target="main" name="Search">
	
<CFOUTPUT>
	<INPUT TYPE=text NAME=SearchString SIZE=20
<CFIF ParameterExists(SearchString)>VALUE="#SearchString#"</CFIF>>
	<INPUT TYPE=SUBMIT NAME=search1 VALUE="Search"> 
</CFOUTPUT>
	</FORM> --->
<!--- 
	<BR>
<FONT FACE="Arial, Helvetica" SIZE=-1><b>Reindex Allaire Documentation</b></FONT><br>	
<FORM action="reindexalldocuments.cfm">
<INPUT TYPE="submit" VALUE="Reindex">
</form> --->

	
</CFIF>

</td>
</tr>
</table>
	</td>
</tr>
</table>

</BODY>
</HTML>
