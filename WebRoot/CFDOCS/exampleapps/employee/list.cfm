<!--- This template lists the employees, grouped by department, and lets
	the user click on an employee to view details --->

<!--- Get employees (and their departments); and order by
	department because that's what we're going to group by --->
<CFQUERY datasource="cfx" NAME="GetEmployees">
SELECT * FROM tblEmp, tblDpt
WHERE strDptIDFK = strDptID
ORDER BY strDptName,strDptIDFK, strLName, strFName
</CFQUERY>

<!--- These two lines set up the <HEAD></HEAD>, <BODY></BODY>,
	and other page layout attributes that should be consistent though the site  --->
<CFINCLUDE TEMPLATE="_header.cfm">

<TR>
	<TD COLSPAN="3">

		<TABLE WIDTH="90%" BORDER="0" CELLSPACING="0" CELLPADDING="5" ALIGN="CENTER">
		
		<!--- Output the query data; the code between this <CFOUTPUT>
			and the next <CFOUTPUT> is only outputted once per
			DepartmentID, since it is grouped by DepartmentID --->
		<CFOUTPUT QUERY="GetEmployees" GROUP="strDptID">
			<TR>
				<TD COLSPAN="4" HEIGHT=30 VALIGN="bottom">
					<BR><BR><FONT FACE="Myriad Web, Verdana, Helvetica" SIZE="4"><B>#strDptName#</B></FONT>
                </TD>
			</TR>

			<!--- Now we start outputting for every record
				(i.e. every employee) --->
			<CFOUTPUT>
				<TR>
					<TD WIDTH=25 NOWRAP>&nbsp;</TD>
					<TD ALIGN="right" VALIGN="top">
						<FONT FACE="Arial, Geneva, Helvetica" SIZE="-1">
							<A HREF="details.cfm?EmpID=#strEmpID#">#strLName#, #strFName#</A><BR>
						</FONT>
						<FONT FACE="Arial, Geneva, Helvetica" SIZE="-2">
							#strTitle#<BR>
						</FONT>
					</TD>
					<TD VALIGN="top" ALIGN="right"><FONT FACE="Arial, Geneva, Helvetica" SIZE="-1">#strPhone#<BR></FONT></TD>
					<TD VALIGN="top" ALIGN="right"><FONT FACE="Arial, Geneva, Helvetica" SIZE="-1"><A HREF="mailto:#strEmail#">#strEmail#</A><BR></FONT></TD>
				</TR>
			</CFOUTPUT>

        </CFOUTPUT>
		
		</TABLE>
	</TD>
</TR>
<TR>
	<TD HEIGHT="35" COLSPAN="3">&nbsp;</TD>
</TR>
</TABLE>		

<!--- Close the document --->
<CFINCLUDE TEMPLATE="_footer.cfm">