<!--- This template displays the employees for any given department and lets
	the user click on an employee to view details. --->

<!--- If we weren't given a DepartmentID, this template can't be
	processed; go instead to the employee list --->
<CFIF NOT IsDefined("URL.DepartmentID")>
	<CFLOCATION URL="index.cfm">
</CFIF>

<!--- Get department name using department ID (from URL);
	this allows us to display department name in header --->
<CFQUERY NAME="GetDepartmentName" datasource="cfx">
SELECT strDptName FROM tblDpt
WHERE strDptID = '#URL.DepartmentID#'
</CFQUERY>

<!--- Get employees (and their departments); and order by
	department because that's what we're going to group by --->
<CFQUERY datasource="cfx" NAME="GetEmployees">
SELECT * FROM tblEmp
WHERE strDptIDFK = '#URL.DepartmentID#'
ORDER BY strDptIDFK, strLName, strFName
</CFQUERY>

<!--- This includes the HTML that goes into the header. --->
<CFINCLUDE TEMPLATE="_header.cfm">

<CFOUTPUT QUERY="GetEmployees">

<!--- Now we start outputting for every record (i.e. every employee) --->
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

<TR>
	<TD HEIGHT="35" COLSPAN="3">&nbsp;</TD>
</TR>

</TABLE>

<CFINCLUDE TEMPLATE="_footer.cfm">

</BODY>
</HTML>