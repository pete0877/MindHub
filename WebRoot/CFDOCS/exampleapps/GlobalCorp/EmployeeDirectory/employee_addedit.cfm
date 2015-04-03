<!--- ColdFusion(r) Express Global Corp. Example Application --->

<!--- If the user is cancelling the operation, kick them back to this page --->
<CFIF isdefined("form.cancel")>
	<CFSET message="The operation was cancelled">
	<CFLOCATION URL="#getfilefrompath(getbasetemplatepath())#?message=#urlencodedformat(message)#">
</CFIF>

<!--- If the user chose to delete an employee with the form --->
<CFIF isdefined("form.delete")>
	<CFQUERY NAME="DELETE" DATASOURCE="#application.ds#">
	DELETE FROM   tblAttnd
	WHERE  strEmpIDFK = '#form.employee_id#'	
	</CFQUERY>
	<CFQUERY NAME="DELETE" DATASOURCE="#application.ds#">
	DELETE FROM   tblRes
	WHERE  strEmpIDFK = '#form.employee_id#'	
	</CFQUERY>
	<CFQUERY NAME="DELETE" DATASOURCE="#application.ds#">
	DELETE FROM   tblMes
	WHERE  strEmpIDFK = '#form.employee_id#'	
	</CFQUERY>
	<CFQUERY NAME="DELETE" DATASOURCE="#application.ds#">
	DELETE FROM   tblEmp
	WHERE  strEmpID = '#form.employee_id#'	
	</CFQUERY>
	<CFSET MESSAGE="The employee was deleted">
	<CFLOCATION URL="#getfilefrompath(getbasetemplatepath())#?message=#urlencodedformat(message)#">
</CFIF>

<!---- If there is an employee_id in the url, then we  must be editing --->
<CFIF isdefined("url.employee_id")>
	<CFSET form.action="prepare_edit">
	<CFSET form.employee_id=url.employee_id>
</CFIF>	

<!--- Param the "action"  (possibles include: add, edit, prepare_edit) param the employee_id for cleanliness set error = 0 for a default, this will be changed if field validation trips up on any user input --->
<CFPARAM NAME="form.action" DEFAULT="add">
<CFPARAM NAME="form.employee_ID" DEFAULT="" TYPE="string">
<CFPARAM NAME="message" DEFAULT="">
<CFSET error=0>

<!--- If the action is add or edit then let's check for all required fields (firstname, lastname, email, password) --->
<CFIF isdefined("gogo")>

	<!--- Check for a firstname --->	
	<CFIF NOT len(trim(form.firstname))>
		<CFSET error=1>
		<CFSET message=message & "Please enter a first name for the employee<br>">	
	</CFIF>
	
	<!--- Check for a lastname --->
	<CFIF NOT len(trim(form.lastname))>
		<CFSET error=1>
		<CFSET message=message & "Please enter a last name for the employee<br>">
	</CFIF>

	<!--- Check for a well-formed email address --->
	<CFIF NOT REFind(".+@.+\..+",form.email)>
		<CFSET error=1>
		<CFSET message=message & "Please enter a valid email address for the employee<br>">
	</CFIF>	
	<!--- Check for password --->
	<CFIF NOT len(trim(form.password))>
		<CFSET error=1>
		<CFSET message=message & "Please enter a password for the employee<br>">
	</CFIF>
	<!--- Check for address --->
	<CFIF NOT len(trim(form.address))>
		<CFSET error=1>
		<CFSET message=message & "Please enter an address for the employee<br>">
	</CFIF>
	<!--- Check for city --->
	<CFIF NOT len(trim(form.city))>
		<CFSET error=1>
		<CFSET message=message & "Please enter a city for the employee<br>">
	</CFIF>
	<!--- Check for state --->
	<CFIF NOT len(trim(form.state))>
		<CFSET error=1>
		<CFSET message=message & "Please enter a state for the employee<br>">
	</CFIF>
	<!--- Make sure ZIP is 5 numbers --->
	<CFIF len(trim(form.zip)) neq 5 OR NOT isnumeric(form.zip) >
		<CFSET error=1>
		<CFSET message=message & "Please enter a zip-code for the employee<br>">
	</CFIF>
	<!--- Make sure phone is at least 7 characters --->
	<CFIF len(trim(form.phone)) lt 7>
		<CFSET error=1>
		<CFSET message=message & "Please enter a valid phone number for the employee<br>">
	</CFIF>
	
