<!--- This INSERT statement does the actual adding
	of the data to the database; notice that the
	SQL statement contains Cold Fusion #variables# --->
<CFQUERY datasource="cfx" NAME="InsertEmployee">
INSERT INTO tblEmp (
    strEmpId,
	strFName,
	strLName,
	strTitle,
	strDptIDFK,
	binIsTemp,
	strEmail,
	<CFIF Form.dtStrtDate NEQ "">dtStrtDate,</CFIF>
	strPhone,
	glbPersnl)
VALUES ('#CreateUUID()#',
    '#Form.strFName#',
	'#Form.strLName#',
	'#Form.strTitle#',
	'#Form.strDptIDFK#', 
	<!--- "Temporary" was an input of type
		CHECKBOX, which is either TRUE or
		does not exist at all --->
	<CFIF IsDefined("Form.binIsTemp")>
		1,
	<CFELSE>
		0,
	</CFIF>
	'#Form.strEmail#',
	<CFIF Form.dtStrtDate NEQ "">#CreateODBCDate(Form.dtStrtDate)#,</CFIF>
	'#Form.strPhone#',
	'#Form.glbPersnl#')
</CFQUERY>

<!--- Go back to the Add Employee page --->
<CFLOCATION URL="insertform.cfm">
