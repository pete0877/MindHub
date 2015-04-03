<CFPARAM NAME="Session.AddContent.ObjectID" DEFAULT="">

<CFTRANSACTION ISOLATION="READ_COMMITTED">

<CFIF Session.AddContent.ObjectID IS "">

	<CFSET CurrentDateTime = Now()>

	<CFSET NewObjectID = CreateUUID()>
		
	<CFQUERY datasource="cfx" NAME="CreateNewObject">
	INSERT INTO tblPbObj (strObjID, strClsIDFK, dtCreated, dtUpdated)
	VALUES ('#NewObjectID#', '#Session.AddContent.ClassID#', #CurrentDateTime#, #CurrentDateTime#)
	</CFQUERY>
		
	<CFSET ObjectID = NewObjectID>

<CFELSE>

	<CFSET ObjectID = Session.AddContent.ObjectID>

</CFIF>

<CFLOOP LIST="#Session.AddContent.ReqContentTypes#" INDEX="CurrType">

	<CFQUERY datasource="cfx" NAME="KillExisting">
	DELETE FROM tblPbCnt
	WHERE strObjIDFK = '#ObjectID#'
		AND strTypIDFK = '#CurrType#'
	</CFQUERY>
	
	<CFSET NewContentID = CreateUUID()>

	<!--- "Pay no attention to the man behind the curtain." --->
	<CFSET DataVar = evaluate("Session.AddContent.Content.Type" & Replace(CurrType,"-","_","ALL"))>

	<CFQUERY datasource="cfx" NAME="InsertNew">
	INSERT INTO tblPbCnt (strCntID, strObjIDFK, strTypIDFK, glbData)
	VALUES ('#NewContentID#', '#ObjectID#', '#CurrType#', '#DataVar#')
	</CFQUERY>

	<CFSET "Session.AddContent.Content.Type#Replace(CurrType,"-","_","ALL")#" = "">

</CFLOOP>

<CFLOOP LIST="#Session.AddContent.OptContentTypes#" INDEX="CurrType">

	<CFQUERY datasource="cfx" NAME="KillExisting">
	DELETE FROM tblPbCnt
	WHERE strObjIDFK = '#ObjectID#'
		AND strTypIDFK = '#CurrType#'
	</CFQUERY>
	
	<CFSET NewContentID = CreateUUID()>

	<CFQUERY datasource="cfx" NAME="InsertNew">
	INSERT INTO tblPbCnt (strCntID, strObjIDFK, strTypIDFK, glbData)
	VALUES ('#NewContentID#', '#ObjectID#', '#CurrType#', '#evaluate("Session.AddContent.Content.Type" & Replace(CurrType,"-","_","ALL"))#')
	</CFQUERY>

	<CFSET "Session.AddContent.Content.Type#Replace(CurrType,"-","_","ALL")#" = "">

</CFLOOP>

</CFTRANSACTION>

<CFLOCATION URL="properties.cfm?ObjectID=#ObjectID#" ADDTOKEN="NO">