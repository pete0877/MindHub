<!--- This template allows the user to provide information about
	a new employee, and passes the data to InsertAction.cfm, which
	adds the data to the database --->

<CFINCLUDE TEMPLATE="_header.cfm">

<!--- This fetches the names and ID's of the departments, which will
	allow us to create a drop-down box --->
<CFQUERY datasource="cfx" NAME="GetDepartments">
SELECT * FROM tblDpt
ORDER BY strDptName
</CFQUERY>

<TR>
   	<TD WIDTH=100 ALIGN="RIGHT" VALIGN="TOP" HEIGHT="150">&nbsp;</TD>

	<TD VALIGN="TOP">

		<CFFORM ACTION="insertaction.cfm" METHOD="POST">
		
		<FONT FACE="Helvetica" SIZE="-1">

		<P>First Name<BR>
		<CFINPUT TYPE="TEXT" NAME="strFName" REQUIRED="YES" MESSAGE="You must enter a first name."></P>
		
		<P>Last Name<BR>
		<CFINPUT TYPE="TEXT" NAME="strLName" REQUIRED="YES" MESSAGE="You must enter a last name."></P>
		
		<P>Title<BR>
		<INPUT TYPE="TEXT" NAME="strTitle"></P>
		
		<P>Department<BR>
		<SELECT NAME="strDptIDFK">
			<CFOUTPUT QUERY="GetDepartments">
				<OPTION VALUE="#strDptID#">
				#strDptName#
			</CFOUTPUT>
		</SELECT></P>
		
		<P><INPUT TYPE="CHECKBOX" NAME="binIsTemp">Temporary</P>
		
		<P>E-mail<BR>
		<INPUT TYPE="TEXT" NAME="strEmail"></P>
		
		<P>Phone<BR>
		<INPUT TYPE="TEXT" NAME="strPhone"></P>
		
		<P>Start Date<BR>
		<CFINPUT TYPE="TEXT" NAME="dtStrtDate" VALIDATE="date" MESSAGE="Please enter a valid start date."></P>
		
		<P>Personal Information<BR>
		<TEXTAREA NAME="glbPersnl" COLS="25" ROWS="5"></TEXTAREA>
		
		<P><INPUT TYPE="SUBMIT" VALUE="Add Employee"></P>

		</FONT>
		
		</CFFORM>

	</TD>

   	<TD WIDTH=100 ALIGN="RIGHT" VALIGN="TOP" HEIGHT="150">&nbsp;</TD>

</TR>

<TR>
	<TD HEIGHT="35" COLSPAN="3">&nbsp;</TD>
</TR>

</TABLE>

<!--- Close the document --->
<CFINCLUDE TEMPLATE="_footer.cfm">