<!--- If all validation is passed run a routine to edit or add an employee --->
	<CFIF NOT error>
		
		<!--- Add employee --->
		<CFIF form.action is "add">
			<CFQUERY NAME="insert_employee" DATASOURCE="#application.ds#">
			INSERT INTO tblEmp
			(
			strEmpID,
			strFname,
			strLname,
			strEmail,
			strPasswd,
			strImagLoc,
			idDptIDFK,
			strAddress,
			strCity,
			strState,
			intZipCode,
			strPhone	
			)
			values
			(
			'#CreateUUID()#',
			'#form.firstname#',
			'#form.lastname#',
			'#form.email#',
			'#form.password#',
			'#form.image_location#',
			'#form.department_id#',
			'#form.address#',
			'#form.city#',
			'#form.state#',
			#form.zip#,
			'#form.phone#'
			)
			</CFQUERY>	
			<CFSET message=form.firstname & " " & form.lastname & " has been added">					
		<!--- Edit an employee --->
		<CFELSE>
		
			<CFQUERY NAME="edit_employee" DATASOURCE="#application.ds#">
			UPDATE 	tblEmp
			SET		strFname = '#form.firstname#',
					strLname = '#form.lastname#',
					strPasswd = '#form.password#',
					strImagLoc = '#form.image_location#',
					idDptIDFK = '#form.department_id#',
					strAddress = '#form.address#',
					strCity = '#form.city#',
					strState = '#ucase(form.state)#',
					intZipCode = #zip#,
					strPhone = '#phone#'
			WHERE	strEmpID = '#form.employee_id#'		
			</CFQUERY>			
			<CFSET message=form.firstname & " " & form.lastname & " has been edited">
		</CFIF>		
		<!--- Locate the user back to this page to refresh and give them a success message --->		
		<CFLOCATION URL="#getfilefrompath(getbasetemplatepath())#?message=#urlencodedformat(message)#">
	</CFIF>
</CFIF>


<!--- If we're editing, then get the information about the user from the database --->

<CFIF form.action is "prepare_edit"	>	
	<CFQUERY NAME="get_employee" DATASOURCE="#application.ds#">
	SELECT * 
	FROM   tblEmp
	WHERE  strEmpID = '#form.employee_id#'
	</CFQUERY>
	
		<CFIF get_employee.recordcount>
			<CFSET form.firstname=Trim(get_employee.strFname)>
			<CFSET form.lastname=Trim(get_employee.strLname)>
			<CFSET form.email=Trim(get_employee.strEmail)>
			<CFSET form.password=Trim(get_employee.strPasswd)>
			<CFSET form.department_id=Trim(get_employee.idDptIDFK)>
			<CFSET form.image_location=Trim(get_employee.strImagLoc)>
			<CFSET form.address=Trim(get_employee.strAddress)>
			<CFSET form.city=Trim(get_employee.strCity)>
			<CFSET form.state=Trim(get_employee.strState)>
			<CFSET form.zip=Trim(get_employee.intZipCode)>
			<CFSET form.phone=Trim(get_employee.strPhone)>
					
	
			<!--- Set the action for the upcoming page --->
			<CFSET form.action="edit">
			
			<CFSET message="Now editing " & form.firstname & " " & form.lastname>

		<CFELSE>
			<CFSET message="That user doesn't seem to exist!">
			<CFSET form.action="add">
		</CFIF>	
</CFIF>	

<!--- Determine the page name, based on adding or editing --->
<CFIF form.action is "edit">
	<CFSET header.page_name = "Edit Employee">
<CFELSE>
	<CFSET header.page_name = "Add Employee">
</CFIF>

<!--- Include the Global Header --->
<CFINCLUDE TEMPLATE="../CommonFiles/_header.cfm">

<!--- Script to Confirm Delete --->
<SCRIPT LANGUAGE="JavaScript">
function confirmThis(message) {
	if(confirm(message)) return true;
	return false;
}
</SCRIPT>



<!--- The Address Book Menu --->
<FONT SIZE="2">
{
<A HREF="index.cfm">Employee Search</A>
 | 
<!--- If we are adding, show link to edit --->
<CFIF NOT Len(form.employee_id)>
<A HREF="employee_choose.cfm">Edit an Employee</A>
<!--- If we are editing, show link to add --->
<CFELSE>
<A HREF="employee_addedit.cfm">Add an Employee</A>
</CFIF> 
 | 
<A HREF="index.cfm?ShowAllEmployees=1">Show All Employees</A>
}
</FONT>


