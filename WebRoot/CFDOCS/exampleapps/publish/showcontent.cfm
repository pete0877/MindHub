<CFSETTING ENABLECFOUTPUTONLY="YES">

<!---

	This template should be called as a custom tag.  It finds
	the PageID, and takes Location as a parameter, and finds
	the objects	that are scheduled to appear at execution time.
	
	Each location on a page where dynamic content is to appear
	should have a call to CF_ShowContent.
	
--->

<!--- First we figure out what the page ID for
	the calling template is --->
<CFQUERY datasource="cfx" NAME="GetPageID">
SELECT * FROM tblPubPg
WHERE strTmpPth = '#GetFileFromPath(GetTemplatePath())#'
</CFQUERY>

<CFIF GetPageID.RecordCount IS 0>
	<CFSETTING ENABLECFOUTPUTONLY="NO">
	<CFOUTPUT><!-- (CF_ShowContent) ERROR: PageID not found!! --></CFOUTPUT>
	<CFEXIT>
</CFIF>

<!--- If location is not specified, default to 1 --->
<CFPARAM NAME="Attributes.Location" DEFAULT="1">

<!--- Then we figure out which objects should appear
	on this page, at this location, at this time --->
<CFPARAM NAME="URL.CurrentTime" DEFAULT="#Now()#">
<CFQUERY datasource="cfx" NAME="GetInstances">
SELECT *
FROM tblPbObj, tblPInt, tblPbDCl

WHERE strObjID = strObjIDFK
	AND strClsIDFK = strClsID
	AND strPgIDFK = '#GetPageID.strPgID#'
	AND (dtStrtTm < #CreateODBCDateTime(URL.CurrentTime)# OR dtStrtTm IS Null)
	AND (dtEndTm > #CreateODBCDateTime(URL.CurrentTime)# OR dtEndTm IS Null)
	AND intLoc = #Attributes.Location#
ORDER BY intPrior DESC
</CFQUERY>

<!--- Now loop over each instance (an instance is
		one call to one object) --->

<CFLOOP QUERY="GetInstances">

	<!--- Pour this object's data into an associative array.

		For example, Object 132 might have the following content:
		
		ObjectID   Type    Data
		---------  ------  -----------
		132		   Title   "This Story's Title"
		132	       Teaser  "Read this interesting article"
		132        Body    "The main story would go here."

		This is in query form.  The corresponding associative
		array would look like this:
		
		Key     Value
		------  -----------
		Title   "This Story's Title"
		Teaser  "Read this interesting article"
		Body    "The main story would go here."
		
		This enables us to access the different content
		by referring to StructName.Title, StructName.Teaser,
		etc. rather than doing a new query each time we 
		want to access a Title, Teaser, etc. --->

	<!--- This query retrieves all content information
		for the current object --->
	<CFQUERY datasource="cfx" NAME="GetContent">
	SELECT *
	FROM tblPbCnt, tblPbCTp
	WHERE strTypIDFK = strTypID
		AND strObjIDFK = '#GetInstances.strObjIDFK#'
	</CFQUERY>

	<!--- This is the struct that will hold our object --->
	<CFSET CurrObject = StructNew()>

	<!--- Loop over the GetContent query; for each row, insert
		a row into the CurrObject struct, with TypeName as the key
		and Data as the value --->
	<CFLOOP QUERY="GetContent">
		<CFSET StructInsert(CurrObject, Trim(strTypName), glbData)>
	</CFLOOP>
	<!--- Two pieces of information should also be in the CurrObject 
		struct: ClassName and ObjectID. Since there was no convenient
		way to work that into the CFLOOP we just did, we add them
		manually. --->
	<CFSET CurrObject.ClassName = Trim(strClsName)>
	<CFSET CurrObject.ObjectID = strObjIDFK>
	<!--- The CurrObject struct now contains all the content for the
		current object, so it is easily accessible. Now the only
		thing left to do is render the content to HTML.
		
		Since different situations might call for objects to be
		rendered differently, the CFML that will actually render
		the content is not hardcoded into ShowContent.cfm but
		rather stored in "handler" files in the viewmode/ subdirectory.
		
		This example app only includes viewmode/default.cfm but you
		can easily create your own by examining that file. --->

	<CFPARAM NAME="Attributes.ViewMode" DEFAULT="default">
	<CFINCLUDE TEMPLATE="viewmode/#Attributes.ViewMode#.cfm">
	
</CFLOOP>

<CFSETTING ENABLECFOUTPUTONLY="NO">
