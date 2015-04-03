<CFINCLUDE TEMPLATE="_header.cfm">

<CFQUERY datasource="cfx" NAME="SearchResults">
	SELECT * FROM tblEmp, tblDpt
	WHERE strDptIDFK = strDptID

	<CFIF Form.Name NEQ "">
		<!--- Access version --->
		<CFIF Server.OS.Name CONTAINS "Windows">
		AND (strFName LIKE '%#Form.Name#%'
		OR strLName LIKE '%#Form.Name#%'
		OR strFName & ' ' & strLName LIKE '%#Form.Name#%')

		<!--- dBase Version --->
        <CFELSE>
		AND (Upper(strFName) LIKE '%#UCase(Form.Name)#%'
		OR Upper(strLName) LIKE '%#UCase(Form.Name)#%'
		OR Upper(Trim(strFName)) + ' ' + Upper(Trim(strLName)) LIKE '%#UCase(Form.Name)#%')

		</CFIF> 
	</CFIF>
	
	<CFIF Form.strDptIDFK NEQ "All">
	AND strDptIDFK = '#Form.strDptIDFK#'
	</CFIF>

	ORDER BY strDptName, strDptIDFK, strLName, strFName
</CFQUERY>

<TR>
	<TD COLSPAN="3">
		<FONT FACE="Arial, Geneva, Helvetica" SIZE="-1">
			<P><CFOUTPUT>Your search for <B>"#Form.Name#"</B> has returned <B>#SearchResults.RecordCount#</B> employee(s).</CFOUTPUT></P>
		</FONT>
	</TD>
</TR>
<TR>
	<TD HEIGHT="50" COLSPAN="3">&nbsp;</TD>
</TR>

<CFOUTPUT QUERY="SearchResults">
<TR>

   	<TD WIDTH=100 ALIGN="RIGHT" VALIGN="TOP" HEIGHT="150">&nbsp;</TD>

    <TD VALIGN="top">
	    <FONT FACE="Myriad Web, Verdana, Helvetica" SIZE="+2">
        	<!--- Notice we're passing the EmpID in the URL... --->
			<A HREF="details.cfm?EmpID=#strEmpID#">#strFName# #strLName#</A><BR>
		</FONT>
    
		<FONT FACE="Helvetica">
        	<FONT SIZE="-2">#strTitle#<BR><BR></FONT>
			<FONT SIZE="-1">
				<CFIF strPhone NEQ ""><FONT FACE="WingDings">(</FONT> #strPhone#<BR></CFIF>
				<CFIF strEmail NEQ ""><FONT FACE="WingDings">*</FONT> <A HREF="mailto:#strEmail#">#strEmail#</A><BR></CFIF></P>
			</FONT>
        </FONT>
    </TD>
    
	<TD WIDTH="100" NOWRAP>&nbsp;</TD>

</TR>
</CFOUTPUT>

</TABLE>

<CFINCLUDE TEMPLATE="_footer.cfm">