<!--- If there is a message (validation error, notification of success of add/edit of an employee, etc. then display the message using len(trim(message)) because 	message is paramed at top of script --->
<CFIF len(trim(message))>
	<CFOUTPUT>
	<P>
	<B>#message#</B>
	</CFOUTPUT>
</CFIF>	

<!--- By default, the values of the fields are blank --->
<CFPARAM NAME="form.firstname" DEFAULT="">
<CFPARAM NAME="form.lastname" DEFAULT="">
<CFPARAM NAME="form.email" DEFAULT="">
<CFPARAM NAME="form.password" DEFAULT="">
<CFPARAM NAME="form.department_id" DEFAULT="0">
<CFPARAM NAME="form.image_location" DEFAULT="">
<CFPARAM NAME="form.address" DEFAULT="">
<CFPARAM NAME="form.city" DEFAULT="">
<CFPARAM NAME="form.state" DEFAULT="">
<CFPARAM NAME="form.zip" DEFAULT="">
<CFPARAM NAME="form.phone" DEFAULT="">

<FORM ACTION="employee_addedit.cfm" METHOD="post">
<!--- Determine if we are adding or editing and set the form action and submit button accordingly --->
<CFIF form.action is "edit">
	<CFSET submittext="Save Changes">
	<CFSET action_field="edit">
<CFELSE>
	<CFSET submittext="Add Employee">
	<CFSET action_field="add">
</CFIF>	

<CFOUTPUT>
<INPUT TYPE="hidden" NAME="gogo" VALUE="yes">
<INPUT TYPE="hidden" NAME="action" VALUE="#action_field#">
<INPUT TYPE="hidden" NAME="employee_id" VALUE="#form.employee_id#">	

<TABLE>
	<TR>
		<TD>First Name:</TD>
		<TD><INPUT TYPE="text" NAME="firstname" SIZE="20" MAXLENGTH="25" VALUE="#htmleditformat(firstname)#"></TD>
	</TR>
	<TR>
		<TD>Last Name:</TD>
		<TD><INPUT TYPE="text" NAME="lastname" SIZE="20" MAXLENGTH="25" VALUE="#htmleditformat(lastname)#"></TD>
	</TR>
	<TR>
		<TD>Password:</TD>
		<TD><INPUT TYPE="text" NAME="password" SIZE="20" MAXLENGTH="25" VALUE="#htmleditformat(password)#"></TD>
	</TR>
	<TR>
		<TD>Email:</TD>
		<TD><INPUT TYPE="text" NAME="email" SIZE="20" MAXLENGTH="25" VALUE="#htmleditformat(email)#"></TD>
	</TR>
	<TR>
		<TD>Phone:</TD>
		<TD><INPUT TYPE="text" NAME="phone" SIZE="20" MAXLENGTH="20" VALUE="#htmleditformat(phone)#"></TD>
	</TR>	
	<TR>
		<TD>Image File:</TD>
		<TD><INPUT TYPE="text" NAME="image_location" SIZE="20" MAXLENGTH="100" VALUE="#htmleditformat(image_location)#"></TD>
	</TR>
	<TR>
		<TD>Address:</TD>
		<TD><INPUT TYPE="text" NAME="address" SIZE="20" MAXLENGTH="50" VALUE="#htmleditformat(address)#"></TD>
	</TR>
	<TR>
		<TD>City:</TD>
		<TD><INPUT TYPE="text" NAME="city" SIZE="20" MAXLENGTH="50" VALUE="#htmleditformat(city)#"></TD>
	</TR>
	<TR>
		<TD>State/Province:</TD>
		<TD><INPUT TYPE="text" NAME="state" SIZE="2" MAXLENGTH="2" VALUE="#htmleditformat(ucase(state))#"></TD>
	</TR>
	<TR>
		<TD>Postal Code:</TD>
		<TD><INPUT TYPE="text" NAME="zip" SIZE="5" MAXLENGTH="5" VALUE="#zip#"></TD>
	</TR>
</CFOUTPUT>		
	
	<CFQUERY NAME="Get_Departments" DATASOURCE="#application.ds#">
		SELECT 	* 
		FROM 	tblDpt
	</CFQUERY>
	
	<TR>
		<TD>Department:</TD>
		<TD>
			<!--- Create a select box, preselect if the form is being show in edit mode, or if the user has submitted the form, but their has been a validation error --->
			<SELECT NAME="department_id">
				<CFOUTPUT QUERY="Get_Departments">
					<OPTION VALUE="#strDptID#"<CFIF FORM.DEPARTMENT_ID IS GET_DEPARTMENTS.strDptID> SELECTED</CFIF>>#strDptName#</OPTION>
				</CFOUTPUT>
			</SELECT>	
		</TD>
	</TR>
	<TR>
		<TD></TD>
		<TD>
			<CFOUTPUT>
				<INPUT TYPE="SUBMIT" VALUE="#submittext#">
				<INPUT TYPE="SUBMIT" NAME="cancel" VALUE="Cancel">
				<!--- If we are not adding a new employee, give an option to delete the  employee shown in the form.  Use some javascript to confirm the delete. --->
				<CFIF Len(form.employee_id)>
					<INPUT TYPE="SUBMIT" NAME="delete" VALUE="Delete!" ONCLICK="return confirmThis('Are you sure you want to delete this employee?\nThis action cannot be undone.');">
				</CFIF>
			</CFOUTPUT>		
		</TD>
	</TR>
</TABLE>


</FORM>


<!--- Include the Global Footer --->
<CFINCLUDE TEMPLATE="../CommonFiles/_footer.cfm">
