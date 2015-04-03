<!--- Get object --->
<CFQUERY datasource="cfx" NAME="GetObjectID">
SELECT strObjIDFK FROM tblPInt
WHERE strInsId = '#InstanceID#'
</CFQUERY>

<CFQUERY datasource="cfx" NAME="DeleteInstance">
DELETE FROM tblPInt
WHERE strInsID = '#InstanceID#'
</CFQUERY>

<CFLOCATION URL="properties.cfm?ObjectID=#GetObjectID.strObjIDFK#" ADDTOKEN="NO">