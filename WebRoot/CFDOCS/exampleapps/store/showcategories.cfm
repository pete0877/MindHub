<!--- This CFQUERY retrieves the categories from the database --->
<CFQUERY DATASOURCE="#Application.datasource#" NAME="GetCategories">
	SELECT strCatID, strCatName, glbCatDesc FROM tblStCat
</CFQUERY>
<!--- clear last category cookie --->
<CFIF IsDefined("Cookie.LastCategory")>
	<cfcookie name="LastCategory" expires="NOW">
</cfif>
<HTML>
<HEAD>
<TITLE>Online Store - Choose Category</TITLE>
</HEAD>

<BODY BGCOLOR="FFcc33" BACKGROUND="images/storebg.gif" LEFTMARGIN=5 TOPMARGIN=0>

<SPAN STYLE="position: absolute; left: 250px; top: 45px">
	<IMG SRC="images/airboard.gif" WIDTH=273 HEIGHT=275 BORDER=0 ALT="" HSPACE=0 VSPACE=0>
</SPAN>

<SPAN STYLE="position: absolute; left: 100px; top: 30px">
	<IMG SRC="images/tack2.gif" WIDTH=249 HEIGHT=58 BORDER=0 ALT="TACK2">
</SPAN>

<SPAN STYLE="position: absolute; left: 255px; top: 255px">
	<IMG SRC="images/allthefunk.gif" WIDTH=291 HEIGHT=38 BORDER=0 ALT="All the Funk You NEED" VSPACE=0 HSPACE=0>
</SPAN>


<SPAN STYLE="position: absolute; left: 200px; width: 350px; top: 380px">
	<!--- This CFOUTPUT block loops over the GetCategories query
	      and creates the following HTML for each one. --->
	<TABLE BORDER="0" CELLPADDING="5" CELLSPACING="0" WIDTH="350">

	<CFOUTPUT QUERY="GetCategories">

	<FORM ACTION="showitems.cfm" METHOD="GET">
		
		<!--- This form variable tells showitems.cfm which 
		      category to get items from --->
		<INPUT TYPE="HIDDEN" NAME="CategoryID" VALUE="#strCatID#">

	<TR>
		<TD><FONT FACE="Arial Black, Helvetica Bold, Geneva, Helvetica" SIZE="5" COLOR="##000000"><I>#strCatName#</I></FONT></TD>
		<TD ALIGN="RIGHT"><FONT FACE="Helvetica" SIZE="-1"><INPUT TYPE="SUBMIT" VALUE="Browse"></FONT></TD>
	</TR>
	<TR>
		<TD COLSPAN="2"><FONT FACE="Helvetica" SIZE="-1">#glbCatDesc#</FONT></TD>
	</TR>
	<TR>
		<TD HEIGHT="15">&nbsp;</TD>
	</TR>

	</FORM>
	
	</CFOUTPUT>
			
	<TR>
		<TD HEIGHT="50">&nbsp;</TD>
	</TR>
			
	</TABLE>
		
	<CF_SHOWDOC ID=1>

</SPAN>

<CFINCLUDE TEMPLATE="_showcart.cfm">

</BODY>
</HTML>
