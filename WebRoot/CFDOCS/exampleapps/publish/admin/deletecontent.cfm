<!--- Delete content --->

<CFQUERY NAME="GetAll" datasource="cfx">
SELECT * FROM tblPbCnt a, tblPbObj, tblPbClD, tblPbDCl, tblPbCTp
WHERE strClsIDFK=strClsIDF2
	AND strTypIDF2=strTypIDFK
	AND a.strObjIDFK=strObjID
	AND strClsID=strClsIDFK
	AND a.strTypIDFK=strTypID
	AND strCntID='#ContentID#'
</CFQUERY>

<CFIF GetAll.binIsReqd IS TRUE>

	<CFOUTPUT QUERY="GetAll">
		<SCRIPT LANGUAGE="JAVASCRIPT">
		alert('All objects of class #strClsName# must have a #strTypName#.\n\nThis content cannot be deleted.');
		location.replace('properties.cfm?ObjectID=#strObjID#')
		</SCRIPT>
	</CFOUTPUT>

<CFELSE>

	<CFQUERY datasource="cfx" NAME="DeleteContent">
	DELETE FROM tblPbCnt
	WHERE strCntID = '#ContentID#'
	</CFQUERY>
	
	<CFLOCATION URL="properties.cfm?ObjectID=#GetAll.strObjID#" ADDTOKEN="NO">

</CFIF>