<!--- This block of code prepares two lists (stored as Session variables);
	the first stores the TypeID's of the optional content types we're going
	to add, and the second stores the corresponding TypeName's. --->

<CFPARAM NAME="Form.OptContentTypes" DEFAULT="">
<CFIF Form.OptContentTypes NEQ "">
	<CFQUERY datasource="cfx" NAME="GetOptContentTypes">
	SELECT * FROM tblPbClD, tblPbCTp 
	WHERE strTypIDF2 =strTypID
		AND strClsIDF2 = '#Session.AddContent.ClassID#'
		AND binIsReqd=0
		AND strTypID IN (#ListQualify(Form.OptContentTypes,"'")#)
	ORDER BY strClsItmI
	</CFQUERY>
	
	<CFSET Session.AddContent.OptContentTypes = Form.OptContentTypes>
	<CFSET Session.AddContent.OptContentTypeNames = ValueList(GetOptContentTypes.strTypName)>
	<CFSET Session.AddContent.OptContentTypeFile = ValueList(GetOptContentTypes.binTypFile)>
</CFIF>

<CFIF Session.AddContent.ReqContentTypes NEQ "">

	<CFLOCATION URL="addcontent.req.cfm?CurrPos=1" ADDTOKEN="NO">

<CFELSEIF Session.AddContent.OptContentTypes NEQ "">

	<CFLOCATION URL="addcontent.opt.cfm?CurrPos=1" ADDTOKEN="NO">

<CFELSE>
	Nothing to do...
</CFIF>