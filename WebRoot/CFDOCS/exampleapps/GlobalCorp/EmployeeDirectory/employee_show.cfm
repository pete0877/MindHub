<!--- ColdFusion(r) Express Global Corp. Example Application --->

<!--- For database security and error check, make sure the employee_id exists and is a string --->
<CFPARAM NAME="url.employee_id" DEFAULT="" TYPE="string">

<!--- Make sure the url parameter is a real value --->
<CFIF NOT Len(Trim(url.employee_id))>
	<CFLOCATION URL="index.cfm">
</CFIF>

<!--- Get the employees --->
<CFQUERY NAME="get_employee" DATASOURCE="#application.ds#">
SELECT 	strFname,
		strLname,
		strPasswd,
		strEmail,
		strAddress,
		strState,
		strCity,
		intZipCode,
		strPhone,
		strImagLoc,
		strDptName
FROM	tblEmp, 
		tblDpt
WHERE	strEmpID = '#url.employee_id#'
AND		strDptID = idDptIDFK

</CFQUERY>


<!--- Include the Global Header --->
<CFSET header.page_name="Employee Information">
<CFINCLUDE TEMPLATE="../CommonFiles/_header.cfm">

<!--- The Address Book Menu --->
<FONT SIZE="2">
{
<A HREF="index.cfm">Employee Search</A>
 | 
<A HREF="employee_choose.cfm">Edit an Employee</A>
 | 
<A HREF="employee_addedit.cfm">Add an Employee</A>
 | 
<A HREF="index.cfm?ShowAllEmployees=1">Show All Employees</A>
}
</FONT>

<P>

<!--- If no employee was found in the database --->
<CFIF NOT get_employee.recordcount>

	<B>I'm sorry, there has been an error.  I could not find that employee.</B>

<!--- Output information about the employee --->
<CFELSE>
	
	<CFOUTPUT QUERY="get_employee">
	<!--- If the employee has an image, show it --->
	<CFIF len(trim(strImagLoc))>
		<IMG SRC="../#application.employee_image_path##strImagLoc#" ALIGN="left" HSPACE="10">
 	</CFIF>
	<B>Name:</B> #strFname# #strLname#
	<BR>
	<B>Department:</B> #strDptName#
	<BR>
	<B>Email:</B> <A HREF="mailto:#strEmail#">#strEmail#</A>
	<BR>
	<B>Phone:</B> #strPhone#
	<BR>
	
	<P>
	#strAddress#<BR>
	#strCity#, #strState# #intZipCode#<P>
	
	
	</CFOUTPUT>
	
</CFIF>


<P>

<!--- Include the Global Footer --->
<CFINCLUDE TEMPLATE="../CommonFiles/_footer.cfm">
