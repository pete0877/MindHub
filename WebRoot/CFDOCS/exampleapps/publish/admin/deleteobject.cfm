<!--- Delete instances --->
<CFQUERY datasource="cfx" NAME="KillInstances">
DELETE FROM tblPInt
WHERE strObjIDFK = '#ObjectID#'
</CFQUERY>

<!--- Delete content --->
<CFQUERY datasource="cfx" NAME="KillContent">
DELETE FROM tblPbCnt
WHERE strObjIDFK = '#ObjectID#'
</CFQUERY>

<!--- Delete object --->
<CFQUERY datasource="cfx" NAME="KillObject">
DELETE FROM tblPbObj
WHERE strObjID = '#ObjectID#'
</CFQUERY>

<CFLOCATION URL="getobject.cfm" ADDTOKEN="NO">