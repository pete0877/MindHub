<CFSET TITLE = "Employee Search">
<CFINCLUDE TEMPLATE="_header.cfm">

<CFQUERY datasource="cfx" NAME="GetDepartments">
SELECT * FROM tblDpt
</CFQUERY>
<TR>
	<TD HEIGHT="250" WIDTH="10" nowrap>&nbsp;</TD>
	<TD VALIGN="TOP">
		<FORM ACTION="searchaction.cfm" METHOD="POST">
		<FONT FACE="Myriad Web, Verdana, Helvetica" SIZE="-1">

        <P>Name (first, last, or both)<BR>
		<INPUT TYPE="TEXT" NAME="Name"></P>

		<P>Department<BR>
		<SELECT NAME="strDptIDFK">
			<OPTION VALUE="All">Search All
			<CFOUTPUT QUERY="GetDepartments">
				<OPTION VALUE="#strDptID#">#strDptName#
			</CFOUTPUT>
		</SELECT></P>

		<P><INPUT TYPE="SUBMIT" VALUE="Search"></P>


		</FONT>
		</FORM>
	</TD>
    <TD>&nbsp;</TD>
</TR>
</TABLE>
<CFINCLUDE TEMPLATE="_footer.cfm">