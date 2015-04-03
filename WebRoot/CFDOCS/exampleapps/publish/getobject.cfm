<CFSETTING ENABLECFOUTPUTONLY="YES">

<!--- Get full object data --->


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
	want to access a Title, Teaser, etc.
	
	Once the associative array is built, it is made accessible
	to the calling template so that the object can be rendered
	into HTML. --->

<!--- This query retrieves all content information
	for the current object --->
<CFQUERY datasource="cfx" NAME="GetContent">
	SELECT *
	FROM tblPbCnt, tblPbCTp
	WHERE strTypIDFK = strTypID
		AND strObjIDFK = '#Attributes.ObjectID#'
</CFQUERY>

<!--- This is the struct that will hold our object --->
<CFSET Object = StructNew()>

<!--- Loop over the GetContent query; for each row, insert
	a row into the Content struct, with TypeName as the key
	and Data as the value --->
<CFLOOP QUERY="GetContent">
	<CFSET StructInsert(Object, Trim(strTypName), glbData)>
</CFLOOP>

<!--- The ObjectID should also be part of the struct. --->
<CFSET Object.ObjectID = Attributes.ObjectID>

<!--- Make the object accessible to the calling template. --->
<CFSET Caller.Object = Object>

<!--- The CurrObject struct now contains all the content for the
	current object, so it is easily accessible. Now the only
	thing left to do is render the content to HTML.
	Note that different classes of objects (text, file,
	link) are outputted in different ways; if you were
	to create your own custom object classes, you'd have
	to hard-code how they are rendered, just like the
	classes below. --->
	
<CFSETTING ENABLECFOUTPUTONLY="NO">