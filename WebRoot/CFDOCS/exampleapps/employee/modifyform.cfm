<!--- This template is just an HTML form that passes information to ModifyAction.cfm.
	Each of the input elements have default values from the database. --->

<CFINCLUDE TEMPLATE="_header.cfm">

<CFQUERY datasource="cfx" NAME="GetDepartments">
SELECT * FROM tblDpt
ORDER BY strDptName
</CFQUERY>

<!--- The query GetEmployee will be used to populate the input fields. --->
<CFQUERY datasource="cfx" NAME="GetEmployee">
SELECT * FROM tblEmp, tblDpt
WHERE strDptIDFK = strDptID
	AND strEmpID = '#URL.EmpID#'
</CFQUERY>

<TR>
   	<TD WIDTH=100 ALIGN="RIGHT" VALIGN="TOP" HEIGHT="150">&nbsp;</TD>

	<TD VALIGN="TOP">

		<CFOUTPUT QUERY="GetEmployee">
        
		<CFFORM ACTION="modifyaction.cfm" METHOD="POST">
		<INPUT TYPE="HIDDEN" NAME="strEmpID" VALUE="#strEmpID#">
		
		<FONT FACE="Helvetica" SIZE="-1">

		<P>First Name<BR>
		<CFINPUT TYPE="TEXT" NAME="strFName" VALUE="#strFName#" REQUIRED="YES" MESSAGE="You must enter a first name."></P>
		
		<P>Last Name<BR>
		<CFINPUT TYPE="TEXT" NAME="strLName" VALUE="#strLName#" REQUIRED="YES" MESSAGE="You must enter a first name."></P>
		
		<P>Title<BR>
		<INPUT TYPE="TEXT" NAME="strTitle" VALUE="#strTitle#"></P>
		
		<P>Department<BR>
		<SELECT NAME="strDptIDFK">
			<CFLOOP QUERY="GetDepartments">
				<OPTION VALUE="#strDptID#"
				<CFIF strDptID IS GETEMPLOYEE.strDptIDFK>SELECTED</CFIF>
				>
				#strDptName#
			</CFLOOP>
		</SELECT></P>
		
		<P><INPUT TYPE="CHECKBOX" NAME="binIsTemp"
		<CFIF binIsTemp IS TRUE>CHECKED</CFIF>
		>Temporary</P>
		
		<P>E-mail<BR>
		<INPUT TYPE="TEXT" NAME="strEmail" VALUE="#strEmail#"></P>
		
		<P>Phone<BR>
		<INPUT TYPE="TEXT" NAME="strPhone" VALUE="#strPhone#"></P>
		
		<P>Start Date<BR>
		<CFINPUT TYPE="TEXT" NAME="dtStrtDate" VALIDATE="date" MESSAGE="Please enter a valid start date." VALUE="#DateFormat(dtStrtDate,"mm/dd/yy")#"></P>
		
		<P>Personal Information<BR>
		<TEXTAREA NAME="glbPersnl" COLS="25" ROWS="5">#glbPersnl#</TEXTAREA>
		
		<P><INPUT TYPE="SUBMIT" VALUE="Modify"></P>

		</FONT>
		
		</CFFORM>
        </CFOUTPUT>

	</TD>

   	<TD WIDTH=100 ALIGN="RIGHT" VALIGN="TOP" HEIGHT="150">&nbsp;</TD>

</TR>

<TR>
	<TD HEIGHT="35" COLSPAN="3">&nbsp;</TD>
</TR>

</TABLE>

<CFINCLUDE TEMPLATE="_footer.cfm">