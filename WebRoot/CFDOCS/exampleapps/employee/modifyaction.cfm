<!--- This template takes info passed from ModifyForm.cfm and updates the database. --->

<CFQUERY datasource="cfx" NAME="ModifyEmployee">
UPDATE tblEmp
SET strFName = '#Form.strFName#',
	strLName = '#Form.strLName#',
	strTitle = '#Form.strTitle#',
	strDptIDFK = '#Form.strDptIDFK#',
	binIsTemp = <CFIF IsDefined("Form.binIsTemp")>1,<CFELSE>0,</CFIF>
	strEmail = '#Form.strEmail#',
	strPhone = '#Form.strPhone#',
	<CFIF Form.dtStrtDate NEQ "">
		dtStrtDate = #CreateODBCDate(Form.dtStrtDate)#,
	</CFIF>
	glbPersnl = '#Form.glbPersnl#'
WHERE strEmpID = '#Form.strEmpID#'
</CFQUERY>

<CFLOCATION URL="details.cfm?EmpID=#Form.strEmpID#">
