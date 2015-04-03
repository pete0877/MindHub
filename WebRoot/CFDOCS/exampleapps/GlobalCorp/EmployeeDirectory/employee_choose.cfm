<!--- Include the Global Header --->
<CFSET header.page_name="Choose an Employee to Edit">
<CFINCLUDE TEMPLATE="../CommonFiles/_header.cfm">

<!--- Get all employees --->
<CFQUERY NAME="Get_Employee" DATASOURCE="#application.ds#">
SELECT 	 strEmpID,strFname,strLname
FROM 	 tblEmp
ORDER BY strLname

</CFQUERY>


<!--- The Address Book Menu --->
<FONT SIZE="2">
{
<A HREF="index.cfm">Employee Search</A>
 | 
<A HREF="employee_addedit.cfm">Add an Employee</A>
 | 
<A HREF="index.cfm?ShowAllEmployees=1">Show All Employees</A>
}
</FONT>


<!--- Create a pulldown to choose an employee --->
<FORM ACTION="employee_addedit.cfm" METHOD="post">

	<SELECT NAME="employee_id">
		<CFOUTPUT QUERY="Get_Employee">
			<!--- The function HTMLEditFormat() escapes and HTML characters that may create problems inside the form element.  It is a good idea to use it when populating HTML Forms dynamically. --->
			<OPTION VALUE="#strEmpID#">#HTMLEditFormat(strFname & " " & strLname)#</OPTION>
		</CFOUTPUT>
	</SELECT>

	<INPUT TYPE="hidden" NAME="action" VALUE="prepare_edit">
	<INPUT TYPE="submit" NAME="submit" VALUE="Edit Employee">	
	
</FORM>


<!--- Include the Global Footer --->
<CFINCLUDE TEMPLATE="../CommonFiles/_footer.cfm">	