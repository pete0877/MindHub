<!--- ColdFusion(r) Express Global Corp. Example Application --->

<!--- Include the Global Header --->
<CFSET header.page_name="Find Employees">
<CFINCLUDE TEMPLATE="../CommonFiles/_header.cfm">

<!--- Param the search parameter so that we can fill the search field with the same criteria after the submit button is pressed --->
<CFPARAM NAME="search_parameter" DEFAULT="">

<!--- By default, don't show all employees --->
<CFPARAM NAME="ShowAllEmployees" DEFAULT="0">

<!--- A check to make sure the ShowAllEmployees variable is Boolean (to prevent people from tampering with the URL and throwing errors).  If it is not, kick the user back to the page --->
<CFIF NOT IsBoolean(ShowAllEmployees)>
	<CFLOCATION URL="index.cfm">
</CFIF>

<!--- The menu links --->
<FONT SIZE="2">
<!--- Link to add employees --->
{ <A HREF="employee_addedit.cfm">Add an Employee</A>
 |  
<!--- Link to edit employees --->
<A HREF="employee_choose.cfm">Edit an Employee</A>
 | 
<!--- If we are currently showing all employees, show a link to hide them --->
<CFIF ShowAllEmployees>
	<A HREF="index.cfm">Hide All Employees</A>
<!--- If we are not currently showing all employees, show a link to show all employees --->
<CFELSE>
	<A HREF="index.cfm?ShowAllEmployees=1">Show All Employees</A>
</CFIF>
 }
</FONT>

<!--- Paint the search form, pass "gofindemployees" to indicate to this page that a search should be executed against the employee table.  This form submits to itself --->
<CFOUTPUT>
<FORM ACTION="#getfilefrompath(getbasetemplatepath())#" METHOD="post">

<B>Enter employee first name or last name:</B><BR>
<INPUT TYPE="text" NAME="search_parameter" VALUE="#search_parameter#">
<INPUT TYPE="hidden" NAME="GoFindEmployees" VALUE="yes">
<INPUT TYPE="submit" NAME="search" VALUE="Search for an Employee">
</FORM>
</CFOUTPUT>

<!--- If the form has been submitted or the user showing all employes --->
<CFIF IsDefined("GoFindEmployees") OR ShowAllEmployees>
	<!--- If a search parameter exists from the form, make sure it is not blank --->
	<CFIF IsDefined("form.search_parameter") AND NOT len(trim(search_parameter))>
		<B>Please enter a name or email address to search for!</B><P>
	<CFELSE>
		<CFQUERY NAME="Search" DATASOURCE="#application.ds#">
		SELECT 	strFname,
				strLname,
				strEmpID,
				strEmail,
				strDptName
		FROM    tblEmp, 
				tblDpt
		WHERE	idDptIDFK = strDptID
		<!--- Add search criteria if the search_parameter field exists (checks against firstname, lastname and email address) --->
		<CFIF IsDefined("form.search_parameter")>
			<CFIF Server.OS.Name CONTAINS "Windows">
				AND		
				(
				strLName LIKE '%#search_parameter#%'
				OR
				strFName LIKE '%#search_parameter#%'
				OR
				strEmail LIKE '%#search_parameter#%'
				)
			<CFELSE>
				AND 
				(
				Upper(strFName) LIKE '%#UCase(search_parameter)#%'
				OR 
				Upper(strLName) LIKE '%#UCase(search_parameter)#%'
				OR 
				Upper(strEmail) LIKE '%#UCase(search_parameter)#%'
				)
			</CFIF>
		</CFIF>
		ORDER BY strLName
		</CFQUERY>
			<!---- indicate that no employees matched the search --->		
			<CFIF NOT search.recordcount>
				<B>Sorry, your search returned no records</B>
				<P>
			<!---- display the search results --->
			<CFELSE>
				<B>Your Search Returned <CFOUTPUT>#search.recordcount# result<CFIF search.recordcount IS NOT 1>s</CFIF>:</CFOUTPUT></B>
				<UL>
				<CFOUTPUT QUERY="Search">
				<LI><A HREF="employee_show.cfm?employee_id=#strEmpID#">#strFname# #strLname#</A> - #strDptName#</LI>
				</CFOUTPUT>
				</UL>
			</CFIF>			
	</CFIF>
</CFIF>


<!--- Include the Global Footer --->
<CFINCLUDE TEMPLATE="../CommonFiles/_footer.cfm">